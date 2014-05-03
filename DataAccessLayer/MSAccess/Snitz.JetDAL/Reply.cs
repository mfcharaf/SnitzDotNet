using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.OLEDbDAL
{
    public class Reply : IReply
    {

        private const string REPLY_COLS =
            "R.REPLY_ID,R.CAT_ID,R.FORUM_ID,R.TOPIC_ID,R.R_AUTHOR,R.R_DATE,R.R_IP,R.R_STATUS,R.R_LAST_EDIT," +
            "R.R_LAST_EDITBY,R.R_SIG,R.R_MESSAGE,AM.M_NAME AS Author, EM.M_NAME AS Editor,AM.M_VIEW_SIG, AM.M_SIG ";

        #region IReply Members

        public void MoveReplies(TopicInfo newtopic, List<int> replyids)
        {
            const string strSql = "UPDATE FORUM_REPLY SET CAT_ID=@CatId, FORUM_ID=@ForumId, TOPIC_ID=@TopicId WHERE REPLY_ID IN ({0}) ";
            int[] values = replyids.ToArray();

            var inparms = values.Select((s, i) => "@p" + i.ToString()).ToArray();
            var inclause = string.Join(",", inparms);
            List<OleDbParameter> parms = values.Select((t, i) => new OleDbParameter(inparms[i], OleDbType.Numeric) { Value = t }).ToList();
            parms.Add(new OleDbParameter("@CatId", OleDbType.Numeric) { Value = newtopic.CatId });
            parms.Add(new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = newtopic.ForumId });
            parms.Add(new OleDbParameter("@TopicId", OleDbType.Numeric) { Value = newtopic.Id });
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, String.Format(strSql, inclause), parms.ToArray());

        }

        public void SetReplyStatus(int replyid, int status)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            const string strSql = "UPDATE FORUM_REPLY SET R_STATUS=@Status WHERE REPLY_ID=@ReplyId";
            parms.Add(new OleDbParameter("@Status", OleDbType.Numeric) { Value = status });
            parms.Add(new OleDbParameter("@ReplyId", OleDbType.Numeric) { Value = replyid });

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        #endregion

        #region IBaseObject<ReplyInfo> Members

        public ReplyInfo GetById(int id)
        {
            const string strSql = "SELECT " + REPLY_COLS +
                                  "FROM (FORUM_REPLY R " +
                                  "LEFT OUTER JOIN FORUM_MEMBERS EM ON R.R_LAST_EDITBY = EM.MEMBER_ID) " +
                                  "LEFT OUTER JOIN FORUM_MEMBERS AS AM ON R.R_AUTHOR = AM.MEMBER_ID " +
                                  "WHERE REPLY_ID=@ReplyId";
            ReplyInfo reply = null;
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@ReplyId", OleDbType.Numeric) { Value = id }))
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
            const string insert = "INSERT INTO FORUM_REPLY (CAT_ID,FORUM_ID,TOPIC_ID,R_AUTHOR,R_MESSAGE,R_DATE,R_IP,R_STATUS,R_SIG)";
            string values = String.Format("VALUES({0},{1},{2},{3},@Message,'{4}','{5}',{6},{7})"
                , reply.CatId, reply.ForumId, reply.TopicId, reply.AuthorId, reply.Date.ToString("yyyyMMddHHmmss"), reply.PosterIp, reply.Status, reply.UseSignatures ? 1 : 0);

            OleDbParameter msg = new OleDbParameter("@Message", OleDbType.VarChar) { Value = reply.Message };
            int res = Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, insert + values, msg));
            reply.Id = res;
            new Topic().UpdateLastTopicPost(reply);
            new Forum().UpdateLastForumPost(reply);
            new Member().UpdateLastMemberPost(reply);
            return reply.Id;
        }

        public void Update(ReplyInfo reply)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            StringBuilder replySql = new StringBuilder("UPDATE FORUM_REPLY SET ");
            replySql.AppendLine("R_MESSAGE=@Message,");
            if (reply.LastEditedById.HasValue)
            {
                replySql.AppendLine("R_LAST_EDIT=@EditDate,");
                replySql.AppendLine("R_LAST_EDITBY=@EditedBy,");
                parms.Add(new OleDbParameter("@EditDate", OleDbType.VarChar) { Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss") });
                parms.Add(new OleDbParameter("@EditedBy", OleDbType.Numeric) { Value = reply.LastEditedById });
            }

            replySql.AppendLine("R_SIG=@UseSig ");
            replySql.AppendLine("WHERE REPLY_ID=@ReplyId");
            parms.Add(new OleDbParameter("@Message", OleDbType.VarChar) { Value = reply.Message });
            parms.Add(new OleDbParameter("@UseSig", OleDbType.Numeric) { Value = reply.UseSignatures });
            parms.Add(new OleDbParameter("@ReplyId", OleDbType.Numeric) { Value = reply.Id });

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, replySql.ToString(), parms.ToArray());

        }

        public void Delete(ReplyInfo reply)
        {
            const string deleteSql = "DELETE FROM FORUM_REPLY WHERE REPLY_ID=@ReplyId";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, deleteSql, new OleDbParameter("@ReplyId", OleDbType.Numeric) { Value = reply.Id });
        }

        public void Dispose()
        {

        }

        #endregion

        public IEnumerable<ReplyInfo> GetByParent(TopicInfo topic, int start, int maxrecs)
        {
            string replytable = "FORUM_REPLY";
            if (topic.IsArchived)
                replytable = "FORUM_A_REPLY";
            List<ReplyInfo> topics = new List<ReplyInfo>();
            List<OleDbParameter> parms = new List<OleDbParameter>();

            StringBuilder over = new StringBuilder();
            over.AppendLine("FROM (((");
            over.AppendFormat("SELECT TOP {0} sub.REPLY_ID, sub.R_DATE ", maxrecs);
            over.AppendLine("FROM  (");
            over.AppendFormat(" SELECT TOP {0} REPLY_ID,R_DATE FROM " + replytable + " WHERE TOPIC_ID=@TopicId ORDER BY R_DATE desc", (Math.Max(topic.ReplyCount, maxrecs) - (start * maxrecs)));
            over.AppendLine(") sub ");
            over.AppendLine(" ORDER BY sub.R_DATE asc");
            over.AppendLine(") RE ");
            over.AppendLine(" INNER JOIN " + replytable + " R on RE.REPLY_ID = R.REPLY_ID) ");
            over.AppendLine("LEFT OUTER JOIN FORUM_MEMBERS EM ON R.R_LAST_EDITBY = EM.MEMBER_ID) ");
            over.AppendLine("LEFT OUTER JOIN FORUM_MEMBERS AS AM ON R.R_AUTHOR = AM.MEMBER_ID ");

            parms.Add(new OleDbParameter("@TopicId", OleDbType.Numeric) { Value = topic.Id });
            //(TotRows - ((Page_Number - 1) * PageSize)

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, " SELECT " + REPLY_COLS + over, parms.ToArray()))
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
