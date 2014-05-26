/*
####################################################################################################################
##
## Snitz.BLL - Topics
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzCommon;
using SnitzMembership;


namespace Snitz.BLL
{
    public static class Topics
    {
        public static string LastEditTimeAgo(object topic)
        {
            TopicInfo thistopic = (TopicInfo) topic;

            ProfileCommon profile = ProfileCommon.GetUserProfile(HttpContext.Current.User.Identity.Name);

            if (thistopic.LastEditDate.HasValue && !(thistopic.LastEditDate.Value == DateTime.MinValue))
                return String.Format("<abbr class='timeago' title='{0}'>{1}</abbr>",
                                     thistopic.LastEditDate.Value.ToISO8601Date(HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset),
                                     thistopic.LastEditDate.Value.ToForumDateDisplay(" ", true, HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset));
            else
                return String.Empty;
        }

        public static TopicInfo GetTopic(int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            TopicInfo topic = dal.GetById(topicid);
            if (topic != null)
            {
                topic.Forum = Forums.GetForum(topic.ForumId);
                topic.PollId = GetTopicPollId(topicid);
                topic.IsArchived = false;
            }
            else
            {
                //let's check archives
                IArchiveForum archDal = Factory<IArchiveForum>.Create("ArchiveForums");
                topic = archDal.GetTopic(topicid);
                if (topic != null)
                {
                    topic.Forum = Forums.GetForum(topic.ForumId);
                    topic.IsArchived = true;
                }
            }
            return topic;
        }

        public static IEnumerable<ReplyInfo> GetRepliesForTopic(TopicInfo topic, int startrec, int maxrecs)
        {
            IReply dal = Factory<IReply>.Create("Reply");
            
            return dal.GetByParent(topic,startrec,maxrecs);
        }

        public static TopicInfo GetNextPrevTopic(int topicid, string which)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.GetNextPrevTopic(topicid, which);
        }

        public static void UpdateLastTopicPost(ReplyInfo reply)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.UpdateLastTopicPost(reply);
        }

        public static void SetTopicStatus(int topicid, int status)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.SetTopicStatus(topicid,status);
        }

        public static void MakeSticky(int topicid, bool sticky)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.MakeSticky(topicid,sticky);
        }

        public static void ChangeTopicForum(int topicid, int forumid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.ChangeTopicForum(topicid,forumid);
        }

        public static void UpdateViewCount(int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.UpdateViewCount(topicid);
            
        }

        public static IEnumerable<TopicInfo> GetTopicsForSiteMap(int maxRecords)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.GetTopicsForSiteMap(maxRecords);
        }

        public static int? GetTopicPollId(int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.GetPollId(topicid);
        }

        public static int Add(TopicInfo topic)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.Add(topic);
        }

        public static void Delete(TopicInfo topic)
        {
            Delete(topic.Id);
        }
        public static void Delete(int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            TopicInfo topic = GetTopic(topicid);
            dal.Delete(topic);
        }

        public static void Update(TopicInfo topic)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.Update(topic);
        }

        public static void Update(int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            dal.Update(GetTopic(topicid));
        }

        public static List<TopicInfo> GetLatestTopics(int topicCount)
        {
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");
            ITopic dal = Factory<ITopic>.Create("Topic");

            var topics = dal.GetLatestTopics(topicCount,"");

            List<int> roleList = new List<int> { 0 };
            roleList.AddRange(SnitzCachedLists.UserRoles().Select(role => role.Key));

            List<TopicInfo> allowedTopics = new List<TopicInfo>();
            foreach (var activeTopic in topics)
            {
                activeTopic.Forum = Forums.GetForum(activeTopic.ForumId);

                if (activeTopic.Status == 2 || activeTopic.Status == 3)
                {
                    if (!(isadmin || ismoderator || user == activeTopic.AuthorName))
                        continue;
                }
                if (activeTopic.Forum.AllowedRoles.Count == 0)
                {
                    allowedTopics.Add(activeTopic);
                }
                else
                {
                    if (activeTopic.Forum.AllowedRoles.Any(role => roleList.Contains(role) || isadmin))
                    {
                        allowedTopics.Add(activeTopic);
                    }
                }
            }
            return allowedTopics;
        }

        public static IEnumerable<SearchResult> FindTopics(SearchParamInfo sparams, int currentPage, string orderby, ref int rowcount)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            List<SearchResult> topics = new List<SearchResult>(dal.FindTopics(sparams, currentPage,orderby, ref rowcount));
            return topics;

        }

        public static void Update(int topicid, string message, string subject, MemberInfo member, bool isAdministrator, bool usesig)
        {
            TopicInfo topic = GetTopic(topicid);
            topic.Message = message;
            topic.Subject = subject;
            if (!isAdministrator)
            {
                topic.LastEditedById = member.Id;
                topic.LastEditDate = DateTime.UtcNow;                 
            }
               
            topic.UseSignatures = usesig;
            Update(topic);
        }

        public static int GetNewTopicCount(string lastHereDate, bool isAdminOrModerator, int startRowIndex, int maximumRows)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.GetTopicCount(lastHereDate, startRowIndex, maximumRows, null, isAdminOrModerator, null,true);
        }

        public static List<TopicInfo> GetNewTopics(string lastHereDate, bool isAdminOrModerator, int startRowIndex, int maximumRows)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return new List<TopicInfo>(dal.GetTopics(lastHereDate, startRowIndex, maximumRows, null, isAdminOrModerator, null, true));
        }

        public static void Dispose()
        {

        }

        public static List<SearchResult> GetTopicsBySubject(string topicSubject)
        {
            int rowcount = 0;
            ITopic dal = Factory<ITopic>.Create("Topic");
            SearchParamInfo bysubject = new SearchParamInfo
            {
                MessageAndSubject = false,
                SearchFor = topicSubject,
                Match = null
            };
            return new List<SearchResult>(dal.FindTopics(bysubject, 0, "", ref rowcount));
        }

        public static bool IsArchived(TopicInfo topic)
        {
            return false;
            //todo
        }

        public static bool AllowArchive(TopicInfo topic)
        {
            return false;
            //todo
        }
    }
}
