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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using SnitzConfig;

namespace SnitzCommon
{
//DaylightTime dtt = TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year);
//    If (dtt.Start < PostDate || dtt.End > PostDate) & AdjustForDaylightSavings == true
//        // adjust time, maybe something like this:
//        // DisplayPostDate = PostDate.ToUniversalTime().AddHours(GMToffset - 1).ToLocalTime()

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Fetches a list of controls
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<Control> GetAllControls(this Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;
                foreach (Control descendant in control.GetAllControls())
                {
                    yield return descendant;
                }
            }
        }

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
        /// surrounds a string with CDATA tags
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string WrapCData(this string value)
        {
            if (value == null)
                return value;
            
            return String.Format("<![CDATA[{0}]]>", value);
        }

        /// <summary>
        /// Removes CDATA tags which wrap a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripCData(this string value)
        {
            if (value == null)
                return value;
            return value.Replace("<![CDATA[", "").Replace("]]>", "");
        }
        /// <summary>
        /// Removes all bb code tags from a string
        /// </summary>
        /// <param name="fstring"></param>
        /// <returns></returns>
        public static string CleanForumCodeTags(this string fstring)
        {
            //fstring = Regex.Replace(fstring, @"\[code].*[/code]", "", RegexOptions.Multiline);
            return Regex.Replace(fstring, @"\[(?<tag>.+)\](?<content>.*?)\[\/\k<tag>\]", "${content}", RegexOptions.Multiline);
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
                
                forumAdjust = new TimeSpan((int) Config.TimeAdjust, 0, 0); //Forum TimeZone adjustment
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
        /// Converts Double hours into hours and minutes Timespan
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static TimeSpan DoubleToHours(this double val)
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

        /// <summary>
        /// Return the [Description] attribute of a property
        /// </summary>
        /// <param name="propertyname"></param>
        /// <param name="modconfig"></param>
        /// <returns></returns>
        public static string GetModPropertyDescription(this string propertyname, object modconfig)
        {
            
            //Get property descriptor for current property
            var property = modconfig.GetType().GetProperty(propertyname);
            var attribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true)[0];
            var description = (DescriptionAttribute)attribute;
            return description.Description;
        }

        public static string GetFileNameWithoutExtensions(this string path)
        {
            string result = path;
            while (result.Contains("."))
            {
                result = Path.GetFileNameWithoutExtension(result);
            }
            return result;
        }

        public static XmlElement GetXmlElement(this string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement;
        }
    }
}
