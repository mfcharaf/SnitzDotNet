/*
####################################################################################################################
##
## SnitzMembership - SnitzLink
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		03/08/2013
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
    /// <summary>
    /// Favourites and Bookmark Url's
    /// </summary>
    [Serializable]
    public class SnitzLink : IEquatable<SnitzLink>

    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }


        public SnitzLink(){}

        public SnitzLink (string name, string url, int id)
        {
            ID = id;
            Name = name;
            Url = url;
        }

        public bool Equals(SnitzLink other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name) && this.Url.Equals(other.Url));

        }
    }
}
