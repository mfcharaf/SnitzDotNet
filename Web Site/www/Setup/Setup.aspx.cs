using System;
using System.Linq;
using System.Web.Security;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;
using SnitzConfig;



namespace SnitzUI.Setup
{
    public partial class Setup : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!Snitz.BLL.Admin.DatabaseExists())
                {
                    //no database so lets create one
                    updateType.Value = "new";
                    AdminUserRequired.Visible = true;
                    //UpdateDB.Text = "Create Database";
                }
                else
                {
                    //make sure that dbo is the table owner otherwise we will have problems
                    Snitz.BLL.Admin.CheckDBOwner();

                    // ok let's check if the Members table exist or not
                    if (!Snitz.BLL.Admin.DoesTableExist("FORUM_MEMBERS"))
                    {
                        //no members table so let's create the base forum tables
                        updateType.Value = "empty";
                        //UpdateDB.Text = "Create Tables";
                        AdminUserRequired.Visible = true;
                    }
                    else if (Snitz.BLL.Members.GetMemberCount() == 0)
                    {
                        AdminUserRequired.Visible = true;
                        updateType.Value = "upgradeadmin";                        
                    }
                    else
                    {
                        AdminUserRequired.Visible = false;
                        updateType.Value = "upgrade";
                        //UpdateDB.Text = "Upgrade Database";
                    }
                }
            }
        }
    }
}