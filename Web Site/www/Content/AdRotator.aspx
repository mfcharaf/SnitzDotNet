<%-- 
##############################################################################################################
## Snitz Forums .net
##############################################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##############################################################################################################
--%>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdRotator.aspx.cs" Inherits="SnitzUI.AdRotator" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
    html 
    {
	    font-family: Tahoma, Arial, Helvetica, sans-serif;
	    font-size: 0.9em;	
    }
    body
    {
        background-color: #FFFFFF;
        margin: 0px;
        color: #FFBB2F;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin:auto;text-align:center;">
        <asp:AdRotator ID="adr" runat="server" AdvertisementFile="~/App_Data/classifieds.xml"
        OnAdCreated="AdRotator1AdCreated" Target="_blank" />
    </div>
    </form>
</body>
</html>
