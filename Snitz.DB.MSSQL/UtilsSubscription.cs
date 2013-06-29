using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using SnitzCommon;
using SnitzConfig;

namespace SnitzData
{
    public static partial class Util
    {
        public static List<MemberSubscription> GetAllSubscriptions()
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                List<MemberSubscription> subscriptions = new List<MemberSubscription>();
                foreach (ForumSubscriptions forumSubscription in dc.ForumSubscriptions)
                {
                    Forum forum = GetForum(forumSubscription.ForumId);
                    Category category = GetCategory(forumSubscription.CategoryId);
                    Member member = GetMember(forumSubscription.MemberId);
                    MemberSubscription ms = new MemberSubscription();
                    ms.CategoryName = category.Name;
                    ms.MemberId = forumSubscription.MemberId;
                    ms.CategoryId = category.Id;
                    ms.MemberName = member.Name;
                    ms.ForumSubject = String.Format("<a href='/Content/Forums/forum.aspx?FORUM={0}'>{1}</a>", forum.Id, forum.Subject);
                    ms.ForumId = forum.Id;
                    if (forumSubscription.TopicId != 0)
                    {
                        Topic topic = Util.GetTopic(forumSubscription.TopicId);
                        ms.Topicid = topic.Id;
                        ms.TopicSubject = String.Format("<a href='/Content/Forums/topic.aspx?TOPIC={0}'>{1}</a>", topic.Id, topic.Subject);
                    }
                    subscriptions.Add(ms);
                }
                return subscriptions.OrderBy(s => s.CategoryId).OrderBy(s => s.ForumId).OrderBy(s => s.Topicid).ToList();
            }
        }

        public static void ProcessSubscriptions(Enumerators.Subscription subType, Topic topic, Reply reply)
        {
            var t = new Thread(() => SendSubscriptions(subType, topic, reply));
            t.IsBackground = true;
            t.Start();

        }

        public static int[] GetTopicSubscriptions(int topicid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                var subs =
                    (from subscrition in dc.ForumSubscriptions
                     where subscrition.TopicId == topicid
                     select subscrition.MemberId);
                return subs.ToArray();
            }
        }

        public static int[] GetForumSubscriptions(int forumid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                var subs =
                    (from subscrition in dc.ForumSubscriptions
                     where subscrition.ForumId == forumid && subscrition.TopicId == 0
                     select subscrition.MemberId);
                return subs.ToArray();
            }
        }

        public static void AddTopicSubscription(int memberid, int topicid)
        {
            Topic topic = GetTopic(topicid);
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                ForumSubscriptions fs = new ForumSubscriptions();
                fs.MemberId = memberid;
                fs.CategoryId = topic.CatId;
                fs.ForumId = topic.ForumId;
                fs.TopicId = topic.Id;
                dc.ForumSubscriptions.InsertOnSubmit(fs);
                dc.SubmitChanges();
            }
        }

        public static void AddForumSubscription(int memberid, int forumid)
        {
            Forum forum = GetForum(forumid);
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                ForumSubscriptions fs = new ForumSubscriptions();
                fs.MemberId = memberid;
                fs.CategoryId = forum.CatId;
                fs.ForumId = forum.Id;
                fs.TopicId = 0;
                dc.ForumSubscriptions.InsertOnSubmit(fs);
                dc.SubmitChanges();
            }
        }

        public static IEnumerable<ForumSubscriptions> GetUserSubscriptions(int userid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                var subs =
                    (from subscrition in dc.ForumSubscriptions
                     where subscrition.MemberId == userid
                     select subscrition);
                return subs.ToList();
            }
        }
        
        public static bool IsUserSubscribedToForum(int userid, int forumid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                var subs =
                    (from subscrition in dc.ForumSubscriptions
                     where subscrition.MemberId == userid &&
                     (subscrition.ForumId == forumid && subscrition.TopicId == 0)
                     select subscrition);
                return subs.Count() > 0;
            }
        }
        
        public static bool IsUserSubscribedToTopic(int userid, int topicid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                var subs =
                    (from subscrition in dc.ForumSubscriptions
                     where subscrition.MemberId == userid &&
                     subscrition.TopicId == topicid
                     select subscrition);
                return subs.Count() > 0;
            }
        }

        public static void RemoveTopicSubscription(int memberid, int topicid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.MemberId == memberid && s.TopicId == topicid));
                dc.SubmitChanges();
            }
        }

        public static void RemoveForumSubscription(int memberid, int forumid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.MemberId == memberid && s.ForumId == forumid));
                dc.SubmitChanges();
            }
        }

        public static void RemoveMemberSubscription(int memberid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.MemberId == memberid));
                dc.SubmitChanges();
            }
        }
        
        
        private static void RemoveAllForumSubscriptions(int forumid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.ForumId == forumid));
                dc.SubmitChanges();
            }
        }
        private static void RemoveAllCategorySubscriptions(int catid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.CategoryId == catid));
                dc.SubmitChanges();
            }
        }
        private static void RemoveAllTopicSubscriptions(int topicid)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumSubscriptions.DeleteAllOnSubmit(dc.ForumSubscriptions.Where(s => s.TopicId == topicid));
                dc.SubmitChanges();
            }
        }
        private static void UpdateTopicSubscription(Topic topic)
        {
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                //update the forumid/catid for any topic subs
                dc.ExecuteCommand("UPDATE FORUM_SUBSCRIPTIONS Set FORUM_ID = {0}, CAT_ID={1} WHERE TOPIC_ID={2}", topic.ForumId, topic.CatId, topic.Id);
            }
        }
        private static void SendSubscriptions(Enumerators.Subscription subType, Topic topic, Reply reply)
        {
            int replyid = -1;
            int authorid = topic.T_AUTHOR.Value;
            int[] memberid = new int[] { };
            StringBuilder Message = new StringBuilder("Hello {0}");
            string strSubject = String.Empty;

            if (reply != null)
            {
                replyid = reply.Id;
                authorid = reply.R_AUTHOR.Value;
            }
            Message.Append("<br/><p>");
            switch (subType)
            {
                case Enumerators.Subscription.ForumSubscription:
                    using (LookupsDataContext dc = new LookupsDataContext())
                    {
                        memberid =
                            (from subs in dc.ForumSubscriptions
                             where subs.ForumId == topic.ForumId && subs.TopicId == 0
                             select subs.MemberId).ToArray();
                    }
                    if (memberid.Length > 0)
                    {
                        strSubject = Config.ForumTitle + " - New posting";

                        Message.AppendFormat(
                            "{0} has posted to the forum {1} at {2} that you requested notification on.",
                            topic.Author.Name, topic.Forum.Subject, Config.ForumTitle);
                    }
                    break;
                case Enumerators.Subscription.TopicSubscription:
                    using (LookupsDataContext dc = new LookupsDataContext())
                    {
                        memberid =
                            (from subs in dc.ForumSubscriptions
                             where subs.TopicId == topic.Id
                             select subs.MemberId).ToArray();
                    }
                    if (memberid.Length > 0)
                    {
                        strSubject = Config.ForumTitle + " - Reply to a posting";
                        Message.AppendFormat("{0} has replied to a topic on <b>{1}</b> that you requested notification to.", reply.Author.Name, Config.ForumTitle);  
                    }

                    break;
            }
            Message.AppendFormat(" Regarding the subject - {0}.", topic.Subject);
            Message.Append("<br/>");
            Message.Append("<br/>");
            Message.AppendFormat("You can view the posting <a href=\"{0}/Content/Forums/topic.aspx?whichpage=-1&TOPIC={1}", Config.ForumUrl, topic.Id);
            if (replyid > 0)
                Message.AppendFormat("#{0}", replyid);
            Message.Append("\">here</a>");
            Message.Append("</p>");
            foreach (int id in memberid)
            {
                Member member = GetMember(id);
                //don't send the author notification of their own posts
                if (id == authorid)
                    continue;
                snitzEmail email = new snitzEmail
                {
                    subject = strSubject,
                    msgBody = String.Format(Message.ToString(), member.Name),
                    toUser = new MailAddress(member.Email, member.Name),
                    fromUser = "Forum Administrator"
                };
                email.send();
            }
        }


    }
}
