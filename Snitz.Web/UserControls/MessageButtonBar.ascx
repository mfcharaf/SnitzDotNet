<%@ Control Language="C#" AutoEventWireup="True" Inherits="MessageButtonBar" CodeBehind="MessageButtonBar.ascx.cs"
    EnableViewState="true" %>

<span class="buttonbarTxt"><a name="<%# ThisId %>" id='<%# ThisId %>'>
    <asp:ImageButton ID="imgPosticon" SkinID="Document" runat="server" ImageAlign="Middle"
        ToolTip="Bookmark Post" CommandArgument='<%# ThisId %>' OnClick="BookMark" CausesValidation="False"
        OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Bookmark', 'Do you want to delete?');
                        mainScreen.LoadServerControlHtml(' Bookmark',{'pageID':3,'data': 'Bookmark this post?'},'confirmHandlers.BeginRecieve');
                        return false;" Style="vertical-align: top;" /></a>&nbsp;
    <asp:Label ID="date" runat="server" Style="padding:0px;margin-bottom:4px;"></asp:Label>
</span>
<div class="buttonbarImg">
    <asp:ImageButton ID="TopicApprove" SkinID="approve" runat="server" ToolTip="Approve Post"
        CausesValidation="False" EnableViewState="False" CommandArgument='<%# ThisId %>' />
    <asp:HyperLink ID="hReplyQuote" rel="nofollow" SkinID="ReplyQuote" runat="server"
        EnableViewState="False" Text="<%$ Resources:webResources, lblReplyQuote %>" ToolTip="<%$ Resources:webResources, lblReplyQuote %>"></asp:HyperLink>
    <asp:HyperLink ID="hEdit" rel="nofollow" SkinID="EditPost" runat="server" EnableViewState="False"
        Visible="False"></asp:HyperLink>
    <asp:ImageButton ID="ViewIP" SkinID="ShowIP" runat="server" OnClientClick="" CausesValidation="False"
        EnableViewState="False" ToolTip="<%$ Resources:webResources, lblShowIP %>" />
    <asp:ImageButton ID="TopicDelete" SkinID="DeletePost" CommandArgument='<%# ThisId %>'
        ToolTip="<%$ Resources:webResources, lblDelPost %>" runat="server" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to delete?'); 
        mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': '[MESSAGE]'},'confirmHandlers.BeginRecieve');
        return false;" CausesValidation="False" EnableViewState="False" OnClick="DeletePost" />
    <asp:ImageButton ID="SplitTopic" SkinID="SplitTopic" ToolTip="Split Topic" runat="server"
        OnClientClick="" CausesValidation="False" EnableViewState="False"
        CommandArgument='<%# ThisId %>' />
</div>
