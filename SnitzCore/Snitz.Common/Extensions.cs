/*
####################################################################################################################
##
## SnitzCommon - Extensions
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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using SnitzConfig;

namespace SnitzCommon
{
    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extensions
    {
        public static object ConvertDBNull(this object obj)
        {
            return obj ?? DBNull.Value;
        }
        public static object ConvertDBNull(this object obj, object def)
        {
            return obj ?? def;
        }
        /// <summary>
        /// Checks if a string is a valid number
        /// </summary>
        /// <param name="expression">String to evaluate</param>
        /// <returns></returns>
        public static bool IsNumeric(this string expression)
        {
            double retNum;
            bool isNum = Double.TryParse(Convert.ToString(expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// Check if string contains ANY of the required values
        /// </summary>
        /// <param name="str"></param>
        /// <param name="values">Values to look for</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length > 0)
            {
                if (str != null) return values.Any(str.Contains);
            }

            return false;
        }

        /// <summary>
        /// Checks if a string contains ALL the required values
        /// </summary>
        /// <param name="str"></param>
        /// <param name="values">Values to look for</param>
        /// <returns></returns>
        public static bool ContainsAll(this string str, params string[] values)
        {
            if (!string.IsNullOrEmpty(str) || values.Length > 0)
            {
                if (str != null) return values.Aggregate(true, (current, value) => current && str.Contains(value));
            }

            return false;
        }
        
        /// <summary>
        /// Converts a Snitz date string into a DateTime
        /// </summary>
        /// <param name="forumdate">forum date string to convert</param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string forumdate)
        {
            //CultureInfo provider = CultureInfo.CurrentUICulture;
            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-GB");
            //pad the forumdate incase we are converting the DOB

            try
            {
                return DateTime.ParseExact(forumdate.PadRight(14, '0'), "yyyyMMddHHmmss", ci);
            }
            catch (Exception)
            {

                return null;
            }

        }

        /// <summary>
        /// Removes all bb code tags from a string
        /// </summary>
        /// <param name="fstring"></param>
        /// <returns></returns>
        public static string CleanForumCodeTags(this string fstring)
        {
            //fstring = Regex.Replace(fstring, @"\[code].*[/code]", "", RegexOptions.Multiline);
            return Regex.Replace(fstring, @"(\[[^\[]*\])", "", RegexOptions.Multiline);
        }
        
        /// <summary>
        /// Converts a forum date string to an ISO 8601 format for Xml
        /// </summary>
        /// <param name="fDate"></param>
        /// <param name="TimeDiff">Users TimeZone difference</param>
        /// <returns></returns>
        public static string ToISO8601Date(this string fDate, int TimeDiff)
        {
            //2008-07-17T09:24:17Z
            Int64 ii;

            if (!Int64.TryParse(fDate, out ii))
            {
                try
                {
                    Convert.ToDateTime(fDate);
                    fDate = Convert.ToDateTime(fDate).ToString("yyyyMMddHHmmss");
                }
                catch { fDate = null; }
            }

            TimeSpan forumAdjust = new TimeSpan();

            try
            {
                if ((HttpContext.Current.User.Identity.IsAuthenticated) && (TimeDiff != 99))
                    forumAdjust = new TimeSpan(TimeDiff, 0, 0); //User TimeZone adjustment
            }
            catch (Exception)
            {
                forumAdjust = new TimeSpan(Config.TimeAdjust, 0, 0); //Forum TimeZone adjustment
            }
            string plusminus = "+";
            if (forumAdjust.TotalHours < 0)
                plusminus = "";

            if ((fDate == null) || (fDate.Trim() == "")) return "";

            int fYear = Int32.Parse(fDate.Substring(0, 4));
            int fMonth = Int32.Parse(fDate.Substring(4, 2));
            int fDay = Int32.Parse(fDate.Substring(6, 2));

            int fHour = Int32.Parse(fDate.Substring(8, 2));
            int fMin = Int32.Parse(fDate.Substring(10, 2));
            int fSec = Int32.Parse(fDate.Substring(12, 2));
            DateTime dtForum = new DateTime(fYear, fMonth, fDay, fHour, fMin, fSec) + forumAdjust;
            return dtForum.ToString("yyyy-MM-ddTHH:mm:ss") + plusminus + ((int)forumAdjust.TotalHours).ToString().PadLeft(2, '0');

        }

        /// <summary>
        /// Converts a forum date string to an ISO 8601 format for Xml
        /// </summary>
        /// <param name="fDate"></param>
        /// <param name="authenticated"> </param>
        /// <param name="timeDiff">Users TimeZone difference</param>
        /// <returns></returns>
        public static string ToISO8601Date(this DateTime fDate, bool authenticated, int timeDiff)
        {
            double offset = 0.0;
            TimeSpan forumAdjust = new TimeSpan(0);

            try
            {
                if ((authenticated) && (timeDiff != 99))
                {
                    offset = timeDiff;
                    forumAdjust = DoubleToHours(offset);
                }
            }
            catch (Exception)
            {
                offset = Config.TimeAdjust;
                forumAdjust = DoubleToHours(offset);
            }
            string plusminus = "+";
            if (offset < 0)
                plusminus = "-";

            DateTime dtForum = fDate + forumAdjust;
            string datestr = String.Format("{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", dtForum.Year, dtForum.Month, dtForum.Day, dtForum.Hour, dtForum.Minute, dtForum.Second);
            datestr = datestr + plusminus + Math.Abs(forumAdjust.Hours).ToString().PadLeft(2, '0') + ":" + forumAdjust.Minutes.ToString().PadLeft(2, '0');
            string iso8601date = dtForum.ToString("yyyy-MM-ddTHH:mm:ss") + plusminus + Math.Abs(forumAdjust.Hours).ToString().PadLeft(2,'0') + ":" + forumAdjust.Minutes.ToString().PadLeft(2,'0');
            return datestr;
        }
        
        /// <summary>
        /// Converts Double hours int hours and minutes Timespan
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static TimeSpan DoubleToHours(double val)
        {

            double hours = Math.Floor(val);
            double minutes = (val - hours) * 60.0;

            int Hours = (int)Math.Floor(hours);
            int Minutes = (int)Math.Floor(minutes);

            TimeSpan nTime = new TimeSpan(Hours, Minutes, 0);
            return nTime;
        } 

        /// <summary>
        /// Converts a DateTime into a forum date string
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToForumDateStr(this DateTime date)
        {
            string datestr = String.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}",date.Year,date.Month,date.Day,date.Hour,date.Minute,date.Second);
            return datestr;
        }

        /// <summary>
        /// Formats a DateTime value into the correct forum date + Time display format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="seperator">string to place between the date and time</param>
        /// <param name="showtime">flag to indicate if time is displayed</param>
        /// <param name="authenticated">Is the user authenticated</param>
        /// <param name="timediff">Users time difference</param>
        /// <returns></returns>
        public static string ToForumDateDisplay(this DateTime date, string seperator, bool showtime, bool authenticated, int timediff)
        {
            string dateFormat = Config.DateFormat;
            string timeFormat = Config.TimeFormat;

            TimeSpan forumAdjust = new TimeSpan();

            try
            {
                if ((authenticated) && (timediff != 99))
                    forumAdjust = new TimeSpan(timediff, 0, 0); //User TimeZone adjustment
            }
            catch (Exception)
            {
                forumAdjust = new TimeSpan(Config.TimeAdjust, 0, 0); //Forum TimeZone adjustment
            }

            DateTime? dtForum = date + forumAdjust;
            if (showtime)
            {
                //if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fa")
                //{
                //    PersianDate dtPersianForum = new PersianDate(dtForum.Value);
                //    string test = String.Format("<span dir='rtl'>{0} {1} {2}</span>", Common.TranslateNumerals(dtPersianForum.Day), dtPersianForum.MonthPersianName,
                //                        Common.TranslateNumerals(dtPersianForum.Year));
                //    return test + " " + Common.TranslateNumerals(dtForum.Value.ToString(timeFormat)); 
                //}
                return Common.TranslateNumerals(dtForum.Value.ToString(dateFormat + seperator + timeFormat));

            }
            //if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "fa")
            //{
            //    PersianDate dtPersianForum = new PersianDate(dtForum.Value);
            //    return String.Format("<span dir='rtl'>{0} {1} {2}</span>", Common.TranslateNumerals(dtPersianForum.Day), dtPersianForum.MonthPersianName,
            //                            Common.TranslateNumerals(dtPersianForum.Year));
            //}
            return Common.TranslateNumerals(dtForum.Value.ToString(dateFormat));
        }

        /// <summary>
        /// Return the [Description] attribute of a property
        /// </summary>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static string GetPropertyDescription( this string propertyname)
        {

            //Get property descriptor for current property
            var property = typeof(Config).GetProperty(propertyname);
            var attribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true)[0];
            var description = (DescriptionAttribute)attribute;
            return description.Description;
        }
    }
}
