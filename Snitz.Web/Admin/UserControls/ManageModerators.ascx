<%@ Control Language="C#" AutoEventWireup="True" Inherits="Admin_ManageModerators" Codebehind="ManageModerators.ascx.cs" %>
<asp:Panel ID="Panel1" runat="server" CssClass="clearfix">
        <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td colspan="2" class="tableheader" style="width: 517px">
                    <asp:Label ID="Label1" runat="server" Text="Moderator Management"></asp:Label>
                </td>
            </tr>
            <tr>
            <td colspan="2" style="width: 517px" >
            <br />
            <asp:Label ID="Label3" runat="server" Text="&nbsp;&nbsp;View: "></asp:Label>
            <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" AutoPostBack="True">
                <asp:ListItem Selected="True" Value="0">Moderator View</asp:ListItem>
                <asp:ListItem Value="1">Forum View</asp:ListItem>
            </asp:DropDownList><br />
            </td>
            </tr>
            <tr>
                <td colspan="2">
                <asp:MultiView ID="MultiView1" runat="server">
                <asp:View ID="ModeratorView" runat="server">
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width:100%;">
                <tr><td>
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" >
                <tr>
                <td  style="width:5%;height: 45px"></td>
                <td align="left"  style="width:95%;height: 45px">
                <asp:Label ID="modLabel" runat="server" Text="Moderator: "></asp:Label>
                <asp:DropDownList ID="ModeratorList" runat="server"  AutoPostBack="True" OnSelectedIndexChanged="ModeratorList_SelectedIndexChanged">
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
                <td align="center"><asp:Label ID="ForumLabel" runat="server" Text="Moderated Forums: "></asp:Label></td>
               
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
                <td align="center"><asp:ListBox ID="MdForumsList" runat="server" Rows="8" SelectionMode="Multiple"></asp:ListBox></td>
                </tr>
                <tr style="height:80px">
                <td><asp:Button ID="SaveBtn" runat="server" Text="Save Changes" OnClick="SaveBtn_Click" CausesValidation="False" /><br />
                </td>
                <td style="width: 32px"></td>
                <td><asp:Button ID="CancelBtn" runat="server" Text="Cancel Changes" OnClick="CancelBtn_Click" CausesValidation="False" /><br /></td>
                </tr>
                <tr>
                <td colspan="3">
                <asp:RequiredFieldValidator ID="req1" runat="server"  SetFocusOnError="true" ControlToValidate="AvForumsList" Text="&nbsp" ValidationGroup=Mod1 ErrorMessage="Please select an available forum"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod1" DisplayMode="List" ID="sum1" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"  SetFocusOnError="true" ControlToValidate=MdForumsList Text="&nbsp" ValidationGroup="Mod2" ErrorMessage="Please select a moderated forum"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod2" DisplayMode="List" ID="ValidationSummary1" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
                </td></tr>
                </table>
                </td></tr>
                </table>
                </asp:View>
                <asp:View ID="ForumView" runat="server">
               <table align="left" border="0" cellpadding="0" cellspacing="0" style="width:100%;">
                <tr><td>
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" >
                <tr>
                <td  style="width:5%;height: 45px"></td>
                <td align="left"  style="width:95%;height: 45px">
                <asp:Label ID="Label2" runat="server" Text="Forum: "></asp:Label>
                <asp:DropDownList ID="ForumsList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ForumsList_SelectedIndexChanged">
                </asp:DropDownList></td>
                </tr>
                </table>
                </td>
                </tr>
                <tr>
                <td>
                              
                <table align="center" border="0" cellpadding="0" cellspacing="0" style="width: 100%;">
                <tr>
                <td align="center"><asp:Label ID="Label4" runat="server" Text="Available Moderators: "></asp:Label></td>
                <td align="center" style="width:32px"></td>
                <td align="center"><asp:Label ID="Label5" runat="server" Text="Current Moderators: "></asp:Label></td>
               
                </tr>
                <tr>
                <td align="center"><asp:ListBox ID="AvModsList" runat="server" Rows="8"  SelectionMode="Multiple" Width="120px"></asp:ListBox></td>
                <td align="center" style="width:32px">
                    <asp:Button ID="Ad2" runat="server" Text="->" Width="30px" OnClick="Ad2_Click" ValidationGroup="Mod3" /><br />
                    <asp:Button ID="Ad2All" runat="server" Text=">>" Width="30px" OnClick="Ad2All_Click" /><br />
                    <asp:Button ID="Rem2" runat="server" Text="<-" Width="30px" OnClick="Rem2_Click" ValidationGroup="Mod4" /><br />
                    <asp:Button ID="Rem2All" runat="server" Text="<<" Width="30px" OnClick="Rem2All_Click" /><br />
                    </td>
                <td align="center"><asp:ListBox ID="CurModsList" runat="server" Rows="8" Width="120px"></asp:ListBox></td>
                <td>&nbsp;</td>
                </tr>
                <tr style="height:80px">
                <td><asp:Button ID="SaveFBtn" runat="server" Text="Save Changes" OnClick="SaveFBtn_Click" /><br /></td>
                <td></td>
                <td><asp:Button ID="CancelFBtn" runat="server" Text="Cancel Changes" OnClick="CancelFBtn_Click" /><br /></td>
                <td></td>
                </tr>
                <tr>
                <td colspan="3">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="AvModsList"  SetFocusOnError="true" Text="&nbsp;" ValidationGroup="Mod3" ErrorMessage="Please select an available moderator"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod3" DisplayMode="List" ID="ValidationSummary2" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
                 <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CurModsList"  SetFocusOnError="true" Text="&nbsp;" ValidationGroup="Mod4" ErrorMessage="Please select an existing moderator"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup="Mod4" DisplayMode="List" ID="ValidationSummary3" runat="server"  ShowMessageBox="true" ShowSummary="false"/>
                </td></tr>
                </table>
                </td></tr>
                </table>
                </asp:View>
            </asp:MultiView>
            </td>
            </tr>                    
            
</table>

    </asp:Panel>
