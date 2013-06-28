<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="emoticons.ascx.cs" Inherits="SnitzUI.UserControls.emoticons" %>
<%@ Import Namespace="SnitzConfig" %>
<div id="emoticons">
    <asp:DataList ID="DataList1" runat="server" RepeatColumns="6" Width="100px" ShowHeader="true" Visible="<%# Config.EmoticonTable %>">
        <HeaderTemplate>
            emoticons</HeaderTemplate>
        <HeaderStyle HorizontalAlign="Center" />
        <ItemTemplate>
            <a href="#" title="<%# Eval("Code") %>">
                <img src="<%# Eval("ImageUrl") %>" title='<%# Eval("Description") %>' alt="<%# Eval("Description") %>" /></a>
        </ItemTemplate>
    </asp:DataList>
</div>