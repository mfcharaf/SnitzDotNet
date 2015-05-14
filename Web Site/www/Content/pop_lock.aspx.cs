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
using Snitz.BLL;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;

public partial class pop_lock : PageBase
{
    private string mode;
    private string user;
    private int LockId;
    private int ForumId;
    private int LockState;
    private string strError = "";
    private bool CanLock;

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        if (Session["_theme"] == null)
        {
            Session.Add("_theme", Config.DefaultTheme);
            //Page.Theme = ((string)Session["_theme"]);
        }
    }
    private void Page_Load(object sender, EventArgs e)
    {

        if (Request.Params["ID"] == null)
            Response.Redirect("error.aspx?msg=errInvalidForumId");
        mode = Request.Params["mode"];
        LockState = Int32.Parse(Request.Params["lock"]);

        if (mode == "M")
            user = Request.Params["ID"];
        else
        {
            LockId = Int32.Parse(Request.Params["ID"]);
            ForumId = Int32.Parse(Request.Params["FId"]);
        }

        CanLock = (Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator") || Moderators.IsUserForumModerator(HttpContext.Current.User.Identity.Name, ForumId));
        if (!CanLock)
            CloseWindow();


        if (Page.IsPostBack)
        {
            switch (mode.ToUpper())
            {
                case "M" :
                    SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
                    if(smp != null)
                    switch (LockState)
                    {
                        case 0:
                            if (!String.IsNullOrEmpty(user))
                                if (smp.UnlockUser(user))
                                    strError = "Member Un-Locked";
                            break;
                        case 1:
                            if (!String.IsNullOrEmpty(user))
                                if (smp.LockUser(user))
                                    strError = "Member Locked";
                            break;
                    }

                    break;
                    
            }
        }
        else
        {
            switch (mode.ToUpper())
            {
                case "T":
                    switch (LockState)
                    {
                        case 0:
                            Page.Title = webResources.lblLock;
                            ((Label)Master.FindControl("Label1")).Text = webResources.lblLock;
                            break;
                        case 1:
                            Page.Title = webResources.lblUnlock;
                            ((Label)Master.FindControl("Label1")).Text = webResources.lblUnlock;
                            break;
                    }
                    
                    break;
                case "F" :
                    switch (LockState)
                    {
                        case 0:
                            Page.Title = webResources.lblLockForum;
                            ((Label)Master.FindControl("Label1")).Text = webResources.lblLockForum;
                            break;
                        case 1:
                            Page.Title = webResources.lblUnLockForum;
                            ((Label)Master.FindControl("Label1")).Text = webResources.lblUnLockForum;
                            break;
                    }

                    break;
                case "M" :
                    switch (LockState)
                    {
                        case 0:
                            Page.Title = webResources.lblUnlockUser;
                            ((Label)Master.FindControl("Label1")).Text = String.Format(webResources.lblUnlockUser,user);
                            break;
                        case 1:
                            Page.Title = webResources.lblLockUser;
                            ((Label)Master.FindControl("Label1")).Text = String.Format(webResources.lblLockUser, user);
                            break;
                    }
                    break;
            }

        }
    }
    protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = false;
        CustomValidator1.ErrorMessage = strError;
        if (CustomValidator1.ErrorMessage == "")
            args.IsValid = true;

        if (args.IsValid)
            CloseWindow();
    }
    protected void CloseWindow()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("window.opener;");
        sb.Append("opener.location.reload();");
        sb.Append("window.close();");

        ClientScript.RegisterClientScriptBlock(typeof(Page), "CloseWindowScript", sb.ToString(), true);
    }

}
