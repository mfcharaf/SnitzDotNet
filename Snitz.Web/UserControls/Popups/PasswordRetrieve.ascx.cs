using System;
using System.Web.UI.WebControls;
using SnitzCommon;
using SnitzConfig;

namespace SnitzUI.UserControls.Popups
{
    public partial class PasswordRetrieve : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            recover.MailDefinition.From = Config.AdminEmail;
            recover.MailDefinition.BodyFileName = Config.CultureSpecificDataDirectory + "PasswordReset.txt";
            recover.MailDefinition.Subject = String.Format("{0} : {1} {2}", Config.ForumTitle, Resources.webResources.lblPassword, Resources.webResources.btnReset);

        }

        protected void RecoverSendingMail(object sender, MailMessageEventArgs e)
        {
            string memberIP = Common.GetIPAddress();
            e.Message.Body = e.Message.Body.Replace("[IPAddress]", memberIP).Replace("[forumTitle]", Config.ForumTitle);
        }
    }
}