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
<%@ Import Namespace="SnitzCommon" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Register TagPrefix="stats" TagName="Statistics" Src="~/UserControls/Statistics.ascx" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.5.51116.0, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register TagPrefix="asp" Namespace="SnitzUI.UserControls" Assembly="Snitz.UI" %>

<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="CPHead">
    <script src="/scripts/bbcode.min.js" type="text/javascript"></script>
    <script src="/scripts/smilies.min.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS" />

    <script type="text/javascript">
        var expandedIndex = [];
        var allcpe = [];

        function pagebind() {
            $(document).ready(function () {
                $(".bbcode").each(function () {
                    $(this).html(parseBBCode(parseEmoticon($(this).text(), '<%= Page.Theme %>')));
                });
                jQuery("abbr.timeago").timeago();
            });
        };

        pagebind();
        
        $.fn.serializeNoViewState = function () {
            return this.find("input,select,hidden,textarea")
                .not("[type=hidden][name^=__]")
                .serialize();
        };

        function UpdateRoleList(ddlid, hdnid, remove) {
            var rolelist = $get(hdnid).value;
            var newrole = $("#" + ddlid + " option:selected").text();

            var tbl = $('#roletbl');
            if (remove) {
                $("#roletbl td:contains('" + newrole + "')").parent().remove();
                var regx = new RegExp("\\b" + newrole + "(,|$)", "igm");
                rolelist = rolelist.replace(regx, "");
            } else {
                rolelist = rolelist + ',' + newrole;
                if (tbl.html() == null) { // no table so create one
                    $('<table id="roletbl"><tr><td>' + newrole + '</td></tr></table>').appendTo($('#rolelist'));
                } else {
                    $('#roletbl tr:last').before('<tr><td>' + newrole + '</td></tr>');
                }
            }
            $get(hdnid).value = rolelist;
        }
        function UpdateModerator(ddlid, hdnid, remove) {
            var modlist = $get(hdnid).value;
            var newmod = $("#" + ddlid + " option:selected").text();

            var tbl = $('#modtbl');
            if (remove) {
                $("#roletbl td:contains('" + newmod + "')").parent().remove();
                var regx = new RegExp("\\b" + newmod + "(,|$)", "igm");
                modlist = modlist.replace(regx, "");
            } else {
                modlist = modlist + ',' + newmod;
                if (tbl.html() == null) { // no table so create one
                    $('<table id="modtbl"><tr><td>' + newmod + '</td></tr></table>').appendTo($('#modlist'));
                } else {
                    $('#modtbl tr:last').after('<tr><td>' + newmod + '</td></tr>');
                }
            }
            $get(hdnid).value = modlist;
        }
        function SaveForum() {
            window.PageMethods.SaveForum($("form").serializeNoViewState());
            var millisecondsToWait = 500;
            setTimeout(function () {
                mainScreen.CancelModal();
                location.reload();
            }, millisecondsToWait);
            
        }
        function SaveCategory() {
            window.PageMethods.SaveCategory($("form").serializeNoViewState());
            var millisecondsToWait = 500;
            setTimeout(function () {
                mainScreen.CancelModal();
                location.reload();
            }, millisecondsToWait);
        }

        function pageLoad()
        {
            var allBehaviors = window.Sys.Application.getComponents();
            for (var loopIndex = 0; loopIndex < allBehaviors.length; loopIndex++) {
                var currentBehavior = allBehaviors[loopIndex];
                if (currentBehavior.get_name() == "CollapsiblePanelBehavior") {
                    allcpe.push(currentBehavior);
                }

            }
            if (getCookie()) {
                expandedIndex = getCookie().split(',');;
                for (var cpeIndex = 0; cpeIndex < expandedIndex.length; cpeIndex++) {
                    var expandedcpe = expandedIndex[cpeIndex];
                    $find(expandedcpe).set_Collapsed(false);
                }
            }
        }
        function pageUnload() {
            expandedIndex = null;
            expandedIndex = [];
            for (var cpeIndex = 0; cpeIndex < allcpe.length; cpeIndex++) {
                var currentcpe = allcpe[cpeIndex];
                if (!currentcpe.get_Collapsed()) {
                    //save the expanded cpe's index
                    expandedIndex.push(currentcpe.get_id());
                }
            }
            setCookie(expandedIndex);
        }
        
        function setCookie(cookieValue) {
            var sVar = "cookiename";
            var theCookie = sVar + '=' + cookieValue + '; expires=Fri, 1 Jul 2019 11:11:11 UTC';
            document.cookie = theCookie;
        }
        function getCookie() {
            var sVar = "cookiename";
            var cookies = document.cookie.split('; ');
            for (var i = 1; i <= cookies.length; i++) {
                if (cookies[i - 1].split('=')[0] == sVar) {
                    return cookies[i - 1].split('=')[1];
                }
            }
            return "";
        }
        
        //Sender: Reference to the CollapsiblePanelExtender Client Behavior  
        //eventArgs: Empty EvenArgs  
        function onExpand(sender, eventArgs) {
            //Use sender (instance of CollapsiblePanerExtender client Behavior)  
            //to get ExpandControlID.  
            var expander = $get(sender.get_ExpandControlID());

            //Using RegEx to replace pnlCustomer with hdnCustId.  
            //hdnCustId is a hidden field located within pnlCustomer.  
            //pnlCustomer is a Panel, and Panels are not Naming Container.  
            //So hdnCustId will have the same ID as pnlCustomer but with   
            //'hdnCustId' at the end insted of pnlCustomer.  
            var custId = $get(sender.get_ExpandControlID().replace(/Cat_HeaderPanel/g, 'hdnCatId')).value;

            //Issue AJAX call to PageMethod, and send sender   
            //object as userContext Parameter.  
            PageMethods.GetForums(custId, OnSucceeded, OnFailed, sender);
            
        }
        // Callback function invoked on successful   
        // completion of the page method.  
        function OnSucceeded(result, userContext, methodName) {
            //$get('progress').style.visibility = 'hidden';
            //userContext is sent while issue AJAX call  
            //it is an instance of CollapsiblePanelExtender client Behavior.  
            //Used to get the collapsible element and sent its innerHTML   
            //to the returned result.             
            userContext.get_element().innerHTML = result;
            pagebind();
        }
        // Callback function invoked on failure   
        // of the page method.  
        function OnFailed(error, userContext, methodName) {
            //$get('progress').style.visibility = 'hidden';
            alert(error.get_message());
        }
        function setArgAndPostBack(msg, btn, id) {
            myConfirm(msg, function () {
                $("#__EVENTARGUMENT").val(id);
                __doPostBack(btn, id);
                }, function () {
                    //cancelled so do nothing
                },
              'Confirmation Required'
            );

        }
        function myConfirm(dialogText, okFunc, cancelFunc, dialogTitle) {
            $('<div style="padding: 10px; max-width: 500px; word-wrap: break-word;">' + dialogText + '</div>').dialog({
                draggable: false,
                modal: true,
                resizable: false,
                width: 'auto',
                title: dialogTitle || 'Confirm',
                minHeight: 75,
                buttons: {
                    OK: function () {
                        if (typeof (okFunc) == 'function') {
                            setTimeout(okFunc, 50);
                        }
                        $(this).dialog('destroy');
                    },
                    Cancel: function () {
                        if (typeof (cancelFunc) == 'function') {
                            setTimeout(cancelFunc, 50);
                        }
                        $(this).dialog('destroy');
                    }
                }
            });
        }
        
        
    </script>
</asp:Content>

<asp:Content ID="cph" runat="server" ContentPlaceHolderID="CPHL">
    <div id="GroupDIV" runat="server">
        <strong>Change Category Group</strong>
        <br />
        <asp:DropDownList Visible="true" ID="ddlGroups" runat="server" AutoPostBack="True"
            DataTextField="Value" DataValueField="Key" OnSelectedIndexChanged="DdlGroupsSelectedIndexChanged"
            meta:resourcekey="ddlGroups" EnableViewState="False">
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
                CollapseControlID="Cat_HeaderPanel" ExpandControlID="Cat_HeaderPanel" Collapsed="true"
                runat="server" Enabled="True" TargetControlID="Cat_Panel"  EnableViewState="true" OnExpand="onExpand" >
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
