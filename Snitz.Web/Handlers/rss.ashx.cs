using System;
using System.Linq;
using System.Web;
using SnitzConfig;
using SnitzCommon;
using SnitzData;

namespace SnitzUI
{
    /// <summary>
    /// Summary description for rss
    /// </summary>
    public class rss : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string sTxt;
            string fId = context.Request.Params["Id"];
            if (string.IsNullOrEmpty(fId))
                return;

            if (context.Cache["RSSFEED" + fId] == null)
            {
                sTxt = BuildXmlString(fId);
                context.Cache.Insert("RSSFEED" + fId, sTxt);
            }
            else
                sTxt = context.Cache["RSSFEED" + fId].ToString();

            context.Response.ContentType = "text/xml";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.Write(sTxt);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string BuildXmlString(string forumId)
        {
            if (forumId == null)
                return "";
            Forum forum = Util.GetForum(Convert.ToInt32(forumId));
            string sTitle = Config.ForumTitle;
            string sSiteUrl = Config.ForumUrl;
            string sDescription = forum.Description;
            const string sTTL = "60";

            var oBuilder = new System.Text.StringBuilder();
            oBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            oBuilder.Append("<rss version=\"2.0\"><channel>");

            oBuilder.Append("<title>");
            oBuilder.Append(sTitle);
            oBuilder.Append("</title>");
            oBuilder.Append("<copyright>");
            oBuilder.Append("Copyright 2009, " + sTitle);
            oBuilder.Append("</copyright>");
            oBuilder.Append("<generator>");
            oBuilder.Append("Snitz Forums .Net");
            oBuilder.Append("</generator>");
            oBuilder.Append("<link>");
            oBuilder.Append(sSiteUrl);
            oBuilder.Append("</link>");

            oBuilder.Append("<description>");
            oBuilder.Append(sDescription);
            oBuilder.Append("</description>");
            oBuilder.Append("<pubDate>");
            oBuilder.Append(DateTime.Now);
            oBuilder.Append("</pubDate>");
            oBuilder.Append("<ttl>");
            oBuilder.Append(sTTL);
            oBuilder.Append("</ttl>");

            AppendItems(oBuilder, forum);

            oBuilder.Append("</channel></rss>");
            return oBuilder.ToString();
        }
        
        public void AppendItems(System.Text.StringBuilder oBuilder, Forum forum)
        {
            
            foreach (Topic row in forum.Topics.OrderByDescending(t=>t.LastPostDate).Take(10))
            {
                int topicId = row.Id;
                Topic topic = Util.GetTopic(topicId);
                string sTitle = topic.Subject;
                string sGuid = string.Format("{0}/Content/Forums/topic.aspx?TOPIC={1}", Config.ForumUrl, topicId);
                string sLink = string.Format("{0}/Content/Forums/topic.aspx?TOPIC={1}&amp;whichpage=-1", Config.ForumUrl, topicId);
                if (topic.LastReplyId > 0)
                    sLink += "#" + topic.LastReplyId;

                string sDescription = topic.Message.Length > 512 ? topic.Message.Substring(0, 512) + " ... " : topic.Message;
                string sPubDate = topic.LastPostDate.ToISO8601Date(false, 0);
                string author = topic.Author.Name;
                if (topic.LastReplyId > 0)
                {
                    if (topic.LastReplyId != null)
                    {
                        Reply rep = Util.GetReply((int) topic.LastReplyId);
                        author = rep.Author.Name;
                        sDescription = rep.Message.Length > 256 ? rep.Message.Substring(0, 256) + " ... " : rep.Message;
                    }
                }

                oBuilder.Append("<item>");
                oBuilder.Append("<category>");
                oBuilder.Append(forum.Subject);
                oBuilder.Append("</category>");
                oBuilder.Append("<title><![CDATA[ ");
                oBuilder.Append(sTitle);
                oBuilder.Append(" ]]> </title>");
                oBuilder.Append("<author>");
                oBuilder.Append(author);
                oBuilder.Append("</author>");
                oBuilder.Append("<link>");
                oBuilder.Append(sLink);
                oBuilder.Append("</link>");
                oBuilder.Append("<guid isPermaLink=\"true\">");
                oBuilder.Append(sGuid);
                oBuilder.Append("</guid>");
                oBuilder.Append("<description><![CDATA[ ");
                oBuilder.Append(sDescription);
                oBuilder.Append(" ]]> </description>");
                oBuilder.Append("<pubDate>");
                oBuilder.Append(sPubDate);
                oBuilder.Append("</pubDate>");
                oBuilder.Append("</item>");


            }
        }
    }
}