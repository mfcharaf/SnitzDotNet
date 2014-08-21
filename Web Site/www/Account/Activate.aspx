<%-- 
##############################################################################################################
## Snitz Forums .net
##############################################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##############################################################################################################
--%>
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/plain.Master" AutoEventWireup="true" CodeBehind="Activate.aspx.cs" Inherits="SnitzUI.Activate" %>

<asp:Content ID="Content3" ContentPlaceHolderID="CPM" runat="server">
    <asp:Panel ID="ActivationPanel" runat="server" style="width:50%;margin:auto;" DefaultButton="btnActivate" >
        <table style="text-align: center" class="ForumList">
            <tr>
                <td colspan="2" style="height: 21px" class="Tableheader">
                    <asp:Label ID="Label4" runat="server" Text="Account Activation"></asp:Label></td>
            </tr>
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td style="height: 26px;" align="right">
                    <asp:Label ID="Label1" runat="server" Text="Username"></asp:Label>
                    :</td>
                <td style="width: 422px; height: 26px;" align="left">
                    <asp:TextBox ID="username" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="username"
                        ErrorMessage="username required">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td style="height: 21px" align="right">
                    <asp:Label ID="Label2" runat="server" Text="Password"></asp:Label>
                    :</td>
                <td style="width: 422px; height: 21px" align="left">
                    <asp:TextBox ID="password" runat="server" TextMode="Password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="password"
                        ErrorMessage="Password required">*</asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Label ID="Label5" runat="server" Text="Code:"></asp:Label></td>
                <td align="left" style="height: 24px; width: 422px;">
                    <asp:TextBox ID="ActivationCode" runat="server"></asp:TextBox>
                    <asp:CustomValidator ID="ActivationCodeValidator" runat="server" ControlToValidate="ActivationCode"
                        ErrorMessage="Passkey does not match" OnServerValidate="ActivationCodeValidatorServerValidate"
                        ValidateEmptyText="True" ValidationGroup="activate">*</asp:CustomValidator></td>
            </tr>
            <tr>
                <td style="height: 21px" colspan="2">
                    &nbsp;<asp:ValidationSummary ID="ValidationSummary1" runat="server" HeaderText="Activation Error!"
                        ValidationGroup="activate" Width="100%" />
                    &nbsp;<asp:Button ID="btnActivate" runat="server" OnClick="BtnActivateClick" Text="Activate" /></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="ActivatedPanel" runat="server" Height="50px" HorizontalAlign="Center" Visible="False" style="width:75%;margin:auto;">
        <asp:Literal ID="litAccepted" Visible="false" runat="server" Text="Your membership has now been accepted, you may now begin posting in the forums.<br/>"></asp:Literal>
        <asp:Literal ID="litValidated" Visible="False" runat="server" Text="<p>Thank you for completeing your email address validation.<br/> Your registration is now awaiting approval by an administrator, you will receive an email once your registration has been approved.</p><p>&nbsp;</p>"></asp:Literal>
        <asp:Literal ID="litAlreadyValid" Visible="False" runat="server" Text="Your email address has already been validated.<br/>"></asp:Literal>
    </asp:Panel>
</asp:Content>

