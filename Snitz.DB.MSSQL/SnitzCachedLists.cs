using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Snitz.Providers;
using SnitzData;

/// <summary>
/// SnitzLists - Cached lists
/// </summary>
 
[DataObject(true)]
public class SnitzCachedLists
{
    public static List<WordFilter> GetBadWords()
    {
        List<WordFilter> badWords;

        List<WordFilter> cWords = (List<WordFilter>)HttpContext.Current.Cache["badwords"];

        if (cWords != null)
            badWords = cWords;
        else
        {
            badWords = Util.ListBadWords();
            HttpContext.Current.Cache["badwords"] = badWords;
        }
        return badWords;
    }
    public static IEnumerable<NameFilter> GetNameList()
    {
        List<NameFilter> names;

        var cNames = (List<NameFilter>)HttpContext.Current.Cache["namefilter"];
        if (cNames != null)
            names = cNames;
        else
        {
            names = Util.ListNameFilters();
            HttpContext.Current.Cache["namefilter"] = names;
        }
        return names;
    }
    public static List<ForumJumpto> GetForumListItems()
    {
        List<ForumJumpto> fullforumlist;
        if (HttpContext.Current.Cache["forumjumplist"] != null)
        {
            fullforumlist = (List<ForumJumpto>)HttpContext.Current.Cache["forumjumplist"];
        }
        else
        {
            fullforumlist = Util.ListForumJumpTo();
            if(fullforumlist[0].Id != -1)
                fullforumlist.Insert(0, new ForumJumpto{Name = "[Select Forum]",Id = -1,Category = ""});
            HttpContext.Current.Cache["forumjumplist"] = fullforumlist;
        }
        return fullforumlist;
    }

    public static Dictionary<int,string> UserRoles()
    {
        Dictionary<int,string> result = new Dictionary<int, string>();

        if (HttpContext.Current.Session["RoleList"] != null)
        {
            result = HttpContext.Current.Session["RoleList"] as Dictionary<int, string>;
        }
        else
        {
            result = new SnitzRoleProvider().ListAllRolesForUser(HttpContext.Current.User.Identity.Name);
            if(result.Count > 0)
                HttpContext.Current.Session.Add("RoleList", result);
        }

        return result;
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
