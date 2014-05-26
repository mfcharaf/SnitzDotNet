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
<div class="forumtable" style="width:75%;background-color:rgb(47,47,47);margin:auto;">

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
        DestinationPageUrl="~/default.aspx" >
        
        <LayoutTemplate>
        
            <table border="0" cellpadding="3" class="forumtable" >
                <tr>
                    <td align="center" colspan="2" class="category">
                        <asp:Literal runat="server" ID="loginLabel" Text="<%$ Resources: webResources, lblLoginTitle%>"></asp:Literal>
                        </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <asp:Label ID="LoginReq" runat="server" Text="<%$ Resources: webResources, ErrLoginRequired %>"></asp:Label>
                        </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="<%$ Resources:webResources, lblUsername %>"></asp:Label></td>
                    <td align="left"><asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                            ErrorMessage="<%$ Resources: webResources, ErrNoUsername%>" ToolTip="<%$ Resources: webResources, ErrNoUsername %>"
                            ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="<%$ Resources: webResources, lblPassword%>"></asp:Label></td>
                    <td align="left"><asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                            ErrorMessage="<%$ Resources: webResources, ErrNoPassword %>" ToolTip="<%$ Resources: webResources, ErrNoPassword %>"
                            ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center" style="height: 20px">
                        <asp:CheckBox ID="RememberMe" runat="server" Checked="True" Text="<%$ Resources: webResources, lblRememberMe %>" />
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2" style="color: red">
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="2">
                        <captcha:CaptchaControl ID="CAPTCHA" runat="server" ShowSubmit="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:LinkButton ID="LoginButton" runat="server" CommandName="Login" Text="<%$ Resources: webResources, btnLogin %>"
                            ValidationGroup="Login1" style="float:right;" />
                    </td>
                </tr>
            </table>

        </LayoutTemplate>
   
    </asp:Login>
    <br/>
    <center class="forumtable"><asp:Label ID="Label3" runat="server" Text="<%$ Resources: extras, lblRetrieve %>"></asp:Label><br/>
    <asp:LinkButton CssClass="resetLnk" ID="LinkButton1" runat="server" Text="<%$ Resources: webResources, btnReset %>"
        CausesValidation="False" >Pass</asp:LinkButton></center>
</div>

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
        TargetControlID="LinkButton1" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="CancelModal();" DropShadow="true" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Ads"/>
</asp:Content>
