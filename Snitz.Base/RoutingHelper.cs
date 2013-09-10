/*
####################################################################################################################
##
## SnitzBase - RoutingHelper
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SnitzCommon
{
	public class RoutingHelper
	{
		public RequestContext RequestContext { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RoutingHelper"/> class.
		/// </summary>
		public RoutingHelper()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RoutingHelper"/> class.
		/// </summary>
		/// <param name="requestContext">The request context.</param>
		public RoutingHelper( RequestContext requestContext )
		{
			this.RequestContext = requestContext;
		}

		/// <summary>
		/// Gets a parameterized virtual path.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public string VirtualPath( object values )
		{
			return VirtualPath( null, values );
		}

		/// <summary>
		/// Gets a parameterized virtual path.
		/// </summary>
		/// <param name="routeName">The route name.</param>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public string VirtualPath( string routeName, object values )
		{
			return ( RequestContext != null )
				? RouteTable.Routes.GetVirtualPath( RequestContext, routeName, new RouteValueDictionary( values ) ).VirtualPath
				: null;
		}

		/// <summary>
		/// Returns an HTML anchor.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public string ActionLink( string url )
		{
			return ActionLink( url, url );
		}

		/// <summary>
		/// Returns an HTML anchor.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public string ActionLink( string url, string text )
		{
			return string.Format( "<a href=\"{0}\">{1}</a>", url, text );
		}

		/// <summary>
		/// Gets a route value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public object Value( string key )
		{
			return ( RequestContext != null ) ? RequestContext.RouteData.Values[ key ] : null;
		}

		/// <summary>
		/// Gets the base URL.
		/// </summary>
		/// <value>The base URL.</value>
		public string BaseUrl
		{
			get { return RequestContext.HttpContext.Request.Url.GetLeftPart( UriPartial.Authority ) + VirtualPathUtility.ToAbsolute( "~/" ); }
		}

		/// <summary>
		/// Gets SiteMap path for the current request.
		/// </summary>
		/// <returns></returns>
		public string SiteMapPath()
		{
			var pages = SiteMap.CurrentNode.For( n => n != null, n => n.ParentNode ).Reverse().Select( n => SiteMapLink( n ) );

			return string.Join( " > ", pages.ToArray() );
		}

		/// <summary>
		/// Gets a SiteMap link.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		private string SiteMapLink( SiteMapNode node )
		{
			var span = string.Format( "<span class=\"siteMapLink\">{0}</span>", node.Title );

			return ( node != SiteMap.CurrentNode )
				? string.Format( "<a href=\"{0}\">{1}</a>", node.Url, span )
				: span;
		}
	}
}