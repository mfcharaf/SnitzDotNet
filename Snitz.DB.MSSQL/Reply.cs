using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SnitzCommon;

namespace SnitzData
{
    public partial class Reply
    {
        public Enumerators.PostStatus Status
        {
            get { return (Enumerators.PostStatus) this.R_STATUS; }
            set { this.R_STATUS = (short)value; }
        }

        public string ParsedMessage
        {
            get
            {
                return
                    this.Message.ReplaceNoParseTags().ReplaceBadWords().ReplaceSmileTags().ReplaceImageTags().ReplaceURLs().ReplaceTableTags().
                        ReplaceVideoTag().ReplaceFileTags().ReplaceCodeTags(this.Id, "R").ReplaceTags();
            }
        }
        public string CleanedMessage
        {
            get
            {
                string fstring = this.Message;
                fstring = Regex.Replace(fstring, @"\[code].*[/code]", "", RegexOptions.Singleline);
                return fstring;
            }
        }
        public DateTime? LastEditDate
        {
            get
            {
                return string.IsNullOrEmpty(this.R_LAST_EDIT)
                  ? (DateTime?)null : this.R_LAST_EDIT.ToDateTime();
            }
        }
        public DateTime Date
        {
            get { return this.R_DATE.ToDateTime().Value; }
            set { this.R_DATE = value.ToForumDateStr(); }
        }

        public Member Author
        {
            get { return new SnitzDataClassesDataContext().GetAuthor(this.R_AUTHOR); }
        }
        public Member LastEditBy
        {
            get { return new SnitzDataClassesDataContext().GetAuthor(this.R_LAST_EDITBY); }
        }
        public string LastEditTimeAgo
        {

            //return string.Format("<abbr class='timeago' title='{0}'>{1}</abbr>", date.ToISO8601Date(authenticated, timediff), date.ToForumDateDisplay(" ", true, authenticated, timediff));
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
        public string DateTimeAgo
        {

            get
            {
                Member currentuser = Util.GetMember(HttpContext.Current.User.Identity.Name);
                    return String.Format("<abbr class='timeago' title='{0}'>{1}</abbr>",
                                         this.Date.ToISO8601Date(
                                             HttpContext.Current.User.Identity.IsAuthenticated, currentuser.TimeOffset),
                                         this.Date.ToForumDateDisplay(" ", true,
                                                                                    HttpContext.Current.User.Identity.
                                                                                        IsAuthenticated,
                                                                                    currentuser.TimeOffset));
            }
        }

        public void Delete()
        {
            Util.DeleteReply(this.Id);
        }
    }
}
