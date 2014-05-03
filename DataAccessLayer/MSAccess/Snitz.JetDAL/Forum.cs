using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzCommon;

namespace Snitz.OLEDbDAL
{
    public class Forum : IForum
    {
        private const string FORUM_COLS = "F.FORUM_ID,F.CAT_ID,F.F_STATUS,F.F_SUBJECT,F.F_URL,F.F_TOPICS" +
            ",F.F_COUNT,F.F_LAST_POST,F.F_PRIVATEFORUMS,F.F_TYPE,F.F_LAST_POST_AUTHOR,F.F_A_TOPICS,F.F_A_COUNT,F.F_MODERATION" +
            ",F.F_SUBSCRIPTION,F.F_ORDER, F.F_COUNT_M_POSTS,F.F_LAST_POST_TOPIC_ID,F.F_LAST_POST_REPLY_ID,F.F_POLLS,F.F_DESCRIPTION" +
            ",F.F_L_ARCHIVE,F.F_ARCHIVE_SCHED,T.T_SUBJECT,M.M_NAME ";

        private const string FROM_CLAUSE = " FROM (FORUM_FORUM F LEFT OUTER JOIN " +
            "FORUM_MEMBERS M ON F.F_LAST_POST_AUTHOR = M.MEMBER_ID) LEFT OUTER JOIN " +
            "FORUM_TOPICS T ON F.F_LAST_POST_TOPIC_ID = T.TOPIC_ID ";

        #region IForum Members

        public IEnumerable<ForumJumpto> ListForumJumpTo()
        {
            List<ForumJumpto> forumlist = new List<ForumJumpto>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT F.FORUM_ID,F_SUBJECT,C.CAT_NAME FROM FORUM_FORUM F LEFT OUTER JOIN FORUM_CATEGORY C ON F.CAT_ID = C.CAT_ID ORDER BY C.CAT_ORDER,F.F_ORDER", null))
            {
                while (rdr.Read())
                {
                    ForumJumpto jumpto = new ForumJumpto { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Category = rdr.GetString(2) };
                    forumlist.Add(jumpto);
                }
                return forumlist;
            }
        }

        public IEnumerable<ForumInfo> GetByParent(int catid)
        {
            List<ForumInfo> forums = new List<ForumInfo>();

            OleDbParameter parm = new OleDbParameter("@CatId", OleDbType.Numeric) { Value = catid };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT " + FORUM_COLS + FROM_CLAUSE + "WHERE F.CAT_ID = @CatId ORDER BY F.F_ORDER", parm))
            {
                while (rdr.Read())
                {
                    forums.Add(BoHelper.CopyForumToBO(rdr));
                }
            }
            return forums;
        }

        public IEnumerable<int> AllowedRoles(int forumid)
        {
            List<int> roles = new List<int>();
            //return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
            const string sqlStr = "SELECT ROLE_ID FROM FORUM_ROLES WHERE FORUM_ID = @ForumId";

            OleDbParameter parm = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sqlStr, parm))
            {
                while (rdr.Read())
                {
                    roles.Add(rdr.GetInt32(0));
                }
            }
            return roles;
        }

        public IEnumerable<string> Roles(int forumid)
        {
            List<string> roles = new List<string>();
            //return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
            const string sqlStr = "SELECT aspnet_Roles.RoleName " +
                                  "FROM FORUM_ROLES LEFT OUTER JOIN aspnet_Roles ON FORUM_ROLES.Role_Id = aspnet_Roles.RoleId WHERE FORUM_ID = @ForumId";

            OleDbParameter parm = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sqlStr, parm))
            {
                while (rdr.Read())
                {
                    roles.Add(rdr.GetString(0));
                }
            }
            return roles;
        }

        //public IEnumerable<TopicInfo> GetTopics(int forumid)
        //{
        //    List<TopicInfo> topics = new List<TopicInfo>();
        //    OleDbParameter parm = new OleDbParameter("@Forum", OleDbType.Numeric) { Value = forumid };
        //    using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + Topic.TopicCols + Topic.TopicFrom + " WHERE T.FORUM_ID = @Forum AND T.T_STICKY=0", parm))
        //    {
        //        while (rdr.Read())
        //        {
        //            topics.Add(BoHelper.CopyTopicToBO(rdr));
        //        }

        //    }
        //    return topics;
        //}

        public IEnumerable<TopicInfo> GetStickyTopics(int forumid)
        {
            List<TopicInfo> stickytopics = new List<TopicInfo>();
            OleDbParameter parm = new OleDbParameter("@Forum", OleDbType.Numeric) { Value = forumid };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + Topic.TopicCols + Topic.TopicFrom + " WHERE T.FORUM_ID = @Forum AND T.T_STICKY=1", parm))
            {
                while (rdr.Read())
                {
                    stickytopics.Add(BoHelper.CopyTopicToBO(rdr));
                }

            }
            return stickytopics;
        }

        public void UpdateLastForumPost(object post)
        {
            string updateForumSql = "UPDATE FORUM_FORUM SET F_COUNT=F_COUNT+1 [INCTOPIC], F_LAST_POST_TOPIC_ID=@TopicId, ";
            if (post is ReplyInfo)
            {
                updateForumSql += "F_LAST_POST_REPLY_ID=@ReplyId,";
            }
            updateForumSql += "F_LAST_POST=@PostDate, F_LAST_POST_AUTHOR=@PostAuthor WHERE FORUM_ID=@ForumId ";

            List<OleDbParameter> parms = new List<OleDbParameter>();
            OleDbParameter topicid = new OleDbParameter("@TopicId", OleDbType.Numeric);
            OleDbParameter forumid = new OleDbParameter("@ForumId", OleDbType.Numeric);
            
            OleDbParameter replyid = new OleDbParameter("@ReplyId", OleDbType.Numeric);
            OleDbParameter postdate = new OleDbParameter("@PostDate", OleDbType.VarChar);
            OleDbParameter postauthor = new OleDbParameter("@PostAuthor", OleDbType.Numeric);
            if (post is ReplyInfo)
            {
                ReplyInfo reply = (ReplyInfo)post;
                forumid.Value = reply.ForumId;
                topicid.Value = reply.TopicId;
                replyid.Value = reply.Id;
                postdate.Value = reply.Date.ToString("yyyyMMddHHmmss");
                postauthor.Value = reply.AuthorId;
                updateForumSql = updateForumSql.Replace("[INCTOPIC]", "");
            }
            else if (post is TopicInfo)
            {
                TopicInfo topic = (TopicInfo)post;
                forumid.Value = topic.ForumId;
                topicid.Value = topic.Id;
                replyid.Value = null;
                postdate.Value = topic.Date.ToString("yyyyMMddHHmmss");
                postauthor.Value = topic.AuthorId;
                updateForumSql = updateForumSql.Replace("[INCTOPIC]", ",F_TOPICS=F_TOPICS+1");
            }
            parms.Add(topicid);
            if (post is ReplyInfo)
                parms.Add(replyid);
            parms.Add(postdate);
            parms.Add(postauthor);
            parms.Add(forumid);

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateForumSql, parms.ToArray());
        }

        public void SetForumStatus(int forumid, int status)
        {
            const string strSql = "UPDATE FORUM_FORUM SET F_STATUS=@Status WHERE FORUM_ID=@ForumId";
            OleDbParameter forum = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid };
            OleDbParameter fstatus = new OleDbParameter("@Status", OleDbType.Numeric) { Value = status };
            OleDbParameter[] parms = new OleDbParameter[2];
            parms[0] = fstatus;
            parms[1] = forum;

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms);
        }

        public void EmptyForum(int forumid)
        {
            const string strSql =
                "DELETE FROM FORUM_A_REPLY WHERE FORUM_ID = @ForumId " +
                "DELETE FROM FORUM_A_TOPICS WHERE FORUM_ID = @ForumId " +
                "DELETE FROM FORUM_REPLY WHERE FORUM_ID = @ForumId " +
                "DELETE FROM FORUM_TOPICS WHERE FORUM_ID=@ForumId ";
            OleDbParameter forum = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, forum);
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "UPDATE FORUM_FORUM SET F_LAST_POST_AUTHOR=NULL,F_LAST_POST=NULL,F_LAST_POST_TOPIC_ID=NULL,F_LAST_POST_REPLY_ID=NULL WHERE FORUM_ID=@ForumId", forum);
        }

        public string[] GetForumRoles(int forumid)
        {
            const string strSql = "SELECT AR.LoweredRoleName FROM FORUM_ROLES FR LEFT OUTER JOIN aspnet_Roles AR ON FR.Role_Id = AR.RoleId WHERE FORUM_ID=@ForumId";
            List<string> forums = new List<string>();

            OleDbParameter parm = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    forums.Add(rdr.GetString(0));
                }
            }
            return forums.ToArray();
        }

        #endregion

        #region IBaseObject<ForumInfo> Members

        public ForumInfo GetById(int id)
        {
            ForumInfo forum = null;

            OleDbParameter parm = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = id };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT " + FORUM_COLS + FROM_CLAUSE + " WHERE F.FORUM_ID = @ForumId ", parm))
            {
                while (rdr.Read())
                {
                    forum = BoHelper.CopyForumToBO(rdr);
                }
            }
            if (forum != null)
            {
                forum.AllowedRoles = new List<int>(AllowedRoles(forum.Id));
                forum.Roles = new List<string>(Roles(forum.Id));
            }
            return forum;
        }

        public IEnumerable<ForumInfo> GetByName(string name)
        {
            List<ForumInfo> forums = new List<ForumInfo>();

            OleDbParameter parm = new OleDbParameter("@Subject", OleDbType.VarChar) { Value = "%" + name + "%" };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT " + FORUM_COLS + FROM_CLAUSE + " WHERE F.F_SUBJECT = @Subject", parm))
            {
                while (rdr.Read())
                {
                    forums.Add(BoHelper.CopyForumToBO(rdr));
                }
            }
            return forums;
        }

        public int Add(ForumInfo forum)
        {
            if (!String.IsNullOrEmpty(forum.Url))
                forum.Type = 1;
            List<OleDbParameter> parms = new List<OleDbParameter>();

            const string strsql = "INSERT INTO FORUM_FORUM " +
                                  "(CAT_ID,F_STATUS,F_MAIL,F_SUBJECT,F_URL,F_DESCRIPTION,F_TOPICS,F_COUNT,F_LAST_POST,F_PASSWORD_NEW,F_PRIVATEFORUMS " +
                                  ",F_TYPE,F_IP,F_LAST_POST_AUTHOR,F_A_TOPICS,F_A_COUNT,F_MODERATION,F_SUBSCRIPTION,F_ORDER,F_L_ARCHIVE,F_ARCHIVE_SCHED " +
                                  ",F_L_DELETE,F_DELETE_SCHED,F_DEFAULTDAYS,F_COUNT_M_POSTS,F_LAST_POST_TOPIC_ID,F_LAST_POST_REPLY_ID,F_POLLS) " +
                                  "VALUES ";
            const string strvalues = "(@CatId,@Status,0,@Subject,@Url, @Description,0,0,NULL, @Password,0," +
                                     "@Type,'127.0.0.1', NULL, 0, 0, @Moderation, @Subscription,@Order,'',30," +
                                     "'',365,30,@CountPosts,NULL,NULL,@Polls)";

            parms.Add(new OleDbParameter("@CatId", OleDbType.Numeric) { Value = forum.CatId });
            parms.Add(new OleDbParameter("@Status", OleDbType.Numeric) { Value = forum.Status });
            parms.Add(new OleDbParameter("@Subject", OleDbType.VarChar) { Value = forum.Subject });
            parms.Add(new OleDbParameter("@Url", OleDbType.VarChar) { Value = forum.Url.ConvertDBNull("") });
            parms.Add(new OleDbParameter("@Description", OleDbType.VarChar) { Value = forum.Description });
            parms.Add(new OleDbParameter("@Password", OleDbType.VarChar) { Value = forum.Password.ConvertDBNull("") });
            parms.Add(new OleDbParameter("@Type", OleDbType.Numeric) { Value = forum.Type });
            parms.Add(new OleDbParameter("@Moderation", OleDbType.Numeric) { Value = forum.ModerationLevel });
            parms.Add(new OleDbParameter("@Subscription", OleDbType.Numeric) { Value = forum.SubscriptionLevel });
            parms.Add(new OleDbParameter("@Order", OleDbType.Numeric) { Value = forum.Order });
            parms.Add(new OleDbParameter("@CountPosts", OleDbType.Numeric) { Value = forum.UpdatePostCount });
            parms.Add(new OleDbParameter("@Polls", OleDbType.Numeric) { Value = forum.AllowPolls });

            return SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strsql + strvalues, parms.ToArray());

        }

        public void Update(ForumInfo forum)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            StringBuilder strSql = new StringBuilder("UPDATE FORUM_FORUM SET ");
            strSql.AppendLine("CAT_ID=@CatId,");
            strSql.AppendLine("F_STATUS=@Status,");
            strSql.AppendLine("F_SUBJECT=@Subject,");
            strSql.AppendLine("F_URL=@Url,");
            strSql.AppendLine("F_DESCRIPTION=@Description,");
            strSql.AppendLine("F_PASSWORD_NEW=@Password,");
            strSql.AppendLine("F_TYPE=@Type,");
            strSql.AppendLine("F_MODERATION=@Moderation,");
            strSql.AppendLine("F_SUBSCRIPTION=@Subscription,");
            strSql.AppendLine("F_ORDER=@Order,");
            strSql.AppendLine("F_COUNT_M_POSTS=@CountPosts,");
            strSql.AppendLine("F_POLLS=@Polls");
            strSql.AppendLine("WHERE FORUM_ID=@ForumId");

            parms.Add(new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forum.Id });
            parms.Add(new OleDbParameter("@CatId", OleDbType.Numeric) { Value = forum.CatId });
            parms.Add(new OleDbParameter("@Status", OleDbType.Numeric) { Value = forum.Status });
            parms.Add(new OleDbParameter("@Subject", OleDbType.VarChar) { Value = forum.Subject });
            parms.Add(new OleDbParameter("@Url", OleDbType.VarChar) { Value = forum.Url.ConvertDBNull(), IsNullable = true });
            parms.Add(new OleDbParameter("@Description", OleDbType.VarChar) { Value = forum.Description });
            parms.Add(new OleDbParameter("@Password", OleDbType.VarChar) { Value = forum.Password.ConvertDBNull(), IsNullable = true });
            parms.Add(new OleDbParameter("@Type", OleDbType.Numeric) { Value = forum.Type });
            parms.Add(new OleDbParameter("@Moderation", OleDbType.Numeric) { Value = forum.ModerationLevel });
            parms.Add(new OleDbParameter("@Subscription", OleDbType.Numeric) { Value = forum.SubscriptionLevel });
            parms.Add(new OleDbParameter("@Order", OleDbType.Numeric) { Value = forum.Order });
            parms.Add(new OleDbParameter("@CountPosts", OleDbType.Numeric) { Value = forum.UpdatePostCount });
            parms.Add(new OleDbParameter("@Polls", OleDbType.Numeric) { Value = forum.AllowPolls });

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql.ToString(), parms.ToArray());
        }

        public void Delete(ForumInfo forum)
        {
            const string strSql = "DELETE FROM FORUM_FORUM WHERE FORUM_ID=@ForumId ";

            EmptyForum(forum.Id);

            OleDbParameter forumid = new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forum.Id };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, forumid);
        }

        public void Dispose()
        {

        }

        #endregion

        public IEnumerable<ForumInfo> GetAll()
        {
            List<ForumInfo> forums = new List<ForumInfo>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT " + FORUM_COLS + FROM_CLAUSE + " ORDER BY F.F_SUBJECT", null))
            {
                while (rdr.Read())
                {
                    forums.Add(BoHelper.CopyForumToBO(rdr));
                }
            }
            return forums;
        }

    }
}
