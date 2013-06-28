using System;
using System.Web.Security;
using SnitzCommon;

namespace PrivateMessaging.Data
{
    public partial class PrivateMessage
    {
        public DateTime Sent { get { return this.SentDate.ToDateTime().Value; } }

    }
    
}
