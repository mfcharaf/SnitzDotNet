/*
####################################################################################################################
##
## Snitz.BLL - ForumEvents
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
using Snitz.Entities;
using Snitz.EventsCalendar.IDAL;
using Snitz.IDAL;


namespace Snitz.BLL
{
    public static partial class ForumEvents
    {

        public static List<IEvent> GetEvents(DateTime start, DateTime end)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return new List<IEvent>(dal.GetEvents(start.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss")));

        }

        public static List<IEvent> GetEventsForToday(string today)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return new List<IEvent>(dal.GetEvents(today, null));
        }

        public static void AddEvent(string title, string description, int type, DateTime eventDate, int userid)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            EventInfo forumevent = new EventInfo
                                       {
                                           Title = title,
                                           Description = description,
                                           Type = type,
                                           Date = eventDate,
                                           MemberId = userid
                                       };
            dal.Add(forumevent);
        }

        public static EventInfo GetEvent(int id)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return dal.GetById(id);
        }

        public static void DeleteEvent(int id)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            dal.Delete(GetEvent(id));
        }

        public static void UpdateEvent(int id, string title, string description, int type, DateTime eventDate)
        {
            EventInfo forumevent = GetEvent(id);
            forumevent.Title = title;
            forumevent.Description = description;
            forumevent.Type = type;
            forumevent.Date = eventDate;
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            dal.Update(forumevent);
        }

        private static bool DayForward(IEvent evnt, DateTime day)
        {
            if (evnt.ThisDayForwardOnly)
            {
                int c = DateTime.Compare(day, evnt.Date);

                if (c >= 0)
                    return true;

                return false;
            }

            return true;
        }
        public static bool NeedsRendering(IEvent evnt, DateTime day)
        {
            if (!evnt.Enabled)//&& !_showDisabledEvents)
                return false;

            DayOfWeek dw = evnt.Date.DayOfWeek;

            if (evnt.RecurringFrequency == RecurringFrequencies.Daily)
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.Weekly && day.DayOfWeek == dw)
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryWeekend && (day.DayOfWeek == DayOfWeek.Saturday ||
                day.DayOfWeek == DayOfWeek.Sunday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryMonWedFri && (day.DayOfWeek == DayOfWeek.Monday ||
                day.DayOfWeek == DayOfWeek.Wednesday || day.DayOfWeek == DayOfWeek.Friday))
            {
                return DayForward(evnt, day);
            }
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryTueThurs && (day.DayOfWeek == DayOfWeek.Thursday ||
                day.DayOfWeek == DayOfWeek.Tuesday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.EveryWeekday && (day.DayOfWeek != DayOfWeek.Sunday &&
                day.DayOfWeek != DayOfWeek.Saturday))
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Yearly && evnt.Date.Month == day.Month &&
                evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Monthly && evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            if (evnt.RecurringFrequency == RecurringFrequencies.Custom && evnt.CustomRecurringFunction != null)
            {
                if (evnt.CustomRecurringFunction(evnt, day))
                    return DayForward(evnt, day);
                return false;
            }

            if (evnt.RecurringFrequency == RecurringFrequencies.None && evnt.Date.Year == day.Year &&
                evnt.Date.Month == day.Month && evnt.Date.Day == day.Day)
                return DayForward(evnt, day);
            return false;
        }

    }
}
