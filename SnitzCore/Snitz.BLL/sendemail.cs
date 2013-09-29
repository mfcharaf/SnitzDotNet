/*
####################################################################################################################
##
## Snitz.BLL - sendemail
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
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using Snitz.Entities;
using SnitzConfig;

namespace Snitz.BLL
{
    /// <summary>
    /// Summary description for sendemail
    /// </summary>
    public class snitzEmail
    {
        private MemberInfo user;
        private static MailAddress _toUser;
        private static MailAddress _replyTo;
        private static MailAddress _fromUser;
        private static string _subject;
        private static string _msgBody;

        public MailAddress toUser
        {
            set
            {
                _toUser = value;
            }
        }
        public string fromUser
        {
            set
            {

                if (value.Contains("Administrator"))
                {
                    _fromUser = new MailAddress(Config.AdminEmail, Config.ForumTitle);
                    _replyTo = new MailAddress(Config.AdminEmail, Config.ForumTitle);
                }
                else
                {
                    user = Members.GetMember(value);
                    _replyTo = new MailAddress(user.Email, user.Username);
                    _fromUser = new MailAddress(Config.AdminEmail, Config.ForumTitle);
                }
            }
        }
        public string subject
        {
            set
            {
                _subject = value;
            }
        }
        public string msgBody
        {
            set
            {
                _msgBody = value;
            }
        }

        public void send()
        {

            MailMessage message = new MailMessage {IsBodyHtml = true, From = _fromUser, ReplyTo = _replyTo};
            message.To.Add(_toUser);
            message.Subject = _subject;
            message.Body = "<html><body>" + _msgBody + "</body></html>";
            SmtpClient client = new SmtpClient {Host = Config.EmailHost, Port = Config.EmailPort};
            if (Config.EmailAuthenticate)
            {
                NetworkCredential credential = new NetworkCredential(Config.EmailAuthUser, Config.EmailAuthPwd);
                client.Credentials = credential;
            }
            //client.SendCompleted += MailSent;
            client.Send(message);
        }

        private void MailSent(object sender, AsyncCompletedEventArgs e)
        {
            MailMessage msg = (MailMessage) e.UserState;
            string result = "";

            if (e.Cancelled)
                result = String.Format("Send canceled for mail with subject [{0}].",msg.Subject);
            else
                result = e.Error != null ? String.Format("Error {1} occurred when sending mail [{0}] ",msg.Subject,e.Error) : String.Format("Message [{0}] sent.", msg.Subject);

            //could maybe send a PM to user informing them of the mail status or write to a log etc. etc.
            //PrivateMessageInfo pm = new PrivateMessageInfo();
            //pm.Message = result;
            //pm.Subject = "Send mail result";
            //pm.FromMemberId = Config.AdminUserId;
            //pm.SentDate = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            //pm.ToMemberId = Members.GetMember(msg.ReplyTo.DisplayName).Id;
            //pm.OutBox = 0;
            //PrivateMessages.SendPrivateMessage(pm);
        }
    }
}
