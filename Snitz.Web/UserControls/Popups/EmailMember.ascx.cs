using System;
using System.Web.Security;
using SnitzCommon;


namespace SnitzUI.UserControls.Popups
{

    public partial class EmailMember : TemplateUserControl
    {
        private int _memberid;

        protected string MemberName { get; set; }

        protected string MemberEmail { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                _memberid = (int)Data;
                MembershipUser mu = Membership.GetUser(_memberid, false);
                if (mu != null)
                {
                    MemberName = mu.UserName;
                    MemberEmail = mu.Email;
                }
            }
        }
    }
}