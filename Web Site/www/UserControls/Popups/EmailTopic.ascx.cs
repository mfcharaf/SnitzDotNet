/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - EmailTopic.ascx
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
using Snitz.BLL;
using SnitzCommon;
using SnitzConfig;

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
                var topic = Topics.GetTopic(_topicid);

                string topicUrl = System.Web.VirtualPathUtility.AppendTrailingSlash(Config.ForumUrl) + "Content/Forums/topic.aspx?TOPIC_ID=" + _topicid;
                string msg = string.Format(Resources.webResources.lblSendTopicMessage, topicUrl, topic.Subject);
                msg = msg.Replace("<br />", Environment.NewLine);
                msg = msg.Replace("<p>", Environment.NewLine);
                Message = msg.Replace("</p>", Environment.NewLine);
            }
        }

    }
}