<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnnounceBox.ascx.cs" Inherits="SnitzUI.UserControls.AnnounceBox" %>
<div class="announceBox clearfix">
    <asp:LoginView ID="LoginView1" runat="server" >
        <AnonymousTemplate>
            <asp:Literal runat="server" ID="anonText" Text="<%# AnonMessage %>"></asp:Literal>
        </AnonymousTemplate>
        <LoggedInTemplate>
            <asp:Literal runat="server" ID="authText" Text="<%# AuthMessage %>"></asp:Literal>
        </LoggedInTemplate>
    </asp:LoginView>  
</div>