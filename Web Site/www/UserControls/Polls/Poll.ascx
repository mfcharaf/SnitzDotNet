<%@ Control Language="C#" AutoEventWireup="true" Inherits="Poll" CodeBehind="Poll.ascx.cs" %>
<%@ Import Namespace="SnitzConfig" %>
<div class="PollBox">
<asp:FormView ID="PollFormView" runat="server"     DataKeyNames="Id"
    OnDataBound="PollFormViewDataBound" EnableModelValidation="True">
    <HeaderTemplate>
            <div class="category">
                <span class="cattitle">&nbsp;Poll&nbsp;&nbsp;</span><asp:Button ID="btnPollActive" runat="server" OnClientClick="MakeActive();return false;" Text="Mark as Active Poll" Visible='<%# page.IsAdministrator && (Config.ActivePoll != (int)Eval("Id")) %>'  EnableViewState="False" UseSubmitBehavior="False" CausesValidation="False" />
            </div>
    </HeaderTemplate>
    <ItemTemplate>

        <asp:Label CssClass="Poll_DisplayText" ID="DisplayTextLabel" runat="server" Text='<%# Bind("DisplayText") %>' EnableViewState="False"></asp:Label>
        <asp:HiddenField ID="hidTopicId" runat="server" Value='<%# Bind("TopicId") %>' />
        <input type="hidden" id="pollIdhdn" value="<%# PollId %>"/>
        <asp:Panel ID="pnlTakePoll" runat="server" CssClass="Poll_TakePoll" EnableViewState="False">
            <asp:RadioButtonList ID="rblPollAnswer" runat="server" ValidationGroup="poll"
                DataTextField="DisplayText" DataValueField="Id" EnableViewState="False">
            </asp:RadioButtonList>
            <asp:Button ID="btnSubmitVote" runat="server" OnClientClick="CastVote();return false;" Text="Vote" ValidationGroup="poll" EnableViewState="False" />
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="rblPollAnswer" ValidationGroup="poll"
                Display="Dynamic" ErrorMessage="You must first select an option." EnableViewState="False"></asp:RequiredFieldValidator>
        </asp:Panel>

        <asp:Panel ID="pnlPollResults" runat="server" CssClass="Poll_PollResults" EnableViewState="False">
            <asp:DataList ID="resultsDataList" runat="server" DataKeyField="Id" OnDataBinding="ResultsDataListDataBinding" OnItemDataBound="ResultsDataListItemDataBound">
                <ItemTemplate>
                    <asp:Label ID="DisplayTextLabel" runat="server" Text='<%# Eval("Answer") %>' EnableViewState="False"></asp:Label>
                    (<asp:Label ID="VotesLabel" runat="server" Text='<%# Eval("Votes") %>' EnableViewState="False"></asp:Label>&nbsp;votes)<br />
                    <asp:Image ID="PercentageImage" runat="server" SkinID="percentImage" Height="7px" Visible="True" EnableViewState="False" />
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
</div>
<br class="seperator"/>

