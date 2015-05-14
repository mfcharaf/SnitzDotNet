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
    public class ForumModerator : IForumModerator
    {

        #region IBaseObject<ForumModeratorInfo> Members

        public ForumModeratorInfo GetById(int id)
        {
            ForumModeratorInfo moderator = null;
            string sqlStr =
                "SELECT FM.MOD_ID, FM.FORUM_ID,FM.MEMBER_ID,FM.MOD_TYPE,M.M_NAME FROM " +
                Config.ForumTablePrefix + "MODERATOR FM LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS M ON FM.MEMBER_ID = M.MEMBER_ID " +
                "WHERE FM.MOD_ID=@ModId";
            SqlParameter parm = new SqlParameter("@ModId", SqlDbType.VarChar) { Value = id };
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sqlStr, parm))
            {
                while (rdr.Read())
                {
                    moderator = new ForumModeratorInfo
                    {
                        Id= rdr.GetInt32(0),
                        ForumId = rdr.GetInt32(1),
                        MemberId = rdr.GetInt32(2),
                        Type = rdr.GetInt32(3),
                        Name = rdr.SafeGetString(4)
                    };
                }
            }
            return moderator;
        }

        public IEnumerable<ForumModeratorInfo> GetByName(string name)
        {
            //not required
            return null;
        }

        public int Add(ForumModeratorInfo forumModerator)
        {
            string sqlStr = "INSERT INTO " + Config.ForumTablePrefix + "MODERATOR (FORUM_ID,MEMBER_ID,MOD_TYPE) VALUES (@ForumId,@MemberId,0)";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@ForumId", SqlDbType.Int) {Value = forumModerator.ForumId},
                new SqlParameter("@MemberId", SqlDbType.Int) {Value = forumModerator.MemberId}
            };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, sqlStr, parms.ToArray()));
        }

        public void Update(ForumModeratorInfo forumModerator)
        {
            string sqlStr = "UPDATE " + Config.ForumTablePrefix + "MODERATOR SET FORUM_ID=@ForumId,MEMBER_ID=@MemberId WHERE MOD_ID=@ModId";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@ModId", SqlDbType.Int) {Value = forumModerator.Id},
                new SqlParameter("@ForumId", SqlDbType.Int) {Value = forumModerator.ForumId},
                new SqlParameter("@MemberId", SqlDbType.Int) {Value = forumModerator.MemberId}
            };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sqlStr, parms.ToArray());
        }

        public void Delete(ForumModeratorInfo forumModerator)
        {
            string sqlStr = "";
            if (forumModerator.Id > 0)
            {
                sqlStr = "DELETE FROM " + Config.ForumTablePrefix + "MODERATOR WHERE MOD_ID=@Id";
                    SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sqlStr, new SqlParameter("@Id",SqlDbType.Int){Value = forumModerator.Id});
            }
            else if (forumModerator.MemberId > 0 && forumModerator.ForumId > 0)
            {
                sqlStr = "DELETE FROM " + Config.ForumTablePrefix + "MODERATOR WHERE MEMBER_ID=@Id AND FORUM_ID=@ForumId";
                List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.Int) {Value = forumModerator.MemberId},
                    new SqlParameter("@ForumId", SqlDbType.Int) {Value = forumModerator.ForumId}
                };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sqlStr, parms.ToArray());
                
            }
        }

        public void Dispose()
        {

        }

        #endregion

        /// <summary>
        /// returns a list of All Moderators
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ForumModeratorInfo> GetAll()
        {
            List<ForumModeratorInfo> moderators = new List<ForumModeratorInfo>();
            string sqlStr =
                "SELECT M.MEMBER_ID,M.M_NAME FROM " + Config.MemberTablePrefix + "MEMBERS M WHERE M.M_LEVEL=2 AND M_STATUS=1";

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sqlStr, null))
            {
                while (rdr.Read())
                {
                    moderators.Add(new ForumModeratorInfo { MemberId = rdr.GetInt32(0), Name = rdr.SafeGetString(1) });
                }
            }
            return moderators;
        }

        public IEnumerable<ForumModeratorInfo> GetByParent(int forumid)
        {
            List<ForumModeratorInfo> moderators = new List<ForumModeratorInfo>();
            string sqlStr =
                "SELECT FM.MOD_ID, M.MEMBER_ID,M.M_NAME FROM " + Config.ForumTablePrefix + "MODERATOR FM LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS AS M ON FM.MEMBER_ID = M.MEMBER_ID WHERE M.M_LEVEL=2 AND FM.FORUM_ID=@ForumId";

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sqlStr, new SqlParameter("@ForumId",SqlDbType.Int){Value = forumid}))
            {
                while (rdr.Read())
                {
                    moderators.Add(new ForumModeratorInfo { MemberId = rdr.GetInt32(1), Name = rdr.SafeGetString(2),Id=rdr.GetInt32(0),ForumId = forumid});
                }
            }
            return moderators;
        }

        public List<ForumInfo> GetUnModeratedForums(int memberId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT F.FORUM_ID,F.CAT_ID,F.F_STATUS,F.F_SUBJECT,F.F_URL,F.F_TOPICS");
            sql.AppendLine(",F.F_COUNT,F.F_LAST_POST,F.F_PRIVATEFORUMS,F.F_TYPE,F.F_LAST_POST_AUTHOR,F.F_A_TOPICS,F.F_A_COUNT,F.F_MODERATION");
            sql.AppendLine(",F.F_SUBSCRIPTION,F.F_ORDER, F.F_COUNT_M_POSTS,F.F_LAST_POST_TOPIC_ID,F.F_LAST_POST_REPLY_ID,F.F_POLLS,F.F_DESCRIPTION");
            sql.AppendLine(",F.F_L_ARCHIVE,F.F_ARCHIVE_SCHED,T.T_SUBJECT,M.M_NAME ");
            sql.AppendLine("FROM ");
            sql.AppendFormat("{0}FORUM  F LEFT OUTER JOIN ",Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS M ON F.F_LAST_POST_AUTHOR = M.MEMBER_ID LEFT OUTER JOIN ",Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}TOPICS T ON F.F_LAST_POST_TOPIC_ID = T.TOPIC_ID ",Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("WHERE F.FORUM_ID NOT IN (SELECT FORUM_ID FROM {0}MODERATOR WHERE MEMBER_ID=@MemberId)",Config.ForumTablePrefix).AppendLine();

            List<ForumInfo> forums = new List<ForumInfo>();

            SqlParameter parm = new SqlParameter("@MemberId", SqlDbType.Int) { Value = memberId };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parm))
            {
                while (rdr.Read())
                {
                    forums.Add(BoHelper.CopyForumToBO(rdr));
                }
            }
            return forums;
        }

        public List<ForumInfo> GetModeratedForums(int memberId)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT F.FORUM_ID,F.CAT_ID,F.F_STATUS,F.F_SUBJECT,F.F_URL,F.F_TOPICS");
            sql.AppendLine(",F.F_COUNT,F.F_LAST_POST,F.F_PRIVATEFORUMS,F.F_TYPE,F.F_LAST_POST_AUTHOR,F.F_A_TOPICS,F.F_A_COUNT,F.F_MODERATION");
            sql.AppendLine(",F.F_SUBSCRIPTION,F.F_ORDER, F.F_COUNT_M_POSTS,F.F_LAST_POST_TOPIC_ID,F.F_LAST_POST_REPLY_ID,F.F_POLLS,F.F_DESCRIPTION");
            sql.AppendLine(",F.F_L_ARCHIVE,F.F_ARCHIVE_SCHED,T.T_SUBJECT,M.M_NAME ");
            sql.AppendLine("FROM ");
            sql.AppendFormat("{0}FORUM  F LEFT OUTER JOIN ", Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS M ON F.F_LAST_POST_AUTHOR = M.MEMBER_ID LEFT OUTER JOIN ", Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}TOPICS T ON F.F_LAST_POST_TOPIC_ID = T.TOPIC_ID ", Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("WHERE F.FORUM_ID IN (SELECT FORUM_ID FROM {0}MODERATOR WHERE MEMBER_ID=@MemberId)", Config.ForumTablePrefix).AppendLine();

            List<ForumInfo> forums = new List<ForumInfo>();

            SqlParameter parm = new SqlParameter("@MemberId", SqlDbType.Int) { Value = memberId };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parm))
            {
                while (rdr.Read())
                {
                    forums.Add(BoHelper.CopyForumToBO(rdr));
                }
            }
            return forums;
        }

        public bool IsUserForumModerator(int memberid, int forumid)
        {
            const string strSql = "SELECT MOD_ID FROM FORUM_MODERATOR WHERE FORUM_ID=@ForumId AND MEMBER_ID=@MemberId";
            List<SqlParameter> parms = new List<SqlParameter>
                                       {
                                           new SqlParameter("@MemberId", SqlDbType.Int)
                                           {
                                               Value
                                                   =
                                                   memberid
                                           },
                                           new SqlParameter("@ForumId", SqlDbType.Int)
                                           {
                                               Value
                                                   =
                                                   forumid
                                           }
                                       };

            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            return res != null;
        }

        public List<ForumModeratorInfo> GetAvailableModerators(int forumId)
        {
            var forummoderators = GetByParent(forumId);
            //peopleList2.Where(p => !peopleList1.Any(p2 => p2.ID == p.ID));
            var moderators = GetAll().Where(m=> !forummoderators.Any(fm=>fm.MemberId == m.MemberId));
            return new List<ForumModeratorInfo>(moderators);
        }

        public void SetForumModerators(int forumId, int[] userList)
        {
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "DELETE FROM FORUM_MODERATOR WHERE FORUM_ID=@ForumId", new SqlParameter("@ForumId", SqlDbType.Int) { Value = forumId});
            foreach (int user in userList)
            {
                ForumModeratorInfo mod = new ForumModeratorInfo {ForumId = forumId, MemberId = user};
                Add(mod);
            }
        }

        public void SetUserAsModeratorForForums(int memberId, int[] forumList)
        {
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "DELETE FROM FORUM_MODERATOR WHERE MEMBER_ID=@MemberId", new SqlParameter("@MemberId", SqlDbType.Int) { Value = memberId });
            foreach (int forum in forumList)
            {
                ForumModeratorInfo mod = new ForumModeratorInfo { ForumId = forum, MemberId = memberId };
                Add(mod);
            }
        }
    }
}
