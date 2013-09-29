/*
####################################################################################################################
##
## Snitz.BLL - Forums
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
using System.Web.Caching;
using System.Web.Security;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzMembership.Helpers;

namespace Snitz.BLL
{
    public static class Forums
    {
        /// <summary>
        /// A method to search forums by category id
        /// </summary>
        /// <param name="categoryId">The category id to search by</param>
        /// <param name="startrec"></param>
        /// <param name="maxrecs"></param>
        /// <returns>An interface to an arraylist of the search results</returns>
        public static IEnumerable<ForumInfo> GetForumsByCategory(int categoryId, int startrec, int maxrecs)
        {

            // Return null if the string is empty
            if (categoryId < 1)
                return null;

            // Get an instance of the Product DAL using the DALFactory
            IForum dal = Factory<IForum>.Create("Forum");
            // Run a search against the data store
            return dal.GetByParent(categoryId);
        }

        /// <summary>
        /// A method to return a specific forum
        /// </summary>
        /// <param name="id">id of forum</param>
        /// <returns>A ForumInfo object</returns>
        public static ForumInfo GetForum(int id)
        {

            // Return null if the string is empty
            if (id < 1)
                return null;

            // Get an instance of the Product DAL using the DALFactory
            IForum dal = Factory<IForum>.Create("Forum");

            // Run a search against the data store
            return dal.GetById(id);
        }

        public static IEnumerable<ForumInfo> GetAllForums()
        {
            IForum dal = Factory<IForum>.Create("Forum");
            return dal.GetAll();
        }

        public static IEnumerable<ForumInfo> GetForumBySubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return null;
            IForum dal = Factory<IForum>.Create("Forum");
            return dal.GetByName(subject);
        }

        public static int AddForum(ForumInfo forum)
        {
            if (forum == null)
                return -1;
            IForum dal = Factory<IForum>.Create("Forum");
            return dal.Add(forum);
        }

        public static void DeleteForum(ForumInfo forum)
        {
            if (forum == null)
                return;
            IForum dal = Factory<IForum>.Create("Forum");
            dal.Delete(forum);
        }

        public static void UpdateForum(ForumInfo forum)
        {
            if (forum == null)
                return;
            IForum dal = Factory<IForum>.Create("Forum");
            dal.Update(forum);
        }

        public static List<ForumModeratorInfo> GetForumModerators(int forumid)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            return new List<ForumModeratorInfo>(dal.GetByParent(forumid));
        }

        public static void AddForumModerator(int forumid, int memberid)
        {
            ForumModeratorInfo moderator = new ForumModeratorInfo {ForumId = forumid, MemberId = memberid};

            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            dal.Add(moderator);
        }

        public static void RemoveForumModerator(ForumModeratorInfo moderator)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            dal.Delete(moderator);
        }

        public static void SetForumStatus(int forumid, int status)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            dal.SetForumStatus(forumid,status);
        }

        public static void DeleteForum(int forumid)
        {
            ForumInfo forum = GetForum(forumid);
            IForum dal = Factory<IForum>.Create("Forum");
            dal.Delete(forum);
            Admin.UpdateForumCounts();
        }

        public static void EmptyForum(int forumid)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            dal.EmptyForum(forumid);
            Admin.UpdateForumCounts();
        }

        public static void AddForumModerators(int forumid, string[] moderators)
        {
            //todo: needs testing
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            foreach (string moderator in moderators)
            {
                ForumModeratorInfo mod = new ForumModeratorInfo
                    {
                        ForumId = forumid,
                        MemberId = Convert.ToInt32(moderator)
                    };
                dal.Add(mod);
            }
        }

        public static int SaveForum(ForumInfo forum)
        {
            if (forum.Id < 0)
            {
                forum.Id = AddForum(forum);
            }
            else
            {
                UpdateForum(forum);
            }
            return forum.Id;
        }

        public static List<TopicInfo> GetForumTopicsSince(bool isAdminOrModerator, int? forumid, int? topicstatus, string fromdate, int startRowIndex, int maximumRows)
        {
            if (maximumRows == 0)
                return new List<TopicInfo>();
            ITopic dal = Factory<ITopic>.Create("Topic");
            return new List<TopicInfo>(dal.GetTopics(fromdate, startRowIndex, maximumRows, forumid, isAdminOrModerator, topicstatus));
        }

        public static int GetForumTopicsSinceCount(bool isAdminOrModerator, int forumid, int? topicstatus, string fromdate, int startRowIndex, int maximumRows)
        {
            if (maximumRows == 0)
                return 0;
            ITopic dal = Factory<ITopic>.Create("Topic");
            return dal.GetTopicCount(fromdate, startRowIndex, maximumRows, forumid, isAdminOrModerator, topicstatus);
        }
        
        public static List<ForumJumpto> ListForumJumpTo()
        {
            IForum dal = Factory<IForum>.Create("Forum");
            // Run a search against the data store
            return new List<ForumJumpto>(dal.ListForumJumpTo());
        }

        public static List<TopicInfo> GetStickyTopics(int id)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            return new List<TopicInfo>(dal.GetStickyTopics(id));
        }

        public static List<TopicInfo> GetForumTopics(int forumid, int startrec, int maxrecs)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            return new List<TopicInfo>(dal.GetTopics(null, startrec, maxrecs, forumid,false,-1));
        }

        public static List<int> AllowedForums(MemberInfo member)
        {
            List<int> roleList = new List<int> { 0 };
            bool isadmin = Roles.IsUserInRole("Administrator");
            roleList.AddRange(SnitzCachedLists.UserRoles().Select(role => role.Key));

            IMember dal = Factory<IMember>.Create("Member");
            return new List<int>(dal.GetAllowedForumIds(member, roleList,isadmin));

        }

        public static List<KeyValuePair<int,string>> AllowedForumsList(MemberInfo member)
        {
            List<int> roleList = new List<int> { 0 };
            bool isadmin = Roles.IsUserInRole("Administrator");
            roleList.AddRange(SnitzCachedLists.UserRoles().Select(role => role.Key));

            IMember dal = Factory<IMember>.Create("Member");
            return new List<KeyValuePair<int, string>>(dal.GetAllowedForumList(member, roleList, isadmin));
        }

        public static List<ForumModeratorInfo> GetAvailableModerators(int forumId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            return new List<ForumModeratorInfo>(dal.GetAvailableModerators(forumId));
        }

        public static string[] GetForumRoles(int id)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            return dal.GetForumRoles(id);
        }

        public static bool IsUserInForumRole(string username, int forumid)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            string[] userroles = BusinessUtil.GetRolesForUser(username);

            string[] forumroles = dal.GetForumRoles(forumid);

            foreach (string forumrole in forumroles)
            {
                if (userroles.Contains(forumrole))
                    return true;
            }
            return false;
        }

    }
}
