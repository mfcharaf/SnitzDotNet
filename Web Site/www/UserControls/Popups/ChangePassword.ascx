<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.ascx.cs" Inherits="SnitzUI.UserControls.Popups.ChangePassword" %>
    <asp:ChangePassword ID="ChangePassword1" runat="server" MembershipProvider="SnitzMembershipProvider"
        NewPasswordRegularExpression=".{8,}" PasswordLabelText="Existing Password:" OnChangePasswordError="ChangePassword1ChangePasswordError" >
        <ChangePasswordTemplate>
                        <div class="formLayout">
                                    <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Existing Password:</asp:Label>
                                    <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ChangePassword1"><asp:Image ID="Image1" runat="server" SkinID="Error" /></asp:RequiredFieldValidator><br />
                                    <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                                    <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                                        ErrorMessage="New Password is required." ToolTip="New Password is required."
                                        ValidationGroup="ChangePassword1"><asp:Image ID="Image2" runat="server" SkinID="Error" /></asp:RequiredFieldValidator><br />
                                    <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                    <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
                                        ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
                                        ValidationGroup="ChangePassword1"><asp:Image ID="Image3" runat="server" SkinID="Error" /></asp:RequiredFieldValidator><br />

                                    <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
                                        ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
                                        ValidationGroup="ChangePassword1"><asp:Image ID="Image4" runat="server" SkinID="Error" /></asp:CompareValidator>
                                    <asp:RegularExpressionValidator ID="NewPasswordRegExp" runat="server" ControlToValidate="NewPassword"
                                        Display="Dynamic" ErrorMessage="Password is too short,Please enter a different password containing a minimum of 8 characters." ValidationExpression=".{8,}"
                                        ValidationGroup="ChangePassword1"><asp:Image ID="Image5" runat="server" SkinID="Error" /></asp:RegularExpressionValidator>

                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                    <br />
                                    <div class="clearfix" style="width:80%;margin:auto;">
                                    <div style="float:left">
                                    <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
                                        Text="Change Password" ValidationGroup="ChangePassword1" />
                                        </div>
                                        <div style="float:right">
                                    <asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel" OnClientClick="CancelModal()" />
                                        </div>
                                        </div>
                        </div>
        </ChangePasswordTemplate>
        <SuccessTemplate>
            <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse">
                <tr>
                    <td>
                        <table border="0" cellpadding="0">
                            <tr>
                                <td align="center" colspan="2">
                                    Change Password Complete</td>
                            </tr>
                            <tr>
                                <td>
                                    Your password has been changed!</td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2">
                                    <asp:Button ID="ContinuePushButton" runat="server" CausesValidation="False" CommandName="Continue"
                                        OnClientClick="ResetDone()" Text="Continue" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </SuccessTemplate>
    </asp:ChangePassword>