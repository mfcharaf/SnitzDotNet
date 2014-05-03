<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_PendingMembers" Codebehind="PendingMembers.ascx.cs" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Import Namespace="SnitzCommon" %>
<table border="0" cellpadding="4" cellspacing="1" width="100%" class="forumtable white" style="margin:auto;">
    <tr>
        <td colspan="2" >
            <h2 class="category">Administrator Options</h2></td>
    </tr>
    <tr>
        <td align="center">
            <asp:LinkButton ID="approveAll" runat="server" Text="Approve All Pending Members" Width="250px" OnClick="approveAll_Click" style="float:right"/><br /><br />
            <asp:LinkButton ID="approveSel" runat="server" Text="Approve Selected Pending Members" Width="250px" OnClick="approveSel_Click" style="float:right"/></td>
        <td align="center">
            <asp:LinkButton ID="delAll" runat="server" Text="Delete All Pending Members" Width="250px" OnClick="delAll_Click" /><br /><br />
            <asp:LinkButton ID="delSel" runat="server" Text="Delete Selected Pending Members" Width="250px" OnClick="delSel_Click" />
        </td>
    </tr>
</table>
<br />
<asp:GridView Width="100%" ID="GridViewMemberUser" runat="server" 
DataSourceID="ObjectDataSourceMembershipUser"
CssClass="forumtable white" AutoGenerateColumns="False" 
DataKeyNames="Id" EmptyDataText="No pending registrations" AllowPaging="True">
    <Columns>
        <asp:BoundField DataField="Id" HeaderText="ID" ShowHeader="False" Visible="False" />
        <asp:TemplateField HeaderText="Username/Email">
            <ItemTemplate>
                <asp:HyperLink ID="hypProf" runat="server" NavigateUrl='<%# Bind("ProfileLink") %>' Text='<%# Bind("Username") %>'></asp:HyperLink>
                <br />
                <asp:Label ID="Label4" runat="server" Text='<%# Config.Encrypt ? Cryptos.CryptosUtilities.Decrypt(Eval("Email").ToString()) : Eval("Email") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Date">
            <ItemTemplate>
                <asp:Label ID="Label2" runat="server" Text='<%# Eval("MemberSince").ToString().ToDateTime() %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="80px" />
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Email Validated">
            <EditItemTemplate>
                <asp:TextBox ID="email" runat="server" Text='<%# Eval("IsValid") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="Lemail" runat="server" Text='<%# (Eval("IsValid").ToString() == "1") ? "Yes" : "No" %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="60px" />
        </asp:TemplateField>
        <asp:BoundField DataField="Age" HeaderText="Age" HeaderStyle-Width="50px" />
        <asp:BoundField DataField="FirstName" HeaderText="Firstname" />
        <asp:BoundField DataField="LastName" HeaderText="Lastname" />
        <asp:TemplateField HeaderText="IP">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# Bind("LastIP") %>'></asp:Label><br />
                <asp:HyperLink ID="spamcheck" runat="server" Text="Spam Check" NavigateUrl='<%# string.Format("http://www.stopforumspam.com/ipcheck/{0}",Eval("LastIP")) %>'></asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server" ToolTip='<%# Eval("Username") %>' />
            </ItemTemplate>
            <HeaderStyle Width="20px" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="tableheader" />
</asp:GridView>

<asp:ObjectDataSource ID="ObjectDataSourceMembershipUser" runat="server" 
    SelectMethod="GetPendingMembers" TypeName="Snitz.BLL.Admin" 
    OldValuesParameterFormatString="original_{0}" 
    EnablePaging="True"
    MaximumRowsParameterName="maxRecords" 
    StartRowIndexParameterName="startRecord" 
    SelectCountMethod="GetPendingMemberCount">

</asp:ObjectDataSource>