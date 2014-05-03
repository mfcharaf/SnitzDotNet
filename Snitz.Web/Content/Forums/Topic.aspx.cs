/*
####################################################################################################################
##
## SnitzUI.Content.Forums - Topic.aspx
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
using System.Linq;
using System.Net.Mail;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI
{
    public partial class TopicPage : PageBase, IRoutablePage
    {
        private TopicInfo _topic;
        protected int _archiveView;
        protected internal GridPager ReplyPager;
        private int RowCount
        {
            get
            {
                return (int)ViewState["RowCount"];
            }
            set
            {
                ViewState["RowCount"] = value;
            }
        }

        public delegate void PopulateObject(int myInt);
        
        private void PopulateData(int currentPage)
        {
            FetchData(currentPage);
        }

        private void FetchData(int pageIndex)
        {
            if (replyFilter.SelectedIndex > -1)
            {
                var topics = new List<TopicInfo> {_topic};
                TopicView.DataSource = topics;
                TopicView.DataBind();

            }
            int pageSize = Config.TopicPageSize;
            
            
            var replies = Topics.GetRepliesForTopic(_topic, pageIndex, pageSize);
            if(replyFilter.SelectedIndex > 0)
            {
                switch (replyFilter.SelectedValue)
                {
                    case "Asc" :
                        replies = replies.OrderBy(r => r.Date);
                        break;
                    case "Desc" :
                        replies = replies.OrderByDescending(r => r.Date);
                        break;
                    case "Last" :
                        replies = replies.Where(r => r.Date >= _topic.LastPostDate);
                        break;
                    case "New" :
                        replies = replies.Where(r => r.Date > ((PageBase) Page).LastVisitDateTime);
                        break;
                }
            }
            var page = new PagedDataSource
            {
                AllowCustomPaging = true,
                AllowPaging = true,
                DataSource = replies,
                PageSize = pageSize
            };
            TopicReplies.DataSource = page;
            TopicReplies.DataBind();
        }

        protected static bool ShowSig(object post)
        {
            bool result = false;

            if( post is ReplyInfo)
            {
                var reply = (ReplyInfo)post;
                result = (Config.AllowSignatures && reply.AuthorViewSig && reply.UseSignatures && !String.IsNullOrEmpty(reply.AuthorSignature));
            }
            else if (post is TopicInfo)
            {
                var topic = (TopicInfo)post;
                result = (Config.AllowSignatures && topic.AuthorViewSig && topic.UseSignatures && !String.IsNullOrEmpty(topic.AuthorSignature));

            }
            

            return result;
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");

            if (TopicId == null)
                throw new HttpException(404, "Topic not found");
                //Response.Redirect("~/error.aspx?msg=errInvalidTopicId",true);
            if (Request.QueryString["ARCHIVE"] != null)
            {
                if (Request.QueryString["ARCHIVE"] == "1")
                {
                    //TopicODS.TypeName = "Snitz.BLL.Archive";
                    _archiveView = 1;
                }
            }
            else
            {
                //TopicODS.TypeName = "Snitz.BLL.Forums";
                _archiveView = 0;
            }
            try
            {
                if (TopicId != null)
                {
                    if (Session["TOPIC"] == null)
                        Session.Add("TOPIC", TopicId);
                    else
                        Session["TOPIC"] = TopicId;

                    string skip = "";
                    if (!String.IsNullOrEmpty(Request.Params["dir"]))
                    {
                        skip = Request.Params["dir"];
                    }
                    _topic = Topics.GetTopic(TopicId.Value);
                    if (skip != "")
                    {
                        _topic = Topics.GetNextPrevTopic(_topic.Id, skip);
                        TopicId = _topic.Id;
                    }
                    _topic.Author = Members.GetAuthor(_topic.AuthorId);
                    //Grid pager setup
                    ReplyPager = (GridPager) LoadControl("~/UserControls/GridPager.ascx");
                    ReplyPager.PagerStyle = Enumerators.PagerType.Lnkbutton;
                    ReplyPager.UserControlLinkClick += PagerLinkClick;
                    RowCount = _topic.ReplyCount;
                    ReplyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.TopicPageSize);

                    Page.Title = string.Format(webResources.ttlTopicPage, _topic.Subject.CleanForumCodeTags(), Config.ForumTitle);
                    string pagedescription = _topic.Message.CleanForumCodeTags();
                    metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", HttpUtility.HtmlEncode(pagedescription.Substring(0, Math.Min(160, pagedescription.Length))));

                }
            }
            catch (Exception)
            {
                throw new HttpException(404, "Topic not found");
                //Response.Redirect("~/error.aspx?msg=errInvalidTopicId", true);
            }

            var meta = new HtmlMeta();
            meta.Attributes.Add("name", "description");
            meta.Attributes.Add("content", _topic.Subject);
            Page.Header.Controls.Add(meta);

            if (User.Identity.IsAuthenticated)
            {
                if ((Config.ShowQuickReply && _topic.Status != (int)Enumerators.PostStatus.Closed && _topic.Forum.Status != (int)Enumerators.PostStatus.Closed) || IsAdministrator)
                {
                    var qr = (QuickReply)Page.LoadControl("~/UserControls/QuickReply.ascx"); //loading the user control dynamically
                    qr.thisTopic = _topic;
                    QRPlaceHolder.Controls.Add(qr);
                }
            }
            PopulateObject populate = PopulateData;
            ReplyPager.UpdateIndex = populate;
            pager.Controls.Add(ReplyPager);
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentPage == -1)
                CurrentPage = 0;

            if (Request.Form["__EVENTTARGET"] != null)
            {
                //let's check what async call posted back and see if we need to refresh the page
                string target = Request.Form["__EVENTTARGET"];
                var listOfStrings = new List<string> { "TopicSend","TopicDelete","imgPosticon" };
                bool refreshAfterPostback = listOfStrings.Any(target.EndsWith);
                if (refreshAfterPostback)
                {
                    ReplyPager.CurrentIndex = CurrentPage;
                }
            }
            if (!Page.IsPostBack)
            {
                TopicInfo topic = _topic;
                string session = "FORUM" + topic.ForumId;

                if (!IsPostBack)
                {
                    if (IsAuthenticated)
                    {

                        //do we have access to this forum
                        if (!Forums.IsUserInForumRole(Member.Username, topic.Forum.Id))
                        {
                            if (Session[session] == null || Session[session].ToString() != "true")
                            {

                                if (topic.Forum.Password != null &&
                                    !String.IsNullOrEmpty(topic.Forum.Password.Trim()))
                                {
                                    if (Session[session] == null || Session[session].ToString() == "")
                                    {
                                        Response.Redirect("~/Content/Forums/forum.aspx?FORUM=" + topic.ForumId);
                                    }
                                    else
                                    {
                                        if (Session[session].ToString() != "true")
                                            throw new SecurityException("You are not authorised to view this forum");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Session[session] = "true";
                        }
                    }
                    else if(topic.Forum.Roles.Contains("All"))
                    {
                        Session[session] = "true";
                    }
                    else if (topic.Forum.Roles.Count > 0 && !topic.Forum.Roles.Contains("All"))
                    {
                        if (Session[session] == null || Session[session].ToString() != "true")
                            throw new SecurityException("You must be logged in to view this forum");
                    }

                }

                Topics.UpdateViewCount(_topic.Id);
                if ((Request.Params["whichpage"] != null))
                {
                    if (Request.Params["whichpage"] == "-1")
                    {
                        //jump to last page
                        int pagenum = topic.ReplyCount/Config.TopicPageSize;
                        if (topic.ReplyCount%Config.TopicPageSize == 0)
                            if (topic.ReplyCount > 0)
                                pagenum -= 1;
                        CurrentPage = pagenum;
                        ReplyPager.CurrentIndex = pagenum;
                        
                    }
                    else
                    {
                        int pagenum = Int32.Parse(Request.Params["whichpage"]) - 1;
                        CurrentPage = pagenum;
                        ReplyPager.CurrentIndex = pagenum;
                    }
                    
                }
                else
                    ReplyPager.CurrentIndex = CurrentPage;
            }

            if (CurrentPage != 0)
                TopicView.Visible = false;

            if (Request.Params["reply"] != null)
            {
                string reply = Request.Params["reply"];
                if (reply != "0")
                    JumpToReply(reply);
            }

        }

        private void JumpToReply(string reply)
        {
            int replyPage = Replies.FindReplyPage(Convert.ToInt32(reply));
            CurrentPage = replyPage;
            ReplyPager.CurrentIndex = CurrentPage;
        }

        private void PagerLinkClick(object sender, EventArgs e)
        {
            var lnk = sender as LinkButton;

            if (lnk != null)
            {
                if (lnk.Text.IsNumeric())
                    CurrentPage = int.Parse(lnk.Text)-1;
                else
                {
                    if (lnk.Text.Contains("&gt;"))
                        CurrentPage += 1;
                    else if (lnk.Text.Contains("&lt;"))
                        CurrentPage -= 1;
                    else if (lnk.Text.Contains("&raquo;"))
                        CurrentPage = ReplyPager.PageCount-1;
                    else
                        CurrentPage = 0;
                }
                if (CurrentPage < 0)
                    CurrentPage = 0;
                if (CurrentPage >= ReplyPager.PageCount)
                    CurrentPage = ReplyPager.PageCount - 1;
            }
            ReplyPager.CurrentIndex = CurrentPage;
        }

        protected void ReplyFilterSelectedIndexChanged(object sender, EventArgs e)
        {
            ReplyPager.CurrentIndex = 0;
        }

        protected void TopicViewItemCommand(object sender, FormViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {

                case "prev" :
                    _topic = Topics.GetNextPrevTopic(_topic.Id, "prev");
                    if (Session["TOPIC"].ToString() != _topic.Id.ToString())
                        Response.Redirect("~/Content/Forums/topic.aspx?TOPIC=" + _topic.Id, true);
                    break;
                case "next" :
                    _topic = Topics.GetNextPrevTopic(_topic.Id, "next");
                    if (Session["TOPIC"].ToString() != _topic.Id.ToString())
                        Response.Redirect("~/Content/Forums/topic.aspx?TOPIC=" + _topic.Id, true);
                    break;
            }
        }
        
        protected override SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
        {
            SiteMapNode currentNode = null;
            if (SiteMap.CurrentNode == null)
            {
                var routable = e.Context.CurrentHandler as IRoutablePage;

                if (routable != null)
                {
                    var rc = routable.Routing.RequestContext;
                    var route = rc.RouteData.Route;
                    var segments = route.GetVirtualPath(rc, null).VirtualPath.Split('/');
                    var path = "~/" + string.Join("/", segments.Take(segments.Length - rc.RouteData.Values.Count).ToArray());
                    //SiteMapNode testNode = SiteMap.Provider.FindSiteMapNodeFromKey(path);
                    var findSiteMapNodeFromKey = SiteMap.Provider.FindSiteMapNodeFromKey(path);
                    if (findSiteMapNodeFromKey != null)
                        currentNode = findSiteMapNodeFromKey.Clone(true);
                }
            }
            if (SiteMap.CurrentNode != null)
            {
                currentNode = SiteMap.CurrentNode.Clone(true);
            }
            SiteMapNode tempNode = currentNode;
            TopicInfo topic = _topic;
            string strStatus = "";
            if (topic.Status == 0)
                strStatus = " (locked) ";
            if (topic.IsArchived)
            {
                strStatus = " (Archived)";
                topic.Status = 0;
            }
            tempNode.Title = HttpUtility.HtmlDecode(topic.Subject.CleanForumCodeTags()) + strStatus;
            tempNode = tempNode.ParentNode;
            tempNode.Title = HttpUtility.HtmlDecode(topic.Forum.Subject.CleanForumCodeTags());
            tempNode.Url = tempNode.Url + "?FORUM=" + topic.ForumId;
            if (topic.IsArchived)
                tempNode.Url += "&ARCHIVE=1";
            //TopicView.Visible = Pager1.CurrentIndex < 1;
            return currentNode;

        }
        
        protected void RepliesBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            var reply = (ReplyInfo) item.DataItem;

            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                var mbar = item.FindControl(@"bbr") as MessageButtonBar;
                if (mbar != null) mbar.DeleteClicked += TopicDeleted;
                var popuplink = item.FindControl(@"popuplink") as Literal;

                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = Regex.Replace(reply.AuthorPopup, @"\[!(.*)!]", title);
                }
            }
            var editdiv = item.FindControl(@"editbyDiv");
            if (editdiv != null)
            {
                editdiv.Visible = (reply.LastEditDate.HasValue && reply.LastEditDate.Value != DateTime.MinValue) && Config.ShowEditBy;
            }
        }

        private void TopicDeleted(object sender, EventArgs e)
        {
            ReplyPager.CurrentIndex = CurrentPage;
        }

        protected void TopicBound(object sender, EventArgs e)
        {

            var frm = (FormView)sender;
            var currentTopic = ((TopicInfo) frm.DataItem);
            var mbar = frm.FindControl(@"bbr") as MessageButtonBar;

            if(!Config.ShowTopicNav || currentTopic.IsArchived)
            {
                frm.HeaderRow.Visible = false;
            }
            
            if (mbar != null)
                mbar.DeleteClicked += TopicDeleted;

            var ph = frm.FindControl(@"msgPH") as PlaceHolder; 
            
            currentTopic.PollId = Topics.GetTopicPollId(currentTopic.Id);
            var msgDisplay = new Literal { Text = currentTopic.Message.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls(), Mode = LiteralMode.Encode };

            if (currentTopic.PollId > 0)
            {
                HtmlControl div = (HtmlControl)frm.FindControl(@"msgContent");
                if (div != null)
                {
                    div.Attributes["class"] = "mContent";
                }
                var poll = (Poll)Page.LoadControl(@"~/UserControls/Polls/Poll.ascx");
                if (currentTopic.PollId != null) poll.PollId = currentTopic.PollId.Value;
                if (ph != null)
                {
                    ph.Controls.Add(poll);
                }
                msgDisplay.Mode = LiteralMode.Transform;
                msgDisplay.Text = msgDisplay.Text.ParseTags();
            }
            var editdiv = frm.FindControl(@"editbyDiv");
            if (editdiv != null)
            {
                editdiv.Visible = (currentTopic.LastEditDate.HasValue && currentTopic.LastEditDate.Value != DateTime.MinValue) && Config.ShowEditBy;
            }
            if (ph != null)
            {
                ph.Controls.Add(msgDisplay);
            }
        }

        [WebMethod]
        public static void Approval(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Topics.SetTopicStatus(Convert.ToInt32(topicid), (int)Enumerators.PostStatus.Open);
            if (!String.IsNullOrEmpty(replyid))
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.Open);
        }

        [WebMethod]
        public static void PutOnHold(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Topics.SetTopicStatus(Convert.ToInt32(topicid), (int)Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(replyid))
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.OnHold);
        }

        [WebMethod]
        public static string SendEmail(string name, string email, string message, string subject)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("You must supply a name.");
            }

            if (string.IsNullOrEmpty(email))
            {
                MembershipUser mu = Membership.GetUser(name, false);
                if (mu == null)
                    throw new Exception("You must supply an email address.");
                email = mu.Email;
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Please provide a message to send.");
            }

            string strSubject;
            if (String.IsNullOrEmpty(subject))
                strSubject = "Sent From " + Config.ForumTitle + " by " + HttpContext.Current.User.Identity.Name;
            else
                strSubject = subject;

            var mailsender = new snitzEmail
            {
                toUser = new MailAddress(email, name),
                fromUser = HttpContext.Current.User.Identity.Name,
                subject = strSubject,
                msgBody = message
            };

            mailsender.send();
            return "Your Email has been sent successfully";
        }

        [WebMethod]
        public static string SplitTopic(string jsonform)
        {
            var test = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            var replyIDs = new List<int>();
            foreach (string key in formresult.AllKeys)
            {
                if (key.EndsWith("cbxRow"))
                    replyIDs.Add(Convert.ToInt32(formresult[key]));
            }

            int topicid = Convert.ToInt32(formresult["ctl00$splitTopicId"]);
            int forumId = Convert.ToInt32(formresult["ctl00$ddlForum"]);
            string sort = formresult["ctl00$ddlSort"];

            string subject = formresult["ctl00$tbxSubject"];
            if (String.IsNullOrEmpty(subject))
                return "No subject supplied";

            TopicInfo oldtopic = Topics.GetTopic(topicid);
            ForumInfo forum = Forums.GetForum(forumId);

            if (replyIDs.Count == 0)
                return "No replies selected";
            int lastreplyid = sort == "desc" ? replyIDs[replyIDs.Count - 1] : replyIDs[0];
            ReplyInfo lastreply = Replies.GetReply(lastreplyid);

            //get the reply details
            var topic = new TopicInfo
            {
                Subject = subject,
                Message = lastreply.Message,
                Date = lastreply.Date,
                UseSignatures = lastreply.UseSignatures,
                IsSticky = false,
                PosterIp = lastreply.PosterIp,
                Views = 0,
                ReplyCount = replyIDs.Count - 1,
                Status = (int)Enumerators.PostStatus.Open,
                UnModeratedReplies = 0,
                ForumId = forumId,
                AuthorId = lastreply.AuthorId,
                CatId = forum.CatId
            };

            bool isModeratedForum = forum.ModerationLevel != (int)Enumerators.Moderation.UnModerated;
            if (isModeratedForum)
            {
                if (forum.ModerationLevel == (int)Enumerators.Moderation.AllPosts ||
                    forum.ModerationLevel == (int)Enumerators.Moderation.Topics)
                    topic.Status = (int)Enumerators.PostStatus.UnModerated;
            }

            int newtopicid = Topics.Add(topic);
            //delete the reply used as topic
            Replies.DeleteReply(lastreplyid);
            //move the other replies to this topic
            Replies.MoveReplies(newtopicid, replyIDs);
            //update the original topic count/dates
            Topics.Update(oldtopic.Id);

            Snitz.BLL.Admin.UpdateForumCounts();

            return "Selected replies were moved to a new topic";
        }


        public RoutingHelper Routing { get; set; }
    }
}