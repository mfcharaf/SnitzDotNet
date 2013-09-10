using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzMembership.Helpers;

namespace Snitz.BLL
{
    public static class Moderators 
    {
        public static Dictionary<int, string> GetUnModeratedForumsIdNameList(int memberId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");

            var forums = dal.GetUnModeratedForums(memberId);
            Dictionary<int, string> myDictionary = new Dictionary<int, string>();
            foreach (var forum in forums)
            {
                if (!myDictionary.Keys.Contains(forum.Id))
                    myDictionary.Add(forum.Id, forum.Subject);
            }
            return myDictionary;
        }

        public static Dictionary<int, string> GetModeratedForumsIdNameList(int memberId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            var forums = dal.GetModeratedForums(memberId);
            Dictionary<int, string> myDictionary = new Dictionary<int, string>();
            foreach (var forum in forums)
            {
                if (!myDictionary.Keys.Contains(forum.Id))
                    myDictionary.Add(forum.Id, forum.Subject);
            }
            return myDictionary;
        }

        public static List<MemberInfo> GetAvailableModerators(int forumId)
        {
            List<MemberInfo> moderators = new List<MemberInfo>();
            foreach (ForumModeratorInfo moderator in Forums.GetAvailableModerators(forumId))
            {
                moderators.Add(Members.GetMember(moderator.MemberId));
            }
            return moderators;
        }

        public static List<ForumInfo> ListUnModeratedForums(int memberId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            return dal.GetUnModeratedForums(memberId);
        }

        public static List<ForumInfo> ListModeratedForums(int memberId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            return dal.GetModeratedForums(memberId);
        }

        public static void SetForumModerators(int forumId, int[] userList)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<int, string> GetCurrentModeratorsIdName(int forumId)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<int, string> GetAvailableModeratorsIdName(int forumId)
        {
            
            var moderators = Forums.GetAvailableModerators(forumId);
            Dictionary<int, string> myDictionary = new Dictionary<int, string>();
            foreach (var moderator in moderators)
            {
                if (!myDictionary.Keys.Contains(moderator.Id))
                    myDictionary.Add(moderator.Id, Members.GetMember(moderator.MemberId).Username);
            }
            return myDictionary;
        }

        public static void SetUserAsModeratorForForums(int memberId, int[] forumList)
        {
            throw new NotImplementedException();
        }

        public static bool IsUserInForumRole(string username, int forumid)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            string[] userroles = BusinessUtil.GetRolesForUser(username);

            string[] forumroles = dal.GetForumRoles(forumid);

            foreach (string forumrole in forumroles)
            {
                if (userroles.Contains(forumrole))
                    return true;
            }
            return false;
        }

        public static bool IsUserForumModerator(string currentUser, int forumId)
        {
            IForumModerator dal = Factory<IForumModerator>.Create("ForumModerator");
            var member = Members.GetMember(currentUser);
            return dal.IsUserForumModerator(member.Id, forumId);
        }
    }
}
