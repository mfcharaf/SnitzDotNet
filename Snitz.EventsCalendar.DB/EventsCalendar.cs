using System.Configuration;

namespace EventsCalendar
{
    partial class EventsCalendarDataContext
    {
        public EventsCalendarDataContext()
        : base(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString, mappingSource)
        {
            OnCreated();
        }
    }
}
