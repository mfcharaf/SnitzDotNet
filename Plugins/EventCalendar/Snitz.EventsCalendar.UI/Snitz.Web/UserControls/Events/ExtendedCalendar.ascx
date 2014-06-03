<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="ExtendedCalendar.ascx.cs" Inherits="EventsCalendar.UserControls.ExtendedCalendar" %>
    <asp:Calendar OnDayRender="DayRender" TodaysDate='<%# TodaysDate %>'
    CellPadding="1" CellSpacing= "1"
    Runat="server" id="calMonth" 
    SelectionMode="Day" SkinID="smlCal" ShowNextPrevMonth="False" TitleFormat="Month" 
            CssClass="smallCal" ShowGridLines="True" 
    TitleStyle-CssClass="calTitle" EnableViewState="False" 
    onselectionchanged="ShowDay" >
<TitleStyle CssClass="calTitle"></TitleStyle>
        <WeekendDayStyle BackColor="#FFFFCC" />
    </asp:Calendar>
<asp:LinkButton ID="lnk" runat="server" onclick="lnk_Click" CssClass="calLink"
    CausesValidation="False" Visible="False">LinkButton</asp:LinkButton>