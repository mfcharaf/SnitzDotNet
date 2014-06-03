<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePostButtons.ascx.cs" Inherits="SnitzUI.UserControls.PagePostButtons" %>
        <asp:LinkButton ID="NewTopic" CssClass="Snitzbutton btnTopic" runat="server"  PostBackUrl='<%# Eval("PostTopicUrl") %>' Text='<%$ Resources:webResources,lblNewTopic %>'/>
        <asp:LinkButton ID="ReplyTopic" CssClass="Snitzbutton btnReply" runat="server"  PostBackUrl='<%# Eval("ReplyTopicUrl") %>' Text='<%$ Resources:webResources,lblReply %>'/>
        <asp:LinkButton ID="SendTopic" CssClass="Snitzbutton btnSend" runat="server"  Text='<%$ Resources:webResources,lblSendTopic %>' />
        <asp:LinkButton ID="PrintTopic" CssClass="Snitzbutton btnPrint" runat="server"  Text='<%$ Resources:webResources,lblPrintTopic %>' />
        <asp:LinkButton ID="SubscribeTopic" CssClass="Snitzbutton btnSubscribe" runat="server"   
    OnClick="TopicSubscribe" Text="<%$ Resources:extras,lblSubscribe %>"
    AlternateText="<%$ Resources:extras,lblSubscribe %>" CausesValidation="False" 
    EnableViewState="False"  />    
        <asp:LinkButton ID="UnSubscribeTopic" CssClass="Snitzbutton btnUnSubscribe" runat="server" 
        OnClick="TopicSubscribe" Text="<%$ Resources:extras,lblUnsubscribe %>"
    AlternateText="<%$ Resources:extras,lblUnsubscribe %>" CausesValidation="False" 
    EnableViewState="False" ToolTip="<%$ Resources:extras,lblUnsubscribe %>"  />    
