<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_Membership" Codebehind="Membership.ascx.cs" %>
<%@ Register Src="~/UserControls/MemberSearch.ascx" TagName="MemberSearch" TagPrefix="uc1" %>
<uc1:MemberSearch ID="MemberSearch1" runat="server" />
<br />
<br />
  Number of Users Online: <asp:Label id="UsersOnlineLabel" runat="Server" /><br />

  <asp:Panel id="NavigationPanel" Visible="false" runat="server">
    <table border="0" cellpadding="3" cellspacing="3">
      <tr>
        <td style="width:100px">Page <asp:Label id="CurrentPageLabel" runat="server" />
            of <asp:Label id="TotalPagesLabel" runat="server" /></td>
        <td style="width:60px"><asp:LinkButton id="PreviousButton" Text="< Prev"
                            OnClick="PreviousButton_OnClick" runat="server" 
                CausesValidation="False" /></td>
        <td style="width:60px"><asp:LinkButton id="NextButton" Text="Next >"
                            OnClick="NextButton_OnClick" runat="server" 
                CausesValidation="False" /></td>
      </tr>
    </table>
  </asp:Panel>

<asp:GridView CssClass="forumtable white" CellPadding="3" ID="UserGrid" 
    runat="server" AutoGenerateColumns="False" 
    Width="100%" DataKeyNames="UserName"
    EnableModelValidation="True">
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:HyperLink runat="server" ID="urlProfile" NavigateUrl='<%# "~/Account/profile.aspx?user=" + Eval("UserName") %>' ToolTip='<%# Eval("UserName") %>' EnableViewState="False" rel="nofollow">
                <asp:Image ID="Image1" runat="server" ImageUrl='<%# (bool)Eval("IsActive") ? "~/Admin/images/User-online.png" : "~/Admin/images/User-offline.png" %>' Visible='<%# !(bool)Eval("IsLockedOut") %>' />
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Admin/images/lock.png" Visible='<%# (bool)Eval("IsLockedOut") %>' />
                </asp:HyperLink>
            </ItemTemplate>
            <ItemStyle CssClass="membericon" />
            <HeaderStyle Width="30px" />
        </asp:TemplateField>
        <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True" SortExpression="UserName" />
        <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title"  >
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:TemplateField HeaderText="Posts" SortExpression="Posts">
            <ItemTemplate>
                <%# Eval("posts") %>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:BoundField DataField="LastPostDate" HeaderText="Last Post" 
            SortExpression="LastPostDate" HtmlEncode="False" 
            DataFormatString="{0:dd MMM yyyy&lt;br/&gt;HH:mm:ss}">
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login" 
            SortExpression="LastLoginDate" HtmlEncode="False" 
            DataFormatString="{0:dd MMM yyyy&lt;br/&gt;HH:mm:ss}" >
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:BoundField DataField="LastActivityDate" HtmlEncode="False" 
            DataFormatString="{0:dd MMM yyyy&lt;br/&gt;HH:mm:ss}" HeaderText="Last Visit" />
        <asp:BoundField DataField="CreationDate" HeaderText="Joined" ReadOnly="True" SortExpression="CreationDate" HtmlEncode="False" DataFormatString="{0:dd MMM yyyy}" >
            <ItemStyle CssClass="nowrap" HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" >
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:HyperLink ID="hypUserLock" NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"../pop_lock.aspx?lock=1&mode=M&ID={0}\")",Eval("UserName")) %>' SkinID="UserUnLock" Text='<%# String.Format(Resources.webResources.lblLockUser,Eval("UserName")) %>' runat="server" EnableViewState="False" Visible='<%# (!(bool)Eval("IsLockedOut") && Roles.IsUserInRole("Administrator")) %>' rel="NoFollow"></asp:HyperLink>
                <asp:HyperLink ID="hypUserUnlock" NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"../pop_lock.aspx?lock=0&mode=M&ID={0}\")",Eval("UserName")) %>' SkinID="Userlock" Text='<%# String.Format(Resources.webResources.lblUnlockUser,Eval("UserName")) %>' runat="server" EnableViewState="False" Visible='<%# ((bool)Eval("IsLockedOut") && Roles.IsUserInRole("Administrator")) %>' rel="NoFollow"></asp:HyperLink>
                <asp:HyperLink ID="hypUserEdit" SkinID="UserEdit" Text="<%$ Resources:webResources, lblEditUser %>" runat="server" EnableViewState="False" NavigateUrl='<%# "~/Account/profile.aspx?edit=Y&user=" + Eval("UserName") %>' Visible='<%# Roles.IsUserInRole("Administrator") %>' rel="NoFollow"></asp:HyperLink>
                <asp:HyperLink ID="hypUserDelete" NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"../pop_delete.aspx?lock=1&mode=M&ID={0}\")",Eval("UserName")) %>' SkinID="UserDelete" Text="<%$ Resources:webResources, lblDeleteUser %>" runat="server" EnableViewState="False" Visible='<%# Roles.IsUserInRole("Administrator") %>' rel="NoFollow"></asp:HyperLink>
            </ItemTemplate>

            <HeaderStyle Width="80px" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="tableheader" />
    <FooterStyle CssClass="tableheader" />
    <PagerStyle CssClass="tableheader" />
    <RowStyle CssClass="row" />
    <AlternatingRowStyle CssClass="altrow" />
</asp:GridView>

