/*
####################################################################################################################
##
## Snitz.Entities - PrivateMessages
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
    public class PrivateMessageInfo
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public int FromMemberId { get; set; }

        public int ToMemberId { get; set; }

        public string SentDate { get; set; }

        public string Message { get; set; }

        public string Count { get; set; }

        public int Read { get; set; }

        public int Mail { get; set; }

        public int OutBox { get; set; }

        public MemberInfo ToMember { get; set; }

        public MemberInfo FromMember { get; set; }

        public string ToMemberName { get; set; }

        public string FromMemberName { get; set; }

        public DateTime? Sent { get; set; }
    }
}
