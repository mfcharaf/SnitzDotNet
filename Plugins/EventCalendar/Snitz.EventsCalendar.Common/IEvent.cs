using System;

namespace Snitz.Entities
{
    /// <summary>
    /// An enumeration of built-in recurring event frequencies
    /// </summary>
    public enum RecurringFrequencies
    {
        /// <summary>
        /// Indicates that the event is non recurring will occur only one time
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that the event will occur every day
        /// </summary>
        Daily = 1,
        /// <summary>
        /// Indicates that the event will occur every week day (Mon - Fri)
        /// </summary>
        EveryWeekday = 2,
        /// <summary>
        /// Indicates that the event will occur every Mon, Wed and Fri
        /// </summary>
        EveryMonWedFri = 3,
        /// <summary>
        /// Indicates that the event will occur every Tuesday and Thursday
        /// </summary>
        EveryTueThurs = 4,
        /// <summary>
        /// Indicates that the event will occur every week
        /// </summary>
        Weekly = 5,
        /// <summary>
        /// Indicates that the event will occur every month
        /// </summary>
        Monthly = 6,
        /// <summary>
        /// Indicates that the event will occur once a year, on the month and day specified
        /// </summary>
        Yearly = 7,
        /// <summary>
        /// Indicates that the event will occur every weekend on Saturday and Sunday
        /// </summary>
        EveryWeekend = 8,
        /// <summary>
        /// Indicates that the recuring schedule of this event is unique
        /// </summary>
        Custom = 99
    }

    /// <summary>
    /// An interface for creating event types
    /// </summary>
    public interface IEvent
    {
        int Id { get; set; }
        /// <summary>
        /// The Date that the event occurs
        /// </summary>
        DateTime Date { get; set; }
        /// <summary>
        /// True if the event is enabled, otherwise false
        /// </summary>
        bool Enabled { get; set; }
        int Type { get; set; }
        /// <summary>
        /// A value indicating how often the event occurs
        /// </summary>
        RecurringFrequencies RecurringFrequency { get; set; }
        /// <summary>
        /// The name of the event
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// The roles allowed to view event
        /// </summary>
        string Audience { get; set; }
        
        /// <summary>
        /// If this is a recurring event, set this to true to make the event show up only from the day specified forward
        /// </summary>
        bool ThisDayForwardOnly { get; set; }
        /// <summary>
        /// Set this to a custom function that will automatically determine if the event should be rendered on a given day.
        /// This is only executed if <see cref="RecurringFrequency"/> is set to custom.
        /// </summary>
        CustomRecurringFrequenciesHandler CustomRecurringFunction { get; set; }
        /// <summary>
        /// A function for cloning an event instance
        /// </summary>
        /// <returns>A cloned <see cref="IEvent"/></returns>
        IEvent Clone();
    }

    /// <summary>
    /// A delegate for creating custom recurring frequencies
    /// </summary>
    /// <param name="evnt">The <see cref="IEvent"/> in question</param>
    /// <param name="day">The day in question</param>
    /// <returns>Should return a boolean value that indicates if the event should be rendered on the day passed in</returns>
    public delegate bool CustomRecurringFrequenciesHandler(IEvent evnt, DateTime day);
}
