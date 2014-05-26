<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePicker.ascx.cs"
    Inherits="SnitzUI.UserControls.DatePicker" %>
<asp:Label ID="lbl_Bio_DOB" runat="server" Text="Date of birth" EnableViewState="False"
    AssociatedControlID="ddlYear" CssClass="doblabel"></asp:Label>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
        <asp:DropDownList ID="ddlYear" runat="server" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged"
            DataTextField="Value" DataValueField="Key"
            AutoPostBack="true">
        </asp:DropDownList>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="updpnlMonth" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
        <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="true" DataTextField="MonthName"
            DataValueField="MonthNumber" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
        </asp:DropDownList>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:UpdatePanel ID="updpnlDay" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate>
        <asp:DropDownList ID="ddlday" runat="server" DataTextField="Value" DataValueField="Key">
        </asp:DropDownList>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ddlMonth" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="ddlYear" EventName="SelectedIndexChanged" />
    </Triggers>
</asp:UpdatePanel>

