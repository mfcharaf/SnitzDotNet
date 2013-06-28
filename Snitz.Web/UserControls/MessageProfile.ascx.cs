using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;


public partial class MessageProfile : UserControl
{
    protected SnitzData.Member _author;

    private readonly bool _loggedonuser = HttpContext.Current.User.Identity.IsAuthenticated;

    public MessageProfile()
    {
        if(_author == null)
            base.Dispose();
    }
    public SnitzData.Member Author
    {
        get { return _author; }
        set
        {
            _author = value;
            SetProperties();
        }
    }

    private void SetProperties()
    {
        PageBase page = (PageBase)this.Page;

        if (Config.ShowRankStars || Config.ShowRankTitle)
        {
            Literal litRank = (Literal)FindControl("Rank");
            if (litRank != null)
            {
                if (Config.ShowRankTitle)
                    litRank.Text = _author.Rank.Title + @"<br/>";
                if (Config.ShowRankStars)
                {
                    litRank.Text += _author.Rank.Stars + @"<br/>";
                }
            }
        }

        
        ProfileCommon prof = ProfileCommon.GetUserProfile(_author.Name);
        if(prof.Gravatar)
        {
            Gravatar avatar = new Gravatar { Email = _author.Email };
            if (_author.AvatarUrl != "")
                avatar.DefaultImage = _author.AvatarUrl;
            phAvatar.Controls.Add(avatar);

        }else
        {
            SnitzMembershipUser mu = (SnitzMembershipUser)Membership.GetUser(_author.Name);
            Literal avatar = new Literal {Text = _author.Avatar};
            if (mu.IsActive)
                avatar.Text = avatar.Text.Replace("'avatar'", "'avatar online'");
            phAvatar.Controls.Add(avatar);
        }
        country.Text = _author.Country;
        posts.Text = String.Format("{0} {1}", Common.TranslateNumerals(_author.PostCount), webResources.lblPosts);

        hProf.Visible = _loggedonuser;
        hProf.OnClientClick =
            String.Format(
                "mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');return false;",_author.Id);

        if (!String.IsNullOrEmpty(_author.HomePage))
        {
            hHome.Visible = _loggedonuser && (_author.HomePage.Replace("http://", "").Trim() != string.Empty);
            hHome.NavigateUrl = string.Format("http://{0}", _author.HomePage.Replace("http://", ""));
            hHome.Text = hHome.ToolTip = Resources.webResources.lblHomePage;
        }
        if (!String.IsNullOrEmpty(_author.ICQ))
        {
            hICQ.Visible = _loggedonuser && (_author.ICQ.Trim() != "");
            hICQ.NavigateUrl = string.Format("http://www.icq.com/people/webmsg.php?to={0}", _author.ICQ);
            hICQ.Text = hICQ.ToolTip = Resources.webResources.lblICQ;
        }
        if (!String.IsNullOrEmpty(_author.Yahoo))
        {
            hYAHOO.Visible = _loggedonuser && (_author.Yahoo.Trim() != "");
            hYAHOO.NavigateUrl = string.Format("http://edit.yahoo.com/config/send_webmesg?.target={0}&;.src=pg",
                                               _author.Yahoo);
            hYAHOO.Text = hYAHOO.ToolTip = Resources.webResources.lblYAHOO;
        }

        hEmail.Visible = ((_loggedonuser || !Config.LogonForEmail) && _author.ReceiveEmails) || page.IsAdministrator;
        hEmail.NavigateUrl = "#";
        hEmail.Attributes.Add("onclick",
                                     string.Format(
                                         "mainScreen.LoadServerControlHtml('Email Member',{{'pageID':10,'data':{0}}},'methodHandlers.BeginRecieve');return false;",
                                         _author.Id));
        //hEmail.Text = hEmail.ToolTip = Resources.webResources.lblEmail + @" " + _author.Name;

        //hAIM.Visible = _loggedonuser && (_author.AIM.Trim() != "");
        //hAIM.NavigateUrl = "javascript:openWindow('pop_messengers.aspx?mode=AIM&user=" + _author.UserName + "&id=" + _author.AIM + "')";
        //hAIM.Text = hAIM.ToolTip = Resources.webResources.lblAIM;

        hMSN.Visible = false; // (drv.Row.ItemArray[15].ToString().Trim() != "");
        hMSN.NavigateUrl = "javascript:openWindow('pop_messengers.aspx?mode=MSN&user=" + _author.Name + "&amp;id=" + _author.MSN + "')";
        hMSN.Text = hMSN.ToolTip = Resources.webResources.lblMSN;

        hSKYPE.Visible = false;
        hSKYPE.NavigateUrl = "";
        hSKYPE.Text = hSKYPE.ToolTip = Resources.webResources.lblSkype;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        var privateSend = LoadControl("~/UserControls/PrivateMessaging/pmSend.ascx");
        phPrivateSend.Controls.Add(privateSend);
    }

}
