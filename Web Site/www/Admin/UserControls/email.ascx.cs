using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Web;
using System.Web.Configuration;
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
        rblLogonForEmail.SelectedValue = Config.LogonForEmail ? "1" : "0";

        rblSMTPAuth.SelectedValue = Config.EmailAuthenticate ? "1" : "0";

        tbxMailUser.Text = Config.EmailAuthUser;
        tbxMailPwd.Text = Config.EmailAuthPwd;
        tbxMailServer.Text = Config.EmailHost;
        tbxAdminEmail.Text = Config.AdminEmail;
        tbxPort.Text = Config.EmailPort.ToString();
        tbxMailServer.Enabled = true;
        tbxMailPwd.Enabled = true;
        tbxMailUser.Enabled = true;
        tbxAdminEmail.Enabled = true;
        
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();

        if (Config.UseEmail != (rblEmail.SelectedValue == "1"))
            toUpdate.Add("UseEmail".GetPropertyDescription(), rblEmail.SelectedValue);
        if (Config.LogonForEmail != (rblLogonForEmail.SelectedValue == "1"))
            toUpdate.Add("LogonForEmail".GetPropertyDescription(), rblLogonForEmail.SelectedValue);

        Config.UpdateKeys(toUpdate);

        Configuration config = WebConfigurationManager.OpenWebConfiguration(
        HttpContext.Current.Request.ApplicationPath);
        SmtpSection settings =
            (SmtpSection)config.GetSection("system.net/mailSettings/smtp");

        settings.From = tbxAdminEmail.Text.Trim();
        if (rblSMTPAuth.SelectedValue == "1")
        {
            settings.Network.UserName = tbxMailUser.Text.Trim();
            settings.Network.Password = tbxMailPwd.Text.Trim();
        }
        else
        {
            settings.Network.UserName = "";
            settings.Network.Password = "";
        }
        settings.Network.Host = tbxMailServer.Text.Trim();
        settings.Network.Port = int.Parse(tbxPort.Text.Trim());
        //settings.Network.EnableSsl = CheckBoxSSL.Checked;
        config.Save();

    }
}
