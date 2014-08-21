<%@ Control Language="C#" AutoEventWireup="true" Inherits="User_Controls_login" Codebehind="login.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="ChangePassword" Src="~/UserControls/popups/ChangePassword.ascx" %>
        <script type="text/javascript">
            function CancelModal() {
                $find('cpMain').hide();
            }
            function ShowModal() {
                $find('cpMain').show();
            }
        </script>

<div id="LoginBox">

        <div class="content" style="padding:3px;">
<asp:LoginView ID="LoginView1" runat="server">
    <LoggedInTemplate>
        <div class="loggedIn" style="vertical-align:middle;text-align:center; white-space:nowrap;">
                <asp:LoginName runat="server" id="ln2" />
                <asp:Literal ID="Literal1" runat="server"></asp:Literal>&nbsp;|&nbsp;
                <asp:LoginStatus runat="server" id="LO2" SkinID="NewLoginStatus"  onloggedout="LO2_LoggedOut"  />
        </div>
    </LoggedInTemplate>
    <AnonymousTemplate>
        <div class="loggedout">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Account/Register.aspx" ToolTip='<%$ Resources:SiteMapLocalizations,RegisterPageTitle %>' Text="<%$ Resources:SiteMapLocalizations,RegisterPageTitle %>"></asp:HyperLink>&nbsp;|&nbsp;<a href="~/Account/Login.aspx" ID="HeadLoginStatus" runat="server">Log In</a>
        </div>
    </AnonymousTemplate>
</asp:LoginView>
        </div>


</div>	            
