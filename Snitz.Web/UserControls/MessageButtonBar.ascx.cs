using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzData;

public partial class MessageButtonBar : UserControl
{

    private int _topicid;
    private string _posttype;
    private string _ip;
    private DateTime _postdate;
    private bool _unmoderated;

    private string currentUser = HttpContext.Current.User.Identity.Name;

    private Member _author;
    private Forum _forum;
    private Topic _topic;
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

        _topic = Util.GetTopic(_topicid);
        _isTopicLocked = _topic.Status == Enumerators.PostStatus.Closed;
        _forum = Util.GetForum(_topic.ForumId);
        _topicid = _topic.Id;
        bool _isForumModerator = new SnitzRoleProvider().IsUserForumModerator(HttpContext.Current.User.Identity.Name, _forum.Id);

        if (_post is Topic)
        {
            _author = _topic.Author;
            ThisId = _topic.Id;
            if (_topic.ReplyCount > 0)
                newerreplies = true;
            _posttype = "TOPICS";
            _postdate = _topic.Date;
            _ip = _topic.PostersIP;

            if (_isadmin || _isForumModerator)
            {
                TopicApprove.Visible = (_topic.Status == Enumerators.PostStatus.UnModerated ||
                                        _topic.Status == Enumerators.PostStatus.OnHold);
                TopicApprove.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        _topic.Id);
                //TopicHold.Visible = _topic.Status == Enumerators.PostStatus.UnModerated;
            }
            if (_topic.Status == Enumerators.PostStatus.UnModerated || _topic.Status == Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (_topic.Status == Enumerators.PostStatus.OnHold)
                    modtext = String.Format("<span class=\"moderation\">!!{0}!!</span>", webResources.OnHold);

            }
            
            imgPosticon.CommandName = "topic";
            TopicDelete.CommandName = "topic";
            SplitTopic.Visible = false;
            hEdit.Text = webResources.lblEditTopic;
            hEdit.ToolTip = webResources.lblEditTopic;
            TopicDelete.OnClientClick = TopicDelete.OnClientClick.Replace("[MESSAGE]", "Do you want to delete this Topic and all its replies?");

        }
        else if (_post is Reply)
        {
            Reply reply = (Reply)_post;
            _author = reply.Author;
            ThisId = reply.Id;
            if (_topic.LastReplyId != reply.Id)
                newerreplies = true;
            _posttype = "REPLY";
            _postdate = reply.Date;
            _ip = reply.PostersIP;
            
            if (_isadmin || _isForumModerator)
            {
                TopicApprove.Visible = (reply.Status == Enumerators.PostStatus.UnModerated ||
                                        reply.Status == Enumerators.PostStatus.OnHold);
                TopicApprove.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0},{1}'}}, 'methodHandlers.BeginRecieve');return false;",
                        "",reply.Id);
                //TopicHold.Visible = reply.Status == Enumerators.PostStatus.UnModerated;
            }
            if (reply.Status == Enumerators.PostStatus.UnModerated || reply.Status == Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (reply.Status == Enumerators.PostStatus.OnHold)
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
        TopicDelete.Visible = (currentUser.ToLower() == _author.Name.ToLower() &&
            !newerreplies);
        
        TopicDelete.Visible = TopicDelete.Visible || (_isForumModerator || _isadmin);
        imgPosticon.AlternateText = String.Format("#{0}", ThisId);


        date.Text = _unmoderated ? modtext : Common.TimeAgoTag(_postdate, page.IsAuthenticated, page.Member != null ? page.Member.TimeOffset : 0);
        
        
        ViewIP.Visible = _isadmin && Config.LogIP;
        ViewIP.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('IP Lookup',{{'pageID':4,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        _ip);

        hEdit.NavigateUrl = string.Format("~/Content/Forums/post.aspx?method=edit&type={0}&id={1}&TOPIC={2}", _posttype, ThisId, _topicid);
        hEdit.Visible = (currentUser.ToLower() == _author.Name.ToLower() && !newerreplies);
        hEdit.Visible = hEdit.Visible && !(_isTopicLocked || _forum.Status == Enumerators.PostStatus.Closed);    // but not if it is locked
        hEdit.Visible = hEdit.Visible || _isForumModerator || _isadmin;      //override for admins/moderator

        hReply.NavigateUrl = String.Format("~/Content/Forums/post.aspx?method=reply&TOPIC={0}", _topicid);
        hReply.Visible = page.IsAuthenticated && !(_isTopicLocked || _forum.Status == Enumerators.PostStatus.Closed);
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
                Reply r = Util.GetReply(Convert.ToInt32(btn.CommandArgument));
                r.Delete();
                if (this.DeleteClicked != null)
                {
                    this.DeleteClicked(this, EventArgs.Empty);
                }
                break;
            case "topic" :
                Topic t = Util.GetTopic(Convert.ToInt32(btn.CommandArgument));
                int forumid = t.ForumId;
                t.Delete();
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
                Reply r = Util.GetReply(Convert.ToInt32(btn.CommandArgument));
                Topic rt = Util.GetTopic(r.TopicId);
                string rurl = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&whichpage={1}&#{2}", r.TopicId, page.CurrentPage + 1, r.Id);
                List<SnitzLink> rbookmarks = page.Profile.BookMarks;
                rbookmarks.Add(new SnitzLink(rt.Subject,rurl));
                page.Profile.BookMarks = rbookmarks;
                page.Profile.Save();
                break;
            case "topic":
                Topic t = Util.GetTopic(Convert.ToInt32(btn.CommandArgument));
                string url = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}", t.Id);
                List<SnitzLink> bookmarks = page.Profile.BookMarks;
                bookmarks.Add(new SnitzLink(t.Subject,url));
                page.Profile.BookMarks = bookmarks;
                page.Profile.Save();
                break;
        }
    }
}
