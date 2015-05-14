<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ExtendedCalendar.ascx.cs" Inherits="EventsCalendar.UserControls.ExtendedCalendar" %>
    <asp:Calendar OnDayRender="DayRender" TodaysDate='<%# TodaysDate %>'
    Runat="server" id="calMonth" 
    SelectionMode="Day" SkinID="smlCal" ShowNextPrevMonth="False" TitleFormat="Month" 
    EnableViewState="False" 
    onselectionchanged="ShowDay" >
        <SelectedDayStyle Font-Bold="True" Font-Size="12px" />  
        <SelectorStyle CssClass="smallCalSelector" />  
        <NextPrevStyle CssClass="smallCalNextPrev" />  
        <TitleStyle CssClass="smallCalTitle" />  
        <OtherMonthDayStyle CssClass="calendar-prevmonth-day" ForeColor="Silver"/>
        <DayStyle CssClass="calendar-day"/>
        <DayHeaderStyle CssClass="calendar-day-header"/>
    </asp:Calendar>
<asp:LinkButton ID="lnk" runat="server" onclick="LnkClick" CssClass="calLink"
    CausesValidation="False" Visible="False">LinkButton</asp:LinkButton>