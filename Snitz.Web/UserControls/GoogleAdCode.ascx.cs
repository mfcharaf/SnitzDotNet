using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzUI.UserControls
{
    public partial class GoogleAdCode : System.Web.UI.UserControl
    {

        public string AdSlot { get; set; }
        public string AdCode { get { return Config.GoogleAdCode; } }
        public int AdWidth { get; set; }
        public int AdHeight { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}