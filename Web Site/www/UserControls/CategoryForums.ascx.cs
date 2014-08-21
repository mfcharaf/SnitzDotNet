/*
####################################################################################################################
##
## SnitzUI.UserControls - CategoryForums.ascx
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		04/08/2013
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
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using Image = System.Web.UI.WebControls.Image;

namespace SnitzUI.UserControls
{
    public partial class CategoryForums : System.Web.UI.UserControl
    {

        public bool IsAdministrator
        {
            get
            {
                if (HttpContext.Current.Session != null)
                    if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        bool check = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");
                        return check;
                    }
                return false;
            }
        }
        public DateTime LastVisitDateTime
        {
            get
            {
                if (Session["_LastVisit"] != null)
                {
                    string lastvisit = Session["_LastVisit"].ToString();

                    var dateTime = lastvisit.ToDateTime();
                    if (dateTime != null) return dateTime.Value;
                }
                return DateTime.UtcNow;
            }
        }

        protected bool IsAuthenticated
        {
            get { return HttpContext.Current.User.Identity.IsAuthenticated; }
        }

        public MemberInfo Member { get; set; }

        public string CategoryId { get; set; }

        private int _categoryId;

        protected void Page_Load(object sender, EventArgs e)
        {
            _categoryId = Convert.ToInt32(CategoryId);
            repForum.DataSource = Categories.GetCategoryForums(_categoryId,Member);
            repForum.DataBind();
        }

        protected void RepForumItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            string imagedir = Config.ImageDirectory;

            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {

                var forum = (ForumInfo)item.DataItem;

                var lockIcon = item.FindControl("ForumLock") as ImageButton;
                var unlockIcon = item.FindControl("ForumUnLock") as ImageButton;
                var delIcon = item.FindControl("ForumDelete") as ImageButton;
                var editIcon = item.FindControl("ForumEdit") as ImageButton;
                var subscribe = item.FindControl("ForumSub") as ImageButton;
                var unsubscribe = item.FindControl("ForumUnSub") as ImageButton;
                var empty = item.FindControl("ForumEmpty") as ImageButton;
                var archive = item.FindControl("ArchiveForum") as ImageButton;
                var newIcon = item.FindControl("hypNewTopic") as HyperLink;
                var viewarchive = item.FindControl("hypViewArchive") as HyperLink;

                var popuplink = item.FindControl("popuplink") as Literal;
                var ldate = (Literal)item.FindControl("lDate");
                var iconPh = (PlaceHolder)item.FindControl("Ticons");
                var imgLastPost = (Image) item.FindControl("imgLastPost");
                var repeater = (RepeaterItem)item.Parent.Controls[0];
                var header = repeater.FindControl("fTableHeader");
                if (imgLastPost != null)
                {
                    imgLastPost.ImageUrl = imagedir + "page_go.png";
                }
                if (forum.Type == 1) //External link
                {
                    header.Visible = false;
                    var link = (HyperLink)item.FindControl("forumLink");
                    var linkcol = (HtmlTableCell)item.FindControl("linkCol");
                    var tCount = (HtmlTableCell)item.FindControl("tCount");
                    var pCount = (HtmlTableCell)item.FindControl("pCount");
                    var lastpost = (HtmlTableCell)item.FindControl("lastpost");
                    var buttonCol = (HtmlTableCell)item.FindControl("adminBtn");

                    link.NavigateUrl = forum.Url;
                    link.Target = "_blank";
                    linkcol.ColSpan = 4;
                    tCount.Visible = false;
                    pCount.Visible = false;
                    lastpost.Visible = false;
                    archive.Visible = false;

                    if (iconPh != null)
                    {
                        var img = new Image { ID = "imgTopicIcon", ImageUrl = imagedir + "message/weblink.png", AlternateText = "", EnableViewState = false };
                        //img.ApplyStyleSheetSkin(page);
                        iconPh.Controls.Add(img);
                    }
                    if (!IsAuthenticated)
                    {
                        buttonCol.Visible = false;
                        linkcol.ColSpan = 5;
                        return;
                    }

                }

                header.Visible = true;

                if (ldate != null)
                {
                    var offset = 0.0;
                    if (Member != null)
                        offset = Member.TimeOffset;
                    if (forum.LastPostDate.HasValue && !(forum.LastPostDate.Value == DateTime.MinValue))
                        ldate.Text = SnitzTime.TimeAgoTag(forum.LastPostDate.Value, IsAuthenticated, Member);
                    //Common.TimeAgoTag(forum.LastPostDate.Value, IsAuthenticated, offset);
                }
                bool isForumModerator = Moderators.IsUserForumModerator(HttpContext.Current.User.Identity.Name, forum.Id);


                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = forum.LastPostAuthorId != null ? Regex.Replace(forum.LastPostAuthorPopup, @"\[!(.*)!]", title) : "";
                }
                if (archive != null)
                {
                    archive.Visible = archive.Visible && (forum.Type != 1) && Config.Archive;
                    archive.ImageUrl = imagedir + "/admin/archive.png";
                    archive.OnClientClick =
                         "setArgAndPostBack('Do you want to archive posts in this Forum?','ForumArchive'," + forum.Id + ");return false;";
                    
                }
                
                if (IsAuthenticated)
                {
                    if (subscribe != null)
                    {
                        subscribe.ImageUrl = imagedir + "/admin/subscribe.png";
                        subscribe.Visible = IsAuthenticated &&
                                            forum.SubscriptionLevel == (int)Enumerators.Subscription.ForumSubscription;
                        subscribe.Visible = subscribe.Visible && (forum.Type != 1);
                        subscribe.OnClientClick =
                             "setArgAndPostBack('Do you want to subscribe to new posts in the Forum?','ForumSubscribe'," + forum.Id + ");return false;";
                    }
                    if (unsubscribe != null)
                    {
                        unsubscribe.Visible = false;
                        unsubscribe.ImageUrl = imagedir + "/admin/unsubscribe.png";
                        if (subscribe.Visible)
                        {
                            if (Members.IsSubscribedToForum(Member.Id, forum.Id))
                            {
                                subscribe.Visible = false;
                                unsubscribe.Visible = true;
                            }
                        }
                        unsubscribe.Visible = unsubscribe.Visible && (forum.Type != 1);
                        unsubscribe.OnClientClick =
                            "setArgAndPostBack('Do you want to remove your subscription for this Forum?','ForumUnSubscribe'," + forum.Id + ");return false;";
                    }
                    if (newIcon != null)
                    {
                        newIcon.Visible = IsAuthenticated;
                        newIcon.ImageUrl = imagedir + "/admin/document.png";
                        newIcon.Visible = newIcon.Visible && forum.Status != (int)Enumerators.PostStatus.Closed;
                        newIcon.Visible = newIcon.Visible || (IsAdministrator || isForumModerator);
                        newIcon.NavigateUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}",
                                                            forum.Id, forum.CatId);
                        newIcon.Visible = newIcon.Visible && (forum.Type != 1);
                    }
                    if (viewarchive != null)
                    {
                        viewarchive.ImageUrl = imagedir + "/admin/newwindow.png";
                        viewarchive.Visible = forum.ArchivedTopicCount > 0;
                        viewarchive.NavigateUrl = string.Format("~/Content/Forums/forum.aspx?FORUM={0}&ARCHIVE=1",forum.Id);
                        viewarchive.Visible = viewarchive.Visible && (forum.Type != 1);                        
                    }
                    if (lockIcon != null)
                    {
                        lockIcon.ImageUrl = imagedir + "/admin/lock.png";
                        lockIcon.Visible = ((IsAdministrator || isForumModerator) && (forum.Status == (int)Enumerators.PostStatus.Open));
                        lockIcon.Visible = lockIcon.Visible && (forum.Type != 1);
                        lockIcon.OnClientClick =
                            "setArgAndPostBack('Do you want to lock the Forum?','ForumLock'," + forum.Id + ");return false;";
                    }
                    if (unlockIcon != null)
                    {
                        unlockIcon.ImageUrl = imagedir + "/admin/unlock.png";
                        unlockIcon.Visible = ((IsAdministrator || isForumModerator) &&
                                              (forum.Status == (int)Enumerators.PostStatus.Closed));
                        unlockIcon.Visible = unlockIcon.Visible && (forum.Type != 1);
                        unlockIcon.OnClientClick =
                            "setArgAndPostBack('Do you want to unlock the Forum?','ForumUnLock'," + forum.Id + ");return false;";
                    }
                    if (delIcon != null)
                    {
                        delIcon.ImageUrl = imagedir + "/admin/trash.png";
                        delIcon.Visible = IsAdministrator;
                        delIcon.OnClientClick = "setArgAndPostBack('Do you want to delete the Forum?','ForumDelete'," + forum.Id + ");return false;";
                    }
                    if (editIcon != null)
                    {
                        editIcon.ImageUrl = imagedir + "/admin/properties.png";
                        editIcon.OnClientClick = string.Format(
                            "mainScreen.LoadServerControlHtml('Edit Properties',{{'pageID':8,'data':'{0},{1},{2}'}}, 'methodHandlers.BeginRecieve');return false;",
                            forum.Id, forum.CatId, forum.Type);
                        editIcon.Visible = (IsAdministrator || isForumModerator);
                    }
                    if (empty != null)
                    {
                        empty.ImageUrl = imagedir + "/admin/folderX.png";
                        empty.Visible = empty.Visible && (forum.Type != 1);
                        empty.OnClientClick =
                            "setArgAndPostBack('Do you want to delete all the posts in the Forum?','ForumEmpty'," + forum.Id + ");return false;";
                    }
                }
                else
                {
                    var buttonCol = (HtmlTableCell)item.FindControl("adminBtn");
                    var lastpost = (HtmlTableCell)item.FindControl("lastpost");
                    buttonCol.Visible = false;
                    lastpost.ColSpan = 2;
                }
            }
        }
        private static Image GetForumIcon(string username, DateTime lasthere, Enumerators.PostStatus fStatus, DateTime? tLastPost)
        {
            var image = new Image { ID = "imgTopicIcon", EnableViewState = false };
            image.SkinID = "FolderNew";
            string imagedir = Config.ImageDirectory;
            switch (fStatus)
            {
                case Enumerators.PostStatus.Open:
                    image.AlternateText = webResources.lblOldPosts;
                    image.SkinID = "Folder";
                    image.ImageUrl = imagedir + "/folders/foldernoposts.png";
                    if (username != "")
                        if (tLastPost > lasthere)
                        {
                            image.SkinID = "FolderNew";
                            image.ImageUrl = imagedir + "/folders/foldernewposts.png";
                            image.AlternateText = webResources.lblNewPosts;
                        }
                    break;
                default:
                    image.SkinID = "FolderLocked";
                    image.ImageUrl = imagedir + "/folders/folder_locked.png";
                    image.AlternateText = webResources.lblLockedForum;
                    break;
            }
            return image;
        }

        protected void repForum_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
            {
                var forum = (ForumInfo)e.Item.DataItem;
                var iconPh = (PlaceHolder)e.Item.FindControl("Ticons");
                if (iconPh != null && forum.Type != 1)
                {
                   var img =
                        GetForumIcon(HttpContext.Current.User.Identity.Name, LastVisitDateTime, (Enumerators.PostStatus)forum.Status,
                                     forum.LastPostDate);
                    img.EnableViewState = false;
                    img.ApplyStyleSheetSkin(this.Page);
                    
                    iconPh.Controls.Add(img);
                }
            }
        }



    }
}