﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="adrotation.ascx.cs" Inherits="SnitzUI.UserControls.adrotation" %>
<%@ Import Namespace="SnitzConfig" %>
<script language="javascript" type="text/javascript">
    function resizeIframe(obj) {
        obj.style.height = obj.contentWindow.document.body.scrollHeight + 'px';
        obj.style.width = obj.contentWindow.document.body.scrollWidth + 'px';
    }
</script>
<asp:Literal ID="Literal1" runat="server"></asp:Literal>
<asp:Panel runat="server" ID="GoogleAd"> 
<script type="text/javascript"><!--
    google_ad_client = "<%= Config.GoogleAdCode %>";
    /* Snitz-Test-banner */
    google_ad_slot = "8719410527";
    google_ad_width = 468;
    google_ad_height = 60;
//-->
</script>
<script type="text/javascript"
src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
</script>

</asp:Panel>