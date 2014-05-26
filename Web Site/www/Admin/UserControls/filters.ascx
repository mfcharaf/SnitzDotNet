<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_filters" Codebehind="filters.ascx.cs" %>
<asp:Panel ID="Panel4" runat="server" CssClass="ConfigForm clearfix" Visible="True">
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td colspan="2" class="category">
                    <asp:Label ID="Label1" runat="server" Text="Word Filters Configuration"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2"><br />
    <asp:Panel ID="Panel1" runat="server" Visible="False" DefaultButton="btnNewWordFilter">
        <fieldset title="New Filter" style="width:80%;margin:auto;" class="snitzform">
        <legend>New bad word Filter</legend>
        <asp:Label ID="Label4" runat="server" Text="Bad word" AssociatedControlID="tbxBadWord"></asp:Label>&nbsp;
        <asp:TextBox ID="tbxBadWord" runat="server"></asp:TextBox><br />
        <asp:Label ID="Label3" runat="server" Text="Replace with" AssociatedControlID="tbxReplace"></asp:Label>&nbsp;
        <asp:TextBox ID="tbxReplace" runat="server"></asp:TextBox><br />
        <asp:LinkButton ID="btnNewWordFilter" runat="server" OnClick="btnNewWordFilter_Click" Text="Add" />
        </fieldset><br />
                    <asp:GridView ID="GridView1" SkinID="bwfilterGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="Id"
                        DataSourceID="dsWordFilters" Width="80%" CssClass="ForumList" style="margin:auto;">
                        <Columns>
                            <asp:BoundField DataField="Badword" HeaderText="BadWord to Filter" >
                                <ItemStyle Width="40%" Wrap="False" />
                            </asp:BoundField>
                            <asp:BoundField DataField="Replace" HeaderText="Replace With" >
                                <ItemStyle Wrap="False" />
                            </asp:BoundField>
                            <asp:CommandField  />
                        </Columns>
                        <HeaderStyle CssClass="category" />

                    </asp:GridView>
                    <asp:ObjectDataSource ID="dsWordFilters" runat="server" SelectMethod="GetAllBadwords"
                        TypeName="Snitz.BLL.Filters" DeleteMethod="DeleteBadWord" InsertMethod="AddBadword" UpdateMethod="UpdateBadWord">
                        <DeleteParameters>
                            <asp:Parameter Name="Id" Type="Int32" />
                        </DeleteParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="Id" Type="Int32" />
                            <asp:Parameter Name="Badword" Type="String" />
                            <asp:Parameter Name="Replace" Type="String" />
                        </UpdateParameters>
                        <InsertParameters>
                            <asp:Parameter Name="Badword" Type="String" DefaultValue="new badword" />
                            <asp:Parameter Name="Replace" Type="String" DefaultValue="********" />
                        </InsertParameters>
                    </asp:ObjectDataSource>
        <br />
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Visible="False" >
        <fieldset title="New name filter"  style="width:80%;margin:auto" >
        <legend>New name filter</legend>
        <asp:Label ID="nfLabel" runat="server" Text="Name to filter " AssociatedControlID="tbxNameFilter"></asp:Label>
        <asp:TextBox ID="tbxNameFilter" runat="server"></asp:TextBox>&nbsp;
        <asp:LinkButton ID="btnNewNameFilter" runat="server" Text="Add" OnClick="btnNewNameFilter_Click" />
        </fieldset><br />
        <asp:GridView ID="GridView2" SkinID="namefilterGrid"  CellPadding="2" runat="server" AutoGenerateColumns="False" DataSourceID="dsNameFilter" Width="80%" style="margin:auto;"  CssClass="ForumList" DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Username to Filter" >
                    <ItemStyle Wrap="False" HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:CommandField  />
            </Columns>
            <HeaderStyle CssClass="category" />
        </asp:GridView>
        <asp:ObjectDataSource ID="dsNameFilter" runat="server" SelectMethod="GetAllNameFilters"
            TypeName="Snitz.BLL.Filters" DeleteMethod="DeleteNameFilter" InsertMethod="AddNameFilter" UpdateMethod="UpdateNameFilter">
            <DeleteParameters>
                <asp:Parameter Name="Id" Type="Int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="Id" Type="Int32" />
                <asp:Parameter Name="Name" Type="String" />
            </UpdateParameters>
            <InsertParameters>
                <asp:Parameter Name="Name" Type="String" DefaultValue="name filter" />
            </InsertParameters>
        </asp:ObjectDataSource>
        <br />
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server"  Visible="False" >
        <asp:Label ID="Label2" runat="server" Text="There was an error!"></asp:Label>
    </asp:Panel>
                </td>
            </tr>           
            <tr>
                <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">&nbsp;</td>
                <td style="width: 50%;text-align:left">&nbsp;</td>
            </tr>
        </table>
</asp:Panel>
