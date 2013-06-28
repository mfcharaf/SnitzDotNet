<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="True" CodeBehind="Events.aspx.cs" Inherits="EventsCalendar.Events" Culture="auto" UICulture="auto"%>
<%@ Register Src="~/UserControls/Events/UpcomingEvents.ascx" TagName="UpcomingEvents" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Events/EventCalendar.ascx" TagName="EventCalendar" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHead" runat="server">
<style type="text/css">
.eventForm{margin-left:20px;background-color:White;width:auto;padding:10px;}
.eventForm label{width:150px;display:inline-block;}
.eventForm input, .eventForm select{margin-bottom:3px;}
.eventForm input[type="checkbox"] {width:20px;}
.eventForm span.cbx label{width:200px;}
.alignTopLeft{text-align: left; vertical-align: top;}
</style>
<script type="text/javascript">

    function replaceDates() {
        jQuery.each($("div#contentDIV [href]"), function () {
            $(this).html($(this).html().replace(/0/ig, '٠'));
            $(this).html($(this).html().replace(/1/ig, '۱'));
            $(this).html($(this).html().replace(/2/ig, '۲'));
            $(this).html($(this).html().replace(/3/ig, '۳'));
            $(this).html($(this).html().replace(/4/ig, '٤'));
            $(this).html($(this).html().replace(/5/ig, '٥'));
            $(this).html($(this).html().replace(/6/ig, '٦'));
            $(this).html($(this).html().replace(/7/ig, '۷'));
            $(this).html($(this).html().replace(/8/ig, '۸'));
            $(this).html($(this).html().replace(/9/ig, '۹'));
        });

        jQuery.each($("table.calYearLabel td"), function () {
        //alert($(this).text());
            //jQuery.each($("tbody [td]"), function () {

            $(this).html($(this).html().replace(/0/ig, '٠'));
            $(this).html($(this).html().replace(/1/ig, '۱'));
            $(this).html($(this).html().replace(/2/ig, '۲'));
            $(this).html($(this).html().replace(/3/ig, '۳'));
            $(this).html($(this).html().replace(/4/ig, '٤'));
            $(this).html($(this).html().replace(/5/ig, '٥'));
            $(this).html($(this).html().replace(/6/ig, '٦'));
            $(this).html($(this).html().replace(/7/ig, '۷'));
            $(this).html($(this).html().replace(/8/ig, '۸'));
            $(this).html($(this).html().replace(/9/ig, '۹'));
        });

    } 
</script>
<asp:Literal ID="rtlCss" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CPH1" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPHR" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPHL" runat="server">
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="RightCol" runat="server">

    <snitz:sidebar runat="server" ID="sidebar" Show="Ads,Active"/>

               
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="CPM" runat="server">
    <asp:Panel ID="pnlCalendar" runat="server" CssClass="forumtable">
        <uc1:EventCalendar ID="EventCalendar1" runat="server" EnableViewState="False" /><br />
    </asp:Panel>
    <asp:UpdatePanel ID="updNewEvent" runat="server" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Panel ID="pnlAddEvent" runat="server" Visible="false" CssClass="eventForm" 
            GroupingText="Event Details" EnableViewState="False" DefaultButton="Button1">
            <asp:Label ID="Label1" runat="server" Text="Title" 
                AssociatedControlID="tbxTitle" EnableViewState="False"></asp:Label>
            <asp:TextBox ID="tbxTitle" runat="server" MaxLength="50" 
                EnableViewState="False"></asp:TextBox><br />
            <asp:Label ID="Label2" runat="server" Text="Description" 
                AssociatedControlID="tbxDescription" EnableViewState="False"></asp:Label>
            <asp:TextBox ID="tbxDescription" runat="server" MaxLength="400" Width="300px" 
                EnableViewState="False"></asp:TextBox><br />
            <asp:Label ID="Label3" runat="server" Text="Event Type" 
                AssociatedControlID="ddlType" EnableViewState="False"></asp:Label>
            <asp:DropDownList ID="ddlType" runat="server">
            </asp:DropDownList><br />
            <asp:Label ID="Label4" runat="server" Text="Audience" 
                AssociatedControlID="ddlRoles" CssClass="alignTopLeft" EnableViewState="False"></asp:Label>
            <asp:ListBox ID="ddlRoles" runat="server" SelectionMode="Multiple">
            </asp:ListBox>
            <br />
            <asp:Label ID="Label6" runat="server" Text="Recurring event" 
                AssociatedControlID="ddlRecurr" EnableViewState="False"></asp:Label>
            <asp:DropDownList ID="ddlRecurr" runat="server" Enabled="false">
                <asp:ListItem Value="0">[Not Recurring]</asp:ListItem>
                <asp:ListItem Value="1">Every day</asp:ListItem>
                <asp:ListItem Value="2">Every other day</asp:ListItem>
                <asp:ListItem Value="3">Mon - Fri</asp:ListItem>
                <asp:ListItem Value="4">Sat & Sun</asp:ListItem>
                <asp:ListItem Value="5">Every week</asp:ListItem>
                <asp:ListItem Value="6">Every other week</asp:ListItem>
                <asp:ListItem Value="7">Every month</asp:ListItem>
                <asp:ListItem Value="8">Every year</asp:ListItem>
            </asp:DropDownList><br />
            <asp:Label ID="Label5" runat="server" Text="Event Date" 
                AssociatedControlID="Calendar1" EnableViewState="False"></asp:Label>
            <asp:Calendar ID="Calendar1" runat="server" OnDayRender="NewCal_DayRender" 
                OnSelectionChanged="NewCal_DayChange" EnableViewState="False">
                <SelectedDayStyle BackColor="Green" />
            </asp:Calendar>
            <asp:CheckBox CssClass="cbx" ID="cbxMultiSelect" runat="server" Text="Select multiple days" AutoPostBack="true" OnCheckedChanged="MultiSelectChanged" />
            <br />
            <asp:LinkButton ID="Button1" runat="server" Text="<%$ Resources:webresources,btnSubmit %>" OnClick="SubmitEvent" 
                EnableViewState="False" />&nbsp;
            <asp:LinkButton ID="Button2" runat="server" EnableViewState="False" Text="<%$ Resources:webresources,btnCancel %>"/>&nbsp;
            <asp:LinkButton ID="btnDelEvent" runat="server" onclick="btnDelEvent_Click" 
                Text="<%$ Resources:webresources,btnDelete %>" Visible="False" EnableViewState="False" /><br />
        </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <br />
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>


<asp:Content ID="Content9" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
