using System;
using Snitz.BLL.modconfig;

namespace SnitzUI.Admin.Mods
{
    public partial class DefaultMod : System.Web.UI.UserControl
    {
        public string ModName { get; set; }

        private ModController _modcontroler;
        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController(ModName);
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);
            //LoadSettings();
            LoadHelpFile();
        }

        private void LoadHelpFile()
        {
            hlpPanel.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_modcontroler != null)
            {
                _modcontroler.ModInfo.Enabled = rblEnabled.SelectedValue == "1";
                _modcontroler.Save();
            }
        }
    }
}