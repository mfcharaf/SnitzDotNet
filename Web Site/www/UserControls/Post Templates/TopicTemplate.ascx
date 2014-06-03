<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicTemplate.ascx.cs" Inherits="SnitzUI.UserControls.Post_Templates.TopicTemplate" %>
<%@ Register TagPrefix="topic" TagName="MessageProfile" Src="~/UserControls/MessageProfile.ascx" %>
<%@ Register TagPrefix="topic" TagName="MessageButtonBar" Src="~/UserControls/MessageButtonBar.ascx" %>
    <style>
    #rightcolumn,.rightcolumn{ display: none;width: 0px;}
    #contentwrapper{ margin-right: 0px;} 
    .maincolumn{ width: 99%;}       
    </style>
<asp:Panel  ID="PostPanel" runat="server">
    <div class="leftColumn">
        <asp:HiddenField ID="hdnAuthor" runat="server" />
        <asp:Literal ID="popuplink" runat="server" Text=''></asp:Literal>
        <topic:MessageProfile runat="server" ID="AuthorProfile" />
    </div>
    <div class="MessageDIV">
        <div class="buttonbar">
            <topic:MessageButtonBar ID="buttonBar" runat="server" Post='<%# Post %>' />
        </div>
        <div class="mContent bbcode">
            <asp:Literal ID="msgBody" runat="server" Text='' Mode="Encode"></asp:Literal>
        </div>
        <br />
        <div id="editbyDiv" runat="server" class="editedDIV" >
            <asp:Label ID="editedByLbl" runat="server" Text=''></asp:Label>
            <asp:Literal ID="litEditDate" runat="server" Text='' />
        </div>
        <div id="sigDiv" runat="server" class="sigDIV bbcode" >
            <asp:Literal ID="litSig" runat="server" Text='' Mode="Encode"></asp:Literal>
        </div>
    </div>
</asp:Panel>