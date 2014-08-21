<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModeratePreview.ascx.cs" Inherits="SnitzUI.UserControls.Popups.ModeratePreview" %>

<asp:Panel ID="pnlMessage" runat="server" style="border:1px solid blue;width:700px;height:350px;" CssClass="bbcode" ScrollBars="Auto">
    <asp:Label ID="lblPosted" runat="server" Text="Label"></asp:Label>
    <hr />
<asp:Label ID="msgBody" runat="server" style="white-space:normal"></asp:Label>
</asp:Panel>
<asp:HiddenField ID="hdnTopic" runat="server" />
<asp:HiddenField ID="hdnReply" runat="server" />
<asp:HiddenField ID="hdnModerator" runat="server" />
<asp:Panel runat="server" ID="pnlEmail" style="border:1px solid blue;margin:0px;">
    <asp:Label ID="Label1" runat="server" Text="Comments:" ></asp:Label>
    <asp:TextBox runat="server" ID="txtReason" Rows="8" TextMode="MultiLine" style="width:99%;"></asp:TextBox>
</asp:Panel>
<asp:Panel ID="pnlButtons" runat="server" style="padding:5px;">
    <asp:Button ID="btnApprove" runat="server" Text="Approve" 
        CausesValidation="False" UseSubmitBehavior="False" />
    <asp:Button ID="btnHold" runat="server" Text="Place on Hold" 
        CausesValidation="False" UseSubmitBehavior="False" />
    <asp:Button ID="btnDelete" runat="server" Text="Delete" 
        CausesValidation="False" UseSubmitBehavior="False" />    
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
        CausesValidation="False" onclientclick="mainScreen.CancelModal();return false;" />
</asp:Panel>
