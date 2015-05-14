<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="WebUserControl"  Codebehind="JumpTo.ascx.cs" %>
<script type="text/javascript">
    function navFromList(qsParam) {
        document.location.href = "/Content/Forums/forum.aspx?FORUM=" + qsParam;
        return false;
    }
</script>
<%@ Register Assembly="SnitzCommon" Namespace="GroupDropDownList" TagPrefix="cc1" %>
<asp:Label ID="Label1" CssClass="jumpto" runat="server" Text="<%$ Resources:webResources, lblJumpTo %>" ></asp:Label> :
<cc1:GroupDropDownList Width="75%" ID="GroupDropDownList1" runat="server" onchange="navFromList(this.value);" EnableViewState="false">
</cc1:GroupDropDownList>
