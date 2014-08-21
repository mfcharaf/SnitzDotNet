/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - ForumProperties.ascx
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
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;

namespace SnitzUI.UserControls.Popups
{
    public partial class ForumProperties : TemplateUserControl
    {
        private int _catid;
        private int _forumtype;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                string[] ids = Data.ToString().Split(',');
                int forumid = Convert.ToInt32(ids[0]);
                _catid = Convert.ToInt32(ids[1]);
                _forumtype = Convert.ToInt16(ids[2]);

                ForumInfo forum = forumid > 0 ? Forums.GetForum(forumid) : new ForumInfo { Id = -1 };
                SetupPage(forum);
            }
        }

        private void SetupPage(ForumInfo forum)
        {
            hdnForumId.Value = forum.Id.ToString();

            tbxSubject.Text = forum.Subject;
            tbxBody.Text = forum.Description;
            tbxOrder.Text = forum.Order.ToString();
            ddlCat.DataSource = Categories.GetCategories();
            ddlCat.DataValueField = "Id";
            ddlCat.DataTextField = "Name";
            ddlCat.DataBind();
            ddlCat.SelectedValue = _catid.ToString();

            if (_forumtype == 1)
            {
                pnlOptions.Visible = false;
                pnlAuth.Visible = false;
                tbxUrl.Text = forum.Url;
                pnlUrl.Visible = true;
                return;
            }
            pnlUrl.Visible = false;
            tbxUrl.Text = "";

            tbxPassword.Text = forum.Password;

            if (forum.UpdatePostCount.HasValue)
                cbxCountPost.Checked = forum.UpdatePostCount.Value;
            cbxAllowPolls.Checked = forum.AllowPolls;

            cbxBugReport.Checked = (forum.Type == (int)Enumerators.ForumType.BugReports);
            cbxBlogPosts.Checked = (forum.Type == (int) Enumerators.ForumType.BlogPosts);
            if (Config.Moderation)
            {
                if (forum.ModerationLevel != null) ddlMod.SelectedValue = ((int) forum.ModerationLevel).ToString();
            }
            else
            {
                lblMod.Enabled = false;
                ddlMod.Enabled = false;
                ddlMod.SelectedValue = "0";
            }
            #region Subscriptions
            if (Config.SubscriptionLevel > 0)
            {
                ddlSub.Items.Clear();
                ddlSub.Items.Add(new ListItem("No Subscriptions Allowed","0"));
                if(Config.SubscriptionLevel < Enumerators.SubscriptionLevel.Topic)
                {
                    ddlSub.Items.Add(new ListItem("Forum Subscriptions Allowed", "1"));
                }
                ddlSub.Items.Add(new ListItem("Topic Subscriptions Allowed", "2"));
                ddlSub.SelectedValue = forum.SubscriptionLevel.ToString();

            }
            else
            {
                lblSub.Enabled = false;
                ddlSub.Enabled = false;
                ddlSub.Items.Clear();
                ddlSub.Items.Add(new ListItem("No Subscriptions Allowed", "0"));
                ddlSub.SelectedValue = "0";
            }
            #endregion

            if (forum.Id > 0)
            {
                string[] roleList = Forums.GetForumRoles(forum.Id);
                ListView2.DataSource = roleList;
                ListView2.DataBind();
                hdnRoleList.Value = String.Join(",",roleList);
                ListView1.DataSource = Forums.GetForumModerators(forum.Id);
                ListView1.DataBind();
            }
            ddlRole.DataSource = new SnitzRoleProvider().GetAllRoles();
            ddlRole.DataBind();
            ddlModUsers.DataSource = Forums.GetAvailableModerators(forum.Id);
            ddlModUsers.DataValueField = "MemberId";
            ddlModUsers.DataTextField = "Name";
            ddlModUsers.DataBind();
        }
    }
}