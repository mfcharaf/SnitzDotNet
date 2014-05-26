<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ModConfiguration.ascx.cs" Inherits="Admin_ModConfiguration" %>
    <asp:Panel ID="pnl1" runat="server" CssClass="ConfigForm clearfix">
<asp:DataList ID="DataList1" runat="server" OnItemDataBound="DataList1_ItemDataBound">
    <ItemTemplate>
    <div style="float:left;width:540px;">
    <div style="float:left;width:200px;">
        <asp:Label ID="Label1" runat="server" Text='<%# Eval("key")%>'></asp:Label>
    </div>
        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Eval("value") %>' Width="250px"></asp:TextBox></div>
        <asp:Label ID="Label2" runat="server" Text="<hr />" Visible="false"></asp:Label>
    </ItemTemplate>
    <AlternatingItemStyle VerticalAlign="Middle" />
</asp:DataList>
        <asp:Button ID="B1" runat="server" OnClick="B1_Click" Text="Save" /></asp:Panel>
