using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzData;

namespace SnitzUI.UserControls
{
    public partial class emoticons : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataList1.DataSource = Emoticons.GetEmoticons();
            DataList1.DataBind();
        }
    }
}