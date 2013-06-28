<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnnounceBox.ascx.cs" Inherits="SnitzUI.UserControls.AnnounceBox" %>
<div class="announceBox clearfix">
    <asp:LoginView ID="LoginView1" runat="server" >
        <AnonymousTemplate>
            Welcome Guest, If you would like to take full advantage of all the forums functions, please <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/Register.aspx" ToolTip='<%$ Resources:SiteMapLocalizations,RegisterPageTitle %>' Text="<%$ Resources:SiteMapLocalizations,RegisterPageTitle %>"></asp:HyperLink>&nbsp;or&nbsp;<asp:LoginStatus runat="server" id="LO2" SkinID="NewLoginStatus" />
        </AnonymousTemplate>
        <LoggedInTemplate>
            <p>Hi, If you would like to test out the admin features I have created a</p>
                <a href="http://snitzdemo.reddiweb.com">DEMO SITE</a>
            <p>Username: TestAdmin<br />Password: p@ssword<br />Please fell free to have a play around :).</p>

        </LoggedInTemplate>
    </asp:LoginView>  
</div>