using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class DobPicker : Panel
    {
        public DropDownList ValidateControl;
        private DropDownList _ddlDay;
        private DropDownList _ddlMonth;
        private DropDownList _ddlYear;

        public string DOBStr
        {
            get
            {
                int day = Convert.ToInt32(_ddlDay.SelectedValue);
                int month = Convert.ToInt32(_ddlMonth.SelectedValue);
                int year = Convert.ToInt32(_ddlYear.SelectedValue);
                DateTime dt = new DateTime(year, month, day, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);

                return dt.ToForumDateStr().Substring(0, 8);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            _ddlDay = new DropDownList() { ID = "dobDay",DataTextField="Value", DataValueField="Key"};
            _ddlMonth = new DropDownList() { ID = "dobMonth", AutoPostBack = true, DataTextField = "MonthName", DataValueField = "MonthNumber" };
            _ddlYear = new DropDownList() { ID = "dobYear", DataTextField = "Value", DataValueField = "Key", AutoPostBack = true };
            _ddlMonth.DataSource = Enumerable.Range(1, 12).Select(a => new { MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(a), MonthNumber = a });
            _ddlMonth.DataBind();
            int year = Convert.ToInt32(DateTime.UtcNow.ToString("yyyy"));
            int month = Convert.ToInt32(DateTime.UtcNow.ToString("MM"));
            _ddlYear.DataSource = Enumerable.Range(year - 99, 100).Reverse().Select(y => new { Key = y, Value = Common.TranslateNumerals(y) });
            _ddlYear.DataBind();
            _ddlDay.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(year, month)).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
            _ddlDay.DataBind();
            _ddlMonth.SelectedIndexChanged += MonthChanged;
            _ddlYear.SelectedIndexChanged += YearChanged;

            _ddlYear.Style.Add("margin-right","0.5em");
            _ddlMonth.Style.Add("margin-right", "0.5em");

            UpdatePanel updPnl = new UpdatePanel
                                 {
                                     ID = "dateUpdate",
                                     ChildrenAsTriggers = true,
                                     UpdateMode = UpdatePanelUpdateMode.Conditional,
                                     RenderMode = UpdatePanelRenderMode.Inline
                                 };

            updPnl.ContentTemplateContainer.Controls.Add(_ddlYear);
            updPnl.ContentTemplateContainer.Controls.Add(_ddlMonth);
            updPnl.ContentTemplateContainer.Controls.Add(_ddlDay);

            this.Controls.Add(updPnl);

            this.Attributes.Add("style","width:auto;display:inline-block;");
            ValidateControl = _ddlYear;
        }

        private void YearChanged(object sender, EventArgs e)
        {
            _ddlDay.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(_ddlYear.SelectedValue), Convert.ToInt32(_ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
            _ddlDay.DataBind();
        }

        private void MonthChanged(object sender, EventArgs e)
        {
            _ddlDay.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(_ddlYear.SelectedValue), Convert.ToInt32(_ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
            _ddlDay.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        public void SetDOB(DateTime dob)
        {
            int year = Convert.ToInt32(DateTime.UtcNow.ToString("yyyy"));
            int month = Convert.ToInt32(dob.ToString("MM"));
            _ddlMonth.SelectedValue = month.ToString();
            _ddlMonth.Enabled = Enabled;
            _ddlYear.SelectedValue = dob.Year < year - 99 ? (year - 99).ToString() : dob.ToString("yyyy");
            _ddlYear.Enabled = Enabled;
            int day = Convert.ToInt32(dob.ToString("dd"));
            _ddlDay.SelectedValue = day.ToString();
            _ddlDay.Enabled = Enabled;

        }
    }
}
