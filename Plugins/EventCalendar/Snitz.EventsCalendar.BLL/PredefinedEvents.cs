using System;
using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.BLL
{
    public static partial  class ForumEvents
    {
        public static List<IEvent> PresetHolidays()
        {
            List<IEvent> holidays = new List<IEvent>();

            var newYears = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 1, 1),
                RecurringFrequency = RecurringFrequencies.Yearly,
                Title = "New Year's Day"
            };
            holidays.Add(newYears);
            var easter = new HolidayEvent
            {
                Type = 4,
                Date = EasterSunday(DateTime.Now.Year),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = EasterHandler,
                Title = "Easter Monday"
            };
            holidays.Add(easter);
            var goodfriday = new HolidayEvent
            {
                Type = 4,
                Date = EasterSunday(DateTime.Now.Year).AddDays(-2),
                RecurringFrequency = RecurringFrequencies.Yearly,
                Title = "Good Friday"
            };
            holidays.Add(goodfriday);
            var mayDay = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 05, 01),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = MayDayHandler,
                Title = "Early May bank holiday"
            };
            holidays.Add(mayDay);
            var springDay = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 05, 31),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = SpringDayHandler,
                Title = "Spring bank holiday"
            };
            holidays.Add(springDay);
            var august = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 08, 31),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = SpringDayHandler,
                Title = "Summer bank holiday"
            };
            holidays.Add(august);
            var remembranceDay = new HolidayEvent
            {
                Type = 5,
                Date = new DateTime(DateTime.Now.Year, 11, 11),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = RemembranceHandler,
                Title = "Armestice Day"
            };
            holidays.Add(remembranceDay);

            var christmas = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 12, 25),
                RecurringFrequency = RecurringFrequencies.Yearly,
                Title = "Christmas Day"
            };
            holidays.Add(christmas);
            var boxing = new HolidayEvent
            {
                Type = 4,
                Date = new DateTime(DateTime.Now.Year, 12, 26),
                RecurringFrequency = RecurringFrequencies.Custom,
                CustomRecurringFunction = BoxingDayHandler,
                Title = "Boxing Day"
            };
            holidays.Add(boxing);

            return holidays;
        }

        /// <summary>
        /// Selects second sunday in month
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static bool RemembranceHandler(IEvent evnt, DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Sunday && dt.Day > 7 && dt.Day <= 14 && dt.Month == evnt.Date.Month)
                return true;
            return false;
        }

        private static bool EasterHandler(IEvent evnt, DateTime dt)
        {
            DateTime dt2 = EasterSunday(evnt.Date.Year).AddDays(1);
            if (dt.Month == evnt.Date.Month && dt2.Day == dt.Date.Day)
                return true;

            return false;
        }

        /// <summary>
        /// Selects the last Monday in the month
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static bool SpringDayHandler(IEvent evnt, DateTime dt)
        {
            DateTime dt2 = LastDayOfWeekInMonth(dt, DayOfWeek.Monday);
            if (dt.Month == evnt.Date.Month && dt2.Day == dt.Date.Day)
                return true;

            return false;
        }

        private static bool BoxingDayHandler(IEvent evnt, DateTime dt)
        {
            //not if sat or sunday
            if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && (dt.Day > 25 && dt.Day < 29) && dt.Month == evnt.Date.Month)
                return true;
            return false;
        }

        /// <summary>
        /// Selects the first Monday in the month
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static bool MayDayHandler(IEvent evnt, DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Monday && dt.Day <= 7 && dt.Month == evnt.Date.Month)
                return true;
            return false;
        }

        internal static DateTime LastDayOfWeekInMonth(DateTime day, DayOfWeek dow)
        {
            DateTime lastDay = new DateTime(day.Year, day.Month, 1).AddMonths(1).AddDays(-1);
            DayOfWeek lastDow = lastDay.DayOfWeek;

            int diff = dow - lastDow;

            if (diff > 0) diff -= 7;

            System.Diagnostics.Debug.Assert(diff <= 0);

            return lastDay.AddDays(diff);
        }
        internal static DateTime EasterSunday(int year)
        {
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }
    }
}
