using System;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace PrivateMessaging
{
    public partial class PmAlert : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            if (currentUser != null)
            {
                if(Data.Util.GetPMCount(currentUser.ProviderUserKey) > 0)
                {
                    var alert = new StringBuilder();
                    alert.Append("<div style=\"z-index:10000;float:left;\">");
                    alert.Append("<a href='/PrivateMessages' title=''>");
                    alert.Append("<img src='/images/icon_pmgotmail.gif' />");
                    alert.Append("</a>");
                    alert.Append("</div>");
                    pmAlert.Text = alert.ToString();
                }
            }
        }
    }

}
