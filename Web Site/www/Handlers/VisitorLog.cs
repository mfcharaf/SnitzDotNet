#region Using

using System;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using System.Collections.ObjectModel;

#endregion

public class VisitorLog : IHttpModule, IRequiresSessionState
{

	#region IHttpModule Members

	/// <summary>
	/// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
	/// </summary>
	public void Dispose()
	{
		// Nothing to dispose
	}

	/// <summary>
	/// Initializes a module and prepares it to handle requests.
	/// </summary>
	/// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, 
	/// properties, and events common to all application objects within an ASP.NET application</param>
	public void Init(HttpApplication context)
	{
		context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
		SessionStateModule session = (SessionStateModule)context.Modules["Session"];
		session.End += new EventHandler(session_End);
		session.Start += new EventHandler(session_Start);
	}

	#endregion

	/// <summary>
	/// Handles the Start event of the session control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void session_Start(object sender, EventArgs e)
	{
		HttpContext context = HttpContext.Current;
		Visit visit = new Visit();
		visit.UserAgent = context.Request.UserAgent;
		visit.IpAddress = context.Request.UserHostAddress;
		context.Session.Add("visit", visit);
	}

	/// <summary>
	/// Handles the PostRequestHandlerExecute event of the context control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void context_PostRequestHandlerExecute(object sender, EventArgs e)
	{
		HttpContext context = ((HttpApplication)sender).Context;

		if (context.CurrentHandler is Page)
		{
			Visit visit = context.Session["visit"] as Visit;
			if (visit != null)
			{
				VisitAction action = new VisitAction();
				action.Url = context.Request.Url;
				action.Type = "pageview";
				visit.Action.Add(action);
			}
		}
	}

	/// <summary>
	/// Handles the End event of the session control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	void session_End(object sender, EventArgs e)
	{
		HttpContext context = HttpContext.Current;
		Visit visit = context.Session["visit"] as Visit;
		if (visit != null)
		{
			// Log the Visit object to a database 
		}
	}

	public static void AddAction(string message, string type)
	{
		HttpContext context = HttpContext.Current;
		Visit visit = context.Session["visit"] as Visit;
		if (visit != null)
		{
			VisitAction action = new VisitAction();
			action.Url = new Uri(HttpUtility.UrlEncode(message), UriKind.Relative);
			action.Type = type;
			visit.Action.Add(action);
		}
	}
}

public class Visit
{
	public string UserAgent;
	public string IpAddress;
	public Collection<VisitAction> Action = new Collection<VisitAction>();
}

public class VisitAction
{
	public Uri Url;
	public DateTime Date = DateTime.UtcNow;
	public string Type;
}