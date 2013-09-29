/*
####################################################################################################################
##
## Snitz.IDAL - ISubscription
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

using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.IDAL
{
    public interface ISubscription
    {
        IEnumerable<SubscriptionInfo> GetTopicSubscriptions(int topicid);
        IEnumerable<SubscriptionInfo> GetForumSubscriptions(int forumid);
        IEnumerable<SubscriptionInfo> GetCategorySubscriptions(int catid);
        IEnumerable<SubscriptionInfo> GetBoardSubscriptions();
        IEnumerable<SubscriptionInfo> GetMemberSubscriptions(int memberid);
        IEnumerable<SubscriptionInfo> GetAllSubscriptions();
        int Add(SubscriptionInfo subscription);
        int[] GetForumSubscriptionList(int forumId);
        int[] GetTopicSubscriptionList(int forumId);
        void RemoveAllCategorySubscriptions(int categoryid);
        void RemoveAllForumSubscriptions(int forumid);
        void RemoveAllTopicSubscriptions(int topicid);
        void RemoveAllBoardSubscriptions();
        void RemoveAllMemberSubscriptions(int memberid);
        void Delete(int subscriptionid);
        void RemoveMembersForumSubscriptions(int memberid, int forumid);
        void RemoveMembersTopicSubscription(int memberid, int topicid);
        
    }
}
