<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollAdmin.ascx.cs" Inherits="SnitzUI.Admin.PollAdmin" %>
    <asp:SqlDataSource ID="PollDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ForumConnectionString %>"
        SelectCommand="SELECT * FROM [FORUM_Polls]" DeleteCommand="DELETE FROM FORUM_Polls WHERE PollID = @PollID" InsertCommand="INSERT INTO FORUM_Polls (DisplayText,TopicId) VALUES (@DisplayText,@TopicId)">
        <DeleteParameters>
            <asp:Parameter Name="PollID" />
        </DeleteParameters>
        <InsertParameters>
            <asp:Parameter Name="DisplayText" />
            <asp:Parameter Name="TopicId" />
        </InsertParameters>
    </asp:SqlDataSource>
<asp:Panel ID="Panel1" runat="server" CssClass="pollForm">
    <h2 class="category">Poll Configuraton</h2>
    <fieldset>
    <legend>Add Poll</legend>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="PollID"
            DataSourceID="PollDataSource" DefaultMode="Insert" 
            EnableModelValidation="True">
            <Fields>
                <asp:BoundField DataField="PollID" HeaderText="PollID" InsertVisible="False" ReadOnly="True"
                    SortExpression="PollID" />
                <asp:TemplateField HeaderText="Poll Question: ">
                    <InsertItemTemplate>
                        <asp:TextBox ID="NewPollQuestion" runat="server" Columns="75" Text='<%# Bind("DisplayText") %>'></asp:TextBox>

                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="NewPollQuestion"
                            ErrorMessage="You must enter text for the poll question" Display="Dynamic"></asp:RequiredFieldValidator>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowCancelButton="False" ShowInsertButton="True" InsertText="Add a New Poll" />
            </Fields>
        </asp:DetailsView>
    </fieldset>
    <p>
        <asp:GridView ID="PollGridView" runat="server" AutoGenerateColumns="False" DataKeyNames="PollID"
            DataSourceID="PollDataSource" CellPadding="4" ForeColor="#333333" CssClass="pollForm"
            GridLines="None" EnableModelValidation="True" HorizontalAlign="Center">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server" SkinID="EditPost"
                            NavigateUrl='<%# Eval("PollID", "/Admin/default.aspx?action=editpoll&pid={0}") %>' 
                            Text="Edit"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:ImageButton ID="LinkButton1" runat="server" CausesValidation="False" 
                            CommandName="Delete" 
                            OnClientClick="return confirm('This will permanently delete this poll and all of its results. Are you sure you want to do this?');" 
                            SkinID="DeleteMessage" Text="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PollID" HeaderText="Poll ID" InsertVisible="False" 
                    ReadOnly="True" SortExpression="PollID">
                <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Display Text" SortExpression="DisplayText">
                    <EditItemTemplate>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="pollQuestion" 
                            ErrorMessage="You must enter text for the poll question"></asp:RequiredFieldValidator>
                        <asp:TextBox ID="pollQuestion" runat="server" Columns="75" 
                            Text='<%# Bind("DisplayText") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("DisplayText") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="TopicId" HeaderText="Topic Id" 
                    NullDisplayText="(NULL)" />
                <asp:HyperLinkField DataNavigateUrlFields="PollID" DataNavigateUrlFormatString="/admin/default.aspx?action=pollresults&amp;pid={0}"
                    Text="View Results" />
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    </p>
</asp:Panel>