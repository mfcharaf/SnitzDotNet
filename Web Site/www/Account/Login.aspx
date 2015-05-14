<%-- 
##############################################################################################################
## Snitz Forums .net
##############################################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##############################################################################################################
--%>
<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/plain.master" AutoEventWireup="true" Inherits="Login" Title="Forum Login" Codebehind="Login.aspx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register TagPrefix="uc1" TagName="Password" Src="~/UserControls/popups/passwordretrieve.ascx" %>

<asp:Content ID="Content4" ContentPlaceHolderID="CPM" Runat="Server">
        <script type="text/javascript">
            function CancelModal() {
                /// Hides modal dialog 
                $find('mbMain').hide();
                }

        </script>

    <asp:Login runat="server" id="Login1"  
        VisibleWhenLoggedIn="False" EnableViewState="False"  
        RememberMeText="<%$ Resources:webResources, lblRememberMe %>" 
        TitleText="<%$ Resources:webResources, lblLoginTitle %>" 
        UserNameLabelText="<%$ Resources:webResources, lblUsername %>"
        PasswordLabelText="<%$ Resources:webResources, lblPassword %>" 
        FailureText="<%$ Resources:webResources, ErrLoginFailure %>" 
        LoginButtonText="<%$ Resources:webResources, btnLogin %>" 
        PasswordRequiredErrorMessage="<%$ Resources:webResources, ErrNoPassword %>" 
        UserNameRequiredErrorMessage="<%$ Resources:webResources, ErrNoUsername %>" 
        OnAuthenticate="Login1_Authenticate" 
        MembershipProvider="SnitzMembershipProvider" 
        DestinationPageUrl="~/default.aspx" OnLoggedIn="Login1_LoggedIn" >
        <LayoutTemplate>
            <div class="login clearfix">
              <h1><asp:Literal runat="server" ID="loginLabel" Text="<%$ Resources: webResources, lblLoginTitle%>"></asp:Literal></h1>
                <p><asp:Literal ID="LoginReq" runat="server" Text="<%$ Resources: webResources, ErrLoginRequired %>"></asp:Literal></p>
                <p><captcha:CaptchaControl ID="CAPTCHA" runat="server" ShowSubmit="false" /></p>
                <p><asp:TextBox ID="UserName" runat="server" style="width:90%;" placeholder="<%$ Resources:webResources, lblUsername %>"/></p>
                <p><asp:TextBox ID="Password" runat="server" TextMode="Password" style="width:90%;" Text="" placeholder="Password"/></p>
                <p class="remember_me">
                  <label>
                      <asp:CheckBox ID="RememberMe" runat="server" Checked="True" Text="" />
                      <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: webResources, lblRememberMe%>" />
                  </label>&nbsp;<asp:LinkButton ID="LoginButton" runat="server" CommandName="Login" Text="<%$ Resources: webResources, btnLogin %>" ValidationGroup="Login1" style="float:right;" />
                </p>
                <div class="login-error">
                    <p><asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal></p>
                </div>
            </div>
            <div class="login-help">
              <p><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources: extras, lblRetrieve%>" />&nbsp;<asp:LinkButton CssClass="resetLnk" ID="LinkButton1" runat="server" Text="<%$ Resources: extras, lblRetrieveLnk %>" CausesValidation="False" ></asp:LinkButton>.</p>
            </div>
            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                ErrorMessage="<%$ Resources: webResources, ErrNoUsername%>" ToolTip="<%$ Resources: webResources, ErrNoUsername %>"
                ValidationGroup="Login1">*</asp:RequiredFieldValidator>        
            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                ErrorMessage="<%$ Resources: webResources, ErrNoPassword %>" ToolTip="<%$ Resources: webResources, ErrNoPassword %>"
                ValidationGroup="Login1">*</asp:RequiredFieldValidator>       
        </LayoutTemplate>
    </asp:Login>


    <asp:Panel ID="MPanel" runat="server" Style="display: none">
        <div class="mainModalPopup mainModalBorder" style="width:500px;">
            <div class="mainModalInnerDiv mainModalInnerBorder">
            <div id="header" class="clearfix" >
                <div class="mainModalDraggablePanelDiv">
                    <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server">
                        <span class="mainModalTitle" id="spanTitle">Reset Password</span>
                    </asp:Panel>
                </div>
                <div class="mainModalDraggablePanelCloseDiv">
                    <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false"/>
                </div>
            </div>
            <div class="mainModalContent" style="white-space:normal;">
                <div id="mainModalContents">
                    <uc1:Password runat="server" ID="recoverpass" /><br />
                </div>
            </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="MPanel"
        TargetControlID="Login1$LinkButton1" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="CancelModal();" DropShadow="true" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Ads"/>
</asp:Content>
