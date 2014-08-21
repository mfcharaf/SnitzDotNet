using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzConfig;

public partial class Admin_ArchiveForums : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            var forums = Forums.GetAllForums();
            ForumList.DataSource = forums;
            ForumList.DataBind();
                AvForumsList.DataSource = forums;
            AvForumsList.DataTextField = "Subject";
            AvForumsList.DataValueField = "Id";
            ArchiveBtn.Enabled = false;
            Panel2.Visible = false;
        }
        else
            Panel2.Visible = false;
         

        Page.DataBind();
    }
    protected void Ad1_Click(object sender, EventArgs e)
    {
        if (AvForumsList.SelectedItem != null)
        {
            ListItemCollection liC = new ListItemCollection();


            foreach (ListItem li in AvForumsList.Items)
                if (li.Selected)
                {
                    ArchiveForumsList.Items.Add(li);
                    li.Selected = false;
                    liC.Add(li);
                }

            foreach (ListItem li in liC)
                AvForumsList.Items.Remove(li);

            ArchiveBtn.Enabled = true;
            Panel2.Visible = false;


        }

    }
    protected void AdAll_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in AvForumsList.Items)
        {
            ArchiveForumsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            AvForumsList.Items.Remove(li);

        ArchiveBtn.Enabled = true;
        Panel2.Visible = false;
    }
    protected void Rem1_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in ArchiveForumsList.Items)
            if (li.Selected)
            {
                AvForumsList.Items.Add(li);
                li.Selected = false;
                liC.Add(li);
            }

        foreach (ListItem li in liC)
            ArchiveForumsList.Items.Remove(li);

        if (ArchiveForumsList.Items.Count == 0)
            ArchiveBtn.Enabled = false;

        Panel2.Visible = false;

    }
    protected void RemAll_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in ArchiveForumsList.Items)
        {
            AvForumsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            ArchiveForumsList.Items.Remove(li);

        ArchiveBtn.Enabled = false;
        Panel2.Visible = false;
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx?action=archive");
    }
    protected void ArchiveBtn_Click(object sender, EventArgs e)
    {
        int numDays;
        string strStartDate;
        DateTime dt = DateTime.Now ;
              
        numDays = Convert.ToInt32(dateList.SelectedValue);

        TimeSpan ts = new TimeSpan(-numDays,(int) Config.TimeAdjust,0,0);

        dt += ts;

        strStartDate = dt.ToString("yyyyMMddHHmmss");

        if (ArchiveForumsList.Items.Count != 0)
        {
            int count = 0;
            int[] forumList = new int[ArchiveForumsList.Items.Count];

            foreach (ListItem li in ArchiveForumsList.Items)
                forumList[count++] = Convert.ToInt32(li.Value);


            int res = Archive.ArchiveForums(forumList, strStartDate);

            if (res >= 0)
            {
                lblRes.Text = String.Format("Topic Archiving Completed.", res);
                Panel2.Visible = true;
            }
            else
            {
                lblRes.Text = "Error archiving forums.";
                Panel2.Visible = true;
            }
        }
    }

    protected void ForumList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        var row = e.Row;
        ForumInfo data = (ForumInfo) row.DataItem;
        var overdue = (Literal) row.FindControl("overdue");
        if (overdue != null)
        {
            if (data.LastArchived.HasValue)
            {
                var res = data.LastArchived.Value.AddDays(data.ArchiveFrequency ?? 0) < DateTime.UtcNow;
                if (res)
                {
                    row.ForeColor = Color.Red;
                    overdue.Visible = true;
                }
            }
            
        }
    }
}
