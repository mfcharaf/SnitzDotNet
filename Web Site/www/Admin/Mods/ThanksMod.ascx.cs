using System;
using System.IO;
using Snitz.BLL.modconfig;

namespace SnitzUI.Admin.Mods
{
    public partial class ThanksMod : System.Web.UI.UserControl
    {
        private ModController _modcontroler;

        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController("Thanks");
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);

            LoadSettings();
            LoadHelpFile();
        }

        private void LoadHelpFile()
        {
            string helpPath = Server.MapPath(@"HelpFiles\");
            if (File.Exists(helpPath + "ThanksMod.txt"))
            {
                litInstructions.Text = File.ReadAllText(helpPath + "ThanksMod.txt");
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
            rbMember.SelectedValue = _modcontroler.ModInfo.Settings["Member Likes"].ToString();
            rbTopic.SelectedValue = _modcontroler.ModInfo.Settings["Topic Likes"].ToString();
            rbReply.SelectedValue = _modcontroler.ModInfo.Settings["Reply Likes"].ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}