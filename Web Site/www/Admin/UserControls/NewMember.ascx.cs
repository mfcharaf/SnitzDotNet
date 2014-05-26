using System;
using System.Drawing;
using System.IO;
using System.Net.Mail;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Providers;
using SnitzConfig;

public partial class Admin_NewMember : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            roles.DataSource = Roles.GetAllRoles();
            roles.DataBind();
            roles.SelectedValue = "Member";
        }
    }
    protected void ButtonNewUser_Click(object sender, EventArgs e)
    {
        MembershipCreateStatus _return;
        //SnitzMembershipProvider provider = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];


        MembershipUser mu = Membership.CreateUser(tbxUserName.Text, tbxPassword.Text, tbxEmail.Text, ".", ".", cbxApproval.Checked, null, out _return);
        
        switch (_return)
        {
            case MembershipCreateStatus.DuplicateUserName:
                LabelInsertMessage.Text = "Username already exists. Please enter a different user name.";
                break;
            case MembershipCreateStatus.DuplicateEmail:
                LabelInsertMessage.Text = "A username for that e-mail address already exists. Please enter a different e-mail address.";
                break;
            case MembershipCreateStatus.InvalidPassword:
                LabelInsertMessage.Text = "The password provided is invalid. Please enter a valid password value.";
                break;
            case MembershipCreateStatus.InvalidEmail:
                LabelInsertMessage.Text = "The e-mail address provided is invalid. Please check the value and try again.";
                break;
            case MembershipCreateStatus.InvalidAnswer:
                LabelInsertMessage.Text = "The password retrieval answer provided is invalid. Please check the value and try again.";
                break;
            case MembershipCreateStatus.InvalidQuestion:
                LabelInsertMessage.Text = "The password retrieval question provided is invalid. Please check the value and try again.";
                break;
            case MembershipCreateStatus.InvalidUserName:
                LabelInsertMessage.Text = "The user name provided is invalid. Please check the value and try again.";
                break;
            case MembershipCreateStatus.ProviderError:
                LabelInsertMessage.Text = "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                break;
            case MembershipCreateStatus.UserRejected:
                LabelInsertMessage.Text = "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                break;
            case MembershipCreateStatus.Success:
                LabelInsertMessage.Text = "Member " + tbxUserName.Text + " Inserted Successfully.";
                LabelInsertMessage.ForeColor = Color.Green;
                    new SnitzMembershipProvider().UnlockUser(tbxUserName.Text);
                foreach (ListItem item in roles.Items)
                {
                    if(item.Selected)
                        Roles.AddUserToRole(tbxUserName.Text, item.Value);
                }
                    
                    if (cbxSendEmail.Checked)
                        SendEmail(tbxUserName.Text, tbxPassword.Text);
                mu.UnlockUser();
                break;
            default:
                LabelInsertMessage.Text = "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
                break;
        }

        tbxUserName.Text = "";
        tbxPassword.Text = "";
        tbxEmail.Text = "";
        cbxApproval.Checked = false;
        cbxSendEmail.Checked = false;
    }
    private void SendEmail(string user, string password)
    {
        string mailFile = Server.MapPath("~/App_Data/AdminRegisterMail.html");
        string strSubject = "Sent From " + Config.ForumTitle;
        StreamReader file = new StreamReader(mailFile);
        string msgBody = file.ReadToEnd();
        msgBody = msgBody.Replace("<%UserName%>", user);
        msgBody = msgBody.Replace("<%Password%>", password);
        msgBody = msgBody.Replace("<%ForumTitle%>", Config.ForumTitle);
        msgBody = msgBody.Replace("<%ForumUrl%>", Config.ForumUrl);

        MembershipUser mu = Membership.GetUser(user, false);
        snitzEmail mailsender = new snitzEmail
                                    {
                                        toUser = new MailAddress(mu.Email, mu.UserName),
                                        fromUser = "Administrator",
                                        subject = strSubject,
                                        msgBody = msgBody
                                    };
        mailsender.send();

    }

    protected void cbxAutoFill_CheckedChanged(object sender, EventArgs e)
    {
        if(cbxAutoFill.Checked)
        {
            tbxPassword.Text = Membership.GeneratePassword(12, 0);
        }
    }
}
