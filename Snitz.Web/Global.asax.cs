/*
####################################################################################################################
##
## SnitzUI - Global.asax
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
using System.Reflection;
using System.Web;
using System.Web.Routing;

using SnitzCommon;



namespace SnitzUI
{
    public class Global : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // The routes: 
            routes.Add(new Route("Topic/{subject}", new WebFormRouteHandler("~/Content/Forums/Topic.aspx")));
            routes.Add(new Route("Member/{username}", new WebFormRouteHandler("~/Account/Profile.aspx")));
            routes.Add(new Route("MemberList", new WebFormRouteHandler("~/Content/Forums/Members.aspx")));
            routes.Add(new Route("Members", new WebFormRouteHandler("~/Content/Forums/Members.aspx")));
            routes.Add(new Route("FindTopics", new WebFormRouteHandler("~/Content/Forums/Search.aspx")));
            routes.Add(new Route("Search", new WebFormRouteHandler("~/Content/Forums/Search.aspx")));
            routes.Add(new Route("Find", new WebFormRouteHandler("~/Content/Forums/Search.aspx")));
            routes.Add(new Route("Active", new WebFormRouteHandler("~/Content/Forums/Active.aspx")));
            routes.Add(new Route("ActiveTopics", new WebFormRouteHandler("~/Content/Forums/Active.aspx")));
            routes.Add(new Route("Help", new WebFormRouteHandler("~/Content/Faq/help.aspx")));
            routes.Add(new Route("Faq", new WebFormRouteHandler("~/Content/Faq/help.aspx")));
            routes.Add(new Route("PrivateMessages", new WebFormRouteHandler("~/Content/PrivateMessages/PrivateMessageView.aspx")));
            //'Member/NewMail

            Assembly assembly = Assembly.LoadFile(HttpContext.Current.Server.MapPath("~/bin/Snitz.EventsCalendar.UI.dll"));
            const string fullTypeName = "EventsCalendar.EventPageRouteHandler";
            Type calc = assembly.GetType(fullTypeName);
            var types = new Type[1];
            types[0] = typeof(string);
            ConstructorInfo ctor = calc.GetConstructor(types);

            if (ctor != null)
            {
                routes.Add(new Route("Events", (IRouteHandler)ctor.Invoke(new object[] { "~/Content/Events/Events.aspx" })));
                routes.Add(new Route("Events/{date}", (IRouteHandler)ctor.Invoke(new object[] { "~/Content/Events/Events.aspx" })));
            }
            //routes.Add(new Route("Calendar/{date}", new EventPageRouteHandler("~/Content/Faq/Event.aspx")));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            Application.Add("SessionCount", 0);
            // Add Routes.
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            var countSession = (int)Application["SessionCount"];
            if (countSession < 0)
                countSession = 0;
            Application["SessionCount"] = countSession + 1;
            Application.UnLock();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["RunSetup"] == "true")
            {
                if (!(HttpContext.Current.Request.FilePath.Contains("Setup.aspx") || HttpContext.Current.Request.FilePath.Contains("Process.aspx") || HttpContext.Current.Request.FilePath.Contains("App_Themes")))
                    Response.Redirect("~/Setup/Setup.aspx", true);
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            var countSession = (int)Application["SessionCount"];
            if(countSession > 0)
                Application["SessionCount"] = countSession - 1;
            Application.UnLock();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Application.Remove("SessionCount");
        }
    }

}