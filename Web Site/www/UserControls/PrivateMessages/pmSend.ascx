<%@ Control Inherits="SnitzUI.UserControls.PrivateMessaging.PmSend" Language="C#" AutoEventWireup="True" CodeBehind="pmSend.ascx.cs" %>
<div style="position: relative">
    <div id="loadergif" style="position:absolute;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;z-index:5000;display:none;"  >
        <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
    </div>
    <asp:Panel ID="sendPM" runat="server" CssClass="clearfix" EnableViewState="false" Width="600">
        <div class="mainModalContent clearfix">
            <div id="Div3">
                <asp:Label ID="Label1" runat="server" Text="Subject"></asp:Label>&nbsp;<input type="text" id="SubjectTextBox" />
                <br />
                <textarea id="MessageTextBox" class="QRMsgArea PMMsg" rows="5"></textarea>
                <br />
                    <div id="resultText" style="color:red;display:none;"></div><br />
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
</div>
