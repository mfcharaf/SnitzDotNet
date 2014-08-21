<%-- 
###########################################################################################
## Snitz Forums .net
###########################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
###########################################################################################
--%>

<%@ Control Language="C#" AutoEventWireup="true" Inherits="SnitzCaptchaControl" Codebehind="CaptchaControl.ascx.cs" %>

    <img src="/Handlers/CaptchaImage.ashx" alt="CAPTCHA Image" class="captchaImage" />
    <br />
        <label class="captcha-label">Please enter the code shown above&nbsp;</label><br/>
        <asp:TextBox ID="CodeNumberTextBox" runat="server" style="text-transform:uppercase"></asp:TextBox>
        <asp:LinkButton ID="SubmitButton" runat="server" Text="Submit" /><br />
        <p>(Note: If you cannot read the numbers in the above image, reload the page to generate a new one.)</p>
        
        <asp:CustomValidator ID="CaptchaValidator" runat="server" ControlToValidate="CodeNumberTextBox"
            ErrorMessage="ERROR: Values do not match, please try again." 
    OnServerValidate="CaptchaValidator_ServerValidate" ValidateEmptyText="True" 
    Display="Dynamic"></asp:CustomValidator>


