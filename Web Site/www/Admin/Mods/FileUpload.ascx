<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="SnitzUI.Admin.Mods.FileUpload" %>
<%@ Register TagPrefix="uc1" TagName="AdminRadioButton" Src="~/Admin/UserControls/AdminRadioButton.ascx" %>
<asp:Panel ID="modContainer" runat="server" CssClass="" GroupingText="xxx">
    <asp:Label ID="Label3" runat="server" CssClass="mod_enabled_lbl" Text="Mod Enabled" AssociatedControlID="rblEnabled"></asp:Label><uc1:AdminRadioButton ID="rblEnabled" runat="server" Visible="True"  /> <br/>
    <asp:Panel ID="edtPanel" runat="server" GroupingText="Settings">
        <asp:Label ID="Label1" runat="server" Text="Allow Image Upload" AssociatedControlID="rblAllowImageUpload" style="display: inline-block;width:130px"></asp:Label><uc1:AdminRadioButton ID="rblAllowImageUpload" runat="server" Visible="True"  /><br/>
        <asp:Label ID="Label2" runat="server" Text="Allow Attachments" AssociatedControlID="rblAllowAttachments" style="display: inline-block;width:130px"></asp:Label><uc1:AdminRadioButton ID="rblAllowAttachments" runat="server" Visible="True"  /> <br/>                         
        <br style="line-height: 0.1em;"/>
        <asp:Label ID="lblUploadLocation" runat="server" Text="Upload Location" AssociatedControlID="txtUploadLocation"></asp:Label><br/>
        <asp:TextBox ID="txtUploadLocation" runat="server" Width="300"></asp:TextBox><br/>
        <asp:Label ID="lblAllowedFileTypes" runat="server" Text="Allowed File Types" AssociatedControlID="txtAllowedAttachmentTypes"></asp:Label><br/>
        <asp:TextBox ID="txtAllowedAttachmentTypes" runat="server" Width="300"></asp:TextBox><br/>  
        <asp:Label ID="lblAllowedImageTypes" runat="server" Text="Allowed Image Types" AssociatedControlID="txtAllowedImageTypes"></asp:Label><br/>
        <asp:TextBox ID="txtAllowedImageTypes" runat="server" Width="300"></asp:TextBox><br/>  
        <asp:Label ID="lblFileSizeLimit" runat="server" Text="FileSize Limit (Kb)" AssociatedControlID="txtFileSizeLimit"></asp:Label><br/>
        <asp:TextBox ID="txtFileSizeLimit" runat="server" Width="100"></asp:TextBox> <br/>

        <asp:Label ID="lblTotalUploadLimitFileSize" runat="server" Text="Total Upload Limit per user (Mb)" AssociatedControlID="txtTotalUploadLimitFileSize"></asp:Label><br/>
        <asp:TextBox ID="txtTotalUploadLimitFileSize" runat="server" Width="100"></asp:TextBox><br/> 
        <asp:Label ID="lblTotalUploadLimitFileNumber" runat="server" Text="Max Files Allowed per User" AssociatedControlID="txtTotalUploadLimitFileNumber"></asp:Label><br/>
        <asp:TextBox ID="txtTotalUploadLimitFileNumber" runat="server" Width="50"></asp:TextBox> <br/>
        <asp:Panel ID="btnPanel" runat="server" CssClass="mod_btn_pnl">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" />&nbsp;<asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="hlpPanel" runat="server" CssClass="help-panel" Visible="False">
        <h3>Mod Instructions</h3>
        <asp:Literal ID="litInstructions" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Panel>