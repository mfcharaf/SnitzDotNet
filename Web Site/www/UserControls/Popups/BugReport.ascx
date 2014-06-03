<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BugReport.ascx.cs" Inherits="SnitzUI.UserControls.Popups.BugReport" %>

<asp:Panel ID="pnlMain" runat="server" CssClass="clearfix" >
    <asp:Label ID="Label1" runat="server" Text="Subject" AssociatedControlID="txtSubject"></asp:Label>
        <asp:TextBox ID="txtSubject" runat="server" MaxLength="100" Width="500px"></asp:TextBox><br/>

        
        <asp:Label ID="Label2" runat="server" Text="Description of problem:" AssociatedControlID="txtDescription"></asp:Label>
        <br/><asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="6" ValidationGroup="" Width="550px"></asp:TextBox><br/>
        <asp:Label ID="Label3" runat="server" Text="Timezone and locale (if applicable):" AssociatedControlID="txtTimeZone"></asp:Label>
        <asp:TextBox ID="txtTimeZone" runat="server"></asp:TextBox><br/>  
        <asp:Label ID="Label7" runat="server" Text="OS + Browser  name/version(s) tested:" AssociatedControlID="txtEnvironment"></asp:Label>
        <br/><asp:TextBox ID="txtEnvironment" runat="server" Rows="3" TextMode="MultiLine" Width="332px"></asp:TextBox><br/>
        <br/>
        <asp:Label ID="Label9" runat="server" Text="URL to test case:" AssociatedControlID="txtUrl"></asp:Label>
        <asp:TextBox ID="txtUrl" runat="server" Width="428px"></asp:TextBox><br/> 

    <div ID="pnlError" style="float:left;width:390px;color:red;">
        
    </div>
</asp:Panel>
    <asp:Panel ID="pnlButtons" runat="server" HorizontalAlign="Right" >
        <input type="button" id="btnOk" value="Post Report" onclick="BugReport(); return false;" />

        <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
        CausesValidation="False" EnableViewState="False" OnClientClick="mainScreen.CancelModal();return false;" />
    </asp:Panel>
