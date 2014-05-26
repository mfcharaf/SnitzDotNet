<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UnArchiveForums.ascx.cs" Inherits="Admin_UnArchiveForums" %>
<asp:Panel ID="Panel2" Visible="false" runat="server" CssClass="clearfix">

<table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
              <tr height="50">
                <td style="width: 100%">
                    <asp:Label ID="lblRes" runat="server" Text=""></asp:Label>
                </td>
            </tr>
 </table>           
 </asp:Panel>
<asp:Panel ID="Panel1" runat="server" CssClass="clearfix">
 
        <table border="0" cellpadding="2" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td colspan="2" class="tableheader" style="width: 100%">
                    <asp:Label ID="Label1" runat="server" Text="UnArchive Topics:"></asp:Label>
                </td>
            </tr>
            
            <tr>
                <td colspan="2">
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width:100%;">
                <tr><td>
                <table align="left" border="0" cellpadding="0" cellspacing="0" style="width: 100%;" >
                <tr>
                <td width="5%" style="height: 45px"></td>
                <td align="left" width="95%" style="height: 45px">
                <asp:Label ID="dateLabel" runat="server" Text="&nbsp; UnArchive Topics Not Older than: "></asp:Label>
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
                <td align="center"><asp:Label ID="ForumLabel" runat="server" Text="Forums to UnArchive: "></asp:Label></td>
               
                </tr>
                <tr>
                <td align="center"><asp:ListBox ID="AvForumsList" runat="server" Rows="8" SelectionMode="Multiple"></asp:ListBox>&nbsp;
                 
                </td>
                <td align="center" style="width: 32px">
                    <asp:Button ID="Ad1" runat="server" Text="->" Width="30px" OnClick="Ad1_Click" ValidationGroup="Mod1" /><br />
                    <asp:Button ID="AdAll" runat="server" Text=">>" Width="30px" OnClick="AdAll_Click" CausesValidation="False" /><br />
                    <asp:Button ID="Rem1" runat="server" Text="<-" Width="30px" OnClick="Rem1_Click" ValidationGroup="Mod2" />&nbsp;
                    <br />
                    <asp:Button ID="RemAll" runat="server" Text="<<" Width="30px" OnClick="RemAll_Click" CausesValidation="False" /><br />
                    </td>
                <td align="center"><asp:ListBox ID="UnArchiveForumsList" runat="server" Rows="8" SelectionMode="Multiple"></asp:ListBox></td>
                </tr>
                <tr height="80">
                <td><asp:Button ID="ArchiveBtn" runat="server" Text="UnArchive Selected Forums" CausesValidation="False" OnClick="ArchiveBtn_Click" /><br />
                </td>
                <td style="width: 32px"></td>
                <td><asp:Button ID="CancelBtn" runat="server" Text="Cancel" CausesValidation="False" OnClick="CancelBtn_Click" /><br /></td>
                </tr>
                <tr>
                <td colspan="3">
                <asp:RequiredFieldValidator ID=req1 runat=server  SetFocusOnError=true ControlToValidate=AvForumsList Text="&nbsp" ValidationGroup=Mod1 ErrorMessage="Please select an available forum"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup=Mod1 DisplayMode=List ID=sum1 runat=server  ShowMessageBox=true ShowSummary=false/>
                 <asp:RequiredFieldValidator ID=RequiredFieldValidator1 runat=server  SetFocusOnError=true ControlToValidate=UnArchiveForumsList Text="&nbsp" ValidationGroup=Mod2 ErrorMessage="Please select a forum from the forums to unarchive list"></asp:RequiredFieldValidator>
                <asp:ValidationSummary ValidationGroup=Mod2 DisplayMode=List ID=ValidationSummary1 runat=server  ShowMessageBox=true ShowSummary=false/>
          
                </td></tr>
                </table>
                </td></tr>
                </table>
            </td>
            </tr>                    
            
</table>

    </asp:Panel>
