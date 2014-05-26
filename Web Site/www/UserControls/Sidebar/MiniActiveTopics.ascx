<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MiniActiveTopics.ascx.cs" Inherits="SnitzUI.UserControls.MiniActiveTopics" EnableViewState="False" %>
<div class="sideBox">
<asp:Repeater ID="DataList1" runat="server" EnableViewState="False" OnItemDataBound="BindRepeater">
    <HeaderTemplate>
        <div class="category cattitle">
            <asp:Label ID="Label1" runat="server" Text="Latest Posts" style="width:100%;margin:auto;" EnableViewState="False"></asp:Label>
        </div>
        <div class="innertube">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="ovHidden" style="padding:4px;">
        <a class="" href='/Content/Forums/topic.aspx?TOPIC=<%# Eval("Id") %>'
                        target='<%# Eval("Author.LinkTarget") %>' 
                        title="<%# Eval("Subject") %>">
                        <span class="minibbcode mImg" ><%# HttpUtility.HtmlDecode(Eval("Subject").ToString()) %></span></a><br />
        <span class="smallText">last post by:&nbsp;<asp:Literal 
                        ID="popuplink" runat="server" 
                        Text='<%# Eval("LastPostAuthorPopup") %>' EnableViewState="False"></asp:Literal>&nbsp;<asp:HyperLink
                        ID="lpLnk" runat="server" CssClass="profilelnk" SkinID="JumpTo" NavigateUrl='<%# String.Format("~/Content/Forums/topic.aspx?TOPIC={0}&amp;whichpage=-1#{1}", Eval("Id"),Eval("LastReplyId")) %>'
                        ToolTip="<%$ Resources:webResources, lblLastPostJump %>" 
                        Text="<%$ Resources:webResources, lblLastPostJump %>"></asp:HyperLink></span>&nbsp;

                    <asp:Literal runat="server" ID="lastposttime"></asp:Literal>
         </div>
    </ItemTemplate>
    <FooterTemplate></div></FooterTemplate>
    <SeparatorTemplate>
        <hr />
    </SeparatorTemplate>
</asp:Repeater>
</div>
<br class="seperator"/>