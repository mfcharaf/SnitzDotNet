<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultMod.ascx.cs" Inherits="SnitzUI.Admin.Mods.DefaultMod" %>
<%@ Register TagPrefix="uc1" TagName="AdminRadioButton" Src="~/Admin/UserControls/AdminRadioButton.ascx" %>
<asp:Panel ID="modContainer" runat="server" CssClass="" GroupingText="xxx">
    <asp:Label ID="Label3" runat="server" Text="Mod Enabled" AssociatedControlID="rblEnabled" CssClass="mod_enabled_lbl"></asp:Label><uc1:AdminRadioButton ID="rblEnabled" runat="server" Visible="True"  /> <br/>
        <asp:Panel ID="btnPanel" runat="server" CssClass="mod_btn_pnl">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </asp:Panel>
    <asp:Panel ID="hlpPanel" runat="server" Style="height: 300px; overflow: auto; border: 1px solid lightgray;" Visible="False">
        <h3>Mod Instructions</h3>
        <asp:Literal ID="litInstructions" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Panel>