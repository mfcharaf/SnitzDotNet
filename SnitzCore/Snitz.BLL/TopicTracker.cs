using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SnitzConfig;

namespace Snitz.BLL
{
    public static class TopicTracker
    {

        public static void TrackIt(int topicid, int lastmessageid, HttpContext context)
        {
            int lastpage = 0;
            HttpCookie cookie = context.Request.Cookies["topictracker"] ?? new HttpCookie("topictracker");
            if (cookie[topicid.ToString()] != null)
            {
                lastpage = Convert.ToInt32(cookie[topicid.ToString()]);

            }
            cookie.Values[topicid.ToString()] = Math.Max(lastmessageid, lastpage).ToString();
            //cookie.Path = Config.CookiePath;
            cookie.Expires = DateTime.UtcNow.AddMonths(3);
            context.Response.Cookies.Add(cookie);
        }

        public static int LastTopicPage(int topicid, HttpContext context)
        {
            if (context.Request.Cookies["topictracker"] == null)
            {
                return 0;
            }

            HttpCookie cookie = context.Request.Cookies["topictracker"];
            if (cookie == null || cookie[topicid.ToString()] == null)
            {
                return 0;
            }

            return Convert.ToInt32(cookie[topicid.ToString()]);
        }
    }
}
