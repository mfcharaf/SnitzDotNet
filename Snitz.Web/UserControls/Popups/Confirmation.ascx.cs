using System;
using SnitzCommon;

namespace SnitzUI.UserControls.Popups
{
    public partial class Confirmation : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                var action = (string) Data;
                Literal1.Text = action;
            }
        }
    }
}