/*
####################################################################################################################
##
## ASP - rss.ashx
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
using System.Linq;
using System.Web;
using Snitz.BLL;
using Snitz.Entities;
using SnitzConfig;
using SnitzCommon;


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
            ForumInfo forum = Forums.GetForum(Convert.ToInt32(forumId));
            string sTitle = Config.ForumTitle;
            string sSiteUrl = Config.ForumUrl;
            string sDescription = forum.Description;
            const string TTL = "60";

            var oBuilder = new System.Text.StringBuilder();
            oBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            oBuilder.Append("<rss version=\"2.0\"><channel>");

            oBuilder.Append("<title>");
            oBuilder.Append(sTitle.WrapCData());
            oBuilder.Append("</title>");
            oBuilder.Append("<copyright>");
            oBuilder.Append(("Copyright 2009, " + sTitle).WrapCData());
            oBuilder.Append("</copyright>");
            oBuilder.Append("<generator>");
            oBuilder.Append("Snitz Forums .Net");
            oBuilder.Append("</generator>");
            oBuilder.Append("<link>");
            oBuilder.Append(sSiteUrl);
            oBuilder.Append("</link>");

            oBuilder.Append("<description>");
            oBuilder.Append(sDescription.WrapCData());
            oBuilder.Append("</description>");
            oBuilder.Append("<pubDate>");
            oBuilder.Append(DateTime.Now);
            oBuilder.Append("</pubDate>");
            oBuilder.Append("<ttl>");
            oBuilder.Append(TTL);
            oBuilder.Append("</ttl>");

            AppendItems(oBuilder, forum);

            oBuilder.Append("</channel></rss>");
            return oBuilder.ToString();
        }
        
        public void AppendItems(System.Text.StringBuilder oBuilder, ForumInfo forum)
        {

            foreach (TopicInfo row in Forums.GetForumTopics(forum.Id,0,10).OrderByDescending(t => t.LastPostDate))
            {
                int topicId = row.Id;
                TopicInfo topic = Topics.GetTopic(topicId);
                string sTitle = topic.Subject;
                string sGuid = string.Format("{0}Content/Forums/Topic/{1}", Config.ForumUrl, topicId);
                string sLink = string.Format("{0}Content/Forums/topic.aspx?TOPIC={1}&amp;whichpage=-1", Config.ForumUrl, topicId);
                if (topic.LastReplyId > 0)
                    sLink += "#" + topic.LastReplyId;

                string sDescription = topic.Message; //topic.Message.Length > 512 ? topic.Message.Substring(0, 512) + " ... " : topic.Message;
                string sPubDate = topic.LastPostDate.Value.ToISO8601Date(false,null);
                string author = topic.AuthorName;
                if (topic.LastReplyId > 0)
                {
                    if (topic.LastReplyId != null)
                    {
                        ReplyInfo rep = Replies.GetReply((int) topic.LastReplyId);
                        author = rep.AuthorName;
                        sDescription = rep.Message;  //rep.Message.Length > 256 ? rep.Message.Substring(0, 256) + " ... " : rep.Message;
                    }
                }

                oBuilder.Append("<item>");
                oBuilder.Append("<category>");
                oBuilder.Append(forum.Subject.WrapCData());
                oBuilder.Append("</category>");
                oBuilder.Append("<title>");
                oBuilder.Append(sTitle.WrapCData());
                oBuilder.Append("</title>");
                oBuilder.Append("<author>");
                oBuilder.Append(author);
                oBuilder.Append("</author>");
                oBuilder.Append("<link>");
                oBuilder.Append(sLink);
                oBuilder.Append("</link>");
                oBuilder.Append("<guid isPermaLink=\"true\">");
                oBuilder.Append(sGuid);
                oBuilder.Append("</guid>");
                oBuilder.Append("<description>");
                oBuilder.Append(sDescription.ParseTags().WrapCData());
                oBuilder.Append("</description>");
                oBuilder.Append("<pubDate>");
                oBuilder.Append(sPubDate);
                oBuilder.Append("</pubDate>");
                oBuilder.Append("</item>");


            }
        }
    }
}