/*
####################################################################################################################
##
## SnitzCommon - Common
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
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;



namespace SnitzCommon
{
    public static class Common
    {
        /// <summary>
        /// Get visitors IP4 address
        /// </summary>
        /// <returns></returns>
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        /// <summary>
        /// Gets the URL for the root of the site
        /// </summary>
        /// <returns></returns>
        public static string GetSiteRoot()
        {
            string port = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
                port = "";
            else
                port = ":" + port;

            string protocol = System.Web.HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
                protocol = "http://";
            else
                protocol = "https://";

            string sOut = protocol + System.Web.HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port + System.Web.HttpContext.Current.Request.ApplicationPath;

            if (sOut.EndsWith("/"))
            {
                sOut = sOut.Substring(0, sOut.Length - 1);
            }

            return sOut;
        } 

        /// <summary>
        /// Calculates the number of pages
        /// </summary>
        /// <param name="totalNumberOfItems">Total No. of items to display</param>
        /// <param name="pageSize">No. of items per page</param>
        /// <returns></returns>
        public static int CalculateNumberOfPages(int totalNumberOfItems, int pageSize)
        {
            var result = totalNumberOfItems % pageSize;
            if (result == 0)
                return totalNumberOfItems / pageSize;
            return totalNumberOfItems / pageSize + 1;
        }
        


        /// <summary>
        /// Works out the current age of a member based on their date of birth
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to evaluate</param>
        /// <returns></returns>
        public static string GetAgeFromDOB(string dateOfBirth)
        {
            DateTime? birthdate = dateOfBirth.ToDateTime();
            if (birthdate.HasValue)
            {
                TimeSpan diff = (DateTime.UtcNow - birthdate.Value);
                return ((int)(diff.TotalDays / 365.25)).ToString();
            }
            return String.Empty;

        }

        /// <summary>
        /// Replaces Western with Arabic Numerals
        /// </summary>
        /// <param name="sIn">Number to replace</param>
        /// <returns>Arabic Numeral</returns>
        public static string TranslateNumerals(object sIn)
        {
            if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName != "fa")
                return sIn.ToString();
            string input = sIn.ToString();
            var arabicDigits = Thread.CurrentThread.CurrentCulture.NumberFormat.NativeDigits;
            for (int i = 0; i < arabicDigits.Length; i++)
            {
                input = input.Replace(i.ToString(), arabicDigits[i]);
            }
            return input;
        }

    }
}
