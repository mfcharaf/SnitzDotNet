
namespace SnitzCommon.Account
{
    namespace Devarchive_Net.Navigation
    {
        public static class Pages
        {
            public static class Controls
            {

                public static class Common
                {
                    public const string Confirmation = "~/usercontrols/popups/confirmation.ascx";
                    public const string EmailTopic = "~/usercontrols/popups/emailtopic.ascx";
                    public const string EmailMember = "~/usercontrols/popups/emailMember.ascx";
                    public const string ViewEvent = "~/usercontrols/popups/ViewEvent.ascx";
                    public const string BugReport = "~/usercontrols/popups/BugReport.ascx";
                }
                public static class Modules
                {
                    public static class Admin
                    {
                        public const string EditForum = "~/usercontrols/popups/forumproperties.ascx";
                        public const string EditCategory = "~/usercontrols/popups/categoryproperties.ascx";
                        public const string SplitTopic = "~/usercontrols/popups/splittopic.ascx";
                        public const string Moderate = "~/usercontrols/popups/moderatePreview.ascx";
                        public const string SetOrder = "~/usercontrols/popups/OrderForums.ascx";
                    }

                    public static class Profile
                    {
                        public const string SendPrivateMessage = "~/usercontrols/privatemessages/pmsend.ascx";
                        public const string SnitzProfile = "~/usercontrols/popups/UserProfile.ascx";
                        public const string IPLookup = "~/usercontrols/popups/IPLookup.ascx";
                        public const string RetrievePassword = "~/usercontrols/popups/PasswordRetrieve.ascx";
                    }
                }
            }
        }

    }
}