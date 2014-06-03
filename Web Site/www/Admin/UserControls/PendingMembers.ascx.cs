using System;
using System.Net.Mail;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Providers;
using SnitzConfig;


public partial class Admin_PendingMembers : UserControl
{
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
                    Roles.AddUserToRole(user, "Member");
                    //smp.UnlockUser(user);
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
            Membership.DeleteUser(user);
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
        //string strSubject = "Registration Approval";

        MembershipUser mu = Membership.GetUser(username, false);
        snitzEmail mailsender = new snitzEmail
                                    {
                                        toUser = new MailAddress(mu.Email, mu.UserName),
                                        FromUser = Resources.extras.lblAdministrator,
                                        subject = Resources.extras.RegApproval,
                                        msgBody =
                                            String.Format(
                                                Resources.extras.RegApprovalMsg.Replace("[br]", Environment.NewLine),
                                                Config.ForumUrl)
                                    };

        mailsender.send();

    }

}
