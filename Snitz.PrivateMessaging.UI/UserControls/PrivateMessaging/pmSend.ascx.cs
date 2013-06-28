using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Resources;
using SnitzCommon;

namespace PrivateMessaging
{
    public partial class PmSend : UserControl
    {
        public int ToUser { get; set; }
        private string _layout;
        private const string StrCookieUrl = "pmMod";
        private string username = HttpContext.Current.User.Identity.Name;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.Cookies[StrCookieUrl + "paging"] == null || Request.Cookies[StrCookieUrl + "paging"]["outbox"] == null)
            {
                Response.Cookies[StrCookieUrl + "paging"]["outbox"] = "double";
                Response.Cookies[StrCookieUrl + "paging"].Expires = DateTime.UtcNow.AddYears(1);
                _layout = "double";
            }
            else
            {
                _layout = Request.Cookies[StrCookieUrl + "paging"]["outbox"];
            }
        }

        protected void SendPm(object sender, EventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser(username);
            var pm = new Data.PrivateMessage
                         {
                             Subject = tbxSubject.Text,
                             Message = qrMessage.Text,
                             ToMemberId = ToUser,
                             FromMemberId = (int) currentUser.ProviderUserKey,
                             Read = 0,
                             OutBox = _layout != "none" ? 1 : 0,
                             SentDate = DateTime.UtcNow.ToForumDateStr()
                         };
            Data.Util.SendPrivateMessage(pm);
            pmSuccess.Text = PrivateMessage.PmSent;

        }
    }

}
