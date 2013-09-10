/*
####################################################################################################################
##
## Snitz.Entities - FaqInfo
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
    public class FaqCategoryInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int Order { get; set; }
    }

    public class FaqInfo
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public string Link { get; set; }
        public string LinkTitle { get; set; }
        public string LinkBody { get; set; }
        public string Language { get; set; }
        public int Order { get; set; }
    }
}
