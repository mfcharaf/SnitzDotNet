/*
####################################################################################################################
##
## Snitz.BLL - PostExtensions
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ModConfig;
using SnitzConfig;


namespace Snitz.Entities
{
    /// <summary>
    /// Extension methods for parsing posted data
    /// </summary>
    public static class PostExtensions
    {
        private const RegexOptions MATCH_OPTIONS = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        public static string ParseWebUrls(this string message)
        {
            const string urlregex = @"(?:[^]""])(?<url>(?<protocol>https?|ftp|gopher|telnet|file|notes|ms-help)://(?<domain>[a-zA-Z0-9.]+)(?<path>[a-z0-9./\%-]+)(?<querystring>\?[\S-]+)*)";
            string res = Regex.Replace(message, urlregex, "[url]$1[/url]", MATCH_OPTIONS);
            return res; //.ParseVideoTags();
        }

        public static string ParseVideoTags(this string message)
        {
            const string urlregex = @"(http://)*(youtu.be/|www.youtube.com/embed/|youtube.com/embed/)([0-9a-zA-z\-]+)";
            return Regex.Replace(message, urlregex, "[video=\"$3\"]Video Description[/video]", MATCH_OPTIONS);
        }
        
        public static string ReplaceNoParseTags(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            string[] strArray;

            const string oTag = "[noparse]";
            const string cTag = "[/noparse]";

            string strResultString = "";
            string strTempString = text;

            int oTagPos = strTempString.IndexOf(oTag, 0, StringComparison.CurrentCultureIgnoreCase);
            int cTagPos = strTempString.IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase);

            if ((oTagPos >= 0) && (cTagPos > 0))
            {
                strArray = Regex.Split(strTempString, Regex.Escape(oTag), MATCH_OPTIONS);

                for (int counter2 = 0; counter2 < strArray.Length; counter2++)
                {
                    if (Regex.Match(strArray[counter2], Regex.Escape(cTag), MATCH_OPTIONS).Index > 0)
                    {
                        string[] strArray2 = Regex.Split(strArray[counter2], Regex.Escape(cTag), MATCH_OPTIONS);
                        string strCodeText = strArray2[0].Trim();
                        //replace all forumcode [] tags to their hex equivalent
                        strCodeText = Regex.Replace(strCodeText, @"\]", "&#93;", MATCH_OPTIONS); // ## replace by entity equivalent
                        strCodeText = Regex.Replace(strCodeText, @"\[", "&#91;", MATCH_OPTIONS); // ## replace by entity equivalent
                        strCodeText = Regex.Replace(strCodeText, @"\.", "&#46;", MATCH_OPTIONS); // ## replace by entity equivalent
                        strCodeText = Regex.Replace(strCodeText, @"\/", "&#47;", MATCH_OPTIONS); // ## replace by entity equivalent
                        strCodeText = Regex.Replace(strCodeText, @"\:", "&#58;", MATCH_OPTIONS); // ## replace by entity equivalent
                        strCodeText = Regex.Replace(strCodeText, @"http", "&#104;&#116;&#116;&#112;"); // ## replace by entity equivalent
                        //done replacing
                        strResultString = strResultString + strCodeText + strArray2[1];
                    }
                    else
                        strResultString = strResultString + strArray[counter2];
                }
                strTempString = strResultString;
            }
            return strTempString;
        }

    }
}
