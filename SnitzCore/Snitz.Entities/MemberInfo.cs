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


namespace Snitz.Entities
{
    public class MemberInfo
    {
        private RankInfo _rank;
        public int Id { get; set; }
        public string Username { get; set; }
        public string NTUsername { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Signature { get; set; }
        public string MembersIP { get; set; }
        public string LastIP { get; set; }

        public bool AllowEmail { get; set; }
        public bool HideEmail { get; set; }
        public bool ReceiveEmails { get; set; }
        public int MemberLevel { get; set; }
        public int PostCount { get; set; }
        public int Status { get; set; }
        public bool boolStatus { get { return this.Status ==1; } }
        public bool IsValid { get; set; }
        

        #region personal data
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string MaritalStatus { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Occupation { get; set; }
        #endregion

        #region Dates
        public int TimeOffset { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public DateTime? LastPostDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        #endregion

        #region Profile data
        public string Avatar { get; set; }
        public string AIM { get; set; }
        public string Yahoo { get; set; }
        public string ICQ { get; set; }
        public string Skype { get; set; }
        public string Biography { get; set; }
        public string HomePage { get; set; }
        public string Hobbies { get; set; }
        public string LatestNews { get; set; }
        public string FavouriteQuote { get; set; }
        public string PhotoLink { get; set; }
        public string FavLink1 { get; set; }
        public string FavLink2 { get; set; }
        public ProfileInfo ProfileData { get; set; }
        #endregion

        #region Forum Options
        public bool AllowSubscriptions { get; set; }
        public int DefaultView { get; set; }
        public bool ViewSignatures { get; set; }
        public bool UseSignature { get; set; }
        public bool Voted { get; set; }
        public string Theme { get; set; }

        public string PasswordChangeKey { get; set; }
        public string ValidationKey { get; set; }
        public string NewEmail { get; set; }

        public string AvatarUrl
        {
            get
            {
                return String.IsNullOrEmpty(this.Avatar) ? "<img src='/Avatars/default.gif' alt='no avatar img' class='avatar' /><br />" : String.Format("<img src='/Avatars/{0}' alt='avatar img' class='avatar' /><br />", this.Avatar);
            }
            set { this.Avatar = value; }
        }

        #endregion

        public RankInfo Rank
        {
            get
            {
                string title = this.Title;
                if(_rank == null)
                    _rank = new RankInfo(this.Username, ref title, this.PostCount);
                this.Title = title;
                return _rank;
            }
            set
            {
                if(_rank == null || !_rank.Equals(value))
                _rank = value;
            }
        }


        public int[] AllowedForums { get; set; }

        public string ParsedSignature { get; set; }

        public string ProfileLink
        {
            get { return String.Format("/Account/profile.aspx?user={0}", this.Username); }
        }

    }

}