<%@ Control Inherits="SnitzUI.UserControls.PrivateMessaging.PmSend" Language="C#" AutoEventWireup="True" CodeBehind="pmSend.ascx.cs" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:ImageButton CssClass="profBtn" ID="ImageButton1" SkinID="PMSend" runat="server" ToolTip="Send PM"  AlternateText="Send PM"/>


    <asp:Panel ID="sendPM" runat="server" Style="display: none;max-width:600px;" EnableViewState="false">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="header" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD2" runat="server" EnableViewState="false">
                            <span class="mainModalTitle" id="spanTitle1">Send private message</span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB2" CausesValidation="false" EnableViewState="false" />
                    </div>
                </div>
                <div class="mainModalContent clearfix">
                    <div id="Div3">
                    <asp:Label ID="Label1" runat="server" Text="Subject" AssociatedControlID="tbxSubject"></asp:Label>&nbsp;<asp:TextBox ID="tbxSubject" runat="server"></asp:TextBox>
                    <br />
                    <asp:TextBox ID="qrMessage" runat="server" CssClass="QRMsgArea" Rows="5" 
                        TextMode="MultiLine"></asp:TextBox>
                    <br />
                    <asp:Label ID="pmSuccess" runat="server" Text="" />
                    <asp:LinkButton ID="btnSend" runat="server" Text="<%$ Resources:webResources,btnSend %>" OnClick="SendPm" CausesValidation="false" />
                    <asp:LinkButton ID="btnCancel" runat="server" Text="<%$ Resources:webResources,btnCancel %>" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
<asp:ModalPopupExtender ID="Panel1_ModalPopupExtender" runat="server" BackgroundCssClass="modalBackground"
    Enabled="True" TargetControlID="ImageButton1" PopupControlID="sendPM" CancelControlID="btnCancel"></asp:ModalPopupExtender>
