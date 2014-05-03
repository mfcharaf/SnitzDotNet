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
    UICulture="auto"
    CodeBehind="Search.aspx.cs" Inherits="SnitzUI.Search" %>

<%@ Import Namespace="SnitzCommon" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Reference Control="~/UserControls/GridPager.ascx" %>
<asp:Content ID="head" ContentPlaceHolderID="CPHead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>

</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPM" runat="server">
    <div id="searchPanel" class="forumtable" style="padding:8px;" >
    <asp:UpdatePanel ID="updSearch" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
        
            <asp:Panel ID="pnlSearch" CssClass="topicTable" runat="server" 
                GroupingText="<%$ Resources:webResources, lblSearchForums %>" 
                EnableViewState="False">
                <asp:Label ID="Label2" runat="server" AssociatedControlID="ddlForum" 
                    Text="<%$ Resources:webResources, lblSearchIn %>" EnableViewState="False"></asp:Label>
                <asp:DropDownList ID="ddlForum" runat="server" EnableViewState="False">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please select a forum"
                    ControlToValidate="ddlForum" InitialValue="-1" ValidationGroup="Search">*</asp:RequiredFieldValidator>
                <br />
                <asp:CheckBox ID="cbxSubjectOnly" runat="server" 
                    Text="Subject Only" EnableViewState="True" Checked="True"/>
                <br />
                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:webResources, lblSearchKeywords %>"
                    AssociatedControlID="searchFor" EnableViewState="False"></asp:Label>
                <asp:TextBox ID="searchFor" runat="server" EnableViewState="False"></asp:TextBox><asp:RequiredFieldValidator
                    ID="RequiredFieldValidator2" runat="server" ErrorMessage="You must supply a search term"
                    ControlToValidate="searchFor" ValidationGroup="Search">*</asp:RequiredFieldValidator>
                <asp:DropDownList ID="ddlMatch" runat="server" EnableViewState="False">
                    <asp:ListItem Text="Exact Phrase" Value="exact" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="All words" Value="all"></asp:ListItem>
                    <asp:ListItem Text="Any word" Value="any"></asp:ListItem>
                </asp:DropDownList><br />
                <asp:LinkButton ID="lnkAdvanced" runat="server" CausesValidation="False" 
                    OnClick="LinkButton1Click" EnableViewState="False">advanced search ...</asp:LinkButton>
                <br />
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                    ValidationGroup="Search" EnableViewState="False" />
            </asp:Panel>
            <asp:Panel ID="extendedSearch" runat="server" GroupingText="Advanced Search" CssClass="topicTable"
                Visible="False" EnableViewState="False">
                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:webResources, lblSearchUser %>"
                    AssociatedControlID="tbxUserName" EnableViewState="False"></asp:Label>
                <asp:TextBox ID="tbxUserName" runat="server" EnableViewState="False"></asp:TextBox>
                <asp:DropDownList ID="ddlUserPosts" runat="server" EnableViewState="False">
                    <asp:ListItem Value="any" Text="Any post by user"></asp:ListItem>
                    <asp:ListItem Value="topic" Text="Topics started by user" Selected="True"></asp:ListItem>
                </asp:DropDownList>
                <br />
                <asp:Label ID="Label4" runat="server" Text="<%$ Resources:webResources, lblSearchDate %>"
                    AssociatedControlID="tbxDate" EnableViewState="False"></asp:Label>
                <asp:TextBox ID="tbxDate" runat="server" EnableViewState="False"></asp:TextBox>
                <asp:DropDownList ID="ddlSince" runat="server" EnableViewState="False">
                    <asp:ListItem Text="posted on or before" Value="before"></asp:ListItem>
                    <asp:ListItem Text="posted on or since" Value="after" Selected="True"></asp:ListItem>
                </asp:DropDownList>
                <asp:CalendarExtender ID="tbxDateCalendarExtender" runat="server" Enabled="True"
                    TargetControlID="tbxDate">
                </asp:CalendarExtender>
                <br />
                <asp:CheckBox ID="cbxArchive" runat="server" 
                    Text="<%$ Resources:webResources, lblSearchArchive %>" 
                    EnableViewState="False" />
            </asp:Panel>
            <asp:Panel ID="Options" runat="server" GroupingText="Search Options" CssClass="topicTable"
                Visible="False" EnableViewState="False">
                <asp:Label ID="lblShowAs" runat="server" Text="<%$ Resources:webResources, lblShowAs %>"
                    AssociatedControlID="ddlShowAs" EnableViewState="False"></asp:Label>
                <asp:DropDownList ID="ddlShowAs" runat="server" 
                    EnableViewState="False" Enabled="False">
                    <asp:ListItem Text="Topic List" Value="topic"></asp:ListItem>
                    <asp:ListItem Text="<%$ Resources:webResources, lblPosts %>" Value="post"></asp:ListItem>
                </asp:DropDownList>
                <br />
                <asp:Label ID="lblSort" runat="server" Text="<%$ Resources:webResources, lblSortResults %>"
                    AssociatedControlID="ddlSortBy" EnableViewState="False"></asp:Label>
                <asp:DropDownList ID="ddlSortBy" runat="server" Enabled="true" 
                    EnableViewState="False">
                    <asp:ListItem Value="Subject">Subject</asp:ListItem>
                    <asp:ListItem Value="Replies">Number of Replies</asp:ListItem>
                    <asp:ListItem Value="Views">Number of Views</asp:ListItem>
                    <asp:ListItem Value="Date">Topic Start Date</asp:ListItem>
                    <asp:ListItem Value="LastPostDate" Selected="True">Last Post Date</asp:ListItem>
                    <asp:ListItem Value="Author">Author</asp:ListItem>
                    <asp:ListItem Value="ForumOrder">Forum</asp:ListItem>
                </asp:DropDownList><br />
                <asp:Label ID="lblPageSize" runat="server" Text="Results per page" 
                    AssociatedControlID="ddlPageSize" EnableViewState="False"></asp:Label>
                <asp:DropDownList ID="ddlPageSize" runat="server" EnableViewState="False">
                <asp:ListItem Value="10" Text="10" Selected="True"></asp:ListItem>
                <asp:ListItem Value="20" Text="20"></asp:ListItem>
                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                </asp:DropDownList>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Panel ID="SearchButtons" runat="server" CssClass="topicTable clearfix" HorizontalAlign="Right" >
        <asp:LinkButton ID="btnSearch" runat="server" 
            Text="<%$ Resources:webResources, lblSearch %>" OnClick="SearchForums"
            ValidationGroup="Search" style="float:right;" EnableViewState="False"/>
    </asp:Panel>
</div>
    <br />
    <asp:GridView ID="SearchResults" runat="server" Visible="false" 
        AutoGenerateColumns="False" AllowPaging="true"
        OnRowDataBound="ResultsRowDataBound" CellPadding="3" GridLines="None" EnableViewState="False"
        CssClass="topicTable noborder" 
        EmptyDataText="<%$ Resources:webResources, ErrNoTopics %>" 
        AllowSorting="False" onsorting="SearchResults_Sorting" 
        >
        <Columns>
            <%--<asp:BoundField DataField="ForumSubject" SortExpression="ForumSubject" Visible="False"/>--%>
            <asp:TemplateField HeaderText=" ">
                <ItemTemplate>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="20px" VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources:webResources, lblTopic %>" SortExpression="Subject">
                <ItemTemplate>
                    <asp:Image ID="imgPosticonSmall" SkinID="PosticonSmall" runat="server" Visible="False"
                        GenerateEmptyAlternateText="true" />
                    <a class="TopicLnk bbcode" href="/Content/Forums/topic.aspx?TOPIC=<%# Eval("Id") %>" target='<%# Eval("AuthorProfile") %>'
                        title="<%# Eval("Subject") %>">
                        <%# HttpUtility.HtmlDecode(Eval("Subject").ToString()) %>
                    </a>
                </ItemTemplate>
                <ItemStyle VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources:webResources, lblPostAuthor %>" SortExpression="Author.Name">
                <ItemTemplate>
                    <a href='<%# Eval("AuthorProfile") %>' title='<%# Eval("AuthorName") %>'><%# Eval("AuthorName") %></a>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="80px" VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources:webResources, lblReplies %>" SortExpression="ReplyCount">
                <ItemTemplate>
                    <%# Eval("ReplyCount")%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="60px" VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources:webResources, lblViewCount %>" SortExpression="Views">
                <ItemTemplate>
                    <%# Eval("Views") %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="60px" VerticalAlign="Top" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="<%$ Resources:webResources, lblLastPost %>" SortExpression="LastPostDate">
                <ItemTemplate>
                    <span class="smallText">by:&nbsp;<asp:Literal ID="popuplink" runat="server" Text='<%# Eval("LastPostAuthorPopup") %>'></asp:Literal>&nbsp;<asp:HyperLink
                        ID="lpLnk" runat="server" CssClass="profilelnk" SkinID="JumpTo" NavigateUrl='<%# String.Format("/Content/Forums/topic.aspx?TOPIC={0}&whichpage=-1#{1}", Eval("Id"),Eval("LastReplyId")) %>'
                        ToolTip="<%$ Resources:webResources, lblLastPostJump %>" Text="<%$ Resources:webResources, lblLastPostJump %>"></asp:HyperLink></span>
                    <br />
                    <%# Common.TimeAgoTag((DateTime) DataBinder.Eval(Container.DataItem, "LastPostDate"), IsAuthenticated,Member.TimeOffset)%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="122px" VerticalAlign="Top" />
            </asp:TemplateField>
        </Columns>
        <PagerTemplate>
            <asp:PlaceHolder ID="phPager" runat="server"></asp:PlaceHolder>
        </PagerTemplate>
        <HeaderStyle CssClass="tableheader" />
        <RowStyle CssClass="row" />
        <AlternatingRowStyle CssClass="altrow" />
        <PagerStyle CssClass="row" />
        <EmptyDataRowStyle CssClass="emptyrow" />
    </asp:GridView>
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Events,Active,Ads"/>

</asp:Content>