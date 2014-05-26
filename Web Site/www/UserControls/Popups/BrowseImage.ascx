<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BrowseImage.ascx.cs" Inherits="SnitzUI.UserControls.Popups.BrowseImage" %>
<div style="max-width:600px;width:98%;">
<asp:Repeater ID="rptImage" runat="server" >
<ItemTemplate >  
        <img ID="imgLeft" runat="server" src='<%# Eval("ThumbPath") %>' alt='' rel='<%# Eval("ImagePath") %>' class="imgSelect" style="height:100px;margin:3px;" />  
</ItemTemplate>

</asp:Repeater>
</div>