using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

public partial class Poll : UserControl
{
    protected PageBase page;

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

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        page = (PageBase)Page;

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        PollFormView.Visible = (Config.ActivePoll > 0 || ViewState["PollID"] != null);
        if (PollId == 0) return;

        var polls = new List<PollInfo> {Polls.GetTopicPoll(PollId)};
        PollFormView.DataSource = polls;
        PollFormView.DataBind();
    }


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
        {
            RadioButtonList choices = (RadioButtonList) takePollPanel.FindControl("rblPollAnswer");
            if (choices != null)
            {
                choices.DataSource = Polls.GetPollChoices(PollId);
                choices.DataBind();
            }
            takePollPanel.Visible = !showResults;
        }

        var pollResultsPanel = PollFormView.FindControl("pnlPollResults") as Panel;
        if (pollResultsPanel != null)
        {
            DataList results = (DataList) pollResultsPanel.FindControl("resultsDataList");
            if (results != null)
            {
                results.DataSource = Polls.GetResults(PollId);
                results.DataBind();
            }
            pollResultsPanel.Visible = showResults;
        }

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
            string path = Page.Request.FilePath;
            if (path.ToLower().Contains("topic.aspx"))
                viewCommentsPanel.Visible = false;
        }
    }
    #endregion

    #region ResultsDataList Event Handlers
    // Determines how many total votes have been cast for this poll. Used to determine the
    // percentages for each answer as well as for displaying the total number of votes in
    // the TotalVotesLabel Label. 
    private int _totalVotes;
    protected void ResultsDataListDataBinding(object sender, EventArgs e)
    {
        _totalVotes = Polls.GetTotalVotes(PollId);

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

    #region CanUserTakePoll

    /// <summary>
    /// Determines whether a user can take the poll or if the results must be shown.
    /// My implementation only allows authenticated users who have not already taken the poll to vote. 
    /// EXTENSION POINT: If you want to modify what users can take the poll (such as allowing
    ///                  anonymous users), modify the CanUserTakePoll() method...)
    /// </summary>
    /// <returns>true if user can vote</returns>
    private bool CanUserTakePoll()
    {
        // Anonymous visitors cannot take poll
        if (!Request.IsAuthenticated)
            return false;

        // Determine if this user has already taken this poll... if so, they cannot retake it.
        MembershipUser currentUser = Membership.GetUser();
        if (currentUser != null)
        {
            if (currentUser.ProviderUserKey != null)
            {
                var userId = (int)currentUser.ProviderUserKey;
                return Polls.CanUserTakePoll(PollId, userId);
            }
        }

        return false;
    }
    #endregion

}
