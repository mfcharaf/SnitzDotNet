using System;
using System.Web.Security;
using System.Web.UI.WebControls;
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
            FutureEvents.DataSource = Util.GetEvents(DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(60));
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
                var dataItem = (CalEvent)e.Item.DataItem;
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