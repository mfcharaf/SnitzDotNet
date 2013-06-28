using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using SnitzCommon;

namespace SnitzData
{
    /// <summary>
    /// Utility class for Registration
    /// </summary>
    public class RegisterMember
    {
        private SnitzDataClassesDataContext dc;

        public RegisterMember()
        {
            dc = new SnitzDataClassesDataContext();
        }

        public Member GetMember(string user)
        {
            if (String.IsNullOrEmpty(user))
                return null;

            return (from member in dc.Members where member.Name == user select member).SingleOrDefault();
        }
        
        public void SaveMember(Member member)
        {
            try
            {
                dc.SubmitChanges(ConflictMode.ContinueOnConflict);
            }

            catch (ChangeConflictException e)
            {
                Console.WriteLine(e.Message);
                foreach (ObjectChangeConflict occ in dc.ChangeConflicts)
                {
                    // All database values overwrite current values.
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
            dc.SubmitChanges(ConflictMode.FailOnFirstConflict);

        }

        public IQueryable GetRecentTopics(int memberid, Member member)
        {

            TimeSpan ts = new TimeSpan(30, 0, 0, 0);
            DateTime startDate = DateTime.UtcNow - ts;

            var topics = (from topic in dc.Topics
                          join reply in dc.Replies on topic.Id equals reply.TopicId into recentposts
                          from posts in recentposts.DefaultIfEmpty()
                          where (topic.T_LAST_POST).CompareTo(startDate.ToForumDateStr()) > 0
                          where posts.R_AUTHOR == memberid || topic.T_AUTHOR == memberid
                          where posts.R_STATUS < 2 || topic.T_STATUS < 2
                          select topic).Distinct().OrderByDescending(t=>t.T_LAST_POST);
            int[] allowed = member.AllowedForums;
            return topics.Where(t=> allowed.Contains(t.ForumId)).Take(10);
        }

    }
}
