<%@ Control Language="C#" AutoEventWireup="True" Inherits="MessageButtonBar" CodeBehind="MessageButtonBar.ascx.cs"
    EnableViewState="false" %>

<span class="buttonbarTxt"><a name="<%# ThisId %>" id='<%# ThisId %>'>
    <asp:ImageButton  ID="imgPosticon" SkinID="Document" runat="server" ImageAlign="Middle"
        ToolTip="Bookmark Post" CausesValidation="False"
        OnClientClick="" Style="vertical-align: top;" EnableViewState="False" /></a>&nbsp;
    <asp:Label ID="date" runat="server" Style="padding:0px;margin-bottom:4px;" EnableViewState="False"></asp:Label>
</span>
<div class="buttonbarImg">
    <asp:ImageButton ID="TopicApprove" SkinID="approve" runat="server" ToolTip="Moderate Post"
        CausesValidation="False" EnableViewState="False" CommandArgument='<%# ThisId %>' />
    <asp:HyperLink ID="hReplyQuote" rel="nofollow" SkinID="ReplyQuote" runat="server"
        EnableViewState="False" Text="<%$ Resources:webResources, lblReplyQuote %>" ToolTip="<%$ Resources:webResources, lblReplyQuote %>"></asp:HyperLink>
    <asp:HyperLink ID="hEdit" rel="nofollow" SkinID="EditPost" runat="server" EnableViewState="False"
        Visible="False"></asp:HyperLink>
    <asp:ImageButton ID="ViewIP" SkinID="ShowIP" runat="server" OnClientClick="" CausesValidation="False"
        EnableViewState="False" ToolTip="<%$ Resources:webResources, lblShowIP %>" />
    <asp:ImageButton ID="TopicDelete" SkinID="DeletePost" 
        ToolTip="<%$ Resources:webResources, lblDelPost %>" runat="server" OnClientClick="" CausesValidation="False" EnableViewState="False"/>
    <asp:ImageButton ID="SplitTopic" SkinID="SplitTopic" ToolTip="Split Topic" runat="server"
        OnClientClick="" CausesValidation="False" EnableViewState="False"
        CommandArgument='<%# ThisId %>' />
</div>
