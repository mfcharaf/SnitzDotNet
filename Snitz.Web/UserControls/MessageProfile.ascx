<%@ Control Language="C#" EnableViewState="true" AutoEventWireup="true" Inherits="MessageProfile" Codebehind="messageprofile.ascx.cs" %>

<div>
    <span class="smallText">
        <asp:Literal ID="Rank" runat="server"></asp:Literal>
        <asp:PlaceHolder ID="phAvatar" runat="server"></asp:PlaceHolder>
        <asp:Label ID="country" runat="server"></asp:Label><br/>
        <asp:Label ID="posts" runat="server"></asp:Label><br />
    </span>
    <br />
        <div class="userlinks">
            <asp:ImageButton CssClass="profBtn" Visible="false" runat="server" ID="hProf" SkinID="Profile" OnClientClick="" CausesValidation="false" />
            <asp:PlaceHolder runat="server" ID="phPrivateSend"></asp:PlaceHolder>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hHome" SkinID="HomePage"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hEmail" SkinID="Email"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hAIM" SkinID="AIM"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hICQ" SkinID="ICQ"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hMSN" SkinID="MSN"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hYAHOO" SkinID="YAHOO"></asp:HyperLink>
            <asp:HyperLink CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hSKYPE" SkinID="SKYPE"></asp:HyperLink>
        </div>
</div>
