using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;
using SnitzCommon;

namespace EventsCalendar
{
    public class EventPageRouteHandler : IRouteHandler
    {
		public EventPageRouteHandler( string virtualPath )
			: this( virtualPath, true )
		{
		}

        public EventPageRouteHandler(string virtualPath, bool checkPhysicalUrlAccess)
		{
			this.VirtualPath = virtualPath;
			this.CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
		}

		public string VirtualPath { get; private set; }

		public bool CheckPhysicalUrlAccess { get; set; }
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (this.CheckPhysicalUrlAccess && !UrlAuthorizationModule.CheckUrlAccessForPrincipal(
                this.VirtualPath,
                requestContext.HttpContext.User,
                requestContext.HttpContext.Request.HttpMethod))
            {
                throw new SecurityException();
            }

            var page = BuildManager.CreateInstanceFromVirtualPath(
                this.VirtualPath,
                typeof(Page)) as IHttpHandler;

            if (page != null)
            {
                var routablePage = page as IRoutablePage;

                if (routablePage != null) routablePage.Routing = new RoutingHelper(requestContext);

                // The WebForm's Form.Action value may be incorrect; override it with the raw URL:
                var webForm = page as Page;
                if (webForm != null) webForm.Load += delegate { webForm.Form.Action = requestContext.HttpContext.Request.RawUrl; };
            }

            try
            {
                string date = requestContext.RouteData.GetRequiredString("date");
            }
            catch (InvalidOperationException)
            {

            }

            return page;
        }
    }
}