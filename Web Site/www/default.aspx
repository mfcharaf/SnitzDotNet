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

<%@ Page AutoEventWireup="True" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master"
    Title="" Inherits="Homepage" Culture="auto" UICulture="auto" CodeBehind="default.aspx.cs" ValidateRequest ="false"
    MaintainScrollPositionOnPostback="true" enableEventValidation="false" viewStateEncryptionMode ="Never" %>

<%@ MasterType TypeName="BaseMasterPage" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Register TagPrefix="stats" TagName="Statistics" Src="~/UserControls/Statistics.ascx" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="asp" Namespace="SnitzUI.UserControls" Assembly="Snitz.UI" %>

<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="CPHead">
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS" />
    <script type="text/javascript">
        var expandedIndex = [];
        var allcpe = [];
        var pagetheme = '<%= Page.Theme %>';

    </script>    
    <script src="/scripts/common.js" type="text/javascript"></script>

    <script src="/scripts/defaultpage.min.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="cph" runat="server" ContentPlaceHolderID="CPHL">
    <div id="GroupDIV" runat="server">
        <strong>Change Category Group</strong>
        <br />
        <asp:DropDownList Visible="true" ID="ddlGroups" runat="server" AutoPostBack="True"
            DataTextField="Value" DataValueField="Key" OnSelectedIndexChanged="DdlGroupsSelectedIndexChanged"
            EnableViewState="False">
        </asp:DropDownList>
    </div>
</asp:Content>

<asp:Content ID="cpm" runat="server" ContentPlaceHolderID="CPM">
    <asp:Repeater OnItemDataBound="CategoryDataListItemDataBound" runat="server" ID="repCatDL" EnableViewState="False">
        <HeaderTemplate>
            <div id="defaultCatTable" style="table-layout: fixed; width: 100%;">
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Panel ID="Cat_HeaderPanel" runat="server" CssClass="statsPanelHeader clearfix" style="cursor: pointer;" EnableViewState="False">
                <asp:HiddenField ID="hdnCatId" runat="server"   
                        Value='<%#Eval("Id")%>' />
             <asp:Image ID="catExpand" runat="server" GenerateEmptyAlternateText="true" EnableViewState="False" ImageAlign="Middle" />
             <span class="cattitle"><%# "&nbsp;" + Eval("Name")%></span>
             <span class="categorybuttons">
                    <asp:ImageButton ID="NewForum" SkinID="Folder" runat="server" Visible='<%# IsAdministrator %>' Text='<%$ Resources:webResources, lblNewForum %>'
                        ToolTip='<%$ Resources:webResources, lblNewForum %>' EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="NewUrl" SkinID="ForumUrl" runat="server" Visible='<%# IsAdministrator %>' Text='<% $ Resources:webResources, lblNewUrl  %>'
                        ToolTip='<%$ Resources:webResources, lblNewUrl  %>' EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="CatLock" SkinID="LockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                            runat="server" ToolTip="<%$ Resources:webResources, lbllockCat %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                    <asp:ImageButton ID="CatUnLock" SkinID="UnLockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                            runat="server" ToolTip="<%$ Resources:webResources, lblUnlockForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                    <asp:ImageButton ID="EditCat" SkinID="Properties" runat="server" Visible='<%# IsAdministrator %>'
                        Text="<%$ Resources:webResources, lblEditCategory %>" ToolTip="<%$ Resources:webResources, lblEditCategory %>" EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="CatDelete" SkinID="DeleteMessage" Visible='<%# IsAdministrator %>' 
                    runat="server" ToolTip="<%$ Resources:webResources, lblDelCategory %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />

                </td>
            </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="Cat_Panel" runat="server" CssClass="statsPanel" EnableViewState="False">

            </asp:Panel>
            <br  style="line-height:0.5em;" />
            <asp:CollapsiblePanelEx ID="Cat_Panel_CollapsiblePanelExtender" SuppressPostBack="true" SkinID="CatExpandSkin"
                CollapseControlID="catExpand" ExpandControlID="catExpand" Collapsed="true"
                runat="server" Enabled="True"  TargetControlID="Cat_Panel"  EnableViewState="true" OnExpand="onExpand" >
            </asp:CollapsiblePanelEx>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    <!-- popup -->
    <asp:Panel ID="MPanel" runat="server" Style="display: none" EnableViewState="False">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="header" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server" EnableViewState="False">
                            <span class="mainModalTitle" id="spanTitle"></span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false" EnableViewState="False" />
                    </div>
                </div>
                <div class="mainModalContent">
                    <div id="mainModalContents">
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="MPanel"
        Drag="true" PopupDragHandleControlID="MPD"
        TargetControlID="btnHid" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="mainScreen.CancelModal();" DropShadow="true" EnableViewState="False" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server">
    <snitz:SideBar runat="server" ID="sidebar" Show="Poll,Events,Ads,Active,Rss" />
</asp:Content>

<asp:Content ID="C3" runat="server" ContentPlaceHolderID="CPF1">
    <br style="line-height: 0.4em;" />
    <stats:Statistics ID="Statistics1" runat="server" Visible='<%# Config.ShowStats %>' EnableViewState="False" />
</asp:Content>
<asp:Content ID="cpf" runat="server" ContentPlaceHolderID="CPF2"></asp:Content>
