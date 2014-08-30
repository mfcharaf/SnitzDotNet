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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" EnableViewState="false"
    AutoEventWireup="true" Culture="auto" UICulture="auto" CodeBehind="Topic.aspx.cs" MaintainScrollPositionOnPostback="true"
    Inherits="SnitzUI.TopicPage" %>
<%@ MasterType virtualpath="~/MasterTemplates/MainMaster.Master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Reference Control="~/UserControls/GridPager.ascx" %>
<%@ Reference Control="~/UserControls/Post Templates/ReplyTemplate.ascx" %>
<%@ Reference Control="~/UserControls/Post Templates/BlogReplyTemplate.ascx" %>
<%@ Register TagPrefix="topic" Src="~/UserControls/PagePostButtons.ascx" TagName="PostButtons" %>
<%@ Register Src="~/UserControls/Post Templates/TopicTemplate.ascx" TagPrefix="topic" TagName="TopicTemplate" %>
<%@ Register Src="~/UserControls/Post Templates/PollTemplate.ascx" TagPrefix="topic" TagName="PollTemplate" %>
<%@ Register Src="~/UserControls/Post Templates/BlogTemplate.ascx" TagPrefix="topic" TagName="BlogTemplate" %>
<%@ Register Src="~/UserControls/Sidebar/MinWeblog.ascx" TagPrefix="topic" TagName="MinWeblog" %>


<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ID="scripthead" ContentPlaceHolderID="CPHead">
    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="jsshareCSS"/>
    <link href="/css/shCore.css" rel="stylesheet" type="text/css" />
    <asp:Literal runat="server" ID="shareItScripts"></asp:Literal>
    <script src="/scripts/topic.js" type="text/javascript"></script>
    <script type="text/javascript">
        var confirmHandlers = {};

        function BindControlEvents() {
            //jQuery is wrapped in BindEvents function so it can be re-bound after each callback.
            $(".bbcode").each(function () {
                $(this).html(parseBBCode(parseEmoticon($(this).text(), '<%= Page.Theme %>')));
            });
            SyntaxHighlighter.autoloader(
                'js jscript javascript  /Scripts/syntax/shBrushJScript.min.js',
                'c# c-sharp csharp      /Scripts/syntax/shBrushCSharp.min.js',
                'css                    /Scripts/syntax/shBrushCss.min.js',
                'text plain             /Scripts/syntax/shBrushPlain.min.js',
                'sql                    /Scripts/syntax/shBrushSql.min.js',
                'vb vbnet               /Scripts/syntax/shBrushVb.min.js',
                'xml xhtml xslt html    /Scripts/syntax/shBrushXml.min.js'
            );

            SyntaxHighlighter.all();

            $('#ctl00_CPF1_ctl00_emoticons1_DataList1 a').click(function () {
                return false;
            });
            $('a.video').player({
                width: 380,
                chromeless: 0,
                showTime: 0,
                showPlaylist: 0,
                showTitleOverlay: 0
            });
                

            confirmHandlers.BeginRecieve = function (_result) {
                var res = false;
                if (_result.customStyle && _result.customStyle != "") {
                    mainScreen.LoadStyleSheet(_result.customStyle);
                }
                if (_result.html && _result.html != "") {
                    mainScreen.mainModalContentsDiv.innerHTML = _result.html;
                    res = true;
                }
                if (_result.script && _result.script != "") {
                    eval(_result.script);
                }
                if (!res) {
                    mainScreen.CancelModal();
                } else {
                    mainScreen.mainModalExtender._layout();
                    setTimeout('mainScreen.mainModalExtender._layout()', 3000);
                }
            };
        }

        //Initial bind
        $(document).ready(function () {
            BindControlEvents();
            //Re-bind for callbacks
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_endRequest(function () {
                BindControlEvents();
            });
        });

        function pollloaded() { 
            BindControlEvents();
        }       
    </script>
    <script src="/scripts/message_funcs.min.js" type="text/javascript"></script>
<style type="text/css">
    img.avatar{width: 80px;height: 80px;opacity: 0.4;}
    img.online{width: 80px;height: 80px;opacity: 1;}
    .markItUpEditor{min-height: 140px !important;}
    #emoticons img{border:0px;margin:2px;}
    .AspNet-FormView-Data{ padding: 0px;}

</style>
</asp:Content>
<asp:Content ID="ContentMain" ContentPlaceHolderID="CPM" runat="server" EnableViewState="false">

    <div class="POFTop clearfix">
        <topic:PostButtons ID="pbTop" runat="server"></topic:PostButtons>
        <div id="buttons-expanded"></div>
    </div>
    <div id="MessageList" >
        <div class="TopicDiv">
        <asp:FormView ID="TopicView" runat="server" Width="100%" CellPadding="0" EnableViewState="False"
            OnItemCommand="TopicViewItemCommand" OnDataBound="TopicBound" >
            <HeaderTemplate>
                <div class="topicHeader clearfix">
                    <div class="topicPrev">
                        &laquo;
                        <asp:HyperLink ID="prevTopic" runat="server" NavigateUrl='<%# String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&dir=prev", Eval("Id")) %>'
                            Text="Prev Topic" EnableViewState="False"></asp:HyperLink>
                    </div>
                    <div class="topicNext">
                        <asp:HyperLink ID="nextTopic" runat="server" NavigateUrl='<%# String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&dir=next", Eval("Id")) %>'
                            Text="Next Topic" EnableViewState="False"></asp:HyperLink>
                        &raquo;</div>
                </div>
            </HeaderTemplate>
            <HeaderStyle />
            <FooterTemplate>
            </FooterTemplate>
            <ItemTemplate>
                <input type="hidden" id="TopicSubject" value='<%# Eval("Subject") %>' />
                <topic:TopicTemplate runat="server" ID="topicTemplate" Visible="false" CssClass="topic-content" />
                <topic:PollTemplate runat="server" ID="pollTemplate" Visible="false" />
                <topic:BlogTemplate runat="server" ID="blogTemplate" Visible="false" />
            </ItemTemplate>
        </asp:FormView>
        </div>
        <div id="replyHeader">
            <asp:DropDownList ID="replyFilter" runat="server" OnSelectedIndexChanged="ReplyFilterSelectedIndexChanged"
                AutoPostBack="True">
                <asp:ListItem Text="Oldest Post First" Value="Asc" />
                <asp:ListItem Text="Newest Post First" Value="Desc" />
                <asp:ListItem Text="View Last Post" Value="Last" />
                <asp:ListItem Text="View New Posts" Value="New" />
            </asp:DropDownList>
        </div>
        <asp:Panel ID="MessagePanel" runat="server">
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upd" >
            <ProgressTemplate>
                <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;z-index:5000;"  >
                    <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>             
            <asp:UpdatePanel ID="upd" runat="server" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:Repeater ID="TopicReplies" runat="server" Visible="true" EnableViewState="false" OnItemCreated="BindReply">
                        <ItemTemplate>
                            <asp:PlaceHolder runat="server" ID="PostHolder"></asp:PlaceHolder>
                            
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <asp:PlaceHolder runat="server" ID="PostHolder"></asp:PlaceHolder>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                    <asp:PlaceHolder ID="pager" runat="server"></asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
            <!-- popup -->
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
                <ContentTemplate>
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="ContentFooter" ContentPlaceHolderID="CPF1" runat="server">
    <div class="table-bottom">
        <div class="table-bottom-left">
            <topic:PostButtons ID="pbBottom" runat="server"></topic:PostButtons>
        </div>
        <div class="table-bottom-right">
            <snitz:JumpTo ID="jump" runat="server"></snitz:JumpTo>
        </div>
    </div>
    <div class="table-bottom-footer">
        <asp:PlaceHolder ID="QRPlaceHolder" runat="server" EnableViewState="False"></asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <topic:MinWeblog runat="server" id="MinWeblog" Visible="False" />
</asp:Content>
<asp:Content ID="ForumFooter" ContentPlaceHolderID="CPF2" runat="server"></asp:Content>
