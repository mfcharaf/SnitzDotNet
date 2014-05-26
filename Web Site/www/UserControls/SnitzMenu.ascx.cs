/*
####################################################################################################################
##
## SnitzUI.UserControls - SnitzMenu.ascx
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

using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModConfig;
using Snitz.BLL;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;


/// <summary>
/// Summary description for TopMenu
/// </summary>
public partial class SnitzMenu : UserControl
{
    private Orientation _orientation;
    private SiteMapMenus eMenuToLoad = SiteMapMenus.NotSet;
    private SiteMapDataSource dsMenu = new SiteMapDataSource();
    public Orientation layout
    {
        get
        {
            return _orientation;
        }
        set
        {
            _orientation = value;
        }
    }
    public SiteMapMenus MenuToLoad
    {
        get { return eMenuToLoad; }
        set { eMenuToLoad = value; }
    }
    public enum SiteMapMenus
    {
        Admin, Secure, Public, Restricted, NotSet
    }

    protected void SetMenuDataSource(SiteMapMenus menu)
    {
        switch (menu)
        {
            case SiteMapMenus.Admin:
                dsMenu.SiteMapProvider = "SecureSiteMap"; //"AdminSiteMap";
                break;
            case SiteMapMenus.Secure:
                dsMenu.SiteMapProvider = "SecureSiteMap";
                break;
            case SiteMapMenus.Public:
                dsMenu.SiteMapProvider = "SecureSiteMap"; //"PublicSiteMap";
                break;
            case SiteMapMenus.Restricted:
                dsMenu.SiteMapProvider = "SecureSiteMap"; //"RestrictedSiteMap";
                break;
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //SetMenuDataSource(eMenuToLoad);
        dsMenu.SiteMapProvider = "SecureSiteMap";
        Menu1.Orientation = _orientation;
        Menu1.DataSource = dsMenu;
        dsMenu.ShowStartingNode = false;
        Menu1.DataBind();
    }

    protected void MenuItemDataBound(object sender, MenuEventArgs e)
    {
        PageBase page = (PageBase)Page;
        MembershipUser mu = Membership.GetUser(false);

        if (e.Item.NavigateUrl.ToLower().Contains("action=validate") || e.Item.NavigateUrl.ToLower().Contains("pendingmembers"))
        {
            int pCount = SnitzMembershipProvider.GetUnApprovedMemberCount();
            e.Item.Enabled = false;
            if (pCount > 0)
            {
                e.Item.Enabled = true;
                e.Item.Text += @" (" + pCount + @")";
            }

        }
        if (e.Item.NavigateUrl.ToLower().Contains("events"))
        {
            if (!ConfigHelper.IsModEnabled("EventsConfig"))
            {
                e.Item.Text = "";
                e.Item.Enabled = false;
            }            
        }
        if(e.Item.NavigateUrl.Contains("gallery.aspx"))
        {
            if (e.Item.NavigateUrl.Contains("mygallery.aspx"))
            {
                if (!Config.UserGallery)
                {
                    e.Item.Text = "";
                    e.Item.Enabled = false;
                }
            }
            else
            {
                if (!Config.ShowGallery)
                {
                    e.Item.Text = "";
                    e.Item.Enabled = false;
                }                
            }

                
        }
        if (e.Item.NavigateUrl.ToLower().Contains("privatemessage"))
        {
            int unreadcount = 0;

            if (((PageBase)Page).Member != null)
            {
                unreadcount = PrivateMessages.GetUnreadPMCount(((PageBase) Page).Member.Id);
            }
            if (unreadcount > 0)
            {
                e.Item.Text += string.Format(" ({0})", unreadcount);
            }
            if (!Config.PrivateMessaging)
            {
                e.Item.Text = "";
                e.Item.Enabled = false;
            }
        }
        if (e.Item.NavigateUrl.ToLower().Contains("active"))
        {

            if(mu != null)
            {

                int unreadcount = Topics.GetNewTopicCount(page.LastVisitDateTime.ToForumDateStr(), page.IsAdministrator || page.IsModerator, 0, 100);
                if (unreadcount > 0)
                {
                    e.Item.Text += @" (" + unreadcount + @")";
                }               
            }

        }
    }
}

