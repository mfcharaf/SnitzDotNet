/*
####################################################################################################################
##
## PrivateMessaging.UserControls.PrivateMessaging - pmSend.ascx
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
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;

namespace SnitzUI.UserControls.PrivateMessaging
{
    public partial class PmSend : UserControl
    {
        public int ToUser { get; set; }
        private string _layout;
        private const string StrCookieUrl = "pmMod";
        private string username = HttpContext.Current.User.Identity.Name;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.Cookies[StrCookieUrl + "paging"] == null || Request.Cookies[StrCookieUrl + "paging"]["outbox"] == null)
            {
                Response.Cookies[StrCookieUrl + "paging"]["outbox"] = "double";
                Response.Cookies[StrCookieUrl + "paging"].Expires = DateTime.UtcNow.AddYears(1);
                _layout = "double";
            }
            else
            {
                _layout = Request.Cookies[StrCookieUrl + "paging"]["outbox"];
            }
        }

        protected void SendPm(object sender, EventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser(username);
            ProfileCommon profile = ProfileCommon.GetUserProfile(username);

            var pm = new PrivateMessageInfo
                         {
                             Subject = tbxSubject.Text,
                             Message = qrMessage.Text,
                             ToMemberId = ToUser,
                             FromMemberId = (int) currentUser.ProviderUserKey,
                             Read = 0,
                             OutBox = _layout != "none" ? 1 : 0,
                             SentDate = DateTime.UtcNow.ToForumDateStr(),
                             Mail = profile.PMEmail == null ? 0 : profile.PMEmail.Value
                         };
            PrivateMessages.SendPrivateMessage(pm);
            pmSuccess.Text = PrivateMessage.PmSent;
            //do we need to send an email
            MembershipUser toUser = Membership.GetUser(ToUser);
            ProfileCommon toprofile = ProfileCommon.GetUserProfile(toUser.UserName);
            if(toprofile.PMEmail.HasValue)
                if (toprofile.PMEmail.Value == 1)
                {
                    snitzEmail notification = new snitzEmail();
                    notification.toUser = new MailAddress(toUser.Email);
                    notification.subject = Config.ForumTitle + " - New Private message";
                    string strMessage = "Hello " + toUser.UserName;
                    strMessage = strMessage + username + " has sent you a private message at " + Config.ForumTitle + "." + Environment.NewLine;
                    if (String.IsNullOrEmpty(tbxSubject.Text))
                    {
                        strMessage = strMessage + "Regarding - " + tbxSubject.Text + "." + Environment.NewLine + Environment.NewLine;
                    }
                    else
                    {
                        strMessage = strMessage + "With the subject entitled - " + tbxSubject.Text + "." + Environment.NewLine + Environment.NewLine; 
                    }

                    notification.msgBody = strMessage;
                    notification.send();
                }


        }
    }

}
