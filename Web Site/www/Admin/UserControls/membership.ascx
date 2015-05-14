<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_Membership" Codebehind="Membership.ascx.cs" %>
<%@ Register Src="~/UserControls/MemberSearch.ascx" TagName="MemberSearch" TagPrefix="uc1" %>
<uc1:MemberSearch ID="ucSearch" runat="server" />
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
    Width="100%" DataKeyNames="UserName" OnRowDataBound="BindGrid"
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
            <HeaderStyle Width="20px" />
        </asp:TemplateField>
        <asp:BoundField DataField="UserName" HeaderText="Username" ReadOnly="True" SortExpression="UserName" />
        <asp:TemplateField HeaderText="Title" SortExpression="Title">
            <ItemTemplate>
                <asp:Label runat="server" ID="RankTitle"></asp:Label><br/>
                <asp:Literal runat="server" ID="RankStars"></asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Posts" SortExpression="Posts">
            <ItemTemplate>
                <%# Eval("posts") %>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <HeaderStyle Width="50px"></HeaderStyle>
        </asp:TemplateField>
        <asp:BoundField DataField="LastPostDate" HeaderText="Last Post" 
            SortExpression="LastPostDate" HtmlEncode="False" 
            DataFormatString="{0:dd MMM yyyy}">
            <ItemStyle HorizontalAlign="Center" />
            <HeaderStyle Width="60px"></HeaderStyle>
        </asp:BoundField>
        <asp:BoundField DataField="LastLoginDate" HeaderText="Last Login" 
            SortExpression="LastLoginDate" HtmlEncode="False" 
            DataFormatString="{0:dd MMM yyyy&lt;br/&gt;HH:mm:ss}" >
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:BoundField DataField="CreationDate" HeaderText="Joined" ReadOnly="True" SortExpression="CreationDate" HtmlEncode="False" DataFormatString="{0:dd MMM yyyy}" >
            <ItemStyle HorizontalAlign="Center" />
            <HeaderStyle Width="60px"></HeaderStyle>
        </asp:BoundField>
        <asp:BoundField DataField="Country" HeaderText="Country" SortExpression="Country" >
            <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>
        <asp:TemplateField>
            <ItemTemplate>
                        <asp:HyperLink ID="hypUserEdit" SkinID="UserEdit" Text="<%$ Resources:webResources, lblEditUser %>" runat="server" EnableViewState="False" NavigateUrl='<%# "~/Account/profile.aspx?edit=Y&user=" + Eval("Username") %>' rel="NoFollow"></asp:HyperLink>
                        <asp:ImageButton ID="lockUser" SkinID="UserLock" CommandArgument='<%# Eval("Username")%>' CommandName="lock"
                                runat="server" ToolTip="<%$ Resources:webResources, lblLockUser %>" OnClientClick=""
                                CausesValidation="False" EnableViewState="False"/>
                        <asp:ImageButton ID="unlockUser" SkinID="UserUnlock" CommandArgument='<%# Eval("Username")%>' CommandName="lock"
                                runat="server" ToolTip="<%$ Resources:webResources, lblUnLockUser %>" OnClientClick=""
                                CausesValidation="False" EnableViewState="False"/>
                        <asp:ImageButton ID="delUser" SkinID="UserDelete" CommandArgument='<%# Eval("Username")%>' CommandName="lock"
                                runat="server" ToolTip="<%$ Resources:webResources, lblDeleteUser %>" OnClientClick=""
                                CausesValidation="False" EnableViewState="False"/>
            </ItemTemplate>

            <HeaderStyle Width="20px" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="tableheader" />
    <FooterStyle CssClass="tableheader" />
    <PagerStyle CssClass="tableheader" />
    <RowStyle CssClass="row" />
    <AlternatingRowStyle CssClass="altrow" />
</asp:GridView>

