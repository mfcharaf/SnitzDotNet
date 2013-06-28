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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzData;

public partial class Homepage : PageBase
{

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (webResources.TextDirection == "rtl")
            pageCSS.Attributes.Add("href", "css/" + Page.StyleSheetTheme + "/homepagertl.css");
        else
            pageCSS.Attributes.Add("href", "css/" + Page.StyleSheetTheme + "/homepage.css");

        string pagedescription = Config.ForumDescription;
        metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", HttpUtility.HtmlEncode(pagedescription));

    }

    private void Page_Load(object sender, EventArgs e)
	{
        Session["TopicId"] = "";
        Session["ForumId"] = "";
        Session["CatId"] = "";

        var cats = Util.ListCategories();
        if (CatId != null)
            cats = cats.Where(c => c.Id == CatId).ToList();
	    repCatDL.DataSource = cats;

        repCatDL.DataBind();
        Page.Title = string.Format(webResources.ttlDefaultPage, Config.ForumTitle);
        GroupDIV.Visible = Config.ShowGroups;

        WriteJavascript();

        var smp = (SiteMapPath)Master.FindControl("SiteMap");
        if(smp != null)
            smp.Visible = false;
	}

    private void WriteJavascript()
    {
        //writes the javascript to collaps categories
        var sScript = new StringBuilder();
        sScript.AppendLine("function _getRowIndex(col) {");
        sScript.AppendLine("var row = col.parentNode;");
        sScript.AppendLine("var obj = row;");
        sScript.AppendLine("var rtrn = row.rowIndex || 0;");
        sScript.AppendLine("if (rtrn == 0) {");
        sScript.AppendLine("do{");
        sScript.AppendLine("if (row.nodeType == 1) rtrn++;");
        sScript.AppendLine("row = row.previousSibling;");
        sScript.AppendLine("} while (row);");
        sScript.AppendLine("--rtrn;");
        sScript.AppendLine("}");
        sScript.AppendLine("if(obj.parentNode.parentNode.rows[rtrn+1].style.display == 'none'){");
        sScript.AppendLine("obj.parentNode.parentNode.rows[rtrn+1].style.display = obj.parentNode.parentNode.rows[rtrn].style.display;");
        sScript.AppendLine("obj.cells[0].className = 'CategoryExpanded';");
        sScript.AppendLine("}else{");
        sScript.AppendLine("obj.parentNode.parentNode.rows[rtrn+1].style.display= 'none';");
        sScript.AppendLine("obj.cells[0].className = 'CategoryCollapsed';");
        sScript.AppendLine("}");
        sScript.AppendLine("}");
        ClientScript.RegisterClientScriptBlock(GetType(), "collapse_cat", sScript.ToString(), true);
    }

    private static Image GetForumIcon(string username, DateTime lasthere, Enumerators.PostStatus fStatus, DateTime? tLastPost)
    {
        var image = new Image { ID = "imgTopicIcon", EnableViewState = false };

        switch (fStatus)
        {
            case Enumerators.PostStatus.Open:
                image.SkinID = "Folder";
                image.AlternateText = webResources.lblOldPosts;
                if (username != "")
                    if (tLastPost > lasthere)
                    {
                        image.SkinID = "FolderNew";
                        image.AlternateText = webResources.lblNewPosts;
                    }
                break;
            default:
                image.SkinID = "FolderLocked";
                image.AlternateText = webResources.lblLockedForum;
                break;
        }
        return image;
    }

    protected void CategoryDataListItemDataBound(object sender, RepeaterItemEventArgs e)
	{
	    RepeaterItem item = e.Item;
	    if( (item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem) )
	    {
            var nestedRepeater = (Repeater)item.FindControl("repForum");
            var cat = (Category)item.DataItem;
            var lockIcon = item.FindControl("CatLock") as ImageButton;
            var unlockIcon = item.FindControl("CatUnLock") as ImageButton;
            var editIcon = item.FindControl("EditCat") as ImageButton;
            var newIcon = item.FindControl("NewForum") as ImageButton;
            var newUrl = item.FindControl("NewUrl") as ImageButton;
	        var delIcon = item.FindControl("CatDelete") as ImageButton;

            if(delIcon != null)
            {
                delIcon.OnClientClick =
                    "mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to delete the Category?'); " +
                    "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete the Category?'},'confirmHandlers.BeginRecieve'); " +
                    "return false;";
            }
            if (editIcon != null)
            {
                editIcon.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Category Properties',{{'pageID':9,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        cat.Id);
                editIcon.Visible = IsAdministrator;
            }
            if (newIcon != null)
            {
                newIcon.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml(' Forum Properties',{{'pageID':8,'data':'{0},{1},{2}'}}, 'methodHandlers.BeginRecieve');return false;",
                        -1, cat.Id,0);
            }
            if (newUrl != null)
            {
                newUrl.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml(' Url Properties',{{'pageID':8,'data':'{0},{1},{2}'}}, 'methodHandlers.BeginRecieve');return false;",
                        -1, cat.Id,1);                
            }
            if (lockIcon != null)
            {
                lockIcon.Visible = ((IsAdministrator) && (cat.Status == Enumerators.PostStatus.Open));
                lockIcon.OnClientClick = "mainScreen.ShowConfirm(this, 'Confirm Lock', 'Lock?'); " +
                                         "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to lock the Category?'},'confirmHandlers.BeginRecieve'); " +
                                         "return false;";
            }
            if (unlockIcon != null)
            {
                unlockIcon.Visible = ((IsAdministrator) && (cat.Status == Enumerators.PostStatus.Closed));
                unlockIcon.OnClientClick = "mainScreen.ShowConfirm(this, 'Confirm UnLock', 'Unlock?'); " +
                                           "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to unlock the Category?'},'confirmHandlers.BeginRecieve'); " +
                                           "return false;";
            }
	        //RoleList.AddRange(SnitzCachedLists.UserRoles().Select(_role => _role.Key));
	        List<Forum> allowedForums = cat.Forums.AllowedForums(IsAdministrator);

            if (allowedForums.Count == 0 && cat.Forums.Count != 0)
                e.Item.Visible = false;
            nestedRepeater.DataSource = allowedForums;
            nestedRepeater.DataBind();
	    }
	}
    
    protected void DdlGroupsSelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void RepForumItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;
        var page = (PageBase)this.Page;


        if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
        {

            var forum = (Forum)item.DataItem;

            var lockIcon = item.FindControl("ForumLock") as ImageButton;
            var unlockIcon = item.FindControl("ForumUnLock") as ImageButton;
            var delIcon = item.FindControl("ForumDelete") as ImageButton;
            var editIcon = item.FindControl("ForumEdit") as ImageButton;
            var subscribe = item.FindControl("ForumSub") as ImageButton;
            var unsubscribe = item.FindControl("ForumUnSub") as ImageButton;
            var empty = item.FindControl("ForumEmpty") as ImageButton;
            var newIcon = item.FindControl("hypNewTopic") as HyperLink;
            var popuplink = item.FindControl("popuplink") as Literal;
            var ldate = (Literal)item.FindControl("lDate");
            var iconPh = (PlaceHolder)item.FindControl("Ticons");

            var repeater = (RepeaterItem)item.Parent.Controls[0];
            var header = repeater.FindControl("fTableHeader");

            if (forum.Type == 1) //External link
            {
                header.Visible = false;
                var link = (HyperLink) item.FindControl("forumLink");
                var linkcol = (HtmlTableCell)item.FindControl("linkCol");
                var tCount = (HtmlTableCell)item.FindControl("tCount");
                var pCount = (HtmlTableCell)item.FindControl("pCount");
                var lastpost = (HtmlTableCell)item.FindControl("lastpost");
                var buttonCol = (HtmlTableCell)item.FindControl("adminBtn");

                link.NavigateUrl = forum.URL;
                link.Target = "_blank";
                linkcol.ColSpan = 4;
                tCount.Visible = false;
                pCount.Visible = false;
                lastpost.Visible = false;

                if (iconPh != null)
                {
                    var img = new Image { ID = "imgTopicIcon", SkinID = "WebLink", AlternateText = "", EnableViewState = false };
                    img.ApplyStyleSheetSkin(Page);
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

            if (iconPh != null && forum.Type != 1)
            {
                var img =
                    GetForumIcon(HttpContext.Current.User.Identity.Name, LastVisitDateTime, forum.Status,
                                 forum.LastPostDate);
                img.ApplyStyleSheetSkin(Page);
                img.EnableViewState = false;
                iconPh.Controls.Add(img);
            }
            
            if(ldate != null)
            {
                int offset = 0;
                if (page.Member != null)
                    offset = page.Member.TimeOffset;
                if(forum.LastPostDate.HasValue && !(forum.LastPostDate.Value == DateTime.MinValue))
                    ldate.Text = Common.TimeAgoTag(forum.LastPostDate.Value,page.IsAuthenticated,offset);
            }
            bool isForumModerator = new SnitzRoleProvider().IsUserForumModerator(HttpContext.Current.User.Identity.Name, forum.Id);


            if (popuplink != null)
            {
                string title = String.Format(webResources.lblViewProfile, "$1");
                popuplink.Text = forum.LastPostAuthor != null ? Regex.Replace(forum.LastPostAuthor.ProfilePopup, @"\[!(.*)!]", title) : "";
            }
            if (IsAuthenticated)
            {
                if (subscribe != null)
                {
                    subscribe.Visible = IsAuthenticated &&
                                        forum.SubscriptionLevel == (int) Enumerators.Subscription.ForumSubscription;
                    subscribe.Visible = subscribe.Visible && (forum.Type != 1);
                    subscribe.OnClientClick =
                        "mainScreen.ShowConfirm(this, 'Confirm Subscribe', 'Do you want to subscribe to new posts in the Forum?'); " +
                        "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to subscribe to new posts in the Forum?'},'confirmHandlers.BeginRecieve'); " +
                        "return false;";
                }
                if (unsubscribe != null)
                {
                    unsubscribe.Visible = false;
                    if (subscribe.Visible)
                    {
                        if (Member.IsSubscribedToForum(forum.Id))
                        {
                            subscribe.Visible = false;
                            unsubscribe.Visible = true;
                        }
                    }
                    unsubscribe.Visible = unsubscribe.Visible && (forum.Type != 1);
                    unsubscribe.OnClientClick =
                        "mainScreen.ShowConfirm(this, 'Remove Subscription', 'Do you want to remove your subscription for this Forum?'); " +
                        "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to remove your subscription for this Forum?'},'confirmHandlers.BeginRecieve'); " +
                        "return false;";
                }
                if (newIcon != null)
                {
                    newIcon.Visible = IsAuthenticated;
                    newIcon.Visible = newIcon.Visible && forum.Status != Enumerators.PostStatus.Closed;
                    newIcon.Visible = newIcon.Visible || (IsAdministrator || isForumModerator);
                    newIcon.NavigateUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}",
                                                        forum.Id, forum.CatId);
                    newIcon.Visible = newIcon.Visible && (forum.Type != 1);
                }
                if (lockIcon != null)
                {
                    lockIcon.Visible = ((IsAdministrator || isForumModerator) && (forum.Status == Enumerators.PostStatus.Open));
                    lockIcon.Visible = lockIcon.Visible && (forum.Type != 1);
                    lockIcon.OnClientClick =
                        "mainScreen.ShowConfirm(this, 'Confirm Lock', 'Do you want to lock the Forum?'); " +
                        "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to lock the Forum?'},'confirmHandlers.BeginRecieve'); " +
                        "return false;";
                }
                if (unlockIcon != null)
                {
                    unlockIcon.Visible = ((IsAdministrator || isForumModerator) &&
                                          (forum.Status == Enumerators.PostStatus.Closed));
                    unlockIcon.Visible = unlockIcon.Visible && (forum.Type != 1);
                    unlockIcon.OnClientClick =
                        " mainScreen.ShowConfirm(this, 'Confirm UnLock', 'Do you want to unlock the Forum?');" +
                        " mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to unlock the Forum?'},'confirmHandlers.BeginRecieve');" +
                        " return false;";
                }
                if (delIcon != null)
                {
                    delIcon.Visible = IsAdministrator;
                    delIcon.OnClientClick =
                        "mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to delete the Forum?'); " +
                        "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete the Forum?'},'confirmHandlers.BeginRecieve'); " +
                        "return false;";
                }
                if (editIcon != null)
                {
                    editIcon.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml(' Edit Properties',{{'pageID':8,'data':'{0},{1},{2}'}}, 'methodHandlers.BeginRecieve');return false;",
                        forum.Id, forum.CatId,forum.Type);
                    editIcon.Visible = (IsAdministrator || isForumModerator);
                }
                if (empty != null)
                {
                    empty.Visible = empty.Visible && (forum.Type != 1);
                    empty.OnClientClick =
                        "mainScreen.ShowConfirm(this, 'Confirm Empty', 'Do you want to delete all the posts in the Forum?'); " +
                        "mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete all the posts in the Forum?'},'confirmHandlers.BeginRecieve'); " +
                        "return false;";
                }
            }else
            {
                var buttonCol = (HtmlTableCell)item.FindControl("adminBtn");
                var lastpost = (HtmlTableCell)item.FindControl("lastpost");
                buttonCol.Visible = false;
                lastpost.ColSpan = 2;
            }

        }
    }

    #region ImageButton Events

    protected void LockForum(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.SetForumStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Closed);
        Response.Redirect(Request.RawUrl);
    }

    protected void UnLockForum(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.SetForumStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Open);
        Response.Redirect(Request.RawUrl);
    }

    protected void DeleteForum(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.DeleteForum(Convert.ToInt32(btn.CommandArgument));
        Response.Redirect(Request.RawUrl);
    }

    protected void EmptyForum(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.EmptyForum(Convert.ToInt32(btn.CommandArgument));
        Response.Redirect(Request.RawUrl);
    }

    protected void DeleteCategory(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.DeleteCat(Convert.ToInt32(btn.CommandArgument));
        Response.Redirect(Request.RawUrl);
    }

    protected void LockCategory(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.SetCatStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Closed);
        Response.Redirect(Request.RawUrl);
    }

    protected void UnLockCategory(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        Util.SetCatStatus(Convert.ToInt32(btn.CommandArgument), Enumerators.PostStatus.Closed);
        Response.Redirect(Request.RawUrl);
    }

    protected void SubscribeForum(object sender, ImageClickEventArgs e)
    {
        var btn = (ImageButton)sender;
        int forumid = Convert.ToInt32(btn.CommandArgument);
        switch (btn.CommandName)
        {
            case "sub" :
                Util.AddForumSubscription(Member.Id, forumid);
                break;
            case "unsub" :
                Util.RemoveForumSubscription(Member.Id,forumid);
                break;
        }
        
    }

    #endregion

    #region Page methods for Ajax Name and Email checks

    [WebMethod]
    public static void SaveForum(string jsonform)
    {
        var test = HttpUtility.UrlDecode(jsonform);
        System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
        int forumid = Convert.ToInt32(formresult["ctl00$hdnForumId"]);
        var forum = forumid == -1 ? new Forum { Id = -1 } : Util.GetForum(forumid);
        forum.SubscriptionLevel = 0;
        forum.ModerationLevel = 0;
        var roles = new string[] { };
        var moderators = new string[] { };
        string password = "";

        if (!formresult.AllKeys.Contains("ctl00$CountPost"))
            forum.UpdatePostCount = false;
        if (!formresult.AllKeys.Contains("ctl00$AllowPolls"))
            forum.AllowPolls = false;
        try
        {
            foreach (string key in formresult.AllKeys)
            {
                //ctl00$
                if(key != null)
                switch (key.Replace("ctl00$", ""))
                {
                    case "ddlCat":
                        forum.CatId = Convert.ToInt32(formresult[key]);
                        break;
                    case "tbxUrl":
                        forum.URL = formresult[key];
                        break;
                    case "tbxSubject":
                        forum.Subject = formresult[key];
                        break;
                    case "tbxBody":
                        forum.Description = formresult[key];
                        break;
                    case "cbxCountPost":
                        forum.UpdatePostCount = formresult[key] == "on";
                        break;
                    case "ddlMod":
                        forum.ModerationLevel = (Enumerators.Moderation)Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlSub":
                        forum.SubscriptionLevel = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlAuthType":
                        forum.AuthType = (Enumerators.ForumAuthType)Convert.ToInt32(formresult[key]);
                        break;
                    case "hdnRoleList":
                        roles = formresult[key].ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "cbxAllowPolls":
                        forum.AllowPolls = formresult[key] == "on";
                        break;
                    case "hdnModerators":
                        moderators = formresult[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "tbxPassword":
                        password = formresult[key];
                        break;
                }

            }
        }
        catch (Exception)
        {

        }

        forum.Password = password.Trim();

        int newId = Util.SaveForum(forum);
        SnitzRoleProvider.AddRolesToForum(newId,roles);
        Util.AddForumModerators(newId, moderators);
    }
    [WebMethod]
    public static void SaveCategory(string jsonform)
    {
        var test = HttpUtility.UrlDecode(jsonform);
        System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
        int catid = Convert.ToInt32(formresult["ctl00$hdnCatId"]);
        Category cat = catid == -1 ? new Category { Id = -1 } : Util.GetCategory(catid);
        foreach (string key in formresult.AllKeys)
        {
            //ctl00$
            switch (key.Replace("ctl00$", ""))
            {
                case "tbxSubject":
                    cat.Name = formresult[key];
                    break;
                case "ddlMod":
                    cat.ModerationLevel = Convert.ToInt32(formresult[key]);
                    break;
                case "ddlSub":
                    cat.SubscriptionLevel = Convert.ToInt32(formresult[key]);
                    break;
                //case "cbxPassword":
                //    if (formresult[key] != null)
                //        forum.UpdatePostCount = formresult[key] == "1";
                //    break;
                //case "ddlAuthType":
                //    forum.AuthType = (Enumerators.ForumAuthType)Convert.ToInt32(formresult[key]);
                //    break;
            }

        }
        Util.SaveCategory(cat);
    }

    #endregion
}
