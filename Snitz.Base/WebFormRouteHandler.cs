﻿using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.Routing;
using System.Web.Compilation;
using System.Security;
using SnitzData;

namespace SnitzCommon
{
	/// <summary>
	/// The IRoutablePage interface allows us to pass the RequestContext to our WebForm
	/// </summary>
	public interface IRoutablePage
	{
		RoutingHelper Routing { get; set; }
	}

	/// <summary>
	/// WebFormRouteHandler - See Phil Haack's post at http://haacked.com/archive/2008/03/11/using-routing-with-webforms.aspx
	/// </summary>
	public class WebFormRouteHandler : IRouteHandler
	{
		public WebFormRouteHandler( string virtualPath )
			: this( virtualPath, true )
		{
		}

		public WebFormRouteHandler( string virtualPath, bool checkPhysicalUrlAccess )
		{
			this.VirtualPath = virtualPath;
			this.CheckPhysicalUrlAccess = checkPhysicalUrlAccess;
		}

		public string VirtualPath { get; private set; }

		public bool CheckPhysicalUrlAccess { get; set; }

		public IHttpHandler GetHttpHandler( RequestContext requestContext )
		{
			if ( this.CheckPhysicalUrlAccess && !UrlAuthorizationModule.CheckUrlAccessForPrincipal(
				this.VirtualPath,
				requestContext.HttpContext.User,
				requestContext.HttpContext.Request.HttpMethod ) )
			{
				throw new SecurityException();
			}

			var page = BuildManager.CreateInstanceFromVirtualPath(
				this.VirtualPath,
				typeof( Page ) ) as IHttpHandler;

			if ( page != null )
			{
				var routablePage = page as IRoutablePage;

				if ( routablePage != null ) routablePage.Routing = new RoutingHelper( requestContext );

				// The WebForm's Form.Action value may be incorrect; override it with the raw URL:
				var webForm = page as Page;
				if ( webForm != null ) webForm.Load += delegate { webForm.Form.Action = requestContext.HttpContext.Request.RawUrl; };
			}

            if(VirtualPath.EndsWith("Topic.aspx"))
            {
                string topicSubject = requestContext.RouteData.GetRequiredString("subject");

                if (topicSubject != "")
                {
                    var topic = Util.FindTopics(topicSubject);
                    if (topic.Count == 1)
                    {
                        int topicId = topic[0].Id;
                        requestContext.HttpContext.Items.Add("TopicId", topicId);
                    }
                    else
                    {
                        requestContext.HttpContext.Items.Add("Subject", topicSubject);
                        return
                            (Page)
                            BuildManager.CreateInstanceFromVirtualPath("~/Content/Forums/Search.aspx", typeof (Page));
                    }
                }
            }
            if(VirtualPath.EndsWith("Profile.aspx"))
            {
                string userName = requestContext.RouteData.GetRequiredString("username");
                if (userName != "")
                {
                    requestContext.HttpContext.Items.Add("user", userName);
                }
            }
            if(VirtualPath.EndsWith("Event.aspx"))
            {
                string date = requestContext.RouteData.GetRequiredString("date");
                if (date != "")
                {
                    requestContext.HttpContext.Items.Add("date", date);
                }                
            }
		    return page;
		}
	}
}