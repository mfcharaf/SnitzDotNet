using System;
using SnitzCommon;
using SnitzConfig;
using SnitzData;

namespace SnitzUI.UserControls.Popups
{
    public partial class EmailTopic : TemplateUserControl
    {
        private int _topicid;
        protected string Message { get; set; }
        protected int TopicId
        {
            get { return _topicid; }
            set { _topicid = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                _topicid = (int)Data;

                string topicUrl = System.Web.VirtualPathUtility.AppendTrailingSlash(Config.ForumUrl) + "Content/Forums/topic.aspx?TOPIC_ID=" + _topicid;
                string msg = string.Format(Resources.webResources.lblSendTopicMessage, topicUrl, Util.GetTopic(_topicid).Subject);
                msg = msg.Replace("<br />", Environment.NewLine);
                msg = msg.Replace("<p>", Environment.NewLine);
                Message = msg.Replace("</p>", Environment.NewLine);
            }
        }



    }
}