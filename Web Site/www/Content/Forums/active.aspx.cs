/*
####################################################################################################################
##
## SnitzUI.Content.Forums - active.aspx
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

using Image = System.Web.UI.WebControls.Image;
using Polls = Resources.Polls;

public partial class ActiveTopicPage : PageBase
    {
        bool _bGetSelectCount;
        private GridPager _replyPager;
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
        private readonly string _currentUser = HttpContext.Current.User.Identity.Name;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            Session.Timeout = 20;

            ActiveTable.PageSize = Config.TopicPageSize;
            if (!Page.IsPostBack)
            {
                var TopicSinceIndex = SnitzCookie.GetTopicSince();
                var refreshIndex = SnitzCookie.GetActiveRefresh();

                if (TopicSinceIndex != null)
                    ddlTopicsSince.SelectedIndex = Int32.Parse(TopicSinceIndex);
                if (refreshIndex != null)
                {
                    ddlPageRefresh.SelectedIndex = Int32.Parse(refreshIndex);
                    if (ddlPageRefresh.SelectedValue != "")
                    {
                        int reloadTime = 60000 * Convert.ToInt32(ddlPageRefresh.SelectedValue);
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "refresh", "setRefresh('" + reloadTime + "');", true);
                    }
                }
            }
            string pagedescription = String.Format("{0}:{1}", Config.ForumTitle, webResources.ttlActivePage);
            metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", pagedescription.Substring(0, Math.Min(160, pagedescription.Length)));
            
            TopicUpdatePanel.Triggers.Add(new AsyncPostBackTrigger {ControlID = ddlPageRefresh.UniqueID});
            TopicUpdatePanel.Triggers.Add(new AsyncPostBackTrigger { ControlID = ddlTopicsSince.UniqueID });
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            // Determine the index and HeaderText of the column that        
            //the data is sorted by        

            CatForumGrouping();

            base.Render(writer);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                    case "TopicSubscribe" :
                        id = Convert.ToInt32(argument);
                        TopicSubscribe(id);
                        break;
                    case "TopicUnSubscribe":
                        id = Convert.ToInt32(argument);
                        TopicUnSubscribe(id);
                        break;
                    case "DeleteTopic" :
                        id = Convert.ToInt32(argument);
                        DeleteTopic(id);
                        break;
                }
            }
            if (!Page.IsPostBack)
            {
                hdnLastOpened.Value = DateTime.UtcNow.ToForumDateStr();
                Session["FORUM"] = "";
                lblHotTopic.Text = string.Format(webResources.lblHotTopics, Config.HotTopicNum);
            }
            if (ddlPageRefresh.SelectedIndex == 0)
            {
                SnitzCookie.SetActiveRefresh("0");
                ScriptManager.RegisterStartupScript(this, GetType(), "clearrefresh", "cancelRefresh();", true);
            }
            BindActiveTopics();

        }

        protected void ActiveTopicOdsSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            _bGetSelectCount = e.ExecutingSelectCount;
        }

        protected void ActiveTopicOdsSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {

            if (_bGetSelectCount)
            {
                RowCount = (int)e.ReturnValue;
                if (CurrentPage != ActiveTable.PageIndex)
                    CurrentPage = ActiveTable.PageIndex;
            }

        }

        protected void ActiveTableRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {

                var markRead = new ImageButton
                                   {
                    SkinID = "MarkRead",
                    ToolTip = webResources.lblMarkAllRead
                };
                markRead.ApplyStyleSheetSkin(Page);
                markRead.Click += MarkReadClick;
                e.Row.Cells[0].Controls.Add(markRead);
                if (_currentUser == "")
                {
                    e.Row.Cells.RemoveAt(6);
                }

            }
            else if (e.Row.RowType == DataControlRowType.Pager)
            {
                _replyPager = (GridPager)e.Row.FindControl("pager");
                _replyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.TopicPageSize);
                _replyPager.CurrentIndex = CurrentPage;
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var topic = (TopicInfo)e.Row.DataItem;
                var lockIcon = e.Row.Cells[6].FindControl("TopicLock") as ImageButton;
                var unlockIcon = e.Row.Cells[6].FindControl("TopicUnLock") as ImageButton;
                var delIcon = e.Row.Cells[6].FindControl("TopicDelete") as ImageButton;
                var subscribe = e.Row.Cells[6].FindControl("TopicSub") as ImageButton;
                var unsubscribe = e.Row.Cells[6].FindControl("TopicUnSub") as ImageButton;
                var approve = e.Row.Cells[6].FindControl("TopicApprove") as ImageButton;
                var editIcon = e.Row.Cells[6].FindControl("hypEditTopic") as HyperLink;
                var replyIcon = e.Row.Cells[6].FindControl("hypReplyTopic") as HyperLink;
                var newIcon = e.Row.Cells[6].FindControl("hypNewTopic") as HyperLink;
                var popuplink = e.Row.Cells[5].FindControl("popuplink") as Literal;
                var lastpost = e.Row.Cells[5].FindControl("lastpostdate") as Literal;
                
                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = topic.LastPostAuthorId != null ? Regex.Replace(topic.LastPostAuthorPopup, @"\[!(.*)!]", title) : "";
                }
                if (lastpost != null)
                {
                    lastpost.Text = SnitzTime.TimeAgoTag(((TopicInfo) e.Row.DataItem).LastPostDate, IsAuthenticated,Member);
                    //Common.TimeAgoTag(((TopicInfo)e.Row.DataItem).LastPostDate, IsAuthenticated,
                    //                        (Member == null ? Config.TimeAdjust : Member.TimeOffset));
                }
                int replyCount = topic.ReplyCount;
                int topicId = topic.Id;
                int forumId = topic.ForumId;
                int catId = topic.CatId;
                string authorName = topic.AuthorName;
                bool inModeratedList = Moderators.IsUserForumModerator(_currentUser, forumId);

                if (lockIcon != null)
                {
                    lockIcon.Visible = (IsAdministrator || inModeratedList);
                    lockIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to lock the Topic?','LockTopic'," + topicId + ");return false;";
                }
                if (delIcon != null)
                {
                    delIcon.Visible = false;
                    delIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to delete the Topic?','DeleteTopic'," + topicId + ");return false;";
                }
                if (editIcon != null) editIcon.Visible = false;
                if (approve != null)
                {
                    approve.Visible = false;
                    if (topic.Status == (int)Enumerators.PostStatus.UnModerated || topic.Status == (int)Enumerators.PostStatus.OnHold)
                        approve.Visible = (inModeratedList || IsAdministrator);
                    approve.OnClientClick = string.Format("mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0},{1}'}}, 'methodHandlers.BeginRecieve');return false;",
                        true, topic.Id);
                }
                if (subscribe != null)
                {
                    subscribe.Visible = IsAuthenticated;
                    if (IsAuthenticated)
                    {
                        topic.Forum = Forums.GetForum(topic.ForumId);
                    }
                    subscribe.Visible = subscribe.Visible && topic.AllowSubscriptions;
                    subscribe.OnClientClick =
                        "setArgAndPostBack('Do you want to be notified when someone posts a reply?','TopicSubscribe'," + topicId + ");return false;";
                }
                if (unsubscribe != null)
                {
                    unsubscribe.Visible = false;
                    if (subscribe != null && subscribe.Visible)
                    {
                        if (Members.IsSubscribedToTopic(topic.Id, Member == null ? 0 : Member.Id))
                        {
                            subscribe.Visible = false;
                            unsubscribe.Visible = true;
                        }
                    }
                    unsubscribe.OnClientClick =
                        "setArgAndPostBack('Do you want to remove notifications from topic?','TopicUnSubscribe'," + topicId + ");return false;";
                }

                e.Row.Cells[0].Controls.Add(GetRecentTopicIcon(topic, replyCount));

                if (newIcon != null) newIcon.Visible = false;
                if (lockIcon != null)
                {
                    lockIcon.Visible = ((IsAdministrator || inModeratedList) && (topic.Status != (int)Enumerators.PostStatus.Closed));
                }
                if (unlockIcon != null)
                {
                    unlockIcon.Visible = ((IsAdministrator || inModeratedList) && (topic.Status == (int)Enumerators.PostStatus.Closed));
                    unlockIcon.OnClientClick =
                        "setArgAndPostBack('Do you want to unlock the Topic?','UnLockTopic'," + topicId + ");return false;";
                }

                if (replyIcon != null)
                    replyIcon.NavigateUrl = string.Format("/Content/Forums/post.aspx?method=reply&TOPIC={0}&FORUM={1}&CAT={2}", topicId, forumId, catId);
                if (topic.Status == (int)Enumerators.PostStatus.Closed || !IsAuthenticated)
                    if (replyIcon != null) replyIcon.Visible = false;
                if (IsAdministrator || inModeratedList)
                    if (replyIcon != null) replyIcon.Visible = true;
                if (IsAdministrator || inModeratedList)
                {
                    if (delIcon != null) delIcon.Visible = true;
                    if (editIcon != null) editIcon.Visible = true;
                }
                else if (_currentUser.ToLower() == authorName.ToLower())
                {
                    if (replyCount == 0)
                        if (delIcon != null) delIcon.Visible = true;
                    if (editIcon != null) editIcon.Visible = true;
                }
                if (editIcon != null)
                    editIcon.NavigateUrl = string.Format("/Content/Forums/post.aspx?method=edit&type=TOPICS&id={0}&FORUM={1}&CAT={2}", topicId, forumId, catId);

                if (_currentUser == "")
                {
                    if (e.Row.Cells.Count > 2)
                    {
                        e.Row.Cells.RemoveAt(6);
                        //e.Row.Cells[5].ColumnSpan +=1;
                    }

                }
            }
        }
        
        protected void DdlTopicsSinceSelectedIndexChanged(object sender, EventArgs e)
        {
            SnitzCookie.SetTopicSince(ddlTopicsSince.SelectedIndex.ToString());
        }

        protected void DdlPageRefreshSelectedIndexChanged(object sender, EventArgs e)
        {
            //store the variable in a cookie.
            SnitzCookie.SetActiveRefresh(ddlPageRefresh.SelectedIndex.ToString());

            if (ddlPageRefresh.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "clearrefresh", "cancelRefresh();", true);

            }
            else if (ddlPageRefresh.SelectedValue != null)
            {
                int reloadTime = 60000 * Convert.ToInt32(ddlPageRefresh.SelectedValue);
                if (reloadTime > 0)
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "refresh", "setRefresh('" + reloadTime + "');", true);
            }
        }

        private void BindActiveTopics()
        {

            DateTime newdate;
            if ((_currentUser != ""))
            {
                if (Member != null)
                {
                    newdate = LastVisitDateTime;
                    ddlTopicsSince.Items[0].Text += string.Format(@" {0}", Regex.Replace(newdate.ToForumDateDisplay(" ", true, IsAuthenticated, Member), @"(<.*?>)", ""));
                }
            }
            else
            {
                if (!Page.IsPostBack)
                {
                    ddlTopicsSince.Items[12].Selected = true;
                    ddlTopicsSince.SelectedIndex = 12;
                }
            }
            int ddlIndex = ddlTopicsSince.SelectedIndex;
            switch (ddlIndex)
            {
                case 0:
                    Session["_SinceDate"] = LastVisitDateTime.ToForumDateStr();
                    break;
                case 1: //15 minutes
                    newdate = DateTime.UtcNow.AddMinutes(-15);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 2: //30 minutes
                    newdate = DateTime.UtcNow.AddMinutes(-30);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 3: //45 minutes
                    newdate = DateTime.UtcNow.AddMinutes(-45);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 4: // 1 hour
                    newdate = DateTime.UtcNow.AddHours(-1);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 5: // 2 hours
                    newdate = DateTime.UtcNow.AddHours(-2);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 6: // 6 hours
                    newdate = DateTime.UtcNow.AddHours(-6);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 7: // 12 hours
                    newdate = DateTime.UtcNow.AddHours(-12);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 8: //yesterday
                    newdate = DateTime.UtcNow.AddDays(-1);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 9: // 2 days
                    newdate = DateTime.UtcNow.AddDays(-2);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 10: // 1 week
                    newdate = DateTime.UtcNow.AddDays(-7);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 11: // 2 weeks
                    newdate = DateTime.UtcNow.AddDays(-14);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 12: // 1 month
                    newdate = DateTime.UtcNow.AddMonths(-1);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 13: // 2 months
                    newdate = DateTime.UtcNow.AddMonths(-2);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                case 14: // 2 months
                    newdate = DateTime.UtcNow.AddYears(-2);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
                default: // 1 month
                    newdate = DateTime.UtcNow.AddMonths(-1);
                    Session["_SinceDate"] = newdate.ToForumDateStr();
                    break;
            }

        }

        private void CatForumGrouping()
        {
            const int catColumnIndex = 1;
            const int forumColumnIndex = 2;

            // Reference the Table the GridView has been rendered into        
            var gridTable = (Table)ActiveTable.Controls[0];
            // Enumerate each TableRow, adding a sorting UI header if        
            // the sorted value has changed        
            string lastCat = string.Empty;
            string lastForum = string.Empty;
            foreach (GridViewRow gvr in ActiveTable.Rows)
            {
                string currentCat = gvr.Cells[catColumnIndex].Text;
                string currentForum = gvr.Cells[forumColumnIndex].Text;
                
                if (lastCat.CompareTo(currentCat) != 0)
                {
                    CategoryInfo category = Categories.GetCategory(Convert.ToInt32(currentCat));
                    string catLink = String.Format("<a href=\"/default.aspx?CAT={0}\" title=\"{1}\">{1}</a>", category.Id,
                                                   category.Name);
                    
                    // there's been a change in value in the category column                
                    int rowIndex = gridTable.Rows.GetRowIndex(gvr);
                    // Add a new category header row                
                    var sortRow = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
                    var sortCell = new TableCell
                                             {
                                                 Text = catLink,
                                                 CssClass = "tableheader"
                                             };
                    if (IsAuthenticated)
                        sortCell.ColumnSpan = ActiveTable.Columns.Count;
                    else
                        sortCell.ColumnSpan = ActiveTable.Columns.Count - 1;
                    sortRow.Cells.Add(sortCell);
                    gridTable.Controls.AddAt(rowIndex, sortRow);
                    // Update lastValue                
                    lastCat = currentCat;
                }
                if (lastForum.CompareTo(currentForum) != 0)
                {
                    ForumInfo forum = Forums.GetForum(Convert.ToInt32(currentForum));
                    string forumLink = String.Format("<a class=\"bbcode\" href=\"/Content/Forums/forum.aspx?FORUM={0}\" title=\"{1}\">{2}</a>", forum.Id,
                                                    forum.Subject, forum.Subject);
                    // there's been a change in value in the forum column                
                    int rowIndex = gridTable.Rows.GetRowIndex(gvr);
                    // Add a new forum header row                
                    var sortRow = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
                    var spacer = new TableCell { Text = "", CssClass = " ForumHeaderRow iconCol" };
                    var sortCell = new TableCell
                                             {
                                                 
                                                 Text = forumLink,
                                                 CssClass = "ForumHeaderRow"
                                             };
                    if (IsAuthenticated)
                        sortCell.ColumnSpan = ActiveTable.Columns.Count - 1;
                    else
                        sortCell.ColumnSpan = ActiveTable.Columns.Count - 2;
                    sortRow.Cells.Add(spacer);
                    sortRow.Cells.Add(sortCell);
                    gridTable.Controls.AddAt(rowIndex, sortRow);
                    // Update lastValue                
                    lastForum = currentForum;
                }

                gvr.Cells.RemoveAt(2);
                gvr.Cells.RemoveAt(1);
                gvr.Cells[0].ColumnSpan = 3;
                if(_currentUser == "")
                {
                    gvr.Cells.RemoveAt(5);
                    gvr.Cells[4].ColumnSpan = 2;
                }
            }
            if (ActiveTable.BottomPagerRow != null)
            {
                GridViewRow pagerRow = ActiveTable.BottomPagerRow;
                pagerRow.Cells[0].CssClass = "pagerCol";
                //pagerRow.Cells[0].Attributes.Add("colspan",(ActiveTable.Columns.Count-1).ToString());
                pagerRow.Cells[0].Attributes.Add("colspan",
                                                 IsAuthenticated
                                                     ? ActiveTable.Columns.Count.ToString()
                                                     : (ActiveTable.Columns.Count - 1).ToString());
            }
        }

        private Image GetRecentTopicIcon(TopicInfo topic, int tReplies)
        {
            var image = new Image { ID = "postIcon", EnableViewState = false };
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
            image.SkinID = "Folder" + _new + hot + sticky + locked;

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

        protected static string TopicPageLinks(object replyCount, object topicId)
        {
            string retVal = "";

            if (!Config.ShowPaging)
                return retVal;

            if ((int)replyCount > Config.TopicPageSize)
            {
                int pageNum = (int)replyCount / Config.TopicPageSize;
                int rem = (int)replyCount % Config.TopicPageSize;
                if (rem > 0) pageNum += 1;

                for (int x = 1; x < pageNum + 1; x++)
                    retVal += "<a href='/Content/Forums/topic.aspx?TOPIC=" + topicId + "&whichpage=" + x + "'><span class='topicPageLnk' >" + x + "</span></a> ";
            }
            return retVal;
        }

        private void MarkReadClick(object sender, ImageClickEventArgs e)
        {
            if (IsAuthenticated)
            {
                if (Session["_LastVisit"] == null)
                {
                    HttpContext.Current.Session.Add("_LastVisit", hdnLastOpened.Value);
                }
                else
                {
                    Session["_LastVisit"] = hdnLastOpened.Value;
                    Session["_LastVisit"] = hdnLastOpened.Value;
                }
                var since = SnitzCookie.GetLastVisitDate();
                if (since != null)
                {
                    SnitzCookie.SetLastVisitCookie("0");
                }

                Response.Redirect(Request.RawUrl);
            }
            else
            {
                Session["_LastVisit"] = DateTime.UtcNow.ToForumDateStr();
            }
        }

        protected void DeleteTopic(int topicid)
        {
            Topics.Delete(topicid);
            Response.Redirect(Request.RawUrl);
        }

        protected void LockTopic(int topicid)
        {
            Topics.SetTopicStatus(topicid, (int)Enumerators.PostStatus.Closed);
            Response.Redirect(Request.RawUrl);
        }

        protected void UnLockTopic(int topicid)
        {
            Topics.SetTopicStatus(topicid, (int)Enumerators.PostStatus.Open);
            Response.Redirect(Request.RawUrl);
        }

        protected void TopicSubscribe(int topicid)
        {
            Subscriptions.AddTopicSubscription(Member == null ? 0 : Member.Id, topicid);
        }
        protected void TopicUnSubscribe(int topicid)
        {
            Subscriptions.RemoveTopicSubscription(Member == null ? 0 : Member.Id, topicid);
        }
    }