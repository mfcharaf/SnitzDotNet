<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollEdit.ascx.cs" Inherits="SnitzUI.Admin.EditPoll" %>
    <asp:SqlDataSource ID="PollDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ForumConnectionString %>"
        SelectCommand="SELECT * FROM [FORUM_Polls] WHERE ([PollID] = @PollID)" UpdateCommand="UPDATE FORUM_Polls SET DisplayText = @DisplayText,TopicId = @TopicId WHERE PollID = @PollID">
        <SelectParameters>
            <asp:QueryStringParameter Name="PollID" QueryStringField="pid" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="DisplayText" />
            <asp:Parameter Name="PollID" />
            <asp:Parameter Name="TopicId" />
        </UpdateParameters>
    </asp:SqlDataSource>
<asp:Panel ID="Panel1" runat="server" CssClass="pollForm">
  
    <asp:SqlDataSource ID="PollAnswersDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:ForumConnectionString %>"
        DeleteCommand="DELETE FROM [FORUM_PollAnswers] WHERE [PollAnswerID] = @PollAnswerID"
        InsertCommand="INSERT INTO [FORUM_PollAnswers] ([PollID], [DisplayText], [SortOrder]) VALUES (@PollID, @DisplayText, @SortOrder)"
        SelectCommand="SELECT * FROM [FORUM_PollAnswers] WHERE ([PollID] = @PollID) ORDER BY [SortOrder]"
        UpdateCommand="UPDATE [FORUM_PollAnswers] SET [DisplayText] = @DisplayText, [SortOrder] = @SortOrder WHERE [PollAnswerID] = @PollAnswerID">
        <DeleteParameters>
            <asp:Parameter Name="PollAnswerID" Type="Int32" />
        </DeleteParameters>
        <UpdateParameters>
            <asp:Parameter Name="DisplayText" Type="String" />
            <asp:Parameter Name="SortOrder" Type="Int32" />
            <asp:Parameter Name="PollAnswerID" Type="Int32" />
        </UpdateParameters>
        <SelectParameters>
            <asp:QueryStringParameter Name="PollID" QueryStringField="pid" Type="Int32" />
        </SelectParameters>
        <InsertParameters>
            <asp:QueryStringParameter Name="PollID" QueryStringField="pid" Type="Int32" />
            <asp:Parameter Name="DisplayText" Type="String" />
            <asp:Parameter Name="SortOrder" Type="Int32" />
        </InsertParameters>
    </asp:SqlDataSource>
    <asp:Panel ID="Panel2" runat="server" GroupingText="Question">
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="PollID"
            DataSourceID="PollDataSource" DefaultMode="Edit" OnDataBound="PollDatabound"
            EnableModelValidation="True">
            <Fields>
                <asp:TemplateField HeaderText="Poll Question">
                    <EditItemTemplate>
                        <asp:TextBox ID="EditPollDisplayText" runat="server" Text='<%# Bind("DisplayText") %>' Columns="75" ValidationGroup="EditPollQuestion"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="EditPollDisplayText"
                            Display="Dynamic" ErrorMessage="You must enter text for the poll question"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Topic Id">
                    <InsertItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TopicId") %>' Columns="6" MaxLength="6"></asp:TextBox>
                    </InsertItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("TopicId") %>' Columns="6" MaxLength="6"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("TopicId") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Featured poll">
                    <EditItemTemplate>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:CheckBox ID="cbxFeatured" runat="server" OnCheckedChanged="SetFeaturedPoll" AutoPostBack="true"></asp:CheckBox>
                        </ContentTemplate>
                        </asp:UpdatePanel>
                    
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ShowCancelButton="False" ShowEditButton="True" UpdateText="Update Poll Question" ValidationGroup="EditPollQuestion" />
            </Fields>
        </asp:DetailsView>
    </asp:Panel>
    <asp:Panel ID="Panel3" runat="server" GroupingText="Add Answer">
        <asp:DetailsView ID="PollAnswerInsert" runat="server" AutoGenerateRows="False" DataKeyNames="PollAnswerID"
            DataSourceID="PollAnswersDataSource" DefaultMode="Insert"
            EnableModelValidation="True">
            <Fields>
                <asp:TemplateField HeaderText="Answer" SortExpression="DisplayText">
                    <InsertItemTemplate>
                        <asp:TextBox ID="NewPollAnswerDisplayText" runat="server" Columns="75" Text='<%# Bind("DisplayText") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="NewPollAnswerDisplayText" ValidationGroup="AddAnswer"
                            ErrorMessage="You must enter text for the poll answer<br />" Display="Dynamic"></asp:RequiredFieldValidator>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sort Order" SortExpression="SortOrder">
                    <InsertItemTemplate>
                        <asp:TextBox ID="NewPollAnswerSortOrder" runat="server" Columns="3" Text='<%# Bind("SortOrder") %>'></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="NewPollAnswerSortOrder"
                            ErrorMessage="You must enter an integer greater than or equal to zero.<br />" Operator="GreaterThanEqual"
                            Type="Integer" ValueToCompare="0" Display="Dynamic" ValidationGroup="AddAnswer"></asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NewPollAnswerSortOrder"
                            Display="Dynamic" ErrorMessage="You must provide a sort order value." ValidationGroup="AddAnswer"></asp:RequiredFieldValidator>
                    </InsertItemTemplate>
                </asp:TemplateField>
                <asp:CommandField InsertText="Add Answer" ShowCancelButton="False" ShowInsertButton="True" ValidationGroup="AddAnswer" />
            </Fields>
        </asp:DetailsView>
    </asp:Panel>

    <p>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="PollAnswerID"
            DataSourceID="PollAnswersDataSource" CellPadding="4" CssClass="forumtable" 
            GridLines="None" EnableModelValidation="True" HorizontalAlign="Center" 
            onrowediting="GridView1_RowEditing">
            <Columns>
                <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:ImageButton ID="LinkButton1" runat="server" CausesValidation="True" SkinID="approve"
                            CommandName="Update" Text="Update"></asp:ImageButton>
                        &nbsp;<asp:ImageButton ID="LinkButton2" runat="server" CausesValidation="False" SkinID="Cancel"
                            CommandName="Cancel" Text="Cancel"></asp:ImageButton>
                    </EditItemTemplate>
                        
                    <ItemTemplate>
                        <asp:ImageButton ID="LinkButton2" runat="server" CausesValidation="False" SkinID="Edit"
                            CommandName="Edit" Text="Edit"></asp:ImageButton>
                        <asp:ImageButton ID="LinkButton3" SkinID="DeleteMessage" runat="server" CausesValidation="False" CommandName="Delete"
                            OnClientClick="return confirm('This will permanently delete this poll answer and any associated votes. Are you sure you want to do this?');"
                            Text="Delete"></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Display Text" SortExpression="DisplayText" HeaderStyle-HorizontalAlign="Left">
                    <EditItemTemplate>
                        <asp:TextBox ID="EditPollAnswerDisplayText" runat="server" Columns="75" Text='<%# Bind("DisplayText") %>' ValidationGroup="EditAnswer"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="EditPollAnswerDisplayText"
                            Display="Dynamic" ErrorMessage="You must enter text for the poll answer<br />"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("DisplayText") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sort Order" SortExpression="SortOrder">
                    <EditItemTemplate>
                        <asp:TextBox ID="EditPollAnswerSortOrder" runat="server" Columns="3" Text='<%# Bind("SortOrder") %>' ValidationGroup="EditAnswer"></asp:TextBox>
                        <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="EditPollAnswerSortOrder"
                            Display="Dynamic" ErrorMessage="You must enter an integer greater than or equal to zero.<br />"
                            Operator="GreaterThanEqual" Type="Integer" ValueToCompare="0"></asp:CompareValidator>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="EditPollAnswerSortOrder"
                            Display="Dynamic" ErrorMessage="You must enter a sort order value." ValidationGroup="EditAnswer"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("SortOrder") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle Width="100px" />
                </asp:TemplateField>
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