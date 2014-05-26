using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.DataBaseLayer;

public partial class Admin_UnArchiveForums : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            AvForumsList.DataSource = ForumDatasource.GetAllForumsToUnArchive();
            AvForumsList.DataTextField = "F_SUBJECT";
            AvForumsList.DataValueField = "FORUM_ID";
            ArchiveBtn.Enabled = false;
            Panel2.Visible = false;
        }

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
                    UnArchiveForumsList.Items.Add(li);
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
            UnArchiveForumsList.Items.Add(li);
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


        foreach (ListItem li in UnArchiveForumsList.Items)
            if (li.Selected)
            {
                AvForumsList.Items.Add(li);
                li.Selected = false;
                liC.Add(li);
            }

        foreach (ListItem li in liC)
            UnArchiveForumsList.Items.Remove(li);

        if (UnArchiveForumsList.Items.Count == 0)
            ArchiveBtn.Enabled = false;

        Panel2.Visible = false;

    }
    protected void RemAll_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in UnArchiveForumsList.Items)
        {
            AvForumsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            UnArchiveForumsList.Items.Remove(li);

        ArchiveBtn.Enabled = false;
        Panel2.Visible = false;
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx?action=unarchive");
    }
    protected void ArchiveBtn_Click(object sender, EventArgs e)
    {
        int numDays;
        string strStartDate;
        DateTime dt = DateTime.Now;

        numDays = Convert.ToInt32(dateList.SelectedValue);

        TimeSpan ts = new TimeSpan(-numDays, config.timeAdjust, 0, 0);

        dt += ts;

        strStartDate = dt.ToString("yyyyMMddHHmmss");

        if (UnArchiveForumsList.Items.Count != 0)
        {
            int count = 0;
            int[] forumList = new int[UnArchiveForumsList.Items.Count];

            foreach (ListItem li in UnArchiveForumsList.Items)
                forumList[count++] = Convert.ToInt32(li.Value);


            int res = ForumDatasource.UnArchiveForums(forumList, strStartDate);

            if (res >= 0)
            {
                lblRes.Text = string.Format("Topic UnArchiving Completed.<br />{0} Topics Were UnArchived.", res);
                Panel2.Visible = true;
                AvForumsList.DataBind();
            }
            else
            {
                lblRes.Text = "Error unarchiving forums.";
                Panel2.Visible = true;
            }
        }
    }
}
