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
        TimeSpan forumAdjust;
        forumAdjust = new TimeSpan(Config.TimeAdjust, 0, 0);

        for (int tZone = -12; tZone < 13; tZone++)
        {
            ListItem li = new ListItem();
            if (tZone < 0)
                li.Text = string.Format("UTC {0} hour(s)", tZone);
            else
                li.Text = string.Format("UTC +{0} hour(s)", tZone);
            li.Value = tZone.ToString();
            ddlTimeZone.Items.Add(li);
        }
        rblTimeType.SelectedValue = Config.TimeFormat;
        DateTime fDate = DateTime.UtcNow + forumAdjust;
        Label3.Text = fDate.ToString(Config.DateFormat + " " + Config.TimeFormat);
        lblToday.Text = DateTime.UtcNow.ToString(Config.DateFormat + " " + Config.TimeFormat);
        ddlTimeZone.SelectedValue = Config.TimeAdjust.ToString();
        ddlDateType.SelectedValue = Config.DateFormat;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        var toUpdate = new Dictionary<string, string>();

        if (Config.TimeAdjust != Convert.ToInt32(ddlTimeZone.SelectedValue))
            toUpdate.Add("TimeAdjust".GetPropertyDescription(), ddlTimeZone.SelectedValue);
        if (Config.DateFormat != ddlDateType.SelectedValue)
            toUpdate.Add("DateFormat".GetPropertyDescription(), ddlDateType.SelectedValue);
        if (Config.TimeFormat != rblTimeType.SelectedValue)
            toUpdate.Add("TimeFormat".GetPropertyDescription(), rblTimeType.SelectedValue);

        Config.UpdateKeys(toUpdate);

    }
}
