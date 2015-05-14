/*
####################################################################################################################
##
## Snitz.BLL - Emoticon
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		01/08/2013
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Xml.Linq;
using SnitzConfig;


namespace Snitz.BLL
{
    public class Emoticon
    {
        public string ImageUrl { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }
    /// <summary>
    /// Load emoticon collection from xml file
    /// </summary>
    public static class Emoticons
    {

        public static List<Emoticon> GetEmoticons()
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Request.PhysicalApplicationPath != null)
                {
                    string appdatafolder = AppDomain.CurrentDomain.GetData("DataDirectory").ToString(); //Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");
                    XDocument loaded = XDocument.Load(appdatafolder + @"\emoticons.xml");
                    //IEnumerable<XElement> q = from c in loaded.Descendants("emoticon")
                    //        select c;
                    var q = from e in loaded.Descendants("emoticon")
                            group e by e.Attribute("image").Value into g
                            select g.First();

                    var list = new List<Emoticon>();
                    foreach (XElement element in q)
                    {
                        var smile = new Emoticon
                                             {
                                                 Image = (string) element.Attribute("image"),
                                                 Code = (string) element.Attribute("code"),
                                                 Description =
                                                     ReadResourceValue("emoticons", (string) element.Attribute("name"))
                                             };
                        string imageurl = VirtualPathUtility.ToAbsolute(Config.ImageDirectory + "emoticons/");
                        smile.ImageUrl = string.Format("{0}{1}", imageurl, smile.Image);

                        list.Add(smile);
                    }
                    return list;
                }
            }
            return null;
        }

        private static string ReadResourceValue(string file, string key)
        {
            //value for our return value
            var globalResourceObject = HttpContext.GetGlobalResourceObject(file, key);
            if (globalResourceObject != null)
                return globalResourceObject.ToString();
            return null;
        }
    }
}
