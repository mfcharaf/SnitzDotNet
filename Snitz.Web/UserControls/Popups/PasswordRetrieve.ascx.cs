/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - PasswordRetrieve.ascx
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