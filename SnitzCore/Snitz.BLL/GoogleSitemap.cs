
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Snitz.BLL;
using Snitz.Entities;
using SnitzConfig;

[XmlType(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
public enum ChangeFrequency
{
    [XmlEnum(Name = "always")]
    Always,
    [XmlEnum(Name = "hourly")]
    Hourly,
    [XmlEnum(Name = "daily")]
    Daily,
    [XmlEnum(Name = "weekly")]
    Weekly,
    [XmlEnum(Name = "monthly")]
    Monthly,
    [XmlEnum(Name = "yearly")]
    Yearly,
    [XmlEnum(Name = "never")]
    Never
}

public class GoogleSitemap : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/xml";

        var urlCollection = new UrlCollection();

        // Let's add home page
        var homePage = new Url(new Uri(Config.HomeUrl), DateTime.Today, ChangeFrequency.Daily, 0.7M);
        urlCollection.Add(homePage);
        if (!homePage.Location.StartsWith("http://") && !homePage.Location.StartsWith("https://") && !homePage.Location.StartsWith("feed://"))
        {
            throw new Exception("Sitemap URLs must include protocol (e.g. http://). Refer to http://sitemaps.org for details");
        }
        var forumPage = new Url(new Uri(Config.ForumUrl), DateTime.Today, ChangeFrequency.Daily, 0.7M);
        urlCollection.Add(forumPage);

        #region Write out the topics

        foreach (TopicInfo topic in Topics.GetTopicsForSiteMap(100))
        {
            int topicId = topic.Id;

            var thisUrl = new Url(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/Content/Forums/topic.aspx?TOPIC_ID={1}", forumPage.Location.TrimEnd('/'), topicId)), topic.LastPostDate.Value, ChangeFrequency.Daily, 0.5M);
            if (thisUrl.Location.Length >= 2048)
            {
                throw new Exception("Sitemap URLs cannot have more than 2048 characters. Refer to http://sitemaps.org for details");
            }
            urlCollection.Add(thisUrl);
        }

        #endregion
       
        
        //urlCollection.Add(new Url(new Uri(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}policy.aspx", config.forumUrl)), DateTime.Today, ChangeFrequency.Never, 0.0M));
        urlCollection.Add(new Url(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/Content/Faq/faq.aspx", Config.ForumUrl.TrimEnd('/'))), DateTime.Today, ChangeFrequency.Never, 0.0M));
        urlCollection.Add(new Url(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/Content/Forums/active.aspx", Config.ForumUrl.TrimEnd('/'))), DateTime.Today, ChangeFrequency.Always, 1.0M));
        if (urlCollection.Count > 5000)
        {
            throw new Exception("Sitemap file cannot contain more than 5,000 URLs. Refer to http://sitemaps.org for details");
        }
        var serializer = new XmlSerializer(typeof(UrlCollection));
        var xmlTextWriter = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8);
        serializer.Serialize(xmlTextWriter, urlCollection);      
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}