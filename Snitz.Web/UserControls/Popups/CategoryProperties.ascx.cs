using System;
using SnitzCommon;
using SnitzData;

namespace SnitzUI.UserControls.Popups
{
    public partial class CategoryProperties : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Data != null)
            {
                Category cat = Util.GetCategory(Convert.ToInt32(Data));
                SetupForm(cat);
            }
            else
            {
                var cat = new Category();
                SetupForm(cat);
            }
        }

        private void SetupForm(Category cat)
        {
            hdnCatId.Value = cat.Id.ToString();

            tbxSubject.Text = cat.Name;
            if (cat.ModerationLevel.HasValue) ddlMod.SelectedValue = cat.ModerationLevel.Value.ToString();
            if (cat.SubscriptionLevel.HasValue) ddlSub.SelectedValue = cat.SubscriptionLevel.Value.ToString();
        }
    }
}