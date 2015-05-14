using System;


namespace Snitz.Entities
{
    /// <summary>
    /// An event that defines a holiday
    /// </summary>
    public class HolidayEvent : IEvent
    {

        /// <summary>
        /// HolidayEvent Constructor
        /// </summary>
        public HolidayEvent()
        {
            Enabled = true;
            ThisDayForwardOnly = false;
            RecurringFrequency = RecurringFrequencies.None;
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Enabled { get; set; }
        public int Type { get; set; }
        public RecurringFrequencies RecurringFrequency { get; set; }
        public string Title { get; set; }
        public string Audience { get; set; }
        public bool ThisDayForwardOnly { get; set; }
        public CustomRecurringFrequenciesHandler CustomRecurringFunction { get; set; }

        public IEvent Clone()
        {
            return new HolidayEvent
            {
                CustomRecurringFunction = CustomRecurringFunction,
                Date = Date,
                Enabled = Enabled,
                Title = Title,
                Audience = Audience,
                RecurringFrequency = RecurringFrequency,
                ThisDayForwardOnly = ThisDayForwardOnly
            };
        }
    }
}
