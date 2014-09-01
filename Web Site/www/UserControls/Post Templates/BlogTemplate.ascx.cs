using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzMembership;

namespace SnitzUI.UserControls.Post_Templates
{
    public partial class BlogTemplate : System.Web.UI.UserControl
    {
        public object Post { get; set; }
        public string CssClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
                return;

            PageBase page = (PageBase)Page;
            PostPanel.CssClass = this.CssClass;
            TopicInfo topic = (TopicInfo) Post;
            lblSubject.Text = topic.Subject;
            msgBody.Text = topic.Message.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls();
            litDate.Text = SnitzTime.TimeAgoTag(topic.Date, page.IsAuthenticated, page.Member);
            litAuthor.Text = topic.AuthorProfilePopup;

            ProfileCommon prof = ProfileCommon.GetUserProfile(topic.AuthorName);
            var author = Members.GetAuthor(topic.AuthorId);
            if (prof.Gravatar)
            {
                Gravatar avatar = new Gravatar { Email = author.Email };
                if (author.AvatarUrl != "" && author.AvatarUrl.StartsWith("http:"))
                    avatar.DefaultImage = author.AvatarUrl;
                phAvatar.Controls.Add(avatar);

            }
            else
            {

                SnitzMembershipUser mu = (SnitzMembershipUser)Membership.GetUser(author.Username);
                Literal avatar = new Literal { Text = author.AvatarImg };
                if (mu != null && mu.IsActive)
                    avatar.Text = avatar.Text.Replace("'avatar'", "'avatar online'");
                phAvatar.Controls.Add(avatar);
            }
            litViews.Text = String.Format("viewed {0} times", topic.Views);
            blgDay.Text = topic.Date.Day.ToString();
            blgMonth.Text = topic.Date.ToString("MMM");
            hBookmark.OnClientClick = "confirmBookMark('Do you want to bookmark this Blog entry?'," + topic.Id + ",-1);return false;";
            hComments.Text = String.Format("{0} {1}", topic.ReplyCount, webResources.lblComments);

        }

    }
}