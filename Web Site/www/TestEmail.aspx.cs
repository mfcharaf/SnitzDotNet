using System;
using System.Net.Sockets;
using System.Text;
using SnitzConfig;
using SnitzCommon;

namespace SnitzUI
{
    public partial class TestEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            string res = EmailValidator.IsValidEmail(TextBox1.Text.Trim());
            Response.Write(res);
        }
    }
}