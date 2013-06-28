using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.DataBaseLayer;

public partial class Admin_ManageCategories : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Panel2.Visible = false;
            errLbl.Visible = false;
            errLbl2.Visible = false;
        }
    }
    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        switch (e.CommandName)
        {                            

            case "DeleteClick":
                bool resPosts = ForumDatasource.CategoryHasPosts(Convert.ToInt32(e.CommandArgument));

                if (resPosts)
                {
                    bool proceed = chkDelForums.Checked;

                    if (!proceed)
                    {
                        errLbl2.Text = "Cannot delete a category that has forums with posts.<br />" +
                                       "Please delete the posts first, or check the option to delete forums and posts, above.";
                        errLbl2.Visible = true;
                    }
                    else
                    {
                        if (!ForumDatasource.DeleteCategory(Convert.ToInt32(e.CommandArgument)))
                        {
                            errLbl2.Text = "Error deleting the category.";
                            errLbl2.Visible = true;
                        }
                        else 
                        {
                            errLbl2.Text ="Category deleted successfully.";
                            errLbl2.Visible = true;
                        }

                    }
                }
                else
                {
                   
                    if (!ForumDatasource.DeleteCategory(Convert.ToInt32(e.CommandArgument)))
                    {
                        errLbl2.Text = "Error deleting the category.";
                        errLbl2.Visible = true;
                    }
                    else
                    {
                        errLbl2.Text = "Category deleted successfully.";
                            errLbl2.Visible = true;
                    }
                }

                Panel2.Visible = false;
                GridView1.DataBind();
                break;

            case "EditClick":

                DataTable dt = ForumDatasource.GetCategoryFullData(Convert.ToInt32(e.CommandArgument));

                if (dt.Rows.Count != 0)
                {
                    catID.Value  = dt.Rows[0].ItemArray[0].ToString();
                    catStatus.SelectedValue = dt.Rows[0].ItemArray[2].ToString();
                    catMod.SelectedValue = dt.Rows[0].ItemArray[3].ToString();
                    catSub.SelectedValue = dt.Rows[0].ItemArray[4].ToString();
                    txtName.Text = dt.Rows[0].ItemArray[1].ToString();
                    txtOrder.Text = dt.Rows[0].ItemArray[5].ToString();
                    Panel2.Visible = true; 
                }

                break;

            case "LockClick":
                catStatus.SelectedValue = ForumDatasource.LockCategory(Convert.ToInt32(e.CommandArgument)).ToString();
                errLbl2.Visible = false;
                break;

            case "UnlockClick":
                catStatus.SelectedValue = ForumDatasource.UnLockCategory(Convert.ToInt32(e.CommandArgument)).ToString();
                errLbl2.Visible = false;
                break;

          
        }
        
        
        GridView1.DataBind();
        


    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool res;

        res = ForumDatasource.UpdateAllCategoryInfo(Convert.ToInt32(catID.Value), txtName.Text, Convert.ToInt32(txtOrder.Text), 
                       Convert.ToInt32(catStatus.SelectedValue), Convert.ToInt32(catMod.SelectedValue), Convert.ToInt32(catSub.SelectedValue));

        if (!res)
        {
            errLbl.Text = "Error Updating Category";
            errLbl.Visible = false;
        }
        else
        {
            errLbl.Visible = false;
            Panel2.Visible = false;
            GridView1.DataBind();
        }

    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx?action=managecategories");
    }
    
}
