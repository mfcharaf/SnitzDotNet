using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using Snitz.Providers;
using SnitzCommon;

namespace SnitzData
{
    /// <summary>
    /// PagedObjects
    /// Methods for fetching pages of data
    /// </summary>
    [DataObject(true)]
    public static class PagedObjects
    {
        /// <summary>
        /// Fetches a page of topics for a given forum
        /// </summary>
        /// <param name="topicstatus"></param>
        /// <param name="fromdate">oldest date of topics to return</param>
        /// <param name="forumid">Id of the Forum</param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        public static IEnumerable<Topic> GetForumTopicsPaged(int? topicstatus, string fromdate, int forumid, int startRowIndex, int maximumRows)
        {
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");
            int userid = 0;
            MembershipUser mu = Membership.GetUser(user, false);

            if (mu != null) if (mu.ProviderUserKey != null) userid = (int)mu.ProviderUserKey;


            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var topicsQuery = from t in dc.Topics where t.ForumId == forumid && !t.IsSticky select t;
            if (topicstatus != null)
            {
                topicsQuery = topicsQuery.Where(t => t.T_STATUS == topicstatus.Value);
            }
            else
            {
                if (!(isadmin || ismoderator))
                    topicsQuery = topicsQuery.Where(t =>
                        (t.T_STATUS != ((int)Enumerators.PostStatus.UnModerated) &&
                        t.T_STATUS != ((int)Enumerators.PostStatus.OnHold)) || t.T_AUTHOR == userid
                        );           

            }
            if (fromdate != null)
            {
                topicsQuery = topicsQuery.Where(t => t.T_LAST_POST.CompareTo(fromdate) > 0);
            }
            if (HttpContext.Current.Session["ForumSearch"] != null)
            if (!String.IsNullOrEmpty(HttpContext.Current.Session["ForumSearch"].ToString()))
            {
                string lookfor = HttpContext.Current.Session["ForumSearch"].ToString();
                topicsQuery = topicsQuery.Where(t => t.Subject.Contains(lookfor) || t.Message.Contains(lookfor));
            }
            return (IEnumerable<Topic>)topicsQuery
                                     .OrderByDescending(d => d.T_LAST_POST)
                                     .Skip(startRowIndex)
                                     .Take(maximumRows);
        }
        /// <summary>
        /// Returns the total topic for the paged topic query
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fromdate"></param>
        /// <param name="forumid"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        public static int GetForumTopicCount(int? topicstatus, string fromdate, int forumid, int startRowIndex, int maximumRows)
        {
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");
            int userid = 0;
            MembershipUser mu = Membership.GetUser(user, false);

            if (mu != null) if (mu.ProviderUserKey != null) userid = (int)mu.ProviderUserKey;

            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var topicsQuery = from t in dc.Topics where t.ForumId == forumid && !t.IsSticky select t;
            if (topicstatus != null)
            {
                topicsQuery = topicsQuery.Where(t => t.T_STATUS == topicstatus.Value);
            }
            else
            {
                if (!(isadmin || ismoderator))
                    topicsQuery = topicsQuery.Where(t =>
                        (t.T_STATUS != ((int)Enumerators.PostStatus.UnModerated) &&
                        t.T_STATUS != ((int)Enumerators.PostStatus.OnHold)) || t.T_AUTHOR == userid
                        );

            }
            if (fromdate != null)
            {
                topicsQuery = topicsQuery.Where(t => t.T_LAST_POST.CompareTo(fromdate) > 0);
            }
            if (HttpContext.Current.Session["ForumSearch"] != null)
                if (!String.IsNullOrEmpty(HttpContext.Current.Session["ForumSearch"].ToString()))
                {
                    string lookfor = HttpContext.Current.Session["ForumSearch"].ToString();
                    topicsQuery = topicsQuery.Where(t => t.Subject.Contains(lookfor) || t.Message.Contains(lookfor));
                }
            int res = topicsQuery.Count();
            return res;
        }

        /// <summary>
        /// Fetches a page of active topics
        /// </summary>
        /// <param name="lastHereDate"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        public static IEnumerable<Topic> GetActiveTopicsPaged(string lastHereDate, int startRowIndex, int maximumRows)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");

            var topicsQuery = from t in dc.Topics
                              where t.T_LAST_POST.CompareTo(lastHereDate) > 0
                              join c in dc.Categories on t.CatId equals c.Id
                              join f in dc.Forums on t.ForumId equals f.Id
                              orderby c.Order, c.Id, f.Order, f.Id, t.T_LAST_POST descending
                              select t;
            List<int> RoleList = new List<int> { 0 };

            foreach (KeyValuePair<int, string> _role in SnitzCachedLists.UserRoles())
            {
                RoleList.Add(_role.Key);
            }
            List<Topic> allowedTopics = new List<Topic>();
            foreach (var activeTopic in topicsQuery)
            {
                if (activeTopic.Status == Enumerators.PostStatus.UnModerated || activeTopic.Status == Enumerators.PostStatus.OnHold)
                {
                    if (!(isadmin || ismoderator || user == activeTopic.Author.Name))
                        continue;
                }
                if (activeTopic.Forum.AllowedRoles.Count == 0)
                {
                    allowedTopics.Add(activeTopic);
                }
                else
                {
                    foreach (int role in activeTopic.Forum.AllowedRoles)
                    {
                        if (RoleList.Contains(role) || isadmin)
                        {
                            allowedTopics.Add(activeTopic);
                            break;
                        }
                    }
                }
            }
            return allowedTopics

                .Skip(startRowIndex)
                .Take(maximumRows);
            //.OrderBy(c => c.Category.Order).OrderBy(f => f.Forum.Id);
        }

        public static IEnumerable<Topic> GetActiveTopics(int maximumRows)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");

            var topicsQuery = from t in dc.Topics
                              join c in dc.Categories on t.CatId equals c.Id
                              join f in dc.Forums on t.ForumId equals f.Id
                              orderby t.T_LAST_POST descending
                              select t;
            List<int> RoleList = new List<int> { 0 };

            foreach (KeyValuePair<int, string> _role in SnitzCachedLists.UserRoles())
            {
                RoleList.Add(_role.Key);
            }
            List<Topic> allowedTopics = new List<Topic>(maximumRows);
            int succesfulrows = 0;
            foreach (var activeTopic in topicsQuery)
            {
                if (activeTopic.Status == Enumerators.PostStatus.UnModerated || activeTopic.Status == Enumerators.PostStatus.OnHold)
                {
                    if (!(isadmin || ismoderator || user == activeTopic.Author.Name))
                        continue;
                }
                if (activeTopic.Forum.AllowedRoles.Count == 0)
                {
                    allowedTopics.Add(activeTopic);
                    succesfulrows++;
                }
                else
                {
                    foreach (int role in activeTopic.Forum.AllowedRoles)
                    {
                        if (RoleList.Contains(role) || isadmin)
                        {
                            allowedTopics.Add(activeTopic);
                            succesfulrows++;
                            break;
                        }
                    }
                }
                if (succesfulrows > maximumRows)
                    break;
            }
            return allowedTopics
                .Take(maximumRows);
            //.OrderBy(c => c.Category.Order).OrderBy(f => f.Forum.Id);
        }
        /// <summary>
        /// Returns the total topic count for the paged active topic query
        /// </summary>
        /// <param name="lastHereDate"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        public static int GetActiveTopicCount(string lastHereDate, int startRowIndex, int maximumRows)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");

            var topicsQuery = from t in dc.Topics where t.T_LAST_POST.CompareTo(lastHereDate) > 0 select t;

            List<int> RoleList = new List<int> { 0 };

            foreach (KeyValuePair<int, string> _role in SnitzCachedLists.UserRoles())
            {
                RoleList.Add(_role.Key);
            }
            List<Topic> allowedTopics = new List<Topic>();
            foreach (var activeTopic in topicsQuery)
            {
                if (activeTopic.Status == Enumerators.PostStatus.UnModerated || activeTopic.Status == Enumerators.PostStatus.OnHold)
                {
                    if (!(isadmin || ismoderator || user == activeTopic.Author.Name))
                        continue;
                }
                if (activeTopic.Forum.AllowedRoles.Count == 0)
                    allowedTopics.Add(activeTopic);
                else
                {
                    foreach (int role in activeTopic.Forum.AllowedRoles)
                    {
                        if (RoleList.Contains(role) || isadmin)
                            allowedTopics.Add(activeTopic);
                    }
                }
            }
            int res = allowedTopics.Count();
            return res;
        }

        /// <summary>
        /// Fetches a page of replies for a topic
        /// </summary>
        /// <param name="topicid">Id of the topic</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static IEnumerable<Reply> GetTopicRepliesPaged(int topicid, int pageIndex, int pageSize)
        {
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");
            int userid = 0;
            MembershipUser mu = Membership.GetUser(user, false);

            if (mu != null) if (mu.ProviderUserKey != null) userid = (int)mu.ProviderUserKey;
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var repliesQuery = from t in dc.Replies where t.TopicId == topicid select t;

            if(!(isadmin || ismoderator))
            {
                repliesQuery = repliesQuery.Where(r =>
                                                  (r.R_STATUS != ((int) Enumerators.PostStatus.UnModerated) &&
                                                   r.R_STATUS != ((int)Enumerators.PostStatus.OnHold)) || r.R_AUTHOR == userid);
            }

            return (IEnumerable<Reply>)repliesQuery
                                        .OrderBy(r=>r.R_DATE)
                                     .Skip(pageIndex * pageSize)
                                     .Take(pageSize);
        }
        /// <summary>
        /// Returns the total reply count for the paged topic reply query
        /// </summary>
        /// <param name="topicid"></param>
        /// <returns></returns>
        public static int GetTopicReplyCount(int topicid)
        {
            string user = HttpContext.Current.User.Identity.Name;
            bool isadmin = Roles.IsUserInRole(user, "Administrator");
            bool ismoderator = Roles.IsUserInRole(user, "Moderator");
            int userid = 0;
            MembershipUser mu = Membership.GetUser(user, false);

            if (mu != null) if (mu.ProviderUserKey != null) userid = (int)mu.ProviderUserKey;

            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var repliesQuery = (from t in dc.Replies where t.TopicId == topicid select t);
            if (!(isadmin || ismoderator))
            {
                repliesQuery = repliesQuery.Where(r =>
                                                  (r.R_STATUS != ((int)Enumerators.PostStatus.UnModerated) &&
                                                   r.R_STATUS != ((int)Enumerators.PostStatus.OnHold)) || r.R_AUTHOR == userid);
            }
            return repliesQuery.Count();
        }

        /// <summary>
        /// Fetches a page of members
        /// </summary>
        /// <param name="SortExpression"></param>
        /// <param name="StartRecord"></param>
        /// <param name="MaxRecords"></param>
        /// <returns></returns>
        public static List<Member> GetAllMembersPaged(string SortExpression, int StartRecord, int MaxRecords)
        {
            bool descending = false;
            bool isAdmin = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");
            if (SortExpression.Contains("DESC"))
            {
                descending = true;
                SortExpression = SortExpression.Replace(" DESC", "");
            }

            using (var dc = new SnitzDataClassesDataContext())
            {
                var members = from m in dc.Members select m;
                members = !isAdmin ? members.Where(m => m.IsActive == 1) : members.Where(m => m.Name != "n/a");
                if (HttpContext.Current.Session["SearchFilter"] != null)
                {
                    string filter = HttpContext.Current.Session["SearchFilter"].ToString();
                    members = members.Where(filter);
                }

                switch (SortExpression)
                {
                    case "PostCount":
                        members = descending
                                      ? members.OrderByDescending(m => m.M_POSTS)
                                      : members.OrderBy(m => m.M_POSTS);
                        break;
                    case "Title":
                        members = descending
                                      ? members.OrderByDescending(m => m.Rank.Title)
                                      : members.OrderBy(m => m.Rank.Title);
                        break;
                    case "Date":
                        members = descending ? members.OrderByDescending(m => m.Date) : members.OrderBy(m => m.Date);
                        break;
                    case "LastPostDate":
                        members = descending
                                      ? members.OrderByDescending(m => m.M_LASTPOSTDATE ?? "")
                                      : members.OrderBy(m => m.M_LASTPOSTDATE ?? "");
                        break;
                    case "Country":
                        members = descending
                                      ? members.OrderByDescending(m => m.Country)
                                      : members.OrderBy(m => m.Country);
                        break;
                    case "LastVisitDate":
                        members = descending
                                      ? members.OrderByDescending(m => m.M_LASTUPDATED)
                                      : members.OrderBy(m => m.M_LASTUPDATED);
                        break;
                    case "Name":
                        members = descending ? members.OrderByDescending(m => m.Name) : members.OrderBy(m => m.Name);
                        break;
                    default:
                        members = members.OrderByDescending(m => m.M_POSTS);
                        break;
                }
                var page = (IQueryable<Member>) members.Skip(StartRecord).Take(MaxRecords);

                return page.ToList();
            }
        }
        /// <summary>
        /// Returns the total member count for the paged member query
        /// </summary>
        /// <returns></returns>
        public static int SelectMemberCount()
        {
            bool isAdmin = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");

            var dc = new SnitzDataClassesDataContext();
            IQueryable<Member> members = from m in dc.Members orderby m.Date select m;
            members = !isAdmin ? members.Where(m => m.IsActive == 1) : members.Where(m => m.Name != "n/a");
            if (HttpContext.Current.Session["SearchFilter"] != null)
            {
                string filter = HttpContext.Current.Session["SearchFilter"].ToString();
                members = members.Where(filter);
            }
            return members.Count();
        }

        public static IEnumerable<Member> GetPendingMembers(int startrecord, int maxrecords)
        {
            bool isAdmin = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");


            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var members = from m in dc.Members where m.Validated == 0 select m;

            members = members.OrderByDescending(m => m.Date);
            if (HttpContext.Current.Session["SearchFilter"] != null)
            {
                string filter = HttpContext.Current.Session["SearchFilter"].ToString();
                members = members.Where(filter);
            }

            return (IEnumerable<Member>)members
                                     .Skip(startrecord)
                                     .Take(maxrecords);            
        }

        public static int GetPendingCount()
        {
            using(SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return (from m in dc.Members where m.Validated == 0 select m).Count();
            }
        }

    }
}
