/*
####################################################################################################################
##
## SnitzConfig - Enumerators
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

using System.ComponentModel;


namespace SnitzConfig
{
    public static class Enumerators
    {
        /// <summary>
        /// User level enumerators
        /// </summary>
        public enum UserLevels
        {
            [Description("Forum Member")]
            NormalUser = 1,
            [Description("Forum Moderator")]
            Moderator = 2,
            [Description("Forum Administrator")]
            Administrator = 3
        }

        /// <summary>
        /// Status enumerator for Posts
        /// </summary>
        public enum PostStatus
        {
            [Description("Locked")]
            Closed = 0,
            [Description("Open")]
            Open = 1,
            [Description("Unmoderated Post")]
            UnModerated = 2,
            [Description("Post on Hold")]
            OnHold = 3
        }

        /// <summary>
        /// Forum moderation enumerators
        /// </summary>
        public enum Moderation
        {
            [Description("No Moderation")]
            UnModerated = 0,
            [Description("All Posts Moderated")]
            AllPosts,
            [Description("Original Posts Only Moderated")]
            Topics,
            [Description("Replies Only Moderated")]
            Replies
        }

        /// <summary>
        /// Allowed Subscription level
        /// </summary>
        public enum Subscription
        {
            [Description("No Subscriptions Allowed")]
            None = 0,
            [Description("Forum Subscriptions Allowed")]
            ForumSubscription,
            [Description("Topic Subscriptions Allowed")]
            TopicSubscription
        }

        /// <summary>
        /// Event type enumerators
        /// </summary>
        public enum EventType
        {
            [Description("Normal Event")]
            Normal = 1,
            [Description("Birthday")]
            Birthday = 2,
            [Description("Anniversary")]
            Anniversary = 3,
            [Description("Holiday")]
            Holiday = 4,
            [Description("Special")]
            Special = 5

        }

        /// <summary>
        /// Forum authorisation enumerators
        /// </summary>
        public enum ForumAuthType
        {
            [Description("All Visitors")]
            AllVisitors = 0,
            [Description("Allowed Member List")]
            AllowedMembers,
            [Description("Password Protected")]
            Password,
            [Description("Allowed Member List & Password Protected")]
            AllowedMemberPassword,
            [Description("Members Only")]
            Members,
            [Description("Members Only (Hidden)")]
            MembersHidden,
            [Description("Allowed Member List (Hidden)")]
            AllowedMemberHidden,
            [Description("Members Only & Password Protected")]
            MembersPassword
        }

        /// <summary>
        /// Grid pager display type
        /// </summary>
        public enum PagerType
        {
            [Description("Pager With DropdownList")]
            Dropdown,
            [Description("Pager with Buttons")]
            Button,
            [Description("Pager using Hyperlinks")]
            Text,
            [Description("Pager using LinkButtons")]
            Linkbutton
        };

        /// <summary>
        /// Forum Subscription Level
        /// </summary>
        public enum SubscriptionLevel
        {
            [Description("No Subscriptions")]
            None = 0,
            [Description("Subscribe to Whole Board")]
            Board,
            [Description("Subscribe by Category")]
            Category,
            [Description("Subscribe by Forum")]
            Forum,
            [Description("Subscribe by Topic")]
            Topic
        }

        /// <summary>
        /// Forum Rank display type
        /// </summary>
        public enum RankType
        {
            None = 0,
            RankOnly,
            StarsOnly,
            Both
        }
    }
}
