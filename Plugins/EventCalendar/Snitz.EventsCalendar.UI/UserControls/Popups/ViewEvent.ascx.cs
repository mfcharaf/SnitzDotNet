/*
####################################################################################################################
##
## EventsCalendar.UserControls.Popups - ViewEvent.ascx
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
using ModConfig;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace EventsCalendar.UserControls.Popups
{
    public partial class ViewEvent : TemplateUserControl
    {
        private int eventid;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ConfigHelper.IsModEnabled("EventsConfig"))
                return;

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
            EventInfo calEvent = ForumEvents.GetEvent(Convert.ToInt32(eventid));

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
                "<br/>Added by : " + calEvent.Author.Username + "<br/><br/>";

            daydetail_render.InnerHtml = temp;

        }
    }
}