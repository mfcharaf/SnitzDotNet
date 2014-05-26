<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordRetrieve.ascx.cs" Inherits="SnitzUI.UserControls.Popups.PasswordRetrieve" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
    <asp:PasswordRecovery ID="recover" runat="server" OnSendingMail="RecoverSendingMail" MembershipProvider="SnitzMembershipProvider">
        <MailDefinition>
        </MailDefinition>
        <UserNameTemplate>
            <div class="formLayout">
                <asp:Literal ID="L1" runat="server" Text="<%$ Resources: extras, lblForgottenPassword %>"></asp:Literal>
                <asp:Literal ID="L2" runat="server" Text="<%$ Resources: extras, lblRetrievePasswordText %>"></asp:Literal>
                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="<%$ Resources: webResources, lblUserName %>"></asp:Label>
                <asp:TextBox ID="UserName" runat="server"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                    ErrorMessage="<%$ Resources:webResources, ErrNoUsername %>" ToolTip="<%$ Resources:webResources, ErrNoUsername %>"
                    ValidationGroup="recover">
                    <asp:Image ID="Image9" runat="server" SkinID="Error" /></asp:RequiredFieldValidator>
                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="<%$ Resources:webResources, btnSubmit %>"
                    ValidationGroup="recover" /><br />
            </div>
        </UserNameTemplate>
    </asp:PasswordRecovery>
    </ContentTemplate>
</asp:UpdatePanel>    
