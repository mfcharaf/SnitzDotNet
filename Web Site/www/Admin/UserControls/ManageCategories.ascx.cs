using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzConfig;


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
            case "NewClick" :
                catID.Value  = "0";
                catStatus.SelectedValue = "1";
                catMod.SelectedIndex = -1;
                catSub.SelectedIndex = -1;
                txtName.Text = "";
                txtOrder.Text = "";
                Panel2.Visible = true; 
                break;
            case "DeleteClick":
                bool resPosts = Categories.CategoryHasPosts(Convert.ToInt32(e.CommandArgument));

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
                        try
                        {
                            Categories.DeleteCategory(Convert.ToInt32(e.CommandArgument));
                            errLbl2.Text ="Category deleted successfully.";
                            errLbl2.Visible = true;
                        }
                        catch (Exception)
                        {
                            errLbl2.Text = "Error deleting the category.";
                            errLbl2.Visible = true;
                            throw;
                        }
                    }
                }
                else
                {
                    try
                    {
                        Categories.DeleteCategory(Convert.ToInt32(e.CommandArgument));
                        errLbl2.Text = "Category deleted successfully.";
                            errLbl2.Visible = true;
                    }
                    catch (Exception)
                    {
                        errLbl2.Text = "Error deleting the category.";
                        errLbl2.Visible = true;
                        throw;
                    }
                }

                Panel2.Visible = false;
                GridView1.DataBind();
                break;

            case "EditClick":

                CategoryInfo cat = Categories.GetCategory(Convert.ToInt32(e.CommandArgument));
                if (cat != null)
                {
                    catID.Value  = cat.Id.ToString();
                    catStatus.SelectedValue = cat.Status.ToString();
                    catMod.SelectedValue = cat.ModerationLevel.ToString();
                    catSub.SelectedValue = cat.SubscriptionLevel.ToString();
                    txtName.Text = cat.Name;
                    txtOrder.Text = cat.Order.ToString();
                    Panel2.Visible = true; 
                }

                break;

            case "LockClick":
                Categories.SetCatStatus(Convert.ToInt32(e.CommandArgument), (int)Enumerators.PostStatus.Closed);
                catStatus.SelectedValue = "1";
                errLbl2.Visible = false;
                break;

            case "UnlockClick":
                Categories.SetCatStatus(Convert.ToInt32(e.CommandArgument), (int)Enumerators.PostStatus.Open);
                catStatus.SelectedValue = "0";
                errLbl2.Visible = false;
                break;

          
        }
        
        
        GridView1.DataBind();
        


    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        bool res;
        CategoryInfo cat = catID.Value == "0" ? new CategoryInfo() : Categories.GetCategory(Convert.ToInt32(catID.Value));
        cat.Name = txtName.Text;
        cat.Order = Convert.ToInt32(txtOrder.Text);
        cat.Status = Convert.ToInt32(catStatus.SelectedValue);
        cat.ModerationLevel = Convert.ToInt32(catMod.SelectedValue);
        cat.SubscriptionLevel = Convert.ToInt32(catSub.SelectedValue);
        try
        {
            if (catID.Value == "0")
                Categories.AddCategory(cat);
            else
                Categories.UpdateCategory(cat);
            errLbl.Visible = false;
            Panel2.Visible = false;
            GridView1.DataBind();
        }
        catch (Exception)
        {
            errLbl.Text = catID.Value == "0" ? "Error Adding Category" : "Error Updating Category";
            errLbl.Visible = false;
            throw;
        }
    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx?action=managecategories");
    }
    
}
