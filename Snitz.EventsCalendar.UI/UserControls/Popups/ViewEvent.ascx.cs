using System;
using SnitzCommon;

namespace EventsCalendar.UserControls.Popups
{
    public partial class ViewEvent : TemplateUserControl
    {
        private int eventid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                eventid = (int) Data;

            }
            DisplayEvent();
        }

        private void DisplayEvent()
        {
            string temp = "";
            string Category = "";

            // fetch todays events from db
            CalEvent calEvent = Util.GetEvent(Convert.ToInt32(eventid));

            switch (calEvent.Type)
            {
                case 1:
                    Category = "Event";
                    break;
                case 2:
                    Category = "Birthday";
                    break;
                case 3:
                    Category = "Anniversary";
                    break;
                case 4:
                    Category = "Holiday";
                    break;
                case 5:
                    Category = "Special";
                    break;
            }
            temp += "Title : <a href=\"edit.aspx?ID=" + calEvent.Id + "\">" + calEvent.Title + "</a>" +
                //"<br/>Date : " + Item.EventDate.ToDateTime() +
                "<br/>" + calEvent.Description +
                "<br/>Event Type : " + Category +
                "<br/>Audience : " + calEvent.Audience +
                "<br/>Added by : " + calEvent.Members.Username + "<br/><br/>";

            daydetail_render.InnerHtml = temp;

        }
    }
}