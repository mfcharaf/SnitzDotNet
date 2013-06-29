using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnitzCommon;
using SnitzConfig;

namespace SnitzData
{
    public static partial class Util
    {
        /// <summary>
        /// get reply Object
        /// </summary>
        /// <param name="replyid">Id of reply</param>
        /// <returns></returns>
        public static Reply GetReply(int replyid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return dc.GetReply(replyid);
            }
        }
        
        public static int AddReply(Reply message, Member member)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic topic = (from tpc in dc.Topics where tpc.Id == message.TopicId select tpc).SingleOrDefault();
                Forum forum = (from f in dc.Forums where f.Id == topic.ForumId select f).SingleOrDefault();
                Enumerators.Subscription subType = (Enumerators.Subscription)forum.SubscriptionLevel;
                Reply reply = new Reply
                {
                    Message = message.Message,
                    R_AUTHOR = member.Id,
                    R_DATE = DateTime.UtcNow.ToForumDateStr(),
                    R_STATUS = (short)message.Status,
                    CatId = forum.CatId,
                    ForumId = topic.ForumId,
                    TopicId = message.TopicId,
                    PostersIP = Config.LogIP ? message.PostersIP : null,
                    UseSignatures = message.UseSignatures
                };
                if (forum.ModerationLevel == Enumerators.Moderation.AllPosts || forum.ModerationLevel == Enumerators.Moderation.Replies)
                {
                    reply.Status = Enumerators.PostStatus.UnModerated;
                    topic.UnModeratedReplies += 1;
                }
                dc.Replies.InsertOnSubmit(reply);
                dc.SubmitChanges();
                if (reply.Status != Enumerators.PostStatus.UnModerated)
                {
                    //Update topic
                    topic.T_LAST_POST = reply.R_DATE;
                    topic.LastPostAuthorId = reply.R_AUTHOR;
                    topic.LastReplyId = reply.Id;
                    topic.ReplyCount += 1;
                    //update forum
                    forum.F_LAST_POST = reply.R_DATE;
                    forum.F_LAST_POST_AUTHOR = reply.R_AUTHOR;
                    forum.LastTopicId = topic.Id;
                    forum.LastReplyId = reply.Id;
                    forum.PostCount += 1;

                    //update author
                    if (forum.UpdatePostCount.HasValue)
                        if (forum.UpdatePostCount.Value)
                        {
                            Member auth = dc.Members.SingleOrDefault(m => m.Id == reply.R_AUTHOR);
                            auth.M_POSTS += 1;
                            auth.M_LASTPOSTDATE = reply.R_DATE;
                            auth.M_LAST_IP = Config.LogIP ? reply.PostersIP : null;
                        }


                }
                else
                {
                    //topic.UnModeratedReplies += 1;
                }
                dc.SubmitChanges();
                if (subType != Enumerators.Subscription.None && reply.Status != Enumerators.PostStatus.UnModerated)
                {
                    //process subscriptions in a background thread
                    ProcessSubscriptions(subType, topic, reply);
                }
                return reply.Id;
            }
        }
        
        public static void UpdateReply(int replyid, string message, Member member, bool isAdministrator, bool showsig)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Reply reply = dc.Replies.Where(r => r.Id == replyid).SingleOrDefault();
                reply.Message = message;
                reply.UseSignatures = showsig;
                if (!isAdministrator)
                {
                    reply.R_LAST_EDITBY = member.Id;
                    reply.R_LAST_EDIT = DateTime.UtcNow.ToForumDateStr();
                    if (reply.Status != Enumerators.PostStatus.UnModerated)
                    {
                        Topic topic = (from tpc in dc.Topics where tpc.Id == reply.TopicId select tpc).SingleOrDefault();
                        topic.T_LAST_POST = reply.R_LAST_EDIT;
                        topic.LastPostAuthorId = reply.R_LAST_EDITBY;
                        topic.LastReplyId = reply.Id;

                        //update forum
                        Forum forum =
                            (from forum1 in dc.Forums where forum1.Id == topic.ForumId select forum1).SingleOrDefault();
                        forum.F_LAST_POST = reply.R_LAST_EDIT;
                        forum.F_LAST_POST_AUTHOR = reply.R_LAST_EDITBY;
                    }
                }
                dc.SubmitChanges();
            }
        }
        
        public static void MoveReplies(int newtopicid, List<int> replyIDs)
        {
            //todo check moderation status
            Reply lastreply = null;
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Topic newTopic = dc.Topics.SingleOrDefault(t => t.Id == newtopicid);

                var replies = dc.Replies.Where(r => replyIDs.Contains(r.Id));
                newTopic.ReplyCount = replies.Count();

                foreach (Reply reply in replies)
                {
                    reply.TopicId = newtopicid;
                    reply.ForumId = newTopic.ForumId;
                    reply.CatId = newTopic.ForumId;
                    if (lastreply == null)
                        lastreply = reply;
                    else
                    {
                        if (reply.Date > lastreply.Date)
                            lastreply = reply;
                    }
                }
                if (lastreply != null)
                {
                    newTopic.T_LAST_POST = lastreply.R_DATE;
                    newTopic.LastPostAuthorId = lastreply.R_AUTHOR;
                    newTopic.LastReplyId = lastreply.Id;
                }
                dc.SubmitChanges();
            }
        }
        
        public static int FindReplyPage(int replyid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Reply reply = GetReply(replyid);
                if (reply == null)
                    return 1;
                var replies = (from r in dc.Replies where r.TopicId == reply.TopicId select r.Id);

                List<int> Ids = replies.ToList();
                bool found = false;
                int pagesize = SnitzConfig.Config.TopicPageSize;
                int page = 0;
                while (!found)
                {
                    List<int> sublist = (Ids.Skip(page * pagesize).Take(pagesize)).ToList();
                    if (sublist.Contains(replyid))
                    {
                        found = true;
                        page += 1;
                        continue;
                    }
                    page += 1;
                    if (sublist.Count < pagesize)
                    {
                        found = true;
                        page = -1;
                    }
                }
                return page;
            }
        }

        public static void SetReplyStatus(int replyid, Enumerators.PostStatus status)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Reply reply = dc.Replies.SingleOrDefault(r => r.Id == replyid);
                Topic topic = dc.Topics.SingleOrDefault(t => t.Id == reply.TopicId);
                if (reply.Status == Enumerators.PostStatus.UnModerated && status == Enumerators.PostStatus.Open)
                {
                    topic.UnModeratedReplies -= 1;
                }
                reply.R_STATUS = (short)status;

                dc.SubmitChanges();
                UpdateLastTopicPost(topic.Id);
            }
        }
        /// <summary>
        /// Deletes a reply
        /// </summary>
        /// <param name="id">id of reply</param>
        public static void DeleteReply(int id)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Reply reply = dc.Replies.Where(r => r.Id == id).FirstOrDefault();
                int topicid = reply.TopicId;

                dc.Replies.DeleteOnSubmit(reply);
                dc.SubmitChanges();

                UpdateLastTopicPost(topicid);
            }
        }

    }
}
