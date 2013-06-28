using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using SnitzCommon;
using SnitzData;

namespace SnitzUI.UserControls.Popups
{
    public partial class SplitTopic : TemplateUserControl
    {
        private string _sortDirection;
        private Topic _topic;
        private int _replyid;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Data != null)
            {
                string[] data = Regex.Split(Data.ToString(), ",", RegexOptions.ExplicitCapture);
                _replyid = Convert.ToInt32(data[0]);
                Reply reply = Util.GetReply(_replyid);
                int topicid = reply.TopicId;

                splitTopicId.Value = topicid.ToString();
                if (data.Length > 1)
                    _sortDirection = data[1];
                _topic = Util.GetTopic(Convert.ToInt32(topicid));
                ddlForum.DataSource = Util.ListForums();
                ddlForum.DataBind();                
                if(!IsPostBack)
                    ddlForum.SelectedValue = _topic.ForumId.ToString();
                ddlSort.SelectedValue = _sortDirection;
                switch (_sortDirection)
                {
                    case "asc" :
                        ddlSort.Attributes.Add("onchange",String.Format(
                        "mainScreen.LoadServerControlHtml('Split Topic',{{'pageID':6,'data':'{0},desc'}}, 'methodHandlers.BeginRecieve');return false;", _replyid));
                        break;
                    case "desc" :
                        ddlSort.Attributes.Add("onchange", String.Format(
                        "mainScreen.LoadServerControlHtml('Split Topic',{{'pageID':6,'data':'{0},asc'}}, 'methodHandlers.BeginRecieve');return false;", _replyid));

                        break;
                }
                BindForm();
                StartupScript = "jQuery(\"abbr.timeago\").timeago();";
            }
        }

        private void BindForm()
        {
            List<Reply> replies = ddlSort.SelectedValue == "desc" ? 
                _topic.Replies.OrderByDescending(r => r.Date).ToList() : 
                _topic.Replies.OrderBy(r => r.Date).ToList();

            grdReplies.DataSource = replies;
            grdReplies.DataBind();
        }

        protected void SetCheckboxValues(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                var reply = (Reply)e.Row.DataItem;
                var cbx = e.Row.Cells[3].FindControl("cbxRow") as CheckBox;
                if(cbx != null)
                {
                    if (reply.Id == _replyid)
                        cbx.Checked = true;
                    cbx.InputAttributes.Add("value",reply.Id.ToString());
                }
            }
        }
    }

}