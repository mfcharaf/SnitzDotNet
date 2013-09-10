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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using EventsCalendar;
using Snitz.BLL;
using Snitz.Entities;

namespace EventsCalendar.UserControls
{
    public partial class ExtendedCalendar : UserControl
    {
        private DateTime _todaysDate;
        public DateTime TodaysDate { 
            get { return _todaysDate; }
            set { _todaysDate = value; }
        }
        private Collection<CalDate> CalEvents; 
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


            this.PreRender += new EventHandler(Month_PreRender);
        }

        private void Month_PreRender(object sender, EventArgs e)
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
                    calMonth.TitleStyle.BackColor = Color.Green;
        }

        protected void DayRender(object sender, DayRenderEventArgs e)
        {
            Color BackColour = Color.White;
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

                    switch (Item.Type)
                    {
                        case 1:
                            BackColour = Color.Blue;
                            break;
                        case 2:
                            BackColour = Color.Red;
                            break;
                        case 3:
                            BackColour = Color.Orange;
                            break;
                        case 4:
                            BackColour = Color.Green;
                            break;
                        case 5:
                            BackColour = Color.Gray;
                            break;
                        default:
                            BackColour = Color.Silver;
                            break;
                    }


                    DayTextHasChanged = true;
                    ////Set the flag
                }

            }
            if (DayTextHasChanged)
            {
                e.Cell.BackColor = BackColour;
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
        
        protected void lnk_Click(object sender, EventArgs e)
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
            CalEvents = new Collection<CalDate>();
            foreach (EventInfo calEvent in ForumEvents.GetEvents(start, end))
            {
                CalDate cal = new CalDate { Date = calEvent.Date, Title = calEvent.Title, Type = calEvent.Type };
                CalEvents.Add(cal);
            }

        }

        protected void ShowDay(object sender, EventArgs e)
        {
            OnDaySelected(sender, e);
        }

    }
}