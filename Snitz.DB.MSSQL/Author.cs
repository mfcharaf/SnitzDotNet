using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnitzCommon;
using SnitzMembership;

namespace SnitzData
{
    public partial class Member
    {
        public string LinkTarget
        {
            get { ProfileCommon prof = ProfileCommon.GetUserProfile(this.Name);
            if (String.IsNullOrEmpty(prof.LinkTarget))
                return "_blank";
                return prof.LinkTarget;
            }
        }

        public string ProfilePopup
        {
            get
            {
                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
                        this.Id, this.Name, this.Name);
            }
        }

        public string UserName { get; set; }

        public string UserLevel { get { return EnumHelper.GetDescription((Enumerators.UserLevels) this.M_LEVEL); } }

        public bool ReceiveEmails
        {
            get { return this.ReceiveEmail == 1; }
            set { throw new NotImplementedException(); }
        }

        public bool ViewSignatures
        {
            get { return this.ViewSig == 1; }
            set { throw new NotImplementedException(); }
        }

        public static List<Topic> GetAuthoredTopics(int memberid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            return (from topic in dc.Topics where topic.T_AUTHOR == memberid select topic).ToList();
        }

        public static List<Topic> GetTopicsRepliedTo(int memberid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var topicsreplied = (from topic in dc.Topics
                    join reply in dc.Replies on topic.Id equals reply.TopicId
                    where reply.Author.Id == memberid
                    orderby topic.T_LAST_POST descending
                    select topic).Distinct().Take(10);
            return (List<Topic>) topicsreplied;
        }

        public string ProfilePopup2( string title)
        {

                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"{1}\">{2}</a>",
                        this.Id, string.Format(title,this.Name), this.Name);

        }
    }
}
