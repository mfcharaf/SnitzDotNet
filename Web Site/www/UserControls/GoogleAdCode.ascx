<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GoogleAdCode.ascx.cs" Inherits="SnitzUI.UserControls.GoogleAdCode" %>
<div id="googleAdSide">
    <script type="text/javascript"><!--
    google_ad_client = "<%= AdCode %>";
        /* Snitz-Test */
        google_ad_slot = "<%= AdSlot %>";
        google_ad_width = <%= AdWidth %>;
        google_ad_height = <%= AdHeight %>;
        //-->
    </script>
    <script type="text/javascript"
    src="http://pagead2.googlesyndication.com/pagead/show_ads.js">
    </script>
    </div>
<br class="seperator"/>