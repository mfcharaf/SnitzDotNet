<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ExtendedCalendar.ascx.cs" Inherits="EventsCalendar.UserControls.ExtendedCalendar" %>
    <asp:Calendar OnDayRender="DayRender" TodaysDate='<%# TodaysDate %>'
    CellPadding="1" CellSpacing= "0"
    Runat="server" id="calMonth" 
    SelectionMode="Day" SkinID="smlCal" ShowNextPrevMonth="False" TitleFormat="Month" 
    CssClass="smallCal" ShowGridLines="True" 
    TitleStyle-CssClass="calTitle" EnableViewState="False" 
    onselectionchanged="ShowDay" >
    <OtherMonthDayStyle ForeColor="Gray" />  
    <DayStyle CssClass="smallCalDay" />  
    <SelectedDayStyle Font-Bold="True" Font-Size="12px" />  
    <SelectorStyle CssClass="smallCalSelector" />  
    <NextPrevStyle CssClass="smallCalNextPrev" />  
    <TitleStyle CssClass="smallCalTitle" />  
    </asp:Calendar>
<asp:LinkButton ID="lnk" runat="server" onclick="LnkClick" CssClass="calLink"
    CausesValidation="False" Visible="False">LinkButton</asp:LinkButton>