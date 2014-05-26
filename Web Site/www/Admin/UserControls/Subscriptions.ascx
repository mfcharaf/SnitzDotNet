<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscriptions.ascx.cs"
    Inherits="Admin_Subscriptions" %>
<asp:GridView ID="grdSubs" runat="server" AutoGenerateColumns="False" 
    CellPadding="2" OnRowDeleting="Rowdeleting"
    DataKeyNames="ForumId,TopicId,MemberId" EnableModelValidation="True" 
    OnRowCommand="grdSubs_RowCommand" CssClass="forumtable">
    <Columns>
        <asp:BoundField DataField="CategoryName" HeaderText="Category" />
        <asp:BoundField DataField="ForumSubject" HeaderText="Forum" HtmlEncode="False" />
        <asp:BoundField DataField="TopicSubject" HeaderText="Topic" HtmlEncode="False" />
        <asp:BoundField DataField="MemberName" HeaderText="User" />
        <asp:TemplateField ShowHeader="False">
            <ItemTemplate>
                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" CommandName="Delete"
                    SkinID="DeleteMessage" Text="Delete" Height="16px" Width="16px" />
            </ItemTemplate>
            <HeaderTemplate>
                <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False" CommandName="DeleteAll"
                    SkinID="DeleteMessage" ToolTip="Remove All" Text="Delete" Height="16px" Width="16px" />
            </HeaderTemplate>
            <HeaderStyle Width="20px" />
            <ControlStyle CssClass="msgIcons" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="category" />
</asp:GridView>
