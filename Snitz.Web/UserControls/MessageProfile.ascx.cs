/*
####################################################################################################################
##
## SnitzUI.UserControls - MessageProfile.ascx
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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;


public partial class MessageProfile : UserControl
{
    private int _authorId;
    private AuthorInfo _author;
    private readonly bool _loggedonuser = HttpContext.Current.User.Identity.IsAuthenticated;

    public MessageProfile()
    {
        if(_author == null)
            base.Dispose();
    }
    public int AuthorId
    {
        get { return _authorId; }
        set
        {
            _authorId = value;
            _author = Members.GetAuthor(_authorId);
            SetProperties();
        }
    }

    private void SetProperties()
    {
        PageBase page = (PageBase)Page;

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


        ProfileCommon prof = ProfileCommon.GetUserProfile(_author.Username);
        if(prof.Gravatar)
        {
            Gravatar avatar = new Gravatar { Email = _author.Email };
            if (_author.AvatarUrl != "")
                avatar.DefaultImage = _author.AvatarUrl;
            phAvatar.Controls.Add(avatar);

        }else
        {
            SnitzMembershipUser mu = (SnitzMembershipUser)Membership.GetUser(_author.Username);
            Literal avatar = new Literal { Text = _author.AvatarUrl };
            if (mu != null && mu.IsActive)
                avatar.Text = avatar.Text.Replace("'avatar'", "'avatar online'");
            phAvatar.Controls.Add(avatar);
        }
        country.Text = _author.Country;
        posts.Text = String.Format("{0} {1}", Common.TranslateNumerals(_author.PostCount), webResources.lblPosts);

        hProf.Visible = _loggedonuser;
        hProf.OnClientClick =
            String.Format(
                "mainScreen.LoadServerControlHtml('Public Profile',{{'pageID':1,'data':{0}}}, 'methodHandlers.BeginRecieve');return false;", _author.Id);

        if (!String.IsNullOrEmpty(_author.HomePage))
        {
            hHome.Visible = _loggedonuser && (_author.HomePage.Replace("http://", "").Trim() != string.Empty);
            hHome.NavigateUrl = string.Format("http://{0}", _author.HomePage.Replace("http://", ""));
            hHome.Text = hHome.ToolTip = webResources.lblHomePage;
        }
        if (!String.IsNullOrEmpty(_author.ICQ))
        {
            hICQ.Visible = _loggedonuser && (_author.ICQ.Trim() != "");
            hICQ.NavigateUrl = string.Format("http://www.icq.com/people/webmsg.php?to={0}", _author.ICQ);
            hICQ.Text = hICQ.ToolTip = webResources.lblICQ;
        }
        if (!String.IsNullOrEmpty(_author.Yahoo))
        {
            hYAHOO.Visible = _loggedonuser && (_author.Yahoo.Trim() != "");
            hYAHOO.NavigateUrl = string.Format("http://edit.yahoo.com/config/send_webmesg?.target={0}&;.src=pg",
                                               _author.Yahoo);
            hYAHOO.Text = hYAHOO.ToolTip = webResources.lblYAHOO;
        }

        hEmail.Visible = (((_loggedonuser || !Config.LogonForEmail) && _author.ReceiveEmails) || page.IsAdministrator) && Config.UseEmail;
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
        hMSN.NavigateUrl = "javascript:openWindow('pop_messengers.aspx?mode=MSN&user=" + _author.Username + "&amp;id=" + _author.MSN + "')";
        hMSN.Text = hMSN.ToolTip = webResources.lblMSN;

        hSKYPE.Visible = false;
        hSKYPE.NavigateUrl = "";
        hSKYPE.Text = hSKYPE.ToolTip = webResources.lblSkype;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (_loggedonuser)
        {
            var privateSend = LoadControl("~/UserControls/PrivateMessages/pmSend.ascx");
            phPrivateSend.Controls.Add(privateSend);
        }
    }

}
