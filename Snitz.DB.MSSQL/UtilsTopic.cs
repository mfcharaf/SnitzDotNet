using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SnitzCommon;
using SnitzConfig;

namespace SnitzData
{
    public static partial class Util
    {
        /// <summary>
        /// Fetches a topic from the database
        /// </summary>
        /// <param name="topicid">Id of topic</param>
        /// <returns></returns>
        public static Topic GetTopic(int topicid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return dc.GetTopic(topicid);
            }
        }

        /// <summary>
        /// Returns the next or prev topic in a forum
        /// </summary>
        /// <param name="topic">List of forum topics</param>
        /// <param name="which">prev/next flag</param>
        /// <returns></returns>
        public static Topic GetTopic(Topic topic, string which)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                IEnumerable<Topic> res;
                //Topic topic = topics.SingleOrDefault();

                if (which == "next")
                    res = (from t in dc.Topics
                           where t.ForumId == topic.ForumId && t.T_LAST_POST.CompareTo(topic.T_LAST_POST) > 0
                           orderby t.T_LAST_POST
                           select t);
                else
                    res = (from t in dc.Topics
                           where t.ForumId == topic.ForumId && t.T_LAST_POST.CompareTo(topic.T_LAST_POST) < 0
                           orderby t.T_LAST_POST descending
                           select t);

                if (res.Count() > 0)
                {
                    res = res.Take(1);
                }
                if (res.Count() == 0)
                {
                    return topic;
                }
                return res.SingleOrDefault();
            }
        }

        public static Topic GetTopic(int topicid, bool archive)
        {
            if (!archive)
                using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
                {
                    return (from topic in dc.Topics where topic.Id == topicid select topic).SingleOrDefault();
                }
            return null;
        }

        /// <summary>
        /// deletes a topic
        /// </summary>
        /// <param name="id">id of topic</param>
        public static void DeleteTopic(int id)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.Replies.DeleteAllOnSubmit(dc.Replies.Where(r => r.TopicId == id));
                Topic topic = dc.Topics.Where(t => t.Id == id).FirstOrDefault();
                int forumid = topic.ForumId;
                dc.Topics.DeleteAllOnSubmit(dc.Topics.Where(t => t.Id == id));
                dc.SubmitChanges();
                UpdateLastForumPost(forumid);
                RemoveAllTopicSubscriptions(id);
            }
        }
        /// <summary>
        /// Updates last post info of a Topic
        /// </summary>
        /// <param name="topicid"></param>
        private static void UpdateLastTopicPost(int topicid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = dc.Topics.Where(t => t.Id == topicid).FirstOrDefault();
                topic.ReplyCount = dc.Replies.Where(t => t.TopicId == topicid).Count();
                var lastreply = dc.Replies.Where(t => t.TopicId == topicid).OrderByDescending(r => r.R_DATE).FirstOrDefault();
                if (lastreply == null)
                {
                    topic.T_LAST_POST = null;
                    topic.LastPostAuthorId = 0;
                }
                else
                {
                    topic.LastReplyId = lastreply.Id;
                    topic.LastPostAuthorId = lastreply.R_AUTHOR;
                    DateTime lastreplydate = lastreply.LastEditDate ?? lastreply.Date;
                    topic.T_LAST_POST = lastreplydate.ToForumDateStr();
                }
                dc.SubmitChanges();
                UpdateLastForumPost(topic.ForumId);
            }
        }

        public static int AddTopic(Topic message, Member member)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Forum forum = (from f in dc.Forums where f.Id == message.ForumId select f).SingleOrDefault();
                Enumerators.Subscription subType = (Enumerators.Subscription)forum.SubscriptionLevel;
                Topic topic = new Topic
                {
                    Subject = message.Subject,
                    Message = message.Message,
                    ReplyCount = 0,
                    T_AUTHOR = member.Id,
                    LastReplyId = 0,
                    T_DATE = message.Date.ToForumDateStr(),
                    T_STATUS = (short)message.Status,
                    LastPostAuthorId = member.Id,
                    CatId = forum.CatId,
                    ForumId = message.ForumId,
                    Id = message.Id,
                    PostersIP = Config.LogIP ? message.PostersIP : null,
                    UseSignatures = message.UseSignatures,
                    UnModeratedReplies = 0,
                    ViewCount = 0
                };
                topic.T_LAST_POST = topic.T_DATE;
                if (forum.ModerationLevel == Enumerators.Moderation.AllPosts || forum.ModerationLevel == Enumerators.Moderation.Topics)
                    topic.Status = Enumerators.PostStatus.UnModerated;
                dc.Topics.InsertOnSubmit(topic);
                dc.SubmitChanges();

                //update author
                Member auth = dc.Members.SingleOrDefault(m => m.Id == member.Id);
                auth.M_LASTPOSTDATE = topic.T_DATE;
                auth.M_LAST_IP = Config.LogIP ? topic.PostersIP : null;
                if (forum.UpdatePostCount.HasValue)
                    if (forum.UpdatePostCount.Value)
                        auth.M_POSTS += 1;

                dc.SubmitChanges();
                //update forum
                if (topic.Status != Enumerators.PostStatus.UnModerated)
                {
                    forum.F_LAST_POST = topic.T_DATE;
                    forum.F_LAST_POST_AUTHOR = member.Id;
                    forum.LastTopicId = topic.Id;
                    forum.TopicCount += 1;
                    forum.PostCount += 1;
                    dc.SubmitChanges();
                }
                if (subType != Enumerators.Subscription.None && topic.Status != Enumerators.PostStatus.UnModerated)
                    //process subscriptions in a background thread
                    ProcessSubscriptions(subType, topic, null);
                return topic.Id;
            }
        }

        public static void UpdateTopic(int topicid, string message, string subject, Member member, bool isAdministrator, bool showsig)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = (from tpc in dc.Topics where tpc.Id == topicid select tpc).SingleOrDefault();
                topic.Subject = subject;
                topic.Message = message;
                topic.UseSignatures = showsig;
                if (!isAdministrator)
                {
                    topic.T_LAST_EDIT = DateTime.UtcNow.ToForumDateStr();
                    topic.T_LAST_EDITBY = member.Id;

                    //update forum
                    if (topic.Status != Enumerators.PostStatus.UnModerated)
                    {
                        Forum forum =
                            (from forum1 in dc.Forums where forum1.Id == topic.ForumId select forum1).SingleOrDefault();
                        forum.F_LAST_POST = topic.T_LAST_EDIT;
                        forum.F_LAST_POST_AUTHOR = topic.T_LAST_EDITBY;
                    }
                }
                dc.SubmitChanges();
            }
        }

        public static void SetTopicStatus(int topicid, Enumerators.PostStatus status)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = dc.Topics.SingleOrDefault(t => t.Id == topicid);
                topic.T_STATUS = (short)status;
                dc.SubmitChanges();
                UpdateLastForumPost(topic.ForumId);
            }

        }

        public static void MakeSticky(int topicid, bool sticky)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = dc.Topics.SingleOrDefault(t => t.Id == topicid);
                topic.IsSticky = sticky;
                dc.SubmitChanges();
            }
        }

        public static void ChangeTopicForum(int topicid, string forumid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                int newforumid = Convert.ToInt32(forumid);
                Topic topic = (from t in dc.Topics where t.Id == topicid select t).SingleOrDefault();
                int oldforum = topic.ForumId;
                Forum newforum = (from f in dc.Forums where f.Id == newforumid select f).Single();
                if (newforum.ModerationLevel == Enumerators.Moderation.UnModerated)
                    if (topic.Status == Enumerators.PostStatus.UnModerated || topic.Status == Enumerators.PostStatus.OnHold)
                        topic.Status = newforum.Status;
                topic.ForumId = newforumid;
                topic.CatId = newforum.CatId;
                dc.SubmitChanges();
                var replies = (from r in dc.Replies where r.TopicId == topicid select r);
                foreach (Reply reply in replies)
                {
                    if (newforum.ModerationLevel == Enumerators.Moderation.UnModerated)
                        if (reply.Status == Enumerators.PostStatus.UnModerated || reply.Status == Enumerators.PostStatus.OnHold)
                            reply.Status = newforum.Status;

                    reply.ForumId = newforumid;
                    reply.CatId = newforum.CatId;
                }
                dc.SubmitChanges();
                UpdateLastForumPost(oldforum);
                UpdateLastForumPost(newforumid);
                if (topic.Subscribers.Count() > 0)
                    UpdateTopicSubscription(topic);
            }
        }

        public static void UpdateViewCount(int topicid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = (from t in dc.Topics where t.Id == topicid select t).SingleOrDefault();
                topic.ViewCount += 1;
                dc.SubmitChanges();
            }
        }

        public static void UpdateTopic(int topic)
        {
            // reply count
            //last post id
            //last post date
            //last post author

            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic toUpd = dc.Topics.Where(t => t.Id == topic).SingleOrDefault();
                var replies = (from r in dc.Replies
                               where r.TopicId == toUpd.Id
                               orderby r.R_LAST_EDIT ?? r.R_DATE descending
                               select r);
                toUpd.ReplyCount = replies.Count();

                Reply lastreply = replies.FirstOrDefault();
                if (lastreply != null)
                {
                    toUpd.LastReplyId = lastreply.Id;
                    toUpd.T_LAST_POST = lastreply.R_LAST_EDIT ?? lastreply.R_DATE;
                    toUpd.LastPostAuthorId = lastreply.R_AUTHOR;
                }
                dc.SubmitChanges();
            }
        }

        public static List<Topic> GetTopicsForSiteMap(Int32 maxRecords)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                var res =
                    (from topic in dc.Topics
                     where topic.T_STATUS < 2
                     orderby topic.T_LAST_POST descending, topic.Id descending
                     select topic);

                return ((IEnumerable<Topic>)res.Take(maxRecords)).ToList();
            }
            //string strSQL = "SELECT TOP " + maxRecords + " T.TOPIC_ID, T.T_SUBJECT, T.T_LAST_POST FROM " + config.tablePrefix + "TOPICS T WHERE T_STATUS < 2 ORDER BY T.T_LAST_POST DESC, T.TOPIC_ID DESC";

            //using (SqlConnection conn = new SqlConnection(_connectionString))
            //{
            //    SqlCommand cmd = new SqlCommand(strSQL, conn);
            //    SqlDataAdapter da = new SqlDataAdapter(cmd);
            //    DataTable dt = new DataTable();
            //    da.Fill(dt);
            //    return dt;
            //}
        }

        public static int? GetPollID(int id)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                int? res = dc.ExecuteQuery<int>("SELECT pollid FROM FORUM_POLLS WHERE Topicid={0}", id).FirstOrDefault();
                return res;
            }
        }

        public static List<Topic> FindTopics(string topicSubject)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                var res =
                    (from topic in dc.Topics
                     where topic.Subject == HttpUtility.UrlDecode(topicSubject)
                     orderby topic.T_LAST_POST descending, topic.Id descending
                     select topic);

                return res.ToList();
            }
        }
    }
}
