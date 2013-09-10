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
        private const RegexOptions matchOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        public static string ParseWebUrls(this string message)
        {
            const string urlregex = @"(?:[^]""])(?<url>(?<protocol>https?|ftp|gopher|telnet|file|notes|ms-help)://(?<domain>[a-zA-Z0-9.]+)(?<path>[a-z0-9./\%-]+)(?<querystring>\?[\S-]+)*)";
            string res = Regex.Replace(message, urlregex, "[url]$1[/url]", matchOptions);
            return res; //.ParseVideoTags();
        }

        public static string ParseVideoTags(this string message)
        {
            const string urlregex = @"(?<!\[noparse.*)http://youtu.be/|http://youtube.com/embed/(?!.*noparse])";
            return Regex.Replace(message, urlregex, "", matchOptions);
        }
    }
}
