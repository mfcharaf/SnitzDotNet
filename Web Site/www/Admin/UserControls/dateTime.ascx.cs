using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzCommon;
using SnitzConfig;

public partial class Admin_dateTime : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
            GetValues();
    }
    void GetValues()
    {
        TimeSpan forumAdjust = Config.TimeAdjust.DoubleToHours();
        DateTime fDate = DateTime.UtcNow;

        if (Config.DayLightSavingAdjust && !String.IsNullOrEmpty(Config.TimeZoneString))
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Config.TimeZoneString);
            fDate = TimeZoneInfo.ConvertTimeFromUtc(fDate, tz);
        }
        else
        {
            fDate += forumAdjust;
        }
        rblTimeType.SelectedValue = Config.TimeFormat;
        
        Label3.Text = fDate.ToString(Config.DateFormat + " " + Config.TimeFormat);
        lblToday.Text = DateTime.UtcNow.ToString(Config.DateFormat + " " + Config.TimeFormat);
        if(!String.IsNullOrEmpty(Config.TimeZoneString))
            lbTimeZone.SelectedValue = Config.TimeZoneString;
        ddlDateType.SelectedValue = Config.DateFormat;
        rblDaylightSaving.SelectedValue = Config.DayLightSavingAdjust ? "1" : "0";
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();

        if (Config.TimeZoneString != lbTimeZone.SelectedValue)
        {
            toUpdate.Add("TimeZoneString".GetPropertyDescription(), lbTimeZone.SelectedValue);
            TimeZoneInfo tzone = TimeZoneInfo.FindSystemTimeZoneById(lbTimeZone.SelectedValue);
            toUpdate.Add("TimeAdjust".GetPropertyDescription(), tzone.BaseUtcOffset.TotalHours.ToString());
        }
        if (Config.DateFormat != ddlDateType.SelectedValue)
            toUpdate.Add("DateFormat".GetPropertyDescription(), ddlDateType.SelectedValue);
        if (Config.TimeFormat != rblTimeType.SelectedValue)
            toUpdate.Add("TimeFormat".GetPropertyDescription(), rblTimeType.SelectedValue);
        if (Config.DayLightSavingAdjust != (rblDaylightSaving.SelectedValue == "1"))
            toUpdate.Add("DayLightSavingAdjust".GetPropertyDescription(), rblDaylightSaving.SelectedValue);
        Config.UpdateKeys(toUpdate);

    }
}
