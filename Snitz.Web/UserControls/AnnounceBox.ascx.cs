using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzUI.UserControls
{
    public partial class AnnounceBox : System.Web.UI.UserControl
    {
        public bool Show { get; set; }  
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = Show;
        }
    }
}