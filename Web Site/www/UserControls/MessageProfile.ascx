<%@ Control Language="C#" EnableViewState="false" AutoEventWireup="true" Inherits="MessageProfile" Codebehind="messageprofile.ascx.cs" %>

<div>
    <span class="smallText">
        <asp:Literal ID="Rank" runat="server" EnableViewState="False"></asp:Literal>
        <asp:PlaceHolder ID="phAvatar" runat="server" EnableViewState="False"></asp:PlaceHolder>
        <asp:Label ID="country" runat="server" EnableViewState="False"></asp:Label><br/>
        <asp:Label ID="posts" runat="server" EnableViewState="False"></asp:Label><br />
    </span>
        <div class="userlinks">
            <asp:ImageButton CssClass="profBtn" Visible="false" runat="server" ID="hProf" SkinID="Profile" OnClientClick="" CausesValidation="false" EnableViewState="False" />
            
            <asp:PlaceHolder runat="server" ID="phPrivateSend" EnableViewState="False"></asp:PlaceHolder>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hHome" SkinID="HomePage"></asp:HyperLink>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hEmail" SkinID="Email"></asp:HyperLink>
            <br/>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hAIM" SkinID="AIM"></asp:HyperLink>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hICQ" SkinID="ICQ"></asp:HyperLink>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hYAHOO" SkinID="YAHOO"></asp:HyperLink>
            <asp:HyperLink EnableViewState="False" CssClass="profBtn" rel="nofollow" Visible="false" runat="server" ID="hSKYPE" SkinID="SKYPE"></asp:HyperLink>
        </div>
</div>
