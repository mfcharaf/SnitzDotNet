<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_dateTime" Codebehind="dateTime.ascx.cs" %>
    <asp:Panel ID="Panel1" runat="server" CssClass="clearfix" DefaultButton="btnSubmit">
        <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td colspan="2" class="category">
                    <asp:Label ID="Label1" runat="server" Text="Server Date/Time Configuration"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    &nbsp;</td>
                <td style="width: 50%;text-align:left">&nbsp;

                </td>
            </tr>            
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblTimeType" runat="server" Text="Time Display"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:RadioButtonList ID="rblTimeType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="HH:mm:ss">24hr</asp:ListItem>
                        <asp:ListItem Value="hh:mm:ss tt">12hr</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblTimeZone" runat="server" Text="Forum TimeZone"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:DropDownList ID="ddlTimeZone" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblCurrentDate" runat="server" Text="Current UTC Date/Time"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:Label ID="lblToday" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; text-align: right">
                    <asp:Label ID="Label2" runat="server" Text="Current Forum Date/Time"></asp:Label>&nbsp;:&nbsp;</td>
                <td style="width: 50%; text-align: left">
                    <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label></td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblDateType" runat="server" Text="Date Display"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
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
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    &nbsp;</td>
                <td style="width: 50%;text-align:left">&nbsp;

                </td>
            </tr>
                    <tr>
                <td style="width: 50%;text-align:right;">
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />&nbsp;
                    <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />
                </td>
            </tr></table>

    </asp:Panel>