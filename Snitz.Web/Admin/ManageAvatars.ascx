<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_ManageAvatars" Codebehind="ManageAvatars.ascx.cs" %>
<asp:Label ID="Label2" runat="server" Text="Click on an Avatar to select it then use the delete button to remove the Avatar."></asp:Label><br />
<asp:DataList ID="Avatars" runat="server" RepeatColumns="6" RepeatDirection="Horizontal" OnItemCommand="Avatars_ItemCommand" CssClass="forumtable">
    <ItemTemplate>
        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl='<%# Eval("Path") %>' CommandName="select" CommandArgument='<%# Eval("Name") %>' />
        <br />
        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
    </ItemTemplate>
    <SelectedItemTemplate>
        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl='<%# Eval("Path") %>' CommandArgument='<%# Eval("Name") %>' BorderColor="Red" BorderWidth="2px" />
        <br />
        <asp:Button ID="Button1" runat="server" CommandArgument='<%# Eval("Name") %>' CommandName="delete"
            Text="Delete" />
    </SelectedItemTemplate>
</asp:DataList>
