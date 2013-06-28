using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using SnitzCommon;

namespace EventsCalendar
{
    public static class Util
    {

        public static List<CalEvent> GetEvents(DateTime start, DateTime end)
        {
            using(EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                var events = (from cal in dc.CalEvents where cal.EventDate.CompareTo(start.ToForumDateStr())>=0 select cal) ;
                events = (from c in events where c.EventDate.CompareTo(end.ToForumDateStr()) <= 0 select c);
                return events.OrderBy(e=>e.EventDate).ToList();
            }
        }

        public static List<CalEvent> GetEventsForToday(string today)
        {
            using(EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                dc.DeferredLoadingEnabled = false;    
                DataLoadOptions loadOptions = new DataLoadOptions();    
                loadOptions.LoadWith<CalEvent>(t => t.Members);    
                dc.LoadOptions = loadOptions;
                return (from events in dc.CalEvents where events.EventDate == today select events).ToList();
            }
        }

        public static void AddEvent(string title, string description, int type, DateTime eventDate, int userid)
        {
            using(EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                CalEvent @event = new CalEvent();
                @event.Title = title;
                @event.Description = description;
                @event.EventDate = eventDate.ToForumDateStr();
                @event.Author = userid;
                @event.Type = type;
                dc.CalEvents.InsertOnSubmit(@event);
                dc.SubmitChanges();

            }
        }

        public static CalEvent GetEvent(int id)
        {
            using (EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                dc.DeferredLoadingEnabled = false;
                DataLoadOptions loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<CalEvent>(t => t.Members);
                dc.LoadOptions = loadOptions;
                return (from e in dc.CalEvents where e.Id == id select e).Single();
            }
        }

        public static void DeleteEvent(int id)
        {
            using (EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                var ev = from e in dc.CalEvents where e.Id == id select e ;
                dc.CalEvents.DeleteAllOnSubmit(ev);
                dc.SubmitChanges();
            }
        }

        public static void UpdateEvent(int id, string title, string description, int type, DateTime eventDate)
        {
            using (EventsCalendarDataContext dc = new EventsCalendarDataContext())
            {
                CalEvent @event = (from e in dc.CalEvents where e.Id == id select e).Single();
                @event.Title = title;
                @event.Description = description;
                @event.EventDate = eventDate.ToForumDateStr();
                @event.Type = type;
                dc.SubmitChanges();

            }
        }
    }
}
