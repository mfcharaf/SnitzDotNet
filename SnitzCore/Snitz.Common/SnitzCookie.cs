using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using SnitzConfig;

namespace SnitzCommon
{
    public static class SnitzCookie
    {

        #region LastVisitDate
        public static string GetLastVisitDate()
        {
            return GetCookieValue("LastVisit");
        }

        public static void SetLastVisitCookie(string value)
        {
            SetCookie("LastVisit",value);
        }
        #endregion

        #region Default Language

        public static string GetDefaultLanguage()
        {
            return GetCookieValue("ddlLang");
        }

        #endregion

        #region Topic read tracker
        public static void TrackIt(int topicid, int lastmessageid)
        {
            Dictionary<string, string> pages = GetMultipleUsingSingleKeyCookies("topictracker");
            
            if (pages.ContainsKey(topicid.ToString()))
            {
                int lastpage = Convert.ToInt32(pages[topicid.ToString()]);
                pages[topicid.ToString()] = Math.Max(lastmessageid, lastpage).ToString();
            }
            else
            {
                pages.Add(topicid.ToString(),lastmessageid.ToString());
            }
            SetMultipleUsingSingleKeyCookies("topictracker", pages);
        }

        public static int LastTopicPage(int topicid)
        {
            var pages = GetMultipleUsingSingleKeyCookies("topictracker");
            if (pages.ContainsKey(topicid.ToString()))
            {
                return Convert.ToInt32(pages[topicid.ToString()]);
            }
            return 0;
        }
        #endregion

        #region Active Topic cookies
        public static string GetActiveRefresh()
        {
            return GetCookieValue("ActiveRefresh");
        }

        public static void SetActiveRefresh(string value)
        {
            SetCookie("ActiveRefresh",value);
        }
        public static string GetTopicSince()
        {
            return GetCookieValue("SinceDate");
        }

        public static void SetTopicSince(string value)
        {
            SetCookie("SinceDate",value);
        }
        #endregion

        #region Private Messages

        public static Dictionary<string,string> GetPMCookie()
        {
            return GetMultipleUsingSingleKeyCookies("pmModPaging");
        }

        public static void SetPMCookie(string key, string value)
        {
            var current = GetMultipleUsingSingleKeyCookies("pmModPaging");
            if (current.ContainsKey(key))
            {
                current[key] = value;
            }
            else
            {
                current.Add(key,value);
            }
            SetMultipleUsingSingleKeyCookies("pmModPaging",current);
        }

        #endregion


        #region private methods


        private static HttpRequest GetHttpRequest()
        {
            return  HttpContext.Current.Request;
        }
 

        private static HttpResponse GetHttpResponse()
        {
            return  HttpContext.Current.Response;
        }
 

        public static string GetCookieValue(string cookieKey)
        {
            HttpCookie cookie = GetHttpRequest().Cookies.Get(cookieKey);
            return cookie != null ? cookie.Value : null;
        }
 

        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = GetHttpRequest().Cookies.Get(name) ?? new HttpCookie(name);
            cookie.Value = value;
            cookie.Path = Config.CookiePath;
            cookie.Expires = DateTime.UtcNow.AddMonths(2);
            GetHttpResponse().Cookies.Add(cookie);
        }
 

        private static Dictionary<string, string> GetMultipleUsingSingleKeyCookies(string cookieName)
        {
 
            //creating dic to return as collection.
            Dictionary<string, string> dicVal = new Dictionary<string, string>(); 


        //Check whether the cookie available or not.
            if (GetHttpRequest().Cookies[cookieName] != null)
            {
 
                //Creating a cookie.
                HttpCookie cookie = GetHttpRequest().Cookies[cookieName];

                if (cookie != null)
                {
                    //Getting multiple values from single cookie.
                    NameValueCollection nameValueCollection = cookie.Values; 
     
                    //Iterate the unique keys.
                    foreach (string key in nameValueCollection.AllKeys)
                    {
                    dicVal.Add(key, cookie[key]);
                    }
                }
            }
            return dicVal;
        }
 
         private static void SetMultipleUsingSingleKeyCookies(string cookieName, Dictionary<string, string> dic)
         {
             if (GetHttpRequest().Cookies[cookieName] == null)
             {
                 HttpCookie cookie = new  HttpCookie(cookieName);
  
                 //This adds multiple cookies to the same key.
                 foreach (KeyValuePair<string, string> val in dic)
                 {
                     cookie[val.Key] = val.Value;
                 }

                 cookie.Path = Config.CookiePath;
                 cookie.Expires = DateTime.UtcNow.AddDays(30);
                 GetHttpResponse().Cookies.Add(cookie);
             }
          }

        #endregion
    }
}
