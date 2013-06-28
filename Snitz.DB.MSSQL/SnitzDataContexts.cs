using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SnitzData
{
    partial class SnitzDataClassesDataContext
    {

        public SnitzDataClassesDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
            {
            OnCreated();
            }
    }

    partial class SnitzFaqDataDataContext
    {

        public SnitzFaqDataDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }
    }

    partial class LookupsDataContext
    {

        public LookupsDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }

    }
    partial class PollDataDataContext
    {
        public PollDataDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }
    }
}


