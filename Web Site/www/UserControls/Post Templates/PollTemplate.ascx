﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollTemplate.ascx.cs" Inherits="PollTemplate" %>
<%@ Register TagPrefix="topic" TagName="MessageProfile" Src="~/UserControls/MessageProfile.ascx" %>
<%@ Register TagPrefix="topic" TagName="MessageButtonBar" Src="~/UserControls/MessageButtonBar.ascx" %>
<%@ Import Namespace="SnitzConfig" %>
    <style>
    #rightcolumn,.rightcolumn{ display: none;width: 0px;}
    #contentwrapper{ margin-right: 0px;} 
    .maincolumn{ width: 99%;}       
    </style>
<div class="TopicDiv">
    <div class="leftColumn">
        <asp:HiddenField ID="hdnAuthor" runat="server" />
        <asp:Literal ID="popuplink" runat="server" Text=''></asp:Literal>
        <topic:MessageProfile runat="server" ID="AuthorProfile" />
    </div>
    <div class="MessageDIV">
        <div class="buttonbar">
            <topic:MessageButtonBar ID="buttonBar" runat="server" Post='<%# Post %>' />
        </div>
        <div class="mContent">
            <asp:FormView CssClass="PollBox" ID="PollFormView" runat="server"     DataKeyNames="Id"
                OnDataBound="PollFormViewDataBound" EnableModelValidation="True">
                <HeaderTemplate>
                        <div class="category">
                            <span class="cattitle">&nbsp;Poll&nbsp;&nbsp;</span><asp:Button ID="btnPollActive" runat="server" OnClientClick="MakeActive();return false;" Text="Mark as Active Poll" Visible='<%# page.IsAdministrator && (Config.ActivePoll != (int)Eval("Id")) %>'  EnableViewState="False" UseSubmitBehavior="False" CausesValidation="False" />
                        </div>
                </HeaderTemplate>
                <ItemTemplate>
                    <script type="text/javascript">
                        function MakeActive() {
                            alert("add code for this");
                        }
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
            <span class="bbcode"><asp:Literal ID="msgBody" runat="server" Text='' Mode="Encode"></asp:Literal></span>
        </div>
        <br />
    </div>
</div>
          
