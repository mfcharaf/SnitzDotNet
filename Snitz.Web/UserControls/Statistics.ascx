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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="ucStatistics" Codebehind="Statistics.ascx.cs" EnableTheming="true" EnableViewState="False" %>
<div id="statsControl" class="forumtable">
    <asp:Panel ID="Stats_HeaderPanel" runat="server" CssClass="statsPanelHeader clearfix" style="cursor: pointer;" EnableViewState="False">
            <asp:Image ID="statsExpand" runat="server" GenerateEmptyAlternateText="true" EnableViewState="False" ImageAlign="Middle" />
            <asp:Label runat="server" Text='<%$ Resources:webResources, lblStatistics %>' 
                ID="lblStats" EnableViewState="False" ></asp:Label>
    </asp:Panel>
    <asp:Panel ID="Stats_Panel" runat="server" CssClass="statsPanel" EnableViewState="False">
        <asp:Label ID="lblActiveUsers" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
        <asp:Label ID="lblActiveSessions" runat="server" CssClass="smallText" EnableViewState="false"></asp:Label><br />
        <asp:Label ID="lblLastVisit" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
        <asp:Label ID="lblMemberStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
        <asp:Label ID="lblTopicStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
        <asp:Label ID="lblArchiveStats" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
        <asp:Label ID="lblNewestMember" runat="server" CssClass="smallText" EnableViewState="False"></asp:Label><br />
    </asp:Panel>
    <asp:CollapsiblePanelExtender ID="Stats_Panel_CollapsiblePanelExtender" SuppressPostBack="true" SkinID="StatsSkin"
        CollapseControlID="Stats_HeaderPanel" ExpandControlID="Stats_HeaderPanel" Collapsed="true"
        runat="server" Enabled="True" TargetControlID="Stats_Panel"  EnableViewState="true">
    </asp:CollapsiblePanelExtender>
</div>