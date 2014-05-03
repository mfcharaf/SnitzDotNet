<%-- 
##############################################################################################################
## Snitz Forums .net
##############################################################################################################
## Copyright (C) 2012-2014 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##############################################################################################################
--%>
<%@ Page Title="" validateRequest="false" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="SnitzUI.Content.FAQ.Help" %>
<%@ Reference Control = "~/UserControls/SideColumn.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPhead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="faqCSS"/>
    <link rel="stylesheet" type="text/css" href="/Scripts/prettify.css"/>
    <script src="/scripts/editor.min.js" type="text/javascript"></script>
    <script src="/scripts/run_prettify.js"></script>
    <script src="/scripts/confirmdialog.js" type="text/javascript"></script>
        <script type="text/javascript">
            $(document).ready(function () {
                $(".QRMsgArea").markItUp(mySettings);

            });

        </script>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="CPHL" runat="server">
    <style type="text/css">
        .codebox { max-height: 2000px;}
    </style>
    <div class="POTButtons clearfix">
        <asp:LinkButton ID="addTopic" Text="Add help Topic" runat="server" ToolTip="Add new help Topic" Visible='<%# IsAdministrator || Roles.IsUserInRole("FaqAdmin") %>' OnClick="NewTopic" EnableViewState="False" />
        <asp:LinkButton EnableViewState="False" ID="manageCats" OnClick="ManageCategories" runat="server" Text="Manage Categories" ToolTip="Manage Categories" Visible='<%# IsAdministrator || Roles.IsUserInRole("FaqAdmin") %>' />
    </div>
</asp:Content>

<asp:Content ID="Content8" ContentPlaceHolderID="CPM" runat="server">
    <asp:MultiView ID="FaqViews" runat="server" ActiveViewIndex="0">
        <asp:View runat="server" ID="ViewFaq">
            <div id="wrap" class="forumtable">
                <div class="category clearfix">
                    <div class="table-bottom-left"><asp:Label ID="Label2" runat="server" Text="Snitz Forum help topics"></asp:Label></div>
                    <div class="table-bottom-right">
                        <asp:ImageButton ID="btnSearch" runat="server" SkinID="Search"
                            OnClick="SearchFaq" CssClass="floatright" EnableViewState="False" /><asp:TextBox ID="searchFor" runat="server"
                                Width="175px" CssClass="floatright"></asp:TextBox>
                    </div>
                </div>
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
                        <asp:Panel ID="pnlRead" runat="server" Visible="true" EnableViewState="False">
                            <h3>
                                <asp:ImageButton ID="btnDeleteFaq" SkinID="DeleteMessage" runat="server" OnClientClick=""
                                    Visible="false" EnableViewState="False" />
                                <asp:ImageButton ID="btnEdit" SkinID="Edit" runat="server" OnClick="ToggleEdit" Visible="false" EnableViewState="False" />
                                &nbsp;<asp:Label ID="faqQuestion" runat="server" Text="" EnableViewState="False"></asp:Label></h3>
                            <p>
                                <asp:Literal ID="faqAnswer" runat="server" Text="Here you can find answers to questions about how the forum works. Use the links on the left or the search box to find what you are looking for. " EnableViewState="False"></asp:Literal>
                            </p>
                        </asp:Panel>

                    </div>
                </div>
            </div>
        </asp:View>
        <asp:View runat="server" ID="EditFaq">
            <asp:HiddenField ID="hdnEditFaq" runat="server" />
            <asp:Panel ID="pnlCategory" runat="server"  EnableViewState="False">
                <asp:Label ID="lblCat" runat="server" Text="Category" AssociatedControlID="ddlCategory" EnableViewState="False"></asp:Label><asp:DropDownList
                    ID="ddlCategory" runat="server" DataValueField="Id" DataTextField="Description" EnableViewState="False">
                </asp:DropDownList>
            </asp:Panel>
            <asp:Label ID="Label1" runat="server" Text="Question" AssociatedControlID="tbxQuestion" EnableViewState="False"></asp:Label><asp:TextBox
                ID="tbxQuestion" runat="server" Width="60%"></asp:TextBox><br />
            <asp:TextBox ID="tbxAnswer" runat="server" TextMode="MultiLine" Rows="12" CssClass="QRMsgArea" EnableViewState="False"></asp:TextBox>
            <asp:LinkButton ID="LinkButton2" runat="server" OnClick="AddNewQuestion">Submit</asp:LinkButton> 
            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="CancelEdit">Cancel</asp:LinkButton>            
        </asp:View>
        <asp:View runat="server" ID="EditCategory">
            <asp:Panel runat="server" ID="Category" CssClass="faqCat" EnableViewState="False" GroupingText="Manage Categories">
                <asp:Panel ID="Panel1" runat="server" EnableViewState="False" >
                    <asp:Label ID="Label3" runat="server" Text="Category" AssociatedControlID="ddlCategoryEdit" EnableViewState="False"></asp:Label>
                    <asp:DropDownList ID="ddlCategoryEdit" AutoPostBack="true" OnSelectedIndexChanged="SelectCategory" runat="server" DataValueField="Id" DataTextField="Description" EnableViewState="False">
                    </asp:DropDownList>
                </asp:Panel>
                <asp:Label ID="Label4" runat="server" Text="Name" AssociatedControlID="catDescription" EnableViewState="False"></asp:Label><asp:TextBox
                    ID="catDescription" runat="server" Width="60%"></asp:TextBox><br />
                <asp:Label ID="Label6" runat="server" Text="Language code" AssociatedControlID="catLang" EnableViewState="False"></asp:Label><asp:TextBox
                    ID="catLang" runat="server" Width="30px" ReadOnly="True"></asp:TextBox><br />
                <asp:Label ID="Label5" runat="server" Text="Order" AssociatedControlID="catOrder" EnableViewState="False"></asp:Label><asp:TextBox
                    ID="catOrder" runat="server" Width="20px"></asp:TextBox><br /><br />
                <asp:LinkButton ID="btnAddCat" SkinID="Edit" runat="server" OnClick="AddNewCategory" EnableViewState="False" />
                <asp:LinkButton ID="btnCancel" runat="server" OnClick="CancelEdit">Cancel</asp:LinkButton>                            
            </asp:Panel>            
        </asp:View>
    </asp:MultiView>
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:sidebar runat="server" ID="sidebar1" Show="Poll,Events,Ads,Active,Rss"/>
</asp:Content>

