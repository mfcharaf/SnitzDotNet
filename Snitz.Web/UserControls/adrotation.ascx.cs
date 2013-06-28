using System;
using System.Web;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzUI.UserControls
{
    
    public partial class adrotation : System.Web.UI.UserControl
    {
        public string Filter { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        private const string Iframe =
            "<iframe id=\"iframe1\" width=\"{0}\" height=\"{1}\"  frameborder=\"0\" style=\"background-color:#ffffff;\" scrolling=\"no\" src=\"/Content/AdRotator.aspx?f={2}\" name=\"iframe1\"></iframe>";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Config.ShowHeaderAds)
            {
                GoogleAd.Visible = Config.ShowGoogleAds;
                if(!Config.ShowGoogleAds)
                    Literal1.Text = String.Format(Iframe, Width, Height, Filter);
            }
        }
    }
}