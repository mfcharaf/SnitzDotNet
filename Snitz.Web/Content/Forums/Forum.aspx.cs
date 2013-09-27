/*
####################################################################################################################
##
## SnitzUI.Content.Forums - Forum.aspx
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
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using Polls = Resources.Polls;


namespace SnitzUI
{
    public partial class ForumPage : PageBase
    {
        const string session = "FORUMID";

        bool _bGetSelectCount;
        private ForumInfo _currentForum;
        public bool IsForumModerator;
        private GridPager _replyPager;
        private int RowCount
        {
            get
            {
                if (ViewState["RowCount"] == null)
                    return 0;

                return (int)ViewState["RowCount"];
            }
            set
            {
                ViewState["RowCount"] = value;
            }
        }
        private string Collapsed
        {
            get
            {
                if (ViewState["Collapsed"] != null)
                    return ViewState["Collapsed"].ToString();
                return "0";
            }
            set
            {
                if (ViewState["Collapsed"] != null)
                    ViewState["Collapsed"] = value;
                else
                {
                    ViewState.Add("Collapsed", value);
                }
            }
        }        
        
        protected override void OnInit(EventArgs e)
        {
            
            base.OnInit(e);

            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            ForumTable.PageSize = Config.TopicPageSize;

            if (ForumId != null)
                IsForumModerator = Moderators.IsUserForumModerator(HttpContext.Current.User.Identity.Name, ForumId.Value);
            if (IsAdministrator || IsForumModerator)
            {
                ddlShowTopicDays.Items.Add(new ListItem("Unmoderated Posts", "999"));
            }
            if (stickystate.Value != "")
                Collapsed = stickystate.Value;

            topicUPD.Triggers.Add(new AsyncPostBackTrigger {ControlID = ddlShowTopicDays.UniqueID, EventName="SelectedIndexChanged"});
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (ForumId != null)
            {
                _currentForum = Forums.GetForum(ForumId.Value);
                if (_currentForum == null) throw new ArgumentException("Invalid Forum ID");
                if(_currentForum.Type == 1)
                {
                    Response.Redirect(_currentForum.Url,true);
                }
                
                fLogin.forum = _currentForum;
                if (!IsPostBack)
                {
                    if (IsAuthenticated)
                    {

                        //do we have access to this forum
                        if (!Forums.IsUserInForumRole(Member.Username, ForumId.Value))
                        {
                            if (Session[session] == null || Session[session].ToString() != ForumId.ToString())
                            {

                                if (_currentForum.Password != null &&
                                    !String.IsNullOrEmpty(_currentForum.Password.Trim()))
                                {
                                    if (Session[session] == null || Session[session].ToString() == "")
                                    {
                                        var mp = (ModalPopupExtender) fLogin.FindControl("popup");
                                        var masterPage = this.Page.Master;
                                        if (masterPage != null)
                                        {
                                            var cph = (ContentPlaceHolder) masterPage.FindControl("CPM");

                                            cph.Visible = false;
                                        }
                                        mp.Show();
                                    }
                                    else
                                    {
                                        if (Session[session].ToString() != ForumId.ToString())
                                            throw new SecurityException("You are not authorised to view this forum");
                                    }
                                }
                            }
                        }else
                        {
                            Session[session] = ForumId.ToString();
                        }
                    }

                }
                _currentForum.Roles = Forums.GetForumRoles(_currentForum.Id).ToList();
                if (!IsAuthenticated && (_currentForum.Roles.Count > 0 && !_currentForum.Roles.Contains("All")))
                {
                    if (Session[session] == null || Session[session].ToString() != ForumId.ToString())
                        throw new SecurityException("You must be logged in to view this forum");
                }
                if (IsAdministrator || IsForumModerator)
                    Session["IsAdminOrModerator"] = true;
                else
                    Session["IsAdminOrModerator"] = false;

                Page.Title = string.Format(webResources.ttlForumPage, _currentForum.Subject,  Config.ForumTitle);
                lblHotTopic.Text = string.Format(webResources.lblHotTopics, Config.HotTopicNum);
                string pagedescription = _currentForum.Description.CleanForumCodeTags();
                metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", pagedescription);


            }
            else
            {
                //Response.Redirect("error.aspx?msg=errInvalidForumId", true);
                throw new HttpException(404,"Forum not found");
            }
            InitializeStickyCollapse();
            

            if (Request.Params["whichpage"] != null)
            {
                try
                {
                    _replyPager.CurrentIndex = Int32.Parse(Request.Params["whichpage"]) - 1;
                }
                catch (Exception)
                {
                    //Response.Redirect("error.aspx?msg=errInvalidPageNumber",true);
                    throw new HttpException(404, "forum page not found");
                }
                
            }
            if (!IsPostBack)
            {
                //create CacheKeyDependency if it does not exists
                if (Cache[TopicODS.CacheKeyDependency] == null || (int)Cache[TopicODS.CacheKeyDependency] != ForumId)
                {
                    object obj = ForumId;
                    Cache[TopicODS.CacheKeyDependency] = obj;
                }
                BindData();
            }
                
        }

        private void BindData()
        {
            _currentForum.StickyTopics = Forums.GetStickyTopics(_currentForum.Id);

            StickyGrid.DataSource = _currentForum.StickyTopics;
            StickyGrid.DataBind();
            
            if (_currentForum.StickyTopics.Count == 0)
            {
                Sticky_HeaderPanel.Visible = false;
                StickyPanel.Visible = false;
                StickyGrid.Visible = false;
            }
            else if (stickystate.Value != "")
            {
                Sticky_Panel_CollapsiblePanelExtender.Collapsed = stickystate.Value == "1";
            }
            int numberOfDays = 0;

            if (ddlShowTopicDays.SelectedIndex > 0)
                numberOfDays = Convert.ToInt32(ddlShowTopicDays.SelectedValue);

            var diff = new TimeSpan(numberOfDays, 0, 0, 0);
            DateTime newdate = DateTime.UtcNow;

            switch (ddlShowTopicDays.SelectedValue)
            {
                case "-1": //All Topics
                    Session["LastPostDate"] = "";
                    Session["TopicStatus"] = "";
                    break;
                case "0": //All Open Topics
                    Session["LastPostDate"] = "";
                    Session["TopicStatus"] = Enumerators.PostStatus.Open;
                    break;
                case "999" :
                    Session["LastPostDate"] = "";
                    Session["TopicStatus"] = Enumerators.PostStatus.UnModerated;
                    break;
                case "": //All Topics
                    Session["LastPostDate"] = "";
                    Session["TopicStatus"] = "";
                    break;
                default: //Topics within Date Range
                    Session["LastPostDate"] = (newdate - diff).ToForumDateStr();
                    Session["TopicStatus"] = "";
                    break;
            }

            //TopicODS.Select();

        }
     
        private void InitializeStickyCollapse()
        {
            ControlPanelExtender(Collapsed == "1");
        }

        void ControlPanelExtender(bool value)
        {
            Sticky_Panel_CollapsiblePanelExtender.Collapsed = value;
            Sticky_Panel_CollapsiblePanelExtender.ClientState = value.ToString().ToLower();
        }
        
        protected void NumberOfDaysSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlShowTopicDays.SelectedIndex > -1)
            {
                BindData();
            }
        }

        protected void ForumTableRowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var topic = (TopicInfo) e.Row.DataItem;
                topic.Forum = Forums.GetForum(topic.ForumId);
                topic.PollId = Topics.GetTopicPollId(topic.Id);
                string currentUser = HttpContext.Current.User.Identity.Name;

                Image img = GetTopicIcon(topic);
                img.ApplyStyleSheetSkin(Page);
                e.Row.Cells[0].Controls.Add(img);

                var stickyIcon = e.Row.Cells[6].FindControl("Stick") as ImageButton;
                var unstickyIcon = e.Row.Cells[6].FindControl("UnStick") as ImageButton;
                var lockIcon = e.Row.Cells[6].FindControl("TopicLock") as ImageButton;
                var unlockIcon = e.Row.Cells[6].FindControl("TopicUnLock") as ImageButton;
                var delIcon = e.Row.Cells[6].FindControl("TopicDelete") as ImageButton;
                var approve = e.Row.Cells[6].FindControl("TopicApprove") as ImageButton;
                var subscribe = e.Row.Cells[6].FindControl("TopicSub") as ImageButton;
                var unsubscribe = e.Row.Cells[6].FindControl("TopicUnSub") as ImageButton;
                //ImageButton hold = e.Row.Cells[6].FindControl("TopicHold") as ImageButton;
                var editIcon = e.Row.Cells[6].FindControl("hypEditTopic") as HyperLink;
                var replyIcon = e.Row.Cells[6].FindControl("hypReplyTopic") as HyperLink;
                var noArchiveIcon = e.Row.Cells[6].FindControl("hypNoArchiveTopic") as HyperLink;
                var archiveIcon = e.Row.Cells[6].FindControl("hypArchiveTopic") as HyperLink;
                var popuplink = e.Row.Cells[5].FindControl("popuplink") as Literal;

                if(popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = topic.LastPostAuthorId != null ? Regex.Replace(topic.LastPostAuthorPopup, @"\[!(.*)!]", title) : "";
                }
                if(subscribe != null)
                {
                    subscribe.Visible = IsAuthenticated;
                    subscribe.Visible = subscribe.Visible && topic.AllowSubscriptions;
                }
                if(unsubscribe != null)
                {
                    unsubscribe.Visible = false;
                    if(subscribe.Visible)
                    {
                        if(Members.IsSubscribedToTopic(topic.Id,Member.Id))
                        {
                            subscribe.Visible = false;
                            unsubscribe.Visible = true;
                        }
                    }
                }
                if (stickyIcon != null)
                {
                    stickyIcon.Visible = ((IsAdministrator || IsForumModerator) && (!topic.IsSticky));
                }
                if (unstickyIcon != null)
                {
                    unstickyIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.IsSticky));
                    
                }
                if (lockIcon != null)
                {
                    lockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status != (int)Enumerators.PostStatus.Closed));
                }
                if (unlockIcon != null)
                {
                    unlockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status == (int)Enumerators.PostStatus.Closed));
                }
                if (replyIcon != null)
                    replyIcon.NavigateUrl = "/Content/Forums/post.aspx?method=reply&TOPIC_ID=" + topic.Id;
                if (noArchiveIcon != null)
                {
                    noArchiveIcon.Visible = IsAdministrator && (Topics.IsArchived(topic));
                    noArchiveIcon.NavigateUrl = "javascript:openConfirmDialog('pop_archive.aspx?archive=0&ID=" + topic.Id + "')";
                }
                if (archiveIcon != null)
                {
                    archiveIcon.Visible = IsAdministrator && (Topics.AllowArchive(topic));
                    archiveIcon.NavigateUrl = "javascript:openConfirmDialog('pop_archive.aspx?archive=1&ID=" + topic.Id + "')";
                }
                if (delIcon != null)
                {
                    delIcon.Visible = false;
                }
                if (editIcon != null)
                {
                    editIcon.NavigateUrl = "/Content/Forums/post.aspx?method=edit&type=TOPICS&id=" + topic.Id;
                    editIcon.Visible = false;
                }
                if(approve != null)
                {
                    approve.Visible = false;
                    if (topic.Status == (int)Enumerators.PostStatus.UnModerated || topic.Status == (int)Enumerators.PostStatus.OnHold)
                        approve.Visible = (IsForumModerator || IsAdministrator);
                    approve.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        topic.Id);
                }
                //if(hold != null)
                //{
                //    hold.Visible = false;
                //    if (topic.Status == Enumerators.PostStatus.UnModerated)
                //        hold.Visible = (IsForumModerator || IsAdministrator);
                //}
                if (!IsAuthenticated)
                    if (replyIcon != null) replyIcon.Visible = false;

                if (IsAdministrator || IsForumModerator )
                {
                    if (replyIcon != null) replyIcon.Visible = true;
                    if (delIcon != null) delIcon.Visible = true;
                    if (editIcon != null) editIcon.Visible = true;
                }
                else if (topic.Status == (int)Enumerators.PostStatus.Closed ||topic.Forum.Status == (int)Enumerators.PostStatus.Closed)
                {
                    if (replyIcon != null) replyIcon.Visible = false;
                    if (delIcon != null) delIcon.Visible = false;
                    if (editIcon != null) editIcon.Visible = false;
                }
                else if (currentUser.ToLower() == topic.AuthorName.ToLower())
                {
                    if (delIcon != null) delIcon.Visible = (topic.ReplyCount == 0);
                    if (editIcon != null) editIcon.Visible = true;
                }

                
                if (currentUser == "")
                {
                    e.Row.Cells.RemoveAt(6);
                    e.Row.Cells[5].ColumnSpan = 2;
                }
            }
            else if (e.Row.RowType == DataControlRowType.Pager)
            {
                _replyPager = (GridPager)e.Row.FindControl("pager");
                _replyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.TopicPageSize);
                _replyPager.CurrentIndex = CurrentPage;
            }
        }

        private Image GetTopicIcon(TopicInfo topic)
        {
            var image = new Image { ID = "imgTopicIcon" };
            string _new = "";
            string hot = "";
            string locked = "";
            string sticky = "";

            if (topic.LastPostDate > LastVisitDateTime)
            {
                image.AlternateText = webResources.lblNewPosts;
                image.ToolTip = webResources.lblNewPosts;
                if(topic.ReplyCount >= Config.HotTopicNum)
                _new = "New";
            }
            if (topic.ReplyCount >= Config.HotTopicNum)
                hot = "Hot";
            switch ((Enumerators.PostStatus)topic.Status)
            {
                case Enumerators.PostStatus.Open:
                    locked = "";
                    image.ToolTip = webResources.lblOldPosts;
                    break;
                case Enumerators.PostStatus.UnModerated:
                    image.AlternateText = webResources.Unmoderatedpost;
                    image.ToolTip = webResources.Unmoderatedpost;
                    break;
                case Enumerators.PostStatus.OnHold:
                    image.AlternateText = webResources.OnHold;
                    image.ToolTip = webResources.OnHold;
                    break;
                default:
                    locked = "Locked";
                    hot = "";
                    image.AlternateText = webResources.lblLockedTopic;
                    image.ToolTip = webResources.lblTopicLocked;
                    break;
            }
            if (topic.IsSticky)
            {
                sticky = "Sticky";
                image.AlternateText = webResources.lblStickyTopic;
                image.ToolTip = locked == "" ? webResources.lblStickyTopic : webResources.lblStickyTopic + ", " + webResources.lblTopicLocked;
            }

            image.SkinID = "Folder" + _new + hot +  sticky + locked;

            if (topic.Status == (int)Enumerators.PostStatus.UnModerated)
            {
                image.ToolTip = webResources.Unmoderatedpost;
                image.SkinID = "UnModerated";
            }
            if (topic.Status == (int)Enumerators.PostStatus.OnHold)
            {
                image.ToolTip = webResources.TopicOnHold;
                image.SkinID = "OnHold";
            }
            if (topic.UnModeratedReplies > 0)
            {
                image.ToolTip = webResources.UnmoderatedPosts;
                image.SkinID = "UnmoderatedPosts";
            }
            if (topic.PollId > 0)
            {
                image.ToolTip = Polls.lblPoll;
                image.SkinID = "Poll";
            }
            return image;
        }

        protected override SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
                SiteMapNode tempNode = currentNode;

                tempNode.Title = _currentForum.Subject.CleanForumCodeTags();
                if (_currentForum.ModerationLevel != (int)Enumerators.Moderation.UnModerated)
                {
                    tempNode.Title += String.Format(" ({0})", EnumHelper.GetDescription((Enumerators.Moderation)_currentForum.ModerationLevel));
                }
                if (_currentForum.Status == (int)Enumerators.PostStatus.Closed)
                {
                    tempNode.Title += webResources.ForumIsLocked;
                }
                return currentNode;
            }
            return null;
        }

        protected void TopicOdsSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            _bGetSelectCount = e.ExecutingSelectCount;
            if (e.ExecutingSelectCount)
            {
                //Cancel the event   
                return;
            }
        }

        protected void TopicOdsSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (_bGetSelectCount)
            {
                RowCount = (int)e.ReturnValue;
                if (CurrentPage != ForumTable.PageIndex)
                    CurrentPage = ForumTable.PageIndex;
            }
        }

        protected void DeleteTopic(object sender, ImageClickEventArgs e)
        {
            var lbl = UpdateProgress1.FindControl("lblProgress");
            if (lbl != null)
                ((Label) lbl).Text = "Deleting Topic";
            var btn = (ImageButton) sender;
            Topics.Delete(Convert.ToInt32(btn.CommandArgument));
            InvalidateCache();
            ForumTable.DataBind();
        }

        protected void LockTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Topics.SetTopicStatus(Convert.ToInt32(btn.CommandArgument), (int)Enumerators.PostStatus.Closed);
            InvalidateCache();
            ForumTable.DataBind();
        }

        protected void UnLockTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Topics.SetTopicStatus(Convert.ToInt32(btn.CommandArgument), (int)Enumerators.PostStatus.Open);
            InvalidateCache();
            ForumTable.DataBind();
        }

        protected void StickTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Topics.MakeSticky(Convert.ToInt32(btn.CommandArgument), true);
            InvalidateCache();
            ForumTable.DataBind();
        }

        protected void UnStickTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Topics.MakeSticky(Convert.ToInt32(btn.CommandArgument), false);
            InvalidateCache();
            ForumTable.DataBind();
        }

        private void InvalidateCache()
        {
            object obj = -1;
            Cache[TopicODS.CacheKeyDependency] = obj;
        }

        protected void FilterTopics(object sender, ImageClickEventArgs e)
        {
            var searchfor = (TextBox) this.ForumTable.HeaderRow.Cells[6].FindControl("SearchFor");

            Session.Add("ForumSearch", searchfor.Text);
            Response.Redirect(Request.RawUrl, true);
        }

        protected void TopicSubscribe(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            int topicid = Convert.ToInt32(btn.CommandArgument);
            switch (btn.CommandName)
            {
                case "sub" :
                    Subscriptions.AddTopicSubscription(Member.Id, topicid);
                    break;
                case "unsub" :
                    Subscriptions.RemoveTopicSubscription(Member.Id, topicid);
                    break;
            }
            
        }


        [WebMethod]
        public static void Approval(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
            {
                int id = Convert.ToInt32(topicid);
                TopicInfo topic = Topics.GetTopic(id);
                //topic.Forum = Forums.GetForum(topic.ForumId);
                Topics.SetTopicStatus(id, (int)Enumerators.PostStatus.Open);
                if(topic.Forum.SubscriptionLevel == (int)Enumerators.Subscription.ForumSubscription)
                    Subscriptions.ProcessForumSubscriptions(topic);

            }
            if (!String.IsNullOrEmpty(replyid))
            {
                int id = Convert.ToInt32(replyid);
                ReplyInfo reply = Replies.GetReply(id);
                TopicInfo topic = Topics.GetTopic(reply.TopicId);
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.Open);
                if(topic.AllowSubscriptions)
                    Subscriptions.ProcessTopicSubscriptions(topic, reply);
            }

        }
        [WebMethod]
        public static void PutOnHold(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Topics.SetTopicStatus(Convert.ToInt32(topicid), (int)Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(replyid))
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.OnHold);
        }

    }
}