<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>
<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/PopUp.master" AutoEventWireup="true" Inherits="pop_lock" Title="" Culture="auto" UICulture="auto" Codebehind="pop_lock.aspx.cs" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <br />
    <asp:Label ID="Label1" runat="server" Text="Are You Sure?" EnableViewState="False"></asp:Label><br />
    <br />
    <asp:Button ID="btnOk" runat="server" Text="<%$ Resources:webResources, btnYes %>" EnableViewState="False"/>&nbsp;
    <asp:Button ID="btnNo" runat="server" Text="<%$ Resources:webResources, btnNo %>" EnableViewState="False" OnClientClick="javascript:window.close();return false;" UseSubmitBehavior="False"/><br />
    <br />
    &nbsp;<asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="CustomValidator"
        OnServerValidate="CustomValidator1_ServerValidate"></asp:CustomValidator>
</asp:Content>

