/*
####################################################################################################################
##
## EventsCalendar.UserControls.Events - EventCalendar.ascx
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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using ModConfig;
using Snitz.BLL;
using Snitz.BLL.modconfig;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using Calendar = System.Web.UI.WebControls.Calendar;

namespace EventsCalendar.UserControls
{
    public class CalMonths
    {
        public DateTime Date { get; set; }
    }

    public struct CalDate
    {
        public string Title;
        public DateTime Date;
        public int Type;

        public int Id;

    }
    
    public partial class EventCalendar : UserControl
    {
        private List<IEvent> _events;
        //private Collection<CalDate> _calEvents;
        private DateTime _tempDate;
        private bool _showholidays;

        private int CurrentYear
        {
            get
            {
                if (ViewState["CalYear"] != null)
                    return Convert.ToInt32(ViewState["CalYear"]);
                string stryear = DateTime.Today.ToString("yyyy");
                int year = Convert.ToInt32(stryear);
                return year;
            }
            set
            {
                if (ViewState["CalYear"] != null)
                    ViewState["CalYear"] = value;
                else
                {
                    ViewState.Add("CalYear", value);
                }
            }
        }
        public bool ShowHolidays
        {
            protected get { return _showholidays; }
            set { _showholidays = value; }
        }
        private static bool EventAdmin
        {
            get
            {
                var modcontroler = new ModController("Events");
                var allowedRoles = modcontroler.ModInfo.Settings["EventAdminRoles"] != null
                    ? modcontroler.ModInfo.Settings["EventAdminRoles"].ToString()
                    : String.Empty;
                var dsRoles = allowedRoles.Split(',').ToList();
                return dsRoles.Any(Roles.IsUserInRole);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ddlYear.Items.Clear();
            string stryear = DateTime.Today.ToString("yyyy");
            int year = Convert.ToInt32(stryear) - 1;

            for (int i = 0; i < 6; i++)
            {
                ListItem li = new ListItem { Text = Common.TranslateNumerals(year + i), Value = (year + i).ToString() };
                ddlYear.Items.Add(li);
            }

            if (IsPostBack)
            {
                foreach (string key in Request.Form.AllKeys)
                {
                    if(key != null)
                    if (key.StartsWith("Date"))
                    {
                        string date = key.Substring(4, 8);

                        AddEventClick(date);
                    }
                }
                if (Request.Params.AllKeys.Contains(ddlYear.UniqueID))
                {
                    string selected = Request.Params[ddlYear.UniqueID];
                    if (ddlYear.SelectedIndex == 0)
                    {
                        ddlYear.SelectedValue = selected;
                        DdlYearSelectedIndexChanged(sender, e);
                    }
                }
            } 
            if(_tempDate == DateTime.MinValue)
                _tempDate = DateTime.Today;
            GetEvents(_tempDate.AddDays(-40),_tempDate.AddDays(40));
            
            if (!IsPostBack)
            {
                BindYearlyCal();
                ddlYear.SelectedValue = CurrentYear.ToString();
            }
            pnlButtons.Visible = calMonth.Visible;
            pnlYearPick.Visible = calYear.Visible;
        }

        private void AddEventClick(string dt)
        {
            Response.Redirect("\\Content\\Events\\Events.aspx?mode=new&d=" + dt);
        }

        private void BindYearlyCal()
        {

            string[] localizedMonths = Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames;

            List<CalMonths> year = new List<CalMonths>();
            for (int i = 0; i < localizedMonths.Length-1; i++)
            {
                DateTime date = new DateTime(CurrentYear, i + 1, 1, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);
                CalMonths month = new CalMonths();
                month.Date = date;
                year.Add(month);
            }
            rptMonths.DataSource = year;
            rptMonths.DataBind();
        }

        private void GetEvents(DateTime start, DateTime end)
        {
            _events = ForumEvents.GetEvents(start, end);
            if (_showholidays)
                _events.AddRange(ForumEvents.PresetHolidays());
        }

        protected void Calendar1DayRender(object sender, DayRenderEventArgs e)
        {
            var dt = new DateTime(e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day, 23, 59, 0);
            List<IEvent> evnts = _events.Where(evnt => ForumEvents.NeedsRendering(evnt, dt)).ToList().OrderBy(d => d.Date).ToList();

            StringBuilder temp = new StringBuilder(); 
            bool dayTextHasChanged = false;
            DateTime dayHold = DateTime.MinValue;

            foreach (var item in evnts)
            {
                string headerstyle;
                if (dayHold != item.Date)
                {
                    if (dayTextHasChanged)
                    {
                        break; 
                    }
                    dayHold = item.Date;
                }

                switch (item.Type)
                {
                    case 1:
                        headerstyle = "cal-head-event";
                        break;
                    case 2:
                        headerstyle = "cal-head-birthday";
                        break;
                    case 3:
                        headerstyle = "cal-head-anniversary";
                        break;
                    case 4:
                        headerstyle = "cal-head-holiday";
                        break;
                    case 5:
                        headerstyle = "cal-head-special";
                        break;
                    default:
                        headerstyle = "cal-head";
                        break;
                }

                if (EventAdmin)
                    temp.AppendFormat("<br><a href=\"/Content/Events/Events.aspx?mode=edit&id={0}\" title=\"Edit event: {1}\">{1}</a>", item.Id, item.Title);
                else
                    temp.AppendFormat("<span class=\"{0}\" ><br>{1}</span>", headerstyle, item.Title);

                dayTextHasChanged = true;
                ////Set the flag

            }

            if (Calendar1.SkinID.ToLower().Contains("lge") && EventAdmin)
            {
                ImageButton btn = new ImageButton
                                      {
                                          ImageUrl = Config.ImageDirectory + "admin/calendar.png",
                                          ID = "Date" + e.Day.Date.ToString("MMddyyyy"),
                                          ToolTip = "Add Event"
                                      };
                //btn.ApplyStyleSheetSkin(Page);
                e.Cell.Controls.Add(btn);
            }            
            if (dayTextHasChanged)
            {
                if (Calendar1.SkinID.ToLower().Contains("lge"))
                {
                    e.Cell.Controls.Add(new LiteralControl(temp.ToString()));
                }
            }

        }

        protected void Calendar1SelectionChanged(object sender, EventArgs e)
        {
            string temp = "No Events for today";
            string category = "";
            Calendar currentCal = sender as Calendar;
            if (currentCal != null) _tempDate = currentCal.SelectedDate;
            // fetch todays events from db
            List<IEvent> todaysEvents = GetEventsForToday(_tempDate).ToList();
            if (todaysEvents.Any())
                temp = "";
            foreach (EventInfo item in todaysEvents)
            {
                item.Author = Members.GetAuthor(item.MemberId);
                switch (item.Type)
                {
			        case 1 :
                        category = "Event";
                        break;
			        case 2 :
                        category = "Birthday";
                        break;
			        case 3 :
                        category = "Anniversary";
                        break;
			        case 4 :
                        category = "Holiday";
                        break;
                    case 5 :
                        category = "Special";
                        break;
                }
                temp += "Title : <a href=\"edit.aspx?ID=" + item.Id + "\">" + item.Title + "</a>" + 
                    //"<br/>Date : " + Item.EventDate.ToDateTime() +
                    "<br/>" + item.Description +
                    "<br/>Event Type : " + category + 
                    "<br/>Audience : " + item.Audience +
                    "<br/>Added by : " + item.Author.Username + "<br/><br/>";

            }

 
            daydetail_render.InnerHtml = temp;
            if (currentCal != null)
                selectedday.InnerHtml = Common.TranslateNumerals(currentCal.SelectedDate.ToString("MMM d, yyyy"));

            mpeModal.Show();
            GetEvents(_tempDate.AddDays(-40), _tempDate.AddDays(40));

        }

        private static IEnumerable<IEvent> GetEventsForToday(DateTime date)
        {

            return ForumEvents.GetEventsForToday(date.ToForumDateStr());
        }

        protected void MonthChanged(object sender, MonthChangedEventArgs e)
        {
            _tempDate = e.NewDate;
            GetEvents(_tempDate.AddDays(-40), _tempDate.AddDays(40));
        }

        protected void ChangeView(object sender, EventArgs e)
        {
            calYear.Visible = true;
            calMonth.Visible = false;
            pnlButtons.Visible = false;
            pnlYearPick.Visible = true;
            _tempDate = new DateTime(CurrentYear, 1, 1);
            ddlYear.SelectedValue = CurrentYear.ToString();
            BindYearlyCal();
        }

        public void SetMonth(string monthnum, string year)
        {
            calYear.Visible = false;
            calMonth.Visible = true;
            CurrentYear = String.IsNullOrEmpty(year) ? 2012 : Convert.ToInt32(year);
            _tempDate = new DateTime(CurrentYear, Convert.ToInt32(monthnum), 1, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);
            
            GetEvents(_tempDate.AddDays(-10), _tempDate.AddDays(40));
            Calendar1.TodaysDate = _tempDate;
            ddlYear.SelectedValue = CurrentYear.ToString();
            pnlButtons.Visible = calMonth.Visible;
            pnlYearPick.Visible = false;

        }

        protected void DdlYearSelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentYear = Convert.ToInt32(ddlYear.SelectedValue);
            _tempDate = new DateTime(CurrentYear,1,1);
            BindYearlyCal();
            pnlYearPick.Visible = true;
            pnlButtons.Visible = false;
        }

        protected void RptMonthsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ExtendedCalendar cal = e.Item.FindControl("ExtendedCalendar1") as ExtendedCalendar;
                
                var test = (CalMonths)e.Item.DataItem;
                if(test.Date.Month == DateTime.UtcNow.Month)
                    cal.TodaysDate = DateTime.UtcNow.Date;
                if (cal != null)
                {
                    cal.ShowHolidays = ConfigHelper.GetBoolValue("EventsConfig", "EventShowHolidays");
                    cal.DaySelected += Calendar1SelectionChanged;
                }
            }
            if (e.Item.ItemType == ListItemType.Separator)
            {
                if ((e.Item.ItemIndex + 1) % 3 != 0)
                {
                    e.Item.Controls.Clear();
                }
            }
        }
    }
}