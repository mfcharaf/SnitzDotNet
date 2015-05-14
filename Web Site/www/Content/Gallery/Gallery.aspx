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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="true" CodeBehind="Gallery.aspx.cs" Inherits="SnitzUI.Gallery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPMeta" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHead" runat="server">

    <link rel="stylesheet" type="text/css" runat="server" id="markitupCSS" />

    <link rel="stylesheet" type="text/css" href="/css/jquery.lightbox-0.5.css" media="screen" />
    <style type="text/css">
        .thumb
        {
            margin: 5px;
            border: 1px solid silver;
            padding: 5px;
        }

    </style>
    <script type="text/javascript" src="/scripts/jquery.lightbox-0.5.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $('.gallerylnk').lightBox();
        });       
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPAd" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPSpace" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPH1" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="CPHR" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="CPHL" runat="server">
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="CPM" runat="server">
<div class="galleryView clearfix">
    <div style="width:120px;float:left;height:100%;margin:0px;margin-right:10px;background-color:White;" class="forumtable">
        <asp:ListView ID="ListView1" runat="server" OnItemCommand="ShowGallery">
            <LayoutTemplate>
                <ul >
                    <li runat="server" id="itemPlaceholder">
                    </li>
                </ul>
            </LayoutTemplate>
            <ItemTemplate>
                <li>
                    <asp:LinkButton   
                        ID="LinkButton1"  
                        CssClass="galleryLnk"
                        runat="server"  
                        CommandName="gallerySelect"  
                        CommandArgument='<%# Container.DataItem %>'
                        Text='<%# Container.DataItem %>' >  
                    </asp:LinkButton>  
                </li>
            </ItemTemplate>
            <EmptyDataTemplate>No public galleries</EmptyDataTemplate>
        </asp:ListView>
    </div>
    <div style="float:left;width:auto;border:1px solid gray;background-color:White;">
        <asp:Repeater ID="rptImage" runat="server" >
            <ItemTemplate >  
            <asp:HyperLink NavigateUrl='<%# Eval("ImagePath") %>' ToolTip='<%# Eval("Name") %>'
                                        ID="hypImg" runat="server" rel='<%# Eval("ImagePath") %>' CssClass="gallerylnk">
                <image src='<%# Eval("ThumbPath") %>' id="imgThumb" class="thumb" /></asp:HyperLink>
                <asp:CheckBox runat="server" ID="selImg" Visible="<%# ShowEdit %>"/>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
    <asp:Label runat="server" ID="strMsg"></asp:Label>
    <asp:Panel runat="server" ID="btnPanel" Visible="<%# ShowEdit %>">
        <asp:LinkButton ID="delImages" CssClass="Snitzbutton" runat="server"  Text='<%$ Resources:webResources,lblDelete %>' OnClick="delImages_Click"/>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content9" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>
<asp:Content ID="Content10" ContentPlaceHolderID="RightCol" runat="server">
</asp:Content>
<asp:Content ID="Content11" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
