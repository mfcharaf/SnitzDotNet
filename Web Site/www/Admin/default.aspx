<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>
<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="true" Inherits="AdminHome" Title="Untitled Page" Codebehind="default.aspx.cs" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<%@ Register Src="UserControls/AdminPanel.ascx" TagName="AdminPanel" TagPrefix="uc1" %>
<%@ Register Src="UserControls/Statistics.ascx" TagName="Admin_Statistics" TagPrefix="uc8" %>
<%@ Reference Control="UserControls/features.ascx" %>
<%@ Reference Control="UserControls/system.ascx" %>
<%@ Reference Control="UserControls/ManageRoles.ascx" %>
<%@ Reference Control="UserControls/email.ascx" %>
<%@ Reference Control="UserControls/dateTime.ascx" %>
<%@ Reference Control="UserControls/filters.ascx" %>
<%@ Reference Control="UserControls/membership.ascx" %>
<%@ Reference Control="UserControls/Pendingmembers.ascx" %>
<%@ Reference Control="UserControls/NewMember.ascx" %>
<%@ Reference Control="UserControls/ManageAvatars.ascx" %>
<%@ Reference Control="UserControls/ManageProfile.ascx" %>
<%@ Reference Control="UserControls/Subscriptions.ascx" %>
<%@ Reference Control="UserControls/EmoticonAdmin.ascx" %>
<%@ Reference Control="~/UserControls/Polls/PollAdmin.ascx" %>
<%@ Reference Control="~/UserControls/Polls/PollEdit.ascx" %>
<%@ Reference Control="~/UserControls/Polls/Poll.ascx" %>


<asp:Content ID="cphead" runat="server" ContentPlaceHolderID="CPHead">
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="menuCSS"/>
    <style type="text/css">
        .style1
        {
            width: 35%;
        }
    </style>
</asp:Content>
<asp:Content ID="adOverride" runat="server" contentplaceholderid="CPAd">
</asp:Content>
<asp:Content ID="C3" runat="server" ContentPlaceHolderID="CPM">
    <div style="width:95%;margin:auto" class="clearfix">
        <div style="float:left;width:25%; left: 0; top: 0;">
            <uc1:AdminPanel ID="Menu" runat="server" />
        </div>
            
        <div style="width:70%;margin:auto;height:auto;vertical-align:top;float:left;border-radius:5px;">
            <asp:Panel ID="Panel1" runat="server" Width="100%"  
                meta:resourcekey="Panel1Resource1">
            <div class="forumtable">
            <div style="width:100%;text-align:center;height:23px;padding-top:4px;" class="category categorylink">Snitz™ Forums .Net Admin Panel</div>
            <div style="width:98%;margin:auto;padding:3px;" >
            <p>Thank you for choosing Snitz™ .Net as your Forum solution. This screen will give you a quick overview of all the various statistics of your Forum. The links in the menu panel to the left will allow you to control to set various parameters to control your forum.</p>
            <p>For the latest information on updates to Snitz™ Forums , why not subscribe to our mailing list.<br /><br /></p></div></div>
            <br />
            <uc8:Admin_Statistics ID="Statistics1" runat="server" />
                &nbsp;
            <br />
            <div class="forumtable">
            <div style="width:100%;text-align:center;height:23px;padding-top:4px;" class="category categorylink">Snitz™ Forums .Net Developers &amp; Contributors</div>
            <div style="width:100%;padding:10px;">
                <table>
                <tr><td style="text-align:right;vertical-align:top;font-weight:bold;white-space:nowrap" 
                        class="style1">Snitz™ Forum Software :</td><td style="width:auto">&copy; Snitz Forums 2000</td></tr>
                <tr><td style="text-align:right;vertical-align:top;font-weight:bold;white-space:nowrap" 
                        class="style1">.Net Development :</td><td style="width:auto">
                    Huw Reddick</td></tr>
                <tr><td style="text-align:right;vertical-align:top;font-weight:bold;white-space:nowrap" 
                        class="style1">Language Translations :</td>
                    <td style="width:auto;vertical-align:top;">
                        Irish: Pádhraic Ó Láimhín<br />
                        German: Michael Reisinger <br />
				        Italian: Dario Panada<br />
				        French: Machina Adibou<br />
				        Japanese: Taku Shingai<br />
                        Persian: Sadra
                    </td>
                </tr>
                <tr><td style="text-align:right;vertical-align:top;font-weight:bold;white-space:nowrap" 
                        class="style1">Testers :</td>
                    <td style="width:auto;vertical-align:top;">
                        Pacnorwest<br />
                    </td>
                </tr>
                </table>
            </div></div>
            </asp:Panel>
            <asp:PlaceHolder ID="CP1" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    <br />
</asp:Content>

