#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
#endregion

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
using SnitzConfig;
using SnitzData;
using SnitzMembership;

namespace SnitzCommon
{
	public class PageBase : Page
	{
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

        public readonly ProfileCommon Profile = ProfileCommon.GetProfile();
        public ScriptManager PageScriptManager
        {
            get { return _pageScriptManager ?? (_pageScriptManager = ScriptManager.GetCurrent(this)); }
        }

	    public readonly bool IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
        public bool IsModerator
        {
            get
            {
                if (HttpContext.Current.Session != null)
                if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                {
                    if (HttpContext.Current.Session["moderator"] != null)
                        return (bool)HttpContext.Current.Session["moderator"];
                    bool check = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Moderator");
                    HttpContext.Current.Session.Add("moderator", check);
                    return check;

                }
                return false;
            }
        }
        public bool IsAdministrator
        {
            get
            {
                if (HttpContext.Current.Session !=null)
                if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                {
                    if (HttpContext.Current.Session["adminuser"] != null)
                        return (bool)HttpContext.Current.Session["adminuser"];
                    bool check = Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator");
                    HttpContext.Current.Session.Add("adminuser",check);
                    return check;

                }
                return false;
            }
           
        }

	    public readonly SnitzData.Member Member = Util.GetMember(HttpContext.Current.User.Identity.Name);

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
	         
        public override String StyleSheetTheme
        {
            get
            {
                //HttpCookie cookie = Request.Cookies.Get("Theme");
                //return cookie != null ? cookie.Value : Config.DefaultTheme;
                return Config.DefaultTheme;
            }
            set
            {
                HttpCookie cookie = new HttpCookie("Theme", value);
                Response.Cookies.Add(cookie);
                //Request.Cookies["Theme"].Value = value;
            }
        }
        
        public Stopwatch stopWatch = Stopwatch.StartNew();

        private ScriptManager _pageScriptManager;
        private string LastVisitCookie 
        { 
            get
            {
                HttpCookie cookie = Request.Cookies.Get("LastVisit");
                return cookie != null ? cookie.Value : null;
            }
            set
            {
                HttpCookie cookie = new HttpCookie("LastVisit", value) {Expires = DateTime.UtcNow.AddDays(61)};
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
                        _lastLoggedOn = mu.LastLoginDate;
                        return _lastLoggedOn.Value;
                    }
                    
                }
                return DateTime.UtcNow;
	        }
	    }
	    

	    protected virtual void Page_PreRender(object sender, EventArgs e)
        {
            stopWatch.Stop();
            BaseMasterPage master = (BaseMasterPage) this.Master;
            //TODO: fix this later
            if (master != null)
                master.PageTimer = string.Format(Resources.webResources.lblTimer, ((float)stopWatch.ElapsedMilliseconds / 1000));
        }

        public void Page_Error(object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            string err = "<b>Error Caught in Page_Error event</b><hr><br/>" +
                         "<br/><b>Error in: </b>" + Request.Url.ToString() +
                         "<br/><b>Error Message: </b>" + objErr.Message.ToString();
            if (IsAdministrator || Config.DebugMode)
                    err += "<br/><b>Stack Trace:</b><br/>" + objErr.StackTrace.ToString();
            Response.Write("<div style=\"width:auto;margin:100px;border:1px solid red;color:DarkBlue;font-family:Tahoma,Arial,Helvetica;padding:4px;\">");
            Response.Write(err);
            Response.Write("<br/></div>");
            Response.Write("<div style=\"width:auto;margin:100px;margin-top:0px;font-family:Tahoma,Arial,Helvetica;text-align:center;\">");
            Response.Write("<a href=\"/default.aspx\" title=\"Return to forum\">Return to Forum</a>");
            Response.Write("</div>");
            Server.ClearError();
        }

		public event SiteMapResolveEventHandler SiteMapResolve;

        protected override void OnPreInit(EventArgs e)
        {
            
            #region Theme Setting
            string ThemeName = Config.DefaultTheme;
            HttpCookie themecookie = Request.Cookies.Get("Theme");
            if (themecookie != null)
            {
                ThemeName = StyleSheetTheme;
                Session.Add("_theme", ThemeName);
            }
            if (Session["_theme"] == null)
            {
                //if (IsAuthenticated)
                //{
                //    if (Member != null)
                //        if (Member.Theme  != "")
                //            ThemeName = Member.Theme;
                //}
                Session.Add("_theme", ThemeName);

            }
            #endregion

            //DO we need to run the Database setup
            if (ConfigurationManager.AppSettings["RunSetup"] == "true")
            {
                Response.Redirect("~/Setup/Setup.aspx", true);
            }
            base.OnPreInit(e);


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
                //Our session is empty, reset everything
                if (current.User.Identity.IsAuthenticated)
                {
                    //we are logged in so get the lastheredate from the db and update the session
                    current.Session.Add("_LastVisit", LastLoggedOn.ToForumDateStr());
                    //update lastactivitydate
                    MembershipUser mu = Membership.GetUser(current.User.Identity.Name, true);
                    if (mu != null)
                    {
                        mu.LastLoginDate = DateTime.UtcNow;
                        Membership.UpdateUser(mu);
                    }
                }
                else
                {
                    //we are not logged in, so check the cookie for our lastheredate
                    current.Session.Add("_LastVisit", LastVisitCookie ?? DateTime.UtcNow.ToForumDateStr());
                }
                
                
            }
            else
            {
                //we have a session so just update LastVisit
                if (current.User.Identity.IsAuthenticated)
                {
                    Membership.GetUser(current.User.Identity.Name, true);
                    //we are logged in so get the lastheredate from the db and update the session
                    LastVisitCookie = LastLoggedOn.ToForumDateStr();
                }
                else
                {
                    //we are not logged in, so set cookie for our lastheredate
                    LastVisitCookie = DateTime.UtcNow.ToForumDateStr();
                }
            }

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

        #region Page methods for Ajax Name and Email checks

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
                bool res = Util.CastVote(Membership.GetUser().ProviderUserKey, Convert.ToInt32(answerid));
                if (res)
                    return "Your vote was cast";
            }
            throw new Exception("Error casting vote");

        }

        #endregion
    }
}
