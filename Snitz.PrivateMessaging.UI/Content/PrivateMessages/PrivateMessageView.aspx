<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="True" Culture="auto"
    UICulture="auto" EnableEventValidation="false" CodeBehind="PrivateMessageView.aspx.cs" Inherits="PrivateMessaging.PrivateMessageView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPMeta" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHead" runat="server">
<link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <style type="text/css">

    fieldset{padding:14px;background-color:white;}
    textarea.QRMsgArea{
	border: 1px solid #000;
	width: 97% !important;
	width: 90%;

}
label{
 display:inline-block;
 width:100px;
 float:left;
 }

  input {
  border:solid 1px #aacfe4;
 margin:0px 0 5px 5px;

 }
 input[type='text']{width:300px;}
  .pmoptions label{width:auto; float:none;}
  .pmoptions input{border:0px;}
  .youtube, .browse, .upload{display:none;}
  .mainModalContent{margin:0px;padding:4px;}
  .AspNet-DataList table{width:80%;margin:auto;}
    </style>
    <script src="/scripts/editor.min.js" type="text/javascript"></script>
        <script type="text/javascript">
        $(document).ready(function () {
            $(".QRMsgArea").markItUp(mySettings);

        });
        </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPAd" runat="server">
<%-- If you want to display the ad rotator, remove this content tag --%>

</asp:Content>

<asp:Content ID="Content7" ContentPlaceHolderID="CPM" runat="server">
<div class="PMContainer">
    <asp:PlaceHolder runat="server" ID="ViewPm"></asp:PlaceHolder>
</div>
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>
<asp:Content ID="Content9" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
