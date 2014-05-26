using System;
using System.Configuration.Provider;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.Entities;
using Snitz.Providers;
using SnitzMembership;

public partial class Admin_ManageRoles : UserControl
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            RoleView.SetActiveView(viewNewRole);
    }

    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteClick")
        {
            try
            {
                if (Roles.DeleteRole(e.CommandArgument.ToString(), !delPopRoles.Checked))
                {
                    errLbl2.Text = "Role deleted successfully.";
                    errLbl2.Visible = true;
                }
            }
            catch (ProviderException p)
            {
                errLbl2.Text = p.Message;
                errLbl2.Visible = true;
            }

            GridView1.DataBind();
        }

        if (e.CommandName == "EditClick")
        {
            SnitzRoleProvider srp = new SnitzRoleProvider();
            
            int roleid = Convert.ToInt32(e.CommandArgument);
            RoleInfo role  = SnitzRoleProvider.GetRoleFull(roleid);

            txtRoleID.Text = roleid.ToString();
            txtName.Text = role.RoleName;
            txtDescription.Text = role.Description.ToString();
            UsersInRole.Text = String.Format("{0} User(s) in {1} Role", srp.GetUsersInRole(txtName.Text).Length, role.RoleName);
            RoleView.SetActiveView(viewEditRole);
            errLbl2.Visible = false;

            UserListPanel.Visible = (roleid > 1);
            RefreshUserList();
        }

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SnitzRoleProvider.UpdateRoleInfo(Convert.ToInt32(txtRoleID.Text), txtName.Text, txtDescription.Text);

        GridView1.DataBind();
        RoleView.SetActiveView(viewNewRole);
        errLbl2.Visible = false;

    }
    protected void btnReset_Click(object sender, EventArgs e)
    {
        RoleView.SetActiveView(viewNewRole);
        errLbl2.Visible = false;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        RefreshUserList();
    }

    private void RefreshUserList()
    {
        UserList.Items.Clear();
        SnitzRoleProvider srp = new SnitzRoleProvider();
        srp.GetUsersInRole(txtName.Text);
        UserList.DataSource = srp.GetUsersInRole(txtName.Text);
        UserList.DataBind();

        UsersInRole.Text = String.Format("{0} User(s) in {1} Role", UserList.Items.Count, txtName.Text);
        NewUserForRole.Text = "";
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        if (UserList.SelectedIndex == -1)
            return;
        string[] roles = new string[1];
        string[] users = new string[1];

        roles[0] = txtName.Text;
        users[0] = UserList.SelectedItem.Text;

        SnitzRoleProvider srp = new SnitzRoleProvider();
        srp.RemoveUsersFromRoles(users, roles);
        RefreshUserList();
    }
    protected void NewUsername_Click(object sender, EventArgs e)
    {
        string[] roles = new string[1];
        string[] users = new string[1];
        
        SnitzRoleProvider srp = new SnitzRoleProvider();
        roles[0] = txtName.Text;
        users[0] = NewUserForRole.Text;

        srp.AddUsersToRoles(users, roles);
        RefreshUserList();

    }

    protected void btnNewSubmit_Click(object sender, EventArgs e)
    {
        SnitzRoleProvider srp = new SnitzRoleProvider();
        // Check if the RoleID exists

        bool idExists = srp.RoleExists(Convert.ToInt32(txtNewRoleID.Text));

        if (idExists) // Check if the RoleID exists
        {
            errLbl.Text = "The chosen RoleID already exists. Please choose another!";
            errLbl.Visible = true;
            return;
        }

        errLbl.Visible = false;

        // Check if the role name already exists

        bool nameExists = Roles.RoleExists(txtNewName.Text);

        if (nameExists) // Check if the RoleID exists
        {
            errLbl.Text = "The chosen role name already exists. Please choose another!";
            errLbl.Visible = true;
            return;
        }

        //Ok, so the data  that could be invalid is valid, let's create the role.

        bool res = srp.CreateRoleFullInfo(txtNewName.Text, txtNewDescription.Text, Convert.ToInt32(txtNewRoleID.Text));

        if (!res)
        {
            errLbl.Text = "Error writing new role to database";
            errLbl.Visible = true;
        }
        else
        {
            errLbl.Text = "New Role Successfully Created!";
            errLbl.Visible = true;

            txtNewRoleID.Text = "";
            txtNewName.Text = "";
            txtNewDescription.Text = "";
        }
    }

    protected void btnNewReset_Click(object sender, EventArgs e)
    {
        errLbl.Visible = false;
        //  btnSubmit.Enabled = true;
        Response.Redirect("default.aspx?action=roles");
    }
}
