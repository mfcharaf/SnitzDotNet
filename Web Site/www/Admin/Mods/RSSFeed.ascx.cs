using System;
using System.IO;
using Snitz.BLL.modconfig;

namespace SnitzUI.Admin.Mods
{
    public partial class RSSFeed : System.Web.UI.UserControl
    {
        private ModController _modcontroler;

        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController("RSSFeed");
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);

            LoadSettings();
            LoadHelpFile();
            
        }

        private void LoadHelpFile()
        {
            string helpPath = Server.MapPath(@"HelpFiles\");
            if (File.Exists(helpPath + "RSSFeed.txt"))
            {
                litInstructions.Text = File.ReadAllText(helpPath + "RSSFeed.txt");
                hlpPanel.Visible = true;
            }
            else
            {
                litInstructions.Text = "";
                hlpPanel.Visible = false;
            }
        }

        private void LoadSettings()
        {
            txtUrl.Text = _modcontroler.ModInfo.Settings["Url"].ToString();
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (_modcontroler != null)
            {
                _modcontroler.ModInfo.Enabled = rblEnabled.SelectedValue == "1";
                _modcontroler.ModInfo.Settings["Url"] = txtUrl.Text;
                _modcontroler.Save();
            }
        }
    }
}