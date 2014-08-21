using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL.modconfig;

namespace EventsCalendar.Admin.Mods
{
    public partial class EventsAdmin : System.Web.UI.UserControl
    {
        private ModController _modcontroler;
        private string _allowedRoles;

        private List<string> _dsRoles
        {
            get
            {
                if (ViewState["dsRoles"] != null)
                    return (List<string>) ViewState["dsRoles"];
                return new List<string>();
            }
            set
            {
                //Filter = value;
                ViewState.Add("dsRoles", value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController("Events");
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);

            LoadSettings();
            //LoadHelpFile();
        }

        private void LoadHelpFile()
        {
            throw new NotImplementedException();
        }

        private void LoadSettings()
        {
            if (_dsRoles.Count == 0)
            {
                _allowedRoles = _modcontroler.ModInfo.Settings["EventAdminRoles"] != null
                    ? _modcontroler.ModInfo.Settings["EventAdminRoles"].ToString()
                    : String.Empty;
                _dsRoles = _allowedRoles.Split(',').ToList();
                AllowedRoles.DataSource = _dsRoles;
                AllowedRoles.DataBind();
                txtRoles.Text = String.Join(Environment.NewLine, _allowedRoles.Split(','));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_modcontroler != null)
            {
                _modcontroler.ModInfo.Enabled = rblEnabled.SelectedValue == "1";
                _modcontroler.ModInfo.Settings["EventAdminRoles"] = String.Join(",", _dsRoles.ToArray());
                _modcontroler.Save();
            }
        }

        protected void AddRoleClick(object sender, EventArgs e)
        {
            AllowedRoles.DataSource = null;
            foreach (ListItem item in lbRoles.Items)
            {
                if (item.Selected && !_dsRoles.Contains(item.Value))
                    _dsRoles.Add(item.Value);
            }
            AllowedRoles.DataSource = _dsRoles;
            AllowedRoles.DataBind();
        }
    }
}