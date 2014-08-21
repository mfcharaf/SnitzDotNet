<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EventsAdmin.ascx.cs" Inherits="EventsCalendar.Admin.Mods.EventsAdmin" %>
<%@ Register TagPrefix="a" Namespace="SnitzCommon.Controls" Assembly="SnitzCommon" %>
<asp:Panel ID="modContainer" runat="server" CssClass="" GroupingText="Events Calendar Settings">
    <asp:Label ID="Label3" runat="server" Text="Mod Enabled" AssociatedControlID="rblEnabled" CssClass="mod_enabled_lbl" ></asp:Label><br/>
    <a:AdminRadioButtonList ID="rblEnabled" runat="server" Visible="True"  /> 
    <asp:Panel ID="edtPanel" runat="server" style="padding:6px;" GroupingText="Settings">
        <asp:Label ID="lblAllowed" runat="server" Text="Roles allowed to Add events" AssociatedControlID="lbRoles"></asp:Label><br/>
        <a:RolesListBox runat="server" ID="lbRoles" Rows="6"/>
        <asp:ListBox runat="server" ID="AllowedRoles" Rows="6"/>
        <asp:TextBox ID="txtRoles" runat="server" Width="300" TextMode="MultiLine" Rows="6"></asp:TextBox><br/>
        <asp:Button ID="btnAddRole" runat="server" Text="Add Roles" OnClick="AddRoleClick" />
        <asp:Panel ID="btnPanel" runat="server" CssClass="mod_btn_pnl">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </asp:Panel>
    </asp:Panel>
    <h3>Mod Instructions</h3>
    <asp:Panel ID="hlpPanel" runat="server" CssClass="help-panel"  Visible="False">
        <asp:Literal ID="litInstructions" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Panel>