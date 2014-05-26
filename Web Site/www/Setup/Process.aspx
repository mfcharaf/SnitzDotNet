<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Process.aspx.cs" Inherits="SnitzUI.Setup.Process" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:HiddenField ID="updateType" runat="server" />
    <asp:HiddenField ID="adminUsername" runat="server" />
    <asp:HiddenField ID="adminPassword" runat="server" />
    <asp:HiddenField ID="adminEmail" runat="server" />
    </div>
    </form>
</body>
</html>
