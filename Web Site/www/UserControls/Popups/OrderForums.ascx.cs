using System;
using System.Web;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;

namespace SnitzUI.UserControls.Popups
{
    public partial class OrderForums : TemplateUserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            rptCatOrder.DataSource = Categories.GetCategories();
            rptCatOrder.DataBind();
        }

        protected void BindCategories(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cat = (CategoryInfo)e.Item.DataItem;
                var order = (DropDownList)e.Item.FindControl("cOrder");
                var catid = (HiddenField)e.Item.FindControl("hdnCatOrderId");
                for (int i = 0; i < 100; i++)
                {
                    order.Items.Add(i.ToString());
                }
                order.SelectedValue = cat.Order.ToString();
                catid.Value = cat.Id.ToString();
                var forums = e.Item.FindControl("rptForumOrder");
                if (forums != null)
                {
                    ((Repeater) forums).DataSource = Categories.GetCategoryForums(cat.Id, Members.GetMember(HttpContext.Current.User.Identity.Name));
                    ((Repeater) forums).DataBind();
                }
            }
        }

        protected void BindForums(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var forum = (ForumInfo)e.Item.DataItem;
                var order = (DropDownList)e.Item.FindControl("fOrder");
                var forumid = (HiddenField)e.Item.FindControl("hdnForumOrderId");
                for (int i = 0; i < 100; i++)
                {
                    order.Items.Add(i.ToString());
                }
                order.SelectedValue = forum.Order.ToString();
                forumid.Value = forum.Id.ToString();
            }
        }
    }
}