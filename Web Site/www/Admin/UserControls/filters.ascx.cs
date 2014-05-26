using System;
using System.Web.UI;

public partial class Admin_filters : UserControl
{
    private string _FilterType;
    public string FilterType
    {
        get
        {
            return _FilterType;
        }
        set
        {
            _FilterType = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        switch (FilterType)
        {
            case "badwords" :
                Panel1.Visible = true;
                Panel2.Visible = false;
                Panel3.Visible = false;
                break;
            case "usernames" :
                Panel1.Visible = false;
                Panel2.Visible = true;
                Panel3.Visible = false;
                break;
            default :
                Panel1.Visible = false;
                Panel2.Visible = false;
                Panel3.Visible = true;
                break;
        }
    }
    protected void BadWordInsert(object sender, EventArgs e)
    {
        dsWordFilters.Insert();
    }


    protected void btnNewNameFilter_Click(object sender, EventArgs e)
    {
        dsNameFilter.InsertParameters[0].DefaultValue = tbxNameFilter.Text;
        dsNameFilter.Insert();
        tbxNameFilter.Text = "";
    }
    protected void btnNewWordFilter_Click(object sender, EventArgs e)
    {
        dsWordFilters.InsertParameters[0].DefaultValue = tbxBadWord.Text;
        dsWordFilters.InsertParameters[1].DefaultValue = tbxReplace.Text;
        dsWordFilters.Insert();
        tbxBadWord.Text = "";
        tbxReplace.Text = "";
    }
}
