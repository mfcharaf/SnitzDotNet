/*
####################################################################################################################
##
## Snitz.Entities - SearchParamInfo
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
    public class SearchParamInfo
    {
        public int ForumId { get; set; }

        public string Match { get; set; }

        public string AuthorPostType { get; set; }

        public string Author { get; set; }

        public string SearchFor { get; set; }

        public bool MessageAndSubject { get; set; }

        public int PageSize { get; set; }

        public DateTime BeforeDate { get; set; }

        public DateTime SinceDate { get; set; }

        public bool Archived { get; set; }

        public bool SubjectOnly { get; set; }
    }
}
