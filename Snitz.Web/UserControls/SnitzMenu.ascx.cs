using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzCommon;
using Snitz.Providers;


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
            default:
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
        if (e.Item.NavigateUrl.Contains("action=validate") || e.Item.NavigateUrl.Contains("pendingmembers.aspx"))
        {
            SnitzMembershipProvider mp = new SnitzMembershipProvider();
            int pCount = SnitzMembershipProvider.GetUnApprovedMemberCount();
            e.Item.Enabled = false;
            if (pCount > 0)
            {
                e.Item.Enabled = true;
                e.Item.Text += @" (" + pCount + @")";
            }

        }
        if(e.Item.NavigateUrl.Contains("gallery.aspx"))
        {
            if (!SnitzConfig.Config.ShowGallery)
            {
                e.Item.Text = "";
                e.Item.Enabled = false;
            }
                
        }
        if (e.Item.NavigateUrl.Contains("privatemessageview.aspx"))
        {
            // Create an object array consisting of the parameters to the method.
            // Make sure you get the types right or the underlying
            // InvokeMember will not find the right method
            Object[] args = { ((PageBase)Page).Member.Id };

            Object result = DynaInvoke.InvokeMethod(Context.Server.MapPath("~/bin/PrivateMessaging.Data.dll"), "Util", "GetPMCount", args);
            //int unreadcount = PMData.Util.GetPMCount(((PageBase) Page).Member.Id);
            int unreadcount = (int) result;
            if (unreadcount > 0)
            {
                e.Item.Text += @" (" + unreadcount + @")";
            }

        }
        if (e.Item.NavigateUrl.Contains("active.aspx"))
        {
            MembershipUser mu = Membership.GetUser(false);
            if(mu != null)
            {
                int unreadcount = SnitzData.PagedObjects.GetActiveTopicCount(mu.LastLoginDate.ToForumDateStr(),0,0);
                if (unreadcount > 0)
                {
                    e.Item.Text += @" (" + unreadcount + @")";
                }               
            }

        }
    }
}

