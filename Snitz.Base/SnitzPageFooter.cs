using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzBase
{
    public partial class SnitzPageFooter : UserControl
    {
        protected HyperLink LnkForumFooter;

        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();
            LnkForumFooter = this.FindControl("lnkForumFooter") as HyperLink;

            if (LnkForumFooter != null)
            {
                LnkForumFooter.ToolTip = @"Powered By: Snitz Forums.Net v2.0";
                LnkForumFooter.Text = @"Snitz Forums.Net";
                LnkForumFooter.NavigateUrl = "http://forum.snitz.com";
                if (!Config.ShowPoweredByImage) LnkForumFooter.ImageUrl = "";
            }
        }
    }
}
