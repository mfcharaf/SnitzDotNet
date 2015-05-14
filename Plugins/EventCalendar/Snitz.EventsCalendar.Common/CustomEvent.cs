using System;


namespace Snitz.Entities
{
    /// <summary>
    /// A custom or user-defined event
    /// </summary>
    public class CustomEvent : IEvent
    {
        public float EventLengthInHours
        {
            get;
            set;
        }

        public string Title { get; set; }
        public string Audience { get; set; }

        public bool Enabled
        {
            get;
            set;
        }

        public int Type { get; set; }

        public CustomRecurringFrequenciesHandler CustomRecurringFunction
        {
            get;
            set;
        }

        public bool IgnoreTimeComponent
        {
            get;
            set;
        }

        public int Id { get; set; }

        public DateTime Date
        {
            get;
            set;
        }

        public RecurringFrequencies RecurringFrequency
        {
            get;
            set;
        }

        public bool ThisDayForwardOnly
        {
            get;
            set;
        }

        /// <summary>
        /// CustomEvent Constructor
        /// </summary>
        public CustomEvent()
        {
            EventLengthInHours = 1.0f;
            Enabled = true;
            IgnoreTimeComponent = false;
            ThisDayForwardOnly = true;
            RecurringFrequency = RecurringFrequencies.None;
        }

        public IEvent Clone()
        {
            return new CustomEvent
            {
                CustomRecurringFunction = CustomRecurringFunction,
                Date = Date,
                Enabled = Enabled,
                Title = Title,
                IgnoreTimeComponent = IgnoreTimeComponent,
                RecurringFrequency = RecurringFrequency,
                ThisDayForwardOnly = ThisDayForwardOnly,
                EventLengthInHours = EventLengthInHours
            };
        }
    }
}
