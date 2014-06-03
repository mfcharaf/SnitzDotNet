/*
####################################################################################################################
##
## Snitz.Entities - ReplyInfo
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
using System.Text.RegularExpressions;


namespace Snitz.Entities
{
    public class ReplyInfo
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public int ForumId { get; set; }
        public int CatId { get; set; }

        public string Message { get; set; }
        public int Status { get; set; }
        public int AuthorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime? LastEditDate { get; set; }
        public int? LastEditedById { get; set; }
        public bool UseSignatures { get; set; }
        public string PosterIp { get; set; }

        public AuthorInfo Author { get; set; }
        public AuthorInfo LastEditedBy { get; set; }

        public string AuthorName { get; set; }
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
        public string AuthorProfileLink
        {
            get { return String.Format("/Account/profile.aspx?user={0}", this.AuthorName); }
        }
        public string EditorName { get; set; }

        public bool AuthorViewSig { get; set; }

        public string AuthorSignature { get; set; }

        public string CleanedMessage
        {
            get
            {
                string fstring = this.Message;
                fstring = Regex.Replace(fstring, @"\[code].*[/code]", "", RegexOptions.Singleline);
                return fstring;
            }
        }
    }
}
