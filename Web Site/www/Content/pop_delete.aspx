<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>
<%@ Page Language="C#" MasterPageFile="~/PopUp.master" AutoEventWireup="true" CodeFile="pop_delete.aspx.cs" Inherits="pop_delete" Title="<%$ Resources:webResources, ttlPopupDel %>" Culture="auto" UICulture="auto" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br />
    <asp:Label ID="Label1" runat="server" Text="" EnableViewState="False" meta:resourcekey="Label1Resource1"></asp:Label><br />
    <br />
    <asp:Button ID="btnYes" runat="server" Text="<%$ Resources:webResources, btnYes %>" EnableViewState="False" OnClick="btnOk_Click"/>
    <asp:Button ID="btnNo" runat="server" Text="<%$ Resources:webResources, btnNo %>" EnableViewState="False" OnClientClick="javascript:window.close();return false;" UseSubmitBehavior="False" /><br />
    <br />
    &nbsp;<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="CustomValidator"
        OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
</asp:Content>

