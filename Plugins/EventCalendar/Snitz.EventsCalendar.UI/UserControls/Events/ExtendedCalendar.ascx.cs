/*
####################################################################################################################
##
## EventsCalendar.UserControls.Events - ExtendedCalendar.ascx
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
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;

namespace EventsCalendar.UserControls
{
    public partial class ExtendedCalendar : UserControl
    {
        private DateTime _todaysDate;
        private bool _showholidays;

        public DateTime TodaysDate {
            protected get { return _todaysDate; }
            set { _todaysDate = value; }
        }
        public bool ShowHolidays
        {
            protected get { return _showholidays; }
            set { _showholidays = value; }
        }
        private List<IEvent> _events;
        //private Collection<CalDate> _calEvents; 
        public event EventHandler DaySelected;

        private void OnDaySelected(object sender, EventArgs e)
        {
            if (DaySelected != null)
            {
                DaySelected(sender, e);
            }
        } 


        protected void Page_Load(object sender, EventArgs e)
        {


            this.PreRender += MonthPreRender;
        }

        private void MonthPreRender(object sender, EventArgs e)
        {
            DateTime now = DateTime.Today;
            lnk.Text = _todaysDate.ToString("MMMM");
            int year = Convert.ToInt32(_todaysDate.ToString("yyyy"));
            int month = Convert.ToInt32(_todaysDate.ToString("MM"));
            lnk.CommandName = year.ToString();
            lnk.CommandArgument = month.ToString();
            int days = DateTime.DaysInMonth(year, month);
            GetEvents(_todaysDate, _todaysDate.AddDays(days));
            if(now.Year == _todaysDate.Year)
                if (_todaysDate.Month == now.Month)
                    calMonth.TitleStyle.CssClass = "calender-current-month";
        }

        protected void DayRender(object sender, DayRenderEventArgs e)
        {
            var dt = new DateTime(e.Day.Date.Year, e.Day.Date.Month, e.Day.Date.Day, 23, 59, 0);
            List<IEvent> evnts = _events.Where(evnt => ForumEvents.NeedsRendering(evnt, dt)).ToList().OrderBy(d => d.Date).ToList();
            
            string headerstyle = "";
            bool dayTextHasChanged = false;
            DateTime dayHold = DateTime.MinValue;
            string tooltip = "";

            foreach (var item in evnts)
            {
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
                        headerstyle = " cal-head-event";
                        break;
                    case 2:
                        headerstyle = " cal-head-birthday";
                        break;
                    case 3:
                        headerstyle = " cal-head-anniversary";
                        break;
                    case 4:
                        headerstyle = " cal-head-holiday";
                        break;
                    case 5:
                        headerstyle = " cal-head-special";
                        break;
                    default:
                        headerstyle = " cal-head";
                        break;
                }

                tooltip += item.Title;

                dayTextHasChanged = true;
                ////Set the flag

            }

            if (dayTextHasChanged && !e.Cell.CssClass.Contains("calendar-prevmonth-day"))
            {
                e.Cell.CssClass += headerstyle;
                e.Cell.ToolTip = tooltip;
                //e.Cell.BackColor = BackColour;
            }
            if (e.Day.Date.DayOfYear == DateTime.UtcNow.DayOfYear)
            {
                e.Cell.CssClass += " cal-head-today";
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // turn user control to html code
            string output = RenderToString(calMonth);
            string monthselect = GetControlHtml();
            Regex regNextMonth = new Regex(
                String.Format("{0}",calMonth.TodaysDate.ToString("MMMM")),
                RegexOptions.IgnoreCase
                | RegexOptions.Singleline
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                );
            output = regNextMonth.Replace(output, monthselect, 1);
            // output the modified code
            writer.Write(output);
        }
        
        protected void LnkClick(object sender, EventArgs e)
        {
            LinkButton monthlink = sender as LinkButton;
            int year;
            int month;
            if (_todaysDate.Year != 1)
            {
                year = Convert.ToInt32(_todaysDate.ToString("yyyy"));
                month = Convert.ToInt32(_todaysDate.ToString("MM"));
            }
            else
            {
                year = Convert.ToInt32(monthlink.CommandName);
                month = Convert.ToInt32(monthlink.CommandArgument);
            }

            if (monthlink != null) Response.Redirect("\\Content\\Events\\Events.aspx?month=" + month + "&year=" + year);
        }

        private string GetControlHtml()
        {

            return RenderToString(lnk) + " ";
        }

        private static string RenderToString(Control c)
        {
            bool previousVisibility = c.Visible;
            c.Visible = true; // make visible if not

            // get html code for control
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter localWriter = new HtmlTextWriter(sw);
            c.RenderControl(localWriter);
            string output = sw.ToString();

            // restore visibility
            c.Visible = previousVisibility;

            return output;
        }

        private void GetEvents(DateTime start, DateTime end)
        {
            _events = ForumEvents.GetEvents(start, end);
            if(_showholidays)
                _events.AddRange(ForumEvents.PresetHolidays());
        }

        protected void ShowDay(object sender, EventArgs e)
        {
            OnDaySelected(sender, e);
        }

    }
}