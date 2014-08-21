<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumProperties.ascx.cs"
    Inherits="SnitzUI.UserControls.Popups.ForumProperties" %>
<div id="divEditForum" style="font-size:smaller;line-height: 1em;" >
    <asp:HiddenField ID="hdnForumId" runat="server" />
    <asp:Panel ID="pnlMain" runat="server" GroupingText="<%$ Resources: webResources,lblForum %>">
        <asp:Label ID="Label3" runat="server" Text="Category" AssociatedControlID="ddlCat" EnableViewState="False"></asp:Label>
        <asp:DropDownList ID="ddlCat" runat="server"></asp:DropDownList>
        <br />
        <asp:Label ID="Label1" runat="server" Text="<%$ Resources: webResources,lblSubject %>" AssociatedControlID="tbxSubject"></asp:Label>
        <asp:TextBox ID="tbxSubject"  runat="server" EnableViewState="False"></asp:TextBox>
        <br />
        <asp:Panel runat="server" ID="pnlUrl">
            <asp:Label ID="lblUrl" runat="server" Text="<%$ Resources: webResources,lblHyperlink %>"></asp:Label><asp:TextBox ID="tbxUrl" runat="server"></asp:TextBox><br runat="server" ID="brUrl"/>
        </asp:Panel>
        <asp:Label ID="Label2" runat="server" Text="<%$ Resources: webResources,lblMessage %>" AssociatedControlID="tbxBody"></asp:Label>
        <asp:TextBox ID="tbxBody"  runat="server" Columns="20" Rows="3" TextMode="MultiLine" EnableViewState="False"></asp:TextBox>
    </asp:Panel>
    
    <asp:Panel ID="pnlOptions" runat="server" GroupingText="<%$ Resources: webResources,lblOptions %>">
        <asp:Label ID="Label4" runat="server" Text="Forum Order" AssociatedControlID="tbxSubject"></asp:Label>
        <asp:TextBox ID="tbxOrder" CssClass="testClass" runat="server" Width="20px" EnableViewState="False"></asp:TextBox>
        <br/>
        <asp:CheckBox ID="cbxAllowPolls" Text="Allow user polls" runat="server" />
        <asp:CheckBox ID="cbxBugReport" CssClass="cbx"  runat="server" Text="Bug Forum" />
        <asp:CheckBox ID="cbxBlogPosts" CssClass="cbx" runat="server" Text="Blog Forum" /><br />
        <asp:CheckBox ID="cbxCountPost" CssClass="cbx"  runat="server" Text="Increase user Post Count" /><br />
        <br />
        <asp:Label ID="lblMod" runat="server" Text="Moderation level" 
            EnableViewState="False"></asp:Label>
        <asp:DropDownList ID="ddlMod" runat="server" >
            <asp:ListItem Value="0">No Moderation for this forum</asp:ListItem>
            <asp:ListItem Value="1">All Posts Moderated</asp:ListItem>
            <asp:ListItem Value="2">Original Posts Only Moderated</asp:ListItem>
            <asp:ListItem Value="3">Replies Only Moderated</asp:ListItem>
        </asp:DropDownList>
        <br />
        <asp:Label ID="lblSub" runat="server" Text="Subscription Level" 
            EnableViewState="False"></asp:Label>
        <asp:DropDownList ID="ddlSub" runat="server" >
            <asp:ListItem Value="0">No Subscriptions Allowed</asp:ListItem>
            <asp:ListItem Value="1">Forum Subscriptions Allowed</asp:ListItem>
            <asp:ListItem Value="2">Topic Subscriptions Allowed</asp:ListItem>
        </asp:DropDownList>
        <br />
    </asp:Panel>
    <asp:Panel ID="pnlAuth" runat="server" GroupingText="Authorisation" style="width:99%">
        <asp:Label ID="lblPassword" runat="server" Text="Forum Password" AssociatedControlID="tbxPassword"></asp:Label>&nbsp;
        <asp:TextBox ID="tbxPassword" runat="server" Width="120px" ></asp:TextBox>
        <br />
        <asp:Panel ID="Panel3" runat="server" GroupingText="Moderators"
            Width="49%" Style="float: left;">
            <asp:HiddenField ID="hdnModerators" runat="server"/>
            <div class="ListViewContainer" id="modlist">
            <asp:ListView ID="ListView1" runat="server" EnableViewState="False">
                <LayoutTemplate>
                    <table cellpadding="0" cellspacing="0" id="modtbl">
                        <tr runat="server" id="itemPlaceholder"></tr>
                    </table>
                </LayoutTemplate>
                <EmptyItemTemplate>
                    <table id="modtbl" cellpadding="5" cellspacing="5">
                    <tr><td>No Moderators</td></tr>
                    </table>
                </EmptyItemTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <%# Eval("Name") %>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            </div>
            <asp:DropDownList ID="ddlModUsers" runat="server">
            </asp:DropDownList>
            <button type="button" onclick="UpdateModerator('<%= ddlModUsers.ClientID %>','<%= hdnModerators.ClientID %>');">Add Moderator</button>
            <%--<button type="button" onclick="UpdateModerator('<%= ddlModUsers.ClientID %>','<%= hdnModerators.ClientID %>','true');">Remove Moderator</button>--%>

        </asp:Panel>

        <asp:Panel ID="Panel4" runat="server" GroupingText="Allowed Roles"
            Width="49%" Style="float: left;">
            <asp:HiddenField ID="hdnRoleList" runat="server"/>
            <div class="ListViewContainer" id="rolelist">
            <asp:ListView ID="ListView2" runat="server" EnableViewState="False" >
                <LayoutTemplate>
                    <table cellpadding="0" cellspacing="0" id="roletbl">
                        <tr runat="server" id="itemPlaceholder">
                        </tr>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                          <%# Container.DataItem %>  
                        </td>
                    </tr>
                </ItemTemplate>
                <SelectedItemTemplate>
                    <tr>
                        <td style="background-color: blue;color: white;">
                            <%# Container.DataItem %>
                        </td>
                    </tr>
                </SelectedItemTemplate>
            </asp:ListView>
            </div>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="forumrole">
            </asp:DropDownList>
            <button type="button" onclick="UpdateRoleList('<%= ddlRole.ClientID %>','<%= hdnRoleList.ClientID %>');">Add Role</button>
            <button type="button" onclick="UpdateRoleList('<%= ddlRole.ClientID %>','<%= hdnRoleList.ClientID %>','true');">Remove Role</button>
        </asp:Panel>
        
        
        <br />
    </asp:Panel>
    <asp:Panel ID="Panel5" runat="server">
        <button onclick="SaveForum();return false;" type="button">
            <%= Resources.webResources.btnSubmit %></button>&nbsp;
        <button onclick="mainScreen.CancelModal();return false;" type="button">
                <%= Resources.webResources.btnCancel %></button>
    </asp:Panel>
</div>
