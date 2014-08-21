<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RssView.ascx.cs" Inherits="SnitzUI.UserControls.RssView" EnableViewState="false" %>

<div class="sideBox">
    <asp:Repeater ID="MyRSSBlog" runat="server" EnableViewState="False">
        <HeaderTemplate>
            <div class="category">
                <span class="cattitle">&nbsp;<%# Doc.SelectSingleNode("/rss/channel/title").InnerText %></span>
            </div>
            <div class="innertube">
        </HeaderTemplate>
        <ItemTemplate>
            <div style="max-width: 300px; overflow: hidden;">
                <p>
                <i>Forum: <em style="color: #8c0017"><%# XPath("category")%></em></i>
                <asp:HyperLink ID="myLink" runat="server" Target="_blank" NavigateUrl='<%# XPath("link") %>' EnableViewState="False">        
                <h3><%# XPath("title") %></h3></asp:HyperLink>
                <div style="max-height: 124px; overflow: hidden; text-overflow: ellipsis; padding:5px;margin-bottom: 3px;">
                    <%# Regex.Replace(XPath("description").ToString(),"src=\"image","src=\"" + ImgUrl + "image",RegexOptions.IgnoreCase) %>
                </div>
                <i class="smallText">&nbsp;Published on <%# XPath("pubDate").ToString().Remove(XPath("pubDate").ToString().IndexOf(" -")) %></i><br/>
                </p>
            </div>
        </ItemTemplate>
        <SeparatorTemplate>
            <br style="line-height: 0.3em;"/>
        </SeparatorTemplate>
        <FooterTemplate></div></FooterTemplate>
    </asp:Repeater>   
    <asp:Literal runat="server" ID="noFeed"></asp:Literal>   
</div>
<br class="seperator"/>




