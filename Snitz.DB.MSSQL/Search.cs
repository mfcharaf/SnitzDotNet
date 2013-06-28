using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnitzConfig;
using SnitzData.Filters;

namespace SnitzData
{
    public static class Search
    {
        /// <summary>
        /// Gets a List of Topics according the provided parameters
        /// </summary>
        /// <param name="params">SearchParams object</param>
        /// <param name="pagenum"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public static object FindTopics(SearchParams @params, int pagenum, ref int rowcount)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                string[] searchTerms;
                if (String.IsNullOrEmpty(@params.SearchFor))
                    searchTerms = null;
                else
                {
                    switch (@params.Match)
                    {
                        case "any":
                            searchTerms = @params.SearchFor.Split(new char[] {' '});
                            break;
                        case "all":
                            searchTerms = @params.SearchFor.Split(new char[] {' '});
                            break;
                        default:
                            searchTerms = new string[] {@params.SearchFor};
                            break;
                    }
                }
                var posts = dc.Topics
                    .Where(t => t.ForumId == @params.ForumId)
                    .CreatedSince(@params.SinceDate)
                    .CreatedBefore(@params.BeforeDate)
                    .WhereAuthorIs(@params.Author, dc, @params.AuthorPostType)
                    .WhereMessageContains(searchTerms, @params.Match, @params.MessageAndSubject, dc)
                    .Distinct()
                    .OrderByDescending(t=>t.T_LAST_POST);
                    //.WithPaging(0, 15);

                //var res = from t in posts
                //          join c in dc.Categories on t.CatId equals c.Id
                //          join f in dc.Forums on t.ForumId equals f.Id
                //          orderby c.Order, c.Id, f.Order, f.Id, t.T_LAST_POST descending
                //          select t ;)
                rowcount = posts.Count();
                int startrec = pagenum*@params.PageSize;
                return posts.WithPaging(startrec, @params.PageSize).ToList();
            }

        }
    }
}
