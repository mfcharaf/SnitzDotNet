/*'
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
using System;
using System.Web;
using Resources;
using SnitzCommon;
using Snitz.Providers;
using SnitzData;


public partial class Statistics : System.Web.UI.UserControl
{
    
    private const string ProfileUrl = "<a href=\"/Account/profile.aspx?user={0}\" title=\"{1}\" rel=\"NoFollow\">{0}</a>";
    private PageBase _page;
    private int _topicCount;
    private int _archiveTopicCount, _archiveReplyCount;
    private int _memberCount, _activeMembers;
    private int _totalPostCount;

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

        string newmemberlink = String.Format(ProfileUrl, Stats.NewestMember, String.Format(webResources.lblViewProfile, Stats.NewestMember));
        NewMember = string.Format(webResources.lblStatsNewMember, newmemberlink);

        GetCounts();

        MemberStats = string.Format(webResources.lblStatsMembers, _activeMembers,Common.TranslateNumerals(_memberCount), Common.TranslateNumerals(_totalPostCount), GetLastPost(), GetLastPostAuthor());
        TopicStats = string.Format(webResources.lblStatsTopics, Common.TranslateNumerals(_topicCount), Stats.ActiveTopicCount);
        ArchiveLabel = string.Format(webResources.lblStatsArchive, Common.TranslateNumerals(_archiveTopicCount + _archiveReplyCount), Common.TranslateNumerals(_archiveTopicCount));
        int activemembers = new SnitzMembershipProvider().GetNumberOfUsersOnline();
        int totalsessions = Convert.ToInt32(Application["SessionCount"]);
        int anonusers = totalsessions - activemembers;
        lblActiveSessions.Text = extras.GuestLabel + anonusers;
        ActiveUsers = string.Format(webResources.lblStatsMembersOnline,
                                    String.Join(",", new SnitzMembershipProvider().GetOnlineUsers()));
    }

    private string GetLastPost()
    {
        const string url = "<a href=\"/Content/Forums/topic.aspx?TOPIC={0}&REPLY={1}\">{2}</a>";
        Topic lastpost = Stats.LastPost;
        if (lastpost == null)
            return String.Empty;
        string lastpostDate = Common.TimeAgoTag(lastpost.LastPostDate, HttpContext.Current.User.Identity.IsAuthenticated, _page.Member != null ? _page.Member.TimeOffset : 0);
        return String.Format(url, lastpost.Id, lastpost.LastReplyId, lastpostDate);
    }
    private static string GetLastPostAuthor() 
    {
        var author = (Member)Stats.LastPostAuthor;
        return author == null ? String.Empty : String.Format(ProfileUrl, author.Name, String.Format(webResources.lblViewProfile,author.Name));
    }

    private void GetCounts()
    {

        _topicCount = Stats.TopicCount;
        _archiveTopicCount = Stats.ArchiveTopicCount;
        _archiveReplyCount = Stats.ArchiveReplyCount;
        _activeMembers = Stats.ActiveMembers;
        _memberCount = Stats.MemberCount;
        _totalPostCount = Stats.TotalPostCount;

    }


}
