using System;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using SnitzCommon;



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
        string separator = "<br/>";

        if (!Request.Path.EndsWith("register.aspx"))
        {
            PageBase page = (PageBase)this.Page;

            login = (System.Web.UI.WebControls.Login)LoginView1.FindControl("Login1");

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                LoginName Lname = (LoginName)LoginView1.FindControl("ln2");
                Literal lit = (Literal) LoginView1.FindControl("Literal1");
                separator = _skinid == "LoginTop" ? @"&nbsp;" : @"<br/>";
                lit.Text = separator;
                string lastloggedOn = Common.TimeAgoTag(page.LastVisitDateTime, page.IsAuthenticated, page.Member != null ? page.Member.TimeOffset : 0);
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

    }

}
