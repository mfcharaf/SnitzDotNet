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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master"
    AutoEventWireup="true" Culture="auto" UICulture="auto" CodeBehind="Topic.aspx.cs" MaintainScrollPositionOnPostback="true"
    Inherits="SnitzUI.TopicPage" %>
<%@ Import Namespace="Snitz.BLL" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Import Namespace="Snitz.Entities" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Reference Control="~/UserControls/GridPager.ascx" %>
<%@ Reference Control="~/UserControls/Polls/Poll.ascx" %>
<%@ Register TagPrefix="topic" Src="~/UserControls/MessageButtonBar.ascx" TagName="MessageButtonBar" %>
<%@ Register TagPrefix="topic" Src="~/UserControls/MessageProfile.ascx" TagName="MessageProfile" %>
<%@ Register TagPrefix="topic" Src="~/UserControls/PagePostButtons.ascx" TagName="PostButtons" %>

<asp:Content runat="server" ID="metatag" ContentPlaceHolderID="CPMeta">
    <asp:Literal ID="metadescription" runat="server"></asp:Literal>
</asp:Content>
<asp:Content runat="server" ID="scripthead" ContentPlaceHolderID="CPHead">
    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js"></script>
    <script type="text/javascript" src="/scripts/jquery.youtube.player.min.js"></script>
    <script src="/scripts/editor.min.js" type="text/javascript"></script>
    <script src="/scripts/bbcode.min.js" type="text/javascript"></script>
    <script src="/scripts/smilies.min.js" type="text/javascript"></script>
    <script src="/scripts/postpage.min.js" type="text/javascript"></script>
    <script src="/Scripts/topic_print.min.js" type="text/javascript"></script>
    <script src="/scripts/confirmdialog.js" type="text/javascript"></script>
    <script type="text/javascript">
        function pagebind() {
            $(document).ready(function () {
                $(".bbcode").each(function () {
                    $(this).html(parseBBCode(parseEmoticon($(this).text(), '<%= Page.Theme %>')));
                });
                $('#ctl00_CPF1_ctl00_emoticons1_DataList1 a').click(function () {
                    //var emoticon = $(this).attr("title");
                    //$.markItUp({ replaceWith: emoticon });
                    return false;
                });
                $('a.video').player({
                    width: 380,
                    chromeless: 0,
                    showTime: 0,
                    showPlaylist: 0,
                    showTitleOverlay: 0
                });

            });
            
        };

        pagebind();
        function pollloaded() { alert("poll loaded");
            pagebind();
        }
        
        $.fn.serializeNoViewState = function () {
            return this.find("input,select,hidden,textarea")
                .not("[type=hidden][name^=__]")
                .serialize();
        };
        function SendForm() {
            var name = $get("ToName").value;
            var email = $get("ToEmail").value;
            var message = $get("MessageTextBox").value;
            var subject = $get("SubjectTextBox").value;
            PageMethods.SendEmail(name, email, message, subject, OnSucceeded, OnFailed);
        }
        function SplitTopic() {
            PageMethods.SplitTopic($("form").serializeNoViewState(), OnSucceeded, OnFailed);
        }
        function ApprovePost(topicid, replyid) {
            PageMethods.Approval(topicid, replyid);
            var millisecondsToWait = 500;
            setTimeout(function () {
                location.reload();
            }, millisecondsToWait);
            
        }
        function OnHold(topicid, replyid) {
            PageMethods.PutOnHold(topicid, replyid);
            var millisecondsToWait = 500;
            setTimeout(function () {
                __doPostBack("", "");
            }, millisecondsToWait);
        }
        function uploadError(sender, args) {
            addToClientTable(args.get_fileName(), "<span style='color:red;'>" + args.get_errorMessage() + "</span>");
            return false;
        }
        function uploadComplete(sender, args) {
            var contentType = args.get_contentType();

            try {
                var fileExtension = args.get_fileName();
                if (fileExtension.indexOf('.pdf') != -1 || contentType.indexOf('image') < 0) {
                    $get("errDiv").style.display = 'block';
                    $get(errLabelId).innerHTML = "File type not permitted";
                    $get(clientMsgId).style.display = 'none';
                    return false;
                }
            } catch (e) { alert(e.Message); }
            try {

                if (parseInt(args.get_length()) > 2000000) {
                    $get("errDiv").style.display = 'block';
                    $get(errLabelId).innerHTML = "File should be less than 2Mb";
                    $get(clientMsgId).style.display = 'none';
                    return false;
                }
            } catch (e) { alert(e.Message); }

            var text = args.get_length() + " bytes";
            if (contentType.length > 0) {
                text += ", '" + contentType + "'";
            }
            window.addToClientTable(args.get_fileName(), text);

        }

        function OnSucceeded(results, userContext, methodName) {
            alert(results);
            mainScreen.CancelModal();
            if (methodName == "SplitTopic" || methodName == "CastVote")
                location.reload();
            else
                return false;
        }
        function OnFailed(error, userContext, methodName) {
            // Alert user to the error.
            alert(error.get_message());
            mainScreen.CancelModal();
            return false;
        }

    </script>
    <style type="text/css">
        img.avatar
        {
            width: 80px;
            height: 80px;
            opacity: 0.4;

        }
        img.online
        {
            width: 80px;
            height: 80px;
            opacity: 1;

        }
        .topicSplit label, .topicSplit input
        {
            font-size: 0.9em;
            display: inline-block;
            width: 120px;
            margin-bottom: 10px;
        }
        .topicSplit input[type="text"]
        {
            width: 400px;
        }
        .topicSplit input, .topicSplit select
        {
            border: 1px solid Silver;
        }
        .topicSplit select
        {
            max-width: 200px;
        }
        .topicSplit label
        {
            text-align: right;
            padding-right: 20px;
        }
        .forumtable
        {
            border-color: #0FA1B8;
        }
        .markItUpEditor
        {
            min-height: 140px !important;
        }
        #emoticons img{border:0px;margin:2px;}
        .AspNet-FormView-Data{ padding: 0px;}
    </style>
</asp:Content>
<asp:Content ID="ContentMain" ContentPlaceHolderID="CPM" runat="server">
    <div class="POFTop clearfix">
        <topic:PostButtons ID="pbTop" runat="server"></topic:PostButtons>
    </div>
    <div id="MessageList" style="width: 100%;">
        <asp:FormView ID="TopicView" runat="server" Width="100%" CellPadding="0" EnableViewState="False"
            OnItemCommand="TopicViewItemCommand" OnDataBound="TopicBound" style="padding: 0px;">
            <HeaderTemplate>
                <div class="topicHeader clearfix">
                    <div class="topicPrev">
                        &laquo;
                        <asp:HyperLink ID="prevTopic" runat="server" NavigateUrl='<%# String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&dir=prev", Eval("Id")) %>'
                            Text="Prev Topic"></asp:HyperLink>
                    </div>
                    <div class="topicNext">
                        <asp:HyperLink ID="nextTopic" runat="server" NavigateUrl='<%# String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&dir=next", Eval("Id")) %>'
                            Text="Next Topic"></asp:HyperLink>
                        &raquo;</div>
                </div>
            </HeaderTemplate>
            <HeaderStyle />
            <FooterTemplate>
            </FooterTemplate>
            <ItemTemplate>
                <input type="hidden" id="TopicSubject" value='<%# Eval("Subject") %>' />
                <div class="TopicDiv clearfix">
                    <div class="leftColumn">
                        <a href='<%# Eval("Author.ProfileLink") %>' title='<%# Eval("Author.Username") %>'><%# Eval("Author.Username") %></a>
                        <topic:MessageProfile runat="server" ID="TopicAuthor" AuthorId='<%# DataBinder.Eval(Container.DataItem, "AuthorId")%>' />
                    </div>
                    <div class="MessageDIV">
                        <div class="buttonbar">
                            <topic:MessageButtonBar ID="bbT" runat="server" Post='<%# Container.DataItem %>' DeleteClick="MessageDeleted" />
                        </div>
                        <div id="msgContent" class="mContent bbcode" runat="server">
                            <asp:PlaceHolder ID="msgPH" runat="server"></asp:PlaceHolder>
                        </div>
                        <br />
                        <div id="editbyDiv" runat="server" class="editedDIV" visible='<%# Eval("LastEditedById") != null && Config.ShowEditBy %>'>
                            <asp:Label ID="Label2" runat="server" Text='<%# String.Format("Edited by {0} - ", Eval("EditorName")) %>'></asp:Label>
                            <asp:Literal ID="litDate1" runat="server" Text='<%# Topics.LastEditTimeAgo(Container.DataItem)%>' />
                        </div>
                        <div id="r1" runat="server" class="sigDIV bbcode" visible="<%# ShowSig(Container.DataItem) %>">
                            <%# DataBinder.Eval(Container.DataItem, "AuthorSignature")%>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:FormView>
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
            
            <asp:UpdatePanel ID="upd" runat="server" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <script type="text/javascript">
                        var prm = Sys.WebForms.PageRequestManager.getInstance();
                        var confirmHandlers = {};

                        prm.add_endRequest(

                            function () {

                            jQuery("abbr.timeago").timeago();

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

                    </script>
                    <asp:Repeater ID="TopicReplies" runat="server" Visible="true" EnableViewState="false"
                        OnItemDataBound="RepliesBound">
                        <ItemTemplate>
                            <div class="ReplyDiv clearfix">
                                <div class="leftColumn">
                                    <asp:HiddenField ID="hdnAuthor" runat="server" />
                                    <asp:Literal ID="popuplink" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AuthorPopup")%>'></asp:Literal>
                                    <topic:MessageProfile runat="server" ID="ReplyAuthor" AuthorId='<%# DataBinder.Eval(Container.DataItem, "AuthorId")%>' />
                                </div>
                                <div class="MessageDIV">
                                    <div class="buttonbar">
                                        <topic:MessageButtonBar ID="bbR" runat="server" Post='<%# Container.DataItem %>' />
                                    </div>
                                    <div class="mContent bbcode">
                                        <asp:Literal ID="msgBody" runat="server" Text='<%# Eval("Message").ToString().ReplaceNoParseTags().ParseVideoTags().ParseWebUrls() %>' Mode="Encode"></asp:Literal>
                                    </div>
                                    <br />
                                    <div id="editbyDiv" runat="server" class="editedDIV" visible='<%# Eval("LastEditedById") != null && Config.ShowEditBy %>'>
                                        <asp:Label ID="Label2" runat="server" Text='<%# String.Format("Edited by {0} - ", Eval("EditorName")) %>'></asp:Label>
                                        <asp:Literal ID="litDate1" runat="server" Text='<%# Replies.LastEditTimeAgo(Container.DataItem)%>' />
                                    </div>
                                    <div id="r1" runat="server" class="sigDIV bbcode" visible="<%# ShowSig(Container.DataItem) %>">
                                        <%# DataBinder.Eval(Container.DataItem, "AuthorSignature")%>
                                    </div>
                                </div>
                            </div>
                            <br class="clearfix" style="height: 2px;" />
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="AltReplyDiv clearfix">
                                <div class="leftColumn">
                                    <asp:HiddenField ID="hdnAuthor" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "AuthorName")%>' />
                                    <asp:Literal ID="popuplink" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AuthorPopup")%>'></asp:Literal>
                                    <topic:MessageProfile runat="server" ID="ReplyAuthor" AuthorId='<%# DataBinder.Eval(Container.DataItem, "AuthorId")%>' />
                                </div>
                                <div class="MessageDIV">
                                    <div class="buttonbar">
                                        <asp:HyperLink ID="hypGoUp" rel="nofollow" SkinID="GotoTop" runat="server" EnableViewState="False"
                                            NavigateUrl="#top" style="margin-left:5px;"></asp:HyperLink>
                                        <topic:MessageButtonBar ID="bbR" runat="server" Post='<%# Container.DataItem %>' />
                                    </div>
                                    <div class="mContent bbcode">
                                        <asp:Literal ID="msgBody" runat="server" Text='<%# Eval("Message").ToString().ReplaceNoParseTags().ParseVideoTags().ParseWebUrls() %>' Mode="Encode"></asp:Literal>
                                    </div>
                                    <br />
                                    <div id="editbyDiv" runat="server" class="editedDIV" visible='<%# Eval("LastEditedById") != null && Config.ShowEditBy %>'>
                                        <asp:Label ID="Label2" runat="server" Text='<%# String.Format("Edited by {0} - ", Eval("EditorName")) %>'></asp:Label>
                                        <asp:Literal ID="litDate1" runat="server" Text='<%# Replies.LastEditTimeAgo(Container.DataItem)%>' />
                                    </div>
                                    <div id="r1" runat="server" class="sigDIV bbcode" visible="<%# ShowSig(Container.DataItem) %>">
                                        <%# DataBinder.Eval(Container.DataItem, "AuthorSignature")%>
                                    </div>
                                </div>
                            </div>
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
<asp:Content ID="ForumFooter" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
