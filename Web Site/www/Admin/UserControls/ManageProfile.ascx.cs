using System;
using System.Web.UI.WebControls;
using Snitz.BLL;

namespace SnitzUI.Admin
{
    public partial class Admin_ManageProfile : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataSource = CustomProfile.GetColumns();
            GridView1.DataBind();
        }

        protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void ProfileGridDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0)
                {
                    e.Row.Enabled = false;
                    e.Row.Cells[5].Controls.Clear();
                }
                string value = e.Row.Cells[1].Text;

                if (value == "smallint")
                {
                    e.Row.Cells[1].Text = @"Boolean";
                }
                if(value=="ntext")
                {
                    e.Row.Cells[1].Text = @"nvarchar";
                    e.Row.Cells[2].Text = @"Max";
                }
            }
        }
    }
}