<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="SnitzUI.Setup.Setup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
.adminPanel {
    width: 60%;
    height: 400px;
    margin: auto;
}

/** You can use this style for your INPUT, TEXTAREA, SELECT elements **/
form input[type=text] {
	border: 1px solid #000000;
	/** remember to change image path **/
	background-color: #eeeeee;
	font-family: tahoma, helvetica, sans-serif;
	font-style: normal;
	font-size: 14px;
	color: #454743;
    margin: 2px;
}

/** You can use this style for your LABEL elements **/
label {
	font-family: tahoma, helvetica, sans-serif;
	font-weight: bold;
	font-size: 13px;
	color: #000000;
    width: 120px;
	float: left;
    margin: 2px;
}

        
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="AdminUserRequired" runat="server" GroupingText="Admin User" CssClass="adminPanel">
        <asp:Label ID="Label1" runat="server" Text="Admin Username" AssociatedControlID="adminUser"></asp:Label><asp:TextBox ID="adminUser" runat="server"></asp:TextBox><br/>
        <asp:Label ID="Label2" runat="server" Text="Password" AssociatedControlID="adminPassword"></asp:Label><asp:TextBox ID="adminPassword" runat="server"></asp:TextBox><br/>
        <asp:Label ID="Label3" runat="server" Text="Email address" AssociatedControlID="adminEmail"></asp:Label><asp:TextBox ID="adminEmail" runat="server"></asp:TextBox><br/>
        <asp:Button ID="UpdateDB" runat="server" Text="Upgrade Database" OnClick="UpdateDB_Click" />
        

        </asp:Panel>
        <asp:HiddenField ID="updateType" runat="server" />
    <div style="font-family: Verdana,Arial,sans-serif;font-size: 11px;">
        <asp:Literal ID="litSetupResult" runat="server"></asp:Literal>
        <asp:HyperLink ID="lnkReturn" runat="server" NavigateUrl="~/default.aspx" Visible="False">Go to Forum</asp:HyperLink>
    </div>
    </form>
</body>
</html>
