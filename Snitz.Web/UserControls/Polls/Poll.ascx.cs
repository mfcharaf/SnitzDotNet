using System;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using SnitzConfig;

public partial class Poll : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PollFormView.Visible = (Config.ActivePoll > 0);
    }

    #region Public Properties
    public int PollId
    {
        get
        {
            if (ViewState["PollID"] == null)
                return Config.ActivePoll;
            return (int)ViewState["PollID"];
        }
        set { ViewState["PollID"] = value; }
    }

    public bool PollEnabled
    {
        get
        {
            if (ViewState["PollEnabled"] == null)
                return true;
            return (bool)ViewState["PollEnabled"];
        }
        set { ViewState["PollEnabled"] = value; }
    }
    #endregion

    #region SqlDataSource Control Event Handlers
    /*
     * The following three event handlers are used to populate the @PollID parameter
     * based on the User Control's PollID property for assorted SqlDataSource controls...
     */

    protected void PollAnswersDataSourceSelecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        e.WhereParameters.Add("PollID", PollId);
    }
    protected void PollResultsDataSourceSelecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        e.Command.Parameters["@PollID"].Value = PollId;
    }

    // Sets the @UserID parameter when voting in order to associate the currently logged on
    // user's UserId value with the vote.
    protected void PollAnswersDataSourceInserting(object sender, SqlDataSourceCommandEventArgs e)
    {
        int userId = -1;
        MembershipUser currentUser = Membership.GetUser();
        if (currentUser != null)
            if (currentUser.ProviderUserKey != null) userId = (int)currentUser.ProviderUserKey;

        e.Command.Parameters["@UserID"].Value = userId;
    }
    #endregion

    #region PollFormView DataBound Event Handler
    // This event handler fires when data is bound to the FormView. It shows either
    // the poll taking or poll results interface depending on the result of
    // CanUserTakePoll() method. If PollEnabled is false, then the poll results are
    // always shown, regardless of what CanUserTakePoll() returns.
    // EXTENSION POINT: If you want to modify what users can take the poll (such as allowing
    //                  anonymous users), modify the CanUserTakePoll() method...)
    protected void PollFormViewDataBound(object sender, EventArgs e)
    {
        // Determine if the user can take the poll
        bool showResults;
        if (!PollEnabled)
            showResults = true;
        else
            showResults = !CanUserTakePoll();

        // Show/hide the Panels based on the value of showResults
        var takePollPanel = PollFormView.FindControl("pnlTakePoll") as Panel;
        if (takePollPanel != null)
            takePollPanel.Visible = !showResults;

        var pollResultsPanel = PollFormView.FindControl("pnlPollResults") as Panel;
        if (pollResultsPanel != null)
            pollResultsPanel.Visible = showResults;

        var viewCommentsPanel = PollFormView.FindControl("pnlPollComments") as Panel;
        var hTopicId = PollFormView.FindControl("hidTopicId") as HiddenField;

        if(viewCommentsPanel != null)
        {
            if (hTopicId != null)
            {
                viewCommentsPanel.Visible = hTopicId.Value != "";
                if(hTopicId.Value != "")
                {
                    var lnk = viewCommentsPanel.FindControl("lnkTopic") as HyperLink;
                    if (lnk != null)
                    {
                        lnk.Text = "View comments";
                        lnk.NavigateUrl = String.Format("~/Content/Forums/Topic.aspx?TOPIC={0}", hTopicId.Value);
                    }
                }
            }
            string path = this.Page.Request.FilePath;
            if (path.ToLower().Contains("topic.aspx"))
                viewCommentsPanel.Visible = false;
        }
    }
    #endregion

    //#region "Vote" Button Click Event Handler
    //// When the "Vote" button is clicked, this event handler executes. It calls the
    //// PollAnswersDataSource's Insert() method, thereby INSERTing a record into the
    //// UserResponses table. It then rebinds the data to the control, which causes the
    //// poll interface to be updated, showing the poll results (since the user has now
    //// taken the poll) and with the updated poll results.
    //protected void btnSubmitVote_Click(object sender, EventArgs e)
    //{
    //    SqlDataSource answersDataSource = PollFormView.FindControl("PollAnswersDataSource") as SqlDataSource;
    //    answersDataSource.Insert();

    //    PollFormView.DataBind();        // rebind the data to the poll interface
    //}
    //#endregion

    #region ResultsDataList Event Handlers
    // Determines how many total votes have been cast for this poll. Used to determine the
    // percentages for each answer as well as for displaying the total number of votes in
    // the TotalVotesLabel Label. 
    private int _totalVotes;
    protected void ResultsDataListDataBinding(object sender, EventArgs e)
    {
        // Calculate the total # of votes
        using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString))
        {
            conn.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM FORUM_POLLRESPONSE r INNER JOIN FORUM_POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID", conn);
            cmd.Parameters.Add(new SqlParameter("@PollID", PollId));
            _totalVotes = (int)cmd.ExecuteScalar();
            conn.Close();
        }

        // Display the # of votes
        var totalVotesLabel = PollFormView.FindControl("TotalVotesLabel") as Label;
        if (totalVotesLabel != null) totalVotesLabel.Text = string.Format("{0:d} votes...", _totalVotes);
    }


    // This event handler fires once for each poll answer when viewing the results.
    // It determines the number of votes for each answer and computes and displays the percentage.
    protected void ResultsDataListItemDataBound(object sender, DataListItemEventArgs e)
    {
        // Determine how many votes were made for this answer...
        var votes = (int)DataBinder.Eval(e.Item.DataItem, "Votes");

        // Programmatically access the Label & Image controls...
        var percentLabel = (Label)e.Item.FindControl("PercentageLabel");
        var percentImage = (Image)e.Item.FindControl("PercentageImage");
        
        // Calculate the percentage...
        if (_totalVotes > 0)
        {
            double pct = (Convert.ToDouble(votes) / Convert.ToDouble(_totalVotes)) * Convert.ToDouble(100);
            percentLabel.Text = string.Format("{0}%", pct.ToString("0.0"));
            percentImage.Width = Unit.Percentage(pct);
        }
        else
        {
            percentLabel.Text = "0%";
            percentImage.Visible = false;
        }
    }
    #endregion

    #region CanUserTakePoll Method (EXTENSION POINT)
    // Determines whether a user can take the poll or if the results must be shown.
    // My implementation only allows authenticated users who have not already taken the
    // poll to vote. 
    // EXTENSION POINT: If you want to modify what users can take the poll (such as allowing
    //                  anonymous users), modify the CanUserTakePoll() method...)
    private bool CanUserTakePoll()
    {
        // Anonymous visitors cannot take poll
        if (!Request.IsAuthenticated)
            return false;

        // Determine if this user has already taken this poll... if so, she cannot retake it.
        MembershipUser currentUser = Membership.GetUser();
        if (currentUser != null)
        {
            if (currentUser.ProviderUserKey != null)
            {
                var userId = (int)currentUser.ProviderUserKey;
                bool hasUserTakenPoll;

                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM FORUM_POLLRESPONSE r INNER JOIN FORUM_POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID AND r.UserID = @UserID", conn);
                    cmd.Parameters.Add(new SqlParameter("@PollID", PollId));
                    cmd.Parameters.Add(new SqlParameter("@UserID", userId));

                    hasUserTakenPoll = ((int)cmd.ExecuteScalar()) > 0;
                    conn.Close();
                }

                return hasUserTakenPoll == false;
            }
        }

        return false;
    }
    #endregion

    protected void PollDataSourceSelecting(object sender, LinqDataSourceSelectEventArgs e)
    {
        e.WhereParameters.Add("PollID", PollId);
    }
}
