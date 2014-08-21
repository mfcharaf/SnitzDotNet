﻿/*
####################################################################################################################
##
## SnitzUI.Content.Forums - Post.aspx
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using AjaxControlToolkit;
using ModConfig;
using Snitz.BLL;
using Snitz.BLL.modconfig;
using Snitz.Entities;
using SnitzUI.MasterTemplates;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI
{
    public partial class PostPage : PageBase
    {
        private string _action;
        private string _type;
        private int _recId;
        private ForumInfo _forum;
        private TopicInfo _thisTopic;
        private bool _topicLocked;
        private bool _inModeratedList;
        bool pingSiteMap = Config.PingSearchEngine;
        public string AllowedFileTypes { get; set; }
        public bool AllowAttachments { get; set; }
        public bool AllowImageUploads { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var master = (SingleCol)Master;
            if (master != null) master.rootScriptManager.EnablePageMethods = true;

            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");

            ForumDropDown.DataSource = Forums.AllowedForumsList(Member);
            ForumDropDown.DataBind();
            

            ModConfigBase controller = (ModConfigBase)ConfigHelper.ModClass("UploadConfig");
            AllowedFileTypes = "";
            AllowAttachments = Convert.ToBoolean(Convert.ToInt16(controller.ModConfiguration.Settings["AllowAttachments"]));
            AllowImageUploads = Convert.ToBoolean(Convert.ToInt16(controller.ModConfiguration.Settings["AllowImageUpload"]));
            if (AllowImageUploads)
                AllowedFileTypes += controller.ModConfiguration.Settings["AllowedImageTypes"].ToString();
            if (AllowAttachments)
            {
                if (AllowedFileTypes != "")
                    AllowedFileTypes += ",";
                AllowedFileTypes += controller.ModConfiguration.Settings["AllowedAttachmentTypes"].ToString();
            }
            if (AllowImageUploads || AllowAttachments)
            {
                string style = ""; //!ShowAttachments ? ".upload{display:none;}" : 
                if (!Config.UserGallery || !AllowImageUploads)
                    style += ".browse{display:none;}";

                uploadStyle.Text = !String.IsNullOrEmpty(style) ? string.Format("<style>{0}</style>", style) : "";
            }
            else
            {
                uploadStyle.Text = "<style>.upload{display:none;} .browse{display:none;}</style>";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["method"] == null)
                return;
            if(!IsAuthenticated)
                throw new HttpException(403, "You must be logged in to post messages");

            InitialiseVariablesFromParams();
            
            if ((!Page.IsPostBack))
            {
                if (_action != "edit")
                {
                    //do flood check
                    if (Session["LastPostMade"] != null)
                    {
                        if ((Config.FloodCheck) && !(IsAdministrator || IsModerator))
                        {
                            var diff1 = new TimeSpan(0, 0, Config.FloodTimeout);
                            DateTime? lastpost = Session["LastPostMade"].ToString().ToDateTime();
                            DateTime dt = DateTime.UtcNow - diff1;
                            if (lastpost > dt)
                                throw new HttpException(403, "FloodCheck");

                        }
                    }
                }

                SetupPageControls();

            }
            AsyncFileUpload1.UploaderStyle = AsyncFileUpload.UploaderStyleEnum.Modern;
            AsyncFileUpload1.UploadedComplete += AsyncFileUpload1UploadedComplete;
            AsyncFileUpload1.UploadedFileError += AsyncFileUpload1UploadedFileError;


        }

        private void SetupPageControls()
        {
            cbxSig.Checked = Member.UseSignature;
            cbxLock.Checked = _topicLocked;
            if (_thisTopic != null)
                cbxSticky.Checked = _thisTopic.IsSticky;

            switch (_action)
            {
                case "topic":
                    SubjectDiv.Visible = true;
                    ForumDiv.Visible = (IsAdministrator || (_inModeratedList && !Config.RestrictModeratorMove));

                    ForumDropDown.SelectedValue = ForumId.ToString();
                    cbxSticky.Visible = (IsAdministrator || _inModeratedList);
                    cbxLock.Visible = (IsAdministrator || _inModeratedList);
                    if (ForumId.HasValue)
                    {
                        if (_forum.Type == 3 && String.IsNullOrEmpty(Message.Text))
                        {
                            var file = new StreamReader(Server.MapPath(Config.CultureSpecificDataDirectory + "bugtemplate.txt"));
                            string content = file.ReadToEnd();
                            Message.Text = content;
                            file.Close();
                            file.Dispose();
                        }                        
                    }
                    break;
                case "reply":
                    SubjectDiv.Visible = false;
                    cbxSticky.Visible = false;
                    cbxLock.Visible = (IsAdministrator || _inModeratedList);
                    ForumDiv.Visible = false;
                    break;
                case "quote":
                    SubjectDiv.Visible = false;
                    cbxSticky.Visible = false;
                    ForumDiv.Visible = false;
                    cbxLock.Visible = (IsAdministrator || _inModeratedList);
                    Message.Text = GetQuotedMessage();
                    break;
                case "edit":
                    switch (_type)
                    {
                        case "topics":
                            cbxSticky.Visible = (IsAdministrator || _inModeratedList);
                            cbxLock.Visible = (IsAdministrator || _inModeratedList);
                            SetupForEditMessage();
                            break;
                        case "reply":
                            cbxSticky.Visible = false;
                            cbxLock.Visible = (IsAdministrator || _inModeratedList);
                            SetupForEditMessage();
                            break;
                        case "forum":
                            if (Request.UrlReferrer != null) Response.Redirect(Request.UrlReferrer.AbsoluteUri);
                            break;
                    }

                    break;
                default:
                    if (Request.UrlReferrer != null) Response.Redirect(Request.UrlReferrer.AbsoluteUri);
                    break;
            }
        }

        private void InitialiseVariablesFromParams()
        {
            _action = Request.Params["method"].ToLower();

            if (Request.Params["id"] != null)
                _recId = Int32.Parse(Request.Params["id"]);
            if (Request.Params["type"] != null)
                _type = Request.Params["type"].ToLower();

            if (ForumId != null)
            {
                _inModeratedList = Moderators.IsUserForumModerator(User.Identity.Name, ForumId.Value);
                _forum = Forums.GetForum(ForumId.Value);
                if (_forum.Type == 3 && String.IsNullOrEmpty(Message.Text) && _action == "topic")
                {
                        var file = new StreamReader(Server.MapPath(Config.CultureSpecificDataDirectory + "bugtemplate.txt"));
                        Message.Text = file.ReadToEnd();
                        file.Close();
                        file.Dispose();
                }
            }
            if (TopicId != null)
            {
                _thisTopic = Topics.GetTopic(TopicId.Value);
                if (IsModerator)
                    _inModeratedList = Moderators.IsUserForumModerator(User.Identity.Name, _thisTopic.ForumId);
                _topicLocked = _thisTopic.Status == (int)Enumerators.PostStatus.Closed;
            }
            else if (_type == "topics")
            {
                TopicId = Int32.Parse(Request.Params["id"]);
                _thisTopic = Topics.GetTopic(TopicId.Value);
                if (IsModerator)
                {
                    _inModeratedList = Moderators.IsUserForumModerator(User.Identity.Name, _thisTopic.ForumId);
                }
                _topicLocked = _thisTopic.Status == (int)Enumerators.PostStatus.Closed;
            }

            switch (_action)
            {
                case "topic":
                    Page.Title = Config.ForumTitle + ": "  + Resources.webResources.lblNewTopic;
                    break;
                case "reply":
                case "quote":
                    Page.Title = Config.ForumTitle + ": " + Resources.webResources.lblReplyToTopic;
                    break;
                case "edit":
                    switch (_type)
                    {
                        case "reply":
                            Page.Title = Config.ForumTitle + ": " + Resources.webResources.lblEditReply;
                            break;
                        case "topics":
                            Page.Title = Config.ForumTitle + ": " + Resources.webResources.lblEditTopic;
                            break;
                    }
                    break;
            }
        }

        private string GetQuotedMessage()
        {
            switch (_type)
            {
                case "reply" :
                    ReplyInfo reply = Replies.GetReply(_recId);
                    return String.Format("[quote=\"{0}\"]{1}[/quote]", reply.AuthorName, reply.Message);
                case "topics" :
                    return String.Format("[quote=\"{0}\"]{1}[/quote]", _thisTopic.AuthorName, _thisTopic.Message);
            }
            return String.Empty;
        }

        private void SetupForEditMessage()
        {

            string msg = String.Empty;
            //fetch original message from db
            switch (_type)
            {
                case "reply":
                    ReplyInfo reply = Replies.GetReply(_recId);
                    SubjectDiv.Visible = false;
                    ForumDiv.Visible = false;
                    msg = HttpUtility.HtmlDecode(reply.Message);
                    break;
                case "topics":
                    ForumDiv.Visible = (IsAdministrator || (_inModeratedList && !Config.RestrictModeratorMove));

                    ForumDropDown.SelectedValue = _thisTopic.ForumId.ToString();
                    SubjectDiv.Visible = true;
                    tbxSubject.Text = HttpUtility.HtmlDecode(_thisTopic.Subject);
                    string poll = "";
                    if(_thisTopic.PollId != null)
                    {
                        poll = Polls.GetTopicPollString(_thisTopic.PollId);
                    }
                    msg = poll + HttpUtility.HtmlDecode(_thisTopic.Message);
                    break;
            }

            Message.Text = msg; 
        }
        
        protected void PostMessage(object sender, EventArgs e)
        {
            btnSubmit.Enabled = false;
           
            switch (_action.ToLower())
            {
                case "topic":
                    Session.Add("LastPostMade", DateTime.UtcNow.ToForumDateStr());
                    PostNewTopic();
                    break;
                case "reply":
                case "quote":
                    Session.Add("LastPostMade", DateTime.UtcNow.ToForumDateStr());
                    PostNewReply();
                    break;
                case "edit":
                    switch (_type)
                    {
                        case "reply" :
                            EditReply();
                            break;
                        case "topics":
                            EditTopic();
                            break;
                    }
                    break;
            }

        }

        private void EditReply()
        {
            Replies.UpdateReply(_recId, Message.Text, Member, IsAdministrator, cbxSig.Checked);

            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                Topics.SetTopicStatus(_thisTopic.Id, (int) Enumerators.PostStatus.Closed);
            if (_inModeratedList || IsAdministrator)
                Topics.MakeSticky(_thisTopic.Id, cbxSticky.Checked);
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + TopicId + "&whichpage=-1#" + _recId);
        }

        private void EditTopic()
        {
            #region check for changes to poll

            var pollregex = new Regex(@"(?<poll>\[poll=\x22(?<question>.+?)\x22](?<answers>.+?)\[\/poll])",
                RegexOptions.Singleline);

            if (pollregex.IsMatch(Message.Text))
            {
                string topicPoll = pollregex.Match(Message.Text).Value;
                if (topicPoll == "" || topicPoll == "remove")
                {
                    if (_thisTopic.PollId.HasValue) Polls.DeleteTopicPoll(_thisTopic.PollId.Value);
                }
                else if (_thisTopic.Forum.AllowPolls)
                {
                    var answers = new Regex(@"\[\*=(?<sort>[0-9]+)](?<answer>.+?)\[/\*]",
                        RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                    string question = "";
                    var choices = new SortedList<int, string>();

                    MatchCollection mc = pollregex.Matches(topicPoll);
                    if (mc.Count > 0)
                    {
                        foreach (Match m in mc)
                        {
                            question = m.Groups["question"].Value;
                            string answer = m.Groups["answers"].Value;

                            MatchCollection ans = answers.Matches(answer);
                            foreach (Match match in ans)
                            {
                                choices.Add(Convert.ToInt32(match.Groups["sort"].Value), match.Groups["answer"].Value);
                            }
                        }
                        if (_thisTopic.PollId.HasValue)
                            Polls.UpdateTopicPoll(_thisTopic.PollId.Value, question, choices);
                    }
                }
                Message.Text = pollregex.Replace(Message.Text, "");
            }

            #endregion

            int oldforumId = _thisTopic.ForumId;
            Topics.Update(_thisTopic.Id, Message.Text, tbxSubject.Text, Member, IsAdministrator, cbxSig.Checked);
            if (ForumDropDown.SelectedValue != oldforumId.ToString() && ForumDiv.Visible)
            {
                //move the topic
                int forumid = Convert.ToInt32(ForumDropDown.SelectedValue);
                Topics.ChangeTopicForum(_thisTopic.Id, forumid);
                Snitz.BLL.Admin.UpdateForumCounts();
                object obj = -1;
                Cache["RefreshKey"] = obj;
                _thisTopic.Author = Members.GetAuthor(_thisTopic.AuthorId);

                if (Config.MoveNotify && _thisTopic.Author.Status != 0)
                {
                    _forum = Forums.GetForum(forumid);
                    string mailFile = ConfigurationManager.AppSettings["TopicMoveEmail"];
                    string strSubject = "Sent From " + Regex.Replace(Config.ForumTitle, @"&\w+;", "") + ": Topic move notification";

                    var builder = new UriBuilder("http",
                        Request.Url.DnsSafeHost,
                        Request.Url.Port, Page.ResolveUrl("~/Content/Forums/forum.aspx"), string.Format("?FORUM={0}", _forum.Id));

                    var file = new StreamReader(mailFile);
                    string msgBody = file.ReadToEnd();
                    msgBody = msgBody.Replace("<%UserName%>", _thisTopic.AuthorName);
                    msgBody = msgBody.Replace("<%ForumUrl%>", Config.ForumTitle);
                    msgBody = msgBody.Replace("<%TopicSubject%>", _thisTopic.Subject);
                    msgBody = msgBody.Replace("<%MovedTo%>", _forum.Subject);
                    msgBody = msgBody.Replace("<%URL%>", builder.Uri.AbsoluteUri);

                    var mailsender = new SnitzEmail
                    {
                        toUser = new MailAddress(_thisTopic.Author.Email, _thisTopic.AuthorName),
                        FromUser = "Forum Administrator",
                        subject = strSubject,
                        IsHtml = true,
                        msgBody = msgBody
                    };
                    mailsender.Send();
                }
            }
            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                Topics.SetTopicStatus(_thisTopic.Id, (int) Enumerators.PostStatus.Closed);
            if (_inModeratedList || IsAdministrator)
                Topics.MakeSticky(_thisTopic.Id, cbxSticky.Checked);

            if (pingSiteMap)
            {
                Ping("");
            }
            InvalidateForumCache();
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + _thisTopic.Id);
        }

        private void PostNewTopic()
        {
            string ipaddress = Common.GetIP4Address();
            #region Poll check code
            string topicPoll = String.Empty;

            var poll = new Regex(@"(?<poll>\[poll=\x22(?<question>.+?)\x22](?<answers>.+?)\[\/poll])", RegexOptions.Singleline);

            if (poll.IsMatch(Message.Text))
            {
                //there are poll tags, so store them and remove from the message text
                topicPoll = poll.Match(Message.Text).Value;
                Message.Text = poll.Replace(Message.Text, "");
            }
            #endregion
            var topic = new TopicInfo
                              {
                                  Subject = tbxSubject.Text,
                                  Message = Message.Text,
                                  Date = DateTime.UtcNow,
                                  UseSignatures = cbxSig.Checked,
                                  IsSticky = cbxSticky.Checked,
                                  PosterIp = ipaddress,
                                  Status = (int)Enumerators.PostStatus.Open,
                                  UnModeratedReplies = 0,
                                  AuthorId = Member.Id,
                                  ReplyCount = 0,
                                  Views = 0
                              };
            if (ForumId.HasValue)
            {
                topic.Forum = Forums.GetForum(ForumId.Value);
                topic.ForumId = ForumId.Value;
                
            }
            if (cbxLock.Checked)
                topic.Status = (int)Enumerators.PostStatus.Closed;
            else if (!_inModeratedList)
            {
                if (topic.Forum.ModerationLevel == (int)Enumerators.Moderation.AllPosts ||
                    topic.Forum.ModerationLevel == (int)Enumerators.Moderation.Topics)
                    topic.Status = (int)Enumerators.PostStatus.UnModerated;
            }
            if (CatId != null) topic.CatId = CatId.Value;
            topic.Id = Topics.Add(topic);
            if (pingSiteMap) { Ping(""); }

            if (topicPoll != String.Empty && topic.Forum.AllowPolls)
                CreatePoll(topicPoll, topic.Id);
            InvalidateForumCache();
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + topic.Id);
        }

        private static void CreatePoll(string topicPoll, int topicid)
        {
            var poll = new Regex(@"(?<poll>\[poll=\x22(?<question>.+?)\x22](?<answers>.+?)\[\/poll])", RegexOptions.Singleline);

            var answers = new Regex(@"\[\*=(?<sort>[0-9]+)](?<answer>.+?)\[/\*]", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            string question = "";
            var s = new SortedList<int, string>();

            MatchCollection mc = poll.Matches(topicPoll);
            if (mc.Count > 0)
            {
                foreach (Match m in mc)
                {
                    question = m.Groups["question"].Value;
                    string answer = m.Groups["answers"].Value;
                    MatchCollection ans = answers.Matches(answer);
                    foreach (Match match in ans)
                    {
                        s.Add(Convert.ToInt32(match.Groups["sort"].Value), match.Groups["answer"].Value);
                    }
                }

                Polls.AddTopicPoll(topicid, question, s);
            }
        }

        private void PostNewReply()
        {
            string ipaddress = Common.GetIP4Address();

            var poll = new Regex(@"(?<poll>\[poll=(?<question>.+?)](?<answers>.+?)\[\/poll])", RegexOptions.Singleline);

            if (poll.IsMatch(Message.Text))
            {
                //Polls not allowed in replies
                Message.Text = poll.Replace(Message.Text, "");
            }

            var reply = new ReplyInfo
                              {
                                  Message = Message.Text,
                                  PosterIp = ipaddress,
                                  Status = (int)Enumerators.PostStatus.Open,
                                  UseSignatures = cbxSig.Checked,
                                  AuthorId = Member.Id,
                                  Date = DateTime.UtcNow
                              };
            if (!_inModeratedList)
            {

                if (_thisTopic.Forum.ModerationLevel == (int)Enumerators.Moderation.AllPosts ||
                    _thisTopic.Forum.ModerationLevel == (int)Enumerators.Moderation.Replies )
                {
                    reply.Status = (int)Enumerators.PostStatus.UnModerated;
                    _thisTopic.UnModeratedReplies += 1;
                }
            }

            reply.TopicId = _thisTopic.Id;
            reply.CatId = _thisTopic.CatId;
            reply.ForumId = _thisTopic.ForumId;
            reply.Id = Replies.AddReply(reply);

            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                _thisTopic.Status = (int)Enumerators.PostStatus.Closed;
            Topics.Update(_thisTopic);
            if (pingSiteMap) { Ping(""); }
            InvalidateForumCache();
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + TopicId + "&whichpage=-1#" + reply.Id);
        }

        private void InvalidateForumCache()
        {
            object obj = -1;
            Cache["RefreshKey"] = obj;
        }

        protected override SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
        {
            if (SiteMap.CurrentNode == null)
                return null;
            SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);

            SiteMapNode tempNode = currentNode;
            
            switch (_action)
            {
                case "topic":
                    tempNode.Title = Resources.webResources.lblNewTopic;
                    tempNode = tempNode.ParentNode;
                    if (ForumId != null)
                    {
                        tempNode.Title = _forum.Subject;
                        tempNode.Url = "~/Content/Forums/forum.aspx?FORUM=" + ForumId;
                    }
                    break;
                case "reply":
                case "quote":
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    tempNode.Title = _thisTopic.Subject +  " (" + textInfo.ToTitleCase(_action) + ")";
                    tempNode = tempNode.ParentNode;
                    tempNode.Title = string.Format("{0}", _thisTopic.Forum.Subject);
                    tempNode.Url = "~/Content/Forums/topic.aspx?TOPIC=" + _thisTopic.Id;
                    break;
                case "edit":
                    switch (_type)
                    {
                        case "reply":
                            tempNode.Title = Resources.webResources.lblEditReply;
                            break;
                        case "topics":
                            tempNode.Title = Resources.webResources.lblEditTopic + " " + _thisTopic.Subject;
                            break;
                    }
                    tempNode = tempNode.ParentNode;
                    tempNode.Title = string.Format("{0}", _thisTopic.Forum.Subject);
                    tempNode.Url = "~/Content/Forums/topic.aspx?TOPIC=" + _thisTopic.Id;
                    break;
            }

            return currentNode; 
        }

        protected void BtnCancelClick(object sender, EventArgs eventArgs)
        {
            if(TopicId == null)
                Response.Redirect("/Content/Forums/forum.aspx?FORUM=" + ForumId);
            else
                Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + TopicId + "&whichpage=-1");
        }

        #region async file upload

        private void AsyncFileUpload1UploadedFileError(object sender, AsyncFileUploadEventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "error", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';});", true);
        }

        private void AsyncFileUpload1UploadedComplete(object sender, AsyncFileUploadEventArgs e)
        {
            ProcessUpload(e);
        }

        private void ProcessUpload(AsyncFileUploadEventArgs e)
        {
            if (e.FileName == null) return;

            ModConfigBase controller = (ModConfigBase)ConfigHelper.ModClass("UploadConfig");
            string types = "";
            if (AllowAttachments)
                types += controller.ModConfiguration.Settings["AllowedAttachmentTypes"].ToString();
            if (AllowImageUploads)
            {
                if (types != "")
                    types += ",";
                types += controller.ModConfiguration.Settings["AllowedImageTypes"].ToString();
            }
            string[] allowedTypes = types.Split(',');
            int fileSizeLimit = Convert.ToInt32(controller.ModConfiguration.Settings["FileSizeLimit"].ToString()) * 1024;
            string uploadpath = controller.ModConfiguration.Settings["FileUploadLocation"].ToString();
            string filext = Path.GetExtension(AsyncFileUpload1.PostedFile.FileName).Replace(".", "");
            string contentType = AsyncFileUpload1.PostedFile.ContentType;
            bool allowed = false;

            if (contentType.Contains("image") && !AllowImageUploads)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "prohibited",
                    "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Image upload prohibited';});", true);
                AsyncFileUpload1.FailedValidation = true;
                return;
            }
            if (!contentType.Contains("image") && !AllowAttachments)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "prohibited",
                    "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File attachments prohibited';});", true);
                AsyncFileUpload1.FailedValidation = true;
                return;
            }

            foreach (string allowedType in allowedTypes)
            {
                if (filext == allowedType)
                {
                    allowed = true;
                    break;
                }
            }
            if (!allowed || (int.Parse(e.FileSize) > fileSizeLimit))
            {
                AsyncFileUpload1.FailedValidation = true;
                return;
            }

            var name = Path.GetFileName(e.FileName);
            if (contentType.Contains("image"))
            {
                uploadpath = "/Gallery";
            }
            string savePath =
                Page.MapPath(String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name,
                    name.Replace(" ", "+")));
            if (File.Exists(savePath))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "exists",
                    "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File already exists';});", true);
                AsyncFileUpload1.FailedValidation = true;
                return;
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size",
                "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " +
                e.FileSize + "';});", true);

            if (!Directory.Exists(Page.MapPath(String.Format("{0}/{1}", uploadpath, HttpContext.Current.User.Identity.Name))))
            {
                Directory.CreateDirectory(
                    Page.MapPath(String.Format("{0}/{1}", uploadpath, HttpContext.Current.User.Identity.Name)));
            }

            if (contentType.Contains("image"))
            {
                if (e.FileName != null && AllowImageUploads)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag",
                        "$(function() {top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[img]" +
                        String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) +
                        "[/img]';});", true);
                }
            }
            else
            {
                if (e.FileName != null && AllowAttachments)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag",
                        "$(function() {top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[file=" + e.FileName + "]" +
                        String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) +
                        "[/file]';});", true);
                }
            }

            AsyncFileUpload1.SaveAs(savePath);
            if (contentType.Contains("image"))
            {
                string thumbPath =
                    Page.MapPath(String.Format("{0}/{1}/thumbnail/{2}", uploadpath, HttpContext.Current.User.Identity.Name,
                        name.Replace(" ", "+")));
                if (!Directory.Exists(Page.MapPath(String.Format("{0}/{1}/thumbnail", uploadpath, HttpContext.Current.User.Identity.Name))))
                {
                    Directory.CreateDirectory(Page.MapPath(String.Format("{0}/{1}/thumbnail", uploadpath, HttpContext.Current.User.Identity.Name)));
                }
                GalleryFunctions.CreateThumbnail(savePath, thumbPath, 100);
            }
        }

        #endregion

        /// <summary>
        /// Notify search engine of new post
        /// </summary>
        /// <param name="yahooAppID">required for Yahoo</param>
        private static void Ping(string yahooAppID)
        {
            const string sitemapFile = "/Handlers/Sitemap.ashx";
            // Google
            try
            {
                WebRequest request = WebRequest.Create("http://www.google.com/webmasters/tools/ping?sitemap=" + sitemapFile);
                WebResponse response = request.GetResponse();
            }
            catch (Exception errGoogle)
            {
                // TODO: handle error!
            }

            // Yahoo
            try
            {
                // notice how they are not following the standard....
                if (!String.IsNullOrEmpty(yahooAppID))
                {
                    WebRequest request =
                        WebRequest.Create("http://search.yahooapis.com/SiteExplorerService/V1/updateNotification?appid=" + yahooAppID + "&url=" + sitemapFile);
                    WebResponse response = request.GetResponse();
                }
            }
            catch (Exception errYahoo)
            {
                // TODO: handle error
            }

            // Ask.com
            try
            {
                WebRequest request = WebRequest.Create("http://submissions.ask.com/ping?sitemap=" + sitemapFile);
                WebResponse response = request.GetResponse();
            }
            catch (Exception errAsk)
            {
                // TODO: handle error!
            }
        }

        #region Page methods 

        /// <summary>
        /// Called Asynchronously by the Preview window to parse [bb] tags
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [WebMethod]
        public static string ParseForumCode(string data)
        {
            string theme = HttpContext.Current.Session["PageTheme"] == null ? "Light" : HttpContext.Current.Session["PageTheme"].ToString();
            data = HttpUtility.UrlDecode(data);
            string parsedText = data.ParseTags();

            return string.Format("<head><link rel=\"stylesheet\" type=\"text/css\" href=\"~/{0}/base.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"~/{0}/bluegrey.css\" /><script type=\"text/javascript\">window.focus();</script></head><body><br/><div class=\"TopicDiv clearfix\">{1}</div></body>", theme, parsedText);
        }

        #endregion


    }
}