/*
####################################################################################################################
##
## SnitzUI - default.aspx
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzUI.UserControls;


public partial class Homepage : PageBase
{

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (webResources.TextDirection == "rtl")
            pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/homepagertl.css");
        else
            pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/homepage.css");

        string pagedescription = Config.ForumDescription;
        metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", HttpUtility.HtmlEncode(pagedescription));

    }

    private void Page_Load(object sender, EventArgs e)
	{
        if (Page.IsPostBack)
        {
            string postbackbtn = Request.Form["__EVENTTARGET"];
            string argument = Request.Form["__EVENTARGUMENT"];
            int id;
            switch (postbackbtn)
            {
                case "ForumDelete":
                    id = Convert.ToInt32(argument);
                    DeleteForum(id);
                    break;
                case "ForumLock":
                    id = Convert.ToInt32(argument);
                    SetForumLockState(id, Enumerators.PostStatus.Closed);
                    break;
                case "ForumUnLock":
                    id = Convert.ToInt32(argument);
                    SetForumLockState(id, Enumerators.PostStatus.Open);
                    break;
                case "ForumEmpty":
                    id = Convert.ToInt32(argument);
                    EmptyForum(id);
                    break;
                case "DeleteCategory":
                    id = Convert.ToInt32(argument);
                    DeleteCategory(id);
                    break;
                case "LockCategory":
                    id = Convert.ToInt32(argument);
                    SetCategoryLockState(id, Enumerators.PostStatus.Closed);
                    break;
                case "UnLockCategory":
                    id = Convert.ToInt32(argument);
                    SetCategoryLockState(id, Enumerators.PostStatus.Open);
                    break;
                case "ForumArchive":
                    id = Convert.ToInt32(argument);

                    Archive.ArchiveForums(new int[] {id}, null);
                    break;

            }

        }
        if (!Config.ShowStats)
        {
            ContentPlaceHolder holder = (ContentPlaceHolder)Master.FindControl("CPF1");
            holder.Controls.Clear();
        }
        Session["TopicId"] = "";
        Session["ForumId"] = "";
        Session["CatId"] = "";

        var cats = Categories.GetCategories();
        if (CatId != null)
            cats = cats.Where(c => c.Id == CatId).ToList();
        repCatDL.DataSource = cats;

        repCatDL.DataBind();
        Page.Title = string.Format(webResources.ttlDefaultPage, Config.ForumTitle);
        GroupDIV.Visible = Config.ShowGroups;

        var smp = (SiteMapPath)Master.FindControl("SiteMap");
        if (smp != null)
            smp.Visible = false;
	}

    protected void CategoryDataListItemDataBound(object sender, RepeaterItemEventArgs e)
    {
	    RepeaterItem item = e.Item;
	    if( (item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem) )
	    {
            var cat = (CategoryInfo)item.DataItem;
            var lockIcon = item.FindControl("CatLock") as ImageButton;
            var unlockIcon = item.FindControl("CatUnLock") as ImageButton;
            var editIcon = item.FindControl("EditCat") as ImageButton;
            var newIcon = item.FindControl("NewForum") as ImageButton;
            var newUrl = item.FindControl("NewUrl") as ImageButton;
	        var delIcon = item.FindControl("CatDelete") as ImageButton;
            var subscribe = item.FindControl("CatSub") as ImageButton;
            var unsubscribe = item.FindControl("CatUnSub") as ImageButton;
            if(delIcon != null)
            {
                delIcon.OnClientClick =
                    "confirmPostBack('Do you want to delete the Category?','DeleteCategory'," + cat.Id + ");return false;";
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
                lockIcon.Visible = ((IsAdministrator) && (cat.Status == (int)Enumerators.PostStatus.Open));
                lockIcon.OnClientClick =
                    "confirmPostBack('Do you want to lock the Category?','LockCategory'," + cat.Id + ");return false;";
            }
            if (unlockIcon != null)
            {
                unlockIcon.Visible = ((IsAdministrator) && (cat.Status == (int)Enumerators.PostStatus.Closed));
                unlockIcon.OnClientClick =
                    "confirmPostBack('Do you want to unlock the Category?','UnLockCategory'," + cat.Id + ");return false;";

            }
            if (subscribe != null)
            {
                subscribe.Visible = IsAuthenticated;
                subscribe.Visible = subscribe.Visible && cat.SubscriptionLevel == (int)Enumerators.CategorySubscription.CategorySubscription;
                subscribe.OnClientClick = "confirmCatSubscribe('Do you want to be notified when someone posts in this category?'," + cat.Id + ",false);return false;";
            }
            if (unsubscribe != null)
            {
                unsubscribe.Visible = false;
                if (subscribe.Visible)
                {
                    if (Members.IsSubscribedToCategory(Member.Id,cat.Id))
                    {
                        subscribe.Visible = false;
                        unsubscribe.Visible = true;
                    }
                }
                unsubscribe.OnClientClick = "confirmCatSubscribe('Do you want to remove notifications in this category?'," + cat.Id + ",true);return false;";
            }
	        cat.ForumCount = Forums.GetForumsByCategory(cat.Id, 0, 1000).Count();
            if (!Categories.GetCategoryForums(cat.Id, Member).Any() && cat.ForumCount != 0)
                e.Item.Visible = false;

	    }
	}
    
    protected void DdlGroupsSelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected void DeleteCategory(int catid)
    {
        Categories.DeleteCategory(catid);
        Response.Redirect(Request.RawUrl);
    }

    protected void SetCategoryLockState(int catid, Enumerators.PostStatus lockstatus)
    {
        Categories.SetCatStatus(catid, (int)lockstatus);
        Response.Redirect(Request.RawUrl);
    }

    #region Forum button Events

    protected void SetForumLockState(int forumid,Enumerators.PostStatus lockstatus)
    {
        Forums.SetForumStatus(forumid, (int)lockstatus);
        Response.Redirect(Request.RawUrl);
    }

    protected void DeleteForum(int forumid)
    {
        Forums.DeleteForum(forumid);
        Response.Redirect(Request.RawUrl);
    }

    protected void EmptyForum(int forumid)
    {
        Forums.EmptyForum(forumid);
        Response.Redirect(Request.RawUrl);
    }

    #endregion

}
