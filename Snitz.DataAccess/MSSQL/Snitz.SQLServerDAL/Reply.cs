using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class Reply : IReply
    {

        private const string REPLY_COLS =
            "R.REPLY_ID,R.CAT_ID,R.FORUM_ID,R.TOPIC_ID,R.R_AUTHOR,R.R_DATE,R.R_IP,R.R_STATUS,R.R_LAST_EDIT," + 
            "R.R_LAST_EDITBY,R.R_SIG,R.R_MESSAGE,AM.M_NAME AS Author, EM.M_NAME AS Editor,AM.M_VIEW_SIG, AM.M_SIG ";

        #region IReply Members

        public void MoveReplies(TopicInfo newtopic, List<int> replyids)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "REPLY SET CAT_ID=@CatId, FORUM_ID=@ForumId, TOPIC_ID=@TopicId WHERE REPLY_ID IN ({0}) ";
            int[] values = replyids.ToArray();

            var inparms = values.Select((s, i) => "@p" + i.ToString()).ToArray();
            var inclause = string.Join(",", inparms);
            List<SqlParameter> parms = values.Select((t, i) => new SqlParameter(inparms[i], SqlDbType.Int) {Value = t}).ToList();
            parms.Add(new SqlParameter("@CatId", SqlDbType.Int) { Value = newtopic.CatId });
            parms.Add(new SqlParameter("@ForumId", SqlDbType.Int) { Value = newtopic.ForumId });
            parms.Add(new SqlParameter("@TopicId", SqlDbType.Int) { Value = newtopic.Id });
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, String.Format(strSql,inclause), parms.ToArray());

        }

        public void SetReplyStatus(int replyid, int status)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            string strSql = "UPDATE " + Config.ForumTablePrefix + "REPLY SET R_STATUS=@Status WHERE REPLY_ID=@ReplyId";
            parms.Add(new SqlParameter("@Status", SqlDbType.Int) { Value = status });
            parms.Add(new SqlParameter("@ReplyId", SqlDbType.Int) { Value = replyid });

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        #endregion

        #region IBaseObject<ReplyInfo> Members

        public ReplyInfo GetById(int id)
        {
            string strSql = "SELECT " + REPLY_COLS +
                                  "FROM " + Config.ForumTablePrefix + "REPLY R " +
                                  "LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS EM ON R.R_LAST_EDITBY = EM.MEMBER_ID " +
                                  "LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS AS AM ON R.R_AUTHOR = AM.MEMBER_ID " +
                                  "WHERE REPLY_ID=@ReplyId";
            ReplyInfo reply = null;
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@ReplyId", SqlDbType.Int){Value = id}))
            {
                while (rdr.Read())
                {
                    reply = BoHelper.CopyReplyToBO(rdr);
                }
            }
            return reply;
        }

        public IEnumerable<ReplyInfo> GetByName(string name)
        {
            //Not used by Reply
            return null;
        }

        public int Add(ReplyInfo reply)
        {
            string insert = "INSERT INTO " + Config.ForumTablePrefix + "REPLY (CAT_ID,FORUM_ID,TOPIC_ID,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_SIG)";
            string values = String.Format("VALUES({0},{1},{2},{3},@Message,'{4}','{5}',{6},{7}); SELECT SCOPE_IDENTITY();"
                , reply.CatId, reply.ForumId, reply.TopicId, reply.AuthorId, reply.Date.ToString("yyyyMMddHHmmss"), reply.PosterIp,reply.Status, reply.UseSignatures ? 1 : 0);

            SqlParameter msg = new SqlParameter("@Message", SqlDbType.NVarChar) {Value = reply.Message};
            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, insert + values, msg));
            reply.Id = res;
            new Topic().UpdateLastTopicPost(reply);
            new Forum().UpdateLastForumPost(reply);
            new Member().UpdateLastMemberPost(reply);
            return reply.Id;
        }

        public void Update(ReplyInfo reply)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            StringBuilder replySql = new StringBuilder();
            replySql.AppendFormat("UPDATE {0}REPLY SET",Config.ForumTablePrefix).AppendLine();
            replySql.AppendLine("R_MESSAGE=@Message,");
            if (reply.LastEditedById.HasValue)
            {
                replySql.AppendLine("R_LAST_EDIT=@EditDate,");
                replySql.AppendLine("R_LAST_EDITBY=@EditedBy,");   
                parms.Add(new SqlParameter("@EditDate",SqlDbType.VarChar){Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")});
                parms.Add(new SqlParameter("@EditedBy",SqlDbType.Int){Value = reply.LastEditedById});
            }

            replySql.AppendLine("R_SIG=@UseSig ");
            replySql.AppendLine("WHERE REPLY_ID=@ReplyId");
            parms.Add(new SqlParameter("@Message", SqlDbType.NVarChar) { Value = reply.Message });
            parms.Add(new SqlParameter("@UseSig", SqlDbType.Int) { Value = reply.UseSignatures });
            parms.Add(new SqlParameter("@ReplyId", SqlDbType.Int) { Value = reply.Id });

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, replySql.ToString(), parms.ToArray());

        }

        public void Delete(ReplyInfo reply)
        {
            string deleteSql = "DELETE FROM " + Config.ForumTablePrefix + "REPLY WHERE REPLY_ID=@ReplyId;";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, deleteSql, new SqlParameter("@ReplyId", SqlDbType.Int) { Value = reply.Id });
        }

        public void Dispose()
        {

        }

        #endregion

        public IEnumerable<ReplyInfo> GetByParent(TopicInfo topic, int start, int maxrecs)
        {
            start = start*maxrecs;

            string replytable = Config.ForumTablePrefix + "REPLY";
            if (topic.IsArchived)
                replytable = Config.ForumTablePrefix + "A_REPLY";

            List<ReplyInfo> topics = new List<ReplyInfo>();
            List<SqlParameter> parms = new List<SqlParameter>();
            string selectOver = "WITH ReplyEntities AS (SELECT ROW_NUMBER() OVER (ORDER BY R_DATE ASC) AS Row, REPLY_ID FROM " + replytable + " WHERE TOPIC_ID=@TopicId) ";
            string selectOverFrom =
                "FROM ReplyEntities RE INNER JOIN " + replytable + " R on RE.REPLY_ID = R.REPLY_ID " +
                "LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS EM ON R.R_LAST_EDITBY = EM.MEMBER_ID " +
                "LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS AS AM ON R.R_AUTHOR = AM.MEMBER_ID " +
                "WHERE RE.Row Between @Start AND @MaxRows ORDER BY RE.Row ASC";

            parms.Add(new SqlParameter("@TopicId", SqlDbType.Int) { Value = topic.Id });
            parms.Add(new SqlParameter("@Start", SqlDbType.Int) { Value = start + 1 });
            parms.Add(new SqlParameter("@MaxRows", SqlDbType.VarChar) { Value = start + maxrecs });

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, selectOver + " SELECT " + REPLY_COLS + selectOverFrom, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyReplyToBO(rdr));
                }
            }
            return topics;
        }

    }
}
