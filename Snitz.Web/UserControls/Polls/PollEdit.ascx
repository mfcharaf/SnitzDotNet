<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PollEdit.ascx.cs" Inherits="SnitzUI.Admin.EditPoll" %>
<%@ Register TagPrefix="cc1" Namespace="Snitz.ThirdParty" Assembly="Snitz.Controls" %>
<asp:Panel ID="Panel1" runat="server" CssClass="pollForm">
  
    <asp:Panel ID="Panel2" runat="server" GroupingText="Question">
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" DataKeyNames="Id"
            DefaultMode="Edit" OnDataBound="PollDatabound"
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
            DefaultMode="Insert"
            EnableModelValidation="True" 
            oniteminserting="PollAnswerInsert_ItemInserting">
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
        <cc1:BulkEditGridViewEx ID="GridView1" runat="server" 
            AutoGenerateColumns="False" DataKeyNames="Id"
            CellPadding="4" CssClass="forumtable" 
            GridLines="None" EnableModelValidation="True" HorizontalAlign="Center" 
            onrowediting="GridView1_RowEditing" onrowupdating="GridView1_RowUpdating" 
            BulkEdit="False" EnableInsert="False" InsertRowCount="1" SaveButtonID="">
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
                <asp:BoundField DataField="Id" ShowHeader="False" ReadOnly="True" >
                <ControlStyle Width="25px" />
                <HeaderStyle Width="30px" />
                </asp:BoundField>
                <asp:BoundField DataField="DisplayText" HeaderText="Display Text" 
                    HeaderStyle-HorizontalAlign="Left">
                <HeaderStyle HorizontalAlign="Left" Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="Order" HeaderText="Sort Order" >
                <HeaderStyle Width="100px" />
                </asp:BoundField>
            </Columns>
            <EditRowStyle BackColor="#999999" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </cc1:BulkEditGridViewEx>
    </p>

</asp:Panel> 