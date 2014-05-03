using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.OLEDbDAL
{
    public class ArchiveForums : IArchiveForum
    {
        private const string TopicCols =
            " T.TOPIC_ID,T.CAT_ID,T.FORUM_ID,T.T_STATUS,T.T_SUBJECT,T.T_AUTHOR,T.T_REPLIES,T.T_VIEW_COUNT,T.T_LAST_POST" +
            ",T.T_DATE,T.T_IP,T.T_LAST_POST_AUTHOR,T.T_STICKY,T.T_LAST_EDIT,T.T_LAST_EDITBY,T.T_SIG,T.T_LAST_POST_REPLY_ID" +
            ",T.T_UREPLIES,T.T_MESSAGE,A.M_NAME AS [Author], LPA.M_NAME AS [LastPostAuthor], EM.M_NAME AS [Editor],A.M_VIEW_SIG, A.M_SIG ";
        const string TopicFrom =
            "FROM FORUM_A_TOPICS T " +
            "LEFT OUTER JOIN FORUM_MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID " +
            "LEFT OUTER JOIN FORUM_MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID " +
            "LEFT OUTER JOIN FORUM_MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID ";
        public void ArchiveTopic(int forumid, int topicid)
        {
            const string topicSql = "INSERT INTO FORUM_A_TOPICS (CAT_ID,FORUM_ID,TOPIC_ID,T_STATUS,T_MAIL,T_SUBJECT,T_MESSAGE,T_AUTHOR,T_REPLIES,T_UREPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_LAST_POSTER,T_IP,T_LAST_POST_AUTHOR,T_LAST_POST_REPLY_ID,T_LAST_EDIT,T_LAST_EDITBY,T_STICKY,T_SIG) " +
                                    "SELECT CAT_ID,FORUM_ID,TOPIC_ID,T_STATUS,T_MAIL,T_SUBJECT,T_MESSAGE,T_AUTHOR,T_REPLIES,T_UREPLIES,T_VIEW_COUNT,T_LAST_POST,T_DATE,T_LAST_POSTER,T_IP,T_LAST_POST_AUTHOR,T_LAST_POST_REPLY_ID,T_LAST_EDIT,T_LAST_EDITBY,T_STICKY,T_SIG " +
                                    "FROM FORUM_TOPICS WHERE TOPIC_ID=@TopicId";
            const string replySql = "INSERT INTO FORUM_A_REPLY " + "" +
                                    "(CAT_ID,FORUM_ID,TOPIC_ID,REPLY_ID,R_MAIL,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_LAST_EDIT,R_LAST_EDITBY,R_SIG) " +
                                    "SELECT CAT_ID,FORUM_ID,TOPIC_ID,REPLY_ID,R_MAIL,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_LAST_EDIT,R_LAST_EDITBY,R_SIG " +
                                    "FROM FORUM_REPLY WHERE TOPIC_ID=@TopicId";
            const string deleteSql = "DELETE FROM FORUM_REPLY WHERE TOPIC_ID=@TopicId; DELETE FROM FORUM_TOPICS WHERE TOPIC_ID=@TopicId ";
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

            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@ForumId", OleDbType.Numeric){Value=forumid},
                                           new OleDbParameter("@TopicId", OleDbType.Numeric){Value=topicid}
                                       };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, topicSql + replySql + deleteSql + updateSql, parms.ToArray());
        }

        public void ArchiveForum(int forumid, string archivedate)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@ArchiveForumID", OleDbType.Numeric){Value=forumid},
                                           new OleDbParameter("@ArchiveDate", OleDbType.Numeric){Value=archivedate}
                                       };
            //todo: replace stored proc with inline sql
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.StoredProcedure, "ArchiveForum", parms.ToArray());
        }

        public IEnumerable<ReplyInfo> GetReplies(TopicInfo topic, int startrec, int maxrecs)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TopicInfo> GetTopics(int startRowIndex, int maximumRows, int forumid, bool isAdminOrModerator)
        {
            List<OleDbParameter> param = new List<OleDbParameter>();
            int totalrecs = GetTopicCount(startRowIndex, maximumRows, forumid, isAdminOrModerator);

            StringBuilder over = new StringBuilder();
            over.AppendLine("FROM ((((");
            over.AppendFormat("SELECT Top {0} sub.TOPIC_ID ", maximumRows);
            over.AppendLine("FROM (");
            over.AppendFormat("SELECT TOP {0} TOPIC_ID ", Math.Max((totalrecs - (startRowIndex * maximumRows)), maximumRows));
            over.AppendLine("FROM FORUM_A_TOPICS");
            over.AppendLine("ORDER BY T_LAST_POST DESC,T_SUBJECT");
            over.AppendLine(") sub");
            over.AppendLine(") TE INNER JOIN FORUM_A_TOPICS T on TE.TOPIC_ID = T.TOPIC_ID ) ");
            over.AppendLine("LEFT OUTER JOIN FORUM_MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID )");
            over.AppendLine("LEFT OUTER JOIN FORUM_MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID )");
            over.AppendLine("LEFT OUTER JOIN FORUM_MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID ");
            over.AppendLine("WHERE FORUM_ID=@ForumId");
            param.Add(new OleDbParameter("@ForumId", OleDbType.Integer) { Value = forumid });

            List<TopicInfo> topics = new List<TopicInfo>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + TopicCols + over.ToString() , param.ToArray()))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyTopicToBO(rdr));
                }
            }
            return topics;
        }

        public IEnumerable<int> GetReplyIdList(int topicid)
        {
            throw new System.NotImplementedException();
        }

        public TopicInfo GetNextPrevTopic(int topicid, string which)
        {
            throw new System.NotImplementedException();
        }

        public int GetTopicCount(int startRowIndex, int maximumRows, int forumid, bool isAdminOrModerator)
        {
            const string strSql = "SELECT COUNT(TOPIC_ID) FROM FORUM_A_TOPICS  WHERE FORUM_ID=@ForumId ";

            List<OleDbParameter> param = new List<OleDbParameter>
                                         {
                                             new OleDbParameter("@ForumId", OleDbType.Integer)
                                             {
                                                 Value=forumid
                                             }
                                         };


            var count = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, param.ToArray());
            return Convert.ToInt32(count);
        }

        public TopicInfo GetTopic(int topicid)
        {
            TopicInfo topic = null;

            OleDbParameter parm = new OleDbParameter("@TopicId", SqlDbType.Int) { Value = topicid };

            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + TopicCols + TopicFrom + "WHERE T.TOPIC_ID = @TopicId", parm))
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
