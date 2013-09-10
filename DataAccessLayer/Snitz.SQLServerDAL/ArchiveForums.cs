using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;

namespace Snitz.SQLServerDAL
{
    public class ArchiveForums : IArchiveForum
    {
        public void ArchiveTopic(int forumid, int topicid)
        {
            const string topicSql = "INSERT INTO FORUM_A_TOPICS (CAT_ID,FORUM_ID,TOPIC_ID,T_STATUS,T_MAIL,T_SUBJECT,T_MESSAGE,T_AUTHOR,T_REPLIES,T_UREPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_LAST_POSTER,T_IP,T_LAST_POST_AUTHOR,T_LAST_POST_REPLY_ID,T_LAST_EDIT,T_LAST_EDITBY,T_STICKY,T_SIG) " +
                                    "SELECT CAT_ID,FORUM_ID,TOPIC_ID,T_STATUS,T_MAIL,T_SUBJECT,T_MESSAGE,T_AUTHOR,T_REPLIES,T_UREPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_LAST_POSTER,T_IP,T_LAST_POST_AUTHOR,T_LAST_POST_REPLY_ID,T_LAST_EDIT,T_LAST_EDITBY,T_STICKY,T_SIG " + 
                                    "FROM FORUM_TOPICS WHERE TOPIC_ID=@TopicId; ";
            const string replySql = "INSERT INTO FORUM_A_REPLY " + "" +
                                    "(CAT_ID,FORUM_ID,TOPIC_ID,REPLY_ID,R_MAIL,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_LAST_EDIT,R_LAST_EDITBY,R_SIG) " +
                                    "SELECT CAT_ID,FORUM_ID,TOPIC_ID,REPLY_ID,R_MAIL,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_LAST_EDIT,R_LAST_EDITBY,R_SIG " +
                                    "FROM FORUM_REPLY WHERE TOPIC_ID=@TopicId; ";
            const string deleteSql = "DELETE FROM FORUM_REPLY WHERE TOPIC_ID=@TopicId; DELETE FROM FORUM_TOPICS WHERE TOPIC_ID=@TopicId; ";
            const string updateSql = "UPDATE FORUM_FORUM SET " +
                                     "F_TOPICS = (SELECT COUNT(*) FROM FORUM_TOPICS WHERE FORUM_ID = @ArchiveForumID )," +
                                     "F_COUNT = (SELECT COUNT(*) FROM FORUM_REPLY WHERE FORUM_ID = @ArchiveForumID )," +
                                     "F_A_TOPICS = (SELECT COUNT(*) FROM FORUM_A_TOPICS WHERE FORUM_ID = @ArchiveForumID )," +
                                     "F_A_COUNT = (SELECT	COUNT(*) FROM FORUM_A_REPLY WHERE FORUM_ID = @ArchiveForumID ) " +
                                     "WHERE FORUM_ID = @ForumId " +
                                     "UPDATE FORUM_TOTALS SET " +
                                     "P_COUNT = (	SELECT COUNT(*)	FROM FORUM_REPLY)," +
                                     "P_A_COUNT = (SELECT COUNT(*) FROM FORUM_A_REPLY )," +
                                     "T_COUNT = (SELECT COUNT(*) FROM FORUM_TOPICS )," +
                                     "T_A_COUNT = (SELECT COUNT(*) FROM FORUM_A_TOPICS)";

            List<SqlParameter> parms = new List<SqlParameter>
                                       {
                                           new SqlParameter("@ForumId", SqlDbType.Int){Value=forumid},
                                           new SqlParameter("@TopicId", SqlDbType.Int){Value=topicid}
                                       };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, topicSql + replySql + deleteSql + updateSql, parms.ToArray());
        }

        public void ArchiveForum(int forumid, string archivedate)
        {
            List<SqlParameter> parms = new List<SqlParameter>
                                       {
                                           new SqlParameter("@ArchiveForumID", SqlDbType.Int){Value=forumid},
                                           new SqlParameter("@ArchiveDate", SqlDbType.Int){Value=archivedate}
                                       };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.StoredProcedure, "ArchiveForum", parms.ToArray());
        }
    }
}
