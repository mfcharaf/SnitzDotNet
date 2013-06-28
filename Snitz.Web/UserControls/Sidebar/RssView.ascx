<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssView.ascx.cs" Inherits="SnitzUI.UserControls.RssView" EnableViewState="false" %>

<div class="sideBox">
    
    <asp:Repeater ID="MyRSSBlog" DataSourceID="RSSxmlSource" runat="server" EnableViewState="False">
        <HeaderTemplate>
            <div class="category">
                <span class="cattitle">Snitz Forums 2000</span>
            </div>
            <div class="innertube">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="max-width: 300px; overflow: hidden">
                 <p><asp:HyperLink ID="myLink" runat="server" Target="_blank" NavigateUrl='<%# XPath("link") %>' EnableViewState="False">        
                <h3><%# XPath("title") %></h3></asp:HyperLink>
                <i class="smallText">Published on <%# XPath("pubDate") %></i><br/>
                <div style="max-height: 120px; overflow: hidden;padding:5px;">
                    <%# XPath("description") %>
                </div>
                <i>Category: <em style="color: #8c0017"><%# XPath("category")%></em></i></p>
            </div>
        </ItemTemplate>
        <SeparatorTemplate>
            <hr />
        </SeparatorTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>
    <asp:XmlDataSource ID="RSSxmlSource" runat="server"
    DataFile="http://forum.snitz.com/forum/rssfeed2.asp"
    XPath="rss/channel/item[position()<4]"></asp:XmlDataSource>
        
</div>
<br class="seperator"/>




