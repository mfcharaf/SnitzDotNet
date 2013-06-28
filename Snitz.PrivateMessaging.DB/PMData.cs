using System.Configuration;

namespace PrivateMessaging.Data
{
    partial class PMDataDataContext
    {
        public PMDataDataContext()
            : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }
    }
}
