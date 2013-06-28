using System;
using System.Web.UI;


public partial class Admin_UpdateCounts : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        bool res = SnitzData.Util.UpdateForumCounts();

        Label2.Text = res ? "Forum Counts Updated Successfully" : "Error Updating Forum Counts";
    }
}
