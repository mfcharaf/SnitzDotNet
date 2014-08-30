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

<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="true"
    Inherits="ActiveTopicPage" Title="<%$ Resources:webResources, ttlActivePage %>" Culture="auto"
    UICulture="auto" MaintainScrollPositionOnPostback="true" CodeBehind="active.aspx.cs" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<%@ Import Namespace="SnitzCommon" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register TagPrefix="uc2" Src="~/UserControls/GridPager.ascx" TagName="gridpager" %>
<%@ Reference Control="~/UserControls/SideColumn.ascx" %>
<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="head" ContentPlaceHolderID="CPHead" runat="server">

    <script src="/scripts/common.js" type="text/javascript"></script>
    <script type="text/javascript">
        var urltarget = '<%# Profile.LinkTarget %>';
        var RefreshTimer;
        $(document).ready(function () {
            $(".bbcode").each(function () {
                $(this).html(parseBBCode(parseEmoticon($(this).text(), '<%= Page.Theme %>')));
            });
            //need to fix the Page method path in case we got here from a routing rule
            PageMethods.set_path('/Content/Forums/active.aspx');
        });
        function setRefresh(interval) {
            if (RefreshTimer == undefined) {
                RefreshTimer = self.setInterval('RefreshActive()', interval);
            }
        };
        function cancelRefresh() {
            if (RefreshTimer != undefined) {
                RefreshTimer = self.clearInterval(RefreshTimer);
            }
        };
        function RefreshActive() {
            window.__doPostBack('ctl00$CPHR$ddlPageRefresh', '');
        };

    </script>
    <script src="/scripts/message_funcs.min.js" type="text/javascript"></script>
    <style type="text/css">
        #breadcrumbDIV{ display: none;}
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="CPH1" runat="server">
    <script type="text/javascript" language="javascript">
<!-- 

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var postBackElement;

        function CancelAsyncPostBack() {
            if (prm.get_isInAsyncPostBack()) {
                prm.abortPostBack();
            }
        }

        prm.add_initializeRequest(InitializeRequest);
        prm.add_endRequest(EndRequest);

        function InitializeRequest(sender, args) {
            if (prm.get_isInAsyncPostBack()) {
                args.set_cancel(true);
            }
            postBackElement = args.get_postBackElement();
            if (postBackElement.id == '<%= ddlTopicsSince.ClientID %>' || postBackElement.id == '<%= ddlPageRefresh.ClientID %>') {
                $get('<%= UpdateProgress1.ClientID %>').style.display = 'block';
            }
        }
        function EndRequest(sender, args) {
            if (postBackElement.id.search("ucSearch") == '<%= ddlTopicsSince.ClientID %>' || postBackElement.id == '<%= ddlPageRefresh.ClientID %>') {
                $get('<%= UpdateProgress1.ClientID %>').style.display = 'none';
            }
        }

        // -->
    </script>       
</asp:Content>
<asp:Content ID="CPHL" ContentPlaceHolderID="CPHL" runat="server">
    <div class="clearfix">
        <asp:DropDownList CssClass="ddTopicSince" ID="ddlTopicsSince" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="DdlTopicsSinceSelectedIndexChanged" EnableViewState="False">
            <asp:ListItem>Since Last Visit</asp:ListItem>
            <asp:ListItem Value="15m">Last 15 minutes</asp:ListItem>
            <asp:ListItem Value="30m">Last 30 minutes</asp:ListItem>
            <asp:ListItem Value="45m">Last 45 minutes</asp:ListItem>
            <asp:ListItem Value="60m">Last hour</asp:ListItem>
            <asp:ListItem Value="120m">Last 2 hours</asp:ListItem>
            <asp:ListItem Value="360m">Last 6 hours</asp:ListItem>
            <asp:ListItem Value="720m">Last 12 hours</asp:ListItem>
            <asp:ListItem Value="1d">Yesterday</asp:ListItem>
            <asp:ListItem Value="2d">Last 2 days</asp:ListItem>
            <asp:ListItem Value="7d">Last week</asp:ListItem>
            <asp:ListItem Value="14d">Last fortnight</asp:ListItem>
            <asp:ListItem Value="1m">Last month</asp:ListItem>
            <asp:ListItem Value="2m">Last 2 months</asp:ListItem>
            <asp:ListItem Value="1y">Last Year</asp:ListItem>
        </asp:DropDownList>
        <asp:HiddenField ID="hdnLastOpened" runat="server" EnableViewState="False" />
    </div>
</asp:Content>
<asp:Content ID="CPHR" ContentPlaceHolderID="CPHR" runat="server">
    <asp:DropDownList ID="ddlPageRefresh" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlPageRefreshSelectedIndexChanged"
        EnableViewState="False" Style="float: right;">
        <asp:ListItem Value="">Don't reload automatically</asp:ListItem>
        <asp:ListItem Value="1">reload page every minute</asp:ListItem>
        <asp:ListItem Value="2">reload page every 2 minutes</asp:ListItem>
        <asp:ListItem Value="5">reload page every 5 minutes</asp:ListItem>
        <asp:ListItem Value="10">reload page every 10 minutes</asp:ListItem>
        <asp:ListItem Value="15">reload page every 15 minutes</asp:ListItem>
    </asp:DropDownList>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="CPM" runat="server">
    <asp:ObjectDataSource EnablePaging="True" ID="ActiveTopicODS" OldValuesParameterFormatString="original_{0}" OnSelected="ActiveTopicOdsSelected" OnSelecting="ActiveTopicOdsSelecting" runat="server" 
        SelectCountMethod="GetNewTopicCount" 
        SelectMethod="GetNewTopics" TypeName="Snitz.BLL.Topics">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="" Name="lastHereDate" SessionField="_SinceDate" Type="String" />
            <asp:SessionParameter DefaultValue="false" Name="isAdminOrModerator" SessionField="_IsAdminOrModerator" Type="Boolean" />
            <asp:Parameter Name="startRowIndex" Type="Int32" />
            <asp:Parameter Name="maximumRows" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="TopicUpdatePanel" >
    <ProgressTemplate>
        <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;"  >
            <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
        </div>
    </ProgressTemplate>
</asp:UpdateProgress> 
    <asp:UpdatePanel ID="TopicUpdatePanel" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <script type="text/javascript">
                window.Sys.Application.add_load(BindEvents);
            </script>
            <br class="seperator" />
            <asp:GridView ID="ActiveTable" runat="server"
                AutoGenerateColumns="False" AllowPaging="True"
                DataSourceID="ActiveTopicODS" Width="100%"
                EmptyDataText="<%$ Resources:webResources, ErrNoTopics %>"
                CellPadding="3" GridLines="None"
                EnableViewState="False" CssClass="forumtable noborder"
                EnableModelValidation="True"
                OnRowDataBound="ActiveTableRowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText=" ">
                        <HeaderStyle CssClass="iconCol" />
                        <ItemTemplate>
                        </ItemTemplate>
                        <ItemStyle CssClass="iconCol" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="CatId" SortExpression="CatId" HeaderStyle-Width="0px" />
                    <asp:BoundField DataField="ForumId" SortExpression="ForumId" HeaderStyle-Width="0px" />
                    <asp:TemplateField HeaderText="<%$ Resources:webResources, lblTopic %>"
                        SortExpression="Subject">
                        <ItemTemplate>
                            <asp:Image ID="imgPosticonSmall" SkinID="PosticonSmall" runat="server" Visible="False"
                                GenerateEmptyAlternateText="true" />
                            <a href='/Content/Forums/topic.aspx?TOPIC=<%# Eval("Id") %>'
                                title='<%# Eval("Subject")%>'>
                                <span class="bbcode"><%# HttpUtility.HtmlDecode(Eval("Subject").ToString()) %></span></a>
                            <br />
                            <%# TopicPageLinks(Eval("ReplyCount"),Eval("Id")) %>
                        </ItemTemplate>
                        <HeaderStyle CssClass="subjCol" />
                        <ItemStyle CssClass="subjCol" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:webResources, lblPostAuthor %>"
                        SortExpression="Author.Name">
                        <ItemTemplate>
                            <asp:Literal ID="profLink" runat="server"
                                Text='<%# Eval("AuthorProfilePopup") %>' />
                        </ItemTemplate>
                        <HeaderStyle CssClass="authorCol" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:webResources, lblReplies %>"
                        SortExpression="ReplyCount">
                        <ItemTemplate>
                            <%# Common.TranslateNumerals(Eval("ReplyCount"))%>
                        </ItemTemplate>
                        <HeaderStyle CssClass="countCol" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:webResources, lblViewCount %>"
                        SortExpression="ViewCount">
                        <ItemTemplate>
                            <%# Common.TranslateNumerals(Eval("Views"))%>
                        </ItemTemplate>
                        <HeaderStyle CssClass="countCol"/>
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="<%$ Resources:webResources, lblLastPost %>"
                        SortExpression="LastPostDate">
                        <ItemTemplate>
                            <span class="smallText">by:&nbsp;<asp:Literal
                                ID="popuplink" runat="server"
                                Text='<%# Eval("LastPostAuthorPopup") %>'></asp:Literal>&nbsp;<asp:HyperLink
                                    ID="lpLnk" runat="server" CssClass="profilelnk" SkinID="JumpTo" NavigateUrl='<%# String.Format("/Content/Forums/topic.aspx?TOPIC={0}&whichpage=-1#{1}", Eval("Id"),Eval("LastReplyId")) %>'
                                    ToolTip="<%$ Resources:webResources, lblLastPostJump %>"
                                    Text="<%$ Resources:webResources, lblLastPostJump %>"></asp:HyperLink></span>
                            <br />
                            <asp:Literal runat="server" ID="lastpostdate"></asp:Literal>
                            
                        </ItemTemplate>
                        <HeaderStyle CssClass="lastpostCol" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    </asp:TemplateField>
                    <asp:TemplateField InsertVisible="False">
                        <HeaderStyle CssClass="buttonCol" />
                        <ItemTemplate>
                            <asp:ImageButton ID="TopicLock" SkinID="LockTopic" Visible='<%# IsAdministrator %>'
                                CommandArgument='<%# Eval("Id")%>' runat="server" ToolTip="<%$ Resources:webResources, lbllock %>"
                                OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="TopicUnLock" SkinID="UnLockTopic" Visible='<%# IsAdministrator %>'
                                CommandArgument='<%# Eval("Id")%>' runat="server" ToolTip="<%$ Resources:webResources, lblUnlock %>"
                                OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="TopicDelete" SkinID="DeleteMessage" Visible='<%# IsAdministrator %>'
                                CommandArgument='<%# Eval("Id")%>' runat="server" ToolTip="<%$ Resources:webResources, lblDelPost %>"
                                OnClientClick="" CausesValidation="False" EnableViewState="False"/>
                            <asp:HyperLink rel="nofollow" ID="hypEditTopic" SkinID="EditTopic" runat="server"
                                Visible="False" Text="<%$ Resources:webResources, lblEditPost %>" ToolTip="<%$ Resources:webResources, lblEditPost %>"></asp:HyperLink>
                            <asp:HyperLink rel="nofollow" ID="hypReplyTopic" SkinID="ReplyTopic" runat="server"
                                Visible="true" Text="<%$ Resources:webResources, lblReply %>" ToolTip="<%$ Resources:webResources, lblReply %>"></asp:HyperLink>
                            <asp:HyperLink rel="nofollow" ID="hypNewTopic" SkinID="NewTopic" runat="server" Text="<%$ Resources:webResources, lblNewTopic %>"
                                ToolTip="<%$ Resources:webResources, lblNewTopic %>"></asp:HyperLink>
                            <asp:ImageButton ID="TopicSub" SkinID="Subscribe" CommandArgument='<%# Eval("Id")%>' CommandName="sub"
                                runat="server" ToolTip="<%$ Resources:webResources, lblSubscribeTopic %>" OnClientClick=""
                                CausesValidation="False" EnableViewState="False"/>
                            <asp:ImageButton ID="TopicUnSub" SkinID="UnSubscribe" CommandArgument='<%# Eval("Id")%>' CommandName="unsub"
                                runat="server" ToolTip="<%$ Resources:webResources, lblUnSubscribeTopic %>" OnClientClick=""
                                CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="TopicApprove" SkinID="approve" runat="server" />

                        </ItemTemplate>
                        <ItemStyle CssClass="buttonCol" />
                    </asp:TemplateField>
                </Columns>
                <PagerTemplate>
                    <uc2:gridpager ID="pager" runat="server" PagerStyle="Linkbutton" />
                </PagerTemplate>
                <HeaderStyle CssClass="category cattitle" />
                <RowStyle CssClass="row" />
                <AlternatingRowStyle CssClass="altrow" />
                <PagerStyle CssClass="row" />
                <EmptyDataRowStyle CssClass="emptyrow" />
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Profile popup -->
    <asp:Panel ID="MPanel" runat="server" Style="display: none">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="header" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server">
                            <span class="mainModalTitle" id="spanTitle"></span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false" />
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
        TargetControlID="btnHid" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="mainScreen.CancelModal();" DropShadow="true" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>

<asp:Content ID="contentfooter" ContentPlaceHolderID="CPF1" runat="server">
    <div class="table-bottom">
        <div class="table-bottom-left">
            &nbsp;
        </div>
        <div class="table-bottom-right">
            <snitz:JumpTo ID="JumpTo1" runat="server" />
        </div>
    </div>
    <div class="table-bottom-footer">
        <asp:Image ID="imgNewPosts" EnableViewState="false" SkinID="FolderNew" runat="server"
            GenerateEmptyAlternateText="true" />
        <asp:Label ID="lblNewPosts" runat="server" Text="<%$ Resources:webResources, lblNewPosts %>"></asp:Label>.
                <br />
        <asp:Image ID="imgOldPosts" EnableViewState="false" SkinID="Folder" runat="server"
            GenerateEmptyAlternateText="true" />
        <asp:Label ID="lblOldPosts" runat="server" Text="<%$ Resources:webResources, lblOldPosts %>"></asp:Label>.
                (<asp:Image ID="imgHotTopic" EnableViewState="false" SkinID="FolderHot" runat="server"
                    GenerateEmptyAlternateText="true" />
        <asp:Label ID="lblHotTopic" runat="server" Text="<%$ Resources:webResources, lblHotTopics %>"></asp:Label>)<br />
        <asp:Image ID="imgfolderNewLocked" EnableViewState="false" SkinID="FolderLocked"
            runat="server" GenerateEmptyAlternateText="true" />
        <asp:Label ID="lblLockedTopic" runat="server" Text="<%$ Resources:webResources, lblLockedTopic %>"></asp:Label>.       
    </div>
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server">
    <snitz:SideBar runat="server" ID="sidebar" Show="Poll,Ads,Stats,Events" />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
