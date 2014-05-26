<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminRadioButton.ascx.cs" Inherits="SnitzUI.Admin.AdminRadioButton" %>
<asp:RadioButtonList ID="rblOption" runat="server" 
    RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="cbx">
    <asp:ListItem Value="1" Text="<%$ Resources:webResources,btnYes %>" ></asp:ListItem>
    <asp:ListItem Value="0" Selected="True" Text="<%$ Resources:webResources,btnNo %>" ></asp:ListItem>
</asp:RadioButtonList>