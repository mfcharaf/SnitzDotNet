/*
####################################################################################################################
##
## Snitz.Entities - StatsInfo
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		01/08/2013
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
    public class StatsInfo
    {
        public string NewestMember { get; set; }

        public int MemberCount { get; set; }

        public int TotalPostCount { get; set; }

        public int ActiveTopicCount { get; set; }

        public TopicInfo LastPost { get; set; }

        public AuthorInfo LastPostAuthor { get; set; }

        public int TopicCount { get; set; }

        public int ArchiveTopicCount { get; set; }

        public int ArchiveReplyCount { get; set; }

        public int ActiveMembers { get; set; }
    }
}
