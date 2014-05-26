/*
####################################################################################################################
##
## SnitzUI.MasterTemplates - Plain.Master
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

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
                rtlCss.Text = @"<link rel='stylesheet' type='text/css' runat='server' href='/css/" + Page.Theme + @"/rtl.css' />";
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