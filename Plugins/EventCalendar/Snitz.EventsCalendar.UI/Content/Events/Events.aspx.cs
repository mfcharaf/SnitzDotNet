/*
####################################################################################################################
##
## EventsCalendar.Content.Events - Events.aspx
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
using System.Threading;
using System.Web.Security;
using System.Web.UI.WebControls;
using ModConfig;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace EventsCalendar
{
    public partial class Events : PageBase
    {
        private static List<DateTime> list = new List<DateTime>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ConfigHelper.IsModEnabled("EventsConfig"))
            {
                throw new NotSupportedException("Events Calendar not enabled");
                Response.End();
            }
            if(!IsPostBack)
            {
                if (Request.QueryString["month"] != null)
                    EventCalendar1.SetMonth(Request.QueryString["month"],Request.QueryString["year"]);
                if(Request.QueryString["eventid"] != null)
                {
                    
                }
            }
            if (Resources.webResources.TextDirection == "rtl")
            {
                //lets add the rtl css file
                rtlCss.Text = @"<link rel='stylesheet' type='text/css' runat='server' href='/css/" + Page.Theme + @"/eventsrtl.css' />";
            }
            if(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fa")
            Page.ClientScript.RegisterStartupScript(this.GetType(), "loctime", "$(document).ready(function () {replaceDates();});", true);
            if (Request.QueryString["mode"] != null)
                SetupForm();

        }

        private void SetupForm()
        {

            string mode = Request.QueryString["mode"];
            string id = Request.QueryString["id"];
            DateTime eventdate;

            Array itemValues = System.Enum.GetValues(typeof(Enumerators.EventType));
            Array itemNames = System.Enum.GetNames(typeof(Enumerators.EventType));

            ddlRoles.DataSource = Roles.GetAllRoles();
            ddlRoles.DataBind();

            for (int i = 0; i <= itemNames.Length - 1; i++)
            {
                string name = itemNames.GetValue(i).ToString();
                int value = (int)itemValues.GetValue(i);
                ListItem item = new ListItem(name, value.ToString());
                ddlType.Items.Add(item);
            }
            switch (mode)
            {
                case "edit":
                    EventInfo @event = ForumEvents.GetEvent(Convert.ToInt32(id));
                    eventdate = @event.Date;
                    tbxTitle.Text = @event.Title;
                    tbxDescription.Text = @event.Description;
                    ddlRoles.SelectedValue = @event.Audience;
                    ddlType.SelectedValue = @event.Type.ToString();
                    btnDelEvent.CommandArgument = @event.Id.ToString();
                    btnDelEvent.Visible = true;
                    if (ViewState["newdate"] == null)
                        ViewState["newdate"] = eventdate.ToString("MMddyyyy");
                    break;
                default :
                    if (ViewState["newdate"] == null)
                        ViewState["newdate"] = Request.QueryString["d"];

                    btnDelEvent.Visible = false;
                    eventdate = ViewState["newdate"] != null ? DateTime.ParseExact(ViewState["newdate"].ToString(), "MMddyyyy", null) : DateTime.UtcNow.Date;
                    break;
            }
            
            
            pnlAddEvent.Visible = true;
            pnlCalendar.Visible = false;
            Calendar1.VisibleDate = eventdate;
            Calendar1.SelectedDate = eventdate;
        }

        protected void SubmitEvent(object sender, EventArgs e)
        {
            List<DateTime> newList = (List<DateTime>)Session["SelectedDates"];
            string mode = Request.QueryString["mode"];
            string id = Request.QueryString["id"];

            if(mode=="edit")
            {
                ForumEvents.UpdateEvent(Convert.ToInt32(id),tbxTitle.Text, tbxDescription.Text, Convert.ToInt32(ddlType.SelectedValue),
                                 DateTime.ParseExact(ViewState["newdate"].ToString(), "MMddyyyy", null));
            }
            else
            {
                

            if (newList.Count > 0)
            {
                
                foreach (DateTime dt in newList)
                {
                    ForumEvents.AddEvent(tbxTitle.Text, tbxDescription.Text, Convert.ToInt32(ddlType.SelectedValue),
                                         dt, Member.Id);
                }
            }
            else
                ForumEvents.AddEvent(tbxTitle.Text, tbxDescription.Text, Convert.ToInt32(ddlType.SelectedValue),
                                         DateTime.ParseExact(ViewState["newdate"].ToString(), "MMddyyyy", null), Member.Id);
            }
            Thread.Sleep(500);
            Response.Redirect("\\Content\\Events\\Events.aspx",true);
        }

        protected void NewCal_DayRender(object sender, DayRenderEventArgs e)
        {
            if (e.Day.IsSelected == true)
            {
                if (cbxMultiSelect.Checked)
                    list.Add(e.Day.Date);
            }
            Session["SelectedDates"] = list;

        }

        protected void NewCal_DayChange(object sender, EventArgs e)
        {
            if (cbxMultiSelect.Checked && Session["SelectedDates"] != null)
            {
                List<DateTime> newList = (List<DateTime>)Session["SelectedDates"];
                foreach (DateTime dt in newList)
                {
                    Calendar1.SelectedDates.Add(dt);
                }
                list.Clear();
            }else
            {
                ViewState["newdate"] = Calendar1.SelectedDate.ToString("MMddyyyy");
            }

        }

        protected void MultiSelectChanged(object sender, EventArgs e)
        {
            if (!cbxMultiSelect.Checked)
            {
                Session["SelectedDates"] = null;
                list.Clear();
            }
        }

        protected void btnDelEvent_Click(object sender, EventArgs e)
        {
            ForumEvents.DeleteEvent(Convert.ToInt32(btnDelEvent.CommandArgument));
            Thread.Sleep(500);
            Response.Redirect("\\Content\\Events\\Events.aspx", true);
        }
    }
}