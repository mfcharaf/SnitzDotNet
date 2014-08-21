using System;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class RolesListBox : ListBox
    {
        protected override void OnInit(EventArgs e)
        {

            base.OnInit(e);
            
            this.DataSource = Roles.GetAllRoles();
            this.DataBind();    
            this.SelectionMode = ListSelectionMode.Multiple;
        }

    }
}
