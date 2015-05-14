using System;
using System.Web.Security;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;

namespace SnitzUI.UserControls.Post_Templates
{
    public partial class BlogReplyTemplate : System.Web.UI.UserControl
    {
        public object Post { get; set; }
        public string CssClass { get; set; }
        public bool Alternate { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
                return;

            PageBase page = (PageBase)Page;
            PostPanel.CssClass = this.CssClass;
            ReplyInfo reply = (ReplyInfo)Post;
            msgBody.Text = reply.Message.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls();
            litDate.Text = SnitzTime.TimeAgoTag(reply.Date, page.IsAuthenticated, page.Member);
            litAuthor.Text = reply.AuthorProfilePopup;
            ProfileCommon prof = ProfileCommon.GetUserProfile(reply.AuthorName);
            var author = Members.GetAuthor(reply.AuthorId);
            if (prof.Gravatar)
            {
                Gravatar avatar = new Gravatar { Email = author.Email };
                if (author.AvatarUrl != "" && author.AvatarUrl.StartsWith("http:"))
                    avatar.DefaultImage = author.AvatarUrl;
                avatar.CssClass = "avatar";
                phAvatar.Controls.Add(avatar);

            }
            else
            {

                SnitzMembershipUser mu = (SnitzMembershipUser)Membership.GetUser(author.Username);
                Literal avatar = new Literal { Text = author.AvatarImg };
                if (mu != null && mu.IsActive && !(Config.AnonMembers.Contains(mu.UserName)))
                    avatar.Text = avatar.Text.Replace("'avatar'", "'avatar online'");
                phAvatar.Controls.Add(avatar);
            }
        }
    }
}