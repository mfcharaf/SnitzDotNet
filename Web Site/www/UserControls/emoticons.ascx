<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="emoticons.ascx.cs" Inherits="SnitzUI.UserControls.Emoticons" %>
<%@ Import Namespace="SnitzConfig" %>
<div id="emoticons">
    <asp:DataList ID="DataList1" runat="server" ShowHeader="true" Visible="<%# Config.EmoticonTable %>" RepeatDirection="Horizontal" RepeatLayout="Flow">

        <ItemTemplate>
            <a href="#" title="<%# Eval("Code") %>" rel="<%# Eval("Code") %>">
                <img src="<%# Eval("ImageUrl") %>" title='<%# Eval("Description") %>' alt="<%# Eval("Description") %>" /></a>
        </ItemTemplate>
    </asp:DataList>
</div>