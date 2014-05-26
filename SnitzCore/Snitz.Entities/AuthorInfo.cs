/*
####################################################################################################################
##
## Snitz.Entities - AuthorInfo
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


using SnitzCommon;
using SnitzConfig;


namespace Snitz.Entities
{
    public class AuthorInfo : MemberInfo
    {
        public AuthorInfo(MemberInfo member)
        {
            string title = member.Title;

            this.Id = member.Id;
            this.Username = member.Username;
            this.AllowEmail = member.AllowEmail;
            this.Avatar = member.Avatar;
            this.Country = member.Country;
            this.Email = member.Email;
            this.HideEmail = member.HideEmail;
            this.HomePage = member.HomePage;
            this.MemberLevel = member.MemberLevel;
            this.PostCount = member.PostCount;
            this.Signature = member.Signature;
            this.ReceiveEmails = member.ReceiveEmails;
            this.Status = member.Status;
            this.Rank = new RankInfo(member.Username, ref title, member.PostCount);
            this.Title = title;
            this.Yahoo = member.Yahoo;
            this.AIM = member.AIM;
            this.Skype = member.Skype;
            this.ICQ = member.ICQ;
            //this.FaceBook = member.FaceBook;
            //this.GooglePlus = member.GooglePlus;
        }

        public string ProfilePopup
        {
            get
            {
                return
                    string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
                        this.Id, this.Username, this.Username);
            }
        }
        public string ProfilePopup2(string title)
        {

            return
                string.Format(
                    "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"{1}\">{2}</a>",
                    this.Id, string.Format(title, this.Username), this.Username);

        }

        public string UserLevel { get { return EnumHelper.GetDescription((Enumerators.UserLevels)this.MemberLevel); } }

    }
}