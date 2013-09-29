/*
####################################################################################################################
##
## Snitz.Entities - ForumInfo
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
using System.Collections.Generic;

namespace Snitz.Entities
{
    [Serializable]
    public class ForumJumpto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public List<string> Roles { get; set; }
    }
    /// <summary>
    /// Business entity used to model a Forum
    /// </summary>
    [Serializable]
    public class ForumInfo
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int Status { get; set; }
        public string Url { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int TopicCount { get; set; }
        public int PostCount { get; set; }
        public DateTime? LastPostDate { get; set; }
        public int? LastPostAuthorId { get; set; }
        public string Password { get; set; }
        public int? AuthType { get; set; }
        public int Type { get; set; }
        public int? ModerationLevel { get; set; }
        public int? SubscriptionLevel { get; set; }
        public int Order { get; set; }
        public bool AllowPolls { get; set; }
        public int? LastPostTopicId { get; set; }
        public int? LastPostReplyId { get; set; }
        public bool? UpdatePostCount { get; set; }
        public int? ArchivedTopicCount { get; set; }
        public int? ArchivedPostCount { get; set; }

        public AuthorInfo LastPostAuthor { get; set; }
        public CategoryInfo CategoryInfo { get; set; }
        public AuthorInfo LastPostAuthorInfo { get; set; }
        public List<string> Roles { get; set; }

        public ForumInfo(int forumId, string subject, string description)
        {
            this.Id = forumId;
            this.Subject = subject;
            this.Description = description;
        }

        public ForumInfo()
        {
        }

        public List<TopicInfo> StickyTopics { get; set; }

        public List<int> AllowedRoles { get; set; }

        public string LastPostSubject { get; set; }

        public string LastPostAuthorName { get; set; }
        public string LastPostAuthorPopup
        {
            get
            {
                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
                        this.LastPostAuthorId, this.LastPostAuthorName, this.LastPostAuthorName);
            }
        }

        public DateTime? LastArchived { get; set; }
        public int? ArchiveFrequency { get; set; }

        public int PrivateForum { get; set; }
    }
}