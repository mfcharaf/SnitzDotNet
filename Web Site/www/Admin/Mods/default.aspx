<%@ Page Title="Mod Administration" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master"  AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="SnitzUI.Admin.Mods._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPMeta" runat="server">
</asp:Content>
<asp:Content ID="head" ContentPlaceHolderID="CPhead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="menuCSS"/>

</asp:Content>
<asp:Content ID="adOverride" ContentPlaceHolderID="CPAd" runat="server">
    
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="CPSpace" runat="server">
</asp:Content>
<asp:Content ID="headTitle" ContentPlaceHolderID="CPH1" runat="server">

</asp:Content>

<asp:Content ID="main" ContentPlaceHolderID="CPM" runat="server">
    <div id="container" style="" class="clearfix">
        <div id="leftcolumn">
            <h2>Available Mods</h2>
            <asp:DataList ID="ModMenu" runat="server">
                <ItemTemplate>
                    <asp:LinkButton CssClass="modLink" ID="LinkButton1" runat="server" Text='<%# Eval("key") %>' CommandArgument='<%# Eval("value") %>' OnClick="LoadMod"></asp:LinkButton>
                </ItemTemplate>
            </asp:DataList>
        </div>

        <div id="contentcolumn" style="position:relative;">
            <asp:UpdatePanel runat="server" ChildrenAsTriggers="True" ID="updPnl">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ModMenu"/>
                </Triggers>
                <ContentTemplate>
                    <asp:PlaceHolder ID="modPh" runat="server"></asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdateProgress runat="server" ID="updProgress" AssociatedUpdatePanelID="updPnl" >
                <ProgressTemplate>
                    <div style="position:absolute;top:0px;left:0px; width:100%;height:100%;background:#fff;filter: alpha(opacity=50);-moz-opacity:.5; opacity:.5;"  >
                        <img src="/images/ajax-loader.gif" style="position:relative; top:40%;left:40%" />
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>    
        </div>   
    </div>
    
</asp:Content>

<asp:Content ID="foot" ContentPlaceHolderID="W3CVal" runat="server">
</asp:Content>