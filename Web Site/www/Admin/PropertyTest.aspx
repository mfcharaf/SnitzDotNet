<%@ Page Language="C#" AutoEventWireup="true" StyleSheetTheme="Light" CodeBehind="PropertyTest.aspx.cs" Inherits="SnitzUI.Admin.PropertyTest" %>
<%@ Register TagPrefix="udc" Namespace="SnitzCommon.Controls" Assembly="SnitzCommon" %>
<%@ Register TagPrefix="ajaxtoolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.5.7.1213, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>
<%@ Register Src="~/Admin/UserControls/Registration.ascx" TagPrefix="captcha" TagName="Registration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>
    <style>
        div.label_mandatory, div.label_not_mandatory{ width: auto;display: block;}
         .label_mandatory label, span.label_mandatory {
             display: inline-block;
             margin: 1px 8px 1px 0;
             padding-right:4px;
             border-right: solid 4px #0FA1B8;
             white-space: nowrap;
             width: 150px;
             vertical-align: top;
         }
        .label_not_mandatory label{
            display: inline-block;
            margin: 1px 16px 1px 0;
            white-space: nowrap;
            width: 150px;
            vertical-align: top;
        }
        .label_not_mandatory span.cbxLabel label{ width: auto;}
        .label_mandatory img {
            margin-left: 2px;
            vertical-align: middle;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
            <ajaxtoolkit:ToolkitScriptManager ID="MainSM" runat="server" ScriptMode="Release" CombineScripts="True" LoadScriptsBeforeUI="true" EnablePageMethods="true" EnableScriptGlobalization="true" EnableScriptLocalization="true">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/main.min.js" />
        </Scripts>
    </ajaxtoolkit:ToolkitScriptManager>
    <div style="padding:10px;background-color: white;">
        <div style="width:49%;float:left;">
            <captcha:Registration runat="server" ID="Registration" />
        </div>
        <div style="width:49%;float: right;">
            <asp:PlaceHolder runat="server" ID="mControls"></asp:PlaceHolder>
            <asp:Label ID="mandatory1" runat="server" Text="Items marked" CssClass="label_mandatory"></asp:Label>&nbsp;<asp:Label
                        ID="mandatory2" runat="server" Text="are mandatory"></asp:Label>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="validation" />
        </div>
        <br style="clear: both" />
        
        <br />
        
    </div>
        <asp:Button ID="Button1" runat="server" Text="Submit" />
    </form>
</body>
</html>
