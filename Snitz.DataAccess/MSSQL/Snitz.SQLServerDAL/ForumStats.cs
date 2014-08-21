using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class ForumStats : IForumStats
    {
        private string TOPIC_SELECT = "SELECT TOP(1) TOPIC_ID,CAT_ID,FORUM_ID,T_STATUS,T_SUBJECT,T_AUTHOR,T_REPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_IP,T_LAST_POST_AUTHOR,T_STICKY,T_LAST_EDIT,T_LAST_EDITBY,T_SIG,T_LAST_POST_REPLY_ID,T_UREPLIES,T_MESSAGE FROM " + Config.ForumTablePrefix + "TOPICS ORDER BY T_LAST_POST DESC";

        private string ACTIVE_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_STATUS=1; ";
        private string ACTIVE_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS WHERE T_STATUS=1 AND T_LAST_POST > @LastVisit; ";
        private string ARCHIVED_REPLY = "SELECT COUNT(REPLY_ID) FROM " + Config.ForumTablePrefix + "A_REPLY; ";
        private string ARCHIVED_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "A_TOPICS; ";
        private string TOTAL_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS; ";
        private string TOTAL_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS; ";
        private string TOTAL_POSTS = "SELECT ((SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS)+(SELECT COUNT(REPLY_ID) FROM " + Config.ForumTablePrefix + "REPLY)); ";

        public StatsInfo GetStatistics(string lastvisit)
        {
            StatsInfo stats = new StatsInfo();
            SqlParameter parm = new SqlParameter("@LastVisit", SqlDbType.VarChar) {Value = lastvisit};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, ACTIVE_MEMBERS + ACTIVE_TOPICS + ARCHIVED_REPLY + ARCHIVED_TOPICS + TOTAL_MEMBERS + TOTAL_TOPICS + TOTAL_POSTS, parm))
            {
                while (rdr.Read())
                {
                    stats.ActiveMembers = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.ActiveTopicCount = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.ArchiveReplyCount = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.ArchiveTopicCount = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.MemberCount = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.TopicCount = rdr.GetInt32(0);
                }
                rdr.NextResult();
                while (rdr.Read())
                {
                    stats.TotalPostCount = rdr.GetInt32(0);
                }
            }

            stats.LastPost = GetLastPost();
            if (stats.LastPost != null)
                if (stats.LastPost.LastPostAuthorId != null)
                    stats.LastPostAuthor = GetLastPostAuthor(stats.LastPost.LastPostAuthorId.Value);
            stats.NewestMember = GetNewestMember();

            return stats;
        }

        private AuthorInfo GetLastPostAuthor(int authorId)
        {
            var member = new Member().GetById(authorId);
            AuthorInfo author = new AuthorInfo(member);
            return author;
        }

        private string GetNewestMember()
        {
            return (string)SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, "SELECT TOP(1) M_NAME FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_VALID=1 AND M_STATUS=1 ORDER BY M_DATE DESC", null);
            
        }

        private TopicInfo GetLastPost()
        {
            TopicInfo topic = null;

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, TOPIC_SELECT , null))
            {
                while (rdr.Read())
                {
                    topic = BoHelper.CopyTopicToBO(rdr);
                }
            }
            return topic;
        }

    }
}
