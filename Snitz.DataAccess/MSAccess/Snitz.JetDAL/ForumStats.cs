using System.Data;
using System.Data.OleDb;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class ForumStats : IForumStats
    {
        private string TOPIC_SELECT = "SELECT TOP 1 TOPIC_ID,CAT_ID,FORUM_ID,T_STATUS,T_SUBJECT,T_AUTHOR,T_REPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_IP,T_LAST_POST_AUTHOR,T_STICKY,T_LAST_EDIT,T_LAST_EDITBY,T_SIG,T_LAST_POST_REPLY_ID,T_UREPLIES,T_MESSAGE FROM " + Config.ForumTablePrefix + "TOPICS ORDER BY T_LAST_POST DESC";

        private string ACTIVE_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_STATUS=1 ";
        private string ACTIVE_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS WHERE T_STATUS=1 AND T_LAST_POST > @LastVisit ";
        private string ARCHIVED_REPLY = "SELECT COUNT(REPLY_ID) FROM " + Config.ForumTablePrefix + "A_REPLY ";
        private string ARCHIVED_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "A_TOPICS ";
        private string TOTAL_MEMBERS = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS ";
        private string TOTAL_TOPICS = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS; ";
        private string TOTAL_POSTS = "SELECT Sum([Topics]) AS [Total Topics], Sum([Replies]) AS [Total Replies]" +
                                     "FROM " +
                                     "(SELECT Count(*) AS Topics, 0 AS Replies FROM " + Config.ForumTablePrefix + "TOPICS " +
                                     "UNION SELECT 0, Count(*) FROM " + Config.ForumTablePrefix + "REPLY) AS Subq";

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

        public int LogDownload(string file)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("IF EXISTS (SELECT ID FROM " + Config.ForumTablePrefix + "FILELOG WHERE FILENAME=@FileName)");
            sql.AppendLine("UPDATE " + Config.ForumTablePrefix + "FILELOG SET COUNTER=COUNTER+1 WHERE FILENAME=@FileName");
            sql.AppendLine("ELSE");
            sql.AppendLine("INSERT INTO " + Config.ForumTablePrefix + "FILELOG (FILENAME,COUNTER) VALUES (@FileName,1)");
            OleDbParameter parm = new OleDbParameter("@FileName", SqlDbType.VarChar) { Value = file };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parm);

            sql.Length = 0;
            sql.AppendLine("SELECT COUNTER FROM " + Config.ForumTablePrefix + "FILELOG WHERE FILENAME=@FileName");

            return (int) SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parm);

        }

        private AuthorInfo GetLastPostAuthor(int authorId)
        {
            var member = new Member().GetById(authorId);
            AuthorInfo author = new AuthorInfo(member);
            return author;
        }

        private string GetNewestMember()
        {
            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, "SELECT TOP 1 M_NAME FROM " + Config.MemberTablePrefix + "MEMBERS  WHERE M_VALID=1 AND M_STATUS=1 ORDER BY M_DATE DESC", null);
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
