<%@ Control Inherits="SnitzUI.UserControls.PrivateMessaging.PmSend" Language="C#" AutoEventWireup="True" CodeBehind="pmSend.ascx.cs" %>

<asp:Panel ID="sendPM" runat="server" CssClass="clearfix" EnableViewState="false" Width="600">
    <div class="mainModalContent clearfix">
        <div id="Div3">
            <asp:Label ID="Label1" runat="server" Text="Subject"></asp:Label>&nbsp;<input type="text" id="SubjectTextBox" />
            <br />
            <textarea id="MessageTextBox" class="QRMsgArea" rows="5"></textarea>
            <br />
                <input type="hidden" id="toUser" value="<%= this.ToUser %>" />
                <input type="hidden" id="layout" value="<%= this.Layout %>" />
            <div ID="Panel5">
                <button onclick="SendPM();return false;" type="button">
                    <%= Resources.webResources.btnSend %></button>&nbsp;
                <button onclick="mainScreen.CancelModal();return false;" type="button">
                    <%= Resources.webResources.btnCancel %></button>
            </div>
        </div>
    </div>
</asp:Panel>
