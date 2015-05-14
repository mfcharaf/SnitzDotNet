/*
####################################################################################################################
##
## Snitz.BLL - Categories
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
using System.Linq;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Categories
    {
        public static IEnumerable<ForumInfo> GetCategoryForums(int categoryid, MemberInfo member)
        {
            IForum dal = Factory<IForum>.Create("Forum");
            var allowedforums = Forums.AllowedForums(member);
            if (member == null)
            {
                allowedforums = Forums.ViewableForums();
            }
            return dal.GetByParent(categoryid).Where(catforum => allowedforums.Contains(catforum.Id));
        }

        public static void UpdateOrder(Dictionary<int, int> catlist)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.UpdateCategoryOrder(catlist);
        }
        public static CategoryInfo GetCategory(int categoryid)
        {
            ICategory dal = Factory<ICategory>.Create("Category");

            // Run a search against the data store
            return dal.GetById(categoryid);
        }

        public static List<CategoryInfo> GetCategories()
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            return new List<CategoryInfo>(dal.GetAll());
        }

        public static IEnumerable<CategoryInfo> GetCategoryByName(string name)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            return dal.GetByName(name);
        }

        public static int AddCategory(CategoryInfo category)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            return dal.Add(category);
        }

        public static void DeleteCategory(CategoryInfo category)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.Delete(category);
        }

        public static void UpdateCategory(CategoryInfo category)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.Update(category);
        }

        public static void Dispose()
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.Dispose();
        }

        public static void DeleteCategory(int catid)
        {
            CategoryInfo cat = GetCategory(catid);
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.Delete(cat);
        }

        public static void SetCatStatus(int catid, int status)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            dal.SetStatus(catid, status);
        }

        public static bool CategoryHasPosts(int catid)
        {
            ICategory dal = Factory<ICategory>.Create("Category");
            return dal.HasPosts(catid);
        }
    }
}
