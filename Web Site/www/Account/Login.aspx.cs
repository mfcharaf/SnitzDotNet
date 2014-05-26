/*
####################################################################################################################
##
## SnitzUI.Account - Login.aspx
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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzUI.UserControls;
using SnitzCommon;
using SnitzConfig;


public partial class Login : PageBase
{
    private static string returnurl;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
            returnurl = Request.UrlReferrer.PathAndQuery;
 
        Login1.InstructionText = Resources.extras.lblRegisterLink;

            if (Config.RequireRegistration && !Config.ProhibitNewMembers)
                Login1.InstructionText = Resources.extras.lblRegisterLink;
            else if (Config.ProhibitNewMembers)
                Login1.InstructionText = Resources.extras.lblProhibitReg;
            Label instructions = (Label)Login1.FindControl("LoginReq");
            if (instructions != null)
                instructions.Text = Login1.InstructionText;

    }
    protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
    {
        TextBox uname = (TextBox) Login1.FindControl("UserName");
        TextBox pword = (TextBox) Login1.FindControl("Password");
        CheckBox remember = (CheckBox) Login1.FindControl("RememberMe");
        if(remember != null)
        {
            Login1.RememberMeSet = remember.Checked;
        }
        e.Authenticated = Membership.ValidateUser(uname.Text, pword.Text);

        SnitzCaptchaControl ct = (SnitzCaptchaControl)Login1.FindControl("CAPTCHA");
        if (ct != null)
            if (ct.Visible)
                e.Authenticated = e.Authenticated && ct.IsValid;
        if(e.Authenticated && !String.IsNullOrEmpty(returnurl) && !returnurl.ToLower().Contains("login"))
            Login1.DestinationPageUrl = returnurl;
    }
}
