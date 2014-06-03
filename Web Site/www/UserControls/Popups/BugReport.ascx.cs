using System;
using SnitzCommon;

namespace SnitzUI.UserControls.Popups
{
    public partial class BugReport : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.StartupScript = "$('.QRMsgArea').markItUp(mySettings)";
            if (Data != null)
            {
                
            }
        }
    }
}