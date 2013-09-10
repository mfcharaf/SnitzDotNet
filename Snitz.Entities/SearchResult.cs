using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snitz.Entities
{
    public class SearchResult : TopicInfo
    {
        public string CategoryTitle { get; set; }
        public int CategoryStatus { get; set; }
        public int CategorySubscriptionLevel { get; set; }

        public string ForumSubject { get; set; }
        public int ForumSubscriptionLevel { get; set; }
        public int ForumStatus { get; set; }
        public int ForumAccessType { get; set; }
        public string ForumPassword { get; set; }
    }
}
