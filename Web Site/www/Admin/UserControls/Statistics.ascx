<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_Statistics" Codebehind="Statistics.ascx.cs" %>
<table runat="server" id="Table1" cellpadding="3" cellspacing="0" class="forumtable white" width="100%" >
    <thead>
    <tr>
        <th colspan="2" class="category categorylink">Forum Statistics</th>
    </tr>
    </thead>
    <tr id="LastVisitRow">
        <td style="width:205px;">
            <asp:Label ID="Label1" runat="server" Text="Snitz Version" 
                meta:resourcekey="Label1Resource1"></asp:Label></td>
        <td>
            <asp:Label ID="lblForumVersion" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td style="width:205px;">
            <asp:Label ID="Label2" runat="server" Text="Database Size (Total/Free)" 
                meta:resourcekey="Label2Resource1"></asp:Label>
        </td>
        <td>
            <asp:Label ID="lblDBSize" runat="server" CssClass="smallText"></asp:Label></td>
    </tr>
    <tr>
        <td style="width:205px;">
            <asp:Label ID="Label3" runat="server" Text="Avatar directory size" 
                meta:resourcekey="Label3Resource1"></asp:Label></td>
        <td>
            <asp:Label ID="lblAvatar" runat="server" CssClass="smallText" Text="&nbsp;"></asp:Label></td>
    </tr>
    <tr>
        <td style="width:205px;">
            <asp:Label ID="Label4" runat="server" Text="Forum directory size" 
                meta:resourcekey="Label4Resource1"></asp:Label></td>
        <td>
            <asp:Label ID="lblFiles" runat="server" CssClass="smallText" Text="&nbsp;"></asp:Label></td>
    </tr>
    <tr>
        <td style="width:205px;">&nbsp;</td>
        <td>&nbsp;</td>
    </tr>
</table>
