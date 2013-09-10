/*
####################################################################################################################
##
## EventsCalendar.UserControls.Events - UpcomingEvents.ascx
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
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;


namespace EventsCalendar.UserControls
{
    public partial class UpcomingEvents : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

                BindUpcomingEvents();

        }

        private void BindUpcomingEvents()
        {
            FutureEvents.DataSource = ForumEvents.GetEvents(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(60));
            FutureEvents.DataBind();
            this.Visible = FutureEvents.Items.Count > 0;
        }

        //protected void EventsBound(object sender, DataListItemEventArgs e)
        //{
        //    PageBase page = (PageBase) Page;
        //    if(e.Item.ItemType == ListItemType.Header)
        //    {
        //        HyperLink add = e.Item.FindControl("lnkAdd") as HyperLink;
        //        if (add != null)
        //        {
        //            add.Visible = Roles.IsUserInRole("Administrator");
        //        }
        //    }
        //    if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        var dataItem = (CalEvent)e.Item.DataItem;
        //        Literal view = e.Item.FindControl("viewEvent") as Literal;
        //        if (view != null)
        //            view.Text = string.Format(
        //                "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('View Event',{{'pageID':11,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
        //                dataItem.Id, dataItem.Title, dataItem.Title);
        //        Label date = e.Item.FindControl("lblDate") as Label;
        //        if(date != null)
        //        {
                    

        //            date.Text = dataItem.Date.ToForumDateDisplay(" ",false,page.IsAuthenticated,0);
        //        }
        //    }
        //}

        protected void rptEventBound(object sender, RepeaterItemEventArgs e)
        {
            PageBase page = (PageBase)Page;
            if (e.Item.ItemType == ListItemType.Header)
            {
                HyperLink add = e.Item.FindControl("lnkAdd") as HyperLink;
                if (add != null)
                {
                    add.Visible = Roles.IsUserInRole("Administrator");
                }
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (EventInfo)e.Item.DataItem;
                Literal view = e.Item.FindControl("viewEvent") as Literal;
                if (view != null)
                    view.Text = string.Format(
                        "<a href=\"#\" onclick=\"mainScreen.LoadServerControlHtml('View Event',{{'pageID':11,'data':{0}}}, 'methodHandlers.BeginRecieve');\" title=\"[!{1}!]\">{2}</a>",
                        dataItem.Id, dataItem.Title, dataItem.Title);
                Label date = e.Item.FindControl("lblDate") as Label;
                if (date != null)
                {


                    date.Text = dataItem.Date.ToForumDateDisplay(" ", false, page.IsAuthenticated, 0);
                }
            }
        }
    }
}