<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="MiniStatistics" Codebehind="MiniStatistics.ascx.cs" EnableTheming="true" EnableViewState="False" %>
<div id="statsControl" class="sideBox">
    <asp:Panel ID="Stats_HeaderPanel" runat="server" CssClass="category statsPanelHeader" style="cursor: pointer;" EnableViewState="False">
        <div class="heading">
            <asp:Label runat="server" Text='<%$ Resources:webResources, lblStatistics %>' ID="lblStats" ></asp:Label>
        </div>
    </asp:Panel>

    <asp:Panel ID="Stats_Panel" runat="server" CssClass="statsPanel innertube" EnableViewState="False">

    <asp:Label ID="lblActiveUsers" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
    <asp:Label ID="lblActiveSessions" runat="server" CssClass="smallText" EnableViewState="false"></asp:Label><br />
    <br />
    <asp:Label ID="lblMemberStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />

    <asp:Label ID="lblTopicStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />

    <%--<asp:Label ID="lblArchiveStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />--%>

    <asp:Label ID="lblNewestMember" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label>
</asp:Panel>

</div>
<br class="seperator"/>