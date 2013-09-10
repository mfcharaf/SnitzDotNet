/*
####################################################################################################################
##
## SnitzUI.UserControls - Statistics.ascx
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
using System.Web;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;



public partial class ucStatistics : System.Web.UI.UserControl
{
    
    private const string ProfileUrl = "<a href=\"/Account/profile.aspx?user={0}\" title=\"{1}\" rel=\"NoFollow\">{0}</a>";
    private PageBase _page;
    private int _topicCount;
    private int _archiveTopicCount, _archiveReplyCount;
    private int _memberCount, _activeMembers;
    private int _totalPostCount;
    StatsInfo stats = Statistics.GetStatistics();

    public string ArchiveLabel
    {
        set
        {
            lblArchiveStats.Text = value;
        }
    }
    public string LastVisit
    {
        get
        {
            return lblLastVisit.Text;
        }
        set
        {
            lblLastVisit.Text = value;
        }
    }
    public string NewMember
    {
        set
        {
            lblNewestMember.Text = value;
        }
    }
    public string MemberStats
    {
        set
        {
            lblMemberStats.Text = value;
        }
    }
    public string TopicStats
    {
        set
        {
            lblTopicStats.Text = value;
        }
    }
    public string ActiveUsers
    {
        set { lblActiveUsers.Text = value;}
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //this.Stats_HeaderPanel.Attributes.Add("onclick", "TitleClick()");

        }
        if (Session["CurrentProfile"] != null)
            Session.Remove("CurrentProfile");
        _page = (PageBase)this.Page;
        
        if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
        {
            LastVisit = webResources.lblStatsLastVisit;
            LastVisit = LastVisit + Common.TimeAgoTag(_page.LastVisitDateTime, true, _page.Member != null ? _page.Member.TimeOffset : 0);
        }
        else
            lblLastVisit.Visible = false;

        string newmemberlink = String.Format(ProfileUrl, stats.NewestMember, String.Format(webResources.lblViewProfile, stats.NewestMember));
        NewMember = string.Format(webResources.lblStatsNewMember, newmemberlink);

        GetCounts();

        MemberStats = string.Format(webResources.lblStatsMembers, _activeMembers,Common.TranslateNumerals(_memberCount), Common.TranslateNumerals(_totalPostCount), GetLastPost(), GetLastPostAuthor());
        TopicStats = string.Format(webResources.lblStatsTopics, Common.TranslateNumerals(_topicCount), stats.ActiveTopicCount);
        ArchiveLabel = string.Format(webResources.lblStatsArchive, Common.TranslateNumerals(_archiveTopicCount + _archiveReplyCount), Common.TranslateNumerals(_archiveTopicCount));
        int activemembers = new SnitzMembershipProvider().GetNumberOfUsersOnline();
        int totalsessions = Convert.ToInt32(Application["SessionCount"]);
        int anonusers = totalsessions - activemembers;
        lblActiveSessions.Text = extras.GuestLabel + anonusers;
        ActiveUsers = string.Format(webResources.lblStatsMembersOnline,String.Join(",", new SnitzMembershipProvider().GetOnlineUsers()));
    }

    private string GetLastPost()
    {
        const string url = "<a href=\"/Content/Forums/topic.aspx?TOPIC={0}&whichpage=-1#{1}\">{2}</a>";
        TopicInfo lastpost = stats.LastPost;
        if (lastpost == null)
            return String.Empty;
        string lastpostDate = Common.TimeAgoTag(lastpost.LastPostDate.Value, HttpContext.Current.User.Identity.IsAuthenticated, _page.Member != null ? _page.Member.TimeOffset : 0);
        return String.Format(url, lastpost.Id, lastpost.LastReplyId, lastpostDate);
    }
    private string GetLastPostAuthor() 
    {
        var author = stats.LastPostAuthor;
        return author == null ? String.Empty : String.Format(ProfileUrl, author.Username, String.Format(webResources.lblViewProfile,author.Username));
    }

    private void GetCounts()
    {
        
        _topicCount = stats.TopicCount;
        _archiveTopicCount = stats.ArchiveTopicCount;
        _archiveReplyCount = stats.ArchiveReplyCount;
        _activeMembers = stats.ActiveMembers;
        _memberCount = stats.MemberCount;
        _totalPostCount = stats.TotalPostCount;

    }


}
