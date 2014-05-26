<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumLogin.ascx.cs"
    Inherits="SnitzUI.UserControls.ForumLogin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
</asp:ScriptManagerProxy>
<asp:LinkButton ID="lnkLogin" runat="server" Text="Login" Style="display: none"></asp:LinkButton>
<asp:Panel ID="pnlLogin" runat="server" CssClass="modalPopup" style="display: none">
    <table width="100%" cellpadding="0" cellspacing="0">
        <tr style="background-color: Silver;">
            <td align="right" style="background-color: Silver;">
                <asp:LinkButton ID="lnkCancel" runat="server" Text="[X]"></asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Login ID="login" runat="server" OnAuthenticate="OnAuthenticate" DisplayRememberMe="False"
                            InstructionText="A password is required to access this forum.">
                        </asp:Login>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Panel>
<cc1:ModalPopupExtender ID="popup" runat="server" DropShadow="false" TargetControlID="lnkLogin"
    PopupControlID="pnlLogin" BackgroundCssClass="modalBackgroundDark" CancelControlID="lnkCancel"
    OnCancelScript="HideLoginPopup()">
</cc1:ModalPopupExtender>
