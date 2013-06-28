using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzUI.MasterTemplates
{
    public partial class Plain : BaseMasterPage
    {
        public override string PageTimer
        {
            get
            {
                return TimerLabel.Text;
            }
            set
            {
                TimerLabel.Text = value;
            }
        }
        public override string ForumUrl { get; set; }

        public override string ForumTitle { get; set; }


        void Page_Load()
        {

            lblForumTitle.Text = Config.ForumTitle;
            if (Resources.webResources.TextDirection == "rtl")
            {
                //lets add the rtl css file
                rtlCss.Text = @"<link rel='stylesheet' type='text/css' runat='server' href='/css/" + Page.StyleSheetTheme + @"/rtl.css' />";
            }
            //if (HttpContext.Current.User.Identity.Name == "")
            //{
            //    MainMenu.MenuToLoad = Config.ProhibitNewMembers ? SnitzMenu.SiteMapMenus.Restricted : SnitzMenu.SiteMapMenus.Public;
            //}
            //else MainMenu.MenuToLoad = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator") ? SnitzMenu.SiteMapMenus.Admin : SnitzMenu.SiteMapMenus.Secure;

            homeLink.ToolTip = Config.ForumTitle;
            homeLink.NavigateUrl = Config.ForumUrl;
            lblFooterTitle.Text = Config.ForumTitle;
            lblCopyright.Text = @"&copy; " + Config.Copyright;
            imgGoUp.NavigateUrl = Request.RawUrl + @"#top";
            TimerLabel.Visible = Config.ShowTimer;
        }
    }
}