/*
####################################################################################################################
##
## Snitz.IDAL - ITopic
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
    /// <summary>
    /// Topic Interface
    /// </summary>
    public interface ITopic : IBaseObject<TopicInfo>
    {
        IEnumerable<ReplyInfo> GetReplies(TopicInfo topic,int startrec,int maxrecs);
        IEnumerable<TopicInfo> GetTopicsForSiteMap(int maxRecords);
        IEnumerable<TopicInfo> GetLatestTopics(int topicCount, string lastvisit);
        IEnumerable<TopicInfo> GetTopics(string lastHereDate, int startRowIndex, int maximumRows, int? forumid, bool isAdminOrModerator, int? topicstatus, bool sticky, int? userid);
        IEnumerable<SearchResult> FindTopics(SearchParamInfo sparams, int currentPage, string orderby, ref int rowcount);
        IEnumerable<int> GetReplyIdList(int topic);

        TopicInfo GetNextPrevTopic(int topicid, string which);
        int GetTopicCount(string lastHereDate, int startRowIndex, int maximumRows, int? forumid, bool isAdminOrModerator, int? topicstatus, bool sticky, int? userid);
        int? GetPollId(int topicid);

        void UpdateLastTopicPost(ReplyInfo reply);
        void SetTopicStatus(int topicid, int status);
        void MakeSticky(int topicid, bool sticky);
        void ChangeTopicForum(int topicid, int forumid);
        void UpdateViewCount(int topicid);

    }
}
