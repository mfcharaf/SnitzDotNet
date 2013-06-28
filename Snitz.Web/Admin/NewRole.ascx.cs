using System;
using System.Web.UI;
using Snitz.Providers;

public partial class Admin_NewRole : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsCallback)
            errLbl.Visible = false;

        
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SnitzRoleProvider srp = new SnitzRoleProvider();
        // Check if the RoleID exists

        bool idExists = srp.RoleExists(Convert.ToInt32(txtRoleID.Text));

        if (idExists) // Check if the RoleID exists
        {
            errLbl.Text = "The chosen RoleID already exists. Please choose another!";
            errLbl.Visible=true;
            return;
        }
        
        errLbl.Visible = false;

        // Check if the role name already exists

        bool nameExists = srp.RoleExists(txtName.Text);

        if (nameExists) // Check if the RoleID exists
        {
            errLbl.Text = "The chosen role name already exists. Please choose another!";
            errLbl.Visible = true;
            return;
        }
        
        //Ok, so the data  that could be invalid is valid, let's create the role.

        bool res = srp.CreateRoleFullInfo(txtName.Text,txtDescription.Text,Convert.ToInt32(txtRoleID.Text));
        
        if (!res)
        {
            errLbl.Text = "Error writing new role to database";
            errLbl.Visible = true;
        }
        else
        {
            errLbl.Text = "New Role Successfully Created!";
            errLbl.Visible = true;

            txtRoleID.Text = "";
            txtName.Text = "";
            txtDescription.Text = "";
        }
             
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        errLbl.Visible = false;
      //  btnSubmit.Enabled = true;
        Response.Redirect("default.aspx?action=newrole");
    }
}
