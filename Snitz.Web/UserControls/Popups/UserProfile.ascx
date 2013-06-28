<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserProfile.ascx.cs" Inherits="SnitzUI.UserControls.UserProfile" %>
<%@ Import Namespace="SnitzCommon" %>

<asp:Repeater ID="rpt" runat="server" onitemdatabound="RptItemDataBound">
    <ItemTemplate>
        <asp:Literal ID="AvatarLabel" runat="server" Text='<%# Bind("Avatar") %>' />
        <asp:PlaceHolder ID="phAvatar" runat="server"></asp:PlaceHolder>
        <asp:Label ID="MemberTitleLabel" runat="server" CSSClass="usertitle" Text='<%# Bind("Rank.Title") %>' /><br />
        <asp:Label CssClass="usrlabel" ID="Label1" runat="server" Text="<%$ Resources:webResources, lblName %>"></asp:Label>:
        <asp:Label ID="NameLabel" runat="server" Text='<%# Bind("Name") %>' /><br />
        <asp:Label CssClass="usrlabel" ID="Label2" runat="server" Text="<%$ Resources:webResources, lblCountry %>"></asp:Label>:
        <asp:Label ID="CountryLabel" runat="server" Text='<%# Bind("Country") %>' /><br />
        <asp:Label CssClass="usrlabel" ID="Label3" runat="server" Text="Number of posts"></asp:Label>:
        <asp:Label ID="postsLabel" runat="server" Text='<%# Common.TranslateNumerals(Eval("PostCount"))%>' /><br />
        <asp:Label CssClass="usrlabel" ID="Label4" runat="server" Text="<%$ Resources:webResources, lblMemberSince %>"></asp:Label>:
        <asp:Label ID="MemberSinceLabel" runat="server" Text='<%# String.Format("{0:MMMM dd, yyyy}",Eval("MemberSince")) %>' /><br />
        <asp:Label CssClass="usrlabel" ID="Label6" runat="server" Text="<%$ Resources:webResources, lblLastVisit %>"></asp:Label>:<%# Common.TimeAgoTag((DateTime)DataBinder.Eval(Container.DataItem, "LastVisitDate"), this.IsAuthenticated, this.CurrentUser != null ? this.CurrentUser.TimeOffset : 0)%>
        <hr />
        <asp:Label ID="Label5" runat="server" CssClass="smallText" Text='<%# Bind("ParsedSignature") %>' />
    </ItemTemplate>
</asp:Repeater>


