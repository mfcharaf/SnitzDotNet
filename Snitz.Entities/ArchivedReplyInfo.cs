/*
####################################################################################################################
##
## Snitz.Entities - ArchivedReplyInfo
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

namespace Snitz.Entities
{
    public class ArchivedReplyInfo
    {
        public int ReplyId { get; protected set; }
        public int TopicId { get; set; }
        public int ForumId { get; set; }
        public int CatId { get; set; }

        public string Message { get; set; }
        public int Status { get; set; }
        public int AuthorId { get; set; }
        public string CreatedDate { get; set; }
        public string LastEditDate { get; set; }
        public int LastEditedById { get; set; }
        public int ShowSignature { get; set; }
        public string PosterIp { get; set; }


        public CategoryInfo CategoryInfo { get; set; }
        public ForumInfo Forum { get; set; }
        public ArchivedTopicInfo TopicInfo { get; set; }
        public AuthorInfo AuthorInfo { get; set; }
        public AuthorInfo LastEditedBy { get; set; }
    }
}