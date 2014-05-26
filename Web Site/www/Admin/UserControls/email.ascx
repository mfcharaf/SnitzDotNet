<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_email" Codebehind="email.ascx.cs" %>
    <%@ Register src="AdminRadioButton.ascx" tagname="AdminRadioButton" tagprefix="uc1" %>
    <asp:Panel ID="Panel1" runat="server" CssClass="clearfix forumtable" DefaultButton="btnSubmit">
    <h2 class="category">Email Configuration</h2>
<asp:Panel ID="Panel4" runat="server" CssClass="emailform">
    <asp:Label ID="lblEmailMode" runat="server" Text="E-mail Mode" 
        AssociatedControlID="rblEmail" EnableViewState="False"></asp:Label>
    <asp:RadioButtonList ID="rblEmail" runat="server" RepeatDirection="Horizontal" 
        RepeatLayout="Flow" CssClass="cbx">
        <asp:ListItem Value="1">On</asp:ListItem>
        <asp:ListItem Value="0">Off</asp:ListItem>
    </asp:RadioButtonList>
    <br />
    <asp:Label ID="lblAdminEmail" runat="server" 
        Text="Administrator E-mail Address" AssociatedControlID="tbxAdminEmail"></asp:Label>
    <asp:TextBox ID="tbxAdminEmail" runat="server" Enabled="true" Width="50%"></asp:TextBox>
</asp:Panel>

<asp:Panel ID="Panel2" runat="server" GroupingText="Email Server Config" 
            CssClass="emailserverform">
    <asp:Label ID="lblServer" runat="server" Text="E-mail Server Address" 
        AssociatedControlID="tbxMailServer"></asp:Label>
    <asp:TextBox ID="tbxMailServer" runat="server" Columns="60"></asp:TextBox>
    <br />
    <asp:Label ID="Label2" runat="server" Text="Use SMTP Authentication" 
        AssociatedControlID="rblSMTPAuth"></asp:Label>

    <uc1:AdminRadioButton ID="rblSMTPAuth" runat="server" />
    <br />
    <asp:Label ID="Label3" runat="server" Text="SMTP Username" 
        AssociatedControlID="tbxMailUser"></asp:Label>
    <asp:TextBox ID="tbxMailUser" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="Label4" runat="server" Text="SMTP Password" 
        AssociatedControlID="tbxMailPwd"></asp:Label>
    <asp:TextBox ID="tbxMailPwd" runat="server"></asp:TextBox>
    <br />
    <br />
</asp:Panel>
<asp:Panel ID="Panel3" runat="server" GroupingText="Forum Mail Config" 
            CssClass="emailform">
    <asp:Label ID="lblUniqueEmail" runat="server" Text="Require Unique E-mail" 
        AssociatedControlID="rblUniqueEmail"></asp:Label>
    <uc1:AdminRadioButton ID="rblUniqueEmail" runat="server" />
    <br />
    <asp:Label ID="lblEmailVal" runat="server" Text="E-mail Validation" 
        AssociatedControlID="rblEmailVal"></asp:Label>
    <uc1:AdminRadioButton ID="rblEmailVal" runat="server" />
    <br />
    <asp:Label ID="lblRestrict" runat="server" Text="Restrict Registration" 
        AssociatedControlID="rblRestrictReg"></asp:Label>
    <uc1:AdminRadioButton ID="rblRestrictReg" runat="server" />
    <br />
    <asp:Label ID="lblRequireLogon" runat="server" 
        Text="Require Logon for sending Mail" 
        AssociatedControlID="rblLogonForEmail"></asp:Label>
    <uc1:AdminRadioButton ID="rblLogonForEmail" runat="server" />
    <br />
</asp:Panel>

<asp:Panel ID="Panel5" runat="server">
    <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />&nbsp;
    <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />
</asp:Panel>

    </asp:Panel>