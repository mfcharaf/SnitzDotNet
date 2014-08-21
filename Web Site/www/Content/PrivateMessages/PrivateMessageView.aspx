<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="True" Culture="auto"
    UICulture="auto" EnableEventValidation="false" CodeBehind="PrivateMessageView.aspx.cs" Inherits="SnitzUI.Content.PrivateMessages.PrivateMessageView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPMeta" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHead" runat="server">
<link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <style type="text/css">
        
        #breadcrumbDIV{ display: none;}
        fieldset{padding:14px;}
        textarea.QRMsgArea{
            border: 1px solid #000;
            width: 97% !important;
            width: 90%;

        }
        .forumtable label{
            display:inline-block;
            width:100px;
            float:left;
        }

        .forumtable input {
            border:solid 1px #aacfe4;
            margin:0px 0 5px 5px;

        }
        .forumtable input[type='image']{ margin: 0px;}
        .forumtable input[type='text']{width:300px;}
        .pmoptions label{width:auto; float:none;}
        .pmoptions input{border:0px;}
        .mainModalContent{margin:0px;padding:4px;}
        .AspNet-DataList table{width:80%;margin:auto;}
    </style>
    <script src="/scripts/editor.min.js" type="text/javascript"></script>
        <script type="text/javascript">
        $(document).ready(function () {
            $(".QRMsgArea").markItUp(miniSettings);

        });
        </script>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="CPHL">
    <style type="text/css">
        body{ line-height: 1em;}
    </style>
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
