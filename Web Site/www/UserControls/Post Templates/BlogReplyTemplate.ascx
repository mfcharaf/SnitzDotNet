<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogReplyTemplate.ascx.cs" Inherits="SnitzUI.UserControls.Post_Templates.BlogReplyTemplate" %>
<div class="blog-comments" style="display: none;">
    <asp:Panel  ID="PostPanel" runat="server">
        <div class="blogreply clearfix">
            <span><asp:PlaceHolder runat="server" ID="phAvatar"></asp:PlaceHolder> by&nbsp;<asp:Literal ID="litAuthor" runat="server" Text='' />&nbsp;<asp:Literal ID="litDate" runat="server" Text='' /></span><hr/>
            <div class="mContent bbcode">
                <asp:Literal ID="msgBody" runat="server" Text='' Mode="Encode"></asp:Literal>
            </div>
        </div>
    </asp:Panel>
</div>