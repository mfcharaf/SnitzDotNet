<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePostButtons.ascx.cs" Inherits="SnitzUI.UserControls.PagePostButtons" %>
        <asp:LinkButton ID="NewTopic" runat="server"  PostBackUrl='<%# Eval("PostTopicUrl") %>' Text='<%$ Resources:webResources,lblNewTopic %>'/>
        <asp:LinkButton ID="ReplyTopic" runat="server"  PostBackUrl='<%# Eval("ReplyTopicUrl") %>' Text='<%$ Resources:webResources,lblReplyToTopic %>'/>
        <asp:LinkButton ID="SendTopic" runat="server"  Text='<%$ Resources:webResources,lblSendTopic %>' />
        <asp:LinkButton ID="PrintTopic" runat="server"  Text='<%$ Resources:webResources,lblPrintTopic %>' />
        <asp:LinkButton ID="SubscribeTopic" runat="server"   
    OnClick="TopicSubscribe" Text="<%$ Resources:extras,lblSubscribe %>"
    AlternateText="<%$ Resources:extras,lblSubscribe %>" CausesValidation="False" 
    EnableViewState="False"  />    
        <asp:LinkButton ID="UnSubscribeTopic" runat="server" 
        OnClick="TopicSubscribe" Text="<%$ Resources:extras,lblUnsubscribe %>"
    AlternateText="<%$ Resources:extras,lblUnsubscribe %>" CausesValidation="False" 
    EnableViewState="False" ToolTip="<%$ Resources:extras,lblUnsubscribe %>"  />    
