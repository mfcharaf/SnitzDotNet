using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class TimeZoneListBox : DropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            ReadOnlyCollection<TimeZoneInfo> tzCollection = TimeZoneInfo.GetSystemTimeZones();
            this.DataSource = tzCollection.OrderBy(t => t.BaseUtcOffset.TotalMinutes);
            
            this.DataTextField = "DisplayName";
            this.DataValueField = "Id";
            this.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}
