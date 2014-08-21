<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_system" Codebehind="system.ascx.cs" %>
    <%@ Register src="AdminRadioButton.ascx" tagname="AdminRadioButton" tagprefix="uc1" %>    
<%@ Register TagPrefix="a" Namespace="SnitzCommon.Controls" Assembly="SnitzCommon" %>
<asp:Panel ID="Panel1" runat="server" CssClass="clearfix" DefaultButton="btnSubmit">
        <table border="0" cellpadding="3" cellspacing="0" style="width: 100%;" class="forumtable white">
            <tr>
                <td colspan="2" class="category">
                    <asp:Label ID="L1" runat="server" Text="Main Forum Configuration"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; text-align: right">
                    <asp:Label ID="L3" runat="server" Text="Forum's Title"></asp:Label>&nbsp;:&nbsp;</td>
                <td align="left" valign="top">
                    <asp:TextBox ID="tbxTitle" runat="server" Width="80%"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="L2" runat="server" Text="Forum's Copyright"></asp:Label>&nbsp;:&nbsp;</td>
                <td align="left" valign="top">
                    <asp:TextBox ID="tbxCopyright" runat="server" Width="55%"></asp:TextBox></td>
            </tr>            
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblHomeUrl" runat="server" Text="Home URL"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:TextBox ID="tbxHomeUrl" runat="server" Width="75%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblForumUrl" runat="server" Text="Forum URL"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:TextBox ID="tbxForumUrl" runat="server" Width="75%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblVersion" runat="server" Text="Snitz Version"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:Label ID="tbxVersion" runat="server" Width="50%"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr/>
                </td>
            </tr>
            <tr>
                <td style="width: 50%; text-align: right">
                    <asp:Label ID="Label1" runat="server" Text="Default theme"></asp:Label>
                    :&nbsp;
                </td>
                <td style="width: 50%; text-align: left">
                    <a:ThemeDropDownList ID="ddTheme" runat="server" AutoPostBack="False"></a:ThemeDropDownList>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="Label2" runat="server" Text="Layout"></asp:Label>:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:CheckBox ID="chkRightColum" runat="server" Text="Show right column" />
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;"><asp:Label ID="Label5" runat="server" Text="Show sidebar Ads"></asp:Label>:&nbsp;</td>
                <td style="width: 50%;text-align:left">
                    
                    <asp:CheckBox ID="chkShowSideAds" runat="server" Text="(requires right column enabled)" />
                    
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;"><asp:Label ID="Label3" runat="server" Text="Show header Ads"></asp:Label>:&nbsp;</td>
                <td style="width: 50%;text-align:left">
                    
                    <asp:CheckBox ID="chkShowHeaderAds" runat="server" />
                    
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;"><asp:Label ID="Label4" runat="server" Text="Use Google Ads"></asp:Label>:&nbsp;</td>
                <td style="width: 50%;text-align:left">
                    
                    <asp:CheckBox ID="chkGoogleAds" runat="server" />
                    
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;"><asp:Label ID="Label6" runat="server" Text="Google AdCode"></asp:Label>:&nbsp;</td>
                <td style="width: 50%;text-align:left">
                    
                    <asp:TextBox ID="tbxAdCode" runat="server" Width="189px"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr/>
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblNoNewMembers" runat="server" Text="Prohibit New Members"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <uc1:AdminRadioButton ID="rblNoNewMembers" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblRequireReg" runat="server" Text="Require Registration"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <uc1:AdminRadioButton ID="rblRequireReg" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 50%;text-align:right;">
                    <asp:Label ID="lblUserFilter" runat="server" Text="UserName Filter"></asp:Label>&nbsp;:&nbsp;
                </td>
                <td style="width: 50%;text-align:left">
                    <uc1:AdminRadioButton ID="rblUserFilter" runat="server" />
                </td>
            </tr>

            <tr>
                <td style="width: 50%;text-align:right;">
                </td>
                <td style="width: 50%;text-align:left">
                    <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />&nbsp;
                    <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />    

                </td>
            </tr>
            </table>
    </asp:Panel>
