<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Registration.ascx.cs" Inherits="SnitzUI.Admin.UserControls.Admin_Registration" %>
<%@ Register TagPrefix="uc1" TagName="AdminRadioButton" Src="~/Admin/UserControls/AdminRadioButton.ascx" %>
<style>
.label_not_mandatory label{display: inline-block;margin: 1px 16px 1px 0;white-space: nowrap;width: 150px;}
.label_sys{color:maroon}
.label_not_mandatory span.cbxLabel label{ width: auto;}
.password_strength label{ width: 260px;display: inline-block;}
.password_strength input[type="text"]{ width: 24px;display: inline-block;}
</style>
<asp:Panel ID="regCtls" runat="server" CssClass="clearfix adminform" DefaultButton="btnSubmit">
    <h2 class="category">Registration</h2>
        <div id="content">
        <asp:Panel ID="Panel3" runat="server" GroupingText="Email settings" CssClass="emailform">
            <br style="line-height: 0.1em;"/>
            <asp:Label ID="lblUniqueEmail" runat="server" Text="Require Unique E-mail" 
                AssociatedControlID="rblUniqueEmail"></asp:Label>
            <uc1:AdminRadioButton ID="rblUniqueEmail" runat="server" />
            <br />
            <asp:Label ID="lblEmailVal" runat="server" Text="E-mail Validation" 
                AssociatedControlID="rblEmailVal"></asp:Label>
            <uc1:AdminRadioButton ID="rblEmailVal" runat="server" />
            <br />
            <asp:Label ID="lblRestrict" runat="server" Text="Approve Registrations" 
                AssociatedControlID="rblRestrictReg"></asp:Label>
            <uc1:AdminRadioButton ID="rblRestrictReg" runat="server" />
            <br />
        </asp:Panel>
            <asp:Panel runat="server" ID="pnlGen" GroupingText="Privacy/Security settings">
                <asp:Label ID="lblMinAge" runat="server" Text="Minimum age limit (set to 0 for no limit)"></asp:Label>&nbsp;:&nbsp;
                <asp:TextBox ID="txtMinAge" runat="server" Width="18px"></asp:TextBox><br/>
               <asp:Label ID="Label7" runat="server" Text="Use CAPTCHA for Registration" 
                    AssociatedControlID="rblCaptchaReg" meta:resourcekey="Label3Resource1"></asp:Label>
                <uc1:AdminRadioButton ID="rblCaptchaReg" runat="server" /> <br/>                
            </asp:Panel>
        <asp:Panel runat="server" ID="pwordSettings" GroupingText="Password strength indicator settings" CssClass="password_strength">
            <asp:Label ID="Label1" runat="server" Text="Preferred Password Length" AssociatedControlID="txtPLength"></asp:Label><asp:TextBox ID="txtPLength" runat="server" Text="" MaxLength="2"></asp:TextBox><br/>
            <asp:Label ID="Label2" runat="server" Text="Minimum Numeric Characters" AssociatedControlID="txtMinNum"></asp:Label><asp:TextBox ID="txtMinNum" runat="server" Text=""></asp:TextBox><br/>
            <asp:Label ID="Label3" runat="server" Text="Minimum Symbol Characters" AssociatedControlID="txtMinSym"></asp:Label><asp:TextBox ID="txtMinSym" runat="server" Text=""></asp:TextBox><br/>
            <asp:Label ID="Label6" runat="server" Text="Requires Upper And Lower Case Characters" AssociatedControlID="rblUpperLower"></asp:Label><uc1:AdminRadioButton ID="rblUpperLower" runat="server" SelectedValue="0" /><br/>
            <asp:Label ID="Label4" runat="server" Text="Minimum UpperCase Characters" AssociatedControlID="txtMinUpper"></asp:Label><asp:TextBox ID="txtMinUpper" runat="server" Text=""></asp:TextBox><br/>
            <asp:Label ID="Label5" runat="server" Text="Minimum LowerCase Characters" AssociatedControlID="txtMinLower"></asp:Label><asp:TextBox ID="txtMinLower" runat="server" Text=""></asp:TextBox><br/>
        </asp:Panel>
            <asp:Panel ID="pnlReg" runat="server" CssClass="regform" GroupingText="Member settings">
                <p>If you have set a minimum age, the date of birth field will be automatically chosen and set to required.</p>
                <asp:PlaceHolder runat="server" ID="mRegControls"></asp:PlaceHolder>
            </asp:Panel>
            <br/>
        <asp:Panel ID="Panel5" runat="server">
            <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="BtnSubmitClick" />&nbsp;
            <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />
        </asp:Panel>
        </div>
    </asp:Panel>