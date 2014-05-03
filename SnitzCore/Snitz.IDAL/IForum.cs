/*
####################################################################################################################
##
## Snitz.IDAL - IForum
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
    public interface IForum : IBaseObject<ForumInfo>
    {
        IEnumerable<ForumInfo> GetAll();
        IEnumerable<ForumInfo> GetByParent(int id);
        IEnumerable<int> AllowedRoles(int forumid);
        IEnumerable<string> Roles(int forumId);
        //IEnumerable<TopicInfo> GetTopics(int forumid);
        IEnumerable<TopicInfo> GetStickyTopics(int forumid);
        IEnumerable<ForumJumpto> ListForumJumpTo();

        void UpdateLastForumPost(object post);
        void SetForumStatus(int forumid, int status);
        void EmptyForum(int forumid);
        string[] GetForumRoles(int id);
    }
}
