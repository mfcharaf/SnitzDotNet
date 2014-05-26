<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_ModConfiguration" Codebehind="ModConfiguration.ascx.cs" %>
<%@ Register src="AdminRadioButton.ascx" tagname="AdminRadioButton" tagprefix="uc1" %>
    <asp:Panel ID="pnl1" runat="server" CssClass="ConfigForm clearfix">
        <asp:DataList ID="DataList1" runat="server" OnItemDataBound="DataList1_ItemDataBound">
            <ItemTemplate>
            <asp:Panel runat="server" style="float:left;width:540px;" ID="ctrlHead" Visible="false">
                <asp:CheckBox runat="server" ID="chkEnable" ToolTip="Enable/Disable"/>&nbsp;<asp:Label ID="Label2" runat="server" Text="" Visible="false"></asp:Label>
            </asp:Panel>
            <asp:Panel style="float:left;width:540px;" runat="server" ID="ctrl">
                <div style="float:left;width:200px;">
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("key")%>'></asp:Label>
                </div>
                <asp:TextBox ID="txtCtrl" runat="server" Text='<%# Eval("value") %>' Width="250px"></asp:TextBox>
                <uc1:AdminRadioButton ID="rblCtrl" runat="server" Visible="False" /> <br />
            </asp:Panel>
            </ItemTemplate>
            <AlternatingItemStyle VerticalAlign="Middle" />
        </asp:DataList>
        <asp:Button ID="B1" runat="server" OnClick="B1_Click" Text="Save" />
    </asp:Panel>
