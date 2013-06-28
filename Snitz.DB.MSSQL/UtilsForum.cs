using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using SnitzCommon;

namespace SnitzData
{
    public static partial class Util
    {
        /// <summary>
        /// Get forum object
        /// </summary>
        /// <param name="forumid">Id of forum</param>
        /// <returns></returns>
        public static Forum GetForum(int forumid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return dc.GetForum(forumid);
            }
        }
        /// <summary>
        /// Updates the last post info for a Forum
        /// </summary>
        /// <param name="forumid"></param>
        private static void UpdateLastForumPost(int forumid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {

                Forum forum = dc.Forums.Where(f => f.Id == forumid).FirstOrDefault();
                int replycount = dc.Replies.Where(r => r.ForumId == forumid).Count();
                int topiccount = dc.Topics.Where(t => t.ForumId == forumid).Count();
                forum.PostCount = topiccount + replycount;
                if (forum.PostCount == 0)
                {
                    forum.LastReplyId = 0;
                    forum.LastTopicId = 0;
                    forum.F_LAST_POST = null;
                    forum.F_LAST_POST_AUTHOR = 0;
                }
                else
                {
                    var lastreply = dc.Replies.Where(r => r.ForumId == forumid).OrderByDescending(r => r.R_DATE).FirstOrDefault();
                    var lasttopic = dc.Topics.Where(t => t.ForumId == forumid).OrderByDescending(t => t.T_DATE).FirstOrDefault();
                    DateTime lasttopicdate = lasttopic.LastEditDate ?? lasttopic.Date;
                    DateTime lastreplydate;
                    if (lastreply == null)
                        lastreplydate = DateTime.MinValue;
                    else
                        lastreplydate = lastreply.LastEditDate ?? lastreply.Date;

                    if (lasttopicdate > lastreplydate)
                    {
                        forum.LastReplyId = 0;

                        forum.F_LAST_POST_AUTHOR = lasttopic.T_AUTHOR;
                        forum.LastTopicId = lasttopic.Id;
                        forum.F_LAST_POST = lasttopicdate.ToForumDateStr();
                    }
                    else
                    {
                        forum.LastReplyId = lastreply.Id;
                        forum.LastTopicId = lastreply.TopicId;
                        forum.F_LAST_POST_AUTHOR = lastreply.R_AUTHOR;
                        forum.F_LAST_POST = lastreplydate.ToForumDateStr();
                    }
                }
                dc.SubmitChanges();
            }
        }

        public static void SetForumStatus(int forumid, Enumerators.PostStatus status)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Forum forum = dc.Forums.SingleOrDefault(f => f.Id == forumid);
                forum.F_STATUS = (short)status;
                dc.SubmitChanges();
            }
        }

        public static void DeleteForum(int forumid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.Replies.DeleteAllOnSubmit(dc.Replies.Where(r => r.ForumId == forumid));
                dc.Topics.DeleteAllOnSubmit(dc.Topics.Where(t => t.ForumId == forumid));
                dc.Forums.DeleteAllOnSubmit(dc.Forums.Where(f => f.Id == forumid));
                dc.SubmitChanges();
                RemoveAllForumSubscriptions(forumid);
            }
        }

        public static void EmptyForum(int forumid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.Replies.DeleteAllOnSubmit(dc.Replies.Where(r => r.ForumId == forumid));
                dc.Topics.DeleteAllOnSubmit(dc.Topics.Where(t => t.ForumId == forumid));
                dc.SubmitChanges();

                Forum forum = dc.Forums.SingleOrDefault(f => f.Id == forumid);
                forum.PostCount = 0;
                forum.TopicCount = 0;
                forum.LastReplyId = 0;
                forum.LastTopicId = 0;
                forum.F_LAST_POST = null;
                forum.F_LAST_POST_AUTHOR = 0;
                dc.SubmitChanges();

            }
        }
        
        public static void UpdateForum(int forum)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Forum toUpd = dc.Forums.SingleOrDefault(f => f.Id == forum);
                var replies = (from r in dc.Replies
                               where r.ForumId == toUpd.Id
                               orderby r.R_LAST_EDIT ?? r.R_DATE descending
                               select r);
                var topics = (from t in dc.Topics
                              where t.ForumId == toUpd.Id
                              orderby t.T_LAST_EDIT ?? t.T_DATE descending
                              select t);
                toUpd.TopicCount = topics.Count();
                toUpd.PostCount = toUpd.TopicCount + replies.Count();

                Topic lastTopic = topics.FirstOrDefault();
                Reply lastreply = replies.FirstOrDefault();

                toUpd.LastTopicId = lastTopic != null ? lastTopic.Id : 0;
                toUpd.LastReplyId = lastreply != null ? lastreply.Id : 0;

                DateTime? lasttopicDate = null;
                DateTime? lastreplyDate = null;

                if (lastTopic != null)
                {
                    lasttopicDate = lastTopic.LastEditDate ?? lastTopic.Date;
                }
                if (lastreply != null) lastreplyDate = lastreply.LastEditDate ?? lastreply.Date;

                if (lastreplyDate == null && lasttopicDate == null)
                {
                    toUpd.F_LAST_POST = null;
                }
                else if (lastreplyDate > lasttopicDate)
                {
                    if (lastreply != null)
                    {
                        toUpd.F_LAST_POST = lastreply.R_LAST_EDIT ?? lastreply.R_DATE;
                        toUpd.F_LAST_POST_AUTHOR = lastreply.R_AUTHOR;
                    }
                }
                else if (lasttopicDate > lastreplyDate)
                {
                    if (lastTopic != null)
                    {
                        toUpd.F_LAST_POST = lastTopic.T_LAST_EDIT ?? lastTopic.T_DATE;
                        toUpd.F_LAST_POST_AUTHOR = lastTopic.T_AUTHOR;
                    }
                }

                dc.SubmitChanges();
            }
        }

        public static int SaveForum(Forum forum)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                if (forum.Id == -1)
                {
                    forum.PostCount = 0;
                    forum.TopicCount = 0;
                    forum.Status = Enumerators.PostStatus.Open;
                    forum.ArchivedTopicCount = 0;
                    forum.ArchivedPostCount = 0;
                    forum.F_PRIVATEFORUMS = 0;
                    forum.LastTopicId = 0;
                    forum.LastReplyId = 0;
                    forum.AllowPolls = false;
                    forum.Type = 0;
                    dc.Forums.InsertOnSubmit(forum);
                    dc.SubmitChanges();
                    return forum.Id;
                }

                Forum toUpd = dc.Forums.SingleOrDefault(f => f.Id == forum.Id);
                toUpd.CatId = forum.CatId;
                toUpd.Subject = forum.Subject;
                toUpd.Description = forum.Description;
                toUpd.UpdatePostCount = forum.UpdatePostCount;
                toUpd.AllowPolls = forum.AllowPolls;
                toUpd.ModerationLevel = forum.ModerationLevel;
                toUpd.SubscriptionLevel = forum.SubscriptionLevel;
                toUpd.AuthType = forum.AuthType;
                toUpd.Password = forum.Password;
                toUpd.Order = forum.Order;
                dc.SubmitChanges();
                return toUpd.Id;

            }
        }

        public static void AddForumModerators(int forumid, string[] moderators)
        {
            Forum forum = GetForum(forumid);
            Dictionary<int, string> forummods = forum.Moderators;
            foreach (string moderator in moderators)
            {
                if (!forummods.Values.Contains(moderator))
                {
                    forum.AddModerator(moderator);
                }
            }
        }
        
        public static List<Forum> ListForums()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return dc.Forums.OrderBy(f => f.Subject).ToList();
            }
        }

        public static List<int> AllowedForums(Member member)
        {

            List<int> roleList = new List<int> { 0 };
            bool isadmin = Roles.IsUserInRole("Administrator");
            foreach (KeyValuePair<int, string> _role in SnitzCachedLists.UserRoles())
            {
                roleList.Add(_role.Key);
            }
            List<int> allowedForums = new List<int>();
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                foreach (Forum forum in dc.Forums)
                {
                    if (forum.AllowedRoles.Count == 0)
                        allowedForums.Add(forum.Id);
                    else
                    {
                        foreach (int role in forum.AllowedRoles)
                        {
                            if (roleList.Contains(role) || isadmin)
                            {
                                allowedForums.Add(forum.Id);
                                break;
                            }
                        }
                    }
                }
            }
            return allowedForums;
        }
        /// <summary>
        /// Fetches the Jumpto items from the database
        /// </summary>
        /// <returns></returns>
        public static List<ForumJumpto> ListForumJumpTo()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                var res = (from forums in dc.Forums join category in dc.Categories on forums.CatId equals category.Id orderby category.Order, forums.Order select new ForumJumpto() { Id = forums.Id, Name = forums.Subject, Category = category.Name });
                return res.ToList();
            }
        }

    }
}
