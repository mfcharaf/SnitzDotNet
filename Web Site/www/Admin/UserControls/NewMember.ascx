<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_NewMember" Codebehind="NewMember.ascx.cs" %>
<asp:Panel ID="PanelCreateUser" runat="server" CssClass="forumtable featuresform clearfix" style="width:100%;">
<h2 class="category">Create New User</h2>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
        <div style="width:100%;margin:auto;padding:3px;">
                <asp:Label ID="Label3" runat="server" Text="UserName" AssociatedControlID="tbxUserName"></asp:Label>
                <asp:TextBox ID="tbxUserName" runat="server"></asp:TextBox>
            <br />
                <asp:Label ID="Label4" runat="server" Text="Password" AssociatedControlID="tbxPassword"></asp:Label>
                <asp:TextBox ID="tbxPassword" runat="server"></asp:TextBox>
                <asp:CheckBox ID="cbxAutoFill" runat="server" Text="auto fill" 
                    AutoPostBack="True" oncheckedchanged="cbxAutoFill_CheckedChanged" />
            <br />
                <asp:Label ID="Label2" runat="server" Text="Email" AssociatedControlID="tbxEmail"></asp:Label>
                <asp:TextBox ID="tbxEmail" runat="server" Width="200px"></asp:TextBox>
            <br />
                <asp:Label ID="Label9" runat="server" Text="Approved" AssociatedControlID="cbxApproval"></asp:Label>
                <asp:CheckBox ID="cbxApproval" runat="server" />
            <br />
            <div style="vertical-align: top;">
                <asp:Label ID="Label1" runat="server" Text="Roles" AssociatedControlID="roles" style="vertical-align: top;"></asp:Label>
                <asp:ListBox ID="roles" runat="server" SelectionMode="Multiple"></asp:ListBox></div>
            <br />
            <asp:CheckBox ID="cbxSendEmail" runat="server" Text="Send new user an email" />
            <br />
                <asp:LinkButton ID="ButtonNewUser" runat="server" OnClick="ButtonNewUser_Click" Text="Create New User" />
            <br />
            <asp:Label ID="LabelInsertMessage" runat="server"></asp:Label>
        </div>
    
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
