/*
####################################################################################################################
##
## SnitzUI.UserControls - login.ascx
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
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using Snitz.BLL;
using SnitzCommon;
using SnitzConfig;


public partial class User_Controls_login : System.Web.UI.UserControl
{
    private string _skinid;
    private System.Web.UI.WebControls.Login login;
    public string Skin
    {
        get { return _skinid; }
        set { _skinid = value; }
    }
	
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Request.Path.EndsWith("register.aspx"))
        {
            PageBase page = (PageBase)Page;

            login = (System.Web.UI.WebControls.Login)LoginView1.FindControl("Login1");

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                LoginName Lname = (LoginName)LoginView1.FindControl("ln2");
                Literal lit = (Literal) LoginView1.FindControl("Literal1");
                string separator = _skinid == "LoginTop" ? @"&nbsp;" : @"<br/>";
                lit.Text = separator;
                string lastloggedOn = SnitzTime.TimeAgoTag(page.LastVisitDateTime, page.IsAuthenticated, page.Member);
                if (Lname != null)
                    Lname.FormatString = String.Format(Resources.webResources.lblLoggedOn, HttpContext.Current.User.Identity.Name,separator, lastloggedOn);

            }else
            {
                
                if (login != null)
                {
                    Label uL = (Label) login.FindControl("UserNameLabel");
                    if (uL != null)
                        uL.Text = Resources.webResources.lblUsername;
                    uL = (Label) login.FindControl("PasswordLabel");
                    if (uL != null)
                        uL.Text = Resources.webResources.lblPassword;
                    CheckBox cbx = (CheckBox) login.FindControl("RememberMe");
                    if (cbx != null)
                        cbx.Text = Resources.webResources.lblRememberMe;
                }
            }
            if (login != null)
            {
                RequiredFieldValidator rfv1 = (RequiredFieldValidator) login.FindControl("UserNameRequired");
                rfv1.Text = Resources.webResources.ErrNoUsername;
                RequiredFieldValidator rfv2 = (RequiredFieldValidator) login.FindControl("PasswordRequired");
                rfv2.Text = Resources.webResources.ErrNoPassword;
            }
        }
    }

    protected void LoginAuthenticate(object sender, AuthenticateEventArgs e)
    {
        TextBox uname = (TextBox)login.FindControl("UserName");
        TextBox pword = (TextBox)login.FindControl("Password");

        e.Authenticated = Membership.ValidateUser(uname.Text, pword.Text);

    }

    protected void LO2_LoggedOut(object sender, EventArgs e)
    {
        FormsAuthentication.SignOut();
        Session.Abandon();
        SnitzCookie.LogOut();
        Response.Redirect(Config.ForumUrl,true);
    }

}
