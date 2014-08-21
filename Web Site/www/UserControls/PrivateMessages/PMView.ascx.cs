﻿/*
####################################################################################################################
##
## PrivateMessaging.UserControls.PrivateMessaging - pmView.ascx
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
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzMembership;

namespace SnitzUI.UserControls.PrivateMessaging
{
    public partial class PMView : UserControl
    {
        public int CurrentPage
        {
            get
            {
                // look for current page in ViewState
                object o = ViewState["_CurrentPage"];
                if (o == null)
                    return 1;
                return (int)o;
            }

            set
            {
                ViewState["_CurrentPage"] = value;
            }
        }
        private string _redirectionScript;
        private string _layout;
        private bool _showOutBox;
        private readonly string username = HttpContext.Current.User.Identity.Name;
        private int _pages;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            int totalmembers = Snitz.BLL.PrivateMessages.GetMemberCount(Session["_searchfor"] != null ? Session["_searchfor"].ToString() : String.Empty);
            _pages = Common.CalculateNumberOfPages(totalmembers, 11);
            SetupPager();

            _redirectionScript =
             "function Delayer(){" +
             "setTimeout('Redirection()', 3000);" +
             "};" +
             "function Redirection(){" +
             "window.location = '" + Request.RawUrl + "';" +
             "};" +
             "Delayer();";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = Membership.GetUser(username);
            if (currentUser == null)
                throw new SecurityException("Access Denied");

            ClientScriptManager cs = Page.ClientScript;
            const string csname1 = "chkBoxScript";
            Type cstype = GetType();
            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                var cstext1 = new StringBuilder();
                cstext1.Append("<script type=text/javascript>");
                cstext1.AppendFormat("var allCheckBoxSelector = '#{0} input[id$=\"chkAll\"]:checkbox';", InBox.FindControl("grdInbox").ClientID);
                cstext1.AppendFormat("var checkBoxSelector = '#{0} input[id$=\"cbxDel\"]:checkbox';", InBox.FindControl("grdInbox").ClientID);
                cstext1.Append("$(document).ready(function () {");
                cstext1.Append("$(allCheckBoxSelector).live('click', function () {");
                cstext1.Append("$(checkBoxSelector).attr('checked', $(this).is(':checked'));");
                cstext1.Append("ToggleCheckUncheckAllOptionAsNeeded();");
                cstext1.Append("});");
                cstext1.Append("$(checkBoxSelector).live('click', ToggleCheckUncheckAllOptionAsNeeded);");
                cstext1.Append("ToggleCheckUncheckAllOptionAsNeeded();");
                cstext1.Append("});   ");
                cstext1.AppendLine();
                cstext1.Append("function ToggleCheckUncheckAllOptionAsNeeded() {");
                cstext1.Append("var totalCheckboxes = $(checkBoxSelector),");
                cstext1.Append("checkedCheckboxes = totalCheckboxes.filter(\":checked\"),");
                cstext1.Append("noCheckboxesAreChecked = (checkedCheckboxes.length === 0),");
                cstext1.Append("allCheckboxesAreChecked = (totalCheckboxes.length === checkedCheckboxes.length);");
                cstext1.Append("$(allCheckBoxSelector).attr('checked', allCheckboxesAreChecked);");
                cstext1.Append("}");
                cstext1.Append("</script>");
                cs.RegisterStartupScript(cstype, csname1, cstext1.ToString());
            }

            var profile = Snitz.BLL.PrivateMessages.GetPreferences(currentUser.UserName);
            _layout = profile == null ? "double" : profile.PMLayout;

            if (String.IsNullOrEmpty(_layout))
                _layout = "double";
            switch (_layout)
            {
                case "double":
                    _showOutBox = false;
                    break;
                case "single":
                    _showOutBox = true;
                    break;
                case "none":
                    _showOutBox = false;
                    ButtonOutBox.Visible = false;
                    break;
            }

            if (!IsPostBack)
            {
                SelectPanels();
            }
            SetButtonDisplay();
            BindMemberList(Session["_searchfor"] != null ? Session["_searchfor"].ToString() : String.Empty);
        }

        private void BindMemberList(string searchfor)
        {
            if (Session["_searchfor"] != null)
            {
                searchfor = Session["_searchfor"].ToString();
                Session["SearchFilter"] = "Name," + searchfor;
            }
            Members.DataSource = Snitz.BLL.Members.GetAllMembers("",CurrentPage,15);
            Members.DataBind();
            int totalmembers = Snitz.BLL.PrivateMessages.GetMemberCount(searchfor);
            _pages = Common.CalculateNumberOfPages(totalmembers, 11);

            numPages.Text = _pages.ToString();
        }

        private void SelectPanels()
        {
            if (InBox.Visible)
            {
                BindInbox();
                OutBox.Visible = false;
                ButtonOutBox.Visible = _layout == "double";
                ButtonInBox.Visible = false;
            }
            if (_showOutBox)
            {
                OutBox.Visible = true;
                ButtonOutBox.Visible = _layout == "double";
                ButtonInBox.Visible = false;
                BindOutbox();
            }
        }

        private void BindOutbox()
        {
            MembershipUser currentUser = Membership.GetUser(username);
            if (currentUser != null && currentUser.ProviderUserKey != null)
                grdOutBox.DataSource = Snitz.BLL.PrivateMessages.GetSentMessages((int)currentUser.ProviderUserKey);
            grdOutBox.DataBind();
        }

        private void BindInbox()
        {
            MembershipUser currentUser = Membership.GetUser(username);
            if (currentUser != null && currentUser.ProviderUserKey != null)
                grdInBox.DataSource = Snitz.BLL.PrivateMessages.GetMessages((int)currentUser.ProviderUserKey);
            grdInBox.DataBind();
        }

        protected void SaveOptions(object sender, EventArgs eventArgs)
        {
            MembershipUser currentUser = Membership.GetUser(username);
            string layout = rblLayout.SelectedValue;
            if (currentUser != null)
                Snitz.BLL.PrivateMessages.SavePreferences(username, rblEnabled.SelectedValue, rblNotify.SelectedValue, layout);

            lblResult.Text = Resources.PrivateMessage.PmOptionsSaved;

            ScriptManager.RegisterStartupScript(this, GetType(), "Startup", _redirectionScript, true);

        }

        protected void btnOptions_Click(object sender, ImageClickEventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser(username);
            if (currentUser != null)
            {
                var userprefs = Snitz.BLL.PrivateMessages.GetPreferences(currentUser.UserName);
                PMViews.ActiveViewIndex = 3;
                rblLayout.SelectedValue = _layout;
                if (userprefs != null)
                {
                    rblEnabled.SelectedValue = userprefs.PMReceive.HasValue ? userprefs.PMReceive.Value.ToString() : "1";
                    rblNotify.SelectedValue = userprefs.PMEmail.ToString();
                }
                else
                {
                    rblEnabled.SelectedValue = "1";
                    rblNotify.SelectedValue = "1";
                }
            }
            else
            {
                Response.Write("couldn't get prefs");
            }
            PMViews.ActiveViewIndex = 3;
        }

        protected void btnOutBox_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            PMViews.ActiveViewIndex = 0;
            switch (btn.CommandArgument)
            {
                case "inbox":
                    InBox.Visible = true;
                    OutBox.Visible = false;
                    ButtonOutBox.Visible = true;
                    ButtonInBox.Visible = false;
                    BindInbox();
                    break;
                case "outbox":
                    InBox.Visible = false;
                    OutBox.Visible = true;
                    ButtonOutBox.Visible = false;
                    ButtonInBox.Visible = true;
                    BindOutbox();
                    break;
            }
        }

        protected void InboxBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image imgRead = e.Row.Cells[0].FindControl("pmImgRead") as Image;
                Image imgUnread = e.Row.Cells[0].FindControl("pmImgUnRead") as Image;
                var pm = (PrivateMessageInfo)e.Row.DataItem;
                if (pm.Read == 1)
                {
                    if (imgRead != null) imgRead.Visible = true;
                    if (imgUnread != null) imgUnread.Visible = false;
                }
                else
                {
                    if (imgRead != null) imgRead.Visible = false;
                    if (imgUnread != null) imgUnread.Visible = true;
                }
            }
        }

        protected void btnReceive_Click(object sender, ImageClickEventArgs e)
        {
            PMViews.ActiveViewIndex = 0;
            OutBox.Visible = false;
            InBox.Visible = true;
            SelectPanels();
            SetButtonDisplay();
        }

        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            PMViews.ActiveViewIndex = 1;
            SetButtonDisplay();
        }

        protected void btnCancel_Click(object sender, EventArgs eventArgs)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void btnSend_Click(object sender, EventArgs eventArgs)
        {
            MembershipUser currentUser = Membership.GetUser(username);

            if (currentUser == null || currentUser.ProviderUserKey == null)
                return;

            string[] toMembers = Regex.Split(tbxRecipient.Text, ";");
            foreach (string member in toMembers)
            {
                ProfileCommon profile = ProfileCommon.GetUserProfile(member);
                MembershipUser recipient = Membership.GetUser(member, false);
                if (recipient != null && recipient.ProviderUserKey != null)
                {
                    var pm = new PrivateMessageInfo
                    {
                        FromMemberId = (int) currentUser.ProviderUserKey,
                        Read = 0,
                        Subject = tbxSubject.Text,
                        Message = qrMessage.Text,
                        OutBox = _layout != "none" ? 1 : 0,
                        SentDate = DateTime.UtcNow.ToForumDateStr(),
                        ToMemberId = (int) recipient.ProviderUserKey,
                        Mail = profile.PMEmail == null ? 0 : profile.PMEmail.Value
                    };

                    Snitz.BLL.PrivateMessages.SendPrivateMessage(pm);
                }

            }
            //TODO: Send notify if required
            pmSuccess.Text = Resources.PrivateMessage.PmSent;
            ScriptManager.RegisterStartupScript(this, GetType(), "Startup", _redirectionScript, true);

        }

        protected void ViewMessage(object sender, EventArgs e)
        {

            LinkButton lnk = (LinkButton)sender;
            PrivateMessageInfo pm = Snitz.BLL.PrivateMessages.GetMessage(Convert.ToInt32(lnk.CommandArgument));
            DisplayMessage(pm);
            ButtonReply.CommandArgument = lnk.CommandArgument;
            ButtonReplyQuote.CommandArgument = lnk.CommandArgument;
            ButtonForward.CommandArgument = lnk.CommandArgument;
            ButtonDelete.CommandArgument = lnk.CommandArgument;

            SetButtonDisplay();
        }
        protected void ViewSentMessage(object sender, EventArgs e)
        {
            var lnk = (LinkButton)sender;
            PrivateMessageInfo pm = Snitz.BLL.PrivateMessages.GetMessage(Convert.ToInt32(lnk.CommandArgument));
            pm.FromMember = Snitz.BLL.Members.GetMember(pm.FromMemberId);

            if (pm.FromMember.ProfileData != null && pm.FromMember.ProfileData.Gravatar == 1)
            {
                string avatar = String.Format("{0}/Avatars/{1}", Common.GetSiteRoot(), String.IsNullOrEmpty(pm.FromMember.Avatar) ? "default.gif" : pm.FromMember.Avatar);

                var gravatar = new Gravatar { Email = pm.FromMember.Email };
                if (avatar != "")
                    gravatar.DefaultImage = avatar;
                phAvatar.Controls.Add(gravatar);
            }
            else
            {
                var avatar = new Literal { Text = pm.FromMember.AvatarUrl };
                phAvatar.Controls.Add(avatar);
            }
            PMViews.ActiveViewIndex = 2;
            pmRecipients.Text = "To: " + pm.ToMemberName;
            pmFrom.Text = String.Format("<a href=\"/Account/profile.aspx?user={0}\">{0}</a>", pm.FromMemberName);
            pmTitle.Text = pm.FromMember.Title;
            pmCountry.Text = pm.FromMember.Country;
            pmPostcount.Text = pm.FromMember.PostCount.ToString();
            pmDate.Text = SnitzTime.TimeAgoTag(pm.Sent, true, pm.FromMember);
            pmSubject.Visible = true;
            pmSubject.Text = pm.Subject;
            pmBody.Text = pm.Message.ParseTags();
            SetButtonDisplay();
            ButtonReply.Visible = false;
            ButtonReplyQuote.Visible = false;
            ButtonForward.Visible = false;
            ButtonDelete.Visible = false;
        }

        protected void btnDelMessage_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdInBox.Rows)
            {
                var cb = row.FindControl("cbxDel") as CheckBox;

                if (cb != null)
                    if (cb.Checked)
                    {
                        var dataKey = grdInBox.DataKeys[row.RowIndex];
                        if (dataKey != null)
                        {
                            var currentPmId = dataKey.Value;
                            Snitz.BLL.PrivateMessages.DeletePrivateMessage((int)currentPmId);
                        }
                    }
            }
            pmSuccess.Text = Resources.PrivateMessage.PmSent;
            ScriptManager.RegisterStartupScript(this, GetType(), "Startup", _redirectionScript, true);
        }

        protected void btnRemMessage_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdOutBox.Rows)
            {
                var cb = row.FindControl("cbxDel") as CheckBox;

                if (cb != null)
                    if (cb.Checked)
                    {
                        var dataKey = grdOutBox.DataKeys[row.RowIndex];
                        if (dataKey != null)
                        {
                            var currentPMId = dataKey.Value;
                            Snitz.BLL.PrivateMessages.RemoveFromOutBox((int)currentPMId);
                        }
                    }
            }
            pmSuccess.Text = Resources.PrivateMessage.PmMessageRemoved;
            ScriptManager.RegisterStartupScript(this, GetType(), "Startup", _redirectionScript, true);
        }

        private void SetButtonDisplay()
        {
            ButtonOptions.Visible = !PmViewMessage.Visible;
            ButtonInBox.Visible = ButtonInBox.Visible && !PmViewMessage.Visible;
            ButtonOutBox.Visible = ButtonOutBox.Visible && !PmViewMessage.Visible;
            ButtonMembers.Visible = !PmViewMessage.Visible;
            ButtonNew.Visible = !PmViewMessage.Visible && !PmMessage.Visible;


            ButtonReceive.Visible = true;

            ButtonReply.Visible = PmViewMessage.Visible;
            ButtonReplyQuote.Visible = PmViewMessage.Visible;
            ButtonForward.Visible = PmViewMessage.Visible;
            ButtonDelete.Visible = PmViewMessage.Visible;
        }

        private void DisplayMessage(PrivateMessageInfo pm)
        {
            MemberInfo member = Snitz.BLL.Members.GetMember(pm.FromMemberName);
            ProfileCommon prof = ProfileCommon.GetUserProfile(pm.FromMemberName);
            if (prof.Gravatar)
            {
                Gravatar avatar = new Gravatar { Email = member.Email };
                if (member.AvatarUrl != "")
                    avatar.DefaultImage = member.AvatarUrl;
                phAvatar.Controls.Add(avatar);

            }
            else
            {

                Literal avatar = new Literal { Text = member.AvatarUrl };
                phAvatar.Controls.Add(avatar);
            }
            pmRecipients.Text = pm.Subject;
            pmSubject.Visible = false;
            pmFrom.Text = String.Format("<a href=\"/Account/profile.aspx?user={0}\">{0}</a>", pm.FromMemberName);

            pmTitle.Text = member.Title;
            pmCountry.Text = member.Country;
            pmPostcount.Text = member.PostCount.ToString();
            pmDate.Text = pm.Sent.ToString();

            pmBody.Text = pm.Message.ParseTags();
            PMViews.ActiveViewIndex = 2;

        }

        protected void ButtonReply_Click(object sender, ImageClickEventArgs e)
        {
            PmMessage.Visible = true;
            PmMessage.GroupingText = "Reply To Message";
            ImageButton lnk = (ImageButton)sender;
            var pm = Snitz.BLL.PrivateMessages.GetMessage(Convert.ToInt32(lnk.CommandArgument));
            //DisplayMessage(pm);
            PMViews.ActiveViewIndex = 1;
            SetButtonDisplay();
            tbxRecipient.Text = pm.FromMemberName;
            tbxRecipient.Enabled = false;
            lblRecipient.Visible = true;
            lblMultiple.Visible = false;
            tbxSubject.Text = Resources.PrivateMessage.PmViewRE + pm.Subject;
            tbxSubject.Enabled = false;
            lblSubject.Visible = true;
            PMControls.Visible = false;
        }

        protected void ButtonReplyQuote_Click(object sender, ImageClickEventArgs e)
        {
            PmMessage.Visible = true;
            PmMessage.GroupingText = "Reply To Message";
            ImageButton lnk = (ImageButton)sender;
            var pm = Snitz.BLL.PrivateMessages.GetMessage(Convert.ToInt32(lnk.CommandArgument));
            //DisplayMessage(pm);
            PMViews.ActiveViewIndex = 1;
            SetButtonDisplay();
            tbxRecipient.Text = pm.FromMemberName;
            tbxRecipient.Enabled = false;
            lblRecipient.Visible = true;
            lblMultiple.Visible = false;
            qrMessage.Text = string.Format(@"[quote]{0}[/quote]", pm.Message);
            tbxSubject.Text = Resources.PrivateMessage.PmViewRE + pm.Subject;
            tbxSubject.Enabled = true;
            lblSubject.Visible = true;
            PMControls.Visible = false;
        }
        protected void ButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton lnk = (ImageButton)sender;
            Snitz.BLL.PrivateMessages.DeletePrivateMessage(Convert.ToInt32(lnk.CommandArgument));
            ScriptManager.RegisterStartupScript(this, GetType(), "Startup", _redirectionScript, true);
        }

        protected void ButtonForward_Click(object sender, ImageClickEventArgs e)
        {
            PmMessage.Visible = true;
            PmMessage.GroupingText = "Forward Message";
            ImageButton lnk = (ImageButton)sender;
            PrivateMessageInfo pm = Snitz.BLL.PrivateMessages.GetMessage(Convert.ToInt32(lnk.CommandArgument));
            //DisplayMessage(pm);
            PMViews.ActiveViewIndex = 1;
            SetButtonDisplay();
            tbxRecipient.Text = "";
            tbxRecipient.Enabled = true;
            qrMessage.Text = Resources.PrivateMessage.PmForwardedMessage + Environment.NewLine + pm.Message;
            tbxSubject.Text = Resources.PrivateMessage.PmViewFwd + pm.Subject;
            tbxSubject.Enabled = false;
            lblSubject.Visible = true;
            PMControls.Visible = false;
        }

        protected void SearchMember(object sender, EventArgs e)
        {
            CurrentPage = 1;
            Session["_searchfor"] = tbxFind.Text;
            Members.DataSource = null;
            BindMemberList(tbxFind.Text);

            SetupPager();

        }

        private void SetupPager()
        {
            ddlCurrentPage.Items.Clear();
            for (int i = 1; i <= _pages; i++)
            {
                ddlCurrentPage.Items.Add(i.ToString());
            }
            ddlCurrentPage.SelectedValue = CurrentPage.ToString();
            numPages.Text = _pages.ToString();
        }
        protected void SendPM(object sender, EventArgs e)
        {
            ImageButton lnk = (ImageButton)sender;
            PMViews.ActiveViewIndex = 1;
            tbxRecipient.Text = lnk.CommandArgument;
            SetButtonDisplay();
        }
        protected void ChangePage(object sender, EventArgs e)
        {
            CurrentPage = Convert.ToInt32(ddlCurrentPage.SelectedValue);
            BindMemberList(tbxFind.Text);

        }
        protected void Members_ItemCommand(object source, DataListCommandEventArgs e)
        {
            //Panel1_ModalPopupExtender.Hide();
            PMViews.ActiveViewIndex = 1;
            tbxRecipient.Text = (string)e.CommandArgument;
            SetButtonDisplay();
        }

        protected void MembersDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                ImageButton ib = e.Item.FindControl("sendpm") as ImageButton;
                var scriptManager = ScriptManager.GetCurrent(Page);
                if (scriptManager != null && ib != null) scriptManager.RegisterPostBackControl(ib);
            }
        }

        protected void OutboxBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image imgRead = e.Row.Cells[0].FindControl("pmImgReadx") as Image;
                Image imgUnread = e.Row.Cells[0].FindControl("pmImgUnReadx") as Image;
                var pm = (PrivateMessageInfo)e.Row.DataItem;
                if (pm.Read == 1)
                {
                    if (imgRead != null) imgRead.Visible = true;
                    if (imgUnread != null) imgUnread.Visible = false;
                }
                else
                {
                    if (imgRead != null) imgRead.Visible = false;
                    if (imgUnread != null) imgUnread.Visible = true;
                }
            }
        }
    }
}