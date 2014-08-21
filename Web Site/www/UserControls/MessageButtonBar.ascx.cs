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
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

public partial class MessageButtonBar : UserControl
{

    private int _topicid;
    private string _posttype;
    private string _ip;
    private DateTime _postdate;
    private bool _unmoderated;

    private readonly string currentUser = HttpContext.Current.User.Identity.Name;

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
    public event EventHandler ReplyDeleteClicked;

    protected int ThisId { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            string postbackbtn = Request.Form["__EVENTTARGET"];
            string argument = Request.Form["__EVENTARGUMENT"];
            int id;
            switch (postbackbtn)
            {
                case "DeleteTopic":
                    id = Convert.ToInt32(argument);
                    DeleteTopic(id);
                    break;
                case "DeleteReply":
                    id = Convert.ToInt32(argument);
                    DeleteReply(id);
                    break;
                case "BookMarkTopic":
                    id = Convert.ToInt32(argument);
                    BookMarkTopic(id);
                    break;
                case "BookMarkReply":
                    id = Convert.ToInt32(argument);
                    BookMarkReply(id);
                    break;
            }
        }
        
    }

    private void SetUpButtons()
    {
        if (Post == null)
            return;
        string modtext = "";

        TopicApprove.Visible = false;
        //TopicHold.Visible = false;
        hReplyQuote.Visible = false;
        hEdit.Visible = false;
        ViewIP.Visible = false;
        TopicDelete.Visible = false;
        SplitTopic.Visible = false;

        PageBase page = (PageBase)Page;
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

            if (Cache["M" + _topic.AuthorId] == null)
            {
                _author = Members.GetAuthor(_topic.AuthorId);
                Cache.Insert("M" + _topic.AuthorId, _author, null, DateTime.Now.AddMinutes(10d),
                                System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                _author = (AuthorInfo)Cache["M" + _topic.AuthorId];
            }
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
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0},{1}'}}, 'methodHandlers.BeginRecieve');return false;",
                        false,_topic.Id);
                //TopicHold.Visible = _topic.Status == Enumerators.PostStatus.UnModerated;
            }
            if (_topic.Status == (int)Enumerators.PostStatus.UnModerated || _topic.Status == (int)Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (_topic.Status == (int)Enumerators.PostStatus.OnHold)
                    modtext = String.Format("<span class=\"moderation\">!!{0}!!</span>", webResources.OnHold);

            }
            SplitTopic.Visible = false;
            hEdit.Text = webResources.lblEditTopic;
            hEdit.ToolTip = webResources.lblEditTopic;
            TopicDelete.AlternateText = webResources.lblDelTopic;
            TopicDelete.OnClientClick = "setArgAndPostBack('Do you want to delete the Topic?','DeleteTopic'," + ThisId + ");return false;";
            imgPosticon.OnClientClick = "setArgAndPostBack('Do you want to bookmark the Topic?','BookMarkTopic'," + ThisId + ");return false;";
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
                        "mainScreen.LoadServerControlHtml('Moderation',{{'pageID':7,'data':'{0},{1},{2}'}}, 'methodHandlers.BeginRecieve');return false;",
                        false,"",reply.Id);
                //TopicHold.Visible = reply.Status == Enumerators.PostStatus.UnModerated;
            }
            if (reply.Status == (int)Enumerators.PostStatus.UnModerated || reply.Status == (int)Enumerators.PostStatus.OnHold)
            {
                _unmoderated = true;
                modtext = String.Format("<span class=\"moderation\">{0}</span>", webResources.lblRequireModeration);
                if (reply.Status == (int)Enumerators.PostStatus.OnHold)
                    modtext = String.Format("<span class=\"moderation\">!!{0}!!</span>", webResources.OnHold);
            }

            TopicDelete.AlternateText = webResources.lblDelReply;
            SplitTopic.CommandArgument = ThisId.ToString();
            hEdit.ToolTip = webResources.lblEditReply;
            hEdit.Text = webResources.lblEditReply;
            TopicDelete.OnClientClick = "setArgAndPostBack('Do you want to delete the Reply?','DeleteReply'," + ThisId + ");return false;";
            imgPosticon.OnClientClick = "setArgAndPostBack('Do you want to bookmark the Reply?','BookMarkReply'," + ThisId + ");return false;";
            SplitTopic.Visible = _isForumModerator || _isadmin;
            SplitTopic.OnClientClick = String.Format(
                    "mainScreen.LoadServerControlHtml('Split Topic',{{'pageID':6,'data':'{0},asc'}}, 'methodHandlers.BeginRecieve');return false;", reply.Id);

        }
        TopicDelete.Visible = (currentUser.ToLower() == _author.Username.ToLower() &&
            !newerreplies);
        
        TopicDelete.Visible = TopicDelete.Visible || (_isForumModerator || _isadmin);
        imgPosticon.AlternateText = String.Format("#{0}", ThisId);


        date.Text = _unmoderated ? modtext : SnitzTime.TimeAgoTag(_postdate, page.IsAuthenticated, page.Member);
        
        
        ViewIP.Visible = _isadmin && Config.LogIP;
        ViewIP.OnClientClick = string.Format(
                        "mainScreen.LoadServerControlHtml('IP Lookup',{{'pageID':4,'data':'{0}'}}, 'methodHandlers.BeginRecieve');return false;",
                        _ip);

        hEdit.NavigateUrl = string.Format("~/Content/Forums/post.aspx?method=edit&type={0}&id={1}&TOPIC={2}", _posttype, ThisId, _topicid);
        hEdit.Visible = (currentUser.ToLower() == _author.Username.ToLower() && !newerreplies);
        hEdit.Visible = hEdit.Visible && !(_isTopicLocked || _forum.Status == (int)Enumerators.PostStatus.Closed);    // but not if it is locked
        hEdit.Visible = hEdit.Visible || _isForumModerator || _isadmin;      //override for admins/moderator

        hReplyQuote.Visible = page.IsAuthenticated && !(_isTopicLocked || _forum.Status == (int)Enumerators.PostStatus.Closed);
        hReplyQuote.Visible = hReplyQuote.Visible || (_isForumModerator || _isadmin);
        hReplyQuote.NavigateUrl = String.Format("~/Content/Forums/post.aspx?method=quote&type={0}&id={1}&TOPIC={2}", _posttype, ThisId, _topicid);
        
    }

    private void DeleteTopic(int topicid)
    {
        TopicInfo t = Topics.GetTopic(topicid);
        int forumid = t.ForumId;
        Topics.Delete(t);
        InvalidateForumCache();
        Response.Redirect("~/Content/Forums/Forum.aspx?FORUM=" + forumid);        
    }

    private void DeleteReply(int replyid)
    {

        try
        {
            Replies.DeleteReply(replyid);
            InvalidateForumCache();
        }
        catch { }
        if (ReplyDeleteClicked != null)
        {
            ReplyDeleteClicked(this, EventArgs.Empty);
        }
        //Request.Form["__EVENTARGUMENT"] = null;
    }

    private void BookMarkTopic(int topicid)
    {
        PageBase page = (PageBase)Page;
        TopicInfo t = Topics.GetTopic(topicid);
        string url = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}", t.Id);
        List<SnitzLink> bookmarks = page.Profile.BookMarks;
        if (!bookmarks.Contains(new SnitzLink(t.Subject, url, 0)))
        {
            bookmarks.Add(new SnitzLink(t.Subject, url, bookmarks.Count));
            page.Profile.BookMarks = bookmarks;
            page.Profile.Save();
        }
    }

    private void BookMarkReply(int replyid)
    {
        PageBase page = (PageBase)Page;
        ReplyInfo r = Replies.GetReply(replyid);
        TopicInfo rt = Topics.GetTopic(r.TopicId);
        string rurl = String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&whichpage={1}&#{2}", r.TopicId, page.CurrentPage + 1, r.Id);
        List<SnitzLink> rbookmarks = page.Profile.BookMarks;
        if (!rbookmarks.Contains(new SnitzLink(rt.Subject, rurl, 0)))
        {
            rbookmarks.Add(new SnitzLink(rt.Subject, rurl, rbookmarks.Count));
            page.Profile.BookMarks = rbookmarks;
            page.Profile.Save();
        }
    }

    private void InvalidateForumCache()
    {
        object obj = -1;
        Cache["RefreshKey"] = obj;
    }
}
