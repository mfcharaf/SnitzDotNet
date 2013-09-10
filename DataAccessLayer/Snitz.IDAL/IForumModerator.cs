/*
####################################################################################################################
##
## Snitz.IDAL - IForumModerator
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

using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.IDAL
{
    public interface IForumModerator : IBaseObject<ForumModeratorInfo>
    {
        /// <summary>
        /// Fetches all the records, unpaged
        /// </summary>
        /// <returns>An enumerable list of objects</returns>
        IEnumerable<ForumModeratorInfo> GetAll();
        IEnumerable<ForumModeratorInfo> GetByParent(int forumid);
        List<ForumInfo> GetUnModeratedForums(int memberId);
        List<ForumInfo> GetModeratedForums(int memberId);
        string[] GetForumRoles(int forumid);
        bool IsUserForumModerator(int id, int forumId);
    }
}
