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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="true"
    CodeBehind="Post.aspx.cs" Inherits="SnitzUI.PostPage" validateRequest="false" %>
<%@ Import Namespace="SnitzConfig" %>

<%@ Register TagPrefix="uc1" Src="~/UserControls/popups/BrowseImage.ascx" TagName="imageBrowse" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register src="~/UserControls/emoticons.ascx" tagname="emoticons" tagprefix="uc2" %>
<asp:Content ID="head" ContentPlaceHolderID="CPHead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS" href="" />
    <style type="text/css">
        #postheader{padding: 5px 10px;}
        .rcol{width: auto;margin-left: 140px;padding: 10px;}
        #leftcol{float: left;width: 120px;padding: 10px;}
        #postfooter{clear: both;padding: 5px 10px;text-align: right;height:10px;}
        #postfooter p{margin: 0;}
        * html #postfooter{height: 1px;}
        .mainModalContent{white-space:normal;}
        #emoticons img{border:0px;margin:2px;}
    </style>
    <asp:Literal runat="server" ID="uploadStyle"></asp:Literal>
</asp:Content>
<asp:Content ID="Classifieds" ContentPlaceHolderID="CPAd" runat="server"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">
    <script type="text/javascript" src="/scripts/editor.min.js"></script>
    <script type="text/javascript" src="/Scripts/postpage.min.js"></script>
    <script type="text/javascript">
        var messageId = '<%= Message.ClientID %>';
        var clientMsgId = '<%= clientSide.ClientID %>';
        var imgTagId = '<%= imageTag.ClientID %>';
        var errLabelId = '<%= errLabel.ClientID %>';
        var updPostId = '<%= updPost.ClientID %>';

        function addToClientTable(name, text) {
            var table = $get(clientMsgId);
            var row = table.insertRow(0);
            fillCell(row, 0, name);
            fillCell(row, 1, text);
        }
        function fillCell(row, cellNumber, text) {
            var cell = row.insertCell(cellNumber);
            cell.innerHTML = text;
            cell.style.borderBottom = cell.style.borderRight = "solid 1px #aaaaff";
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
            addToClientTable(args.get_fileName(), text);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHR" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPHL" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPM" runat="server">
    <div id="postForm" class="PostForm">
        <div id="postheader" class="category cattitle">
        <asp:Label ID="postFormTitle" runat="server" Text="<%$ Resources:webResources,ttlPostMessage %>"></asp:Label>
        <input type="hidden" id="caretP" />
        </div>
        <div id="leftcol">
            <div id="spacer" style="height:100px">&nbsp;</div>
                <uc2:emoticons ID="emoticons1" runat="server" />
        </div>
        <div id="main" class="rcol clearfix">
            <asp:Panel ID="ForumDiv" runat="server">
                <asp:Label ID="lblForum" runat="server" AssociatedControlID="ForumDropDown" Width="80px"
                    Text="<%$ Resources:webResources, lblForum %>"></asp:Label>
                <asp:DropDownList ID="ForumDropDown" runat="server" DataTextField="Value" DataValueField="Key"
                    EnableViewState="False">
                </asp:DropDownList>
            </asp:Panel>
            <asp:Panel ID="SubjectDiv" runat="server">
                <asp:Label ID="lblSubject" runat="server" AssociatedControlID="tbxSubject" Width="80px"
                    Text="<%$ Resources:webResources, lblSubject %>"></asp:Label>
                <asp:TextBox ID="tbxSubject" runat="server" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvSubject" ControlToValidate="tbxSubject" runat="server"
                    ErrorMessage="<%$ Resources:webResources, ErrNoSubject %>">*</asp:RequiredFieldValidator>
            </asp:Panel>
            <asp:TextBox ID="Message" runat="server" CssClass="QRMsgArea" Wrap="true" TextMode="MultiLine"></asp:TextBox>
            <asp:Panel ID="cbxPnl" runat="server" style="width:69%;float:left;">
            <asp:CheckBox ID="cbxSig" CssClass="cbx" runat="server" Text="<%$ Resources:webResources, lblIncludeSig %>" Visible="<%# Config.AllowSignatures %>"/><br />
            <asp:CheckBox ID="cbxLock" CssClass="cbx" runat="server" Text="<%$ Resources:webResources, lblPostLockTopic %>"
                Visible="<%# IsAdministrator || IsModerator %>" /><br />
            <asp:CheckBox ID="cbxSticky" TextAlign="Right" CssClass="cbx" runat="server" Text="<%$ Resources:webResources, lblPostMakeSticky %>"
                Visible="<%# IsAdministrator %>" /><br />
            </asp:Panel>
            <asp:Panel ID="btnPnl" runat="server" style="width:30%;float:left;text-align:right;">
                <asp:LinkButton ID="btnSubmit" runat="server" AlternateText="<%$ Resources:webResources, btnSubmit %>"
                    Text="<%$ Resources:webResources, btnSubmit %>" OnClick="PostMessage" />
                <asp:LinkButton ID="btnCancel" runat="server" AlternateText="<%$ Resources:webResources, btnCancel %>"
                    Text="<%$ Resources:webResources, btnCancel %>" OnClick="BtnCancelClick"
                    CausesValidation="false" />
            
            </asp:Panel>
        </div>
        <div id="postfooter">
            <asp:ImageButton ID="btnAttach" runat="server" SkinID="attachB" CausesValidation="False"
                AlternateText="<%$ Resources:UploadMod, lblAttach %>" ToolTip="<%$ Resources:UploadMod, lblAttachTip %>"
                OnClientClick="javascript:openSmallWindow('pop_upload.aspx?type=attach');" Visible="False" />
        </div>
    </div>
    <br class="clearfix" />
    <!-- Upload file popup -->
    <asp:Panel ID="fUpload" runat="server" Style="display: none; clear:both;" EnableViewState="false">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="upheader" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server" EnableViewState="false">
                            <span class="mainModalTitle" id="spanTitle">Image Upload</span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false"
                            EnableViewState="false" />
                    </div>
                </div>
                <div class="mainModalContent">
                    <div id="mainModalContents">
                        <div class="demoarea">
                            <div class="demoheading">
                                Image Upload</div>
                            Click '<i>Select File</i>' for asynchronous upload.
                            <br />
                            <br />
                            <ajaxtoolkit:AsyncFileUpload OnClientUploadError="uploadError" OnClientUploadComplete="uploadComplete"
                                runat="server" ID="AsyncFileUpload1" Width="400px" UploadingBackColor="#CCFFFF"
                                ThrobberID="myThrobber" />
                            &nbsp;<asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="middle" alt="" src="/images/ajax-loader.gif" /></asp:Label>
                            <div>
                            <strong>The latest Server-side event:</strong></div>
                            <asp:Label runat="server" Text="&nbsp;" ID="uploadResult" />
                            <br />
                            <asp:Label runat="server" Text="&nbsp;" ID="imageTag" />
                            <br />
                            <div id="errDiv" style="display: none;">
                                <asp:Label ID="errLabel" runat="server" Text="Label"></asp:Label></div>
                            <div>
                                <strong>Client-side events:</strong></div>
                            <table style="border-collapse: collapse; border-left: solid 1px #aaaaff; border-top: solid 1px #aaaaff;"
                                runat="server" cellpadding="3" id="clientSide" />
                        </div>
                        <br />
                        <asp:HyperLink ID="updPost" runat="server" NavigateUrl="#">Insert link into post</asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="fUpload" Drag="true" PopupDragHandleControlID="MPD"
        TargetControlID="btnHid" BehaviorID="mpUpload" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="$find('mpUpload').hide();" DropShadow="true" />

    <!-- Browse image popup -->
    <asp:Panel ID="browseImage" runat="server" Style="display: none;max-width:600px;" EnableViewState="false">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="brwsheader" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD2" runat="server" EnableViewState="false">
                            <span class="mainModalTitle" id="spanTitle1">Image Browser (click on image to insert into post)</span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB2" CausesValidation="false" EnableViewState="false" />
                    </div>
                </div>
                <div class="mainModalContent">
                    <div id="Div3">
                        <uc1:imageBrowse ID="imgGallery" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="browseImage"
        Drag="true" PopupDragHandleControlID="MPD"
        TargetControlID="btnHid" BehaviorID="mpBrowse" BackgroundCssClass="modalBackground"
        CancelControlID="clB2" OnCancelScript="$find('mpBrowse').hide();" DropShadow="true" />

    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
