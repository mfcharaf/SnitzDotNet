<%@ Register src="emoticons.ascx" tagname="emoticons" tagprefix="uc2" %>
<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="QuickReply" Codebehind="QuickReply.ascx.cs" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Register TagPrefix="uc1" Src="~/UserControls/popups/BrowseImage.ascx" TagName="imageBrowse" %>
<asp:Literal runat="server" ID="uploadStyle"></asp:Literal>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

    <script type="text/javascript">
        var upResId = '<%= uploadResult.ClientID %>';
        var messageId = '<%= qrMessage.ClientID %>';
        var clientMsgId = '<%= clientSide.ClientID %>';
        var imgTagId = '<%= imageTag.ClientID %>';
        var errLabelId = '<%= errLabel.ClientID %>';
        var updPostId = '<%= updPost.ClientID %>';
        var validFilesStr = '<%= AllowedFileTypes %>';

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
            alert('error');
            $get("errDiv").style.display = 'block';
            $get(errLabelId).innerHTML = "<span style='color:red;'>" + args.get_errorMessage() + "</span>";
            $get(clientMsgId).style.display = 'none';
            $get(updPostId).style.display = 'none';

            return false;
        }

        function uploadStart(sender, args) {
            var validFilesArray = validFilesStr.split(',');
            var filename = args.get_fileName();
            var filext = filename.substring(filename.lastIndexOf(".") + 1);
            var isValidFile = false;

            for (var i = 0; i < validFilesArray.length; i++) {
                if (filext == validFilesArray[i]) {
                    isValidFile = true;
                    break;
                }
            }
            
            if (!isValidFile) {
                //you cannot cancel the upload using set_cancel(true)
                //cause an error
                //will  automatically trigger event OnClientUploadError
                var err = new Error();
                err.name = 'Forum Upload Error';
                err.message = 'Invalid file type';
                throw (err);
            }
            $get("errDiv").style.display = 'none';
            $get(errLabelId).innerHTML = "";
            $get(clientMsgId).style.display = 'block';
            $get(updPostId).style.display = 'block';
            return isValidFile;
        }

        function uploadComplete(sender, args) {
            var contentType = args.get_contentType();
            $get(updPostId).style.display = 'block';
            try {

                if (parseInt(args.get_length()) > parseInt('<%= FileSizeLimit * (1024*1024) %>')) {
                    $get("errDiv").style.display = 'block';
                    $get(errLabelId).innerHTML = "File should be less than <%= FileSizeLimit  %> Mb";
                    $get(clientMsgId).style.display = 'none';
                    $get(updPostId).innerHTML = '';
                    return false;
                }
            } catch (e) { alert(e.Message); }

            var text = args.get_length() + " bytes";
            if (contentType.length > 0) {
                text += ", '" + contentType + "'";
            }
            addToClientTable(args.get_fileName(), text);
            return true;
        }

    </script>
    <input type="hidden" id="caretP" />
    <asp:Panel ID="Panel1" runat="server" CssClass="QRBox clearfix">
        <asp:Panel id="QRH" runat="server" Width="100%" CssClass="QRHeader">
            &nbsp;<asp:Label ID="Label1" runat="server" Text="<%$ Resources:webResources, lblQuickReplyHeader %>"></asp:Label>
        </asp:Panel>
        <div id="qrcontentwrapper" class="clearfix">  

            <asp:Panel id="QRR" runat="server" CssClass="QRRight">
                <asp:TextBox ID="qrMessage" ValidationGroup="qreply" runat="server" TextMode="MultiLine" CssClass="QRMsgArea" ></asp:TextBox>
            </asp:Panel>   
            <asp:Panel id="QRL" runat="server" CssClass="QRLeft smallText">
                <br />
                    <uc2:emoticons ID="emoticons1" runat="server" />
                <br />
                <asp:CheckBox ID="cbxSig" runat="server" Text="<%$ Resources:webResources, lblQRSig %>" Visible="<%# Config.AllowSignatures %>" />
            </asp:Panel>        

        </div>  
    </asp:Panel>
    <asp:Panel id="QRF" runat="server" CssClass="QRFooter clearfix" Width="100%" Direction="<%$ Resources:webResources, TextDirectionWord %>">
        <asp:LinkButton ID="Submit" ValidationGroup="qreply" runat="server" AlternateText="<%$ Resources:webResources, btnSubmit %>" Text="<%$ Resources:webResources, btnSubmit %>" OnClick="btnSubmit_Click" CommandName="submit" style="float:right"/>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="qreply" runat="server" ControlToValidate="qrMessage" ErrorMessage="<%$ Resources:webResources, ErrNoMessage %>" ></asp:RequiredFieldValidator>
    </asp:Panel>

    <!-- Upload popup -->
    <asp:Panel ID="fUpload" runat="server" Style="display: none" EnableViewState="false">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="upheader" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server" EnableViewState="false">
                            <span class="mainModalTitle" id="spanTitle">File Upload</span>
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
                            <div class="demoheading">File Upload</div>
                            Click '<i>Select File</i>' for asynchronous upload.
                            <br />
                            <br />
                            <ajaxtoolkit:AsyncFileUpload OnClientUploadError="uploadError" OnClientUploadComplete="uploadComplete" OnClientUploadStarted="uploadStart"
                                runat="server" ID="AsyncFileUpload1" Width="400px" UploadingBackColor="#CCFFFF"
                                ThrobberID="myThrobber" />
                            &nbsp;<asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="middle" alt="" src="/images/ajax-loader.gif" /></asp:Label>
                            
                            <asp:Label runat="server" Text="&nbsp;" ID="uploadResult" />
                            <br />
                            <asp:Label runat="server" Text="&nbsp;" ID="imageTag" />
                            <br />
                            <div id="errDiv" style="display: none;"><asp:Label ID="errLabel" runat="server" Text=""></asp:Label></div>
                            
                            <table style="border-collapse: collapse; border-left: solid 1px #aaaaff; border-top: solid 1px #aaaaff;"
                                runat="server" cellpadding="3" id="clientSide" ></table>
                        </div>
                        <br />
                        <asp:HyperLink ID="updPost" runat="server" NavigateUrl="#">Insert link into post</asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="fUpload"
        TargetControlID="btnHid" BehaviorID="mpUpload" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="$find('mpUpload').hide();" DropShadow="true" />

    <!-- Browse popup -->
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
                    <div id="Div3" style="white-space: normal;">
                        <uc1:imageBrowse ID="imgGallery" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="browseImage"
        TargetControlID="btnHid" BehaviorID="mpBrowse" BackgroundCssClass="modalBackground"
        CancelControlID="clB2" OnCancelScript="$find('mpBrowse').hide();" DropShadow="true" />

    <asp:Button runat="server" ID="btnHid" Style="display: none;" />



