using System;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using SnitzConfig;

namespace SnitzUI.MasterTemplates
{
    public partial class SingleCol : BaseMasterPage
    {
        #region Overrides of BaseMasterPage

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
        #endregion

        public ScriptManager rootScriptManager { get { return MainSM; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            string appPath = Request.ApplicationPath;
            appPath += appPath.EndsWith(@"/") ? String.Empty : @"/";
            string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            string timeagolocale = String.Format("{0}Scripts/locale/jquery.timeago.{1}.js", appPath, lang);
            if (File.Exists(Server.MapPath(timeagolocale)))
                Page.ClientScript.RegisterClientScriptInclude("timeagolang", timeagolocale);
            else if (File.Exists(Server.MapPath(String.Format("{0}Scripts/locale/jquery.timeago.{1}.js", appPath, Thread.CurrentThread.CurrentCulture.Name))))
                Page.ClientScript.RegisterClientScriptInclude("timeagolang", String.Format("{0}Scripts/locale/jquery.timeago.{1}.js", appPath, Thread.CurrentThread.CurrentCulture.Name));
            if (lang == "fa")
                Page.ClientScript.RegisterStartupScript(this.GetType(), "loctime", "$(document).ready(function () {replaceText();});", true);

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
            lblCopyright.Text = @"&copy;" + Config.Copyright;
            imgGoUp.NavigateUrl = Request.RawUrl + @"#top";
            TimerLabel.Visible = Config.ShowTimer;
        }

    }
}