<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SnitzUI.ResourceEditor.ResEditor" ValidateRequest="false" Culture="auto" UICulture="auto" %>

<%@ Register TagName="ResXEditor" TagPrefix="resxEditor" Src="~/ResourceEditor/ResXEditor.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPMeta" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHead" runat="server">
<style type="text/css">
#contentDIV{background-color:White; border:1px solid gray;}
</style>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPAd" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPH1" runat="server">
<script type="text/javascript">
    Sys.Application.add_load(function () {
        var form = Sys.WebForms.PageRequestManager.getInstance()._form;
        form._initialAction = form.action = window.location.href;
    }); 

</script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPHR" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="CPHL" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="CPM" runat="server">
    <div class="innertube">
        <asp:UpdatePanel ID="UpdatePanel" runat="server" ChildrenAsTriggers="true" >
            <ContentTemplate>
               <resxEditor:ResXEditor ID="editor" runat="server" /> 
            </ContentTemplate>
        </asp:UpdatePanel>
        
        <asp:UpdateProgress ID="UpdateProgress" runat="server" DisplayAfter="300" AssociatedUpdatePanelID="UpdatePanel">
            <ProgressTemplate>
                <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;"  >
                    <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
    </div>
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>
<asp:Content ID="Content9" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
<asp:Content ID="Content10" runat="server" contentplaceholderid="CPSpace">

</asp:Content>

