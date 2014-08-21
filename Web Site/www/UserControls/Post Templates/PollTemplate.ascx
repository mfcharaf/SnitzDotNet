<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollTemplate.ascx.cs" Inherits="PollTemplate" %>
<%@ Register TagPrefix="topic" TagName="MessageProfile" Src="~/UserControls/MessageProfile.ascx" %>
<%@ Register TagPrefix="topic" TagName="MessageButtonBar" Src="~/UserControls/MessageButtonBar.ascx" %>
<%@ Import Namespace="SnitzConfig" %>
 <style type="text/css">
    #replyHeader {display: none;}
    .AspNet-FormView{ border: 0px;}
    img.avatar{float: left; margin: 4px;}
    h1 { line-height: 0.5em;}
    .POFTop{margin-left: 90px;}
    .social-media-share{ display:none;}
    .QRBox{ width: 99%;margin-left: auto;margin-right: auto;margin-top: 4px;}
    .QRRight{ width: 99%;margin-top: 2px;}
    .table-bottom,.QRLeft{ display: none;}
    .table-bottom-footer{ width: 60%;margin-left: 0px;margin-right: auto;}
    .markItUpHeader{ display: none;}
    textarea.QRMsgArea{min-height: 50px !important;height: 60px;}

    #rightcolumn,.rightcolumn,.topicHeader{ display: none;width: 0px;}
    #contentwrapper{ margin-right: 0px;} 
    .maincolumn{ width: 99%;}  
    .MessageDIV{ border-left: 0px;}
    
    .QRBox #qrcontentwrapper{border-color: #777;}
    </style>
<div class="TopicDiv" >
    <div class="leftColumn" style="display:none;width:0px">
        <asp:HiddenField ID="hdnAuthor" runat="server" />

    </div>
    <div class="MessageDIV" style="margin-left: 0px;">
        <div class="buttonbar" style="display:none;">
            <topic:MessageButtonBar ID="buttonBar" runat="server" Post='<%# Post %>' />
        </div>
        <div class="poll-content">
            <asp:FormView CssClass="PollBox" ID="PollFormView" runat="server"     DataKeyNames="Id"
                OnDataBound="PollFormViewDataBound" EnableModelValidation="True">
                <HeaderTemplate>
                        <div class="category">
                            <span class="cattitle">&nbsp;Poll&nbsp;&nbsp;</span><asp:Button ID="btnPollActive" runat="server" OnClientClick="MakeActive();return false;" Text="Mark as Active Poll" Visible='<%# page.IsAdministrator && (Config.ActivePoll != (int)Eval("Id")) %>'  EnableViewState="False" UseSubmitBehavior="False" CausesValidation="False" />
                        </div>
                </HeaderTemplate>
                <ItemTemplate>

                    <asp:Label CssClass="Poll_DisplayText" ID="DisplayTextLabel" runat="server" Text='<%# Bind("DisplayText") %>' EnableViewState="False"></asp:Label>?&nbsp;
                    <asp:Literal runat="server" ID="postedBy" Text="&nbsp;Posted by XXXXX"></asp:Literal>
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
                                <asp:Image ID="PercentageImage" runat="server" Height="12px" SkinID="percentImage" Visible="True" EnableViewState="False" />&nbsp;
                                <asp:Label ID="PercentageLabel" runat="server" EnableViewState="False"></asp:Label>
                                <br style="line-height: 0.4em;" />
                            </ItemTemplate>
                        </asp:DataList>

                        <br />
                        <asp:Label runat="server" ID="TotalVotesLabel" CssClass="Poll_TotalVotes" EnableViewState="False"></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="pnlPollComments" runat="server" Visible="false" EnableViewState="False">
                        <asp:HyperLink ID="lnkTopic" runat="server" EnableViewState="False" onclick="$('.blog-comments').css('display', 'block');return false;"></asp:HyperLink>
                    </asp:Panel>
                </ItemTemplate>
                <FooterTemplate></FooterTemplate>
            </asp:FormView>
            <br class="seperator"/>             
            <span class="bbcode"><asp:Literal ID="msgBody" runat="server" Text='' Mode="Encode"></asp:Literal></span>
        </div>
        <br />
    </div>
    <asp:Label runat="server" ID="comments" Text="<%$ Resources:webResources, lblComments %>" CssClass="blog-comments"></asp:Label>
</div>
          
