<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_ArchiveForums" Codebehind="ArchiveForums.ascx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Snitz.ThirdParty" Assembly="Snitz.ThirdParty" %>

<asp:Panel ID="Panel2" Visible="false" runat="server" CssClass="clearfix">

<table border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="ForumList">
              <tr style="height:50px">
                <td style="width: 100%">
                    <asp:Label ID="lblRes" runat="server" Text=""></asp:Label>
                </td>
            </tr>
 </table>           
 </asp:Panel>
<asp:Panel ID="Panel1" runat="server" CssClass="ConfigForm clearfix">

 
        <table border="0" cellpadding="0" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td class="tableheader" style="width: 100%">
                    <asp:Label ID="Label1" runat="server" Text="Archive Topics:"></asp:Label>
                </td>
            </tr>
            
            <tr>
                <td>
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width:100%;">
                <tr><td>
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" >
                <tr>
                <td width="5%" style="height: 45px"></td>
                <td align="left" width="95%" style="height: 45px">
                <asp:Label ID="dateLabel" runat="server" Text="&nbsp; Archive Topics Older than: "></asp:Label>
                <asp:DropDownList ID="dateList" runat="server">
                <asp:ListItem Value="7" Text="1 Week"></asp:ListItem>
                <asp:ListItem Value="14" Text="2 Weeks"></asp:ListItem>
                <asp:ListItem Value="21" Text="3 Weeks"></asp:ListItem>
                <asp:ListItem Value="30" Text="1 Month"></asp:ListItem>
                <asp:ListItem Value="60" Text="2 Months"></asp:ListItem>
                <asp:ListItem Value="90" Text="3 Months"></asp:ListItem>
                <asp:ListItem Value="120" Text="4 Months"></asp:ListItem>
                <asp:ListItem Value="150" Text="5 Months"></asp:ListItem>
                <asp:ListItem Value="180" Text="6 Months"></asp:ListItem>
                <asp:ListItem Value="210" Text="7 Months"></asp:ListItem>
                <asp:ListItem Value="240" Text="8 Months"></asp:ListItem>
                <asp:ListItem Value="270" Text="9 Months"></asp:ListItem>
                <asp:ListItem Value="300" Text="10 Months"></asp:ListItem>
                <asp:ListItem Value="330" Text="11 Months"></asp:ListItem>
                <asp:ListItem Value="365" Text="1 Year"></asp:ListItem>
                <asp:ListItem Value="730" Text="2 Years"></asp:ListItem>
                <asp:ListItem Value="1095" Text="3 Years"></asp:ListItem>
                </asp:DropDownList></td>
                </tr>
                </table>
                </td>
                </tr>
                <tr>
                <td>
                              
                <table align="center" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                <td align="center"><asp:Label ID="AvLabel" runat="server" Text="Available Forums: "></asp:Label></td>
                <td align="center" style="width: 32px"></td>
                <td align="center"><asp:Label ID="ForumLabel" runat="server" Text="Forums to Archive: "></asp:Label></td>
               
                </tr>
                <tr>
                <td align="center"><asp:ListBox ID="AvForumsList" runat="server" Rows="8" SelectionMode="Multiple"></asp:ListBox>&nbsp;
                 
                </td>
                <td align="right" style="width: 32px">
                    <asp:Button ID="Ad1" runat="server" Text="->" Width="30px" OnClick="Ad1_Click" ValidationGroup="Mod1" /><br />
                    <asp:Button ID="AdAll" runat="server" Text=">>" Width="30px" OnClick="AdAll_Click" CausesValidation="False" /><br />
                    <asp:Button ID="Rem1" runat="server" Text="<-" Width="30px" OnClick="Rem1_Click" ValidationGroup="Mod2" /><br />
                    <asp:Button ID="RemAll" runat="server" Text="<<" Width="30px" OnClick="RemAll_Click" CausesValidation="False" /><br />
                    </td>
                <td align="center"><asp:ListBox ID="ArchiveForumsList" runat="server" Rows="8" SelectionMode="Multiple"></asp:ListBox></td>
                </tr>
                <tr style="height:80px">
                <td><asp:Button ID="ArchiveBtn" runat="server" Text="Archive Selected Forums" CausesValidation="False" OnClick="ArchiveBtn_Click" /><br />
                </td>
                <td style="width: 32px"></td>
                <td><asp:Button ID="CancelBtn" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelBtn_Click" /><br /></td>
                </tr>
                <tr>
                <td colspan="3">
                <asp:RequiredFieldValidator ID="req1" runat="server"  SetFocusOnError="true" ControlToValidate="AvForumsList" Text="&nbsp" ValidationGroup="Mod1" ErrorMessage="Please select an available forum"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod1" DisplayMode="List" ID="sum1" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  SetFocusOnError="true" ControlToValidate="ArchiveForumsList" Text="&nbsp" ValidationGroup="Mod2" ErrorMessage="Please select a forum from the forums to archive list"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod2" DisplayMode="List" ID="ValidationSummary1" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
          
                </td></tr>
                </table>
                </td></tr>
                </table>
            </td>
            </tr>                    
            
</table>

    </asp:Panel>
    
    <asp:Panel ID="Panel3" runat="server" CssClass="ConfigForm clearfix">
        <cc1:BulkEditGridViewEx runat="server" ID="ForumList" 
            AutoGenerateColumns="False" BackColor="White" BorderColor="#DEDFDE" 
            BorderStyle="None" BorderWidth="1px" BulkEdit="False" CellPadding="4" 
            EnableInsert="False" EnableModelValidation="True" ForeColor="Black" 
            GridLines="Vertical" InsertRowCount="1" SaveButtonID="" Width="100%" OnRowDataBound="ForumList_RowDataBound">

            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="Id" ReadOnly="True">
                <HeaderStyle Width="50px" />
                </asp:BoundField>
                <asp:BoundField DataField="Subject" HeaderText="Forum" ReadOnly="True">
                <ControlStyle Width="99%" />
                </asp:BoundField>
                <asp:BoundField ApplyFormatInEditMode="True" DataField="LastArchived" 
                    DataFormatString="{0:dd/MM/yyyy}" HeaderText="Last Archived" 
                    NullDisplayText="Not Archived" ReadOnly="True">
                <ControlStyle Width="99%" />
                <HeaderStyle Width="150px" Wrap="False" />
                </asp:BoundField>
                <asp:TemplateField>
                    <EditItemTemplate>
                        <asp:Literal ID="overdue" runat="server" Visible="False">(Overdue)</asp:Literal></EditItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ArchiveFrequency" HeaderText="Schedule">
                <ControlStyle Width="99%" />
                <HeaderStyle Width="100px" />
                </asp:BoundField>
            </Columns>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
        </cc1:BulkEditGridViewEx>

    </asp:Panel>
