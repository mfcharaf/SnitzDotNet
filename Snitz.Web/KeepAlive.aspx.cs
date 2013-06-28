using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzUI
{
    public partial class KeepAlive : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/html";
            Response.Write("Session Updated - Server Time:" + DateTime.UtcNow);
        }
    }
}