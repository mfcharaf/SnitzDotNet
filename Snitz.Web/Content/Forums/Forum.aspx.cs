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
        private PopulateObject populate;
        protected int _archiveView;
        bool _bGetSelectCount;
        private ForumInfo _currentForum;
        public bool IsForumModerator;
        private GridPager _replyPager;
        //private GridPager _topicPager;
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
        private int PreviousPage
        {
            get
            {
                if (ViewState["PreviousPage"] == null)
                    return 0;

                return (int)ViewState["PreviousPage"];
            }
            set
            {
                ViewState["PreviousPage"] = value;
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
        public delegate void PopulateObject(int myInt);

        protected override void OnInit(EventArgs e)
        {
            
            base.OnInit(e);

            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            ForumTable.PageSize = Config.TopicPageSize;

            if (ForumId != null)
            {
                IsForumModerator = Moderators.IsUserForumModerator(HttpContext.Current.User.Identity.Name, ForumId.Value);
                RowCount = Forums.GetForum(ForumId.Value).TopicCount;
            }
            if (IsAdministrator || IsForumModerator)
            {
                ddlShowTopicDays.Items.Add(new ListItem("Unmoderated Posts", "999"));
            }
            if (stickystate.Value != "")
                Collapsed = stickystate.Value;
            
            //_topicPager = (GridPager)LoadControl("~/UserControls/GridPager.ascx");
            //_topicPager.PagerStyle = Enumerators.PagerType.Linkbutton;
            //_topicPager.UserControlLinkClick += PagerLinkClick;
            //_topicPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);

            topicUPD.Triggers.Add(new AsyncPostBackTrigger {ControlID = ddlShowTopicDays.UniqueID, EventName="SelectedIndexChanged"});
            if (Request.QueryString["ARCHIVE"] != null)
            {
                if (Request.QueryString["ARCHIVE"] == "1")
                {
                    TopicODS.TypeName = "Snitz.BLL.Archive";
                    _archiveView = 1;
                }
            }
            else
            {
                TopicODS.TypeName = "Snitz.BLL.Forums";
                _archiveView = 0;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentPage == -1)
                CurrentPage = 0;

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

                populate = PopulateData;
                
                //phPager.Controls.Add(_topicPager);

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
                    //_topicPager.CurrentIndex = Int32.Parse(Request.Params["whichpage"]) - 1;
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
            if (Page.IsPostBack)
            {
                string postbackbtn = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];
                int id;
                switch (postbackbtn)
                {
                    case "LockTopic":
                        id = Convert.ToInt32(argument);
                        LockTopic(id);
                        break;
                    case "UnLockTopic":
                        id = Convert.ToInt32(argument);
                        UnLockTopic(id);
                        break;
                    case "TopicSubscribe":
                        id = Convert.ToInt32(argument);
                        TopicSubscribe(id);
                        break;
                    case "TopicUnSubscribe":
                        id = Convert.ToInt32(argument);
                        TopicUnSubscribe(id);
                        break;
                    case "DeleteTopic":
                        id = Convert.ToInt32(argument);
                        DeleteTopic(id);
                        break;
                    case "StickTopic":
                        id = Convert.ToInt32(argument);
                        StickTopic(id);
                        break;
                    case "UnStickTopic":
                        id = Convert.ToInt32(argument);
                        UnStickTopic(id);
                        break;
                }
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


        }

        private void PopulateData(int myint)
        {
            //RowCount = Forums.GetForum(ForumId.Value).TopicCount;
            //_topicPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
            CurrentPage = myint;
            ForumTable.PageIndex = CurrentPage;
            topicUPD.Update();
        }

        private void PagerLinkClick(object sender, EventArgs e)
        {
            var lnk = sender as LinkButton;
            
            if (lnk != null)
            {
                if (lnk.Text.IsNumeric())
                    CurrentPage = int.Parse(lnk.Text) - 1;
                else
                {
                    if (lnk.Text.Contains("&gt;"))
                        CurrentPage += 1;
                    else if (lnk.Text.Contains("&lt;"))
                        CurrentPage -= 1;
                    else if (lnk.Text.Contains("&raquo;"))
                        CurrentPage = _replyPager.PageCount - 1;
                    else
                        CurrentPage = 0;
                }
                if (CurrentPage < 0)
                    CurrentPage = 0;
                if (CurrentPage >= _replyPager.PageCount)
                    CurrentPage = _replyPager.PageCount - 1;
            }
            _replyPager.CurrentIndex = CurrentPage;
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
                InvalidateCache();
                ForumTable.PageIndex = 0;
                CurrentPage = 0;
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
                var lastpostdate = e.Row.Cells[5].FindControl("lastpostdate") as Literal;
                var lastreadpost = e.Row.Cells[5].FindControl("lastreadJump") as HyperLink;

                if(popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = topic.LastPostAuthorId != null ? Regex.Replace(topic.LastPostAuthorPopup, @"\[!(.*)!]", title) : "";
                }
                if (lastpostdate != null)
                {
                    lastpostdate.Text = Common.TimeAgoTag(((TopicInfo) e.Row.DataItem).LastPostDate, IsAuthenticated,
                        Member == null ? Config.TimeAdjust : Member.TimeOffset);
                }
                if (lastreadpost != null)
                {
                    int lastpage = TopicTracker.LastTopicPage(topic.Id, HttpContext.Current);
                    if (lastpage > 0)
                    {
                        lastreadpost.NavigateUrl = String.Format("/Content/Forums/topic.aspx?TOPIC={0}&whichpage={1}", topic.Id, lastpage+1);
                        lastreadpost.Visible = true;
                    }

                }
                if(subscribe != null)
                {
                    subscribe.Visible = IsAuthenticated;
                    subscribe.Visible = subscribe.Visible && topic.AllowSubscriptions;
                    subscribe.OnClientClick = "setArgAndPostBack('Do you want to be notified when someone posts a reply?','TopicSubscribe'," + topic.Id + ");return false;";
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
                    unsubscribe.OnClientClick = "setArgAndPostBack('Do you want to remove notifications from topic?','TopicUnSubscribe'," + topic.Id + ");return false;";
                }
                if (stickyIcon != null)
                {
                    stickyIcon.Visible = ((IsAdministrator || IsForumModerator) && (!topic.IsSticky) && _archiveView != 1);
                    stickyIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to make the Topic sticky?','StickTopic'," + topic.Id + ");return false;";
                }
                if (unstickyIcon != null)
                {
                    unstickyIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.IsSticky) && _archiveView != 1);
                    unstickyIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to un-stick the Topic?','UnStickTopic'," + topic.Id + ");return false;";
                }
                if (lockIcon != null)
                {
                    lockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status != (int)Enumerators.PostStatus.Closed) && _archiveView != 1);
                    lockIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to lock the Topic?','LockTopic'," + topic.Id + ");return false;";
                }
                if (unlockIcon != null)
                {
                    unlockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status == (int)Enumerators.PostStatus.Closed) && _archiveView != 1);
                    unlockIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to unlock the Topic?','UnLockTopic'," + topic.Id + ");return false;";
                }
                if (replyIcon != null)
                    replyIcon.NavigateUrl = "/Content/Forums/post.aspx?method=reply&TOPIC_ID=" + topic.Id;
                if (noArchiveIcon != null)
                {
                    noArchiveIcon.Visible = IsAdministrator && (Topics.IsArchived(topic)) && _archiveView != 1;
                    noArchiveIcon.NavigateUrl = "javascript:openConfirmDialog('pop_archive.aspx?archive=0&ID=" + topic.Id + "')";
                }
                if (archiveIcon != null)
                {
                    archiveIcon.Visible = IsAdministrator && (Topics.AllowArchive(topic)) && _archiveView != 1;
                    archiveIcon.NavigateUrl = "javascript:openConfirmDialog('pop_archive.aspx?archive=1&ID=" + topic.Id + "')";
                }
                if (delIcon != null)
                {
                    delIcon.Visible = false;
                    delIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to delete the Topic?','DeleteTopic'," + topic.Id + ");return false;";
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
                    if (replyIcon != null) replyIcon.Visible = !topic.IsArchived;
                    if (delIcon != null) delIcon.Visible = true;
                    if (editIcon != null) editIcon.Visible = true;
                }
                else if (topic.Status == (int)Enumerators.PostStatus.Closed || topic.Forum.Status == (int)Enumerators.PostStatus.Closed || topic.IsArchived)
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
                _replyPager.UpdateIndex = populate;
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
            if (topic.LastPostDate > LastVisitDateTime)
            {
                image.AlternateText = webResources.lblNewPosts;
                image.ToolTip = webResources.lblNewPosts;
                _new = "New";
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

            image.GenerateEmptyAlternateText = true;
            image.ApplyStyleSheetSkin(Page);
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
                if (_archiveView == 1)
                {
                    tempNode.Title += " (Archived)";
                }                
                else if (_currentForum.Status == (int)Enumerators.PostStatus.Closed)
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
            if (CurrentPage == PreviousPage)
            {
                if (IsPostBack && CurrentPage != 0)
                    e.Cancel = true;
            }
            else
            {
                PreviousPage = CurrentPage;
            }
        }

        protected void TopicOdsSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            if (_bGetSelectCount)
            {
                RowCount = (int)e.ReturnValue;
                //_topicPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
                if (CurrentPage != ForumTable.PageIndex)
                    CurrentPage = ForumTable.PageIndex;
            }
        }

        private void DeleteTopic(int topicid)
        {
            var lbl = UpdateProgress1.FindControl("lblProgress");
            if (lbl != null)
                ((Label) lbl).Text = "Deleting Topic";

            Topics.Delete(topicid);
            InvalidateCache();
            ForumTable.DataBind();
        }

        private void LockTopic(int topicid)
        {
            Topics.SetTopicStatus(topicid, (int)Enumerators.PostStatus.Closed);
            InvalidateCache();
            ForumTable.DataBind();
        }

        private void UnLockTopic(int topicid)
        {
            Topics.SetTopicStatus(topicid, (int)Enumerators.PostStatus.Open);
            InvalidateCache();
            ForumTable.DataBind();
        }

        private void StickTopic(int topicid)
        {
            Topics.MakeSticky(topicid, true);
            InvalidateCache();
            ForumTable.DataBind();
        }

        private void UnStickTopic(int topicid)
        {
            Topics.MakeSticky(topicid, false);
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

        private void TopicSubscribe(int topicid)
        {
            Subscriptions.AddTopicSubscription(Member.Id, topicid);
        }
        protected void TopicUnSubscribe(int topicid)
        {
            Subscriptions.RemoveTopicSubscription(Member.Id, topicid);
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