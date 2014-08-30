using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using SnitzCommon;

namespace SnitzUI.Admin.UserControls
{
    public partial class Admin_DbsManager : System.Web.UI.UserControl
    {
        private DbsFileProcessor _dbsUpgrade;
        private string _dbsFile;
        private string _dbsFolder;

        protected void Page_Load(object sender, EventArgs e)
        {
            _dbsFolder = HttpContext.Current.Server.MapPath("~/Admin/dbs/");
            DirectoryInfo folder = new DirectoryInfo(_dbsFolder);
            lbFiles.DataSource = folder.GetFiles("*.dbs");

            lbFiles.DataTextField = "Name";
            lbFiles.DataValueField = "FullName";
            lbFiles.DataBind();
        }

        protected void SelectFile(object sender, EventArgs e)
        {
            var file = lbFiles.SelectedValue;
            if (!String.IsNullOrEmpty(file))
            {
                txtDbsFile.Text = "<pre class=\"brush: xml\">" + Server.HtmlEncode(File.ReadAllText(file)) + "</pre>";
            }
        }

        protected void BtnSubmitClick(object sender, EventArgs e)
        {
            var file = lbFiles.SelectedValue;
            if (!String.IsNullOrEmpty(file))
            {
                _dbsUpgrade = new DbsFileProcessor(file);
                if (_dbsUpgrade.Applied)
                {
                    ((PageBase) Page).ShowMessage("Script has already been applied");
                    return;
                }
                lblResult.Text = _dbsUpgrade.Process();
            }
            
        }
    }
}