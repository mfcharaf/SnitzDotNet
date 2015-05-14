/*
####################################################################################################################
##
## SnitzBase - PageBase
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
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using PersianCulture;
using Snitz.BLL;
using SnitzConfig;
using Snitz.Entities;
using SnitzMembership;

namespace SnitzCommon
{
    public interface ISiteMapResolver
    {
        SiteMapNode SiteMapResolve(object sender, SiteMapResolveEventArgs e);
    }

    public class PageBase : Page
	{
        private ScriptManager _pageScriptManager;
        private bool _isMobile;
        /// <summary>
        /// Private property to handle the lastvisit cookie
        /// </summary>
        private string LastVisitCookie 
        {
            get
            {
                return SnitzCookie.GetLastVisitDate();
            }
            set
            {
                SnitzCookie.SetLastVisitCookie(value);
            }
        }
        private DateTime? _lastLoggedOn;
	    private DateTime LastLoggedOn
	    {
	        get
	        {
                if(_lastLoggedOn == null)
                {
                    MembershipUser mu = Membership.GetUser(HttpContext.Current.User.Identity.Name, HttpContext.Current.User.Identity.IsAuthenticated);
                    if (mu != null)
                    {
                        //a > b ? a : b;
                        _lastLoggedOn = LastVisitDateTime > mu.LastLoginDate ? mu.LastLoginDate : LastVisitDateTime;
                        return _lastLoggedOn.Value;
                    }
                    
                }
                return DateTime.UtcNow;
	        }
	    }

        /// <summary>
        /// Keep track of the current pages where page has paged dataset
        /// </summary>
        public int CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] != null)
                    return (int)ViewState["CurrentPage"];
                return 0;
            }
            protected set
            {
                ViewState["CurrentPage"] = value;
            }
        }
        public int? CatId;
	    public int? ForumId;
	    public int? TopicId;

        /// <summary>
        /// Profile of current user
        /// </summary>
        public readonly ProfileCommon Profile = ProfileCommon.GetProfile();
        /// <summary>
        /// True if current user is logged in
        /// </summary>
	    public readonly bool IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        /// <summary>
        /// True if current user is a moderator
        /// </summary>
        public bool IsModerator
        {
            get
            {
                if (HttpContext.Current.Session != null)
                if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                {
                    bool check = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Moderator");
                    return check;
                }
                return false;
            }
        }
        /// <summary>
        /// True if the current user is an administrator
        /// </summary>
        public bool IsAdministrator
        {
            get
            {
                if (HttpContext.Current.Session !=null)
                if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                {
                    bool check = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");
                    return check;
                }
                return false;
            }
           
        }
        /// <summary>
        /// Membership object for current user
        /// </summary>
	    public readonly MemberInfo Member = Members.GetMember(HttpContext.Current.User.Identity.Name);
        /// <summary>
        /// Value of the _LastVisit session, or the current datetime if no cookie exists
        /// </summary>
        public DateTime LastVisitDateTime
	    {
            get
            {
                if(Session["_LastVisit"] != null)
                {
                    string lastvisit = Session["_LastVisit"].ToString();

                    var dateTime = lastvisit.ToDateTime();
                    if (dateTime != null) return dateTime.Value;
                }
                return DateTime.UtcNow;
            }
	    }

        public Stopwatch stopWatch = Stopwatch.StartNew();

        public ScriptManager PageScriptManager
        {
            get { return _pageScriptManager ?? (_pageScriptManager = ScriptManager.GetCurrent(this)); }
        }

        /// <summary>
        /// Sitemap page handler
        /// </summary>
		public event SiteMapResolveEventHandler OnSiteMapResolve;
        
        protected override void OnPreInit(EventArgs e)
        {
            //DO we need to run the Database setup
            try
            {
                if (ConfigurationManager.AppSettings["RunSetup"] == "true")
                {
                    Response.Redirect("~/Setup/Setup.aspx", true);
                }
            }
            catch (Exception)
            {
                throw new FileNotFoundException("Could not load setup page");
            }

            base.OnPreInit(e);


            #region Set Master Template
            var useragent = Request.UserAgent;
            var isAndroid = false;
            
            if (useragent != null)
            {
                if (useragent.ToLower().Contains("android"))
                {
                    isAndroid = true;
                }
            }
            _isMobile = false;
            try
            {
                
                if (Request.Browser.IsMobileDevice || isAndroid || Page.Theme == "Mobile")
                {
                    Session.Add("_MasterPage", "/MasterTemplates/Mobile.Master");
                    _isMobile = true;
                }
                else if (!Config.ShowRightColumn)
                {
                    Session.Add("_MasterPage", "/MasterTemplates/SingleCol.Master");
                }
                if (Session["_MasterPage"] == null)
                {
                    if (MasterPageFile == "/MasterTemplates/MainMaster.master")
                    {
                        Session.Add("_MasterPage", "/MasterTemplates/MainMaster.master");
                        MasterPageFile = ((string)Session["_MasterPage"]);
                    }
                }
                else
                {
                    MasterPageFile = ((string)Session["_MasterPage"]);
                }
            }
            catch (Exception)
            {
                throw new Exception("Problem loading Master Template");
            }
            #endregion

            #region Theme Setting
            try
            {
                Page.Theme = _isMobile ? "Mobile" : Config.UserTheme;

                Session.Add("PageTheme", Page.Theme);
            }
            catch (Exception)
            {
                //set to default
                Page.Theme = Config.DefaultTheme;
            }
            #endregion

            #region Update Last Visit cookie
            HttpContext current = HttpContext.Current;

            //Check for the _LastVisit Session
            try
            {                
                if (current.Session["_LastVisit"] == null)
                {
                    //Is there a last vist cookie
                    if (LastVisitCookie != null)
                    {
                        current.Session.Add("_LastVisit", LastVisitCookie);
                        
                        if (IsAuthenticated)
                        {
                            //we are logged in so update lastheredate in the db and update the session
                            //update lastactivitydate
                            MembershipUser mu = Membership.GetUser(current.User.Identity.Name, true);

                            if (mu != null)
                            {
                                mu.LastActivityDate = mu.LastLoginDate;
                                var dateTime = LastVisitCookie.ToDateTime();
                                if (dateTime.HasValue)
                                    mu.LastLoginDate = dateTime.Value;
                                Membership.UpdateUser(mu);
                            }
                        }
                    }
                    //set the last visit cookie now 
                    LastVisitCookie = DateTime.UtcNow.ToForumDateStr();
                }
            }
            catch (Exception)
            {
                //there was a problem so set it to now
                LastVisitCookie = DateTime.UtcNow.ToForumDateStr();
            }
            #endregion

            current.Session.Add("_IsAdminOrModerator", IsModerator || IsAdministrator);
        }

	    public void ShowMessage(string msg)
	    {
            Page.ClientScript.RegisterStartupScript(GetType(), "msgbox", "alert('" + msg + "');", true); 
	    }

	    protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //If not logged in and registration required, redirect to login page
            if (HttpContext.Current.User.Identity.Name == "" || Member.Username.ToLower() == "guest")
            {
                if (Config.RequireRegistration && (!HttpContext.Current.User.Identity.IsAuthenticated || Member.Username.ToLower() == "guest"))
                    if(!Request.Path.ContainsAny(new[]{"register","activate","passreset","login"}))
                        Response.Redirect("\\Account\\login.aspx");
            }
            //turn on the validator links in the footer if they exist
            if (Master != null)
            {
                var w3CVal = (ContentPlaceHolder)Master.FindControl("W3CVal");
                if (w3CVal != null)
                {
                    w3CVal.Visible = !Request.Url.Host.Contains("localhost");
                }
            }

            //let's capture the querystring params here
            if (!String.IsNullOrEmpty(Request.Params["CAT"]) || !String.IsNullOrEmpty(Request.Params["CAT_ID"]))
            {
                CatId = Int32.Parse(Request.Params["CAT"] ?? Request.Params["CAT_ID"]);
            }

            if (!String.IsNullOrEmpty(Request.Params["FORUM"]) || !String.IsNullOrEmpty(Request.Params["FORUM_ID"]))
            {
                try
                {
                    ForumId = Int32.Parse(Request.Params["FORUM"] ?? Request.Params["FORUM_ID"]);
                    Session["FORUMID"] = ForumId;
                }
                catch
                {
                    throw new HttpException(404, "Forum not found");
                }
                if (ForumId < 0)
                    throw new HttpException(404, "Forum not found");
            }
            if (!String.IsNullOrEmpty(Request.Params["TOPIC"]) || !String.IsNullOrEmpty(Request.Params["TOPIC_ID"]) || HttpContext.Current.Items["TopicId"] != null)
            {
                string topic = Request.Params["TOPIC"] ?? Request.Params["TOPIC_ID"];
                if (HttpContext.Current.Items["TopicId"] != null)
                    topic = HttpContext.Current.Items["TopicId"].ToString();
                TopicId = Convert.ToInt32(topic);
                Session["TopicId"] = TopicId;
            }

		}

	    protected virtual void Page_PreRender(object sender, EventArgs e)
        {
            stopWatch.Stop();
            BaseMasterPage master = (BaseMasterPage) this.Master;
            
            if (master != null)
                master.PageTimer = string.Format(Resources.webResources.lblTimer, ((float)stopWatch.ElapsedMilliseconds / 1000));
        }

        public void Page_Error(object sender, EventArgs e)
        {
            if (!IsAdministrator)
            {
                Exception objErr = Server.GetLastError().GetBaseException();
                string err = "";
                if (objErr is SecurityException)
                {
                    err = "<b>Authentication Problem</b><hr><br/>" +
                             "<br/><b>Only authorised members are allowed access.<br/>Please login to view content.</b>";                     
                }
                else if (objErr.Message == "FloodCheck")
                {
                    err = "<b>Flood control enabled</b><hr><br/>" +
                             "<br/><b>Please try later</b>";   
                }
                else
                {
                    err = "<b>Error Caught in Page_Error event</b><hr><br/>" +
                             "<br/><b>Error in: </b>" + Request.Url.ToString() +
                             "<br/><b>Error Message: </b>" + objErr.Message.ToString();                    
                }

                if (Config.DebugMode)
                    err += "<br/><b>Stack Trace:</b><br/>" + objErr.StackTrace.ToString();
                Response.Write(
                    "<div style=\"width:auto;margin:100px;border:1px solid red;color:DarkBlue;font-family:Tahoma,Arial,Helvetica;padding:4px;\">");
                Response.Write(err);
                Response.Write("<br/></div>");
                Response.Write(
                    "<div style=\"width:auto;margin:100px;margin-top:0px;font-family:Tahoma,Arial,Helvetica;text-align:center;\">");
                Response.Write("<a href=\"/default.aspx\" title=\"Return to forum\">Return to Forum</a>");
                Response.Write("</div>");
                Server.ClearError();
            }
        }
        /// <summary>
        /// Public Method to refresh pages
        /// </summary>
	    public void ReloadPage()
	    {
            Response.Redirect(Request.RawUrl);
	    }

		/// <summary>
		/// Determines whether the two contexts are equal and, therefore, whether the SiteMap.SiteMapResolve event should be fired.
		/// </summary>
		protected virtual bool IsSamePage(HttpContext context1, HttpContext context2)
		{
			//by default, the contexts are considered the same if they map to the same file and have the same query string
			return ((Server.MapPath(context1.Request.AppRelativeCurrentExecutionFilePath) == Server.MapPath(context2.Request.AppRelativeCurrentExecutionFilePath))
				&& (context1.Request.QueryString == context2.Request.QueryString));
		}
        
        /// <summary>
        /// Set the page culture and load calendar if required
        /// </summary>
        protected override void InitializeCulture()
        {
            string lang = SnitzCookie.GetDefaultLanguage();
            if (lang != null)
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(lang);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);

            }
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fa")
                {
                    CultureInfo info = Thread.CurrentThread.CurrentCulture;
                    PersianCultureHelper.SetPersianOptions(info);
                    Thread.CurrentThread.CurrentCulture = info;
                }
            base.InitializeCulture();

        }

	}
}
