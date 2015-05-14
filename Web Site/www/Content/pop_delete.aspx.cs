#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;

public partial class pop_delete : Page
{
    private string mode;
    private string username;
    private int DeleteId;
    private int ForumId;
    private int TopicId;

    private bool IsAdmin;
    private bool IsAuthor;
    private bool IsForumModerator;
    private string strError = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (HttpContext.Current.User.Identity.Name == "")
            CloseWindow("default.aspx");

        
        mode = Request.Params["mode"];
        if (mode == "M")
            username = Request.Params["ID"];
        else 
            DeleteId = Int32.Parse(Request.Params["ID"]);
        if (Request.Params["FORUM_ID"] != null)
        {
            ForumId = Int32.Parse(Request.Params["FORUM_ID"]);
            IsForumModerator = SnitzRoleProvider.IsUserForumModerator(HttpContext.Current.User.Identity.Name, ForumId);
        }
        if (Request.Params["TOPIC_ID"] != null)
            TopicId = Int32.Parse(Request.Params["TOPIC_ID"]);

        if (Request.Params["author"] != null)
            IsAuthor = (HttpContext.Current.User.Identity.Name.ToLower() == Request.Params["author"].ToLower());

        IsAdmin = (Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator"));

        if (!Page.IsPostBack)
        {
            if (!AccessAllowed(mode))
            {
                strError = webResources.ErrNotAuth;
                btnYes.Visible = false;
                btnYes.Enabled = false;
                btnNo.Text = webResources.btnClose;
                CustomValidator1.IsValid = false;
                CustomValidator1.Validate();
            }
        }
    }
    protected bool AccessAllowed(string _mode)
    {
        Label lbl = (Label) Master.FindControl("Label1");
        
        switch (_mode.ToUpper())
        {
            case "C":
                Page.Title = lbl.Text = string.Format(webResources.lblDelete, webResources.lblCategory);
                Label1.Text = string.Format(webResources.lblDelete, "");
                return IsAdmin;
            case "F":
                Page.Title = lbl.Text = string.Format(webResources.lblDelete, webResources.lblForum);
                Label1.Text = string.Format(webResources.lblDelete, "");
                return (IsAdmin || IsForumModerator);
            case "M":
                Page.Title = lbl.Text = string.Format(webResources.lblDelete, webResources.lblUser);
                Label1.Text = string.Format(webResources.lblDelete, "");
                return IsAdmin;
            case "R":
                Page.Title = lbl.Text = string.Format(webResources.lblDelete, webResources.lblReply);
                Label1.Text = string.Format(webResources.lblDelete, "");
                return (IsAdmin || IsAuthor || IsForumModerator);
            case "T":
                Page.Title = lbl.Text = string.Format(webResources.lblDelete, webResources.lblTopic);
                Label1.Text = string.Format(webResources.lblDelete, "");
                return (IsAdmin || IsAuthor || IsForumModerator);
        }
        return false;
    }
    protected void btnOk_Click(object sender, EventArgs e)
    {
        switch (mode)
        {
            case "C":
                strError = webResources.ErrNotImplemented;
                break;
            case "F":
                strError = webResources.ErrNotImplemented;
                break;
            case "M":
                SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
                if (smp != null) smp.DeleteUser(username, true);
                break;
            case "R":
                strError = "";
                strError = Topic.DeleteReply(DeleteId, TopicId, ForumId);
                break;
            case "T":
                strError = "";
                strError = Topic.DeleteTopic(DeleteId, ForumId, IsAuthor, IsAdmin);
                CloseWindow("active.aspx");
                break;
        }
        if ( strError == "")
            Forum.UpdateForumCounts();

    }
    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = false;

        CustomValidator1.ErrorMessage = strError;
        if (CustomValidator1.ErrorMessage == "")
            args.IsValid = true;

        if (args.IsValid)
            CloseWindow("");
    }
    protected void CloseWindow(string location)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("window.opener;");
        sb.AppendFormat("opener.location.reload('{0}');",location);
        sb.Append("window.close();");

        ClientScript.RegisterClientScriptBlock(GetType(), "CloseWindowScript", sb.ToString(), true);
    }
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if (Session["_theme"] == null)
        {
            Session.Add("_theme", config.DefaultTheme);
        }
    }
}
