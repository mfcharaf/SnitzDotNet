<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>
<%@ Page Language="C#" MasterPageFile="~/Master_Templates/MainMaster.master" AutoEventWireup="true" CodeFile="layout.aspx.cs" Inherits="layout" Title="Untitled Page" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<asp:Content ID="C1" runat="server" ContentPlaceHolderID="CPH1">
<div style="width:100%;Height:150px;border:1px solid green;margin:auto;background-color:#c3c3c3;">
CPH1 - Posting buttons and group dropdown go here..
    <div id="ThemeManager">
    <asp:ObjectDataSource ID="dsMasterPages" runat="server" SelectMethod="GetMasterPages" TypeName="MasterPageManager"></asp:ObjectDataSource>
    <span class="smallText">Choose a Layout&nbsp;</span>
    <asp:DropDownList ID="DropDownList2" runat="server" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged" AutoPostBack="True" EnableViewState="False" OnDataBound="DropDownList2_DataBound" DataSourceID="dsMasterPages" DataValueField="name">
    </asp:DropDownList>    
        <br />
        <div id="Div1">
            <asp:ObjectDataSource ID="dsThemes" runat="server" SelectMethod="GetThemes" TypeName="ThemeManager">
            </asp:ObjectDataSource>
            <span class="smallText">
                <asp:Label ID="lbl4" runat="server" SkinID="lblThemes"></asp:Label>
                <br />
            </span>
            <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" DataSourceID="dsThemes"
                DataValueField="name" EnableViewState="False" OnDataBound="DropDownList1_DataBound"
                OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="C3" runat="server" ContentPlaceHolderID="CPHL">
    <div style="width:100%;Height:100px;margin:0px;border:1px solid green;text-align:center;">CPHL</div></asp:Content>
<asp:Content ID="C6" runat="server" ContentPlaceHolderID="CPHR">
    <div style="width:100%;Height:100px;margin:0px;border:1px solid green;text-align:center;">CPHR</div></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPM" Runat="Server">
<div style="width:100%;Height:300px;margin:auto;border:1px solid green;background-color:gray;text-align:center;"><br /><br /><br />CPM<br />This is the main content window and holds the forum tables</div>
<br style="line-height:0.2em" /></asp:Content>

<asp:Content ID="Content4" runat="server" ContentPlaceHolderID="CPF1">
 <div style="width:100%;Height:100px;margin:auto;border:1px solid green;background-color:#c3c3c3;text-align:center;">CPF1<br />This holds the content between the CPM window and the footer. (currently JumpTo and Icon legend)</div></asp:Content>
<asp:Content ID="Content5" runat="server" ContentPlaceHolderID="CPF2">
<div style="width:100%;Height:auto;margin:auto;border:1px solid green;text-align:center;">CPF2<br />Just an extra content box can be moved anywhere, not used by the forum for any functionality, currently holds the W3C validation links.<br /><br /></div></asp:Content>

