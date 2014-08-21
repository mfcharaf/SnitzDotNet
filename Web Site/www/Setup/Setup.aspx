<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Setup.aspx.cs" Inherits="SnitzUI.Setup.Setup" %>
<%@ Register TagPrefix="ajaxtoolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Database setup</title>
    <style type="text/css">
.adminPanel {
    width: 60%;
    color: #000000;
    background-color: whitesmoke;
    padding: 5px;
    margin: auto;
}

/** You can use this style for your INPUT, TEXTAREA, SELECT elements **/
form input[type=text], form input[type=password] {
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
    width: 160px;
    margin: 2px;
    display: inline-block;
}
#trigger input.button {
    float: none;
    display: block;
    width: 100px;
    margin: 10px auto;
    }

.trigger {
    float: none;
    display: block;
    width: 250px;
    margin: 10px auto;
}
    </style>
    <script type="text/javascript">

        function BeginProcess() {
            // Create an iframe.
            var iframe = document.createElement("iframe");

            // Point the iframe to the location of
            //  the long running process.
            var utype = document.getElementById('updateType').value;

            var query = "?u=" + document.getElementById('updateType').value;
            if (utype != 'upgrade') {
                query = query + "&n=" + document.getElementById('adminUser').value + "&p=" + document.getElementById('adminPassword').value + "&e=" + document.getElementById('adminEmail').value;
            }
            iframe.src = "/Setup/Process.aspx" + query;

            // Make the iframe invisible.
            iframe.style.display = "none";

            // Add the iframe to the DOM.  The process
            //  will begin execution at this point.
            document.body.appendChild(iframe);

            // Disable the button and blur it.
            document.getElementById('trigger').blur();
        }

        function UpdateProgress(PercentComplete, Message) {
            //document.getElementById('ContentPlaceHolder2_lbDownload').setAttribute("disabled", "true");
            //document.getElementById('trigger').value = Message;
            var div = document.getElementById('strResult');
            div.innerHTML = Message + div.innerHTML;
            if (PercentComplete == 100) {
                document.getElementById('lnkreturn').style.visibility = "visible";
                document.getElementById('AdminUserRequired').style.visibility = "hidden";
                document.getElementById('trigger').style.visibility = "hidden";

                setTimeout('loadforum()', 5000);
            }
        }

        function loadforum() {
            window.location = "../default.aspx";
        }
  </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="topsection">
        <div id="header" class="clearfix">
            <asp:Label ID="lblForumTitle" CssClass="PageTitle" runat="server" Text="Snitz Test Forum - Database Setup" EnableViewState="False"></asp:Label>
        </div>
        <div class="innertube clearfix">
            <div id="LogoDIV">
                <asp:HyperLink ID="homeLink" SkinID="Logo" runat="server" EnableViewState="False"></asp:HyperLink>
            </div>
            <div id="TitleDIV">
            </div>
        </div>
        <div id="MenuDIV" class="SnitzMenu">
        </div>
    </div>
            
    <ajaxtoolkit:ToolkitScriptManager ID="MainSM" runat="server" ScriptMode="Release" LoadScriptsBeforeUI="true" EnablePageMethods="false">
        <Scripts>
            <asp:ScriptReference Path="setup.js"/>
        </Scripts>
    </ajaxtoolkit:ToolkitScriptManager>     
    <asp:HiddenField ID="updateType" runat="server" />
           
        <asp:Panel ID="AdminUserRequired" runat="server" GroupingText="Forum Admin User" CssClass="adminPanel">
            <asp:Label ID="Label1" runat="server" Text="Admin Username" AssociatedControlID="adminUser"></asp:Label><asp:TextBox ID="adminUser" runat="server"></asp:TextBox><br/>
            <asp:Label ID="Label2" runat="server" Text="Password" AssociatedControlID="adminPassword"></asp:Label>
                <asp:TextBox ID="adminPassword" runat="server" TextMode="Password"></asp:TextBox><br/>
            <asp:Label ID="Label3" runat="server" Text="Email address" AssociatedControlID="adminEmail"></asp:Label><asp:TextBox ID="adminEmail" runat="server"></asp:TextBox><br/>
        </asp:Panel>  
            <div id="lnkreturn" style="font-family: Verdana,Arial,sans-serif;font-size: 14px;color:red;visibility:hidden;margin: auto;width:50%;text-align: center;">
                <asp:HyperLink ID="lnkR" runat="server" NavigateUrl="~/default.aspx">Go to Forum</asp:HyperLink>
            </div>        
 
       <asp:UpdatePanel ID="UpdatePanel1" runat="server"  >
            <ContentTemplate>
                <input type="submit" value="Upgrade Database" id="trigger" onclick="BeginProcess(); return false;" class="trigger" />
                <div id="strResult" style="height:300px;width:75%;margin:auto;overflow-y: scroll;border: 1px solid gray; background-color:whitesmoke" class="adminPanel"></div>
            </ContentTemplate>
        </asp:UpdatePanel>


         <asp:UpdateProgress ID="UpdateProgress1" AssociatedUpdatePanelID="UpdatePanel1" runat="server">
            <ProgressTemplate>  </ProgressTemplate> 
         </asp:UpdateProgress>
         
    </form>
</body>
</html>
