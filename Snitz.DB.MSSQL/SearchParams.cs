using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnitzData
{
    public class SearchParams
    {
        public int ForumId { get; set; }
        public string SearchFor { get; set; }

        public DateTime? SinceDate { get; set;}
        public DateTime? BeforeDate { get; set; }
        public string Author { get; set; }

        public string Match { get; set; }
        public string AuthorPostType { get; set; }

        public bool MessageAndSubject { get; set; }

        public int PageSize { get; set; }
    }
}
