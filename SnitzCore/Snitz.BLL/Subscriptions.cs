/*
####################################################################################################################
##
## Snitz.BLL - Subscriptions
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzConfig;

namespace Snitz.BLL
{
    public static class Subscriptions
    {
        public static void ProcessForumSubscriptions(TopicInfo topic)
        {
            var t = new Thread(() => SendSubscriptions(Enumerators.Subscription.ForumSubscription, topic, null))
                {
                    IsBackground = true
                };
            t.Start();
        }
        public static void ProcessTopicSubscriptions(TopicInfo topic, ReplyInfo reply)
        {
            var t = new Thread(() => SendSubscriptions(Enumerators.Subscription.TopicSubscription, topic, reply))
            {
                IsBackground = true
            };
            t.Start();
        }
        private static void SendSubscriptions(Enumerators.Subscription subType, TopicInfo topic, ReplyInfo reply)
        {
            int replyid = -1;
            int authorid = topic.AuthorId;
            int[] memberids = { };
            StringBuilder Message = new StringBuilder("Hello {0}");
            string strSubject = String.Empty;

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
                        strSubject = Config.ForumTitle + " - New posting";

                        Message.AppendFormat(
                            "{0} has posted to the forum {1} at {2} that you requested notification on.",
                            topic.Author.Username, topic.Forum.Subject, Config.ForumTitle);
                    }
                    break;
                case Enumerators.Subscription.TopicSubscription:
                    
                    memberids = dal.GetTopicSubscriptionList(topic.ForumId);

                    if (memberids.Length > 0)
                    {
                        strSubject = Config.ForumTitle + " - Reply to a posting";
                        Message.AppendFormat("{0} has replied to a topic on <b>{1}</b> that you requested notification to.", reply.Author.Username, Config.ForumTitle);
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
            foreach (int id in memberids)
            {
                MemberInfo member = Members.GetMember(id);
                //don't send the author notification of their own posts
                if (id == authorid)
                    continue;
                snitzEmail email = new snitzEmail
                {
                    subject = strSubject,
                    msgBody = String.Format(Message.ToString(), member.Username),
                    toUser = new MailAddress(member.Email, member.Username),
                    FromUser = "Forum Administrator"
                };
                email.send();
            }
        }

        #region CATEGORY

        public static IEnumerable<SubscriptionInfo> GetCategorySubscriptions(int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.GetCategorySubscriptions(categoryid);
        }

        public static int AddCategorySubscription(int memberid, int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.Add(new SubscriptionInfo { MemberId = memberid, CategoryId = categoryid });
        }

        public static void RemoveCategorySubscription(SubscriptionInfo subscription)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.Delete(subscription.Id);
        }

        public static void RemoveAllCategorySubscriptions(int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllCategorySubscriptions(categoryid);
        }

        #endregion

        #region FORUM

        public static IEnumerable<SubscriptionInfo> GetForumSubscriptions(int forumid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.GetForumSubscriptions(forumid);
        }

        public static int AddForumSubscription(int memberid, int forumid)
        {
            ForumInfo forum = Forums.GetForum(forumid);
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.Add(new SubscriptionInfo { MemberId = memberid, ForumId = forumid, CategoryId = forum.CatId, TopicId = 0 });
        }

        public static void RemoveAllForumSubscriptions(int forumid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllForumSubscriptions(forumid);
        }

        public static void RemoveForumSubscription(int memberid, int forumid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveMembersForumSubscriptions(memberid, forumid);
        }

        #endregion

        #region TOPICS

        public static List<SubscriptionInfo> GetTopicSubscriptions(int topicid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return new List<SubscriptionInfo>(dal.GetTopicSubscriptions(topicid));
        }

        public static void AddTopicSubscription(int memberid, int topicid)
        {
            TopicInfo topic = Topics.GetTopic(topicid);
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.Add(new SubscriptionInfo { MemberId = memberid, ForumId = topic.ForumId, CategoryId = topic.CatId, TopicId = topicid });

        }

        public static void RemoveTopicSubscription(int memberid, int topicid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveMembersTopicSubscription(memberid,topicid);
        }

        public static void RemoveAllTopicSubscriptions(int topicid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllTopicSubscriptions(topicid);
        }

        #endregion

        public static void RemoveMemberSubscriptions(int memberid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllMemberSubscriptions(memberid);
        }
        public static List<SubscriptionInfo> GetMemberSubscriptions(int memberid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return new List<SubscriptionInfo>(dal.GetMemberSubscriptions(memberid));
        }

    }
}
