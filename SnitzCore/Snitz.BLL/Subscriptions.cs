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

        #region CATEGORY

        public static IEnumerable<SubscriptionInfo> GetCategorySubscriptions(int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.GetCategorySubscriptions(categoryid);
        }

        public static int AddCategorySubscription(int memberid, int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.Add(new SubscriptionInfo { MemberId = memberid, CategoryId = categoryid,ForumId = 0,TopicId = 0});
        }

        public static void RemoveCategorySubscription(SubscriptionInfo subscription)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.Delete(subscription.Id);
        }
        public static void RemoveCategorySubscription(int memberid, int categoryid)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveMembersCategorySubscriptions(memberid, categoryid);
        }
        public static void RemoveAllCategorySubscriptions(int categoryid, bool deletingcat)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllCategorySubscriptions(categoryid,deletingcat);
        }
        public static void RemoveAllCategorySubscriptions()
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllCategorySubscriptions();
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

        public static void RemoveAllForumSubscriptions(int forumid, bool deletingforum)
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllForumSubscriptions(forumid, deletingforum);
        }
        public static void RemoveAllForumSubscriptions()
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllForumSubscriptions();
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
        public static void RemoveAllTopicSubscriptions()
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllTopicSubscriptions();
        }
        #endregion

        public static void RemoveAllSubscriptions()
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            dal.RemoveAllSubscriptions();            
        }
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
