using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnitzCommon;

namespace EventsCalendar
{
    public partial class CalEvent
    {
        public DateTime Date { get { return this.EventDate.ToDateTime().Value; } } 
    }
}
