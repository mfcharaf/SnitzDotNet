using System;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace SnitzUI.UserControls.Post_Templates
{
    public partial class TopicTemplate : System.Web.UI.UserControl
    {
        public object Post { get; set; }
        public string CssClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
                return;
            buttonBar.Post = Post;
            PageBase page = (PageBase)Page;
            PostPanel.CssClass = this.CssClass;
            TopicInfo topic = (TopicInfo) Post;
            popuplink.Text = String.Format("<a href='{0}' title='{1}'>{2}</a>", topic.AuthorProfileLink, String.Format(Resources.webResources.lblViewProfile, topic.AuthorName), topic.AuthorName);
            AuthorProfile.AuthorId = topic.AuthorId;
            msgBody.Text = topic.Message.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls();

            editedByLbl.Text = String.Format("Edited by {0} - ", topic.EditorName);
            litEditDate.Text = Common.TimeAgoTag(topic.LastEditDate, page.IsAuthenticated, page.Member != null ? page.Member.TimeOffset : 0);
            sigDiv.Visible = Config.AllowSignatures && topic.AuthorViewSig && topic.UseSignatures && !String.IsNullOrEmpty(topic.AuthorSignature);
            litSig.Text = topic.AuthorSignature;

            editbyDiv.Visible = (topic.LastEditDate.HasValue && topic.LastEditDate.Value != DateTime.MinValue) && Config.ShowEditBy;

        }
    }
}