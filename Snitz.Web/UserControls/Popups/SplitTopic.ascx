<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SplitTopic.ascx.cs" Inherits="SnitzUI.UserControls.Popups.SplitTopic" %>

<style type="text/css">
.mainModalContent{white-space:normal;min-height:100px;}
</style>

<asp:Panel ID="pnlSplit" runat="server" GroupingText="Split Topic" CssClass="topicSplit">

<asp:Label ID="Label1" runat="server" Text="Forum" AssociatedControlID="ddlForum"></asp:Label>
<asp:DropDownList ID="ddlForum" runat="server" DataTextField="Subject" DataValueField="Id">
</asp:DropDownList><br class="clearfix" />
    <asp:Label ID="Label2" runat="server" Text="Topic Subject" AssociatedControlID="tbxSubject"></asp:Label>
    <asp:TextBox ID="tbxSubject" runat="server"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
        ErrorMessage="You must supply a subject" ValidationGroup="splitTopic" 
        ControlToValidate="tbxSubject">*</asp:RequiredFieldValidator>
    <br class="clearfix" />
    <input type="button" id="btnOk" onclick="SplitTopic();return false;" value="Split Topic" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
        CausesValidation="False" EnableViewState="False" OnClientClick="mainScreen.CancelModal();return false;" />
</asp:Panel>
<asp:Panel ID="pnlSort" runat="server" HorizontalAlign="Right">
    <asp:DropDownList ID="ddlSort" runat="server" >
    <asp:ListItem Selected="True" Text="Oldest first" Value="asc"></asp:ListItem>
    <asp:ListItem Text="Newest first" Value="desc"></asp:ListItem>
    </asp:DropDownList>
</asp:Panel>
<asp:Panel ID="pnlPost" runat="server" GroupingText="Original Post" Height="300px">
    <asp:HiddenField ID="splitTopicId" runat="server" />
    <asp:Panel ID="container" runat="server" ScrollBars="Auto" Height="270px" Width="100%">
    
    <asp:GridView ID="grdReplies" runat="server" AutoGenerateColumns="False" ShowHeader="false"
        EnableModelValidation="True" Width="99%" OnRowDataBound="SetCheckboxValues"
            CssClass="splittopic forumtable" DataKeyNames="Id">
        <Columns>
            <asp:BoundField DataField="Id" Visible="false" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Author.Name") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="true" CssClass="smallText" VerticalAlign="Top" Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField> 
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text="<%$ Resources:webResources,lblPostDate %>"></asp:Label><asp:Literal runat="server" Text='<%# Eval("DateTimeAgo") %>'></asp:Literal>
                <hr />
                <asp:Label runat="server" Text='<%# Eval("CleanedMessage") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Wrap="true" CssClass="smallText" VerticalAlign="Top"/>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="cbxRow" runat="server"  />
                </ItemTemplate>
                <ItemStyle Wrap="true" CssClass="smallText" VerticalAlign="Top" Width="25px"/>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </asp:Panel>
    
</asp:Panel>


