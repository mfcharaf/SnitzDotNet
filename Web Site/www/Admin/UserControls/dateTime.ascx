<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_dateTime" Codebehind="dateTime.ascx.cs" %>
<%@ Register TagPrefix="a" Namespace="SnitzCommon.Controls" Assembly="SnitzCommon" %>
<%@ Register TagPrefix="uc1" TagName="AdminRadioButton" Src="~/Admin/UserControls/AdminRadioButton.ascx" %>

<asp:Panel ID="Panel2" runat="server" CssClass="clearfix adminform" DefaultButton="btnSubmit">
<h2 class="category">Server Date/Time Configuration</h2>
    <div id="content">
        <asp:Panel runat="server" ID="pnl2" GroupingText="Display Settings" CssClass="defaultform">
            <asp:Label ID="lblTimeType" runat="server" Text="Time Display" AssociatedControlID="rblTimeType"></asp:Label>
            <asp:RadioButtonList ID="rblTimeType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                <asp:ListItem Value="HH:mm:ss">24hr</asp:ListItem>
                <asp:ListItem Value="hh:mm:ss tt">12hr</asp:ListItem>
            </asp:RadioButtonList><br/>
            <asp:Label ID="lblTimeZone" runat="server" Text="Forum TimeZone" AssociatedControlID="lbTimeZone"></asp:Label>
            <a:TimeZoneListBox ID="lbTimeZone" runat="server" AutoPostBack="False"></a:TimeZoneListBox><br/>
            <asp:Label ID="lblDaylightSaving" runat="server" Text="Adjust for daylight saving" 
                AssociatedControlID="rblDaylightSaving" EnableViewState="False"></asp:Label>
            <uc1:AdminRadioButton ID="rblDaylightSaving" runat="server" /><br/>
            <asp:Label ID="lblDateType" runat="server" Text="Date Display" AssociatedControlID="lblDateType"></asp:Label>
            <asp:DropDownList ID="ddlDateType" runat="server">
                <asp:ListItem value="MM/dd/yyyy">12/31/2000 (US short)</asp:ListItem>
                <asp:ListItem value="dd/MM/yyyy">31/12/2000 (UK short)</asp:ListItem>
                <asp:ListItem value="yyyy/MM/dd">2000/12/31 (Other short)</asp:ListItem>
                <asp:ListItem value="yyyy/dd/MM">2000/31/12 (Other short)</asp:ListItem>
                <asp:ListItem value="MMM dd yyyy">Dec 31 2000 (US med)</asp:ListItem>
                <asp:ListItem value="dd MMM yyyy">31 Dec 2000 (UK med)</asp:ListItem>
                <asp:ListItem value="yyyy MMM dd">2000 Dec 31 (Other med)</asp:ListItem>
                <asp:ListItem value="yyyy dd MMM">2000 31 Dec (Other med)</asp:ListItem>
                <asp:ListItem value="MMM dd yyyy">December 31 2000 (US long)</asp:ListItem>
                <asp:ListItem value="dd MMMM yyyy">31 December 2000 (UK long)</asp:ListItem>
                <asp:ListItem value="yyyy MMMM dd">2000 December 31 (Other long)</asp:ListItem>
                <asp:ListItem value="yyyy dd MMMM">2000 31 December (Other long)</asp:ListItem>
            </asp:DropDownList>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnl3" GroupingText="Current Values" CssClass="defaultform">
            <asp:Label ID="lblCurrentDate" runat="server" Text="Current UTC Date/Time" AssociatedControlID="lblToday"></asp:Label>&nbsp;
            <asp:Label ID="lblToday" runat="server" Text="Label"></asp:Label><br/>
            <asp:Label ID="Label2" runat="server" Text="Current Forum Date/Time" AssociatedControlID="Label3"></asp:Label>&nbsp;<asp:Label ID="Label3" runat="server" Text="Label"></asp:Label><br/>
        </asp:Panel>
        <asp:Panel ID="Panel5" runat="server">
            <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />&nbsp;
            <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />
        </asp:Panel>
    </div>
</asp:Panel>
