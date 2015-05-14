<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>

<%@ Control Language="C#" AutoEventWireup="True"  Inherits="User_Controls_MemberSearch" Codebehind="MemberSearch.ascx.cs" %>

<asp:Panel ID="searchPnl" runat="server" DefaultButton="btnSearch">
<div class="SearchFilters">
        <div class="content clearfix">
            <strong><asp:Label ID="Label1" runat="server" Text="<%$ Resources:webResources, lblSearch %>"></asp:Label>:</strong>&nbsp;
            <asp:CheckBox ID="cbxUserName" runat="server" Text="<%$ Resources:webResources, lblUsername %>" Checked="True" EnableViewState="False" />&nbsp;
            <asp:CheckBox ID="cbxFirstName" runat="server" Text="<%$ Resources:webResources, lblFirstName %>" EnableViewState="False" />&nbsp;
            <asp:CheckBox ID="cbxLastName" runat="server" Text="<%$ Resources:webResources, lblLastName %>" EnableViewState="False" />&nbsp;
            <asp:CheckBox ID="cbxCountry" runat="server" Text="<%$ Resources:webResources, lblCountry %>" EnableViewState="False" />&nbsp;
            <asp:CheckBox ID="cbxEmail" runat="server" Text="<%$ Resources:webResources, lblEmail %>" Visible="False" EnableViewState="False" /><br/> 
            <strong><asp:Label ID="Label2" runat="server" Text="<%$ Resources:webResources, lblFor %>" EnableViewState="False"></asp:Label>:</strong>&nbsp;<asp:TextBox ID="tbxSearchFor" runat="server" meta:resourcekey="tbxSearchForResource1"></asp:TextBox>
            &nbsp;<asp:LinkButton ID="btnSearch" Text="<%$ Resources:webResources, btnSearch %>" runat="server" AlternateText="<%$ Resources:webResources, btnSearch %>" EnableViewState="False" OnClick="btnSearch_Click" style="float:right;"/>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbxSearchFor"
            EnableViewState="False" ErrorMessage="<%$ Resources:webResources, ErrNoSearchString %>"
            SetFocusOnError="True" ><asp:Image ID="Image1" runat="server" SkinID="Error" EnableViewState="False" /></asp:RequiredFieldValidator>
        </div>
</div>

   <div class="SearchLinks">
   <asp:PlaceHolder ID="plcLink" runat="server" EnableViewState="False"></asp:PlaceHolder>
  </div>

</asp:Panel>
