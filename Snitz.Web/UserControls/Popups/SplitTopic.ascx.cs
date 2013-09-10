/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - SplitTopic.ascx
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;


namespace SnitzUI.UserControls.Popups
{
    public partial class SplitTopic : TemplateUserControl
    {
        private string _sortDirection;
        private TopicInfo _topic;
        private int _replyid;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Data != null)
            {
                string[] data = Regex.Split(Data.ToString(), ",", RegexOptions.ExplicitCapture);
                _replyid = Convert.ToInt32(data[0]);
                ReplyInfo reply = Replies.GetReply(_replyid);
                int topicid = reply.TopicId;

                splitTopicId.Value = topicid.ToString();
                if (data.Length > 1)
                    _sortDirection = data[1];
                _topic = Topics.GetTopic(Convert.ToInt32(topicid));
                ddlForum.DataSource = Forums.GetAllForums();
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
            
            List<ReplyInfo> replies = ddlSort.SelectedValue == "desc" ?
                Topics.GetRepliesForTopic(_topic.Id,0,_topic.ReplyCount).OrderByDescending(r => r.Date).ToList() :
                Topics.GetRepliesForTopic(_topic.Id, 0, _topic.ReplyCount).OrderBy(r => r.Date).ToList();

            grdReplies.DataSource = replies;
            grdReplies.DataBind();
        }

        protected void SetCheckboxValues(object sender, GridViewRowEventArgs e)
        {
            if(e.Row.RowType == DataControlRowType.DataRow)
            {
                var reply = (ReplyInfo)e.Row.DataItem;
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