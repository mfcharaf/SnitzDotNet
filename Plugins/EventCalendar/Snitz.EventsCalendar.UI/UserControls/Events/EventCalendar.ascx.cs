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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using Calendar = System.Web.UI.WebControls.Calendar;

namespace EventsCalendar.UserControls
{
    public class calMonths
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
        
        private Collection<CalDate> CalEvents;
        private DateTime _tempDate;
        private int _currentYear
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

        private static bool _eventAdmin
        {
            get
            { return Config.EventAdminRoles.Any(Roles.IsUserInRole); }
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

                        AddEvent_Click(date);
                    }
                }
                if (Request.Params.AllKeys.Contains(ddlYear.UniqueID))
                {
                    string selected = Request.Params[ddlYear.UniqueID];
                    if (ddlYear.SelectedIndex == 0)
                    {
                        ddlYear.SelectedValue = selected;
                        ddlYear_SelectedIndexChanged(sender, e);
                    }
                }
            } 
            if(_tempDate == DateTime.MinValue)
                _tempDate = DateTime.Today;
            GetEvents(_tempDate.AddDays(-40),_tempDate.AddDays(40));
            
            if (!IsPostBack)
            {
                BindYearlyCal();
                ddlYear.SelectedValue = _currentYear.ToString();
            }
            pnlButtons.Visible = calMonth.Visible;
            pnlYearPick.Visible = calYear.Visible;
        }

        private void AddEvent_Click(string dt)
        {
            Response.Redirect("\\Content\\Events\\Events.aspx?mode=new&d=" + dt);
        }

        private void BindYearlyCal()
        {

            string[] localizedMonths = Thread.CurrentThread.CurrentCulture.DateTimeFormat.MonthNames;

            List<calMonths> year = new List<calMonths>();
            for (int i = 0; i < localizedMonths.Length-1; i++)
            {
                DateTime date = new DateTime(_currentYear, i + 1, 1, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);
                calMonths month = new calMonths {Date = date};
                year.Add(month);
            }
            rptMonths.DataSource = year;
            rptMonths.DataBind();
        }

        private void GetEvents(DateTime start, DateTime end)
        {
            CalEvents = new Collection<CalDate>();
            foreach (EventInfo calEvent in ForumEvents.GetEvents(start,end))
            {
                
                CalDate cal = new CalDate { Id= calEvent.Id, Date = calEvent.Date, Title = calEvent.Title, Type = calEvent.Type };
                if ( String.IsNullOrEmpty(calEvent.Audience) || calEvent.Audience == "All" || Roles.IsUserInRole(calEvent.Audience) || Roles.IsUserInRole("Administrator"))
                CalEvents.Add(cal);                
            }

        }

        protected void Calendar1_DayRender(object sender, DayRenderEventArgs e)
        {
            StringBuilder temp = new StringBuilder(); 
            bool DayTextHasChanged = false;
            DateTime DayHold = DateTime.MinValue;

            foreach (CalDate Item in CalEvents)
            {
                if (DayHold != Item.Date)
                {
                    if (DayTextHasChanged)
                    {
                        break; 
                    }
                    DayHold = Item.Date;
                }

                if (e.Day.Date.DayOfYear == Item.Date.DayOfYear)
                {
                    string TextColour;
                    switch (Item.Type)
                    {
                        case 1:
                            TextColour = "Blue";
                            break;
                        case 2:
                            TextColour = "Red";
                            break;
                        case 3:
                            TextColour = "Orange";
                            break;
                        case 4:
                            TextColour = "Green";
                            break;
                        case 5:
                            TextColour = "Gray";
                            break;
                        default:
                            TextColour = "Black";
                            break;
                    }
                  
                    if(_eventAdmin)
                        temp.AppendFormat("<br><a href=\"/Content/Events/Events.aspx?mode=edit&id={0}\" title=\"Edit event: {1}\">{1}</a>", Item.Id, Item.Title);
                    else
                        temp.AppendFormat("<span style=\"font-size:12px; color:{0}\" ><br>{1}</span>", TextColour, Item.Title);

                    DayTextHasChanged = true;
                    ////Set the flag
                }

            }
            if (Calendar1.SkinID.ToLower().Contains("lge") && _eventAdmin)
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
            if (DayTextHasChanged)
            {
                e.Cell.BackColor = Color.Silver;
                if(e.Day.IsOtherMonth)
                    e.Cell.BackColor = ColorTranslator.FromHtml("#F2F2F2");// Color.#F2F2F2
                if (Calendar1.SkinID.ToLower().Contains("lge"))
                {
                    e.Cell.Controls.Add(new LiteralControl(temp.ToString()));
                }
            }

        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            string temp = "No Events for today";
            string Category = "";
            Calendar currentCal = sender as Calendar;
            if (currentCal != null) _tempDate = currentCal.SelectedDate;
            // fetch todays events from db
            IEnumerable<EventInfo> TodaysEvents = GetEventsForToday(_tempDate);
            if (TodaysEvents.Count() > 0)
                temp = "";
            foreach (EventInfo Item in TodaysEvents)
            {
                Item.Author = Members.GetAuthor(Item.MemberId);
                switch (Item.Type)
                {
			        case 1 :
                        Category = "Event";
                        break;
			        case 2 :
                        Category = "Birthday";
                        break;
			        case 3 :
                        Category = "Anniversary";
                        break;
			        case 4 :
                        Category = "Holiday";
                        break;
                    case 5 :
                        Category = "Special";
                        break;
                }
                temp += "Title : <a href=\"edit.aspx?ID=" + Item.Id + "\">" + Item.Title + "</a>" + 
                    //"<br/>Date : " + Item.EventDate.ToDateTime() +
                    "<br/>" + Item.Description +
                    "<br/>Event Type : " + Category + 
                    "<br/>Audience : " + Item.Audience +
                    "<br/>Added by : " + Item.Author.Username + "<br/><br/>";

            }

 
            daydetail_render.InnerHtml = temp;
            if (currentCal != null)
                selectedday.InnerHtml = Common.TranslateNumerals(currentCal.SelectedDate.ToString("MMM d, yyyy"));

            mpeModal.Show();
            GetEvents(_tempDate.AddDays(-40), _tempDate.AddDays(40));

        }

        private static IEnumerable<EventInfo> GetEventsForToday(DateTime date)
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
            _tempDate = new DateTime(_currentYear, 1, 1);
            ddlYear.SelectedValue = _currentYear.ToString();
            BindYearlyCal();
        }

        public void SetMonth(string monthnum, string year)
        {
            calYear.Visible = false;
            calMonth.Visible = true;
            _currentYear = String.IsNullOrEmpty(year) ? 2012 : Convert.ToInt32(year);
            _tempDate = new DateTime(_currentYear, Convert.ToInt32(monthnum), 1, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);
            
            GetEvents(_tempDate.AddDays(-10), _tempDate.AddDays(40));
            Calendar1.TodaysDate = _tempDate;
            ddlYear.SelectedValue = _currentYear.ToString();
            pnlButtons.Visible = calMonth.Visible;
            pnlYearPick.Visible = false;

        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentYear = Convert.ToInt32(ddlYear.SelectedValue);
            _tempDate = new DateTime(_currentYear,1,1);
            BindYearlyCal();
            pnlYearPick.Visible = true;
            pnlButtons.Visible = false;
        }

        protected void rptMonths_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ExtendedCalendar cal = e.Item.FindControl("ExtendedCalendar1") as ExtendedCalendar;
                if (cal != null)
                    cal.DaySelected += Calendar1_SelectionChanged;
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