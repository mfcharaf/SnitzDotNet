<%@ Control Language="C#" AutoEventWireup="true" Inherits="Poll" CodeBehind="Poll.ascx.cs" %>
<%@ Import Namespace="SnitzConfig" %>


<asp:FormView CssClass="PollBox" ID="PollFormView" runat="server" Visible="<%# Config.ActivePoll > 0 %>"
    DataKeyNames="PollID" DataSourceID="PollDataSource"
    OnDataBound="PollFormViewDataBound" EnableModelValidation="True">
    <HeaderTemplate>
            <div class="category">
                <span class="cattitle">Current Poll</span>
            </div>
    </HeaderTemplate>
    <ItemTemplate>
        <script type="text/javascript">
            function CastVote() {
                var test = $(".Poll_TakePoll input:radio:checked").val();
                PageMethods.CastVote(test, VoteSucceeded, VoteFailed);
            }
            function VoteSucceeded(results, userContext, methodName) {
                alert(results);
                mainScreen.CancelModal();
                location.reload();
                return false;
            }

            function VoteFailed(error, userContext, methodName) {
                // Alert user to the error.
                alert(error.get_message());
                mainScreen.CancelModal();
                return false;
            }
        </script>
        <asp:Label CssClass="Poll_DisplayText" ID="DisplayTextLabel" runat="server" Text='<%# Bind("DisplayText") %>' EnableViewState="False"></asp:Label>
        <asp:HiddenField ID="hidTopicId" runat="server" Value='<%# Bind("TopicId") %>' />

        <asp:Panel ID="pnlTakePoll" runat="server" CssClass="Poll_TakePoll" EnableViewState="False">
            <asp:RadioButtonList ID="rblPollAnswer" runat="server" DataSourceID="PollAnswersDataSource" ValidationGroup="poll"
                DataTextField="DisplayText" DataValueField="PollAnswerID" EnableViewState="False">
            </asp:RadioButtonList>
            <asp:LinqDataSource ID="PollAnswersDataSource" runat="server"
                ContextTypeName="SnitzData.PollDataDataContext"
                Select="new (DisplayText, SortOrder, PollAnswerID)" TableName="PollAnswer"
                OnSelecting="PollAnswersDataSourceSelecting"
                Where="PollID == @PollID" OrderBy="SortOrder">
            </asp:LinqDataSource>


            <asp:Button ID="btnSubmitVote" runat="server" OnClientClick="CastVote();return false;" Text="Vote" ValidationGroup="poll" EnableViewState="False" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="rblPollAnswer" ValidationGroup="poll"
                Display="Dynamic" ErrorMessage="You must first select an option." EnableViewState="False"></asp:RequiredFieldValidator>
        </asp:Panel>

        <asp:Panel ID="pnlPollResults" runat="server" CssClass="Poll_PollResults" EnableViewState="False">
            <asp:SqlDataSource ID="PollResultsDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ForumConnectionString %>"
                OnSelecting="PollResultsDataSourceSelecting" SelectCommand="SELECT a.PollAnswerID, a.PollID, a.DisplayText, a.SortOrder, COUNT(r.UserID) as Votes FROM FORUM_POLLANSWERS a LEFT JOIN FORUM_POLLRESPONSE r ON a.PollAnswerID = r.PollAnswerID WHERE a.PollID = @PollID GROUP BY a.PollAnswerID, a.PollID, a.DisplayText, a.SortOrder ORDER BY a.SortOrder">
                <SelectParameters>
                    <asp:Parameter Name="PollID" />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:DataList ID="resultsDataList" runat="server" DataKeyField="PollAnswerID" DataSourceID="PollResultsDataSource" OnDataBinding="ResultsDataListDataBinding" OnItemDataBound="ResultsDataListItemDataBound">
                <ItemTemplate>
                    <asp:Label ID="DisplayTextLabel" runat="server" Text='<%# Eval("DisplayText") %>' EnableViewState="False"></asp:Label>
                    (<asp:Label ID="VotesLabel" runat="server" Text='<%# Eval("Votes") %>' EnableViewState="False"></asp:Label>&nbsp;votes)<br />
                    <asp:Image ID="PercentageImage" runat="server" Height="7px" ImageUrl="~/images/bar.JPG" Visible="True" EnableViewState="False" />
                    <asp:Label ID="PercentageLabel" runat="server" EnableViewState="False"></asp:Label>
                    <br style="line-height: 0.4em;" />
                </ItemTemplate>
            </asp:DataList>

            <br />
            <asp:Label runat="server" ID="TotalVotesLabel" CssClass="Poll_TotalVotes" EnableViewState="False"></asp:Label>
        </asp:Panel>

        <asp:Panel ID="pnlPollComments" runat="server" Visible="false" EnableViewState="False">
            <asp:HyperLink ID="lnkTopic" runat="server" EnableViewState="False"></asp:HyperLink>
        </asp:Panel>
        
    </ItemTemplate>
    <FooterTemplate></FooterTemplate>

</asp:FormView>
<br class="seperator"/>
<asp:LinqDataSource ID="PollDataSource" runat="server"
    ContextTypeName="SnitzData.PollDataDataContext"
    Select="new (PollID, DisplayText, TopicId)" TableName="Poll"
    Where="PollID == @PollID" OnSelecting="PollDataSourceSelecting">
</asp:LinqDataSource>

<asp:LinqDataSource ID="LinqDataSource1" runat="server"
    ContextTypeName="SnitzData.PollDataDataContext" TableName="PollResponse"
    Where="PollID == @PollID" OnSelecting="PollDataSourceSelecting"
    EnableInsert="True">
</asp:LinqDataSource>
