<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RSSFeed.ascx.cs" Inherits="SnitzUI.Admin.Mods.RSSFeed" %>
<%@ Register TagPrefix="uc1" TagName="AdminRadioButton" Src="~/Admin/UserControls/AdminRadioButton.ascx" %>
<asp:Panel ID="modContainer" runat="server" CssClass="" GroupingText="xxx">
    <asp:Label ID="Label3" runat="server" Text="Mod Enabled" AssociatedControlID="rblEnabled" CssClass="mod_enabled_lbl" ></asp:Label><uc1:AdminRadioButton ID="rblEnabled" runat="server" Visible="True"  /> <br/>
    <asp:Panel ID="edtPanel" runat="server" style="padding:6px;" GroupingText="Settings">
        <asp:Label ID="lblUrl" runat="server" Text="Url" AssociatedControlID="txtUrl"></asp:Label><br/>
        <asp:TextBox ID="txtUrl" runat="server" Width="300"></asp:TextBox>
    
        <asp:Panel ID="btnPanel" runat="server" CssClass="mod_btn_pnl">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </asp:Panel>
    </asp:Panel>
    <h3>Mod Instructions</h3>
    <asp:Panel ID="hlpPanel" runat="server" CssClass="help-panel"  Visible="False">
        <asp:Literal ID="litInstructions" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Panel>
