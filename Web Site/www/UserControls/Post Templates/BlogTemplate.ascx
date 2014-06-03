<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogTemplate.ascx.cs" Inherits="SnitzUI.UserControls.Post_Templates.BlogTemplate" %>
<style type="text/css">
    #replyHeader {
        display: none;
    }
    .AspNet-FormView{ border: 0px;}
    img.avatar{float: left; margin: 4px;}
    h1 { line-height: 0.5em;}
    .POFTop{margin-left: 90px;}
    .social-media-share{ float: left;}
    .mContent{ min-height: 40px;}
    .QRBox{ width: 99%;margin-left: auto;margin-right: auto;margin-top: 4px;}
    .QRRight{ width: 99%;margin-top: 2px;}
    .table-bottom,.QRLeft{ display: none;}
    .table-bottom-footer{ width: 60%;margin-left: auto;margin-right: 0px;}
    .markItUpHeader{ display: none;}
    .POFTop .btnTopic{ display: none;}
    .POFTop .btnReply{ display: none;}
    .POFTop .btnPrint{ display: none;}
    textarea.QRMsgArea{min-height: 50px !important;height: 60px;}
    .blogList{background-color: whitesmoke;border: 1px solid #ddd;border-radius:5px;padding:4px;margin-top:14px;max-height: 400px;}
    .blog-date{ width: 60px;height: 70px;background-color: white;float: left;text-align: center;border-radius: 4px;border: 1px solid silver;box-shadow: 3px 3px 4px #000;}
    .blog-day{ color-rendering: black;height: 50px;vertical-align: middle;font-size: x-large;}
    .blog-day span { display:inline-block;vertical-align: middle;line-height: 50px; }
    .blog-month{ width: 100%;background-color: silver;color: white;height: 20px;border-top-left-radius: 4px;border-top-right-radius: 4px;}
    .blog-content{margin-left: 80px;}
    .blogheader{background-color: #eee;border:1px solid #bbb;width:99%;margin-bottom:3px;font-size: smaller;border-radius:5px}
    .blogmessage{background-color: whitesmoke;border:1px solid #bbb;min-height:100px;width:99%;border-top-left-radius: 7px;border-bottom-right-radius: 7px}
    .blogreply{background-color: #fafafa;border:1px solid #ddd;min-height:40px;width:70%;margin-left: 160px;margin-bottom: 3px;}
    .blogreply span{ font-size: x-small;}
    .blog-buttons{
    display:table-cell;
    text-align:center;
    vertical-align:middle;
        color: #bbb;
    }
    .blog-buttons img{ vertical-align: middle;}
    .blog-buttons input{ vertical-align: middle;}
    .blog-content h2{ vertical-align: middle;}
    .blog-content h2 span img{ vertical-align: middle;}
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
            EnableViewState="False" Text="" ToolTip="Comments" onclick="$('.blog-comments').css('display', 'block');return false;"></asp:HyperLink>
            </div>
            
        </div>
        <hr/>
        <h2>Comments<span style="float:right"></span></h2>
    </div>
</asp:Panel>
