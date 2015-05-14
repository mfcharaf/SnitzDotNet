using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using SnitzCommon;
using SnitzConfig;

public partial class Admin_system : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            GetValues();
    }
    void GetValues()
    {
        tbxTitle.Text = Config.ForumTitle;
        tbxCopyright.Text = Config.Copyright;
        tbxHomeUrl.Text = Config.HomeUrl;
        tbxForumUrl.Text = Config.ForumUrl;
        rbForum.Checked = Config.CookiePath != "/";
        ddTheme.SelectedValue = Config.DefaultTheme;
        tbxVersion.Text = SnitzBase.Version.Current;
        chkRightColum.Checked = Config.ShowRightColumn;
        chkShowHeaderAds.Checked = Config.ShowHeaderAds;
        chkShowSideAds.Checked = Config.ShowSideAds;
        chkGoogleAds.Checked = Config.ShowGoogleAds;
        tbxAdCode.Text = Config.GoogleAdCode;
        rblNoNewMembers.SelectedValue = Config.ProhibitNewMembers ? "1" : "0";
        rblRequireReg.SelectedValue = Config.RequireRegistration ? "1" : "0";
        rblUserFilter.SelectedValue = Config.FilterUsernames ? "1" : "0";
        rblThemeChange.SelectedValue = Config.AllowThemeChange ? "1" : "0";
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();
        if (rbForum.Checked)
        {
            Config.CookiePath = HttpContext.Current.Request.ApplicationPath;
        }
        else
        {
            Config.CookiePath = "/";
        }
        if (Config.ShowRightColumn != chkRightColum.Checked)
            toUpdate.Add("ShowRightColumn".GetPropertyDescription(), chkRightColum.Checked ? "1" : "0");
        if (Config.ShowGoogleAds != chkGoogleAds.Checked)
            toUpdate.Add("ShowGoogleAds".GetPropertyDescription(), chkGoogleAds.Checked ? "1" : "0");
        if (Config.ShowHeaderAds != chkShowHeaderAds.Checked)
            toUpdate.Add("ShowHeaderAds".GetPropertyDescription(), chkShowHeaderAds.Checked ? "1" : "0");
        if (Config.ShowSideAds != chkShowSideAds.Checked)
            toUpdate.Add("ShowSideAds".GetPropertyDescription(), chkShowSideAds.Checked ? "1" : "0");
        if (Config.GoogleAdCode != tbxAdCode.Text)
            toUpdate.Add("GoogleAdCode".GetPropertyDescription(), tbxAdCode.Text);

        if (Config.ForumTitle != tbxTitle.Text)
            toUpdate.Add("ForumTitle".GetPropertyDescription(), tbxTitle.Text);
        if (Config.Copyright != tbxCopyright.Text)
            toUpdate.Add("Copyright".GetPropertyDescription(), tbxCopyright.Text);
        if (Config.HomeUrl != tbxHomeUrl.Text)
            toUpdate.Add("HomeUrl".GetPropertyDescription(), tbxHomeUrl.Text);
        if (Config.ForumUrl != tbxForumUrl.Text)
            toUpdate.Add("ForumUrl".GetPropertyDescription(), tbxForumUrl.Text);
        if (Config.DefaultTheme != ddTheme.SelectedValue)
            toUpdate.Add("DefaultTheme".GetPropertyDescription(), ddTheme.SelectedValue);

        if (Config.ProhibitNewMembers != (rblNoNewMembers.SelectedValue == "1"))
            toUpdate.Add("ProhibitNewMembers".GetPropertyDescription(), rblNoNewMembers.SelectedValue);
        if (Config.RequireRegistration != (rblRequireReg.SelectedValue == "1"))
            toUpdate.Add("RequireRegistration".GetPropertyDescription(), rblRequireReg.SelectedValue);
        if (Config.FilterUsernames != (rblUserFilter.SelectedValue == "1"))
            toUpdate.Add("FilterUsernames".GetPropertyDescription(), rblUserFilter.SelectedValue);


        Config.UpdateKeys(toUpdate);
    }
}
