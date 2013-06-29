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
    Title="" Inherits="Homepage" Culture="auto" UICulture="auto" CodeBehind="default.aspx.cs"
    MaintainScrollPositionOnPostback="true" %>

<%@ MasterType TypeName="BaseMasterPage" %>
<%@ Import Namespace="SnitzCommon" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Register TagPrefix="stats" TagName="Statistics" Src="~/UserControls/Statistics.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="head" runat="server" ContentPlaceHolderID="CPHead">
    <script src="/scripts/bbcode.js" type="text/javascript"></script>
    <script src="/scripts/smilies.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS" />

    <script type="text/javascript">
        $(document).ready(function () {
            $(".bbcode").each(function () {
                $(this).html(parseBBCode(parseEmoticon($(this).text())));
            });

        });
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
            PageMethods.SaveForum($("form").serializeNoViewState());
            var millisecondsToWait = 500;
            setTimeout(function () {
                mainScreen.CancelModal();
                location.reload();
            }, millisecondsToWait);
            
        }
        function SaveCategory() {
            PageMethods.SaveCategory($("form").serializeNoViewState());
            var millisecondsToWait = 500;
            setTimeout(function () {
                mainScreen.CancelModal();
                location.reload();
            }, millisecondsToWait);
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
    <asp:Repeater OnItemDataBound="CategoryDataListItemDataBound" runat="server" ID="repCatDL"
        EnableViewState="False">
        <HeaderTemplate>
            <table id="defaultForumTable" class="forumTable" style="table-layout: fixed; width: 100%;">
        </HeaderTemplate>
        <ItemTemplate>
            <tr valign="top" class="category">
                <td class="CategoryExpanded" width="20px" style="cursor: pointer; border-right: 0px; padding: 3px;" onclick="_getRowIndex(this)"></td>
                <td class="category" width="*" style="cursor: pointer; border-right: 0px; border-left: 0px;" onclick="_getRowIndex(this)">
                    <span class="cattitle"><%# "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + Eval("Name")%></span>
                </td>
                <td width="160px" style="border-left: 0px;" class="category" align="right">
                    <asp:ImageButton ID="NewForum" SkinID="Folder" runat="server" Visible='<%# IsAdministrator %>' Text="<%$ Resources:webResources, lblNewForum %>"
                        ToolTip="<%$ Resources:webResources, lblNewForum %>" EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="NewUrl" SkinID="NewUrl" runat="server" Visible='<%# IsAdministrator %>' Text="<%$ Resources:webResources, lblNewUrl %>"
                        ToolTip="<%$ Resources:webResources, lblNewUrl %>" EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="CatLock" SkinID="LockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                            runat="server" ToolTip="<%$ Resources:webResources, lbllockCat %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Lock', 'Lock?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to lock the Category?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="LockCategory" />
                    <asp:ImageButton ID="CatUnLock" SkinID="UnLockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                            runat="server" ToolTip="<%$ Resources:webResources, lblUnlockForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm UnLock', 'Unlock?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to unlock the Category?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="UnLockCategory" />
                    <asp:ImageButton ID="EditCat" SkinID="Properties" runat="server" Visible='<%# IsAdministrator %>'
                        Text="<%$ Resources:webResources, lblEditCategory %>" ToolTip="<%$ Resources:webResources, lblEditCategory %>" EnableViewState="False"></asp:ImageButton>
                    <asp:ImageButton ID="CatDelete" SkinID="DeleteMessage" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                    runat="server" ToolTip="<%$ Resources:webResources, lblDelCategory %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to delete the Category?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete the Category?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="DeleteCategory" />

                </td>
            </tr>
            <asp:Repeater ID="repForum" runat="server" OnItemDataBound="RepForumItemDataBound" EnableViewState="False">
                <HeaderTemplate>
                    <tr style="padding: 0px;">
                        <td style="padding: 0px;" valign="top" colspan="3">
                            <table width="100%" class="forumtable" cellpadding="3" cellspacing="0" style="width: 100%;
                                margin: 0px; table-layout: fixed;" border="0" >
                                <thead runat="server" ID="fTableHeader">
                                <tr>
                                <th class="tableheader" width="20px"></th>
                                <th class="tableheader nowrap" width="*">
                                    <asp:Label ID="LF" runat="server" Text="<%$ Resources:webResources, lblForum %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader center nowrap" width="60px">
                                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:webResources, lblTopics %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader center nowrap" width="60px">
                                    <asp:Label ID="Label2" runat="server" Text="<%$ Resources:webResources, lblPosts %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader center" width="180px">
                                    <asp:Label ID="Label3" runat="server" Text="<%$ Resources:webResources, lblLastPost %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader" style="width: 50px"></th>
                            </tr>
                            </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="row">
                        <td valign="top" width="20px">
                            <asp:PlaceHolder ID="Ticons" runat="server" EnableViewState="False"></asp:PlaceHolder>
                        </td>
                        <td valign="top" runat="server" id="linkCol" width="*">
                            <asp:HyperLink CssClass="forumlink bbcode" NavigateUrl='<%# String.Format("/Content/Forums/forum.aspx?FORUM={0}",Eval("Id")) %>' runat="server" ID="forumLink"><%# Eval("Subject") %></asp:HyperLink><br />
                            <span class="smallText bbcode"><%# Eval("Description") %></span>
                        </td>
                        <td class="nowrap" align="center" valign="top" runat="server" id="tCount" width="60px">
                            <%# Common.TranslateNumerals(Eval("TopicCount"))%>
                        </td>
                        <td class="nowrap" style="text-align: center" valign="top" runat="server" id="pCount" width="60px">
                            <%# Common.TranslateNumerals(Eval("PostCount"))%>
                        </td>
                        <td class="smallText" style="overflow: hidden; white-space: nowrap;" runat="server" id="lastpost" width="160px">
                            <asp:HyperLink ID="hypTopic" runat="server" Visible='<%# (Eval("LastTopicId") != null) %>' NavigateUrl='/Content/Forums/topic.aspx?TOPIC=<%# Eval("LastTopicId") %>' ToolTip='<%# Eval("LastPostSubject") %>'>
                                <span class="bbcode"><%# Eval("LastPostSubject") %></span><br /></asp:HyperLink>
                            <asp:Literal ID="lDate" runat="server" EnableViewState="False" />
                            <br />
                            <asp:Label ID="Label4" runat="server" Text="by:" Visible='<%# Convert.ToInt32(Eval("TopicCount")) > 0 %>' EnableViewState="False"></asp:Label>
                            <asp:Literal ID="popuplink" runat="server" Text='<%# Eval("LastPostAuthor.ProfilePopup") %>' EnableViewState="False"></asp:Literal>

                            &nbsp;<a href='/Content/Forums/topic.aspx?TOPIC=<%# Eval("LastTopicId") %>&amp;whichpage=-1#<%# Eval("LastreplyId") %>'><asp:Image
                                ID="imgLastPost" Visible='<%# (Config.ShowLastPostLink && (Eval("LastTopicId") != null && (int)Eval("LastTopicId") > 0)) %>'
                                SkinID="Lastpost" ImageAlign="Bottom" runat="server" AlternateText="<%$ Resources:webResources, lblLastPostJump %>"
                                ToolTip="<%$ Resources:webResources, lblLastPostJump %>" EnableViewState="False" /></a>
                        </td>
                        <td id="adminBtn" runat="server" valign="top" width="50px">
                            <asp:HyperLink ID="hypNewTopic" SkinID="NewTopic" runat="server" Text="<%$ Resources:webResources, lblNewTopic %>"
                                ToolTip="<%$ Resources:webResources, lblNewTopic %>" EnableViewState="False"></asp:HyperLink>
                            <asp:ImageButton ID="ForumEdit" SkinID="Properties" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                runat="server" ToolTip="<%$ Resources:webResources, lblEditForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ForumLock" SkinID="LockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lbllockForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Lock', 'Do you want to lock the Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to lock the Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="LockForum" />
                            <asp:ImageButton ID="ForumUnLock" SkinID="UnLockTopic" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblUnlockForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm UnLock', 'Do you want to unlock the Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to unlock the Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="UnLockForum" />
                            <asp:ImageButton ID="ForumDelete" SkinID="DeleteMessage" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblDelForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to delete the Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete the Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="DeleteForum" />
                            <asp:ImageButton ID="ForumEmpty" SkinID="EmptyForum" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblEmptyForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Empty', 'Do you want to delete all the posts in the Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to delete all the posts in the Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="EmptyForum" />
                            <asp:ImageButton ID="ForumSub" SkinID="Subscribe" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>' CommandName="sub"
                                    runat="server" ToolTip="<%$ Resources:webResources, lblSubscribeForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Subscribe', 'Do you want to subscribe to new posts in the Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to subscribe to new posts in the Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="SubscribeForum" />
                            <asp:ImageButton ID="ForumUnSub" SkinID="UnSubscribe" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>' CommandName="unsub"
                                    runat="server" ToolTip="<%$ Resources:webResources, lblUnSubscribeForum %>" OnClientClick="mainScreen.ShowConfirm(this, 'Remove Subscription', 'Do you want to remove your subscription for this Forum?');
mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Do you want to remove your subscription for this Forum?'},'confirmHandlers.BeginRecieve');
return false;" CausesValidation="False" EnableViewState="False" OnClick="SubscribeForum" />
                        
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table></td></tr>
                <tr>
                    <td colspan="3" style="height: 10px;"></td>
                </tr>
                </FooterTemplate>
            </asp:Repeater>
        </ItemTemplate>

        <FooterTemplate>
            </table>
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
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="MPanel"
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
<asp:Content ID="cpf" runat="server" ContentPlaceHolderID="CPF2">
</asp:Content>
