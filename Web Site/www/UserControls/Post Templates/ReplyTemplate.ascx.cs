using System;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace SnitzUI.UserControls.Post_Templates
{
    public partial class ReplyTemplate : System.Web.UI.UserControl
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
            if (Post is ReplyInfo)
            {
                ReplyInfo reply = (ReplyInfo)Post;
                popuplink.Text = String.Format("<a href='{0}' title='{1}'>{2}</a>", reply.AuthorProfileLink, String.Format(Resources.webResources.lblViewProfile, reply.AuthorName), reply.AuthorName);
                AuthorProfile.AuthorId = reply.AuthorId;
                msgBody.Text = reply.Message.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls();
                
                editedByLbl.Text = String.Format("Edited by {0} - ", reply.EditorName);
                litEditDate.Text = SnitzTime.TimeAgoTag(reply.LastEditDate, page.IsAuthenticated, page.Member);
                sigDiv.Visible = Config.AllowSignatures && reply.AuthorViewSig && reply.UseSignatures && !String.IsNullOrEmpty(reply.AuthorSignature);
                litSig.Text = reply.AuthorSignature;

                editbyDiv.Visible = (reply.LastEditDate.HasValue && reply.LastEditDate.Value != DateTime.MinValue) && Config.ShowEditBy;
            }

            buttonBar.ReplyDeleteClicked += ReplyDeleteClicked;
            hypGoUp.Visible = Alternate;
        }

        private void ReplyDeleteClicked(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
    }
}