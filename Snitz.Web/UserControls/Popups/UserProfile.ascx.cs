using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using SnitzCommon;
using SnitzData;
using SnitzMembership;


namespace SnitzUI.UserControls
{
    public partial class UserProfile : TemplateUserControl
    {
        public int UserId { get; set; }

        protected bool IsAuthenticated { get; set; }

        protected SnitzData.Member CurrentUser { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                UserId = ((int)Data);
                CurrentUser = Util.GetMember(HttpContext.Current.User.Identity.Name);
                IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;

            rpt.DataSource = Util.GetAuthor(UserId);
            rpt.DataBind();
            }
        }

        protected void RptItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;

            SnitzData.Member author = Util.GetAuthor(UserId).SingleOrDefault();
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                if (author != null)
                {
                    ProfileCommon prof = ProfileCommon.GetUserProfile(author.Name);
                    if (prof.Gravatar)
                    {
                        var avatar = (Literal)item.FindControl("AvatarLabel");
                        avatar.Visible = false;
                        var ph = (PlaceHolder) item.FindControl("phAvatar");
                        ph.Visible = true;
                        var grav = new Gravatar {Email = author.Email};
                        if (author.AvatarUrl != "")
                            grav.DefaultImage = author.AvatarUrl;
                        ph.Controls.Add(grav);

                    }
                    else
                    {
                        var avatar = (Literal)item.FindControl("AvatarLabel");
                        var ph = (PlaceHolder)item.FindControl("phAvatar");
                        avatar.Text = author.Avatar;
                        avatar.Visible = true;
                        ph.Visible = false;
                    }
                }
            }

        }
 
    }
}