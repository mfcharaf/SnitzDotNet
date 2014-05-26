<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Confirmation.ascx.cs"
    Inherits="SnitzUI.UserControls.Popups.Confirmation" %>
<asp:Literal ID="Literal1" runat="server"></asp:Literal><br />
<input type="button" value="Yes" onclick="mainScreen.SubmitConfirm();" class="btn btnYes" />
&nbsp;
<input type="button" value="No" onclick="mainScreen.CancelModal();" class="btn btnCancel" />
