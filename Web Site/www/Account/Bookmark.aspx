<%@ Page Language="C#" Culture="auto" UICulture="auto" CodeBehind="Bookmark.aspx.cs" 
    Inherits="SnitzUI.Bookmark" MasterPageFile="~/MasterTemplates/MainMaster.Master" 
    AutoEventWireup="true" %>
<%@ Import Namespace="Snitz.BLL" %>


<asp:Content ID="Content1" runat="server" contentplaceholderid="CPM">
    <asp:Repeater ID="repBookMarks" runat="server">
        <ItemTemplate>
            <asp:ImageButton ID="ImageButton1" SkinID="DeleteMessage" Width="16px" Height="16px" OnClick="DeleteBookMark"
                OnClientClick="setArgAndPostBack('Do you want to delete the bookmark?','DeleteBookMark','');return false;"
                runat="server" ToolTip="Delete BookMark" CommandArgument='<%# Eval("ID") %>' /><asp:HyperLink ID="hypLink" runat="server"
                    Text='<%# Eval("Name").ToString().ParseTags() %>' NavigateUrl='<%# Eval("Url") %>'></asp:HyperLink><br />
        </ItemTemplate>
    </asp:Repeater>
</asp:Content>



