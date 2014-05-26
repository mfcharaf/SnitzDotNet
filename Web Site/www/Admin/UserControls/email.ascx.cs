using System;
using System.Collections.Generic;
using System.Web.UI;
using SnitzCommon;
using SnitzConfig;

public partial class Admin_email : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
            GetValues();
    }
    void GetValues()
    {
        rblEmail.SelectedValue = Config.UseEmail ? "1" : "0";
        rblEmailVal.SelectedValue = Config.EmailValidation ? "1" : "0";
        rblLogonForEmail.SelectedValue = Config.LogonForEmail ? "1" : "0";
        rblRestrictReg.SelectedValue = Config.RestrictRegistration ? "1" : "0";
        rblUniqueEmail.SelectedValue = Config.UniqueEmail ? "1" : "0";

        rblSMTPAuth.SelectedValue = Config.EmailAuthenticate ? "1" : "0";

        tbxMailUser.Text = Config.EmailAuthUser;
        tbxMailPwd.Text = Config.EmailAuthPwd;
        tbxMailServer.Text = Config.EmailHost;
        tbxAdminEmail.Text = Config.AdminEmail;
        tbxMailServer.Enabled = false;
        tbxMailPwd.Enabled = false;
        tbxMailUser.Enabled = false;
        tbxAdminEmail.Enabled = false;
        rblSMTPAuth.Enabled = "false";
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();

        if (Config.UseEmail != (rblEmail.SelectedValue == "1"))
            toUpdate.Add("UseEmail".GetPropertyDescription(), rblEmail.SelectedValue);
        if (Config.EmailValidation != (rblEmailVal.SelectedValue == "1"))
            toUpdate.Add("EmailValidation".GetPropertyDescription(), rblEmailVal.SelectedValue);
        if (Config.LogonForEmail != (rblLogonForEmail.SelectedValue == "1"))
            toUpdate.Add("LogonForEmail".GetPropertyDescription(), rblLogonForEmail.SelectedValue);
        if (Config.RestrictRegistration != (rblRestrictReg.SelectedValue == "1"))
            toUpdate.Add("RestrictRegistration".GetPropertyDescription(), rblRestrictReg.SelectedValue);
        if (Config.UniqueEmail != (rblUniqueEmail.SelectedValue == "1"))
            toUpdate.Add("UniqueEmail".GetPropertyDescription(), rblUniqueEmail.SelectedValue);
        Config.UpdateKeys(toUpdate);

    }
}
