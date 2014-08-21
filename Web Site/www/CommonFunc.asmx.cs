using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;

namespace SnitzUI
{
    /// <summary>
    /// Summary description for CommonFunc
    /// </summary>
    [WebService(Namespace = "http://forum.snitz.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService()]
    public class CommonFunc : System.Web.Services.WebService
    {

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Moderator")]
        public void Approval(string topicid, string replyid, string jsonform)
        {
            var form = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(form);
            string reason = formresult["ctl00$txtReason"];
            int adminmodid = Convert.ToInt32(formresult["ctl00$hdnModerator"]);

            if (!String.IsNullOrEmpty(topicid))
                Topics.SetTopicStatus(Convert.ToInt32(topicid), (int)Enumerators.PostStatus.Open);
            if (!String.IsNullOrEmpty(replyid))
            {
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.Open);
                ReplyInfo reply = Replies.GetReply(Convert.ToInt32(replyid));
                var topic = Topics.GetTopic(Convert.ToInt32(reply.TopicId));
                topic.UnModeratedReplies -= 1;
                Topics.Update(topic);
            }
            if (!string.IsNullOrEmpty(reason) && Config.UseEmail)
            {
                ProcessModeration(1, Convert.ToInt32(topicid), Convert.ToInt32(replyid), adminmodid, reason);
            }
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Moderator")]
        public void PutOnHold(string topicid, string replyid, string jsonform)
        {
            var form = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(form);
            string reason = formresult["ctl00$txtReason"];
            int adminmodid = Convert.ToInt32(formresult["ctl00$hdnModerator"]);

            if (!String.IsNullOrEmpty(topicid))
                Topics.SetTopicStatus(Convert.ToInt32(topicid), (int)Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(replyid))
                Replies.SetReplyStatus(Convert.ToInt32(replyid), (int)Enumerators.PostStatus.OnHold);
            if (!string.IsNullOrEmpty(reason) && Config.UseEmail)
            {
                ProcessModeration(1, Convert.ToInt32(topicid), Convert.ToInt32(replyid), adminmodid, reason);
            }
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Moderator")]
        public void DeletePost(string topicid, string replyid, string jsonform)
        {
            var form = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(form);
            string reason = formresult["ctl00$txtReason"];
            int adminmodid = Convert.ToInt32(formresult["ctl00$hdnModerator"]);

            if (!String.IsNullOrEmpty(topicid))
                Topics.Delete(Convert.ToInt32(topicid));
            if (!String.IsNullOrEmpty(replyid))
                Replies.DeleteReply(Convert.ToInt32(replyid));
            if (!string.IsNullOrEmpty(reason) && Config.UseEmail)
            {
                ProcessModeration(1, Convert.ToInt32(topicid), Convert.ToInt32(replyid), adminmodid, reason);
            }
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string SendEmail(string name, string email, string message, string subject)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("You must supply a name.");
            }

            if (string.IsNullOrEmpty(email))
            {
                MembershipUser mu = Membership.GetUser(name, false);
                if (mu == null)
                    throw new Exception("You must supply an email address.");
                email = mu.Email;
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Please provide a message to send.");
            }

            string strSubject;
            if (String.IsNullOrEmpty(subject))
                strSubject = "Sent From " + Regex.Replace(Config.ForumTitle, @"&\w+;", "") + " by " + HttpContext.Current.User.Identity.Name;
            else
                strSubject = subject;

            var mailsender = new SnitzEmail
            {
                toUser = new MailAddress(email, name),
                FromUser = HttpContext.Current.User.Identity.Name,
                subject = strSubject,
                IsHtml = false,
                msgBody = message
            };

            mailsender.Send();
            return "Your Email has been sent successfully";
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string SendPrivateMessage(string touser, string message, string subject, string layout)
        {
            string username = HttpContext.Current.User.Identity.Name;
            MembershipUser currentUser = Membership.GetUser(username);
            ProfileCommon profile = ProfileCommon.GetUserProfile(username);
            if (currentUser == null || currentUser.ProviderUserKey == null)
                return null;

            var pm = new PrivateMessageInfo
            {
                Subject = subject,
                Message = message,
                ToMemberId = Convert.ToInt32(touser),
                FromMemberId = (int)currentUser.ProviderUserKey,
                Read = 0,
                OutBox = layout != "none" ? 1 : 0,
                SentDate = DateTime.UtcNow.ToForumDateStr(),
                Mail = profile.PMEmail == null ? 0 : profile.PMEmail.Value
            };
            PrivateMessages.SendPrivateMessage(pm);

            //do we need to send an email
            MembershipUser toUser = Membership.GetUser(Convert.ToInt32(touser));
            if (toUser != null && Config.UseEmail)
            {
                ProfileCommon toprofile = ProfileCommon.GetUserProfile(toUser.UserName);
                if (toprofile.PMEmail.HasValue)
                    if (toprofile.PMEmail.Value == 1)
                    {
                        SnitzEmail notification = new SnitzEmail
                        {
                            FromUser = "Administrator",
                            toUser = new MailAddress(toUser.Email),
                            subject = Regex.Replace(Config.ForumTitle, @"&\w+;", "") + " - New Private message"
                        };
                        string strMessage = "Hello " + toUser.UserName;
                        strMessage = strMessage + username + " has sent you a private message at " + Config.ForumTitle + "." + Environment.NewLine;
                        if (String.IsNullOrEmpty(subject))
                        {
                            strMessage = strMessage + "Regarding - " + subject + "." + Environment.NewLine + Environment.NewLine;
                        }
                        else
                        {
                            strMessage = strMessage + "With the subject entitled - " + message + "." + Environment.NewLine + Environment.NewLine;
                        }

                        notification.msgBody = strMessage;
                        notification.Send();
                    }
            }
            return PrivateMessage.PmSent;
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public string CastVote(string responseid)
        {
            string answerid = responseid;
            if (answerid != null)
            {
                string username = HttpContext.Current.User.Identity.Name;
                MembershipUser currentUser = Membership.GetUser(username);
                if (currentUser != null)
                {
                    bool res = Snitz.BLL.Polls.CastVote(currentUser.ProviderUserKey, Convert.ToInt32(answerid));
                    if (res)
                        return "Your vote was cast";
                }
            }
            throw new Exception("Error casting vote");

        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public string MakePollActive(string responseid)
        {
            int pollid = 0;

            if (Int32.TryParse(responseid, out pollid))
            {
                if (pollid > 0)
                {
                    Config.ActivePoll = pollid;
                    return "Poll marked as Active Poll";
                }
            }
            throw new Exception("Error: Mark as active failed");

        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Moderator")]
        public string SplitTopic(string jsonform)
        {
            var test = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            var replyIDs = new List<int>();
            foreach (string key in formresult.AllKeys)
            {
                if (key.EndsWith("cbxRow"))
                    replyIDs.Add(Convert.ToInt32(formresult[key]));
            }

            int topicid = Convert.ToInt32(formresult["ctl00$splitTopicId"]);
            int forumId = Convert.ToInt32(formresult["ctl00$ddlForum"]);
            string sort = formresult["ctl00$ddlSort"];

            string subject = formresult["ctl00$tbxSubject"];
            if (String.IsNullOrEmpty(subject))
                return "No subject supplied";

            TopicInfo oldtopic = Topics.GetTopic(topicid);
            ForumInfo forum = Forums.GetForum(forumId);

            if (replyIDs.Count == 0)
                return "No replies selected";
            int lastreplyid = sort == "desc" ? replyIDs[replyIDs.Count - 1] : replyIDs[0];
            ReplyInfo lastreply = Replies.GetReply(lastreplyid);

            //get the reply details
            var topic = new TopicInfo
            {
                Subject = subject,
                Message = lastreply.Message,
                Date = lastreply.Date,
                UseSignatures = lastreply.UseSignatures,
                IsSticky = false,
                PosterIp = lastreply.PosterIp,
                Views = 0,
                ReplyCount = replyIDs.Count - 1,
                Status = (int)Enumerators.PostStatus.Open,
                UnModeratedReplies = 0,
                ForumId = forumId,
                AuthorId = lastreply.AuthorId,
                CatId = forum.CatId
            };

            bool isModeratedForum = forum.ModerationLevel != (int)Enumerators.Moderation.UnModerated;
            if (isModeratedForum)
            {
                if (forum.ModerationLevel == (int)Enumerators.Moderation.AllPosts ||
                    forum.ModerationLevel == (int)Enumerators.Moderation.Topics)
                    topic.Status = (int)Enumerators.PostStatus.UnModerated;
            }

            int newtopicid = Topics.Add(topic);
            //delete the reply used as topic
            Replies.DeleteReply(lastreplyid);
            //move the other replies to this topic
            Replies.MoveReplies(newtopicid, replyIDs);
            //update the original topic count/dates
            Topics.Update(oldtopic.Id);

            Snitz.BLL.Admin.UpdateForumCounts();

            return "Selected replies were moved to a new topic";
        }


        private void ProcessModeration(int mode, int topicid, int replyid, int adminmodid, string comments)
        {
            ReplyInfo reply = null;
            MembershipUser author;

            var moderator = Membership.GetUser(adminmodid);

            int forumId;

            var topic = Topics.GetTopic(topicid);
            if (replyid > 0)
            {
                reply = Replies.GetReply(replyid);
            }
            if (reply == null)
            {
                forumId = topic.ForumId;
                author = Membership.GetUser(topic.AuthorId, false);
            }
            else
            {
                forumId = reply.ForumId;
                author = Membership.GetUser(reply.AuthorId, false);
            }
            var forum = Forums.GetForum(forumId);

            var strSubject = new StringBuilder();
            strSubject.AppendFormat("{0} - Your post ", Regex.Replace(Config.ForumTitle, @"&\w+;", ""));
            if (mode == 1)
            {
                strSubject.AppendLine("has been approved ");
            }
            else if (mode == 2)
            {
                strSubject.AppendLine("has been placed on hold ");
            }
            else
            {
                strSubject.AppendLine("has been rejected ");
            }
            var strMessage = new StringBuilder();
            strMessage.AppendFormat("Hello {0}.", author.UserName).AppendLine().AppendLine();
            strMessage.Append("You made a ");

            if (replyid == 0)
                strMessage.Append("post ");
            else
                strMessage.Append("reply to the post ");
            strMessage.AppendFormat("in the {0} forum entitled {1}. {2} has decided to ", forum.Subject, moderator.UserName);

            if (mode == 1)
                strMessage.Append("approve your post ");
            else if (mode == 2)
                strMessage.Append("place your post on hold ");
            else
                strMessage.Append("reject your post ");
            strMessage.AppendLine("for the following reason:").AppendLine();
            strMessage.AppendLine(comments).AppendLine();
            strMessage.AppendFormat("If you have any questions, please contact {0} at {1}", moderator.UserName, moderator.Email).AppendLine();

            var mailsender = new SnitzEmail
            {
                toUser = new MailAddress(author.Email, author.UserName),
                FromUser = "Administrator",
                IsHtml = false,
                subject = strSubject.ToString(),
                msgBody = strMessage.ToString()
            };

            mailsender.Send();
        }

    }
}
