using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Snitz.Entities;
using SnitzConfig;

namespace SnitzUI.Admin
{
    public partial class EditPoll : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DetailsView1.DataSource = Snitz.BLL.Polls.GetPolls();
            DetailsView1.DataBind();

            GridView1.DataSource = Snitz.BLL.Polls.GetPollChoices((int) DetailsView1.SelectedValue);
            GridView1.DataBind();
        }

        protected void SetFeaturedPoll(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox) sender;
            int pollid = Convert.ToInt32(Request.QueryString["pid"]);
            if (chk.Checked)
                Config.ActivePoll = pollid;
            else
            {
                if (Config.ActivePoll == pollid)
                {
                    Config.ActivePoll = -1;
                }
            }
        }

        protected void PollDatabound(object sender, EventArgs e)
        {
            DetailsView view = (DetailsView)sender;
            int pollid = Convert.ToInt32(Request.QueryString["pid"]);

            DetailsViewRowCollection rows = view.Rows;
            DetailsViewRow row = rows[2];
            ((CheckBox)row.Cells[1].Controls[1].Controls[0].Controls[1]).Checked = (pollid == Config.ActivePoll);
        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int idx = e.NewEditIndex;

        }

        protected void PollAnswerInsert_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            PollChoiceInfo choice = new PollChoiceInfo {PollId = Convert.ToInt32(Request.QueryString["pid"])};
            TextBox text = (TextBox) PollAnswerInsert.FindControl("NewPollAnswerDisplayText");
            TextBox order = (TextBox)PollAnswerInsert.FindControl("NewPollAnswerSortOrder");
            choice.DisplayText = text.Text;
            choice.Order = Convert.ToInt32(order.Text);
            Snitz.BLL.Polls.AddChoice(choice);

            Response.Redirect(Request.RawUrl);
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int choiceid = GridView1.GetNewValue<int>(e.RowIndex, 1);
            int pollid = Convert.ToInt32(Request.QueryString["pid"]);
            string displayText = GridView1.GetNewValue<string>(e.RowIndex, 2);
            int order = GridView1.GetNewValue<int>(e.RowIndex, 3);

            PollChoiceInfo choice = new PollChoiceInfo
            {
                Id = choiceid,
                DisplayText = displayText,
                PollId = pollid,
                Order = order
            };
            Snitz.BLL.Polls.UpdatePollAnswer(choice);
        }

        protected void DetailsView1_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            TextBox text = (TextBox)DetailsView1.FindControl("EditPollDisplayText");
            TextBox order = (TextBox)DetailsView1.FindControl("TextBox1");
            HiddenField id = (HiddenField) DetailsView1.FindControl("pollID");
            Snitz.BLL.Polls.UpdateTopicPoll(Convert.ToInt32(id.Value),text.Text,new SortedList<int, string>());
        }

        protected void DetailsView1_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            Response.Redirect("/admin/default.aspx?action=pollconfig");
        }

    }
}