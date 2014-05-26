<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>
<%@ Control Language="C#" ClassName="AdminPanel" %>

<asp:Menu ID="Menu1" runat="server" CssSelectorClass="adminMenu" DataSourceID="dsAdminPanelSiteMap" EnableViewState="False" StaticDisplayLevels="2">
</asp:Menu>
<asp:SiteMapDataSource ID="dsAdminPanelSiteMap" runat="server" SiteMapProvider="ConfigSiteMap" ShowStartingNode="False"/>
