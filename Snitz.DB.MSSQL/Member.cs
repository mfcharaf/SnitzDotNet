using System;
using System.Collections.Generic;
using System.Web;
using SnitzCommon;

namespace SnitzData
{
    public class MemberSubscription
    {
        public int MemberId { get; set; }
        public int CategoryId { get; set; }
        public int ForumId { get; set; }
        public int Topicid { get; set; }

        public string MemberName { get; set; }
        public string CategoryName { get; set; }
        public string ForumSubject { get; set; }
        public string TopicSubject { get; set; }
    }

    public partial class Member
    {
        public int PostCount { get { return (int) this.M_POSTS; } }

        public bool Status
        {
            get { return this.IsActive == 1; }
            set
            {
                if (value)
                    this.IsActive = 1;
                else
                    this.IsActive = 0;
            }
        }
        public bool EmailValided
        {
            get { return this.Validated ==1; }
            set
            {
                if (value)
                    this.Validated = 1;
                else
                    this.Validated = 0;
            }
        }
        public Rank Rank
        {
            get
            {
                string title = this.Title;
                return new Rank(this.Name, ref title, this.M_POSTS);
            }
        }

        public DateTime MemberSince { get { return this.Date.ToDateTime().Value; } }
        
        public string ProfileLink
        {
            get { return String.Format("/Account/profile.aspx?user={0}",this.Name); }
        }
        public string Avatar
        {
            get
            {
                return String.IsNullOrEmpty(this.M_AVATAR) ? "<img src='/Avatars/default.gif' alt='no avatar img' class='avatar' /><br />" : String.Format("<img src='/Avatars/{0}' alt='avatar img' class='avatar' /><br />", this.M_AVATAR);
            }
            set { this.M_AVATAR = value; }
        }
        public string AvatarUrl
        {
            get
            {
                if (String.IsNullOrEmpty(this.M_AVATAR))
                    return String.Format("{0}/Avatars/{1}", Common.GetSiteRoot(), "default.gif"); 
                return String.Format("{0}/Avatars/{1}", Common.GetSiteRoot(), this.M_AVATAR);
            }
        }

        public string Signature
        {
            get { return this.M_SIG; }
            set { this.M_SIG = value; }
        }
        public string ParsedSignature
        {
            get { return this.M_SIG.ParseTags(); }

        }
        public string Hobby
        {
            get { return this.Hobbies; }

        }
        public string News
        {
            get { return this.LatestNews; }

        }
        public string ParsedBiography
        {
            get { return this.Biography.ParseTags(); }

        }

        public DateTime? LastVisitDate { get { return this.M_LASTUPDATED.ToDateTime(); } }
        public DateTime? LastPostDate { get { return this.M_LASTPOSTDATE.ToDateTime(); } }
        public string LastUpdateTimeAgo
        {
            get
            {
                int offset = SnitzConfig.Config.TimeAdjust;
                Member current = Util.GetMember(HttpContext.Current.User.Identity.Name);
                if (current != null)
                    offset = current.TimeOffset;
                return String.IsNullOrEmpty(this.M_LASTUPDATED) ? "" : Common.TimeAgoTag(this.M_LASTUPDATED.ToDateTime().Value, HttpContext.Current.User.Identity.IsAuthenticated, offset);
            }
        }
        public string LastVisitTimeAgo
        {
            get
            {
                int offset = SnitzConfig.Config.TimeAdjust;
                Member current = Util.GetMember(HttpContext.Current.User.Identity.Name);
                if (current != null)
                    offset = current.TimeOffset;
                return String.IsNullOrEmpty(this.M_LASTUPDATED) ? "" : Common.TimeAgoTag(this.LastVisitDate.Value, HttpContext.Current.User.Identity.IsAuthenticated, offset);
            }
        }
        public string LastPostTimeAgo
        {
            get
            {
                int offset = SnitzConfig.Config.TimeAdjust;
                Member current = Util.GetMember(HttpContext.Current.User.Identity.Name);
                if (current != null)
                    offset = current.TimeOffset;
                return String.IsNullOrEmpty(this.M_LASTPOSTDATE) ? "" : Common.TimeAgoTag(this.LastPostDate.Value, HttpContext.Current.User.Identity.IsAuthenticated, offset);
            }
        }
        public string MemberSinceTimeAgo
        {
            get
            {
                int offset = SnitzConfig.Config.TimeAdjust;
                Member current = Util.GetMember(HttpContext.Current.User.Identity.Name);
                if (current != null)
                    offset = current.TimeOffset;
                return Common.TimeAgoTag(this.MemberSince, HttpContext.Current.User.Identity.IsAuthenticated, offset);
            }
        }

        public List<MemberSubscription> Subscriptions
        {
            get { var subs = Util.GetUserSubscriptions(this.Id);
                List<MemberSubscription> subscriptions = new List<MemberSubscription>();
                foreach (ForumSubscriptions forumSubscription in subs)
                {
                    Forum forum = Util.GetForum(forumSubscription.ForumId);
                    Category category = Util.GetCategory(forumSubscription.CategoryId);
                    MemberSubscription ms = new MemberSubscription();
                    ms.CategoryName = category.Name;
                    ms.CategoryId = category.Id;
                    ms.ForumSubject = String.Format("<a href='/Content/Forums/forum.aspx?FORUM={0}'>{1}</a>", forum.Id, forum.Subject);
                    ms.ForumId = forum.Id;
                    if(forumSubscription.TopicId != 0)
                    {
                        Topic topic = Util.GetTopic(forumSubscription.TopicId);
                        ms.Topicid = topic.Id;
                        ms.TopicSubject = String.Format("<a href='/Content/Forums/topic.aspx?TOPIC={0}'>{1}</a>", topic.Id, topic.Subject);
                    }
                    subscriptions.Add(ms);
                }
                return subscriptions;
            }
        }

        public int[] AllowedForums
        {
            get { return Util.AllowedForums(this).ToArray(); }
        }

        public bool IsSubscribedToTopic(int topicid)
        {
            return Util.IsUserSubscribedToTopic(this.Id, topicid);
        }

        public bool IsSubscribedToForum(int forumid)
        {
            return Util.IsUserSubscribedToForum(this.Id, forumid);
        }

        public bool IsTopicAuthor(Topic topic)
        {
            return this.Name == topic.Author.Name;
        }
    }
}
