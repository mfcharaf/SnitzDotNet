using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class GenderDropDownList : DropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Items.Add(new ListItem("[Select Gender]",""));
            this.Items.Add(new ListItem("Female", "Female"));
            this.Items.Add(new ListItem("Male", "Male"));
        }
    }
}
