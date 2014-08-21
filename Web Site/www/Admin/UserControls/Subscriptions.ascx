<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Subscriptions.ascx.cs"
    Inherits="Admin_Subscriptions" %>
<script type="text/javascript">
    //<![CDATA[
    function CheckAll(oCheckbox) {
        var GridView2 = document.getElementById("<%=grdSubs.ClientID %>");
        for (i = 1; i < GridView2.rows.length; i++) {
            if (GridView2.rows[i].cells.length > 2)
         GridView2.rows[i].cells[2].getElementsByTagName("INPUT")[0].checked = oCheckbox.checked;
        }

 }

 //]]>
 </script>


<asp:Panel ID="Panel1" runat="server" GroupingText="Filter">
    <asp:RadioButton ID="rbAll" runat="server" Text="All Subscriptions" AutoPostBack="True" Checked="True" GroupName="subFilter" OnCheckedChanged="CheckedChanged" />
    <br />
    <asp:RadioButton ID="rbBoard" runat="server" Text="Board Subscriptions" AutoPostBack="True" GroupName="subFilter" OnCheckedChanged="CheckedChanged" />
    <br />
    <asp:RadioButton ID="rbCategory" runat="server" Text="Category Subscriptions" AutoPostBack="True" GroupName="subFilter" OnCheckedChanged="CheckedChanged" />
    <br />
    <asp:RadioButton ID="rbForum" runat="server" Text="Forum Subscriptions" AutoPostBack="True" GroupName="subFilter" OnCheckedChanged="CheckedChanged" />
    <br />
    <asp:RadioButton ID="rbTopic" runat="server" Text="Topic Subscriptions" AutoPostBack="True" GroupName="subFilter" OnCheckedChanged="CheckedChanged" />
</asp:Panel>
<br/>

<asp:GridView ID="grdSubs" runat="server" AutoGenerateColumns="False" 
    CellPadding="2" 
    DataKeyNames="CategoryId,ForumId,TopicId,MemberId" EnableModelValidation="True" 
    CssClass="forumtable">
    <Columns>
        <asp:BoundField DataField="CategoryName" HeaderText="Category" />
        <asp:BoundField DataField="ForumSubject" HeaderText="Forum" HtmlEncode="False" />
        <asp:BoundField DataField="TopicSubject" HeaderText="Topic" HtmlEncode="False" />
        <asp:BoundField DataField="MemberName" HeaderText="User" />
        <asp:TemplateField ShowHeader="False">
            <ItemTemplate>
                <input type="checkbox" ID="cbxDel" />
            </ItemTemplate>
            <HeaderTemplate>
                <input type="checkbox" onclick="CheckAll(this)" id="chkAll"/>
            </HeaderTemplate>
            <HeaderStyle Width="20px" />
            <ControlStyle CssClass="msgIcons" />
        </asp:TemplateField>
    </Columns>
    <HeaderStyle CssClass="category" />
</asp:GridView>
<asp:LinkButton runat="server" ID="btnDelete" SkinID="Delete" Text="Delete Selected" OnClick="btnDelete_Click"/>
