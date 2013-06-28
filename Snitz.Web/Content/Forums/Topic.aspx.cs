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
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzData;


namespace SnitzUI
{
    public partial class TopicPage : PageBase, IRoutablePage
    {
        private Topic _topic;
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
                var topics = new List<Topic> {_topic};
                TopicView.DataSource = topics;
                TopicView.DataBind();

            }
            int pageSize = Config.TopicPageSize;
            var replies = PagedObjects.GetTopicRepliesPaged(_topic.Id, pageIndex, pageSize);
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

            if( post is Reply)
            {
                var reply = (Reply)post;
                result = (Config.AllowSignatures && reply.Author.ViewSignatures && reply.UseSignatures && !String.IsNullOrEmpty(reply.Author.Signature));
            }
            else if (post is Topic)
            {
                var topic = (Topic)post;
                result = (Config.AllowSignatures && topic.Author.ViewSignatures && topic.UseSignatures && !String.IsNullOrEmpty(topic.Author.Signature));

            }
            

            return result;
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            markitupCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/markitup.css");

            if (TopicId == null)
                throw new HttpException(404, "Topic not found");
                //Response.Redirect("~/error.aspx?msg=errInvalidTopicId",true);
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
                    _topic = Util.GetTopic(TopicId.Value);
                    if (skip != "")
                    {
                        _topic = Util.GetTopic(_topic, skip);
                        TopicId = _topic.Id;
                    }

                    //Grid pager setup
                    ReplyPager = (GridPager) LoadControl("~/UserControls/GridPager.ascx");
                    ReplyPager.PagerStyle = PagerType.Lnkbutton;
                    ReplyPager.UserControlLinkClick += PagerLinkClick;
                    RowCount = PagedObjects.GetTopicReplyCount(TopicId.Value);
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
                if ((Config.ShowQuickReply && _topic.Status != Enumerators.PostStatus.Closed && _topic.Forum.Status != Enumerators.PostStatus.Closed) || IsAdministrator)
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
                Topic topic = _topic;
                string session = "FORUM" + topic.ForumId;
                //http://localhost:56932/Content/Forums/topic.aspx?TOPIC=69803
                if (!IsPostBack)
                {
                    if (IsAuthenticated)
                    {

                        //do we have access to this forum
                        if (!SnitzRoleProvider.IsUserInForumRole(Member.Name, topic.Forum.Id))
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

                Util.UpdateViewCount(_topic.Id);
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
                JumpToReply(reply);
            }

        }

        private void JumpToReply(string reply)
        {
            int replyPage = Util.FindReplyPage(Convert.ToInt32(reply));
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
                    _topic = Util.GetTopic(_topic, "prev");
                    if (Session["TOPIC"].ToString() != _topic.Id.ToString())
                        Response.Redirect("~/Content/Forums/topic.aspx?TOPIC=" + _topic.Id, true);
                    break;
                case "next" :
                    _topic = Util.GetTopic(_topic, "next");
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
            Topic topic = _topic;
            tempNode.Title = HttpUtility.HtmlDecode(topic.Subject.CleanForumCodeTags());
            tempNode = tempNode.ParentNode;
            tempNode.Title = HttpUtility.HtmlDecode(topic.Forum.Subject.CleanForumCodeTags());
            tempNode.Url = tempNode.Url + "?FORUM=" + topic.ForumId;
            //TopicView.Visible = Pager1.CurrentIndex < 1;
            return currentNode;

        }
        protected void RepliesBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            var reply = (Reply) item.DataItem;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                var mbar = item.FindControl("bbr") as MessageButtonBar;
                if (mbar != null) mbar.DeleteClicked += TopicDeleted;
                var popuplink = item.FindControl("popuplink") as Literal;

                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = reply.Author != null ? Regex.Replace(reply.Author.ProfilePopup, @"\[!(.*)!]", title) : "";
                }
            }
        }

        private void TopicDeleted(object sender, EventArgs e)
        {
            ReplyPager.CurrentIndex = CurrentPage;
        }

        protected void TopicBound(object sender, EventArgs e)
        {
            var frm = (FormView)sender;
            if(!Config.ShowTopicNav)
            {
                frm.HeaderRow.Visible = false;
            }
            var mbar = frm.FindControl("bbr") as MessageButtonBar;
            if (mbar != null)
                mbar.DeleteClicked += TopicDeleted;

            var ph = frm.FindControl("msgPH") as PlaceHolder; 
            
            var currentTopic = ((Topic) frm.DataItem);
            var msgDisplay = new Literal {Text = currentTopic.Message, Mode = LiteralMode.Encode};

            if (currentTopic.PollID > 0)
            {
                var poll = (Poll)Page.LoadControl("~/UserControls/Polls/Poll.ascx");
                if (currentTopic.PollID != null) poll.PollId = currentTopic.PollID.Value;
                if (ph != null) ph.Controls.Add(poll);
            }
            if (ph != null) ph.Controls.Add(msgDisplay);
        }



        [WebMethod]
        public static void Approval(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Util.SetTopicStatus(Convert.ToInt32(topicid), Enumerators.PostStatus.Open);
            if (!String.IsNullOrEmpty(replyid))
                Util.SetReplyStatus(Convert.ToInt32(replyid), Enumerators.PostStatus.Open);
        }
        [WebMethod]
        public static void PutOnHold(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Util.SetTopicStatus(Convert.ToInt32(topicid), Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(replyid))
                Util.SetReplyStatus(Convert.ToInt32(replyid), Enumerators.PostStatus.OnHold);
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

            Topic oldtopic = Util.GetTopic(topicid);
            Forum forum = Util.GetForum(forumId);

            if (replyIDs.Count == 0)
                return "No replies selected";
            int lastreplyid = sort == "desc" ? replyIDs[replyIDs.Count - 1] : replyIDs[0];
            Reply lastreply = Util.GetReply(lastreplyid);

            //get the reply details
            var topic = new Topic
            {
                Subject = subject,
                Message = lastreply.Message,
                Date = lastreply.Date,
                UseSignatures = lastreply.UseSignatures,
                IsSticky = false,
                PostersIP = lastreply.PostersIP,
                ViewCount = 0,
                ReplyCount = replyIDs.Count - 1,
                Status = Enumerators.PostStatus.Open,
                ForumId = forumId,
                CatId = forum.CatId
            };

            bool isModeratedForum = topic.Forum.ModerationLevel != Enumerators.Moderation.UnModerated;
            if (isModeratedForum)
            {
                if (forum.ModerationLevel == Enumerators.Moderation.AllPosts ||
                    forum.ModerationLevel == Enumerators.Moderation.Topics)
                    topic.Status = Enumerators.PostStatus.UnModerated;
            }

            int newtopicid = Util.AddTopic(topic, Util.GetMember(lastreply.Author.Name));
            //delete the reply used as topic
            Util.DeleteReply(lastreplyid);
            //move the other replies to this topic
            Util.MoveReplies(newtopicid, replyIDs);
            //update the original topic count/dates
            Util.UpdateTopic(oldtopic.Id);

            //do we need to update the forum?
            if (oldtopic.ForumId != Util.GetTopic(newtopicid).Id)
            {
                //update original forum count/dates
                Util.UpdateForum(oldtopic.ForumId);
            }

            return "Selected replies were moved to a new topic";
        }


        public RoutingHelper Routing { get; set; }
    }
}