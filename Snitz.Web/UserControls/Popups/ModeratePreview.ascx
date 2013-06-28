<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModeratePreview.ascx.cs" Inherits="SnitzUI.UserControls.Popups.ModeratePreview" %>

<asp:Panel ID="pnlMessage" runat="server" style="border:1px solid blue;width:500px;height:250px;">
    <asp:Label ID="lblPosted" runat="server" Text="Label"></asp:Label>
    <hr />
<asp:Label ID="msgBody" runat="server" style="white-space:normal"></asp:Label>
</asp:Panel>
<asp:HiddenField ID="hdnTopic" runat="server" />
<asp:HiddenField ID="hdnReply" runat="server" />
<asp:Panel ID="pnlButtons" runat="server" style="padding:5px;">
    <asp:Button ID="btnOk" runat="server" Text="Approve" OnClientClick="" 
        CausesValidation="False" />
    <asp:Button ID="btnHold" runat="server" Text="Place on Hold" 
        CausesValidation="False" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
        CausesValidation="False" onclientclick="mainScreen.CancelModal();return false;" 
        UseSubmitBehavior="False" />
</asp:Panel>
