using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class AdminRadioButtonList : RadioButtonList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.RepeatDirection = RepeatDirection.Horizontal;
            this.RepeatLayout = RepeatLayout.Flow;
            this.CssClass = "cbx";
            this.Items.Add(new ListItem("Yes", "1"));
            this.Items.Add(new ListItem("No", "0") { Selected = true });

        }

    }
}
