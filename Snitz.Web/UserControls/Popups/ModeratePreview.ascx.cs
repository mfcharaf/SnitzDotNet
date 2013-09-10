/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - ModeratePreview.ascx
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
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI.UserControls.Popups
{
    public partial class ModeratePreview : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                string[] ids = ((string) Data).Split(',');
                if(ids.Any())
                {
                    if(ids.Count() == 2)
                    {
                        ReplyInfo reply = Replies.GetReply(Convert.ToInt32(ids[1]));
                        lblPosted.Text = String.Format("Posted by:{0} on {1}", reply.AuthorName, reply.Date);
                        msgBody.CssClass = "bbcode";
                        msgBody.Text = reply.Message;
                        hdnReply.Value = reply.Id.ToString();
                    }else
                    {
                        TopicInfo topic = Topics.GetTopic(Convert.ToInt32(ids[0]));
                        lblPosted.Text = String.Format("Posted by:{0} on {1}", topic.AuthorName, topic.Date);
                        msgBody.Text = topic.Message;
                        hdnTopic.Value = topic.Id.ToString();
                    }
                }
                //ApprovePost('<%# hdnTopic.Value %>','<%# hdnReply.Value %>');return false;
                btnOk.OnClientClick = String.Format("ApprovePost('{0}','{1}');return false;", hdnTopic.Value, hdnReply.Value);
                btnHold.OnClientClick = String.Format("OnHold('{0}','{1}');return false;", hdnTopic.Value, hdnReply.Value);
            }

        }

        protected void BtnOkClick(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(hdnTopic.Value))
                Topics.SetTopicStatus(Convert.ToInt32(hdnTopic.Value), (int)Enumerators.PostStatus.Open);
            if (!String.IsNullOrEmpty(hdnReply.Value))
                Replies.SetReplyStatus(Convert.ToInt32(hdnReply.Value), (int)Enumerators.PostStatus.Open);
        }

        protected void BtnHoldClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(hdnTopic.Value))
                Topics.SetTopicStatus(Convert.ToInt32(hdnTopic.Value), (int)Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(hdnReply.Value))
                Replies.SetReplyStatus(Convert.ToInt32(hdnReply.Value), (int)Enumerators.PostStatus.OnHold);

        }
    }
}