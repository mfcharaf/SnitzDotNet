/*
####################################################################################################################
##
## Snitz.Entities - TopicInfo
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System;
using SnitzConfig;

namespace Snitz.Entities
{
    public class TopicInfo
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int CatId { get; set; }

        public string Subject { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int AuthorId { get; set; }
        public int ReplyCount { get; set; }
        public int Views { get; set; }
        public DateTime Date { get; set; }
        public DateTime? LastPostDate { get; set; }
        public DateTime? LastEditDate { get; set; }
        public int? LastPostAuthorId { get; set; }
        public int? LastEditedById { get; set; }
        public int? LastReplyId { get; set; }
        public string PosterIp { get; set; }
        public bool IsSticky { get; set; }
        public bool UseSignatures { get; set; }
        public int UnModeratedReplies { get; set; }
        public int? PollId { get; set; }
        public bool IsArchived { get; set; }

        public CategoryInfo CategoryInfo { get; set; }
        public ForumInfo Forum { get; set; }
        public AuthorInfo Author { get; set; }
        public AuthorInfo LastEditedBy { get; set; }

        public AuthorInfo LastPostAuthor { get; set; }

        public bool AllowSubscriptions
        {
            get
            {
                if (this.Forum.SubscriptionLevel == (int)Enumerators.Subscription.TopicSubscription)
                    return true;
                return false;
            }
        }

        public int PageCount
        {
            get
            {
                int extra = 0;
                if (this.ReplyCount % Config.TopicPageSize > 0)
                    extra = 1;
                if (this.ReplyCount == 0)
                    extra = 1;
                return (this.ReplyCount / Config.TopicPageSize) + extra;
            }
        }

        public string LastPostAuthorName { get; set; }
        public string LastPostAuthorPopup
        {
            get
            {
                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"{1}\">{2}</a>",
                        this.LastPostAuthorId, this.LastPostAuthorName, this.LastPostAuthorName);
            }
        }
        public string AuthorName { get; set; }
        public string AuthorProfileLink
        {
            get { return String.Format("/Account/profile.aspx?user={0}", this.AuthorName); }
        }
        public string AuthorProfilePopup
        {
            get
            {
                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
                        this.AuthorId, this.AuthorName, this.AuthorName);
            }
        }
        public string EditorName { get; set; }

        public bool AuthorViewSig { get; set; }

        public string AuthorSignature { get; set; }
    }
}
