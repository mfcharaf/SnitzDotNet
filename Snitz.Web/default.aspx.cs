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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                case "ForumSubscribe":
                    id = Convert.ToInt32(argument);
                    SubscribeForum(id, false);
                    break;
                case "ForumUnSubscribe":
                    id = Convert.ToInt32(argument);
                    SubscribeForum(id, true);
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
        //WriteJavascript();

        var smp = (SiteMapPath)Master.FindControl("SiteMap");
        if (smp != null)
            smp.Visible = false;
	}

    private void WriteJavascript()
    {
        //writes the javascript to collapse categories
        //var sScript = new StringBuilder();
        //sScript.AppendLine("function _getRowIndex(col) {");
        //sScript.AppendLine("var row = col.parentNode;");
        //sScript.AppendLine("var obj = row;");
        //sScript.AppendLine("var rtrn = row.rowIndex || 0;");
        //sScript.AppendLine("if (rtrn == 0) {");
        //sScript.AppendLine("do{");
        //sScript.AppendLine("if (row.nodeType == 1) rtrn++;");
        //sScript.AppendLine("row = row.previousSibling;");
        //sScript.AppendLine("} while (row);");
        //sScript.AppendLine("--rtrn;");
        //sScript.AppendLine("}");
        //sScript.AppendLine("if(obj.parentNode.parentNode.rows[rtrn+1].style.display == 'none'){");
        //sScript.AppendLine("obj.parentNode.parentNode.rows[rtrn+1].style.display = obj.parentNode.parentNode.rows[rtrn].style.display;");
        //sScript.AppendLine("obj.cells[0].className = 'CategoryExpanded';");
        //sScript.AppendLine("}else{");
        //sScript.AppendLine("obj.parentNode.parentNode.rows[rtrn+1].style.display= 'none';");
        //sScript.AppendLine("obj.cells[0].className = 'CategoryCollapsed';");
        //sScript.AppendLine("}");
        //sScript.AppendLine("}");
        //ClientScript.RegisterClientScriptBlock(GetType(), "collapse_cat", sScript.ToString(), true);
    }

    protected void CategoryDataListItemDataBound(object sender, RepeaterItemEventArgs e)
    {
	    RepeaterItem item = e.Item;
	    if( (item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem) )
	    {
            //var nestedRepeater = (Repeater)item.FindControl("repForum");
            var cat = (CategoryInfo)item.DataItem;
            var lockIcon = item.FindControl("CatLock") as ImageButton;
            var unlockIcon = item.FindControl("CatUnLock") as ImageButton;
            var editIcon = item.FindControl("EditCat") as ImageButton;
            var newIcon = item.FindControl("NewForum") as ImageButton;
            var newUrl = item.FindControl("NewUrl") as ImageButton;
	        var delIcon = item.FindControl("CatDelete") as ImageButton;

            if(delIcon != null)
            {
                delIcon.OnClientClick =
                    "setArgAndPostBack('Do you want to delete the Category?','DeleteCategory'," + cat.Id + ");return false;";
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
                    "setArgAndPostBack('Do you want to lock the Category?','LockCategory'," + cat.Id + ");return false;";
            }
            if (unlockIcon != null)
            {
                unlockIcon.Visible = ((IsAdministrator) && (cat.Status == (int)Enumerators.PostStatus.Closed));
                unlockIcon.OnClientClick =
                    "setArgAndPostBack('Do you want to unlock the Category?','UnLockCategory'," + cat.Id + ");return false;";

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

    protected void SubscribeForum(int forumid, bool remove)
    {
        if(remove)
            Subscriptions.RemoveForumSubscription(Member.Id, forumid);
        else
            Subscriptions.AddForumSubscription(Member.Id, forumid);
    }

    #endregion
    #region Page methods for Ajax 

    [WebMethod]
    public static string GetForums(string categoryId)
    {
        Page page = new Page();
        
        CategoryForums ctl = (CategoryForums)page.LoadControl("~/UserControls/CategoryForums.ascx");
        ctl.Member = Members.GetMember(HttpContext.Current.User.Identity.Name);
        ctl.CategoryId = categoryId;
        page.Controls.Add(ctl);
        HtmlForm tempForm = new HtmlForm();
        tempForm.Controls.Add(ctl);
        page.Controls.Add(tempForm);
        StringWriter writer = new StringWriter();
        HttpContext.Current.Server.Execute(page, writer, false);
        string outputToReturn = writer.ToString();
        outputToReturn = outputToReturn.Substring(outputToReturn.IndexOf("<div"));
        outputToReturn = outputToReturn.Substring(0, outputToReturn.IndexOf("</form>"));
        writer.Close();
        string viewStateRemovedOutput = Regex.Replace(outputToReturn,
        "<input type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\".*?\" />",
        "", RegexOptions.IgnoreCase);
        return viewStateRemovedOutput;
    } 
    [WebMethod]
    public static void SaveForum(string jsonform)
    {
        var test = HttpUtility.UrlDecode(jsonform);
        System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
        int forumid = Convert.ToInt32(formresult["ctl00$hdnForumId"]);
        ForumInfo forum = forumid == -1 ? new ForumInfo { Id = -1,Status = 1} : Forums.GetForum(forumid);
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
                        forum.Url = formresult[key];
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
                    case "tbxOrder":
                        forum.Order = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlMod":
                        forum.ModerationLevel = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlSub":
                        forum.SubscriptionLevel = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlAuthType":
                        forum.AuthType = Convert.ToInt32(formresult[key]);
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

        int newId = Forums.SaveForum(forum);
        SnitzRoleProvider.AddRolesToForum(newId,roles);
        Forums.AddForumModerators(newId, moderators);
    }
    [WebMethod]
    public static void SaveCategory(string jsonform)
    {
        var test = HttpUtility.UrlDecode(jsonform);
        System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
        int catid = Convert.ToInt32(formresult["ctl00$hdnCatId"]);
        CategoryInfo cat = catid == -1 ? new CategoryInfo { Id = -1 } : Categories.GetCategory(catid);
        foreach (string key in formresult.AllKeys)
        {
            //ctl00$
            switch (key.Replace("ctl00$", ""))
            {
                case "tbxSubject":
                    cat.Name = formresult[key];
                    break;
                case "tbxOrder" :
                    cat.Order = Convert.ToInt32(formresult[key]);
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
        Categories.UpdateCategory(cat);
    }

    #endregion
}
