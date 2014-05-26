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
    CodeBehind="Register.aspx.cs" Inherits="SnitzUI.RegisterPage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register src="~/UserControls/DatePicker.ascx" tagname="DatePicker" tagprefix="uc2" %>
<asp:Content ID="head" ContentPlaceHolderID="CPHead" runat="server">
<link rel="stylesheet" type="text/css" runat="server" id="pageCSS"/>
    <script type="text/javascript">
        var txtUserName;
        var txtEmail;

        $(document).ready(function () {
            txtUserName = '<%= ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("UserName")).ClientID %>';
            txtEmail = '<%= ((TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Email")).ClientID %>';

            $("#" + txtUserName).blur(function () {
                // Write All stuff that you want to happen in your TextBox's on blur event here. 
                // Within this function you can access the TextBox using 'this' for eg) this.value 
                if (this.value.length > 0) {
                    $get("spanUsername").style.display = "inline";
                    $get("spanUsername").innerHTML = "Checking Username Availability...<img id=imgWaiting src=/images/ajax-loader.gif>";
                    $get("spanUsername").className = "validationGood";
                    PageMethods.CheckUserName(this.value, OnCheckUserName);
                }
                else {
                    $get("spanUsername").style.display = "inline";
                    $get("spanUsername").className = "validationBad";
                    $get("spanUsername").innerHTML = "Username is required";
                    setTimeout(function () {
                        $('#spanUsername').fadeOut('fast');
                    }, 2000); // <-- time in milliseconds 
                }
            });
            $("#" + txtEmail).blur(function () {
                // Write All stuff that you want to happen in your TextBox's on blur event here. 
                // Within this function you can access the TextBox using 'this' for eg) this.value 
                if (this.value.length > 0) {
                    $get("spanUsername").style.display = "inline";
                    $get("spanUsername").className = "validationGood";
                    $get("spanUsername").innerHTML = "Checking for duplicate email...<img id=imgWaiting1 src=/images/ajax-loader.gif>";
                    PageMethods.CheckEmail(this.value, OnCheckEmail);
                }
                else {
                    $get("spanUsername").innerHTML = "Email is required";
                    $get("spanUsername").className = "validationBad";
                    $get("spanUsername").style.display = "inline";
                    setTimeout(function () {
                        $('#spanUsername').fadeOut('fast');
                    }, 2000); // <-- time in milliseconds 
                }
            });
        });
        //////////////////////////////Username JS///////////////////////////////////////////////////// // 


        function OnCheckEmail(duplicate) {
            if (duplicate == true) {
                $get("spanUsername").style.display = "inline";
                $get("spanUsername").className = "validationBad";
                $get("spanUsername").innerHTML = "Email is already in use";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('fast');
                }, 2000); // <-- time in milliseconds 

            }
            else if (duplicate != true) {
                $get("spanUsername").style.display = "inline";
                $get("spanUsername").innerHTML = "Email Address OK";
                $get("spanUsername").className = "validationGood";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('fast');
                }, 2000); // <-- time in milliseconds 
            }
        }


        function OnCheckUserName(unavailable) {
            if (unavailable == true) {
                $get("spanUsername").style.display = "inline";
                $get("spanUsername").innerHTML = "Username is either not allowed or is already in use";
                $get("spanUsername").className = "validationBad";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('fast');
                }, 2000); // <-- time in milliseconds 
            }
            else if (unavailable != true) {
                $get("spanUsername").style.display = "none";
                setTimeout(function () {
                    $('#spanUsername').fadeOut('fast');
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
            oncreateusererror="CreateUserWizard1CreateUserError">
            <WizardSteps>
                <asp:WizardStep ID="CreateUserWizardStep0" runat="server" Title="Policy" StepType="Start"
                    AllowReturn="False">
                    <div id="formLayout" class="policy">
                        <asp:Literal ID="policy" runat="server" Text="Policy"></asp:Literal>
                        <asp:CheckBox ID="AcceptTerms" runat="server" Text="<%$ Resources:extras, PolicyAgree %>"
                            ValidationGroup="CreateUserWizard1" CausesValidation="true" />
                        <asp:CustomValidator ID="ValTerms" ClientValidationFunction="AcceptTermsCheckBoxValidation"
                            runat="server" ErrorMessage="<%$ Resources:extras, PolicyValidator %>" ValidationGroup="CreateUserWizard1"
                            SetFocusOnError="True">
                        </asp:CustomValidator>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Personal Information" ID="personalInfo">
                    <div class="formLayout">
                    <asp:Label ID="lbl_Bio_Realname" runat="server" Text="Name" EnableViewState="False"
                        AssociatedControlID="fullname"></asp:Label>
                    <asp:TextBox ID="fullname" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_Bio_Realname" runat="server" ControlToValidate="fullname"
                        ErrorMessage="You must supply your full name."  Display="Dynamic" Enabled="False" EnableClientScript="false"><asp:Image ID="Image7" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator><br />
                    <asp:Label ID="lbl_Location_City" runat="server" Text="City" AssociatedControlID="city"></asp:Label>
                    <asp:TextBox ID="city" runat="server"></asp:TextBox>
                    &nbsp;<asp:RequiredFieldValidator ID="RFV_Location_City" runat="server" ControlToValidate="city" EnableClientScript="false"
                        ErrorMessage="City is required"  Display="Dynamic" Enabled="False"><asp:Image ID="Image6" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator><br />
                    <asp:Label ID="lbl_Location_State" runat="server" Text="County" AssociatedControlID="state"></asp:Label>
                    <asp:TextBox ID="state" runat="server"></asp:TextBox>
                    &nbsp;<asp:RequiredFieldValidator ID="RFV_Location_State" runat="server" ControlToValidate="state" EnableClientScript="false"
                        ErrorMessage="County is a required field"  Display="Dynamic" Enabled="False"><asp:Image ID="Image5" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator><br />
                    <asp:Label ID="lbl_Location_Country" runat="server" EnableViewState="False" AssociatedControlID="Country"
                        Text="Country" CssClass="label_mandatory"></asp:Label>
                    <asp:DropDownList ID="Country" runat="server"  CausesValidation="True">
                        <asp:ListItem Text="[Select a Country]" Value="" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RFV_Location_Country" InitialValue="" runat="server" ControlToValidate="Country" EnableClientScript="false"
                        ErrorMessage="Please select your Country" EnableViewState="False"  Display="Dynamic" Enabled="True"><asp:Image ID="Image4" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator><br />
                    <asp:Label ID="lbl_Bio_Sex" runat="server" Text="Gender" EnableViewState="False" AssociatedControlID="Gender"></asp:Label>
                    <asp:DropDownList ID="Gender" runat="server" CausesValidation="True">
                        <asp:ListItem Text="[Select Gender]" Value="" />
                        <asp:ListItem Text="Male" />
                        <asp:ListItem Text="Female" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="RFV_Bio_Sex" runat="server" InitialValue="[Select Gender]" EnableClientScript="false"
                        ControlToValidate="Gender" ErrorMessage="Gender is a required field"  EnableViewState="False"
                        Display="Dynamic" Enabled="False"><asp:Image ID="Image2" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator><br />
                    
                        <uc2:DatePicker ID="DatePicker1" runat="server" />
                        <asp:RequiredFieldValidator ID="RFV_Bio_DOB" runat="server" ControlToValidate="DatePicker1$ddlYear"
                                        ErrorMessage="You must enter your date of birth" InitialValue="2012" EnableViewState="False" Display="Dynamic" Enabled="False"><asp:Image ID="Image3" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator>
                        <br />

                    <asp:Label ID="lbl_Bio_Occupation" runat="server" Text="Occupation" AssociatedControlID="occupation"></asp:Label>
                    <asp:TextBox ID="occupation" runat="server" Width="200px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_Bio_Occupation" runat="server" ControlToValidate="occupation" EnableClientScript="false"
                        Display="Dynamic" ErrorMessage="Please tell us your occupation" Enabled="False">
                        <asp:Image ID="Image1" runat="server" SkinID="Error" />
                    </asp:RequiredFieldValidator>
                    <br />
                    <asp:Label ID="mandatory1" runat="server" Text="Items marked" CssClass="label_mandatory"></asp:Label>&nbsp;<asp:Label
                        ID="mandatory2" runat="server" Text="are mandatory"></asp:Label>
                    <br /><br />
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="validation" />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="moreMe" runat="server" Title="More About Me">
                <div class="formLayout">
                    <asp:Label ID="lbl_Biography" runat="server" Text="About Me"></asp:Label><br />
                    <asp:TextBox ID="biography" runat="server" Rows="4" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_Biography" runat="server" ControlToValidate="biography"
                        Display="Dynamic" ErrorMessage="You must fill in your biography" Enabled="false">*</asp:RequiredFieldValidator>
                    <asp:Label ID="lbl_Hobbies" runat="server" Text="Hobbies"></asp:Label>
                    <asp:TextBox ID="hobbies" runat="server" Rows="4" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_Hobbies" runat="server" ControlToValidate="hobbies"
                        Display="Dynamic" ErrorMessage="You must fill in your hobbies" Enabled="false">*</asp:RequiredFieldValidator>
                    <asp:Label ID="lbl_LNews" runat="server" Text="Latest News"></asp:Label>
                    <asp:TextBox ID="lnews" runat="server" Rows="4" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_LNews" runat="server" ControlToValidate="Lnews"
                        Display="Dynamic" ErrorMessage="You must fill in your latest News" Enabled="false">*</asp:RequiredFieldValidator>
                    <asp:Label ID="lbl_Quote" runat="server" Text="Favorite Quote"></asp:Label>
                    <asp:TextBox ID="quote" runat="server" Rows="4" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_Quote" runat="server" ControlToValidate="quote"
                        Display="Dynamic" ErrorMessage="You must fill in your favorite quote" Enabled="false">*</asp:RequiredFieldValidator>
                    <asp:ValidationSummary ID="VSmore" runat="server" CssClass="validation"/>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="options" runat="server" Title="General Options">
                <div class="checkboxLayout">
                    <asp:Label ID="Label12" runat="server" Text="View Signatures in posts" AssociatedControlID="viewsig"></asp:Label>
                    <asp:RadioButtonList ID="viewsig" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Selected="True" Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="0">No</asp:ListItem>
                    </asp:RadioButtonList><br />
                    <asp:Label ID="Label13" runat="server" Text="Signature checked by default" AssociatedControlID="usesig"></asp:Label>
                    <asp:RadioButtonList ID="usesig" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                        <asp:ListItem Selected="True" Value="0">No</asp:ListItem>
                    </asp:RadioButtonList><br />
                    <asp:Label ID="Label14" runat="server" Text="Receive Emails from members" AssociatedControlID="recemail"></asp:Label><br />
                    <asp:RadioButtonList ID="recemail" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">
                        <asp:ListItem Selected="True" Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="0">No</asp:ListItem>
                    </asp:RadioButtonList><br />
                    <asp:Label ID="Label15" runat="server" Text="Signature" AssociatedControlID="signature"></asp:Label><br />
                    <asp:TextBox ID="signature" runat="server" Rows="4" TextMode="MultiLine" Width="70%"></asp:TextBox>
                    </div>
                </asp:WizardStep>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server" Title="">
                    <ContentTemplate>
                        <div class="formLayout">
                            <span>
                                <asp:Literal ID="uwStep1" runat="server" Text="<%$ Resources:webResources, lblCUserStep1 %>"></asp:Literal>&nbsp;
                                <asp:Literal ID="domain" runat="server"></asp:Literal><br />
                                <br />
                            </span>
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                            <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image5" runat="server" SkinID="Error" /></asp:RequiredFieldValidator>
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" Visible="false"></asp:TextBox>
                            <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                            <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1"><asp:Image ID="Image8" runat="server" SkinID="Error" /></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="EmailRequired1" runat="server" ControlToValidate="Email"
                                ErrorMessage="You must enter a valid email address" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="CreateUserWizard1"><asp:Image ID="Image9" runat="server" SkinID="Error" /></asp:RegularExpressionValidator>
                            <asp:TextBox ID="Question" runat="server" MaxLength="20" Visible="False">Security Phrase</asp:TextBox>&nbsp;
                            <asp:TextBox ID="Answer" runat="server" Enabled="False" Visible="False">1234567bnr</asp:TextBox>

                            <captcha:CaptchaControl ID="CaptchaControl1" runat="server" ShowSubmit="false" />
                            <span id="spanUsername" class="usercheck"></span>
                            <asp:PlaceHolder ID="phCustomStuff" runat="server" Visible="false" />
                            <asp:ValidationSummary ID="VScreate" runat="server" CssClass="validation" ValidationGroup="CreateUserWizard1"/>
                        </div>
                    </ContentTemplate>
                </asp:CreateUserWizardStep>
                <asp:CompleteWizardStep ID="CreateUserWizardStep2" runat="server">
                    <ContentTemplate>
                        <div class="formLayout">
                            <strong>
                                <asp:Label ID="lblsuccess" runat="server" Text="<%$ Resources:extras, SuccessLabel %>"></asp:Label></strong>
                            <p>
                            </p>
                            <asp:Label ID="successmsg" runat="server" Text="<%$ Resources:extras, RegisterSuccess %>"></asp:Label>
                            <br />
                            <asp:ImageButton ID="ContinueButton" runat="server" SkinID="continueB" CausesValidation="False"
                                CommandName="Continue" AlternateText="Continue" ValidationGroup="CreateUserWizard1" />
                        </div>
                    </ContentTemplate>
                </asp:CompleteWizardStep>
            </WizardSteps>
            <HeaderStyle CssClass="category" />
            <MailDefinition Subject="Welcome" BodyFileName="~/App_Data/RegisterMail.html"></MailDefinition>
            <StepStyle CssClass="ForumList" />
            <StartNavigationTemplate>
                <asp:Button ID="StartNextButton" runat="server" CommandName="MoveNext" Text="Continue"
                    ValidationGroup="CreateUserWizard1" />
            </StartNavigationTemplate>
        </asp:CreateUserWizard>
    </div>
    <br />
</asp:Content>
<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Ads"/>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
