using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.OLEDbDAL
{
    public class ForumStats : IForumStats
    {
        private const string TOPIC_SELECT = "SELECT TOP 1 TOPIC_ID,CAT_ID,FORUM_ID,T_STATUS,T_SUBJECT,T_AUTHOR,T_REPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_IP,T_LAST_POST_AUTHOR" + ",T_STICKY,T_LAST_EDIT,T_LAST_EDITBY,T_SIG,T_LAST_POST_REPLY_ID,T_UREPLIES,T_MESSAGE FROM FORUM_TOPICS ORDER BY T_LAST_POST DESC";

        private const string ACTIVE_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM FORUM_MEMBERS WHERE M_STATUS=1 ";
        private const string ACTIVE_TOPICS = "SELECT COUNT(TOPIC_ID) FROM FORUM_TOPICS WHERE T_STATUS=1 AND T_LAST_POST > @LastVisit ";
        private const string ARCHIVED_REPLY = "SELECT COUNT(REPLY_ID) FROM FORUM_A_REPLY ";
        private const string ARCHIVED_TOPICS = "SELECT COUNT(TOPIC_ID) FROM FORUM_A_TOPICS ";
        private const string TOTAL_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM FORUM_MEMBERS ";
        private const string TOTAL_TOPICS = "SELECT COUNT(TOPIC_ID) FROM FORUM_TOPICS; ";
        private const string TOTAL_POSTS = @"SELECT Sum([Topics]) AS [Total Topics], Sum([Replies]) AS [Total Replies]
                                                FROM
                                                (SELECT Count(*) AS Topics, 0 AS Replies FROM FORUM_TOPICS
                                                UNION
                                                SELECT 0, Count(*) FROM FORUM_REPLY) AS Subq";

        public StatsInfo GetStatistics(string lastvisit)
        {
            StatsInfo stats = new StatsInfo();
            OleDbParameter parm = new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = lastvisit };
            OleDbDataReader rdr = null;

            //using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, 
            //    ACTIVE_MEMBERS + ACTIVE_TOPICS + ARCHIVED_REPLY + ARCHIVED_TOPICS + TOTAL_MEMBERS + TOTAL_TOPICS + TOTAL_POSTS, parm))
            //{
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, ACTIVE_MEMBERS);
                while (rdr.Read())
                {
                    stats.ActiveMembers = rdr.GetInt32(0);
                }
            rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, ACTIVE_TOPICS,parm);
                while (rdr.Read())
                {
                    stats.ActiveTopicCount = rdr.GetInt32(0);
                }
                rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, ARCHIVED_REPLY);
                while (rdr.Read())
                {
                    stats.ArchiveReplyCount = rdr.GetInt32(0);
                }
                rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, ARCHIVED_TOPICS);
                while (rdr.Read())
                {
                    stats.ArchiveTopicCount = rdr.GetInt32(0);
                }
                rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, TOTAL_MEMBERS);
                while (rdr.Read())
                {
                    stats.MemberCount = rdr.GetInt32(0);
                }
                rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, TOTAL_TOPICS);
                while (rdr.Read())
                {
                    stats.TopicCount = rdr.GetInt32(0);
                }
                rdr.Close();
                rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, TOTAL_POSTS);
                while (rdr.Read())
                {
                    stats.TotalPostCount = (int) (rdr.GetDouble(0) + rdr.GetDouble(1));
                }
                rdr.Close();
            //}

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
            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, "SELECT TOP 1 M_NAME FROM FORUM_MEMBERS ORDER BY M_DATE DESC", null);
            if (res != System.DBNull.Value)
                return (string) res;
            else
                return null;
        }

        private TopicInfo GetLastPost()
        {
            TopicInfo topic = null;

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, TOPIC_SELECT, null))
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
