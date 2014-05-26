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
	public class PageBase : Page
	{
        private ScriptManager _pageScriptManager;
        /// <summary>
        /// Private property to handle the lastvisit cookie
        /// </summary>
        private string LastVisitCookie 
        { 
            get
            {
                HttpCookie cookie = Request.Cookies.Get("LastVisit");
                return cookie != null ? cookie.Value : null;
            }
            set
            {
                HttpCookie cookie = new HttpCookie("LastVisit", value) {Expires = DateTime.UtcNow.AddYears(1)};
                Response.Cookies.Add(cookie);
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
        /// True if the current user
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
        /// Value of the _LastVisit cookie, or the current datetime if no cookie exists
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
		public event SiteMapResolveEventHandler SiteMapResolve;
        
        protected override void OnPreInit(EventArgs e)
        {
            //DO we need to run the Database setup
            if (ConfigurationManager.AppSettings["RunSetup"] == "true")
            {
                Response.Redirect("~/Setup/Setup.aspx", true);
            }
            base.OnPreInit(e);

            #region Theme Setting
            Page.Theme = Config.UserTheme;
            Session.Add("PageTheme",Page.Theme);
            #endregion

            if(!Config.ShowRightColumn)
            {
                Session.Add("_MasterPage", "~/MasterTemplates/SingleCol.Master");
            }
            if (Session["_MasterPage"] == null)
            {
                if (MasterPageFile == "~/MasterTemplates/MainMaster.master")
                {
                    Session.Add("_MasterPage", "~/MasterTemplates/MainMaster.master");
                    MasterPageFile = ((string)Session["_MasterPage"]);
                }
            }
            else
            {
                MasterPageFile = ((string)Session["_MasterPage"]);
            }

            HttpContext current = HttpContext.Current;

            //Check for the _LastVisit Session
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
                            mu.LastLoginDate = LastVisitCookie.ToDateTime().Value;
                            Membership.UpdateUser(mu);
                        }
                    }
                }
                //set the last visit cookie now 
                LastVisitCookie = DateTime.UtcNow.ToForumDateStr();
                
            }
            else
            {
                //we have a session so just update LastVisit
                //if (IsAuthenticated)
                //{
                //    Membership.GetUser(current.User.Identity.Name, true);
                //    //we are logged in so get the lastheredate from the db and update the session
                //    LastVisitCookie = LastLoggedOn.ToForumDateStr();
                //}
                //else
                //{
                //    //we are not logged in, so set cookie for our lastheredate
                //    LastVisitCookie = DateTime.UtcNow.ToForumDateStr();
                //}
            }
            current.Session.Add("_IsAdminOrModerator", IsModerator || IsAdministrator);
        }
        
        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			//attach to the static SiteMapResolve event
			SiteMap.SiteMapResolve += SiteMapSiteMapResolve;
            if (HttpContext.Current.User.Identity.Name == "")
            {
                if (Config.RequireRegistration && !HttpContext.Current.User.Identity.IsAuthenticated)
                    if(!Request.Path.ContainsAny(new[]{"register","activate","passreset","login"}))
                        Response.Redirect("\\Account\\login.aspx");
            }
            //turn on the validator links in the footer
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
                string category = Request.Params["CAT"] ?? Request.Params["CAT_ID"];
                CatId = Int32.Parse(category);
            }

            if (!String.IsNullOrEmpty(Request.Params["FORUM"]) || !String.IsNullOrEmpty(Request.Params["FORUM_ID"]))
            {
                string forum = Request.Params["FORUM"] ?? Request.Params["FORUM_ID"];
                try
                {
                    ForumId = Int32.Parse(forum);
                    Session["ForumId"] = ForumId;

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

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            //detach from the static SiteMapResolve event - it's no longer needed because the request is finished
            SiteMap.SiteMapResolve -= SiteMapSiteMapResolve;
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
                if (objErr.Message == "FloodCheck")
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


                if (IsAdministrator && Config.DebugMode)
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

	    public void ReloadPage()
	    {
            Response.Redirect(Request.RawUrl);
	    }

		SiteMapNode SiteMapSiteMapResolve(object sender, SiteMapResolveEventArgs e)
		{

		    //only raise the event if the request is for this page - remember this handler is handling a static event that fires for all pages
			//therefore, we don't automatically know that the event is being fired for this page - that is what IsSameRequest's is for
		    return IsSamePage(Context, e.Context) ? OnSiteMapResolve(e) : SiteMap.CurrentNode;
		}

	    /// <summary>
		/// Raises the <see cref="SiteMapResolve"/> event for this page.
		/// </summary>
		protected virtual SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
		{
			if (SiteMapResolve != null)
			{
				return SiteMapResolve(this, e);
			}

			return SiteMap.CurrentNode;
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
        
        protected override void InitializeCulture()
        {
            HttpCookie cookie = Request.Cookies.Get("ddlLang");
            if (cookie != null && cookie.Value != null)
            {
                string lang = cookie.Value;//default to the invariant culture
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
            //Response.Charset = CultureInfo.CurrentCulture.TextInfo.ANSICodePage.ToString();
        }

        #region Page methods

        [System.Web.Services.WebMethod]
        public static object[] ExecuteCommand(string commandName, string targetMethod, object data)
        {
            try
            {
                object[] result = new object[2];
                result[0] = Command.Create(commandName).Execute(data);
                result[1] = targetMethod;
                return result;
            }
            catch
            {
                // TODO: add logging functionality 
                throw;
            }
        }

        [System.Web.Services.WebMethod]
        public static string CastVote(string responseid)
        {
            //var test = HttpUtility.UrlDecode(jsonform);
            //System.Collections.Specialized.NameValueCollection formresult = HttpUtility.ParseQueryString(test);
            //ctl00$rblPollAnswer
            string answerid = responseid;
            if (answerid != null)
            {
                bool res = Polls.CastVote(Membership.GetUser().ProviderUserKey, Convert.ToInt32(answerid));
                if (res)
                    return "Your vote was cast";
            }
            throw new Exception("Error casting vote");

        }

        #endregion
    }
}
