/*
####################################################################################################################
##
## SnitzUI.UserControls.Sidebar - MiniStatistics.ascx
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
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;


public partial class MiniStatistics : System.Web.UI.UserControl
{
    
    private const string PROFILE_URL = "<a href=\"/Account/profile.aspx?user={0}\" title=\"{1}\" rel=\"NoFollow\">{0}</a>";
    
    private PageBase _page;
    private StatsInfo _stats;
    private int _memberCount;
    private int _totalPostCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["CurrentProfile"] != null)
            Session.Remove("CurrentProfile");
        _page = (PageBase) Page;        
        _stats = Statistics.GetStatistics();
        if (_stats == null)
            return;
        PopulateData();
    }

    private void PopulateData()
    {
        int activemembers = new SnitzMembershipProvider().GetNumberOfUsersOnline();
        int totalsessions = Convert.ToInt32(Application["SessionCount"]);
        int dailySessions = Convert.ToInt32(Application["DailyCount"]);
        var dSession = String.Format("{0} visitor(s) today", dailySessions);
        int anonusers = totalsessions - activemembers;

        GetCounts();
        string newmemberlink = String.Format(PROFILE_URL, _stats.NewestMember,String.Format(webResources.lblViewProfile, _stats.NewestMember));
        
        lblNewestMember.Text = string.Format(webResources.lblMiniStatsNewMember, newmemberlink);
        lblMemberStats.Text = string.Format(webResources.lblMiniStatsMembers, Common.TranslateNumerals(_memberCount), Common.TranslateNumerals(_totalPostCount), GetLastPost(), GetLastPostAuthor());
        lblTopicStats.Text = string.Format(webResources.lblMiniStatsTopics, Common.TranslateNumerals(_stats.ActiveTopicCount));
        lblActiveSessions.Text = extras.GuestLabel + Common.TranslateNumerals(anonusers);
        string[] onlineusers = new SnitzMembershipProvider().GetOnlineUsers();
        var remains = onlineusers.Except(Config.AnonMembers);
        lblActiveUsers.Text = string.Format(webResources.lblStatsMembersOnline, String.Join(",", remains.ToArray())) + "<br/>" + dSession;
    }

    private string GetLastPost()
    {
        if (_stats.LastPost == null)
            return "";
        const string url = "<a href=\"/Content/Forums/topic.aspx?TOPIC={0}&whichpage=-1#{1}\">{2}</a>";
        TopicInfo lastpost = _stats.LastPost;

        if (lastpost.LastPostDate != null)
        {
            string lastpostDate = SnitzTime.TimeAgoTag(lastpost.LastPostDate.Value, _page.IsAuthenticated, _page.Member);
            return String.Format(url, lastpost.Id, lastpost.LastReplyId, lastpostDate);
        }
        return String.Empty;
    }
    
    private string GetLastPostAuthor() 
    {
        var author = _stats.LastPostAuthor;
        return author == null ? String.Empty : String.Format(PROFILE_URL, author.Username, String.Format(webResources.lblViewProfile,author.Username));
    }

    private void GetCounts()
    {

        _memberCount = _stats.MemberCount;
        _totalPostCount = _stats.TotalPostCount;

    }
}
