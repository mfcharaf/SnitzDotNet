using System;
using System.IO;
using Snitz.BLL.modconfig;

namespace SnitzUI.Admin.Mods
{
    public partial class ShareIt : System.Web.UI.UserControl
    {
        private ModController _modcontroler;

        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController("ShareIt");
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);
            LoadSettings();
            LoadHelpFile();
        }

        private void LoadSettings()
        {
            txtMediaItems.Text = _modcontroler.ModInfo.Settings["MediaItems"].ToString();
        }

        private void LoadHelpFile()
        {
            string helpPath = Server.MapPath(@"HelpFiles\");
            if (File.Exists(helpPath + "ShareIt.txt"))
            {
                litInstructions.Text = File.ReadAllText(helpPath + "ShareIt.txt");
                hlpPanel.Visible = true;
            }
            else
            {
                litInstructions.Text = "";
                hlpPanel.Visible = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_modcontroler != null)
            {
                _modcontroler.ModInfo.Enabled = rblEnabled.SelectedValue == "1";
                _modcontroler.ModInfo.Settings["MediaItems"] = txtMediaItems.Text;
                _modcontroler.Save();
            }
        }
    }
}