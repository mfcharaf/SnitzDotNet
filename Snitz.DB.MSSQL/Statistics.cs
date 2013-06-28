using System.Linq;
using System.Web;
using SnitzCommon;

namespace SnitzData
{
    public class Stats
    {
        public static int TopicCount
        {
            get {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                return (from topic in dc.Topics select topic).Count();
            }
        }

        public static int ArchiveTopicCount
        {
            get { return 0; }
        }

        public static int ArchiveReplyCount
        {
            get { return 0; }
        }

        public static int ActiveMembers
        {
            get { SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            return (from author in dc.Members where author.IsActive == 1 select author).Count();
            }
        }

        public static int MemberCount
        {
            get
            {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                return (from author in dc.Members select author).Count();
            }
        }

        public static int TotalPostCount
        {
            get {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                return (from topic in dc.Topics where topic.T_STATUS != (int)Enumerators.PostStatus.UnModerated select topic).Count() + (from reply in dc.Replies where reply.R_STATUS != (int)Enumerators.PostStatus.UnModerated select reply).Count();
            }
        }

        public static object LastPostAuthor
        {
            get {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                IQueryable<Topic> lasttopic = (IQueryable<Topic>)(from topics in dc.Topics where topics.T_STATUS != (int)Enumerators.PostStatus.UnModerated orderby topics.T_DATE descending select topics).Take(1);
                IQueryable<Reply> lastreply = (IQueryable<Reply>)(from replies in dc.Replies where replies.R_STATUS != (int)Enumerators.PostStatus.UnModerated orderby replies.R_DATE select replies).Take(1);
                if (lastreply.SingleOrDefault() == null && lasttopic.SingleOrDefault() == null)
                    return null;
                if(lastreply.SingleOrDefault() == null)
                {
                    return lasttopic.SingleOrDefault().Author;
                }
                else
                {
                    return lastreply.SingleOrDefault().Date > lasttopic.SingleOrDefault().Date ? lastreply.SingleOrDefault().Author : lasttopic.SingleOrDefault().Author;
                }
                
            }

        }

        public static Topic LastPost
        {
            get {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                IQueryable<Topic> topic = (IQueryable<Topic>)(from topics in dc.Topics where topics.T_STATUS != (int)Enumerators.PostStatus.UnModerated orderby topics.T_LAST_POST descending select topics).Take(1);
                return topic.SingleOrDefault();
            }
        }

        public static int ActiveTopicCount
        {
            get
            {
                SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
                string sincedate = HttpContext.Current.Session["_LastVisit"].ToString();
                return (from topic in dc.Topics where ((Enumerators.PostStatus)topic.T_STATUS == Enumerators.PostStatus.Open) && topic.T_LAST_POST.CompareTo(sincedate) > 0 select topic).Count();
            }
        }

        public static string NewestMember
        {
            get
            {
                var dc = new SnitzDataClassesDataContext();

                var member = dc.Members.OrderByDescending(m => m.Date).First();


                return member.Name;
            }

        }
    }
}
