#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
#endregion
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using AjaxControlToolkit;
using SnitzUI.MasterTemplates;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzData;

namespace SnitzUI
{
    public partial class PostPage : PageBase
    {
        private string _action;
        private string _type;
        private int _recId;
        private Forum _forum;
        private Topic _thisTopic;
        private bool _topicLocked;
        private bool _inModeratedList;
        bool pingSiteMap = Config.PingSearchEngine;

        /// <summary>  
        /// since asp.net is stingy with the Themes, we have to do our own check to map the Theme directory 

        /// </summary>  
        private string PageThemeDirectory
        {
            get
            {
                if (null == ViewState["PageThemeDirectory"])
                {
                    if (string.IsNullOrEmpty(Page.Theme))
                        ViewState["PageThemeDirectory"] = "/App_Themes/" + Config.DefaultTheme;
                    else
                    {
                        if (Directory.Exists(Server.MapPath("/App_Themes/" + Page.Theme)))
                            ViewState["PageThemeDirectory"] = "/App_Themes/" + Page.Theme;
                        else
                            ViewState["PageThemeDirectory"] = string.Empty;
                    }
                }
                return ViewState["PageThemeDirectory"].ToString();
            }
        }  

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var master = (SingleCol)Master;
            if (master != null) master.rootScriptManager.EnablePageMethods = true;
            
            markitupCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/markitup.css");

            ForumDropDown.DataSource = Util.ListForums();
            ForumDropDown.DataBind();
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
                                throw new HttpException(403, "Access denied please try later");

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
            cbxSig.Checked = (Member.UseSig == 1);
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
                _inModeratedList = new SnitzRoleProvider().IsUserForumModerator(User.Identity.Name, ForumId.Value);
                _forum = Util.GetForum(ForumId.Value);
            }
            if (TopicId != null)
            {
                _thisTopic = Util.GetTopic(TopicId.Value, false);
                _topicLocked = _thisTopic.Status == Enumerators.PostStatus.Closed;
            }
            else if (_type == "topics")
            {
                TopicId = Int32.Parse(Request.Params["id"]);
                _thisTopic = Util.GetTopic(TopicId.Value, false);
                _topicLocked = _thisTopic.Status == Enumerators.PostStatus.Closed;
            }
        }

        private string GetQuotedMessage()
        {
            switch (_type)
            {
                case "reply" :
                    Reply reply = Util.GetReply(_recId);
                    return String.Format("[quote=\"{0}\"]{1}[/quote]", reply.Author.Name, reply.Message);
                case "topics" :
                    return String.Format("[quote=\"{0}\"]{1}[/quote]", _thisTopic.Author.Name, _thisTopic.Message);
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
                    Reply reply = Util.GetReply(_recId);
                    SubjectDiv.Visible = false;
                    //_IsAuthor = (reply.Author.Name.ToLower() == HttpContext.Current.User.Identity.Name.ToLower());
                    msg = HttpUtility.HtmlDecode(reply.Message);
                    break;
                case "topics":
                    ForumDiv.Visible = (IsAdministrator || (_inModeratedList && !Config.RestrictModeratorMove));

                    ForumDropDown.SelectedValue = _thisTopic.ForumId.ToString();
                    SubjectDiv.Visible = true;
                    tbxSubject.Text = HttpUtility.HtmlDecode(_thisTopic.Subject);
                    //_IsAuthor = (thisTopic.Author.Name == HttpContext.Current.User.Identity.Name.ToLower());
                    //ForumDiv.Visible = ForumDiv.Visible || (_IsAuthor && IsModerator);
                    string poll = "";
                    if(_thisTopic.PollID != null)
                    {
                        poll = Util.GetTopicPoll(_thisTopic.PollID);
                    }
                    msg = poll + HttpUtility.HtmlDecode(_thisTopic.Message);
                    break;
            }

            Message.Text = msg; //.Replace("'", "&#39;");
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
                            Util.UpdateReply(_recId, Message.Text, Member, IsAdministrator, cbxSig.Checked);
                            
                            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                                Util.SetTopicStatus(_thisTopic.Id, Enumerators.PostStatus.Closed);
                            if (_inModeratedList || IsAdministrator)
                                Util.MakeSticky(_thisTopic.Id, cbxSticky.Checked);
                            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + TopicId + "&whichpage=-1#" + _recId);
                            break;
                        case "topics":
                            #region check for changes to poll

                            var poll = new Regex(@"(?<poll>\[poll=\x22(?<question>.+?)\x22](?<answers>.+?)\[\/poll])", RegexOptions.Singleline);

                            if (poll.IsMatch(Message.Text))
                            {
                                string topicPoll = poll.Match(Message.Text).Value;
                                if (topicPoll == "" || topicPoll == "remove")
                                {
                                    Util.DeleteTopicPoll(_thisTopic.PollID);
                                }
                                else if(_thisTopic.Forum.AllowPolls)
                                {
                                    var answers = new Regex(@"\[\*=(?<sort>[0-9]+)](?<answer>.+?)\[/\*]",
                                                              RegexOptions.Singleline | RegexOptions.ExplicitCapture);
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

                                        Util.UpdateTopicPoll(_thisTopic.PollID, question, s);
                                    }
                                }
                                Message.Text = poll.Replace(Message.Text, "");
                            }

                            #endregion

                            Util.UpdateTopic(_thisTopic.Id, Message.Text, tbxSubject.Text, Member, IsAdministrator, cbxSig.Checked);
                            if(ForumDropDown.SelectedValue != _thisTopic.ForumId.ToString())
                            {
                                //move the topic
                                Util.ChangeTopicForum(_thisTopic.Id, ForumDropDown.SelectedValue);
                                if(Config.MoveNotify)
                                {
                                    _forum = Util.GetForum(Convert.ToInt32(ForumDropDown.SelectedValue));
                                    string mailFile = Server.MapPath("~/App_Data/TopicMove.txt");
                                    string strSubject = "Sent From " + Config.ForumTitle + ": Topic move notification";

                                    var builder = new UriBuilder("http",
                                                                        Request.Url.DnsSafeHost,
                                                                        Request.Url.Port, Page.ResolveUrl("~/Content/Forums/forum.aspx"), string.Format("?FORUM={0}", _forum.Id));

                                    var file = new StreamReader(mailFile);
                                    string msgBody = file.ReadToEnd();
                                    msgBody = msgBody.Replace("<%UserName%>", _thisTopic.Author.UserName);
                                    msgBody = msgBody.Replace("<%ForumUrl%>", Config.ForumTitle);
                                    msgBody = msgBody.Replace("<%TopicSubject%>", _thisTopic.Subject);
                                    msgBody = msgBody.Replace("<%MovedTo%>", _forum.Subject);
                                    msgBody = msgBody.Replace("<%URL%>", builder.Uri.AbsoluteUri);

                                    var mailsender = new snitzEmail
                                    {
                                        toUser = new MailAddress(_thisTopic.Author.Email, _thisTopic.Author.UserName),
                                        fromUser = "Forum Administrator",
                                        subject = strSubject,
                                        msgBody = msgBody
                                    };
                                    mailsender.send();
                                }
                            }
                            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                                Util.SetTopicStatus(_thisTopic.Id, Enumerators.PostStatus.Closed);
                            if (_inModeratedList || IsAdministrator)
                                Util.MakeSticky(_thisTopic.Id, cbxSticky.Checked);
                            if (pingSiteMap) { Ping(""); }
                            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + _thisTopic.Id);
                            break;
                    }
                    break;
            }

        }

        private void PostNewTopic()
        {
            string ipaddress = Common.GetIPAddress();


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

            var topic = new Topic
                              {
                                  Subject = tbxSubject.Text,
                                  Message = Message.Text,
                                  Date = DateTime.UtcNow,
                                  UseSignatures = cbxSig.Checked,
                                  IsSticky = cbxSticky.Checked,
                                  PostersIP = ipaddress,
                                  Status = Enumerators.PostStatus.Open
                              };

            if (cbxLock.Checked)
                topic.Status = Enumerators.PostStatus.Closed;
            else if (_inModeratedList)
            {
                if(topic.Forum.ModerationLevel == Enumerators.Moderation.AllPosts ||
                    topic.Forum.ModerationLevel == Enumerators.Moderation.Topics)
                topic.Status = Enumerators.PostStatus.UnModerated;
            }

            if (ForumId != null) topic.ForumId = ForumId.Value;
            if (CatId != null) topic.CatId = CatId.Value;
            int topicid = Util.AddTopic(topic, Member);
            if (pingSiteMap) { Ping(""); }

            if (topicPoll != String.Empty && topic.Forum.AllowPolls)
                CreatePoll(topicPoll, topicid);
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + topicid);
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

                Util.AddTopicPoll(topicid, question, s);
            }
        }

        private void PostNewReply()
        {
            string ipaddress = Common.GetIPAddress();

            var poll = new Regex(@"(?<poll>\[poll=(?<question>.+?)](?<answers>.+?)\[\/poll])", RegexOptions.Singleline);

            if (poll.IsMatch(Message.Text))
            {
                //Polls not allowed in replied
                Message.Text = poll.Replace(Message.Text, "");
            }

            var reply = new Reply
                              {
                                  Message = Message.Text,
                                  PostersIP = ipaddress,
                                  Status = Enumerators.PostStatus.Open,
                                  UseSignatures = cbxSig.Checked
                              };
            if (cbxLock.Checked)
                reply.Status = Enumerators.PostStatus.Closed;
            else if (_inModeratedList)
            {
                
                if (_thisTopic.Forum.ModerationLevel == Enumerators.Moderation.AllPosts ||
                    _thisTopic.Forum.ModerationLevel == Enumerators.Moderation.Replies)
                    reply.Status = Enumerators.PostStatus.UnModerated;
            }

            reply.TopicId = _thisTopic.Id;
            reply.CatId = _thisTopic.CatId;
            reply.ForumId = _thisTopic.ForumId;
            int postid = Util.AddReply(reply, Member);
            if (cbxLock.Checked && (_inModeratedList || IsAdministrator))
                Util.SetTopicStatus(_thisTopic.Id, Enumerators.PostStatus.Closed);
            if (pingSiteMap) { Ping(""); }
            Response.Redirect("/Content/Forums/topic.aspx?TOPIC=" + TopicId + "&whichpage=-1#" + postid);
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
                case "edit":
                    TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                    tempNode.Title = textInfo.ToTitleCase(_action) + Resources.webResources.lblMessage;
                    tempNode = tempNode.ParentNode;
                    tempNode.Title = string.Format("{0}:{1}", _thisTopic.Forum.Subject, _thisTopic.Subject);
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
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "error", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';", true);
        }

        private void AsyncFileUpload1UploadedComplete(object sender, AsyncFileUploadEventArgs e)
        {
                string filename = e.FileName;
                if (filename == null)
                    return;
            string contentType = AsyncFileUpload1.PostedFile.ContentType;
            if (!contentType.Contains("image"))
                return;
            var fileName = Path.GetFileName(filename);
            if (fileName != null && fileName.Contains(".pdf"))
            {
                AsyncFileUpload1.FailedValidation = true;
                return;
            }
            if (int.Parse(e.FileSize) > 2000000)
            {
                AsyncFileUpload1.FailedValidation = true;
                return;
            }
            var name = Path.GetFileName(filename);
            if (name != null)
            {
                string savePath = Page.MapPath(String.Format("~/Gallery/{0}/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ","+")));
                string thumbPath = Page.MapPath(String.Format("~/Gallery/{0}/thumbnail/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")));
                if (File.Exists(savePath))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File already exists';", true);
                    AsyncFileUpload1.FailedValidation = true;
                    return;
                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " + AsyncFileUpload1.FileBytes.Length.ToString() + "';", true);
                if (e.FileName != null)
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag", "top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[img]" + String.Format("/Gallery/{0}/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) + "[/img]';", true);

                if (!Directory.Exists(Page.MapPath(String.Format("~/Gallery/{0}", HttpContext.Current.User.Identity.Name))))
                {
                    Directory.CreateDirectory(Page.MapPath(String.Format("~/Gallery/{0}", HttpContext.Current.User.Identity.Name)));
                    Directory.CreateDirectory(Page.MapPath(String.Format("~/Gallery/{0}/thumbnail", HttpContext.Current.User.Identity.Name)));
                }

                AsyncFileUpload1.SaveAs(savePath);
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

        #region Page methods for Ajax Name and Email checks

        /// <summary>
        /// Called Asynchronously by the Preview window to parse [bb] tags
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [WebMethod]
        public static string ParseForumCode(string data)
        {
            var page = new PostPage();
            string test = page.PageThemeDirectory;
            data = HttpUtility.UrlDecode(data);
            string parsedText =
                data.ParseTags();

            parsedText = "<head><link rel=\"stylesheet\" type=\"text/css\" href=\"~/" + test + "/base.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"~/" + test + "/bluegrey.css\" /><script type=\"text/javascript\">window.focus();</script></head><body><br/><div class=\"TopicDiv clearfix\">" + parsedText + "</div></body>";
            return parsedText;
        }

        #endregion


    }
}