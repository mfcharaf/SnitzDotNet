using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace SnitzMembership
{
    partial class MembershipDataDataContext
    {
        public MembershipDataDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
            {
            OnCreated();
            }
        public List<sp_columnsResult> GetProfileColumns()
        {
            List<sp_columnsResult> res = this.GetColumns("ProfileData", null, null, null, null).ToList();
            return res;
        }
    }
}
