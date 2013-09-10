/*
####################################################################################################################
##
## Snitz.BLL - Groups
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
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Groups
    {
        public static void AddGroupCategory(int groupid,int categoryid)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            dal.AddCategory(groupid,categoryid);
        }

        public static IEnumerable<CategoryInfo> GetGroupCategories(int groupid, int startrec, int maxrecs)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            return dal.GetByParent(groupid);
        }

        public static GroupInfo GetGroup(int groupid)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            return dal.GetById(groupid);
        }

        public static IEnumerable<GroupInfo> GetGroup(string groupname)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            return dal.GetByName(groupname);
        }

        public static int AddGroup(GroupInfo group)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            return dal.Add(group);
        }

        public static void DeleteGroup(GroupInfo group)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            dal.Delete(group);
        }

        public static void UpdateGroup(GroupInfo group)
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            dal.Update(group);
        }

        public static void Dispose()
        {
            IGroup dal = Factory<IGroup>.Create("Group");
            dal.Dispose();
        }

    }
}
