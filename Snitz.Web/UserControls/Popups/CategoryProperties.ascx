<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryProperties.ascx.cs" Inherits="SnitzUI.UserControls.Popups.CategoryProperties" %>
<div id="divEditCat" >
    <asp:HiddenField ID="hdnCatId" runat="server" />
    <asp:Panel ID="pnlMain" runat="server" GroupingText="Forum">
        <asp:Label ID="Label1" runat="server" Text="Subject" AssociatedControlID="tbxSubject"></asp:Label>
        <asp:TextBox ID="tbxSubject" CssClass="testClass" runat="server" EnableViewState="False"></asp:TextBox>
        <br />
        <asp:CheckBox Visible="false" ID="cbxCountPost" CssClass="testClass" runat="server" Text="Increase user Post Count" />
        <br />
        <asp:Label ID="Label4" runat="server" Text="Moderation level" EnableViewState="False"></asp:Label>
        <asp:DropDownList ID="ddlMod" runat="server" CssClass="testClass">
            <asp:ListItem Value="0">No Moderation in this Category</asp:ListItem>
            <asp:ListItem Value="1">Moderation Allowed</asp:ListItem>
        </asp:DropDownList>
        <br />
        <asp:Label ID="Label5" runat="server" Text="Subscription Level" 
            EnableViewState="False"></asp:Label>
        <asp:DropDownList ID="ddlSub" runat="server" CssClass="testClass">
            <asp:ListItem Value="0">No Subscriptions Allowed</asp:ListItem>
            <asp:ListItem Value="1">Forum Subscriptions Allowed</asp:ListItem>
            <asp:ListItem Value="2">Topic Subscriptions Allowed</asp:ListItem>
        </asp:DropDownList>
        <br />
    </asp:Panel>
    <asp:Panel ID="Panel5" runat="server">
        <button onclick="SaveCategory();return false;" type="button">
            <%= Resources.webResources.btnSubmit %></button>&nbsp;
        <button onclick="mainScreen.CancelModal();return false;" type="button">
                <%= Resources.webResources.btnCancel %></button>
    </asp:Panel>
</div>