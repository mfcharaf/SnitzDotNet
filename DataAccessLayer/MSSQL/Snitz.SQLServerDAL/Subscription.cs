using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class Subscription : ISubscription
    {

        public void Delete(int subscriptionid)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE SUBSCRIPTION_ID=@SubsId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@SubsId",SqlDbType.Int){Value = subscriptionid});
        }

        public IEnumerable<SubscriptionInfo> GetTopicSubscriptions(int topicid)
        {
            var topicsubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID, TOPIC_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE TOPIC_ID = @Topic";
            SqlParameter parm = new SqlParameter("@Topic", SqlDbType.Int) {Value = topicid};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    topicsubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1), TopicId = rdr.GetInt32(2) });
                }
            }
            return topicsubs;
        }

        public IEnumerable<SubscriptionInfo> GetForumSubscriptions(int forumid)
        {
            var forumsubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID, FORUM_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE FORUM_ID = @Forum AND TOPIC_ID=0";
            SqlParameter parm = new SqlParameter("@Forum", SqlDbType.Int) {Value = forumid};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    forumsubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1), ForumId = rdr.GetInt32(2) });
                }
            }
            return forumsubs;
        }

        public IEnumerable<SubscriptionInfo> GetCategorySubscriptions(int catid)
        {
            var catsubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID, CAT_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE CAT_ID = @Category AND FORUM_ID=0 AND TOPIC_ID=0";
            SqlParameter parm = new SqlParameter("@Category", SqlDbType.Int) {Value = catid};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    catsubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1), CategoryId = rdr.GetInt32(2) });
                }
            }
            return catsubs;
        }

        public IEnumerable<SubscriptionInfo> GetBoardSubscriptions()
        {
            //OR (S.CAT_ID = 0 AND S.FORUM_ID = 0 AND S.TOPIC_ID = 0)
            var boardsubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE CAT_ID = 0 AND FORUM_ID=0 AND TOPIC_ID=0";
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    boardsubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1) });
                }
            }
            return boardsubs;
        }

        public IEnumerable<SubscriptionInfo> GetMemberSubscriptions(int memberid)
        {
            var membersubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID, CAT_ID, FORUM_ID, TOPIC_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member";
            SqlParameter parm = new SqlParameter("@Member", SqlDbType.Int) {Value = memberid};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    membersubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1), CategoryId = rdr.SafeGetInt32(2),ForumId = rdr.SafeGetInt32(3),TopicId = rdr.SafeGetInt32(4)});
                }
            }
            return membersubs;
        }

        public void RemoveAllCategorySubscriptions(int categoryid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE CAT_ID = @Category AND FORUM_ID=0 AND TOPIC_ID=0";
            SqlParameter parm = new SqlParameter("@Category", SqlDbType.Int) {Value = categoryid};
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveAllForumSubscriptions(int forumid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE FORUM_ID = @Forum AND TOPIC_ID=0";
            SqlParameter parm = new SqlParameter("@Forum", SqlDbType.Int) {Value = forumid};
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveAllTopicSubscriptions(int topicid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE TOPIC_ID = @Topic";
            SqlParameter parm = new SqlParameter("@Topic", SqlDbType.Int) {Value = topicid};
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveAllBoardSubscriptions()
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE CAT_ID = 0 AND FORUM_ID=0 AND TOPIC_ID=0";
            
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, null);
        }

        public void RemoveAllMemberSubscriptions(int memberid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member";
            SqlParameter parm = new SqlParameter("@Member", SqlDbType.Int) {Value = memberid};
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveMembersForumSubscriptions(int memberid, int forumid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member AND FORUM_ID = @Forum";
            List<SqlParameter> parms = new List<SqlParameter>(new SqlParameter[2])
                {
                    new SqlParameter("@Member", SqlDbType.Int) {Value = memberid},
                    new SqlParameter("@Forum", SqlDbType.Int) {Value = forumid}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parms.ToArray());
        }

        public void RemoveMembersTopicSubscription(int memberid, int topicid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member AND TOPIC_ID=@TopicId";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@Member", SqlDbType.Int) {Value = memberid},
                new SqlParameter("@TopicId", SqlDbType.Int) {Value = topicid}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parms.ToArray());
        }

        public IEnumerable<SubscriptionInfo> GetAllSubscriptions()
        {
            var allsubs = new List<SubscriptionInfo>();

            string SqlStr =
                "SELECT SUBSCRIPTION_ID, SUBS.MEMBER_ID, SUBS.CAT_ID, SUBS.FORUM_ID, SUBS.TOPIC_ID,C.CAT_NAME,F.F_SUBJECT,T.T_SUBJECT,M.M_NAME " +
                "FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS SUBS " +
                "LEFT JOIN " + Config.ForumTablePrefix + "FORUM F ON F.FORUM_ID = SUBS.FORUM_ID " +
                "LEFT JOIN " + Config.ForumTablePrefix + "CATEGORY C ON C.CAT_ID = SUBS.CAT_ID " +
                "LEFT JOIN " + Config.ForumTablePrefix + "TOPICS T ON T.TOPIC_ID = SUBS.TOPIC_ID " +
                "LEFT JOIN " + Config.MemberTablePrefix + "MEMBERS M ON M.MEMBER_ID = SUBS.MEMBER_ID";
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    allsubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), 
                        MemberId = rdr.GetInt32(1), 
                        CategoryId = rdr.SafeGetInt32(2), 
                        ForumId = rdr.SafeGetInt32(3), 
                        TopicId = rdr.SafeGetInt32(4),
                        CategoryName = rdr.SafeGetString(5),
                        ForumSubject = rdr.SafeGetString(6),
                        TopicSubject = rdr.SafeGetString(7),
                        MemberName = rdr.SafeGetString(8)
                    });
                }
            }
            return allsubs;
        }

        public int[] GetForumSubscriptionList(int forumId)
        {
            var forumsubscribers = new List<int>();
            string SqlStr = "SELECT MEMBER_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE FORUM_ID = @Forum AND COALESCE(TOPIC_ID, 0) = 0";
            SqlParameter parm = new SqlParameter("@Forum", SqlDbType.Int);
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    forumsubscribers.Add(rdr.GetInt32(0));
                }
            }
            return forumsubscribers.ToArray();
        }

        public int[] GetTopicSubscriptionList(int forumId)
        {
            var topicsubscribers = new List<int>();
            string SqlStr = "SELECT MEMBER_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE TOPIC_ID = @Topic";
            SqlParameter parm = new SqlParameter("@Topic", SqlDbType.Int);
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    topicsubscribers.Add(rdr.GetInt32(0));
                }
            }
            return topicsubscribers.ToArray();
        }

        public int Add(SubscriptionInfo subscription)
        {
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "SUBSCRIPTIONS " +
                                  "(MEMBER_ID,CAT_ID,TOPIC_ID,FORUM_ID) VALUES (@MemberId,@CatId,@TopicId,@ForumId)";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@MemberId", SqlDbType.Int) {Value = subscription.MemberId},
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = subscription.CategoryId},
                    new SqlParameter("@TopicId", SqlDbType.Int) {Value = subscription.TopicId},
                    new SqlParameter("@ForumId", SqlDbType.Int) {Value = subscription.ForumId}
                };

            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

    }
}
