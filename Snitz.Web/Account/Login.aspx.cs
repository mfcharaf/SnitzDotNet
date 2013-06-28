using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzUI.UserControls;
using SnitzCommon;
using SnitzConfig;


public partial class Login : PageBase
{
    protected override void OnPreInit(EventArgs e)
    {
        Page.Theme = Config.DefaultTheme;
    }
    protected void Page_Load(object sender, EventArgs e)
    {
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

    }
}
