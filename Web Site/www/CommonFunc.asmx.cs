using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;
using SnitzUI.UserControls;

namespace SnitzUI
{
    /// <summary>
    /// Snitz web service functions for Ajax calls
    /// </summary>
    [WebService(Namespace = "http://forum.snitz.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService()]
    public class CommonFunc : WebService
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

        [WebMethod(EnableSession = true)]
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

        [WebMethod(EnableSession = true)]
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

        [WebMethod(EnableSession = true)]
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

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        [PrincipalPermission(SecurityAction.Demand, Role = "Moderator")]
        public void SaveForum(string jsonform)
        {
            var test = HttpUtility.UrlDecode(jsonform);
            bool forumMoved = false;
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            int forumid = Convert.ToInt32(formresult["ctl00$hdnForumId"]);
            ForumInfo forum = forumid == -1 ? new ForumInfo { Id = -1, Status = 1 } : Forums.GetForum(forumid);
            forum.SubscriptionLevel = 0;
            forum.ModerationLevel = 0;
            var roles = new string[] { };
            var moderators = new string[] { };
            var removemoderators = new string[] { };
            string password = "";

            if (!formresult.AllKeys.Contains("ctl00$cbxCountPost"))
                forum.UpdatePostCount = false;
            if (!formresult.AllKeys.Contains("ctl00$cbxAllowPolls"))
                forum.AllowPolls = false;
            forum.Type = 0;
            try
            {
                foreach (string key in formresult.AllKeys)
                {
                    //ctl00$
                    if (key != null)
                        switch (key.Replace("ctl00$", ""))
                        {
                            case "ddlCat":
                                int currentid = forum.CatId;
                                forum.CatId = Convert.ToInt32(formresult[key]);
                                if (forum.CatId != currentid)
                                    forumMoved = true;
                                break;
                            case "tbxUrl":
                                forum.Url = formresult[key];
                                if (!String.IsNullOrEmpty(forum.Url))
                                    forum.Type = 1;
                                break;
                            case "tbxSubject":
                                forum.Subject = formresult[key];
                                break;
                            case "tbxBody":
                                forum.Description = formresult[key];
                                break;
                            case "cbxCountPost":
                                forum.UpdatePostCount = formresult[key] == "on";
                                break;
                            case "tbxOrder":
                                forum.Order = Convert.ToInt32(formresult[key]);
                                break;
                            case "ddlMod":
                                forum.ModerationLevel = Convert.ToInt32(formresult[key]);
                                break;
                            case "ddlSub":
                                forum.SubscriptionLevel = Convert.ToInt32(formresult[key]);
                                break;
                            case "hdnRoleList":
                                roles = formresult[key].ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                break;
                            case "cbxAllowPolls":
                                forum.AllowPolls = formresult[key] == "on";
                                break;
                            case "cbxBugReport":
                                if (formresult[key] == "on")
                                    forum.Type = (int)Enumerators.ForumType.BugReports;
                                break;
                            case "cbxBlogPosts":
                                if (formresult[key] == "on")
                                    forum.Type = (int)Enumerators.ForumType.BlogPosts;
                                break;
                            case "hdnModerators":
                                moderators = formresult[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                break;
                            case "hdnRemoveMods":
                                removemoderators = formresult[key].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                break;
                            case "tbxPassword":
                                password = formresult[key];
                                break;
                        }
                }
            }
            catch (Exception)
            {

            }

            forum.Password = password.Trim();

            int newId = Forums.SaveForum(forum);
            if (forumMoved)
                Forums.MoveForumPosts(forum);
            SnitzRoleProvider.AddRolesToForum(newId, roles);
            Forums.AddForumModerators(newId, moderators,removemoderators);

        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void SaveCategory(string jsonform)
        {
            var test = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            int catid = Convert.ToInt32(formresult["ctl00$hdnCatId"]);
            CategoryInfo cat = catid == -1 ? new CategoryInfo { Id = -1 } : Categories.GetCategory(catid);
            foreach (string key in formresult.AllKeys)
            {
                //ctl00$
                switch (key.Replace("ctl00$", ""))
                {
                    case "tbxSubject":
                        cat.Name = formresult[key];
                        break;
                    case "tbxOrder":
                        cat.Order = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlMod":
                        cat.ModerationLevel = Convert.ToInt32(formresult[key]);
                        break;
                    case "ddlSub":
                        cat.SubscriptionLevel = Convert.ToInt32(formresult[key]);
                        break;
                    //case "cbxPassword":
                    //    if (formresult[key] != null)
                    //        forum.UpdatePostCount = formresult[key] == "1";
                    //    break;
                    //case "ddlAuthType":
                    //    forum.AuthType = (Enumerators.ForumAuthType)Convert.ToInt32(formresult[key]);
                    //    break;
                }

            }
            Categories.UpdateCategory(cat);
        }

        [WebMethod]
        [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
        public void SaveForumOrder(string jsonform)
        {
            var test = HttpUtility.UrlDecode(jsonform);
            System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            
            Dictionary<int,int> catList = new Dictionary<int, int>();
            Dictionary<int, int> forumList = new Dictionary<int, int>();

            var orderkeys = formresult.AllKeys.Where(k => k.StartsWith("ctl00$rptCatOrder"));
            foreach (string key in orderkeys)
            {
                //ctl00$rptCatOrder
                var stripkey = key.Replace("ctl00$rptCatOrder", "");
                if (stripkey.EndsWith("hdnCatOrderId"))
                {
                    var orderkey = key.Replace("hdnCatOrderId", "cOrder");
                    int catid = Convert.ToInt32(formresult[key]);
                    int catorder = Convert.ToInt32(formresult[orderkey]);
                    catList.Add(catid,catorder);
                }
                if (stripkey.Contains("rptForumOrder"))
                {
                    if (stripkey.EndsWith("hdnForumOrderId"))
                    {
                        var orderkey = key.Replace("hdnForumOrderId", "fOrder");
                        int forumid = Convert.ToInt32(formresult[key]);
                        int forumorder = Convert.ToInt32(formresult[orderkey]);
                        forumList.Add(forumid,forumorder);
                    }
                }
            }
            Categories.UpdateOrder(catList);
            Forums.UpdateOrder(forumList);
        }

        [WebMethod]
        public bool CheckUserName(string userName)
        {
            Thread.Sleep(500);
            var membercheck = Membership.GetUser(userName);
            if ((membercheck != null) && membercheck.UserName.ToLower() != "guest")
            {
                return true;
            }
            if (!IsNameAllowed(userName))
                return true;
            return false;
        }

        [WebMethod]
        public bool CheckEmail(string email)
        {
            Thread.Sleep(500);
            if (!String.IsNullOrEmpty(Membership.GetUserNameByEmail(email)))
            {
                return true;
            }
            return false;
        }

        [WebMethod(EnableSession = true)]
        public string GetForums(string categoryId)
        {
            Page page = new Page();

            CategoryForums ctl = (CategoryForums)page.LoadControl("~/UserControls/CategoryForums.ascx");

            ctl.Member = Members.GetMember(HttpContext.Current.User.Identity.Name);
            ctl.CategoryId = categoryId;
            page.Controls.Add(ctl);
            HtmlForm tempForm = new HtmlForm();
            tempForm.Controls.Add(ctl);
            page.Controls.Add(tempForm);
            StringWriter writer = new StringWriter();
            HttpContext.Current.Server.Execute(page, writer, false);
            string outputToReturn = writer.ToString();
            outputToReturn = outputToReturn.Substring(outputToReturn.IndexOf("<div"));
            outputToReturn = outputToReturn.Substring(0, outputToReturn.IndexOf("</form>"));
            writer.Close();
            string viewStateRemovedOutput = Regex.Replace(outputToReturn,
            "<input type=\"hidden\" name=\"__VIEWSTATE\" id=\"__VIEWSTATE\" value=\".*?\" />",
            "", RegexOptions.IgnoreCase);
            return viewStateRemovedOutput;
        }

        #region bookmarks
        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void BookMarkTopic(int topicid)
        {
            var user = HttpContext.Current.User.Identity.Name;
            ProfileCommon prof = ProfileCommon.GetUserProfile(user);

            TopicInfo t = Topics.GetTopic(topicid);
            string url = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}", t.Id);
            List<SnitzLink> bookmarks = prof.BookMarks;
            if (!bookmarks.Contains(new SnitzLink(t.Subject, url, 0)))
            {
                bookmarks.Add(new SnitzLink(t.Subject, url, bookmarks.Count));
                prof.BookMarks = bookmarks;
                prof.Save();
            }
        }
        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void BookMarkReply(int replyid,int page)
        {
            ReplyInfo r = Replies.GetReply(replyid);
            TopicInfo rt = Topics.GetTopic(r.TopicId);
            var user = HttpContext.Current.User.Identity.Name;
            ProfileCommon prof = ProfileCommon.GetUserProfile(user);

            string rurl = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&whichpage={1}&#{2}", r.TopicId, page + 1, r.Id);
            List<SnitzLink> rbookmarks = prof.BookMarks;
            if (!rbookmarks.Contains(new SnitzLink(rt.Subject, rurl, 0)))
            {
                rbookmarks.Add(new SnitzLink(rt.Subject, rurl, rbookmarks.Count));
                prof.BookMarks = rbookmarks;
                prof.Save();
            }
        }
        #endregion

        #region subscriptions
        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void CategorySubscribe(int catid, bool remove)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var member = Membership.GetUser(user, true);
            if (member != null && member.ProviderUserKey != null)
            {
                if (remove)
                    Subscriptions.RemoveCategorySubscription((int)member.ProviderUserKey, catid);
                else
                    Subscriptions.AddCategorySubscription((int)member.ProviderUserKey, catid);
            }
        }
        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void ForumSubscribe(int forumid, bool remove)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var member = Membership.GetUser(user, true);
            if(member != null && member.ProviderUserKey != null)
            {
                if (remove)
                    Subscriptions.RemoveForumSubscription((int) member.ProviderUserKey, forumid);
                else
                    Subscriptions.AddForumSubscription((int)member.ProviderUserKey, forumid);
            }
        }
        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public void TopicSubscribe(int topicid, bool remove)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var member = Membership.GetUser(user, true);
            if (member != null && member.ProviderUserKey != null)
            {
                if (remove)
                    Subscriptions.RemoveTopicSubscription((int) member.ProviderUserKey, topicid);
                else
                    Subscriptions.AddTopicSubscription((int) member.ProviderUserKey, topicid);
            }
            
        }

        [SoapDocumentMethod(OneWay = true)]
        [WebMethod(EnableSession = true)]
        public void ProcessForumSubscriptions(int topicid, HttpContext context)
        {
            TopicInfo topic = Topics.GetTopic(topicid);
            HttpContext.Current = context;
            var t = new Thread(() => SendSubscriptions(Enumerators.Subscription.ForumSubscription, topic, null,context))
            {
                IsBackground = true
            };
            t.Start();         
        }

        [SoapDocumentMethod(OneWay = true)]
        [WebMethod(EnableSession = true)]
        public void ProcessTopicSubscriptions(int topicid, int replyid,HttpContext context)
        {
            TopicInfo topic = Topics.GetTopic(topicid);
            ReplyInfo reply = Replies.GetReply(replyid);
            
            var t = new Thread(() => SendSubscriptions(Enumerators.Subscription.TopicSubscription, topic, reply,context))
            {
                IsBackground = true
            };
            t.Start();            
        }
        #endregion

        [WebMethod(EnableSession = true)]
        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public object[] ExecuteCommand(string dialogName, string targetMethod, object data)
        {
            try
            {
                object[] result = new object[2];
                result[0] = Command.Create(dialogName).Execute(data);
                result[1] = targetMethod;
                return result;
            }
            catch
            {
                // TODO: add logging functionality 
                throw;
            }
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
        private static bool IsNameAllowed(string username)
        {
            if (Filters.GetAllNameFilters().Any(name => name.Name == username))
            {
                return false;
            }
            return username.ReplaceBadWords() == username;
        }
        private static void SendSubscriptions(Enumerators.Subscription subType, TopicInfo topic, ReplyInfo reply,HttpContext context)
        {
            int replyid = -1;
            int authorid = topic.AuthorId;
            int[] memberids = { };
            StringBuilder Message = new StringBuilder("<html><body>Hello {0}");
            string strSubject = String.Empty;
            HttpContext.Current = context;
            if (reply != null)
            {
                replyid = reply.Id;
                authorid = reply.AuthorId;
            }
            Message.Append("<br/><p>");

            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            switch (subType)
            {
                case Enumerators.Subscription.ForumSubscription:
                    
                    memberids = dal.GetForumSubscriptionList(topic.ForumId);

                    if (memberids.Length > 0)
                    {
                        strSubject = Config.ForumTitle.Replace("&trade;", "").Replace("&copy;", "") + " - New posting";

                        Message.AppendFormat(
                            "{0} has posted to the forum {1} at {2} that you requested notification on.",
                            topic.AuthorName, topic.Forum.Subject, Config.ForumTitle);
                    }
                    break;
                case Enumerators.Subscription.TopicSubscription:

                    memberids = dal.GetTopicSubscriptionList(topic.Id);

                    if (memberids.Length > 0)
                    {
                        strSubject = Config.ForumTitle.Replace("&trade;", "").Replace("&copy;", "") + " - Reply to a posting";
                        Message.AppendFormat("{0} has replied to a topic on <b>{1}</b> that you requested notification to.", reply.AuthorName, Config.ForumTitle);
                    }

                    break;
            }
            Message.AppendFormat(" Regarding the subject - {0}.", topic.Subject);
            Message.Append("<br/>");
            Message.Append("<br/>");
            Message.AppendFormat("You can view the posting <a href=\"{0}Content/Forums/topic.aspx?whichpage=-1&TOPIC={1}", Config.ForumUrl, topic.Id);
            if (replyid > 0)
                Message.AppendFormat("#{0}", replyid);
            Message.Append("\">here</a>");
            Message.Append("</p></body></html>");
            foreach (int id in memberids)
            {
                MemberInfo member = Members.GetMember(id);
                //don't send the author notification of their own posts
                if (id == authorid)
                    continue;
                SnitzEmail email = new SnitzEmail
                {
                    subject = strSubject,
                    msgBody = String.Format(Message.ToString(), member.Username),
                    toUser = new MailAddress(member.Email, member.Username),
                    IsHtml = true,
                    FromUser = "Forum Administrator"
                };
                email.Send();
            }
        }

    }
}
