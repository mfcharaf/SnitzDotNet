using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SnitzConfig;


namespace SnitzData
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
                    string appdatafolder = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "App_Data");
                    XDocument loaded = XDocument.Load(appdatafolder + @"\emoticons.xml");
                    var q = from c in loaded.Descendants("emoticon")
                            select c;
            
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
