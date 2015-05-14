using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SnitzCommon;
using SnitzConfig;

public partial class Admin_features : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
            GetValues();
    }
    void GetValues()
    {
        rblIPLog.SelectedValue = Config.LogIP ? "1":"0";
        rblFlood.SelectedValue = Config.FloodCheck ? "1" : "0";
        ddlFlood.SelectedValue = Config.FloodTimeout.ToString();
        rblGroups.SelectedValue = Config.ShowGroups ? "1" : "0";
        rblBadWords.SelectedValue = Config.FilterBadWords ? "1" : "0";
        rblCaptchaLogin.SelectedValue = Config.UseCaptchaLogin ? "1" : "0";
        rblUserGallery.SelectedValue = Config.UserGallery ? "1" : "0";
        rblShowGallery.SelectedValue = Config.ShowGallery ? "1" : "0";
        rblHotTopics.SelectedValue = Config.HotTopicNum > 0 ? "1" : "0";
        ddlSubs.SelectedValue = ((int)Config.SubscriptionLevel).ToString();
        rblTopicMod.SelectedValue = Config.Moderation ? "1" : "0";

        rblRestrictMove.SelectedValue = Config.RestrictModeratorMove ? "1" : "0";
        rblMoveNotify.SelectedValue = Config.MoveNotify ? "1" : "0";

        rblArchive.SelectedValue = Config.Archive ? "1" : "0";
        rblStats.SelectedValue = Config.ShowStats ? "1" : "0";
        rblSearchAllForums.SelectedValue = Config.AllowSearchAllForums ? "1" : "0";

        rblEditedBy.SelectedValue = Config.ShowEditBy ? "1" : "0";
        rblTopicAvatar.SelectedValue = Config.TopicAvatar ? "1" : "0";
        rblPrevNext.SelectedValue = Config.ShowTopicNav ? "1" : "0";
        rblSend.SelectedValue = Config.SendTopic ? "1" : "0";
        rblPrint.SelectedValue = Config.PrintTopic ? "1" : "0";
        tbxHotTopics.Text = Config.HotTopicNum.ToString();
        tbxPageItems.Text = Config.TopicPageSize.ToString();
        tbxSearchItems.Text = Config.SearchPageSize.ToString();

        rblForumCode.SelectedValue = Config.AllowForumCode ? "1" : "0";
        rblIcons.SelectedValue = Config.AllowIcons ? "1" : "0";
        rblImages.SelectedValue = Config.AllowImages ? "1" : "0";

        rblPageTimer.SelectedValue = Config.ShowTimer ? "1" : "0";
        rblSmilieTable.SelectedValue = Config.EmoticonTable ? "1" : "0";
        rblQuickReply.SelectedValue = Config.ShowQuickReply ? "1" : "0";
        rblSignatures.SelectedValue = Config.AllowSignatures ? "1" : "0";

        rblPrivateMessages.SelectedValue = Config.PrivateMessaging ? "1" : "0";
        ddlShowRank.SelectedValue = ((int)Config.ShowRankType).ToString();

        rblAnnouncement.SelectedValue = Config.Announcement ? "1" : "0";
        tbxAnnouncement.Text = Config.AuthMessage.StripCData();
        tbxAnonAnnouncement.Text = Config.AnonMessage.StripCData();

        txtAnonMembers.Text = String.Join(",", Config.AnonMembers.ToArray());
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();

        if (Config.LogIP != (rblIPLog.SelectedValue == "1"))
            toUpdate.Add("LogIP".GetPropertyDescription(), rblIPLog.SelectedValue);
        if (Config.FloodCheck != (rblFlood.SelectedValue == "1"))
            toUpdate.Add("FloodCheck".GetPropertyDescription(), rblFlood.SelectedValue);
        if (Config.FloodTimeout != Convert.ToInt32(ddlFlood.SelectedValue))
            toUpdate.Add("FloodTimeout".GetPropertyDescription(), ddlFlood.SelectedValue);
        if (Config.ShowGroups != (rblGroups.SelectedValue == "1"))
            toUpdate.Add("ShowGroups".GetPropertyDescription(), rblGroups.SelectedValue);
        if (Config.FilterBadWords != (rblBadWords.SelectedValue == "1"))
            toUpdate.Add("FilterBadWords".GetPropertyDescription(), rblBadWords.SelectedValue);
        
        if (Config.UseCaptchaLogin != (rblCaptchaLogin.SelectedValue == "1"))
            toUpdate.Add("UseCaptchaLogin".GetPropertyDescription(), rblCaptchaLogin.SelectedValue);
        if (Config.UserGallery != (rblUserGallery.SelectedValue == "1"))
            toUpdate.Add("UserGallery".GetPropertyDescription(), rblUserGallery.SelectedValue);
        if (Config.ShowGallery != (rblShowGallery.SelectedValue == "1"))
            toUpdate.Add("ShowGallery".GetPropertyDescription(), rblShowGallery.SelectedValue);
        if (Config.SubscriptionLevel != (Enumerators.SubscriptionLevel)Convert.ToInt32(ddlSubs.SelectedValue))
            toUpdate.Add("SubscriptionLevel".GetPropertyDescription(), ddlSubs.SelectedValue);
        if (Config.Moderation != (rblTopicMod.SelectedValue == "1"))
            toUpdate.Add("Moderation".GetPropertyDescription(), rblTopicMod.SelectedValue);

        if (Config.RestrictModeratorMove != (rblRestrictMove.SelectedValue == "1"))
            toUpdate.Add("RestrictModeratorMove".GetPropertyDescription(), rblRestrictMove.SelectedValue);
        if (Config.MoveNotify != (rblMoveNotify.SelectedValue == "1"))
            toUpdate.Add("MoveNotify".GetPropertyDescription(), rblMoveNotify.SelectedValue);
        if (Config.ShowStats != (rblStats.SelectedValue == "1"))
            toUpdate.Add("ShowStats".GetPropertyDescription(), rblStats.SelectedValue);
        if (Config.AllowSearchAllForums != (rblSearchAllForums.SelectedValue == "1"))
            toUpdate.Add("AllowSearchAllForums".GetPropertyDescription(), rblSearchAllForums.SelectedValue);
        if (Config.ShowEditBy != (rblEditedBy.SelectedValue == "1"))
            toUpdate.Add("ShowEditBy".GetPropertyDescription(), rblEditedBy.SelectedValue);
        if (Config.TopicAvatar != (rblTopicAvatar.SelectedValue == "1"))
            toUpdate.Add("TopicAvatar".GetPropertyDescription(), rblTopicAvatar.SelectedValue);
        if (Config.ShowTopicNav != (rblPrevNext.SelectedValue == "1"))
            toUpdate.Add("ShowTopicNav".GetPropertyDescription(), rblPrevNext.SelectedValue);
        if (Config.SendTopic != (rblSend.SelectedValue == "1"))
            toUpdate.Add("SendTopic".GetPropertyDescription(), rblSend.SelectedValue);
        if (Config.PrintTopic != (rblPrint.SelectedValue == "1"))
            toUpdate.Add("PrintTopic".GetPropertyDescription(), rblPrint.SelectedValue);
        if (rblHotTopics.SelectedValue == "0")
            tbxHotTopics.Text = "0";
        if (Config.HotTopicNum != Convert.ToInt32(tbxHotTopics.Text))
            toUpdate.Add("HotTopicNum".GetPropertyDescription(), tbxHotTopics.Text);
        if (Config.TopicPageSize != Convert.ToInt32(tbxPageItems.Text))
            toUpdate.Add("TopicPageSize".GetPropertyDescription(), tbxPageItems.Text);
        if (Config.SearchPageSize != Convert.ToInt32(tbxSearchItems.Text))
            toUpdate.Add("SearchPageSize".GetPropertyDescription(), tbxSearchItems.Text);

        if (Config.AllowForumCode != (rblForumCode.SelectedValue =="1"))
            toUpdate.Add("AllowForumCode".GetPropertyDescription(), rblForumCode.SelectedValue);
        if (Config.AllowIcons != (rblIcons.SelectedValue == "1"))
            toUpdate.Add("AllowIcons".GetPropertyDescription(), rblIcons.SelectedValue);
        if (Config.AllowImages != (rblImages.SelectedValue == "1"))
            toUpdate.Add("AllowImages".GetPropertyDescription(), rblImages.SelectedValue);
        if (Config.ShowTimer != (rblPageTimer.SelectedValue == "1"))
            toUpdate.Add("ShowTimer".GetPropertyDescription(), rblPageTimer.SelectedValue);
        if (Config.EmoticonTable != (rblSmilieTable.SelectedValue == "1"))
            toUpdate.Add("EmoticonTable".GetPropertyDescription(), rblSmilieTable.SelectedValue);
        if (Config.ShowQuickReply != (rblQuickReply.SelectedValue == "1"))
            toUpdate.Add("ShowQuickReply".GetPropertyDescription(), rblQuickReply.SelectedValue);
        if (Config.AllowSignatures != (rblSignatures.SelectedValue == "1"))
            toUpdate.Add("AllowSignatures".GetPropertyDescription(), rblSignatures.SelectedValue);
        if (Config.ShowRankType != (Enumerators.RankType)Convert.ToInt32(ddlShowRank.SelectedValue))
            toUpdate.Add("ShowRankType".GetPropertyDescription(), ddlShowRank.SelectedValue);
        if (Config.Announcement != (rblAnnouncement.SelectedValue == "1"))
            toUpdate.Add("Announcement".GetPropertyDescription(), rblAnnouncement.SelectedValue);
        if (Config.AnonMessage != tbxAnonAnnouncement.Text)
        {
            toUpdate.Add("AnonMessage".GetPropertyDescription(),tbxAnonAnnouncement.Text.WrapCData());
        }
        if (Config.AuthMessage != tbxAnnouncement.Text)
        {
            toUpdate.Add("AuthMessage".GetPropertyDescription(), tbxAnnouncement.Text.WrapCData());
        }
        if(Config.Archive != (rblArchive.SelectedValue == "1"))
            toUpdate.Add("Archive".GetPropertyDescription(),rblArchive.SelectedValue);
        if (Config.PrivateMessaging != (rblPrivateMessages.SelectedValue == "1"))
            toUpdate.Add("PrivateMessaging".GetPropertyDescription(), rblPrivateMessages.SelectedValue);
        List<string> newAM = txtAnonMembers.Text.Split(',').ToList();
        IEnumerable<string> difference = Config.AnonMembers.Except(newAM);
        if (!difference.Any())
        {
            difference = newAM.Except(Config.AnonMembers);
            if (difference.Any())
            {
                Config.AnonMembers.Clear();
                toUpdate.Add("AnonMembers".GetPropertyDescription(), txtAnonMembers.Text);               
            }
        }
        else
        {
            Config.AnonMembers.Clear();
            toUpdate.Add("AnonMembers".GetPropertyDescription(), txtAnonMembers.Text); 
        }
        Config.UpdateKeys(toUpdate);

    }

}
