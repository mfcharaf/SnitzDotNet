using System;
using System.Linq;
using SnitzCommon;
using SnitzData;

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
                        Reply reply = Util.GetReply(Convert.ToInt32(ids[1]));
                        lblPosted.Text = String.Format("Posted by:{0} on {1}", reply.Author.Name, reply.Date);
                        msgBody.CssClass = "bbcode";
                        msgBody.Text = reply.Message;
                        hdnReply.Value = reply.Id.ToString();
                    }else
                    {
                        Topic topic = Util.GetTopic(Convert.ToInt32(ids[0]));
                        lblPosted.Text = String.Format("Posted by:{0} on {1}", topic.Author.Name, topic.Date);
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
                Util.SetTopicStatus(Convert.ToInt32(hdnTopic.Value),Enumerators.PostStatus.Open);
            if (!String.IsNullOrEmpty(hdnReply.Value))
                Util.SetReplyStatus(Convert.ToInt32(hdnReply.Value), Enumerators.PostStatus.Open);
        }

        protected void BtnHoldClick(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(hdnTopic.Value))
                Util.SetTopicStatus(Convert.ToInt32(hdnTopic.Value), Enumerators.PostStatus.OnHold);
            if (!String.IsNullOrEmpty(hdnReply.Value))
                Util.SetReplyStatus(Convert.ToInt32(hdnReply.Value), Enumerators.PostStatus.OnHold);

        }
    }
}