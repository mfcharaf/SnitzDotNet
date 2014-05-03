<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_NewRole" Codebehind="NewRole.ascx.cs" %>
<asp:Panel ID="pnlNewRole" runat="server" CssClass="clearfix">
        <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
            
            <tr>
                <td colspan="2" class="category">
                    <asp:Label ID="Label1" runat="server" Text="Role Manager"></asp:Label>
                </td>
            </tr>
             
            <tr>
                <td style="width: 30%;text-align:right;">
                <asp:Label runat="server" ID="roleLbl" Text="Role ID"></asp:Label></td><td style="text-align:left">
                    <asp:TextBox ID="txtRoleID" runat="server"  Width="15%" ></asp:TextBox>&nbsp;</td>
            </tr> 
            <tr>
                <td style="width: 30%;text-align:right;">
                <asp:Label runat="server" ID="nameLbl" Text="Role Name"></asp:Label></td><td style="text-align:left">
                    <asp:TextBox ID="txtName" runat="server"  Width="55%" ></asp:TextBox>&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 30%;text-align:right" valign="top">
                    <asp:Label runat="server" ID="descLabel" Text="Role Description"></asp:Label></td><td style="text-align:left">
                    <asp:TextBox ID="txtDescription" runat="server" Rows="2" TextMode="MultiLine" Width="95%"></asp:TextBox></td>
            </tr>                    
            <tr>
                <td colspan="2" style="text-align:center;">
                    <asp:LinkButton ID="btnSubmit" runat="server" Text="Add" OnClick="btnSubmit_Click" />&nbsp;
                    <asp:LinkButton ID="btnReset" runat="server" Text="Cancel" OnClick="btnReset_Click" CausesValidation="False" />
                </td>
            </tr>
            <tr>
                <td colspan="2" >
                    <asp:Label ID="errLbl" runat="server" Visible="False" ForeColor="Red"></asp:Label>
                </td>
            </tr>
</table>
    <asp:RequiredFieldValidator ID="ReqRoleID" runat="server"   SetFocusOnError="true" ControlToValidate="txtRoleID" Text="&nbsp" ErrorMessage="You need to specify a Role Id value"></asp:RequiredFieldValidator></asp:Panel>
    <asp:RangeValidator ID="RoleIDVal" runat="server"   SetFocusOnError="true"  Type="Integer" ControlToValidate="txtRoleID"  Text="&nbsp" MinimumValue="5" MaximumValue="98" ErrorMessage="The value for Role Id must be between 5 and 98"></asp:RangeValidator>
    <asp:RequiredFieldValidator runat="server" ID="NameIDReq" ControlToValidate="txtName" SetFocusOnError="true" Text="&nbsp" ErrorMessage="You need to specify a Role Name"></asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="NameIDVal"  ControlToValidate="txtName" runat="server" Text="&nbsp" ErrorMessage="Please use a minimum of 3 and a maximum of 256 letters, no spaces, starting with an uppercase letter, for Role Name" ValidationExpression="[A-Z][A-Za-z]{2,255}"></asp:RegularExpressionValidator>
    <asp:ValidationSummary ID="ValidationSummary"    ShowMessageBox="true"  ShowSummary="false" DisplayMode="List" runat="server" />
   