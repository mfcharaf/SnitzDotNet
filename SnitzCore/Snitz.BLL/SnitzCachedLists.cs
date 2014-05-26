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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using Snitz.Entities;
using Snitz.Providers;
using SnitzConfig;

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

        public static List<ForumJumpto> GetCachedForumList(bool search)
        {
            List<ForumJumpto> fullforumlist;
            if (HttpContext.Current.Cache["forumjumplist"] != null)
            {
                fullforumlist = (List<ForumJumpto>)HttpContext.Current.Cache["forumjumplist"];
            }
            else
            {
                fullforumlist = Forums.ListForumJumpTo();
                if (search && Config.AllowSearchAllForums)
                {
                    fullforumlist.Insert(0, new ForumJumpto { Name = "[All Forums]", Id = -1, Category = "" });
                }else if (fullforumlist[0].Id != -1)
                {
                    
                        fullforumlist.Insert(0, new ForumJumpto { Name = "[Select Forum]", Id = -99, Category = "" });
                }
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

        public static List<FaqCategoryInfo> GetCachedHelpCategories()
        {
            List<FaqCategoryInfo> faqcatlist;
            if (HttpContext.Current.Cache["faqcatlist"] != null)
            {
                faqcatlist = (List<FaqCategoryInfo>)HttpContext.Current.Cache["faqcatlist"];
            }
            else
            {
                faqcatlist = SnitzFaq.GetFaqCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName).ToList();
                HttpContext.Current.Cache["faqcatlist"] = faqcatlist;
            }
            return faqcatlist; 

        }
        /// <summary>
        /// Method to populate a list with all the class
        /// in the namespace provided by the user
        /// </summary>
        /// <param name="nameSpace">The namespace the user wants searched</param>
        /// <returns></returns>
        public static List<string> GetAllClasses(string nameSpace)
        {
            //create an Assembly and use its GetExecutingAssembly Method
            //http://msdn2.microsoft.com/en-us/library/system.reflection.assembly.getexecutingassembly.aspx

            Assembly asm = Assembly.GetExecutingAssembly();

            //create a list for the namespaces
            //create a list that will hold all the classes
            //the suplied namespace is executing
            //loop through all the "Types" in the Assembly using
            //the GetType method:
            //http://msdn2.microsoft.com/en-us/library/system.reflection.assembly.gettypes.aspx

            List<string> namespaceList = (from type in asm.GetTypes() where type.Namespace == nameSpace select type.Name).ToList();

            //return the list
            return namespaceList.ToList();
        }
    }
}
