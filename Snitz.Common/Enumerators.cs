using System;
using System.ComponentModel;
using System.Reflection;


namespace SnitzCommon
{
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description of the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
    
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

    }
}
