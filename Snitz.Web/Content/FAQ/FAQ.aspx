
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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="true" Culture="auto"
    UICulture="auto" CodeBehind="FAQ.aspx.cs" Inherits="SnitzUI.FAQPage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Reference Control = "~/UserControls/SideColumn.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="markitupCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="faqCSS"/>
    <script src="/scripts/editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        function SetSource(sourceId) {
            var hidSourceId = $("#<%=customPostBack.ClientID%>");
            hidSourceId.val(sourceId);
        }
    </script>

</asp:Content>
<asp:Content ID="newfaq" ContentPlaceHolderID="CPHL" runat="server">
    <asp:HiddenField ID="customPostBack" runat="server" Value="" />
    <div class="POTButtons clearfix">
        <asp:LinkButton ID="addTopic" Text="Add help Topic" runat="server" ToolTip="Add new help Topic" Visible='<%# IsAdministrator || Roles.IsUserInRole("FaqAdmin") %>' OnClientClick = "SetSource(this.id)"  OnClick="NewTopic" EnableViewState="False" />
        <asp:LinkButton EnableViewState="False" ID="manageCats" OnClick="ManageCategories" runat="server" Text="Manage Categories" ToolTip="Manage Categories" Visible='<%# IsAdministrator || Roles.IsUserInRole("FaqAdmin") %>' />
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPM" runat="server">
    <div id="wrap" class="forumtable">
        <div class="category clearfix">
            <div class="table-bottom-left"><asp:Label ID="Label2" runat="server" Text="Snitz Forum help topics"></asp:Label></div>
            <div class="table-bottom-right">
                <asp:ImageButton ID="btnSearch" runat="server" SkinID="Search"
                    OnClick="SearchFaq" CssClass="floatright" EnableViewState="False" /><asp:TextBox ID="searchFor" runat="server"
                        Width="175px" CssClass="floatright"></asp:TextBox>
            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <script type="text/javascript">
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(function () {
                        $("#ctl00_CPM_tbxAnswer").markItUp(mySettings);
                    }); 

                </script>
                <div id="body">
                    <div id="sidebar">
                        <asp:Repeater ID="FaqNav" runat="server" OnItemDataBound="BindQuestions">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="tableheader">
                                    <asp:Label ID="lnkCat" runat="server" Text='<%# Eval("Description") %>' EnableViewState="False" />
                                    <asp:HiddenField ID="hdnCatId" runat="server" Value='<%# Eval("Id") %>' />
                                    <br />
                                </div>
                                <div class="faqQuestion">
                                    <asp:Repeater ID="FaqQuestions" runat="server" OnItemCommand="ViewAnswer">
                                        <ItemTemplate>
                                            <asp:LinkButton CssClass="faqLnk" ID="lnkQuestion" runat="server" CommandArgument='<%# Eval("Key") %>' Text='<%# Eval("Value") %>' EnableViewState="False"></asp:LinkButton>
                                            <br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div id="main">
                        <asp:HiddenField ID="faqId" runat="server" />
                        <asp:Panel runat="server" ID="Category" Visible="False" EnableViewState="False">
                            <asp:Panel ID="Panel1" runat="server" EnableViewState="False">
                                <asp:Label ID="Label3" runat="server" Text="Category" AssociatedControlID="DropDownList1" EnableViewState="False"></asp:Label>
                                <asp:DropDownList ID="DropDownList1" runat="server" DataValueField="Id" DataTextField="Description" EnableViewState="False">
                                </asp:DropDownList>
                            </asp:Panel>
                            <asp:Label ID="Label4" runat="server" Text="Name" AssociatedControlID="TextBox1" EnableViewState="False"></asp:Label><asp:TextBox
                                ID="TextBox1" runat="server" Width="60%"></asp:TextBox><br />
                            
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
                            <asp:LinkButton ID="LinkButton2" runat="server">Cancel</asp:LinkButton>                            
                        </asp:Panel>
                        <asp:Panel ID="pnlRead" runat="server" Visible="true" EnableViewState="False">
                            <h3>
                                <asp:ImageButton ID="btnDelete" SkinID="DeleteMessage" runat="server" OnClick="DeleteFaq"
                                    Visible="false" OnClientClick="mainScreen.ShowConfirm(this, 'Confirm Delete', 'Do you want to make topic sticky?');
                                    mainScreen.LoadServerControlHtml(' Confirm Action',{'pageID':3,'data': 'Delete help topic ?'},'confirmHandlers.BeginRecieve');
                                    return false;" EnableViewState="False" />
                                <asp:ImageButton ID="btnEdit" SkinID="Edit" runat="server" OnClick="ToggleEdit" Visible="false" OnClientClick = "SetSource(this.id)" EnableViewState="False" />
                                &nbsp;<asp:Label ID="faqQuestion" runat="server" Text="" EnableViewState="False"></asp:Label></h3>
                            <p>
                                <asp:Literal ID="faqAnswer" runat="server" Text="Here you can find answers to questions about how the forum works. Use the links on the left or the search box to find what you are looking for. " EnableViewState="False"></asp:Literal>
                            </p>
                        </asp:Panel>
                        <asp:Panel ID="pnlEdit" runat="server" Visible="false" EnableViewState="False">
                            <asp:Panel ID="pnlCategory" runat="server"  Visible="false" EnableViewState="False">
                                <asp:Label ID="lblCat" runat="server" Text="Category" AssociatedControlID="ddlCategory" EnableViewState="False"></asp:Label><asp:DropDownList
                                    ID="ddlCategory" runat="server" DataValueField="Id" DataTextField="Description" EnableViewState="False">
                                </asp:DropDownList>
                            </asp:Panel>
                            <asp:Label ID="Label1" runat="server" Text="Question" AssociatedControlID="tbxQuestion" EnableViewState="False"></asp:Label><asp:TextBox
                                ID="tbxQuestion" runat="server" Width="60%"></asp:TextBox><br />
                            <asp:TextBox ID="tbxAnswer" runat="server" TextMode="MultiLine" Rows="12" CssClass="QRMsgArea" EnableViewState="False"></asp:TextBox>
                            <asp:PlaceHolder ID="phSubmit" runat="server"></asp:PlaceHolder>
                            <asp:LinkButton ID="LinkButton1" runat="server">Cancel</asp:LinkButton>
                        </asp:Panel>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:sidebar runat="server" ID="sidebar1" Show="Poll,Events,Ads,Active,Rss"/>
</asp:Content>