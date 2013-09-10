<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_ManageRoles" CodeBehind="ManageRoles.ascx.cs" %>

<asp:MultiView ID="RoleView" runat="server">
<asp:View ID="viewNewRole" runat="server">
<asp:Panel ID="pnlNewRole" runat="server" CssClass="clearfix" DefaultButton="btnSubmitNew">
    <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
        <tr style="height:0px;padding:0px;"><th style="width:30%;height:0px;padding:0px;"></th><th style="height:0px;padding:0px;"></th></tr>
        <tr>
            <td colspan="2" class="category">
                <asp:Label ID="Label3" runat="server" Text="Add Role"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 30%;">
                <asp:Label runat="server" ID="Label4" Text="Role ID"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNewRoleID" runat="server" Width="15%" ValidationGroup="addrole"></asp:TextBox>&nbsp;
            </td>
        </tr>
        <tr>
            <td style="width: 30%;">
                <asp:Label runat="server" ID="Label5" Text="Role Name"></asp:Label>
            </td>
            <td >
                <asp:TextBox ID="txtNewName" runat="server" Width="55%" ValidationGroup="addrole"></asp:TextBox>&nbsp;
            </td>
        </tr>
        <tr>
            <td style="width: 30%;" valign="top">
                <asp:Label runat="server" ID="Label6" Text="Role Description"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="txtNewDescription" runat="server" Rows="2" TextMode="MultiLine"
                    Width="95%" ValidationGroup="addrole"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="text-align: center;">
                <asp:LinkButton ID="btnSubmitNew" runat="server" Text="Add" OnClick="btnNewSubmit_Click" ValidationGroup="addrole" />&nbsp;
                <asp:LinkButton ID="Button4" runat="server" Text="Cancel" OnClick="btnNewReset_Click" ValidationGroup="addrole"
                    CausesValidation="False" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="Label7" runat="server" Visible="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" SetFocusOnError="true" Display="Dynamic"
        ControlToValidate="txtNewRoleID" Text="&nbsp;" ErrorMessage="You need to specify a Role Id value"></asp:RequiredFieldValidator>
<asp:RangeValidator ID="RangeValidator1" runat="server" SetFocusOnError="true" Type="Integer" Display="Dynamic"
    ControlToValidate="txtNewRoleID" Text="&nbsp;" MinimumValue="5" MaximumValue="98"
    ErrorMessage="The value for Role Id must be between 5 and 98" ValidationGroup="addrole"></asp:RangeValidator>
<asp:RequiredFieldValidator runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtNewName"
    SetFocusOnError="true" Text="&nbsp;" ErrorMessage="You need to specify a Role Name" Display="Dynamic"
    ValidationGroup="addrole"></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtNewName" Display="Dynamic"
    runat="server" Text="&nbsp;" ErrorMessage="Please use a minimum of 3 and a maximum of 256 letters, no spaces, starting with an uppercase letter, for Role Name"
    ValidationExpression="[A-Z][A-Za-z]{2,255}" ValidationGroup="addrole"></asp:RegularExpressionValidator>
<asp:ValidationSummary ID="ValidationSummary1" ShowSummary="false"
    DisplayMode="List" runat="server" ValidationGroup="addrole" />
</asp:Panel>
</asp:View>
<asp:View ID="viewEditRole" runat="server">
<asp:Panel ID="pnlEditRole" runat="server" CssClass="clearfix" Width="100%" DefaultButton="btnSubmit">
    <table border="0" cellpadding="2" cellspacing="0" class="forumtable white" style="width: 100%;">
            <tr>
            <td style="width: 30%;padding:0px">
            </td>
            <td style="padding:0px">
            </td>
        </tr>
        <tr>
            <td colspan="2" class="category">
                <asp:Label ID="Label2" runat="server" Text="Manage Role"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right">
            </td>
            <td style="text-align: left">
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                <asp:Label runat="server" ID="roleLbl" Text="Role ID"></asp:Label>
            </td>
            <td style="text-align: left">
                <asp:TextBox ID="txtRoleID" runat="server" Enabled="false" Columns="3" 
                    ValidationGroup="editrole"></asp:TextBox>&nbsp;
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right;">
                <asp:Label runat="server" ID="nameLbl" Text="Role Name"></asp:Label>
            </td>
            <td style="text-align: left">
                <asp:TextBox ID="txtName" runat="server" Width="55%" ValidationGroup="editrole"></asp:TextBox>&nbsp;
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right" valign="top">
                <asp:Label runat="server" ID="descLabel" Text="Role Description"></asp:Label>
            </td>
            <td style="text-align: left">
                <asp:TextBox ID="txtDescription" runat="server" Rows="3" Width="95%" 
                    ValidationGroup="editrole"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="width: 30%; text-align: right" valign="top">
            </td>
            <td style="text-align: left">
                <asp:LinkButton ID="btnSubmit" runat="server" OnClick="btnSubmit_Click" Text="<%$ Resources:webResources,btnSubmit %>" ValidationGroup="editrole"/>&nbsp;<asp:LinkButton
                    ID="btnReset" runat="server" OnClick="btnReset_Click" CausesValidation="False" Text="<%$ Resources:webResources,btnCancel %>" />
            </td>
        </tr>
        <asp:Panel ID="UserListPanel" runat="server" CssClass="clearfix" Visible="false"
            Width="100%">
            <tr>
                <td colspan="2" style="text-align: right" valign="top">
                    <hr />
                </td>
            </tr>
            <tr>
                <td style="width: 30%; text-align: right" valign="top">
                    &nbsp;
                </td>
                <td style="text-align: left">
                    <asp:Label runat="server" ID="UsersInRole" Text=""></asp:Label>&nbsp;<br />
                    <asp:LinkButton ID="Button1" runat="server" OnClick="Button1_Click" Text="View users in Role" CausesValidation="False" ValidationGroup="editrole" /><br />
                    <asp:Panel ID="Panel3" runat="server" Width="200px">
                        <asp:ListBox ID="UserList" runat="server" Width="100%" Rows="10"></asp:ListBox>
                    </asp:Panel>
                    <asp:LinkButton ID="Button2" runat="server" OnClick="Button2_Click" Text="Remove selected user from Role" ValidationGroup="editrole" />
                </td>
            </tr>
            <tr>
                <td style="width: 30%; text-align: right" valign="top">
                </td>
                <td style="text-align: left">
                    <asp:TextBox ID="NewUserForRole" runat="server" ValidationGroup="editrole"></asp:TextBox>
                    <asp:LinkButton ID="NewUsername" runat="server" OnClick="NewUsername_Click" Text="Add user to Role" ValidationGroup="editrole" />
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    &nbsp;
                </td>
            </tr>
        </asp:Panel>
                <tr>
            <td colspan="2" class="tableheader">
                <asp:Label ID="errLbl" runat="server" Visible="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
    <asp:RequiredFieldValidator ID="ReqRoleID" runat="server" SetFocusOnError="true"
        ControlToValidate="txtRoleID" Text="&nbsp;" ErrorMessage="You need to specify a Role Id value" ValidationGroup="editrole"></asp:RequiredFieldValidator>

<asp:RangeValidator ID="RoleIDVal" runat="server" SetFocusOnError="true" Type="Integer"
    ControlToValidate="txtRoleID" Text="&nbsp;" MinimumValue="1" MaximumValue="99"
    ErrorMessage="The value for Role Id must be between 5 and 98" ValidationGroup="editrole"></asp:RangeValidator>
<asp:RequiredFieldValidator runat="server" ID="NameIDReq" ControlToValidate="txtName"
    SetFocusOnError="true" Text="&nbsp;" ErrorMessage="You need to specify a Role Name" ValidationGroup="editrole"></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="NameIDVal" ControlToValidate="txtName" runat="server"
    Text="&nbsp;" ErrorMessage="Please use a minimum of 3 and a maximum of 256 letters, no spaces, starting with an uppercase letter, for Role Name"
    ValidationExpression="[A-Z][A-Za-z]{2,255}" ValidationGroup="editrole"></asp:RegularExpressionValidator>
<asp:ValidationSummary ID="ValidationSummary" ShowMessageBox="true" ShowSummary="false"
    DisplayMode="List" runat="server" ValidationGroup="editrole" />
</asp:Panel>
</asp:View>
</asp:MultiView>

<br />


<asp:Panel ID="Panel1" runat="server" CssClass="ConfigForm clearfix" Style="width: 100%;" Wrap="False">
    <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="forumtable">
        <tr>
            <td class="category">
                <asp:Label ID="Label1" runat="server" Text="Role Manager"></asp:Label>
            </td>
            <td class="category" align="right">
                <asp:CheckBox ID="delPopRoles" runat="server" Text="Delete roles with members&nbsp;"  Checked="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="errLbl2" runat="server" Visible="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAllRolesFull"
                    TypeName="Snitz.Providers.SnitzRoleProvider"></asp:ObjectDataSource>
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                    CssClass="TopicsTable" Width="100%" AllowPaging="True" EnableViewState="False"
                    OnRowCommand="GridView1_RowCommand" DataSourceID="ObjectDataSource1" EnableModelValidation="True">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Role ID" HeaderStyle-HorizontalAlign="Center"
                            ShowHeader="true">
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="RoleName" HeaderStyle-HorizontalAlign="Center" HeaderText="Name"
                            ShowHeader="true">
                            <ItemStyle VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Description" HeaderStyle-HorizontalAlign="Center" HeaderText="Description"
                            ShowHeader="true">
                            <ItemStyle VerticalAlign="Top" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="editBtn" ImageUrl="~/Admin/images/Write.png" runat="server"
                                    AlternateText="Edit Role" CommandName="EditClick" CommandArgument='<%#Eval("Id") %>'
                                    ImageAlign="AbsMiddle" CausesValidation="false" />
                            </ItemTemplate>
                            <ItemStyle Width="20px" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="delBtn" ImageUrl="~/Admin/images/trash.png" runat="server" AlternateText="Delete Role"
                                    CommandName="DeleteClick" CommandArgument='<%#Eval("RoleName") %>' ImageAlign="AbsMiddle"
                                    CausesValidation="false" />
                            </ItemTemplate>
                            <ItemStyle Width="20px" />
                        </asp:TemplateField>
                    </Columns>
                    <HeaderStyle CssClass="tableheader" />
                    <EmptyDataTemplate>
                        No Roles defined
                    </EmptyDataTemplate>
                    <SelectedRowStyle BackColor="#FFE0C0" />
                    <PagerSettings Mode="NumericFirstLast" />
                    <PagerStyle CssClass="NoBorder" />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Panel>
