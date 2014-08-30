<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
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
<%@ Page Title="" Language="C#" MasterPageFile="~/MasterTemplates/plain.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="SnitzUI.RegisterPage" EnableEventValidation="true" %>

<asp:Content ID="head" ContentPlaceHolderID="CPHead" runat="server">
<link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>

    <script type="text/javascript">
        var txtUserName;
        var txtEmail;

        $(document).ready(function () {
            txtUserName = '<%= ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Username")).ClientID %>';
            txtEmail = '<%= ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email")).ClientID %>';

            $("#" + txtUserName).blur(function () {
                // Write All stuff that you want to happen in your TextBox's on blur event here. 
                // Within this function you can access the TextBox using 'this' for eg) this.value 
                if (this.value.length > 0) {
                    $get("spanUsername").style.visibility = "visible";
                    $get("spanUsername").innerHTML = "Checking Username Availability...<img id=imgWaiting src=/images/ajax-loader.gif height='28'>";
                    $get("spanUsername").className = "validationGood";
                    SnitzUI.CommonFunc.CheckUserName(this.value, OnCheckUserName);
                }
                else {
                    $get("spanUsername").style.visibility = "visible";
                    $get("spanUsername").className = "validationBad";
                    $get("spanUsername").innerHTML = "Username is required";
                    setTimeout(function () {
                        $('#spanUsername').fadeOut('slow',function() {
                            $get("spanUsername").style.display = "inline";
                            $get("spanUsername").style.visibility = "hidden";
                        });
                    }, 2000); // <-- time in milliseconds 
                }
            });
            $("#" + txtEmail).blur(function () {
                // Write All stuff that you want to happen in your TextBox's on blur event here. 
                // Within this function you can access the TextBox using 'this' for eg) this.value 
                if (this.value.length > 0) {
                    $get("spanUsername").style.visibility = "visible";
                    $get("spanUsername").className = "validationGood";
                    $get("spanUsername").innerHTML = "Checking for duplicate email...<img id=imgWaiting1 src=/images/ajax-loader.gif height='28'>";
                    SnitzUI.CommonFunc.CheckEmail(this.value, OnCheckEmail);
                }
                else {
                    $get("spanUsername").innerHTML = "Email is required";
                    $get("spanUsername").className = "validationBad";
                    $get("spanUsername").style.visibility = "visible";
                    setTimeout(function () {
                        $('#spanUsername').fadeOut('slow', function() {
                            $get("spanUsername").style.display = "inline";
                            $get("spanUsername").style.visibility = "hidden";
                        });
                    }, 2000); // <-- time in milliseconds 
                }
            });
        });
        //////////////////////////////Username JS///////////////////////////////////////////////////// // 


        function OnCheckEmail(duplicate) {
            if (duplicate == true) {
                $get("spanUsername").style.visibility = "visible";
                $get("spanUsername").className = "validationBad";
                $get("spanUsername").innerHTML = "Email is already in use";
                $('#' + txtEmail).val("");
                setTimeout(function () {
                    $('#spanUsername').fadeOut('slow', function() {
                        $get("spanUsername").style.display = "inline";
                        $get("spanUsername").style.visibility = "hidden";
                    });
                }, 2000); // <-- time in milliseconds 

            }
            else if (duplicate != true) {
                $get("spanUsername").style.visibility = "visible";
                $get("spanUsername").innerHTML = "Email Address OK";
                $get("spanUsername").className = "validationGood";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('slow', function() {
                        $get("spanUsername").style.display = "inline";
                        $get("spanUsername").style.visibility = "hidden";
                    });
                }, 2000); // <-- time in milliseconds 
            }
        }


        function OnCheckUserName(unavailable) {
            if (unavailable == true) {
                $get("spanUsername").style.visibility = "visible";
                $get("spanUsername").innerHTML = "Username is either not allowed or is already in use";
                $get("spanUsername").className = "validationBad";
                $('#' + txtUserName).val("");
                setTimeout(function () {
                    $('#spanUsername').fadeOut('slow', function() {
                        $get("spanUsername").style.display = "inline";
                        $get("spanUsername").style.visibility = "hidden";
                    });
                }, 2000); // <-- time in milliseconds 
            }
            else if (unavailable != true) {
                $get("spanUsername").style.visibility = "hidden";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('slow', function() {
                        $get("spanUsername").style.display = "inline";
                        $get("spanUsername").style.visibility = "hidden";
                    });
                }, 2000); // <-- time in milliseconds 
            }
        }
        //////////////////////////////Username JS///////////////////////////////////////////////////// //   

        function AcceptTermsCheckBoxValidation(source, args) {

            var vCheckBox = document.getElementById('<%= CreateUserWizard1.FindControl("AcceptTerms").ClientID %>');
            if (!vCheckBox.checked) {
                args.IsValid = false;
            }
            else {
                args.IsValid = true;
            }

        }
 
    </script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="CPH1" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPM" runat="server">
    <div id="RegisterBox">
        <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" CssClass="regBox" HeaderText="<%$ Resources:extras, RegisterTitle %>"
            LoginCreatedUser="False" OnCreatingUser="CreateUserWizard1CreatingUser" OnSendingMail="CreateUserWizard1SendingMail"
            OnCreatedUser="CreateUserWizard1CreatedUser" FinishDestinationPageUrl="~/default.aspx"
            CancelDestinationPageUrl="~/default.aspx" ContinueDestinationPageUrl="~/default.aspx"
            DisableCreatedUser="True" DisplayCancelButton="True" 
            MembershipProvider="SnitzMembershipProvider" 
            oncreateusererror="CreateUserWizard1CreateUserError" OnNextButtonClick="CheckContinue">
            <WizardSteps>
                <asp:WizardStep ID="CreateUserWizardStep0" runat="server" Title="Policy" StepType="Start"
                    AllowReturn="False">
                    <div id="formLayout" class="policy">
                        <asp:Literal ID="policy" runat="server" Text="Policy"></asp:Literal>
                        <asp:CheckBox ID="AcceptTerms" runat="server" Text="<%$ Resources:extras, PolicyAgree %>"
                            ValidationGroup="CreateUserWizard1" CausesValidation="true" />&nbsp;
                        <asp:CustomValidator ID="ValTerms" ClientValidationFunction="AcceptTermsCheckBoxValidation" 
                            runat="server" Display="None" ErrorMessage="<%$ Resources:extras, PolicyValidator %>" ValidationGroup="CreateUserWizard1"
                            SetFocusOnError="True">
                        </asp:CustomValidator>
                        <asp:ValidationSummary ID="valPolicySummary" runat="server" ValidationGroup="CreateUserWizard1" ShowMessageBox="True" />
                    </div>
                </asp:WizardStep>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="">
                    <ContentTemplate>
                        <div class="formLayout clearfix">
                            <div><p>
                                <asp:Literal ID="uwStep1" runat="server" Text="<%$ Resources:webResources, lblCUserStep1 %>"></asp:Literal>
                                <asp:Literal ID="domain" runat="server"></asp:Literal><br/>
                                <asp:Label ID="mandatory1" runat="server" Text="Items marked" CssClass="label_mandatory"></asp:Label>&nbsp;<asp:Label
                                ID="mandatory2" runat="server" Text="are mandatory"></asp:Label>
                                </p>
                            </div>
                            <div>
                                <div style="width:65%;float:left;">
                                    <asp:Label CssClass="label_mandatory" ID="UserNameLabel" runat="server" AssociatedControlID="UserName">Username</asp:Label>
                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                    ErrorMessage="Username is required." ToolTip="Username is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image5" runat="server" SkinID="Error" /></asp:RequiredFieldValidator><br/>
                                    <asp:Label ID="Label1" CssClass="label_mandatory" runat="server" AssociatedControlID="Password">Password</asp:Label>
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password" ></asp:TextBox>
                                    <asp:PasswordStrength ID="Password_PasswordStrength" runat="server" TargetControlID="Password"
                                        PreferredPasswordLength="10"
                                        MinimumNumericCharacters="0"
                                        MinimumSymbolCharacters="0"
                                        RequiresUpperAndLowerCaseCharacters="False"
                                        MinimumLowerCaseCharacters="0"
                                        MinimumUpperCaseCharacters="0"
                                        StrengthIndicatorType="BarIndicator"
                                        BarBorderCssClass="BarBorder" 
                                        TextStrengthDescriptions="Very Poor;Weak;Average;Strong;Excellent"
                                        StrengthStyles="BarIndicatorweak;BarIndicatoraverage;BarIndicatorgood;BarIndicatorexcelent"
                                    >
                                    </asp:PasswordStrength>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image2" runat="server" SkinID="Error" /></asp:RequiredFieldValidator><br/>
                                    <asp:Label ID="Label2" CssClass="label_not_mandatory" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password</asp:Label>
                                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" ></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ConfirmPassword"
                                        ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image3" runat="server" SkinID="Error" /></asp:RequiredFieldValidator>
                                    <asp:CompareValidator runat="server" ID="compPass" ControlToCompare="ConfirmPassword" ControlToValidate="Password" ValidationGroup="CreateUserWizard1" ErrorMessage="Passwords do not match" ><asp:Image ID="Image1" runat="server" SkinID="Error" /></asp:CompareValidator><br/>                                
                                    <asp:Label ID="EmailLabel" CssClass="label_mandatory" runat="server" AssociatedControlID="Email">Email</asp:Label>
                                    <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                        ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image8" runat="server" SkinID="Error" /></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="EmailRequired1" runat="server" ControlToValidate="Email"
                                        ErrorMessage="You must enter a valid email address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                        ValidationGroup="CreateUserWizard1"><asp:Image ID="Image9" runat="server" SkinID="Error" /></asp:RegularExpressionValidator>
                                    <asp:TextBox ID="Question" runat="server" MaxLength="20" Visible="False">Security Phrase</asp:TextBox><asp:TextBox ID="Answer" runat="server" Enabled="False" Visible="False">1234567bnr</asp:TextBox>
                                    <asp:PlaceHolder ID="phCustomStuff" runat="server" />
                                    </div>
                                <div style="width: 34%;float:right;">
                                    <span id="spanUsername" class="usercheck"></span>
                                    <asp:ValidationSummary ID="VScreate" runat="server" CssClass="validation" ValidationGroup="CreateUserWizard1"/>
                                </div>
                            </div>
                            <br/>
                            <div>
                                <captcha:CaptchaControl ID="CaptchaControl1" runat="server" ShowSubmit="false" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:CreateUserWizardStep>
                <asp:WizardStep runat="server" Title="Personal Information" ID="personalInfo" AllowReturn="False"  StepType="Step" OnDeactivate="SaveOptions">
                    <div class="formLayout wizardstep">
                        <asp:PlaceHolder runat="server" ID="mInfoControls" ></asp:PlaceHolder>
                    </div>
                    <div class="formLayout checkboxLayout">
                        <asp:PlaceHolder runat="server" ID="mPostingControls"></asp:PlaceHolder>
                    </div>                    
                    <div class="formLayout">
                        <asp:Label ID="mandatory1" runat="server" Text="Items marked" CssClass="label_mandatory"></asp:Label>&nbsp;<asp:Label
                        ID="mandatory2" runat="server" Text="are mandatory"></asp:Label>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="validation" ValidationGroup="CreateUserWizard1" />
                    </div>
                </asp:WizardStep>
             
                <asp:CompleteWizardStep ID="CreateUserWizardStep3" runat="server">
                    <ContentTemplate>
                        <div class="formLayout">
                            <strong>
                                <asp:Label ID="lblsuccess" runat="server" Text="<%$ Resources:extras, SuccessLabel %>"></asp:Label></strong>
                            <p>
                            </p>
                            <asp:Label ID="successmsg" runat="server" Text="<%$ Resources:extras, RegisterSuccess %>"></asp:Label>
                            <br /><br />
                            <asp:Button ID="ContinueButton" runat="server" SkinID="continueB" CausesValidation="False"
                                CommandName="Continue"  Text="Continue" ValidationGroup="CreateUserWizard1" />
                        </div>
                    </ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps>
            <HeaderStyle CssClass="regHeader" />
            <MailDefinition Subject="Forum Registration" IsBodyHtml="True"></MailDefinition>
            <StepStyle CssClass="ForumList" />
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Continue"
                    ValidationGroup="CreateUserWizard1" />
            </StartNavigationTemplate>
            <StepNavigationTemplate>
                <asp:Button ID="NextButton" runat="server" CommandName="MoveNext" Text="Continue"
                    ValidationGroup="CreateUserWizard1" CausesValidation="True" />
            </StepNavigationTemplate>
        </asp:CreateUserWizard>
    </div>
    <br />
</asp:Content>
<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Ads"/>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
