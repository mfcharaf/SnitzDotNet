using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snitz.Providers;
using SnitzCommon;

namespace SnitzData
{
    public class ForumJumpto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public List<string> Roles
        {
            get
            {
                return SnitzRoleProvider.GetForumRoles(this.Id).ToList();
            }
        }
    }

    public partial class Forum
    {
        //public Category Category { get { return Util.GetCategory(this.CatId); } }
        public List<int> AllowedRoles { get { return new SnitzDataClassesDataContext().GetAllowedRoles(this.Id); } }
        public Enumerators.PostStatus Status
        {
            get { return (Enumerators.PostStatus)this.F_STATUS; }
            set { this.F_STATUS = (short) value; }
        }
        public Enumerators.Moderation ModerationLevel
        {
            get { return (Enumerators.Moderation)this.F_MODERATION; }
            set { this.F_MODERATION = (int) value; }
        }
        public DateTime? LastPostDate
        {
            get {
                return string.IsNullOrEmpty(this.F_LAST_POST)
                           ? (DateTime?) null
                           : this.F_LAST_POST.ToDateTime();
            }
        }
        public int[] Subscribers
        {
            get { return Util.GetForumSubscriptions(this.Id); }
        }
        public string LastPostSubject
        {
            get {
                if (this.LastTopicId == null)
                    return String.Empty;
                Topic topic = new SnitzDataClassesDataContext().GetTopic((int) this.LastTopicId);
                if (topic == null || topic.Subject == null)
                    return String.Empty;
                return topic.Subject;
            }
        }
        public List<Topic> StickyTopics { get { return GetStickyTopics(this.Id); } }
        public List<string> Roles {
            get
            {
                return SnitzRoleProvider.GetForumRoles(this.Id).ToList();
            }
        }
        public string ParsedSubject
        {
            get
            {
                string modStr = "";
                if(this.ModerationLevel != Enumerators.Moderation.UnModerated)
                    modStr = String.Format(" ({0})", EnumHelper.GetDescription(this.ModerationLevel));
                return this.Subject.ReplaceNoParseTags().ReplaceBadWords().ReplaceSmileTags().ReplaceTags() + modStr;
            }
        }
        public IEnumerable<Topic> Topics { get { return GetTopics(this.Id); } }
        public Member LastPostAuthor
        {
            get
            {
                return new SnitzDataClassesDataContext().GetAuthor(this.F_LAST_POST_AUTHOR);
            }
        }
        public string LastPostSubjectAbbreviated
        {
            get
            {
                return LastPostSubject.Length > 30 ? LastPostSubject.Substring(0, 31) + "..." : LastPostSubject;
            }
        }
        public int? LastPostAuthorId { get
        {
            if (this.F_LAST_POST_AUTHOR == null)
                return 0;
            return this.F_LAST_POST_AUTHOR;
        } }
        public string LastPostAuthorName { get
        {
            if (LastPostAuthor == null)
                return "";
            return LastPostAuthor.Name;
        } }
        private static IEnumerable<Topic> GetTopics(int forumid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            return (from topic in dc.Topics where topic.ForumId == forumid && !topic.IsSticky orderby topic.T_LAST_POST descending select topic).ToList();
        }
        private static List<Topic> GetStickyTopics(int forumid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            return (from topic in dc.Topics where topic.ForumId == forumid && topic.IsSticky select topic).ToList();
        }

        public Dictionary<int,string> Moderators
        {
            get { return new LookupsDataContext().GetForumModerators(this.Id); }
        }

        public Enumerators.ForumAuthType AuthType
        {
            get { return (Enumerators.ForumAuthType) this.F_PRIVATEFORUMS; }
            set { this.F_PRIVATEFORUMS = (int) value; }
        }

        public void AddModerator(string moderator)
        {
            ForumModerator fm = new ForumModerator();
            fm.ForumId = this.Id;
            fm.ModeratorId = Util.GetMember(moderator).Id;
            using (LookupsDataContext dc = new LookupsDataContext())
            {
                dc.ForumModerators.InsertOnSubmit(fm);
                dc.SubmitChanges();
            }
        }
    }

}
