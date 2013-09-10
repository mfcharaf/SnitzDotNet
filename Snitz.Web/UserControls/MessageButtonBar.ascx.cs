/*
####################################################################################################################
##
## SnitzUI.UserControls - MessageButtonBar.ascx
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;

public partial class MessageButtonBar : UserControl
{

    private int _topicid;
    private string _posttype;
    private string _ip;
    private DateTime _postdate;
    private bool _unmoderated;

    private string currentUser = HttpContext.Current.User.Identity.Name;

    private AuthorInfo _author;
    private ForumInfo _forum;
    private TopicInfo _topic;
    private object _post;

    private bool _isTopicLocked;

    public object Post
    {
        get { return _post; }
        set { _post = value;
            SetUpButtons();
        }
    }

    public event EventHandler DeleteClicked; 


    protected int ThisId { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {

        //if (_post != null)
        //    SetUpButtons();
        
    }

    private void SetUpButtons()
    {
        if (Post == null)
            return;
        string modtext = "";

        TopicApprove.Visible = false;
        //TopicHold.Visible = false;
        hReply.Visible = false;
        hReplyQuote.Visible = false;
        hEdit.Visible = false;
        ViewIP.Visible = false;
        TopicDelete.Visible = false;
        SplitTopic.Visible = false;

        PageBase page = (PageBase)this.Page;
        bool _isadmin = page.IsAdministrator;
        bool newerreplies = false;

        _topicid = page.TopicId != null ? page.TopicId.Value : Convert.ToInt32(Session["TOPIC"]);

        _topic = Topics.GetTopic(_topicid);
        _isTopicLocked = _topic.Status == (int)Enumerators.PostStatus.Closed;
        _forum = Forums.GetForum(_topic.ForumId);
        _topicid = _topic.Id;
        bool _isForumModerator = Moderators.IsUserForumModerator(HttpContext.Current.User.Identity.Name, _forum.Id);

        if (_post is TopicInfo)
        {
            _author = Members.GetAuthor(_topic.AuthorId);
            ThisId = _topic.Id;
            if (_topic.ReplyCount > 0)
                newerreplies = true;
            _posttype = "TOPICS";
            _postdate = _topic.Date;
            _ip = _topic.PosterIp;

            if (_isadmin || _isForumModerator)
            {
                TopicApprove.Visible = (_topic.Status == (int)Enumerators.PostStatus.UnModerated ||
                                        _topic.Status == (int)Enumerators.PostStatus.OnHold);
                TopicApprove.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        _topic.Id);
                //TopicHold.Visible = _topic.Status == Enumerators.PostStatus.UnModerated;
            }
            if (_topic.Status == (int)Enumerators.PostStatus.UnModerated || _topic.Status == (int)Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (_topic.Status == (int)Enumerators.PostStatus.OnHold)
                    modtext = String.Format("<span class=\"moderation\">!!{0}!!</span>", webResources.OnHold);

            }
            
            imgPosticon.CommandName = "topic";
            TopicDelete.CommandName = "topic";
            SplitTopic.Visible = false;
            hEdit.Text = webResources.lblEditTopic;
            hEdit.ToolTip = webResources.lblEditTopic;
            TopicDelete.OnClientClick = TopicDelete.OnClientClick.Replace("[MESSAGE]", "Do you want to delete this Topic and all its replies?");

        }
        else if (_post is ReplyInfo)
        {
            ReplyInfo reply = (ReplyInfo)_post;
            _author = Members.GetAuthor(reply.AuthorId);
            ThisId = reply.Id;
            if (_topic.LastReplyId != reply.Id)
                newerreplies = true;
            _posttype = "REPLY";
            _postdate = reply.Date;
            _ip = reply.PosterIp;
            
            if (_isadmin || _isForumModerator)
            {
                TopicApprove.Visible = (reply.Status == (int)Enumerators.PostStatus.UnModerated ||
                                        reply.Status == (int)Enumerators.PostStatus.OnHold);
                TopicApprove.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0},{1}'}}, 'methodHandlers.BeginRecieve');return false;",
                        "",reply.Id);
                //TopicHold.Visible = reply.Status == Enumerators.PostStatus.UnModerated;
            }
            if (reply.Status == (int)Enumerators.PostStatus.UnModerated || reply.Status == (int)Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (reply.Status == (int)Enumerators.PostStatus.OnHold)
                    modtext = String.Format("<span class=\"moderation\">!!{0}!!</span>", webResources.OnHold);
            }

            imgPosticon.CommandName = "reply";
            TopicDelete.CommandName = "reply";
            SplitTopic.CommandArgument = ThisId.ToString();
            hEdit.ToolTip = webResources.lblEditReply;
            hEdit.Text = webResources.lblEditReply;
            TopicDelete.OnClientClick = TopicDelete.OnClientClick.Replace("[MESSAGE]", "Do you want to delete this reply?");
            SplitTopic.Visible = _isForumModerator || _isadmin;
            SplitTopic.OnClientClick = //"$find('mpSplit').show();return false;";
                    String.Format(
                    "mainScreen.LoadServerControlHtml('Split Topic',{{'pageID':6,'data':'{0},asc'}}, 'methodHandlers.BeginRecieve');return false;", reply.Id);

        }
        TopicDelete.Visible = (currentUser.ToLower() == _author.Username.ToLower() &&
            !newerreplies);
        
        TopicDelete.Visible = TopicDelete.Visible || (_isForumModerator || _isadmin);
        imgPosticon.AlternateText = String.Format("#{0}", ThisId);


        date.Text = _unmoderated ? modtext : Common.TimeAgoTag(_postdate, page.IsAuthenticated, page.Member != null ? page.Member.TimeOffset : 0);
        
        
        ViewIP.Visible = _isadmin && Config.LogIP;
        ViewIP.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('IP Lookup',{{'pageID':4,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        _ip);

        hEdit.NavigateUrl = string.Format("~/Content/Forums/post.aspx?method=edit&type={0}&id={1}&TOPIC={2}", _posttype, ThisId, _topicid);
        hEdit.Visible = (currentUser.ToLower() == _author.Username.ToLower() && !newerreplies);
        hEdit.Visible = hEdit.Visible && !(_isTopicLocked || _forum.Status == (int)Enumerators.PostStatus.Closed);    // but not if it is locked
        hEdit.Visible = hEdit.Visible || _isForumModerator || _isadmin;      //override for admins/moderator

        hReply.NavigateUrl = String.Format("~/Content/Forums/post.aspx?method=reply&TOPIC={0}", _topicid);
        hReply.Visible = page.IsAuthenticated && !(_isTopicLocked || _forum.Status == (int)Enumerators.PostStatus.Closed);
        hReply.Visible = hReply.Visible || (_isForumModerator || _isadmin);
        hReplyQuote.Visible = hReply.Visible;
        hReplyQuote.NavigateUrl = String.Format("~/Content/Forums/post.aspx?method=quote&type={0}&id={1}&TOPIC={2}", _posttype, ThisId, _topicid);
        
    }

    protected void DeletePost(object sender, ImageClickEventArgs e)
    {
        ImageButton btn = (ImageButton) sender;
        switch (btn.CommandName)
        {
            case "reply" :
                Replies.DeleteReply(Convert.ToInt32(btn.CommandArgument));
                if (this.DeleteClicked != null)
                {
                    this.DeleteClicked(this, EventArgs.Empty);
                }
                break;
            case "topic" :
                TopicInfo t = Topics.GetTopic(Convert.ToInt32(btn.CommandArgument));
                int forumid = t.ForumId;
                Topics.Delete(t);
                Response.Redirect("~/Content/Forums/Forum.aspx?FORUM=" + forumid);
                break;
        }
    }

    protected void BookMark(object sender, ImageClickEventArgs e)
    {
        PageBase page = (PageBase) Page;
        ImageButton btn = (ImageButton)sender;
        switch (btn.CommandName)
        {
            case "reply":
                ReplyInfo r = Replies.GetReply(Convert.ToInt32(btn.CommandArgument));
                TopicInfo rt = Topics.GetTopic(r.TopicId);
                string rurl = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&whichpage={1}&#{2}", r.TopicId, page.CurrentPage + 1, r.Id);
                List<SnitzLink> rbookmarks = page.Profile.BookMarks;
                rbookmarks.Add(new SnitzLink(rt.Subject,rurl));
                page.Profile.BookMarks = rbookmarks;
                page.Profile.Save();
                break;
            case "topic":
                TopicInfo t = Topics.GetTopic(Convert.ToInt32(btn.CommandArgument));
                string url = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}", t.Id);
                List<SnitzLink> bookmarks = page.Profile.BookMarks;
                bookmarks.Add(new SnitzLink(t.Subject,url));
                page.Profile.BookMarks = bookmarks;
                page.Profile.Save();
                break;
        }
    }
}
