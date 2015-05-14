using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resources;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace Snitz.BLL
{
    public static class SnitzTime
    {
        ///// <summary>
        ///// Converts a DateTime into a freindly TimeAgo tag
        ///// </summary>
        ///// <param name="date">Date to display</param>
        ///// <param name="authenticated">Is the user logged in</param>
        ///// <param name="timediff">Users time zone difference</param>
        ///// <returns></returns>
        public static string TimeAgoTag(DateTime? date, bool authenticated, MemberInfo member)
        {
            if (date == DateTime.MinValue || date == null)
                return "";

            return string.Format("<abbr class='timeago' title='{0}' dir='inherit'>{1}</abbr>",
                date.Value.ToISO8601Date(authenticated, member),
                date.Value.ToForumDateDisplay(" ", true, authenticated, member));
        }
        ///// <summary>
        ///// Converts a DateTime into a freindly TimeAgo tag
        ///// </summary>
        ///// <param name="date">Date to display</param>
        ///// <param name="authenticated">Is the user logged in</param>
        ///// <param name="timediff">Users time zone difference</param>
        ///// <returns></returns>
        public static string TimeAgoTag(DateTime? date, bool authenticated, MemberInfo member, string tooltip)
        {
            if (date == DateTime.MinValue || date == null)
                return "";

            return string.Format("<abbr class='timeago' title='{0}' dir='inherit'>{1}</abbr>",
                date.Value.ToISO8601Date(authenticated, member), tooltip);

        }
        /// <summary>
        /// Formats a DateTime value into the correct forum date + Time display format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="seperator">string to place between the date and time</param>
        /// <param name="showtime">flag to indicate if time is displayed</param>
        /// <param name="authenticated">Is the user authenticated</param>
        /// <param name="member">Member Object</param>
        /// <returns></returns>
        public static string ToForumDateDisplay(this DateTime date, string seperator, bool showtime, bool authenticated, MemberInfo member)
        {
            string dateFormat = Config.DateFormat;
            string timeFormat = Config.TimeFormat;
            double timediff = Config.TimeAdjust;

            if (authenticated && member != null)
            {
                timediff = member.TimeOffset;
                if (!String.IsNullOrEmpty(member.TimeZone) && member.UseDaylightSaving)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(member.TimeZone);
                    DateTime time = TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneInfo);
                    var diff = time - date;
                    timediff = diff.TotalHours;
                }                
            }

            TimeSpan forumAdjust = timediff.DoubleToHours();

            DateTime dtForum = date + forumAdjust;
            if (showtime)
            {
                return Common.TranslateNumerals(dtForum.ToString(dateFormat + seperator + timeFormat));
            }

            return Common.TranslateNumerals(dtForum.ToString(dateFormat));
        }

        /// <summary>
        /// Converts a forum date string to an ISO 8601 format for Xml
        /// </summary>
        /// <param name="date"></param>
        /// <param name="authenticated"> </param>
        /// <param name="member">Member object</param>
        /// <returns></returns>
        public static string ToISO8601Date(this DateTime date, bool authenticated, MemberInfo member)
        {
            double timeDiff = Config.TimeAdjust;
            if (authenticated && member != null)
            {
                timeDiff = member.TimeOffset;
                if (!String.IsNullOrEmpty(member.TimeZone) && member.UseDaylightSaving)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(member.TimeZone);
                    DateTime time = TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneInfo);
                    var diff = time - date;
                    timeDiff = diff.TotalHours;
                }
            }

            TimeSpan forumAdjust = timeDiff.DoubleToHours();

            string plusminus = "+";
            if (timeDiff < 0)
                plusminus = "-";

            DateTime dtForum = date + forumAdjust;
            string datestr = String.Format("{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", dtForum.Year, dtForum.Month, dtForum.Day, dtForum.Hour, dtForum.Minute, dtForum.Second);
            datestr = datestr + plusminus + Math.Abs(forumAdjust.Hours).ToString().PadLeft(2, '0') + ":" + forumAdjust.Minutes.ToString().PadLeft(2, '0');
            return datestr;
        }
    }
}
