<%@ Control Language="C#" AutoEventWireup="true"  Inherits="Admin_email" Codebehind="email.ascx.cs" %>
    <%@ Register src="AdminRadioButton.ascx" tagname="AdminRadioButton" tagprefix="uc1" %>
    <asp:Panel ID="Panel1" runat="server" CssClass="clearfix adminform" DefaultButton="btnSubmit">
    <h2 class="category">Email Configuration</h2>
        <div id="content">
<asp:Panel ID="Panel4" runat="server" CssClass="emailform">
    <br style="line-height: 0.1em;"/>
    <asp:Label ID="lblEmailMode" runat="server" Text="E-mail Mode" 
        AssociatedControlID="rblEmail" EnableViewState="False"></asp:Label>
    <uc1:AdminRadioButton ID="rblEmail" runat="server" />
    <br />
    <asp:Label ID="lblAdminEmail" runat="server" 
        Text="Administrator E-mail Address" AssociatedControlID="tbxAdminEmail"></asp:Label>
    <asp:TextBox ID="tbxAdminEmail" runat="server" Enabled="true" Width="50%"></asp:TextBox>
</asp:Panel>
<asp:Panel ID="Panel2" runat="server" GroupingText="Email Server Config" CssClass="emailserverform">
    <br style="line-height: 0.1em;"/>
    <asp:Label ID="lblServer" runat="server" Text="E-mail Server Address" 
        AssociatedControlID="tbxMailServer"></asp:Label>
    <asp:TextBox ID="tbxMailServer" runat="server" Columns="60"></asp:TextBox>
    <asp:Label ID="lblPort" runat="server" Text="Port" 
        AssociatedControlID="tbxPort"></asp:Label>
    <asp:TextBox ID="tbxPort" runat="server" Columns="2"></asp:TextBox>
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
    <asp:TextBox ID="tbxMailPwd" runat="server"  ></asp:TextBox>
    <br />
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
</div>
    </asp:Panel>