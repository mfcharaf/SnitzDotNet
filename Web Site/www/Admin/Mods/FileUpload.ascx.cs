using System;
using System.IO;

using Snitz.BLL.modconfig;

namespace SnitzUI.Admin.Mods
{
    public partial class FileUpload : System.Web.UI.UserControl
    {
        private ModController _modcontroler;

        protected void Page_Load(object sender, EventArgs e)
        {
            _modcontroler = new ModController("FileUpload");
            rblEnabled.SelectedValue = _modcontroler.ModInfo.Enabled ? "1" : "0";
            modContainer.GroupingText = String.Format(" {0} Configuration ", _modcontroler.ModInfo.Name);
            LoadSettings();
            LoadHelpFile();
        }

        private void LoadSettings()
        {
            txtUploadLocation.Text = _modcontroler.ModInfo.Settings["FileUploadLocation"].ToString();
            txtAllowedImageTypes.Text = _modcontroler.ModInfo.Settings["AllowedImageTypes"].ToString();
            txtAllowedAttachmentTypes.Text = _modcontroler.ModInfo.Settings["AllowedAttachmentTypes"].ToString();
            txtFileSizeLimit.Text = _modcontroler.ModInfo.Settings["FileSizeLimit"].ToString();
            txtTotalUploadLimitFileSize.Text = _modcontroler.ModInfo.Settings["TotalUploadLimitFileSize"].ToString();
            txtTotalUploadLimitFileNumber.Text = _modcontroler.ModInfo.Settings["TotalUploadLimitFileNumber"].ToString();

            rblAllowImageUpload.SelectedValue = _modcontroler.ModInfo.Settings["AllowImageUpload"].ToString();
            rblAllowAttachments.SelectedValue = _modcontroler.ModInfo.Settings["AllowAttachments"].ToString();

        }

        private void LoadHelpFile()
        {
            string helpPath = Server.MapPath(@"HelpFiles\");
            if (File.Exists(helpPath + "FileUpload.txt"))
            {
                litInstructions.Text = File.ReadAllText(helpPath + "FileUpload.txt");
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
                _modcontroler.ModInfo.Settings["FileUploadLocation"] = txtUploadLocation.Text;
                _modcontroler.ModInfo.Settings["AllowedImageTypes"] = txtAllowedImageTypes.Text;
                _modcontroler.ModInfo.Settings["AllowedAttachmentTypes"] = txtAllowedAttachmentTypes.Text;
                _modcontroler.ModInfo.Settings["FileSizeLimit"] = txtFileSizeLimit.Text;
                _modcontroler.ModInfo.Settings["TotalUploadLimitFileSize"] = txtTotalUploadLimitFileSize.Text;
                _modcontroler.ModInfo.Settings["TotalUploadLimitFileNumber"] = txtTotalUploadLimitFileNumber.Text;
                _modcontroler.ModInfo.Settings["AllowImageUpload"] = rblAllowImageUpload.SelectedValue;
                _modcontroler.ModInfo.Settings["AllowAttachments"] = rblAllowAttachments.SelectedValue;

                _modcontroler.Save();
            }
        }
    }
}