using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;


public partial class Admin_PendingMembers : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.DataBind();
    }
    protected void approveSel_Click(object sender, EventArgs e)
    {
        StringBuilder usernames = new StringBuilder();

        for (int i = 0; i < GridViewMemberUser.Rows.Count; i++)
        {
            GridViewRow row = GridViewMemberUser.Rows[i];
            CheckBox select = (CheckBox)row.FindControl("chkSelect");
            if (select.Checked)
            {
                usernames.Append(select.ToolTip + ",");
            }

        }
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];

        string[] arrUsers = usernames.ToString().TrimEnd(',').Split(',');
        foreach (string user in arrUsers)
        {
            if (smp != null)
                if (SnitzMembershipProvider.ActivateUser(user))
                {
                    MembershipUser mu = Membership.GetUser(user, false);
                    if (mu != null) mu.UnlockUser();
                    Roles.AddUserToRole(user, "Member");
                    EmailConfirmation(user);
                }
        }
        GridViewMemberUser.DataBind();
    }
    protected void approveAll_Click(object sender, EventArgs e)
    {
        StringBuilder usernames = new StringBuilder();

        for (int i = 0; i < GridViewMemberUser.Rows.Count; i++)
        {
            GridViewRow row = GridViewMemberUser.Rows[i];
            CheckBox select = (CheckBox)row.FindControl("chkSelect");
            usernames.Append(select.ToolTip + ",");
        }
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];

        string[] arrUsers = usernames.ToString().TrimEnd(',').Split(',');
        foreach (string user in arrUsers)
        {
            if (smp != null)
                if (SnitzMembershipProvider.ActivateUser(user))
                {
                    //smp.UnlockUser(user);
                    Roles.AddUserToRole(user, "Member");
                    EmailConfirmation(user);
                }
        }
        GridViewMemberUser.DataBind();
    }
    protected void delSel_Click(object sender, EventArgs e)
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < GridViewMemberUser.Rows.Count; i++)
        {
            GridViewRow row = GridViewMemberUser.Rows[i];
            CheckBox select = (CheckBox) row.FindControl("chkSelect");
            if (select.Checked)
            {
                str.Append(select.ToolTip + ",");
            }

        }

        SnitzProfileProvider pro = (SnitzProfileProvider)ProfileManager.Providers["TableProfileProvider"];
        string usernameList = str.ToString().TrimEnd(',');
        if (pro != null) pro.DeleteProfiles(usernameList.Split(','));

        foreach (string user in usernameList.Split(','))
        {
            var mp = new SnitzMembershipProvider();
            mp.DeleteUser(user, true);
            //Membership.DeleteUser(user,true);
        }
        GridViewMemberUser.DataBind();
    }
    protected void delAll_Click(object sender, EventArgs e)
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < GridViewMemberUser.Rows.Count; i++)
        {
            GridViewRow row = GridViewMemberUser.Rows[i];
            CheckBox select = (CheckBox)row.FindControl("chkSelect");
            str.Append(select.ToolTip + ",");
        }

        SnitzProfileProvider pro = (SnitzProfileProvider)ProfileManager.Providers["TableProfileProvider"];
        string usernameList = str.ToString().TrimEnd(',');
        if (pro != null) pro.DeleteProfiles(usernameList.Split(','));
        foreach (string user in usernameList.Split(','))
        {
            Membership.DeleteUser(user);
        }
        GridViewMemberUser.DataBind();
    }
    protected void EmailConfirmation(string username)
    {
        //todo: load text files from app_data folder;

        MembershipUser mu = Membership.GetUser(username, false);
        SnitzEmail mailsender = new SnitzEmail
                                    {
                                        toUser = new MailAddress(mu.Email, mu.UserName),
                                        FromUser = Resources.extras.lblAdministrator,
                                        subject = Resources.extras.RegApproval,
                                        IsHtml = true,
                                        msgBody = LoadApprovalTemplate(mu)

                                    };
        
        mailsender.Send();

    }

    private string LoadApprovalTemplate(MembershipUser mu)
    {
        string path = Config.CultureSpecificDataDirectory + "RegisterApproval.html";
        var email = File.ReadAllText(Server.MapPath(path));
        
        //<%UserName%>
        //<%OurForum%>
        //<%ForumUrl%>
        return email.Replace("<%UserName%>", mu.UserName).Replace("<%ForumUrl%>", Config.ForumUrl).Replace("<%OurForum%>", Config.ForumTitle);
    }

    protected void RowBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var thisrow = (MemberInfo)(e.Row.DataItem);

        }
    }

    protected void resend_Click(object sender, EventArgs e)
    {
        StringBuilder usernames = new StringBuilder();
        string path = Config.CultureSpecificDataDirectory + "ValidationEmail.html";
        var email = File.ReadAllText(Server.MapPath(path));

        for (int i = 0; i < GridViewMemberUser.Rows.Count; i++)
        {
            GridViewRow row = GridViewMemberUser.Rows[i];
            CheckBox select = (CheckBox)row.FindControl("chkSelect");
            if (select.Checked)
            {
                usernames.Append(select.ToolTip + ",");
            }

        }
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];

        string[] arrUsers = usernames.ToString().TrimEnd(',').Split(',');
        foreach (string user in arrUsers)
        {
            if (smp != null)
            {
                MembershipUser mu = Membership.GetUser(user, false);
                //todo:
                //need to reset password ??
                //email.Replace("<%UserName%>", mu.UserName).Replace("<%OurForum%>", Config.ForumUrl).Replace("<%activationKey%>", mu.PasswordQuestion;);
            }
        }
    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        lblCheckResult.Text = EmailValidator.IsValidEmail(txtCheckEmail.Text.Trim());
    }
}
