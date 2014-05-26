/*
####################################################################################################################
##
## SnitzUI.UserControls - DatePicker.ascx
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

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
                ddlYear.DataSource = Enumerable.Range(year - 99, 100).Reverse().Select(y=> new {Key = y, Value = Common.TranslateNumerals(y)});
                ddlYear.DataBind();
                ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
                ddlday.DataBind();
            }
        }
        public void SetDOB(DateTime dob)
        {
            int year = Convert.ToInt32(DateTime.UtcNow.ToString("yyyy"));
            int month = Convert.ToInt32(dob.ToString("MM"));
            ddlMonth.SelectedValue = month.ToString();
            ddlMonth.Enabled = Enabled;
            ddlYear.SelectedValue = dob.Year < year - 99 ? (year - 99).ToString() : dob.ToString("yyyy");
            ddlYear.Enabled = Enabled;
            int day = Convert.ToInt32(dob.ToString("dd"));
            ddlday.SelectedValue = day.ToString();
            ddlday.Enabled = Enabled;

        }
        protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
            ddlday.DataBind();
        }

        protected void ddlYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlday.DataSource = Enumerable.Range(1, DateTime.DaysInMonth(Convert.ToInt32(ddlYear.SelectedValue), Convert.ToInt32(ddlMonth.SelectedValue))).Select(d => new { Key = d, Value = Common.TranslateNumerals(d) });
            ddlday.DataBind();
        }
    }
}