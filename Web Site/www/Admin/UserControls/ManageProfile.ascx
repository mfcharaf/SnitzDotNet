<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageProfile.ascx.cs" Inherits="SnitzUI.Admin.Admin_ManageProfile" %>
<asp:Panel ID="Panel1" runat="server" GroupingText="Column Properties" CssClass="forumtable profileform">
    <asp:Label ID="Label1" runat="server" Text="Name" AssociatedControlID="colName"></asp:Label>
    <asp:TextBox ID="colName" runat="server"></asp:TextBox>
    <br />
    <asp:Label ID="Label2" runat="server" Text="Type" AssociatedControlID="colType"></asp:Label>
    <asp:DropDownList ID="colType" runat="server">
        <asp:ListItem Text="Varchar" Value="nvarchar"></asp:ListItem>
        <asp:ListItem Text="Text" Value="ntext"></asp:ListItem>
        <asp:ListItem Text="Integer" Value="int"></asp:ListItem>
        <asp:ListItem Text="Boolean" Value="smallint"></asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Label ID="Label3" runat="server" Text="Size" AssociatedControlID="colSize"></asp:Label>
    <asp:TextBox ID="colSize" runat="server" MaxLength="3" Columns="4"></asp:TextBox>&nbsp;
    <asp:CheckBox ID="colNulls" runat="server" Text="Allow Null" />
    <br />
    <asp:Label ID="Label4" runat="server" Text="Default Value" AssociatedControlID="colDefault"></asp:Label>
    <asp:TextBox ID="colDefault" runat="server"></asp:TextBox>
    <br />
    <br />
</asp:Panel>

<asp:Panel ID="Panel2" runat="server" GroupingText="Profile Columns" CssClass="forumtable">
    <asp:GridView ID="GridView1" runat="server" OnRowDataBound="ProfileGridDataBound" 
    AutoGenerateColumns="False" EnableModelValidation="True" 
        onrowcommand="GridView1_RowCommand" Width="75%" style="margin:auto;">
        <Columns>
            <asp:BoundField DataField="ColumnName" 
            HeaderText="Col Name" />
            <asp:BoundField DataField="DataType" 
            HeaderText="Type" />
            <asp:BoundField DataField="Precision" 
            HeaderText="Size" />
            <asp:BoundField DataField="AllowNull" 
            HeaderText="AllowNull" />
            <asp:BoundField DataField="DefaultValue" 
            HeaderText="Default" />
            <asp:TemplateField InsertVisible="False" ShowHeader="False">
                <EditItemTemplate>
                    <asp:ImageButton ID="LinkButton1" runat="server" CausesValidation="True" SkinID="approve"
                        CommandName="Update" Text="Update"></asp:ImageButton>
                    &nbsp;<asp:ImageButton ID="LinkButton2" runat="server" CausesValidation="False" SkinID="Cancel"
                        CommandName="Cancel" Text="Cancel"></asp:ImageButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:ImageButton ID="LinkButton1" runat="server" CausesValidation="False" SkinID="Edit"
                        CommandName="Select" Text="Select"></asp:ImageButton>
                    &nbsp;<asp:ImageButton ID="LinkButton2" runat="server" CausesValidation="False" SkinID="DeleteMessage"
                        CommandName="Delete" Text="Delete"></asp:ImageButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <HeaderStyle CssClass="category" />
    </asp:GridView>
</asp:Panel>


