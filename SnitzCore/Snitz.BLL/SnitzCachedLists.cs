/*
####################################################################################################################
##
## Snitz.BLL - SnitzCachedLists
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		02/08/2013
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
using System.Web;
using Snitz.Entities;
using Snitz.Providers;

namespace Snitz.BLL
{
    public static class SnitzCachedLists
    {
        public static Dictionary<int, string> UserRoles()
        {
            Dictionary<int, string> result = new Dictionary<int, string>();
            if (HttpContext.Current.Session == null)
                return result;

            if (HttpContext.Current.Session["RoleList"] != null)
            {
                result = HttpContext.Current.Session["RoleList"] as Dictionary<int, string>;
            }
            else
            {
                result = new SnitzRoleProvider().ListAllRolesForUser(HttpContext.Current.User.Identity.Name);
                if (result.Count > 0)
                    HttpContext.Current.Session.Add("RoleList", result);
            }

            return result;
        }

        public static List<ForumJumpto> GetCachedForumList()
        {
            List<ForumJumpto> fullforumlist;
            if (HttpContext.Current.Cache["forumjumplist"] != null)
            {
                fullforumlist = (List<ForumJumpto>)HttpContext.Current.Cache["forumjumplist"];
            }
            else
            {
                fullforumlist = Forums.ListForumJumpTo();
                if (fullforumlist[0].Id != -1)
                    fullforumlist.Insert(0, new ForumJumpto { Name = "[Select Forum]", Id = -1, Category = "" });
                HttpContext.Current.Cache["forumjumplist"] = fullforumlist;
            }
            return fullforumlist;
        }

        public static List<BadwordInfo> GetCachedBadWordList()
        {
            List<BadwordInfo> badwordlist;
            if (HttpContext.Current.Cache["badwordlist"] != null)
            {
                badwordlist = (List<BadwordInfo>)HttpContext.Current.Cache["badwordlist"];
            }
            else
            {
                badwordlist = Filters.GetAllBadwords();
                HttpContext.Current.Cache["badwordlist"] = badwordlist;
            }
            return badwordlist;            
        }
    }
}
