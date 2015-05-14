/*
####################################################################################################################
##
## Snitz.Entities - MemberInfo
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
using System.IO;
using System.Web;
using SnitzConfig;


namespace Snitz.Entities
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Registration : System.Attribute
    {
        public bool Display;
        public string Group;
        public string Control;

        public Registration(bool display,string group,string control)
        {
            Display = display;
            Group = group;
            Control = control;
        }
    }

    public class MemberInfo
    {
        //private RankInfo _rank;
        public int Id { get; set; }
        [Registration(true, "Member", "Text")]
        public string Username { get; set; }
        public string NTUsername { get; set; }
        [Registration(true, "Member", "Text")]
        public string Password { get; set; }
        public string Title { get; set; }
        [Registration(true, "Member", "Text")]
        public string Email { get; set; }
        [Registration(true, "Settings", "Text")]
        public string Signature { get; set; }
        
        public string MembersIP { get; set; }
        public string LastIP { get; set; }

        public bool AllowEmail { get; set; }
        [Registration(true, "Settings", "CheckBox")]
        public bool HideEmail { get; set; }
        [Registration(true, "Settings", "CheckBox")]
        public bool ReceiveEmails { get; set; }
        
        public int MemberLevel { get; set; }
        public int PostCount { get; set; }
        public int Status { get; set; }
        public bool boolStatus { get { return this.Status ==1; } }
        public bool IsValid { get; set; }
        

        #region personal data
        [Registration(true, "Member", "Text")]
        public string Firstname { get; set; }
        [Registration(true, "Member", "Text")]
        public string Lastname { get; set; }
        public string Age { get; set; }
        [Registration(true, "Member", "GenderLookup")]
        public string Gender { get; set; }
        [Registration(true, "Member", "DatePicker")]
        public string DateOfBirth { get; set; }
        [Registration(true, "Member:Info", "Lookup")]
        public string MaritalStatus { get; set; }
        [Registration(true, "Member:Info", "Text")]
        public string City { get; set; }
        [Registration(true, "Member:Info", "Text")]
        public string State { get; set; }
        [Registration(true, "Member:Info", "CountryLookup")]
        public string Country { get; set; }
        [Registration(true, "Member:Other", "Text")]
        public string Occupation { get; set; }
        #endregion

        #region Dates
        public double TimeOffset { get; set; }
        [Registration(true, "Settings", "TimeZoneLookup")]
        public string TimeZone { get; set; }
        [Registration(true, "Settings", "CheckBox")]
        public bool UseDaylightSaving { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public DateTime? LastPostDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        #endregion

        #region Profile data
        public string Avatar { get; set; }
        [Registration(true, "Member:Social Media", "Text")]
        public string AIM { get; set; }
        [Registration(true, "Member:Social Media", "Text")]
        public string Yahoo { get; set; }
        [Registration(true, "Member:Social Media", "Text")]
        public string ICQ { get; set; }
        [Registration(true, "Member:Social Media", "Text")]
        public string Skype { get; set; }

        [Registration(true, "Member:Other", "LongText")]
        public string Biography { get; set; }
        [Registration(true, "Member:Other", "Text")]
        public string HomePage { get; set; }
        [Registration(true, "Member:Other", "LongText")]
        public string Hobbies { get; set; }
        [Registration(true, "Member:Other", "LongText")]
        public string LatestNews { get; set; }
        [Registration(true, "Member:Other", "LongText")]
        public string FavouriteQuote { get; set; }
        
        public string PhotoLink { get; set; }
        public string FavLink1 { get; set; }
        public string FavLink2 { get; set; }
        public ProfileInfo ProfileData { get; set; }
        #endregion

        #region Forum Options
        [Registration(true, "Settings", "CheckBox")]
        public bool ViewSignatures { get; set; }
        [Registration(true, "Settings", "CheckBox")]
        public bool UseSignature { get; set; }
        public string Theme { get; set; }

        public bool AllowSubscriptions { get; set; }
        public int DefaultView { get; set; }
        public bool Voted { get; set; }
        public string PasswordChangeKey { get; set; }
        public string ValidationKey { get; set; }
        public string NewEmail { get; set; }

        public string AvatarImg
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Avatar) && !File.Exists(HttpRuntime.AppDomainAppPath + "/Avatars/" + this.Avatar))
                    this.Avatar = "missing.gif";
                return String.IsNullOrEmpty(this.Avatar) ? "<img src='/Avatars/default.gif' alt='no avatar img' class='avatar' />" : String.Format("<img src='/Avatars/{0}' alt='avatar img' class='avatar' />", this.Avatar);
            }
            set { this.Avatar = value; }
        }
        public string AvatarUrl
        {
            get
            {
                if (!String.IsNullOrEmpty(this.Avatar) && !File.Exists(HttpRuntime.AppDomainAppPath + "/Avatars/" + this.Avatar))
                    this.Avatar = "missing.gif";
                return String.IsNullOrEmpty(this.Avatar) ? String.Format("{0}Avatars/default.gif",Config.ForumUrl) : String.Format("{0}Avatars/{1}",Config.ForumUrl, this.Avatar);
            }

        }
        #endregion

        public int[] AllowedForums { get; set; }

        public string ParsedSignature { get; set; }

        public string ProfileLink
        {
            get { return String.Format("/Account/profile.aspx?user={0}", this.Username); }
        }

    }

}