using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzUI.Admin
{
    public partial class AdminRadioButton : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string SelectedValue
        {
            get { return rblOption.SelectedValue; }
            set { rblOption.SelectedValue = value; }
        }

        public string Enabled
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (value.ToLower() == "false")
                    rblOption.Enabled = false;
            }
        }
    }
}