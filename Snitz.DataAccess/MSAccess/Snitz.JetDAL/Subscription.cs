using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class Subscription : ISubscription
    {

        public void Delete(int subscriptionid)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE SUBSCRIPTION_ID=@SubsId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@SubsId", OleDbType.Numeric) { Value = subscriptionid });
        }

        public IEnumerable<SubscriptionInfo> GetTopicSubscriptions(int topicid)
        {
            var topicsubs = new List<SubscriptionInfo>();

            string SqlStr = "SELECT SUBSCRIPTION_ID, MEMBER_ID, TOPIC_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE TOPIC_ID = @Topic";
            OleDbParameter parm = new OleDbParameter("@Topic", OleDbType.Numeric) { Value = topicid };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
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
            OleDbParameter parm = new OleDbParameter("@Forum", OleDbType.Numeric) { Value = forumid };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
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
            OleDbParameter parm = new OleDbParameter("@Category", OleDbType.Numeric) { Value = catid };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
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
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
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
            OleDbParameter parm = new OleDbParameter("@Member", OleDbType.Numeric) { Value = memberid };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
            {
                while (rdr.Read())
                {
                    membersubs.Add(new SubscriptionInfo { Id = rdr.GetInt32(0), MemberId = rdr.GetInt32(1), CategoryId = rdr.SafeGetInt32(2), ForumId = rdr.SafeGetInt32(3), TopicId = rdr.SafeGetInt32(4) });
                }
            }
            return membersubs;
        }

        public void RemoveAllCategorySubscriptions(int categoryid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE CAT_ID = @Category AND FORUM_ID=0 AND TOPIC_ID=0";
            OleDbParameter parm = new OleDbParameter("@Category", OleDbType.Numeric) { Value = categoryid };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveAllForumSubscriptions(int forumid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE FORUM_ID = @Forum AND TOPIC_ID=0";
            OleDbParameter parm = new OleDbParameter("@Forum", OleDbType.Numeric) { Value = forumid };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveAllTopicSubscriptions(int topicid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE TOPIC_ID = @Topic";
            OleDbParameter parm = new OleDbParameter("@Topic", OleDbType.Numeric) { Value = topicid };
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
            OleDbParameter parm = new OleDbParameter("@Member", OleDbType.Numeric) { Value = memberid };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parm);
        }

        public void RemoveMembersForumSubscriptions(int memberid, int forumid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member AND FORUM_ID = @Forum AND TOPIC_ID=0";
            List<OleDbParameter> parms = new List<OleDbParameter>(new OleDbParameter[2])
                {
                    new OleDbParameter("@Member", OleDbType.Numeric) {Value = memberid},
                    new OleDbParameter("@Forum", OleDbType.Numeric) {Value = forumid}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parms.ToArray());
        }

        public void RemoveMembersTopicSubscription(int memberid, int topicid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member AND TOPIC_ID=@TopicId";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@Member", OleDbType.Numeric) {Value = memberid},
                new OleDbParameter("@TopicId", OleDbType.Numeric) {Value = topicid}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parms.ToArray());
        }

        public void RemoveMembersCategorySubscriptions(int memberid, int categoryid)
        {
            string SqlStr = "DELETE FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE MEMBER_ID = @Member AND CAT_ID=@CatId AND FORUM_ID=0 AND TOPIC_ID=0";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@Member", OleDbType.Numeric) {Value = memberid},
                new OleDbParameter("@CatId", OleDbType.Numeric) {Value = categoryid}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, SqlStr, parms.ToArray());
        }

        public IEnumerable<SubscriptionInfo> GetAllSubscriptions()
        {
            var allsubs = new List<SubscriptionInfo>();

            string SqlStr =
                "SELECT SUBSCRIPTION_ID, SUBS.MEMBER_ID, SUBS.CAT_ID, SUBS.FORUM_ID, SUBS.TOPIC_ID,C.CAT_NAME,F.F_SUBJECT,T.T_SUBJECT,M.M_NAME,M.M_STATUS " +
                "FROM (((" + Config.ForumTablePrefix + "SUBSCRIPTIONS SUBS " +
                "LEFT JOIN " + Config.ForumTablePrefix + "FORUM F ON F.FORUM_ID = SUBS.FORUM_ID) " +
                "LEFT JOIN " + Config.ForumTablePrefix + "CATEGORY C ON C.CAT_ID = SUBS.CAT_ID) " +
                "LEFT JOIN " + Config.ForumTablePrefix + "TOPICS T ON T.TOPIC_ID = SUBS.TOPIC_ID) " +
                "LEFT JOIN " + Config.MemberTablePrefix + "MEMBERS M ON M.MEMBER_ID = SUBS.MEMBER_ID";
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    allsubs.Add(new SubscriptionInfo
                    {
                        Id = rdr.GetInt32(0),
                        MemberId = rdr.GetInt32(1),
                        CategoryId = rdr.SafeGetInt32(2),
                        ForumId = rdr.SafeGetInt32(3),
                        TopicId = rdr.SafeGetInt32(4),
                        CategoryName = rdr.SafeGetInt32(2) == null ? "" : rdr.SafeGetString(5, "Category Deleted"),
                        ForumSubject = rdr.SafeGetInt32(3) == null ? "" : rdr.SafeGetString(6, "Forum Deleted"),
                        TopicSubject = rdr.SafeGetInt32(4) == 0 ? "" : rdr.SafeGetString(7, "Topic Deleted"),

                        MemberName = rdr.SafeGetString(8) + (rdr.SafeGetInt16(9) == 0 ? "(Locked)" : "")
                    });
                }
            }
            return allsubs;
        }

        public int[] GetForumSubscriptionList(int forumId)
        {
            var forumsubscribers = new List<int>();
            string SqlStr = "SELECT MEMBER_ID FROM " + Config.ForumTablePrefix + "SUBSCRIPTIONS WHERE FORUM_ID = @Forum AND IIF(TOPIC_ID IS NULL, 0,TOPIC_ID) = 0";
            OleDbParameter parm = new OleDbParameter("@Forum", OleDbType.Numeric);
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
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
            OleDbParameter parm = new OleDbParameter("@Topic", OleDbType.Numeric);
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, parm))
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
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@MemberId", OleDbType.Numeric) {Value = subscription.MemberId},
                    new OleDbParameter("@CatId", OleDbType.Numeric) {Value = subscription.CategoryId},
                    new OleDbParameter("@TopicId", OleDbType.Numeric) {Value = subscription.TopicId},
                    new OleDbParameter("@ForumId", OleDbType.Numeric) {Value = subscription.ForumId}
                };

            return SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

    }
}
