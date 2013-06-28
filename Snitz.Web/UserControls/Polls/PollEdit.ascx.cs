using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzUI.Admin
{
    public partial class EditPoll : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SetFeaturedPoll(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox) sender;
            int pollid = Convert.ToInt32(Request.QueryString["pid"]);
            if (chk.Checked)
                Config.ActivePoll = pollid;
            else
            {
                if (Config.ActivePoll == pollid)
                {
                    Config.ActivePoll = -1;
                }
            }
        }

        protected void PollDatabound(object sender, EventArgs e)
        {
            DetailsView view = (DetailsView)sender;
            int pollid = Convert.ToInt32(Request.QueryString["pid"]);

            DetailsViewRowCollection rows = view.Rows;
            DetailsViewRow row = rows[2];
            ((CheckBox)row.Cells[1].Controls[1].Controls[0].Controls[1]).Checked = (pollid == Config.ActivePoll);
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int idx = e.NewEditIndex;

        }
    }
}