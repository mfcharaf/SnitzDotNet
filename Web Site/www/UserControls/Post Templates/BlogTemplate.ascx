<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTemplate.ascx.cs" Inherits="SnitzUI.UserControls.Post_Templates.BlogTemplate" %>
<style type="text/css">
    #replyHeader {display: none;}
    .AspNet-FormView{ border: 0;}
    img.avatar{float: left; margin: 4px;}
    h1 { line-height: 0.5em;}
    .POFTop {margin-left: 90px;}   
    .social-media-share{ float: left;}
    .mContent{ min-height: 40px;}
    .QRBox{ width: 99%;margin-left: auto;margin-right: auto;margin-top: 4px;}
    .QRRight{ width: 99%;margin-top: 2px;}
    .table-bottom,.QRLeft{ display: none;}
    .table-bottom-footer{ width: 70%;margin-left: 160px;}
    .markItUpHeader{ display: none;}
    textarea.QRMsgArea{min-height: 50px !important;height: 60px;}


</style>
<asp:Panel  ID="PostPanel" runat="server">
    <div class="blog-date">
        <div class="blog-month"><asp:Label runat="server" ID="blgMonth"></asp:Label></div>
        <div class="blog-day"><span><b><asp:Label runat="server" ID="blgDay"></asp:Label></b></span></div>
    </div>
    <div class="blog-content">
        <div class="blogheader clearfix">
            <asp:PlaceHolder runat="server" ID="phAvatar"></asp:PlaceHolder>
            <h1><asp:Label runat="server" ID="lblSubject"></asp:Label></h1>
            <span style="font-size: smaller">posted by <asp:Literal ID="litAuthor" runat="server" Text='' />&nbsp;<asp:Literal ID="litDate" runat="server" Text='' />
            <br/><asp:Literal ID="litViews" runat="server" Text='' /></span>  
        </div>
        <div class="blogmessage">
            <div class="mContent bbcode">
                <asp:Literal ID="msgBody" runat="server" Text='' Mode="Encode"></asp:Literal>
            </div>
            <br />
            <div class="blog-buttons">&nbsp;<asp:ImageButton ID="hBookmark" SkinID="BookMarkBLog" runat="server"
            EnableViewState="False" Text="Bookmark Blog post" ToolTip="Bookmark Blog post"></asp:ImageButton>|
                <asp:HyperLink ID="hEmail" rel="nofollow" SkinID="EmailBlog" runat="server"
            EnableViewState="False" Text="Email link" ToolTip="Email link"></asp:HyperLink>|
                <asp:Image ID="comm" SkinID="BlogComments" runat="server" />&nbsp;<asp:HyperLink ID="hComments" rel="nofollow" runat="server"
            EnableViewState="False" Text="" ToolTip="<%$ Resources:webResources, lblComments %>" onclick="$('.blog-comments').css('display', 'block');return false;"></asp:HyperLink>
            </div>
            
        </div>
        <hr/>
        <h2>Comments<span style="float:right"></span></h2>
    </div>
</asp:Panel>
