using System;
using System.Web.UI.WebControls;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using SnitzData;

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

                Forum forum = forumid > 0 ? Util.GetForum(forumid) : new Forum { Id = -1 };
                SetupPage(forum);
            }else
            {
                
            }
        }

        private void SetupPage(Forum forum)
        {
            hdnForumId.Value = forum.Id.ToString();

            tbxSubject.Text = forum.Subject;
            tbxBody.Text = forum.Description;

            ddlCat.DataSource = Util.ListCategories();
            ddlCat.DataValueField = "Id";
            ddlCat.DataTextField = "Name";
            ddlCat.DataBind();
            ddlCat.SelectedValue = _catid.ToString();

            if (_forumtype == 1)
            {
                pnlOptions.Visible = false;
                pnlAuth.Visible = false;
                tbxUrl.Text = forum.URL;
                pnlUrl.Visible = true;
                return;
            }
            else
            {
                pnlUrl.Visible = false;
                tbxUrl.Text = "";
            }

            tbxPassword.Text = forum.Password;

            if (forum.UpdatePostCount.HasValue)
                cbxCountPost.Checked = forum.UpdatePostCount.Value;
            cbxAllowPolls.Checked = forum.AllowPolls;

            if (Config.Moderation)
            {
                ddlMod.SelectedValue = ((int) forum.ModerationLevel).ToString();
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
                if(Config.SubscriptionLevel < SubscriptionLevel.Topic)
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

            ddlAuthType.SelectedValue = forum.F_PRIVATEFORUMS.ToString();

            if (forum.Id > 0)
            {
                string[] roleList = SnitzRoleProvider.GetForumRoles(forum.Id);
                ListView2.DataSource = roleList;
                ListView2.DataBind();
                hdnRoleList.Value = String.Join(",",roleList);
                ListView1.DataSource = forum.Moderators;
                ListView1.DataBind();
            }
            ddlRole.DataSource = new SnitzRoleProvider().GetAllRoles();
            ddlRole.DataBind();
            ddlModUsers.DataSource = Util.ListModerators();
            ddlModUsers.DataValueField = "Id";
            ddlModUsers.DataTextField = "Name";
            ddlModUsers.DataBind();
        }
    }
}