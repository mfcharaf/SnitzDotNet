using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SnitzCommon;
using SnitzConfig;

namespace SnitzData
{
    public partial class Topic
    {
        public Forum Forum { get { return Util.GetForum(this.ForumId); } }
        
        public Enumerators.PostStatus Status
        {
            get { return (Enumerators.PostStatus)this.T_STATUS; }
            set
            {
                this.T_STATUS = (short)value;
            }
        }

        public DateTime? LastEditDate
        {
            get
            {
                return string.IsNullOrEmpty(this.T_LAST_EDIT)
                        ? (DateTime?)null : this.T_LAST_EDIT.ToDateTime();

            }
        }
        public DateTime Date
        {
            get { return this.T_DATE.ToDateTime().Value; }
            set { this.T_DATE = value.ToForumDateStr(); }
        }
        public DateTime LastPostDate 
        { 
            get
            {
                return this.T_LAST_POST.ToDateTime().HasValue ? this.T_LAST_POST.ToDateTime().Value : this.T_DATE.ToDateTime().Value;
            }
        }
        public string LastEditTimeAgo
        {
            
           get
            {
                Member currentuser = Util.GetMember(HttpContext.Current.User.Identity.Name);
                if (this.LastEditDate.HasValue)
                    return String.Format("<abbr class='timeago' title='{0}'>{1}</abbr>",
                                         this.LastEditDate.Value.ToISO8601Date(
                                             HttpContext.Current.User.Identity.IsAuthenticated, currentuser.TimeOffset),
                                         this.LastEditDate.Value.ToForumDateDisplay(" ", true,
                                                                                    HttpContext.Current.User.Identity.
                                                                                        IsAuthenticated,
                                                                                    currentuser.TimeOffset));
                else
                    return String.Empty;
            }
        }

        public Member Author
        {
            get { return new SnitzDataClassesDataContext().GetAuthor(this.T_AUTHOR); }
        }
        public Member LastPostAuthor
        {
            get { return new SnitzDataClassesDataContext().GetAuthor(this.LastPostAuthorId); }
        }
        public Member LastEditBy
        {
            get { return new SnitzDataClassesDataContext().GetAuthor(this.T_LAST_EDITBY); }
        }
        public IEnumerable<Reply> Replies { get { return GetReplies(this.Id); } }

        public string ParsedMessage
        {
            get
            {
                return this.Message.ReplaceNoParseTags().ReplaceBadWords().ReplaceSmileTags().ReplaceImageTags().ReplaceURLs().ReplaceTableTags().
                      ReplaceVideoTag().ReplaceFileTags().ReplaceCodeTags(this.Id, "T").ReplaceTags();
            }
        }
        public string ParsedSubject
        {
            get
            {
                return this.Subject.ReplaceNoParseTags().ReplaceBadWords().ReplaceSmileTags().ReplaceTags();
            }
        }

        public int PageCount
        {
            get
            {
                int extra = 0;
                if (this.ReplyCount % Config.TopicPageSize > 0)
                    extra = 1;
                return (this.ReplyCount/Config.TopicPageSize) + extra;
            }
        }

        public bool AllowSubscriptions
        {
            get
            {
                if (this.Forum.SubscriptionLevel == (int)Enumerators.Subscription.TopicSubscription)
                    return true;
                return false;
            }
        }

        public int[] Subscribers
        {
            get { return Util.GetTopicSubscriptions(this.Id); }
        }

        public int? PollID
        {
            get { return Util.GetPollID(this.Id); }
            set { throw new NotImplementedException(); }
        }

        private static IEnumerable<Reply> GetReplies(int topicid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            return (from reply in dc.Replies where reply.TopicId == topicid orderby reply.R_DATE select reply).ToList();
        }

        public void Delete()
        {
            Util.DeleteTopic(this.Id);
        }
    }
}
