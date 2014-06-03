<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="UpcomingEvents.ascx.cs" Inherits="EventsCalendar.UserControls.UpcomingEvents" %>

<asp:Panel ID="Panel2" runat="server" CssClass="sideBox" >
    <asp:Repeater runat="server" ID="FutureEvents" OnItemDataBound="rptEventBound">
        <HeaderTemplate>
            <div class="category cattitle" style="width:100%">
            <asp:HyperLink ID="lnkAdd" runat="server" NavigateUrl="~/Content/Events/Events.aspx?mode=new" SkinID="AddEvent" ToolTip="Add Event" EnableViewState="False"></asp:HyperLink>&nbsp;<asp:Label ID="Label1" runat="server" Text="Upcoming events" Style="width: 100%; margin: auto;"></asp:Label>
            </div>
        </HeaderTemplate>
        <ItemTemplate>
            <asp:Label ID="lblDate" runat="server" EnableViewState="False"></asp:Label>&nbsp;
        <asp:Literal ID="viewEvent" runat="server" EnableViewState="False"></asp:Literal>
        </ItemTemplate>
        <SeparatorTemplate>
            <hr />
        </SeparatorTemplate>
        <FooterTemplate></FooterTemplate>
    </asp:Repeater>
</asp:Panel>
<br class="seperator"/>

