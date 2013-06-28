using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SnitzCommon;

namespace PrivateMessaging
{
    public partial class PrivateMessageView : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var pmview = (PmView)Page.LoadControl("~/UserControls/PrivateMessaging/pmview.ascx");

            ViewPm.Controls.Add(pmview);
        }
    }
}