using System;
using System.Globalization;
using System.Linq;
using SnitzCommon;

namespace SnitzUI.UserControls
{

    public partial class DatePicker : System.Web.UI.UserControl
    {
        public string DOBStr { get
        {
            int day = Convert.ToInt32(ddlday.SelectedValue);
            int month = Convert.ToInt32(ddlMonth.SelectedValue);
            int year = Convert.ToInt32(ddlYear.SelectedValue);
            DateTime dt = new DateTime(year, month, day, CultureInfo.CurrentCulture.DateTimeFormat.Calendar);

            return dt.ToForumDateStr().Substring(0,8);
        } }

        public bool Enabled { get; set; }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Populate DropDownLists        
                ddlMonth.DataSource = Enumerable.Range(1, 12).Select(a => new { MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(a), MonthNumber = a });
                ddlMonth.DataBind();
                int year = Convert.ToInt32(DateTime.UtcNow.ToString("yyyy"));
                ddlYear.DataSource = Enumerable.Range(year - 99, 100).Reverse().Select(y=> new {Key = y, Value = SnitzCommon.Common.TranslateNumerals(y)});
                ddlYear.DataBind();
                ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = SnitzCommon.Common.TranslateNumerals(d) });
                ddlday.DataBind();
            }
        }
        public void SetDOB(DateTime dob)
        {
            int month = Convert.ToInt32(dob.ToString("MM"));
            ddlMonth.SelectedValue = month.ToString();
            ddlMonth.Enabled = this.Enabled;
            ddlYear.SelectedValue = dob.ToString("yyyy");
            ddlYear.Enabled = this.Enabled;
            int day = Convert.ToInt32(dob.ToString("dd"));
            ddlday.SelectedValue = day.ToString();
            ddlday.Enabled = this.Enabled;

        }
        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = SnitzCommon.Common.TranslateNumerals(d) });
            ddlday.DataBind();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = SnitzCommon.Common.TranslateNumerals(d) });
            ddlday.DataBind();
        }
    }
}