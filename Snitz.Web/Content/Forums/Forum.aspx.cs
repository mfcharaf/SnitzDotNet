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
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;
using SnitzData;

namespace SnitzUI
{
    public partial class ForumPage : PageBase
    {
        bool _bGetSelectCount;
        private Forum _currentForum;
        public bool IsForumModerator;
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
                IsForumModerator = new SnitzRoleProvider().IsUserForumModerator(HttpContext.Current.User.Identity.Name, ForumId.Value);
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
                _currentForum = Util.GetForum(ForumId.Value);
                if (_currentForum == null) throw new ArgumentException("Invalid Forum ID");
                if(_currentForum.Type == 1)
                {
                    Response.Redirect(_currentForum.URL,true);
                }
                string session = "FORUM" + ForumId;
                fLogin.forum = _currentForum;
                if (!IsPostBack)
                {
                    if (IsAuthenticated)
                    {

                        //do we have access to this forum
                        if (!SnitzRoleProvider.IsUserInForumRole(Member.Name, ForumId.Value))
                        {
                            if (Session[session] == null || Session[session].ToString() != "true")
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
                                        if (Session[session].ToString() != "true")
                                            throw new SecurityException("You are not authorised to view this forum");
                                    }
                                }
                            }
                        }else
                        {
                            Session[session] = "true";
                        }
                    }

                }
                if (!IsAuthenticated && (_currentForum.Roles.Count > 0 && !_currentForum.Roles.Contains("All")))
                {
                    if (Session[session] == null || Session[session].ToString() != "true")
                        throw new SecurityException("You must be logged in to view this forum");
                }


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
            BindData();

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

        }

        private void BindData()
        {
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
                    //ForumTable.DataSource = _Forum.Topics.Where(t => t.Status == Enumerators.PostStatus.Open).ToList();
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
                    //ForumTable.DataSource = _Forum.Topics.Where(t => t.LastPostDate > (newdate - diff)).ToList();
                    break;
            }

            TopicODS.Select();

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
                var topic = (Topic) e.Row.DataItem;

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
                    popuplink.Text = topic.LastPostAuthor != null ? Regex.Replace(topic.LastPostAuthor.ProfilePopup, @"\[!(.*)!]", title) : "";
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
                        if(Member.IsSubscribedToTopic(topic.Id))
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
                    lockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status != Enumerators.PostStatus.Closed));
                }
                if (unlockIcon != null)
                {
                    unlockIcon.Visible = ((IsAdministrator || IsForumModerator) && (topic.Status == Enumerators.PostStatus.Closed));
                }
                if (replyIcon != null)
                    replyIcon.NavigateUrl = "/Content/Forums/post.aspx?method=reply&TOPIC_ID=" + topic.Id;
                if (noArchiveIcon != null)
                {
                    noArchiveIcon.Visible = false; // IsAdministrator && (topic.AllowArchive == 1);
                    noArchiveIcon.NavigateUrl = "javascript:openConfirmDialog('pop_archive.aspx?archive=0&ID=" + topic.Id + "')";
                }
                if (archiveIcon != null)
                {
                    archiveIcon.Visible = false;  //IsAdministrator && (topic.AllowArchive == 0);
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
                    if (topic.Status == Enumerators.PostStatus.UnModerated || topic.Status == Enumerators.PostStatus.OnHold)
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
                else if (topic.Status == Enumerators.PostStatus.Closed || topic.Forum.Status == Enumerators.PostStatus.Closed)
                {
                    if (replyIcon != null) replyIcon.Visible = false;
                    if (delIcon != null) delIcon.Visible = false;
                    if (editIcon != null) editIcon.Visible = false;
                }
                else if (currentUser.ToLower() == topic.Author.Name.ToLower())
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

        private Image GetTopicIcon(Topic topic)
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
            switch (topic.Status)
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

            if (topic.Status == Enumerators.PostStatus.UnModerated)
            {
                image.ToolTip = webResources.Unmoderatedpost;
                image.SkinID = "UnModerated";
            }
            if (topic.Status == Enumerators.PostStatus.OnHold)
            {
                image.ToolTip = webResources.TopicOnHold;
                image.SkinID = "OnHold";
            }
            if (topic.UnModeratedReplies > 0)
            {
                image.ToolTip = webResources.UnmoderatedPosts;
                image.SkinID = "UnmoderatedPosts";
            }
            if (topic.PollID > 0)
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
                if(_currentForum.ModerationLevel != Enumerators.Moderation.UnModerated)
                {
                    tempNode.Title += String.Format(" ({0})", EnumHelper.GetDescription(_currentForum.ModerationLevel));
                }
                if(_currentForum.Status == Enumerators.PostStatus.Closed)
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
            var btn = (ImageButton) sender;
            Util.DeleteTopic(Convert.ToInt32(btn.CommandArgument));
            ForumTable.DataBind();
        }

        protected void LockTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Util.SetTopicStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Closed);
            ForumTable.DataBind();
        }

        protected void UnLockTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Util.SetTopicStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Open);
            ForumTable.DataBind();
        }

        protected void StickTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Util.MakeSticky(Convert.ToInt32(btn.CommandArgument), true);
            ForumTable.DataBind();
        }

        protected void UnStickTopic(object sender, ImageClickEventArgs e)
        {
            var btn = (ImageButton)sender;
            Util.MakeSticky(Convert.ToInt32(btn.CommandArgument), false);
            ForumTable.DataBind();
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
                    Util.AddTopicSubscription(Member.Id, topicid);
                    break;
                case "unsub" :
                    Util.RemoveTopicSubscription(Member.Id, topicid);
                    break;
            }
            
        }


        [WebMethod]
        public static void Approval(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
            {
                int id = Convert.ToInt32(topicid);
                Topic topic = Util.GetTopic(id);

                Util.SetTopicStatus(id, Enumerators.PostStatus.Open);
                if(topic.Forum.SubscriptionLevel == (int)Enumerators.Subscription.ForumSubscription)
                    Util.ProcessSubscriptions(Enumerators.Subscription.ForumSubscription,topic,null);

            }
            if (!String.IsNullOrEmpty(replyid))
            {
                int id = Convert.ToInt32(replyid);
                Reply reply = Util.GetReply(id);
                Topic topic = Util.GetTopic(reply.TopicId);
                Util.SetReplyStatus(Convert.ToInt32(replyid), Enumerators.PostStatus.Open);
                if(topic.AllowSubscriptions)
                    Util.ProcessSubscriptions(Enumerators.Subscription.TopicSubscription,topic,reply);
            }

        }
        [WebMethod]
        public static void PutOnHold(string topicid, string replyid)
        {
            if (!String.IsNullOrEmpty(topicid))
                Util.SetTopicStatus(Convert.ToInt32(topicid), Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(replyid))
                Util.SetReplyStatus(Convert.ToInt32(replyid), Enumerators.PostStatus.OnHold);
        }

    }
}