using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Snitz.Entities;

namespace SnitzUI.Admin
{
    public partial class PollAdmin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<PollInfo> polls = new List<PollInfo>();
            polls = Snitz.BLL.Polls.GetPolls();
            DetailsView1.DataSource = polls;
            DetailsView1.DataBind();
            PollGridView.DataSource = polls;
            PollGridView.DataBind();
        }

        protected void InsertItem(object sender, DetailsViewInsertEventArgs e)
        {
            TextBox question = (TextBox)DetailsView1.FindControl("NewPollQuestion");
            Snitz.BLL.Polls.AddTopicPoll(-1, question.Text, new SortedList<int, string>());

            Response.Redirect(Request.RawUrl);
            
        }
    }
}