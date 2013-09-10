<%-- 
##############################################################################################################
## Snitz Forums .net
##############################################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##############################################################################################################
--%>

<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/SingleCol.Master" AutoEventWireup="true"
    Title="<%$ Resources:webResources, ttlProfilePage %>" Culture="auto" UICulture="auto"
    CodeBehind="Profile.aspx.cs" Inherits="SnitzUI.ProfilePage" %>

<%@ Import Namespace="Snitz.BLL" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/DatePicker.ascx" TagName="DatePicker" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHead" runat="server">

    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS" />
    <link rel="stylesheet" type="text/css" href="/css/jquery.lightbox-0.5.css" media="screen" />
    <link rel="stylesheet" type="text/css" runat="server" id="pageCSS" />

    <script src="/scripts/editor.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="/scripts/jquery.lightbox-0.5.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#ctl00_CPM_TabContainer1_TabSig_tbxSig").markItUp(mySettings);
            $("#ctl00_CPM_TabContainer1_TabSig_tbxQuote").markItUp(mySettings);
            $("#ctl00_CPM_TabContainer1_TabSig_tbxBiog").markItUp(mySettings);
            $("#ctl00_CPM_TabContainer1_TabHobby_tbxHobby").markItUp(mySettings);
            $("#ctl00_CPM_TabContainer1_TabHobby_tbxNews").markItUp(mySettings);
        });
        $(function () {
            $('.gallerylnk').lightBox();
        });
    </script>
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="CPH1" runat="server">
    <script type="text/javascript">
        function ShowUpload() {
            $find('mpUpload').show();
        }
        function UploadDone() {
            location.reload();
            $find('mpUpload').hide();
        }
        var clientMsgId = '<%= clientSide.ClientID %>';
        var imgTagId = '<%= imageTag.ClientID %>';
        var errLabelId = '<%= errLabel.ClientID %>';

        function addToClientTable(name, text) {
            var table = $get(clientMsgId);
            var row = table.insertRow(0);
            fillCell(row, 0, name);
            fillCell(row, 1, text);
        }
        function fillCell(row, cellNumber, text) {
            var cell = row.insertCell(cellNumber);
            cell.innerHTML = text;
            cell.style.borderBottom = cell.style.borderRight = "solid 1px #aaaaff";
        }
        function uploadError(sender, args) {
            addToClientTable(args.get_fileName(), "<span style='color:red;'>" + args.get_errorMessage() + "</span>");
            return false;
        }
        function uploadComplete(sender, args) {
            var contentType = args.get_contentType();

            try {
                var fileExtension = args.get_fileName();
                if (fileExtension.indexOf('.pdf') != -1 || contentType.indexOf('image') < 0) {
                    $get("errDiv").style.display = 'block';
                    $get(errLabelId).innerHTML = "File type not permitted";
                    $get(clientMsgId).style.display = 'none';
                    return false;
                }
            } catch (e) { alert(e.Message); }
            try {

                if (parseInt(args.get_length()) > 2000000) {
                    $get("errDiv").style.display = 'block';
                    $get(errLabelId).innerHTML = "File should be less than 2Mb";
                    $get(clientMsgId).style.display = 'none';
                    return false;
                }
            } catch (e) { alert(e.Message); }

            var text = args.get_length() + " bytes";
            if (contentType.length > 0) {
                text += ", '" + contentType + "'";
            }
            addToClientTable(args.get_fileName(), text);

        }
    </script>
</asp:Content>
<asp:Content ID="adOverride" runat="server" ContentPlaceHolderID="CPAd">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHR" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="CPHL" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="CPM" runat="server">
    <div id="profileDIV" class="profileform">
        <asp:UpdatePanel ID="profileupd" runat="server" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="1"
                    CssClass="ajax__tab_xp2" AutoPostBack="false"
                    OnActiveTabChanged="TabContainer1ActiveTabChanged">
                    <asp:TabPanel runat="server" HeaderText="<%$ Resources:webResources, lblProfile %>"
                        ID="TabProfile">
                        <ContentTemplate>
                            <div id="pMain">
                                <asp:Panel runat="server" ID="pnlAvatar" GroupingText="<%$ Resources:webResources, lblAvatar %>">
                                    <asp:PlaceHolder ID="phAvatar" runat="server"></asp:PlaceHolder>
                                    <br />
                                    <asp:LinkButton ID="btnAvatar" Text="<%$ Resources:webResources, btnUploadAvatar %>" runat="server" OnClientClick="ShowUpload();return false;"
                                        AlternateText="<%$ Resources:webResources, btnUploadAvatar %>" /><br />
                                    <asp:CheckBox ID="cbxGravatar" runat="server" Text="Use Gravatar image" />
                                </asp:Panel>
                                <asp:Panel ID="account" runat="server" GroupingText="<%$ Resources:webResources, lblProfile %>">
                                    <ol>
                                    <li><asp:Label ID="lblName" runat="server" Text="<%$ Resources:webResources, lblUserName %>"
                                        AssociatedControlID="tbxName" EnableViewState="False"></asp:Label><asp:TextBox ID="tbxName"
                                            runat="server" EnableViewState="False" Enabled="<%# IsAdministrator %>"></asp:TextBox></li>
                                    <li><asp:Label ID="lblRealName" runat="server" AssociatedControlID="tbxRealName" Text="<%$ Resources:webResources, lblName %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxRealName" runat="server"></asp:TextBox></li>
                                    <li><asp:Label ID="lblAge" runat="server" AssociatedControlID="tbxAge" Text="<%$ Resources:webResources, lblAge %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxAge" runat="server"></asp:TextBox></li>
                                    <li><asp:Label ID="lblMarStatus" AssociatedControlID="ddlMarStatus" runat="server" Text="<%$ Resources:webResources, lblMarStatus %>"
                                        EnableViewState="False"></asp:Label><asp:DropDownList ID="ddlMarStatus" runat="server">
                                            <asp:ListItem Value=" " meta:resourcekey="ddlMarStatus">[Select]</asp:ListItem>
                                            <asp:ListItem Value="Married" meta:resourcekey="ddlMarried">Married</asp:ListItem>
                                            <asp:ListItem Value="Single" meta:resourcekey="ddlSingle">Single</asp:ListItem>
                                            <asp:ListItem Value="Divorced" meta:resourcekey="ddlDivorced">Divorced</asp:ListItem>
                                            <asp:ListItem Value="Withheld" meta:resourcekey="ddlWithheld">Withheld</asp:ListItem>
                                        </asp:DropDownList></li>
                                    <li><asp:Label ID="lblGender" AssociatedControlID="ddlGender" runat="server" Text="<%$ Resources:webResources, lblGender %>"
                                        EnableViewState="False"></asp:Label><asp:DropDownList ID="ddlGender" runat="server"
                                            SkinID="sknGenderList">
                                            <asp:ListItem Text="[Select Gender]" meta:resourcekey="ddlGender" />
                                            <asp:ListItem Value="Male" meta:resourcekey="ddlMale">Male</asp:ListItem>
                                            <asp:ListItem Value="Female" meta:resourcekey="ddlFemale">Female</asp:ListItem>
                                        </asp:DropDownList>
                                    </li>
                                    <li><asp:Label ID="lblState" AssociatedControlID="tbxState" runat="server" Text="<%$ Resources:webResources, lblState %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxState" runat="server" EnableViewState="False"></asp:TextBox></li>
                                    <li><asp:Label ID="lblCity" AssociatedControlID="tbxCity" runat="server" Text="<%$ Resources:webResources, lblCity %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxCity" runat="server" EnableViewState="False"></asp:TextBox></li>
                                    <li><asp:Label ID="lblCountry" AssociatedControlID="tbxCountry" runat="server" Text="<%$ Resources:webResources, lblCountry %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxCountry" runat="server" EnableViewState="False"></asp:TextBox></li>
                                    <li><asp:Label ID="lblOccupation" AssociatedControlID="tbxOccupation" runat="server"
                                        Text="<%$ Resources:webResources, lblOccupation %>" EnableViewState="False"></asp:Label><asp:TextBox
                                            ID="tbxOccupation" runat="server" EnableViewState="False"></asp:TextBox></li>
                                    <li><asp:Label ID="lblTitle" AssociatedControlID="tbxForumTitle" runat="server" Text="Forum Title"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxForumTitle" runat="server"
                                            EnableViewState="False"></asp:TextBox></li>
                                    <li><asp:CheckBox ID="cbxReceiveEmail" CssClass="email" runat="server" Text="<%$ Resources:webResources, lblReceiveEmail %>" /></li>
                                        <li><asp:CheckBox ID="cbxHideEmail" CssClass="email" runat="server" Text="<%$ Resources:webResources, lblHideEmail %>" /></li>
                                        
                                        <li><asp:TextBox ID="newemail" runat="server">Enter a new Email ..</asp:TextBox>&#160;<asp:LinkButton
                                            ID="btnChangeEmail" runat="server" Text="<%$ Resources:webResources, lblChangeEmail %>"
                                            OnClick="ChangeEmailClick" /></li>
                                        <li><asp:Label runat="server" ID="Lemail" Text="Your new email will become current when you have finished the email validation process"
                                            Visible="False" EnableViewState="False"></asp:Label></li>
                                        </ol>
                                </asp:Panel>
                            </div>
                            <asp:Panel runat="server" ID="pnlPassword" GroupingText="Password" Style="float: right; width: 49%; height: auto; min-height: 100px;">
                                <asp:Label ID="lblOldPass" runat="server" Text="Old Password" AssociatedControlID="tbxPassword"></asp:Label><asp:TextBox
                                    ID="tbxPassword" runat="server" ValidationGroup="passChange"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator1" runat="server" ErrorMessage="You must supply your  existing password"
                                        ControlToValidate="tbxPassword" SetFocusOnError="True" ValidationGroup="passChange">*</asp:RequiredFieldValidator><br />
                                <asp:Label ID="lblNewPass" runat="server" Text="New Password" AssociatedControlID="tbxNewPass"></asp:Label><asp:TextBox
                                    ID="tbxNewPass" runat="server" ValidationGroup="passChange"></asp:TextBox><asp:RequiredFieldValidator
                                        ID="RequiredFieldValidator2" runat="server" ControlToValidate="tbxNewPass" ErrorMessage="New Password is required"
                                        ValidationGroup="passChange">*</asp:RequiredFieldValidator><br />
                                <asp:Label ID="lblConfirm" runat="server" Text="Confirm Password" AssociatedControlID="tbxConfirmPass"></asp:Label><asp:TextBox
                                    ID="tbxConfirmPass" runat="server" ValidationGroup="passChange"></asp:TextBox><asp:CompareValidator
                                        ID="CompareValidator1" runat="server" ControlToCompare="tbxNewPass" ControlToValidate="tbxConfirmPass"
                                        ErrorMessage="Passwords do not match" ValidationGroup="passChange">*</asp:CompareValidator><br />
                                <asp:LinkButton ID="btnChangePass" runat="server" ValidationGroup="passChange" Text="Change Password"
                                    OnClick="BtnChangePassClick" /><br />
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="passChange" />
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlDOB" GroupingText="Date of Birth" Style="float: right; width: 49%; height: auto; min-height: 130px;">
                                <uc2:DatePicker ID="DatePicker1" runat="server" />
                                <br />
                                <p>
                                    <asp:Literal ID="L2" runat="server" Text="
                &lt;br /&gt;Only the administrator will have access to your Date of Birth, normal users will only see your age unless the hide age option is selected.&lt;br /&gt; "
                                        EnableViewState="False"></asp:Literal>
                                </p>
                                <asp:CheckBox ID="cbxHideAge" runat="server" Text="<%$ Resources:webResources, lblHideAge %>" />
                            </asp:Panel>
                            <div style="z-index: 101; float: right; width: 49%; margin-right: 0px;">
                                <asp:Panel ID="pnlSiteInf" runat="server" GroupingText="<%$ Resources:webResources, lblSiteInfo %>">
                                    <asp:Label ID="lblTimeOffset" AssociatedControlID="ddlTimeZone" runat="server" Text="<%$ Resources:webResources, lblTimeOffset %>"
                                        EnableViewState="False"></asp:Label><asp:DropDownList ID="ddlTimeZone" runat="server">
                                        </asp:DropDownList>
                                    <br />
                                    <asp:Label ID="lblTheme" AssociatedControlID="ddlTheme" runat="server" Text="<%$ Resources:webResources, lblThemes %>"
                                        EnableViewState="False"></asp:Label><asp:DropDownList ID="ddlTheme" runat="server">
                                            <asp:ListItem Value="BlueGray" Text="BlueGray" />
                                            <asp:ListItem Value="Dark" Text="Dark" />
                                            <asp:ListItem Value="Light" Text="Light" />
                                        </asp:DropDownList>
                                    <br />
                                    <asp:Label ID="lblLanguage" runat="server" Text="Default Language:" AssociatedControlID="ddlLang"
                                        EnableViewState="False"></asp:Label><asp:DropDownList runat="server" ID="ddlLang">
                                            <asp:ListItem Value="en" />
                                            <asp:ListItem Value="fr" />
                                            <asp:ListItem Value="de" />
                                            <asp:ListItem Value="it" />
                                            <asp:ListItem Value="en-ie" />
                                            <asp:ListItem Value="ja" />
                                            <asp:ListItem Value="fa" />
                                        </asp:DropDownList>
                                    <br />
                                    <asp:Label ID="lblLinkTarget" runat="server" AssociatedControlID="ddlTarget" Text="Open Links in"
                                        EnableViewState="False"></asp:Label><asp:DropDownList runat="server" ID="ddlTarget">
                                            <asp:ListItem Value="_blank" Text="New window" />
                                            <asp:ListItem Value="_self" Text="Same window" />
                                        </asp:DropDownList>
                                    <br />
                                </asp:Panel>
                                <asp:Panel ID="contactInfo" runat="server" GroupingText="<%$ Resources:webResources, lblContactInfo %>">
                                    <asp:Label ID="lblAIM" AssociatedControlID="tbxAIM" runat="server" Text="<%$ Resources:webResources, lblAIM %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxAIM" runat="server" EnableViewState="False"></asp:TextBox><br />
                                    <asp:Label ID="lblSkype" AssociatedControlID="tbxSkype" runat="server" Text="<%$ Resources:webResources, lblSKYPE %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxSkype" runat="server" EnableViewState="False"></asp:TextBox><br />
                                    <asp:Label ID="lblICQ" AssociatedControlID="tbxICQ" runat="server" Text="<%$ Resources:webResources, lblICQ %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxICQ" runat="server" EnableViewState="False"></asp:TextBox><br />
                                    <asp:Label ID="lblYAHOO" AssociatedControlID="tbxYAHOO" runat="server" Text="<%$ Resources:webResources, lblYAHOO %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxYAHOO" runat="server" EnableViewState="False"></asp:TextBox><br />
                                    <asp:Label ID="lblMSN" AssociatedControlID="tbxMSN" runat="server" Text="<%$ Resources:webResources, lblMSN %>"
                                        EnableViewState="False"></asp:Label><asp:TextBox ID="tbxMSN" runat="server" EnableViewState="False"></asp:TextBox><br />
                                </asp:Panel>
                            </div>

                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabSig" runat="server" HeaderText="Signature/Bio">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlSig" GroupingText="<%$ Resources:webResources, lblSig %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:CheckBox ID="cbxViewSig" runat="server" Text="<%$ Resources:webResources, cbxViewSiginPost %>"
                                    TextAlign="Left" CssClass="sig" />
                                <br />
                                <asp:CheckBox ID="cbxUseSig" CssClass="sig" runat="server" Text="<%$ Resources:webResources, cbxUseSigbyDefault %>"
                                    TextAlign="Left" />
                                <br />
                                <br />
                                <asp:PlaceHolder ID="phSig" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlQuote" GroupingText="<%$ Resources:webResources, lblQuote %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:PlaceHolder ID="phQuote" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlBio" GroupingText="<%$ Resources:webResources, lblBiog %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:PlaceHolder ID="phBiog" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabHobby" runat="server" HeaderText="News/Hobbies">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlNews" GroupingText="<%$ Resources:webResources, lblLatestNews %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:PlaceHolder ID="phNews" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlHobby" GroupingText="<%$ Resources:webResources, lblHobbies %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:PlaceHolder ID="phHobby" runat="server" EnableViewState="False"></asp:PlaceHolder>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="LinksPanel" runat="server" HeaderText="<%$ Resources:webResources, lblLinks %>">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlLinks" GroupingText="<%$ Resources:webResources, lblLinks %>"
                                Style="width: 100%; height: auto; margin-bottom: 5px;">
                                <asp:Label ID="lblHomePage" CssClass="links" runat="server" Text="<%$ Resources:webResources, lblHomePage %>"
                                    EnableViewState="False"></asp:Label><asp:PlaceHolder ID="phHomePage" runat="server"
                                        EnableViewState="False"></asp:PlaceHolder>
                                <br />
                                <asp:Repeater ID="repFavLinks" runat="server" OnItemDataBound="WebLinksDataBound"
                                    EnableViewState="False">
                                    <HeaderTemplate>
                                        <asp:Label ID="lblLinkHeader" runat="server" Text="<%$ Resources:webResources, lblFavLinks %>"></asp:Label><br />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Panel ID="EditLinks" runat="server">
                                            <asp:TextBox ID="tbxName" runat="server" Text='<%# Eval("Name") %>'></asp:TextBox><asp:TextBox
                                                ID="tbxUrl" runat="server" Text='<%# Eval("Url") %>' Width="50%" />
                                        </asp:Panel>
                                        <asp:Panel ID="ViewLinks" runat="server">
                                            <asp:HyperLink ID="hypLink" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# Eval("Url") %>'></asp:HyperLink>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:LinkButton ID="btnAddLink" runat="server" OnClick="AddLinkClick" Text="<%$ Resources:webResources, btnAddLink %>" />
                                <asp:LinkButton ID="btnSaveLinks" runat="server" OnClick="SaveLinkClick" Text="<%$ Resources:webResources, btnSaveLink %>" />
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabOther" runat="server" HeaderText="Other ..">
                        <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlRoles" GroupingText="<%$ Resources:webResources, lblRoleTitle %>"
                                Style="width: 49%; float: right; height: auto; margin-bottom: 5px;">
                                <asp:Literal ID="LitRoles" runat="server" EnableViewState="False"></asp:Literal>
                            </asp:Panel>
                            <asp:Panel ID="pnlStats" runat="server" GroupingText="<%$ Resources:webResources, lblForumInfo %>"
                                Style="width: 49%; float: left; height: auto; min-height: 75px">
                                <asp:Label ID="lblUserId" runat="server" Text="<%$ Resources:webResources, lblMemberID %>"
                                    EnableViewState="False"></asp:Label><br />
                                <asp:Label ID="lblSince" runat="server" Text="<%$ Resources:webResources, lblMemberSince %>"
                                    EnableViewState="False"></asp:Label><br />
                                <asp:Label ID="lblVisit" runat="server" Text="<%$ Resources:webResources, lblLastVisit %>"
                                    EnableViewState="False"></asp:Label><br />
                                <asp:Label ID="lblPosts" runat="server" Text="<%$ Resources:webResources, lblPosts %>"
                                    EnableViewState="False"></asp:Label><br />
                            </asp:Panel>
                            <asp:Panel ID="pnlTopics" runat="server" GroupingText="<%$ Resources:webResources, lblRecentTopics %>"
                                Style="width: 100%; height: auto; float: left;">
                                <asp:GridView ID="rptRecentTopics" runat="server" OnRowDataBound="RecentTopicsDataBound"
                                    AutoGenerateColumns="False" BorderStyle="None" EmptyDataText="No recent topics"
                                    EnableViewState="False" CellPadding="2" GridLines="None" ShowHeader="False" EnableModelValidation="True">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# "~/Content/Forums/topic.aspx?TOPIC=" + Eval("Id")%>'><%# Eval("Subject") %></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        No recent Topics
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="Bookmarks" runat="server" HeaderText="BookMarks">
                        <ContentTemplate>
                            <asp:Panel ID="Panel1" runat="server" GroupingText="Bookmarks">
                                <asp:Repeater ID="repBookMarks" runat="server">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton1" SkinID="DeleteMessage" Width="16px" Height="16px"
                                            runat="server" ToolTip="Delete BookMark" /><asp:HyperLink ID="hypLink" runat="server"
                                                Text='<%# Eval("Name").ToString().ParseTags() %>' NavigateUrl='<%# Eval("Url") %>'></asp:HyperLink><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="Gallery" runat="server" HeaderText="Images">
                        <ContentTemplate>
                            <asp:CheckBox ID="cbxPublic" runat="server" Text="Public Gallery" /><br />
                            <asp:Repeater ID="dlImages" runat="server">
                                <ItemTemplate>
                                    <asp:HyperLink NavigateUrl='<%# Eval("ImagePath") %>' ToolTip='<%# Eval("Name") %>'
                                        ID="hypImg" runat="server" rel='<%# Eval("ImagePath") %>' CssClass="gallerylnk">
                                        <asp:Image ImageUrl='<%# Eval("ThumbPath") %>' ID="imgThumb" runat="server" CssClass="thumb" />
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </asp:TabPanel>
                    <asp:TabPanel ID="TabSubscriptions" runat="server" HeaderText="Subscriptions">
                        <ContentTemplate>
                            <asp:GridView ID="grdSubs" runat="server" AutoGenerateColumns="False"
                                CellPadding="2" DataKeyNames="ForumId,TopicId"
                                EnableModelValidation="True" OnRowCommand="GrdSubsRowCommand">
                                <Columns>
                                    <asp:BoundField DataField="CategoryName" HeaderText="Category" />
                                    <asp:BoundField DataField="ForumSubject" HeaderText="Forum"
                                        HtmlEncode="False" />
                                    <asp:BoundField DataField="TopicSubject" HeaderText="Topic"
                                        HtmlEncode="False" />
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False"
                                                CommandName="Delete" SkinID="DeleteMessage"
                                                Text="Delete" Height="16px" Width="16px" />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" CausesValidation="False"
                                                CommandName="DeleteAll" SkinID="DeleteMessage" ToolTip="Remove All"
                                                Text="Delete" Height="16px" Width="16px" />
                                        </HeaderTemplate>
                                        <ControlStyle CssClass="msgIcons" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:TabPanel>
                </asp:TabContainer>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:Panel ID="pnlButton" runat="server" CssClass="profileform" DefaultButton="btnUpdate">
        <fieldset style="padding-right: 6px; padding-left: 6px; padding-bottom: 6px; padding-top: 6px; border: 0px;">
            <asp:LinkButton ID="btnEdit" runat="server" Text="<%$ Resources:webResources, btnEdit %>" CausesValidation="False"
                PostBackUrl="~/Account/profile.aspx" AlternateText="<%$ Resources:webResources, btnEdit %>"
                OnClick="EditClick" />
            <asp:LinkButton ID="btnUpdate" runat="server" Text="<%$ Resources:webResources, btnUpdate %>" EnableViewState="False"
                OnClick="UpdateClick" AlternateText="<%$ Resources:webResources, btnUpdate %>" />&nbsp;
            <asp:LinkButton ID="btnCancel" runat="server" Text="<%$ Resources:webResources, btnCancel %>" CausesValidation="False"
                PostBackUrl="~/Account/profile.aspx" AlternateText="<%$ Resources:webResources, btnCancel %>"
                OnClick="CancelClick" />
        </fieldset>
    </asp:Panel>
    <asp:Panel ID="fUpload" runat="server" Style="display: none" EnableViewState="false">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
                <div id="upheader" style="width: 100%;" class="clearfix">
                    <div class="mainModalDraggablePanelDiv">
                        <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server" EnableViewState="false">
                            <span class="mainModalTitle" id="spanTitle">Avatar Upload</span>
                        </asp:Panel>
                    </div>
                    <div class="mainModalDraggablePanelCloseDiv">
                        <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false"
                            EnableViewState="false" />
                    </div>
                </div>
                <div class="mainModalContent">
                    <div id="mainModalContents">
                        <div class="demoarea">
                            <div class="demoheading">
                                Avatar Upload
                            </div>
                            Click '<i>Select File</i>' for asynchronous upload.
                            <br />
                            <br />
                            <asp:AsyncFileUpload OnClientUploadError="uploadError" OnClientUploadComplete="uploadComplete"
                                runat="server" ID="AsyncFileUpload1" Width="400px" UploaderStyle="Modern" UploadingBackColor="#CCFFFF"
                                ThrobberID="myThrobber" />
                            &nbsp;<asp:Label runat="server" ID="myThrobber" Style="display: none;"><img align="middle" alt="" src="/images/ajax-loader.gif" /></asp:Label>
                            <div>
                                <strong>Result from Server:</strong>
                            </div>
                            <asp:Label runat="server" Text="&nbsp;" ID="uploadResult" />
                            <br />
                            <asp:Label runat="server" Text="&nbsp;" ID="imageTag" />
                            <br />
                            <div id="errDiv" style="display: none;">
                                <asp:Label ID="errLabel" runat="server" Text="Label"></asp:Label>
                            </div>
                            <div>
                                <strong>File Info:</strong>
                            </div>
                            <table style="border-collapse: collapse; border-left: solid 1px #aaaaff; border-top: solid 1px #aaaaff;"
                                runat="server" cellpadding="3" id="clientSide" />
                        </div>
                        <br />
                        <asp:LinkButton ID="Button1" runat="server" Text="<%$ Resources:webResources, btnClose %>"
                            UseSubmitBehavior="False" OnClientClick="UploadDone();return false;" CausesValidation="False" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="fUpload" OkControlID="Button1"
        Drag="true" PopupDragHandleControlID="MPD" TargetControlID="btnHid" BehaviorID="mpUpload"
        BackgroundCssClass="modalBackground" CancelControlID="clB" OnCancelScript="$find('mpUpload').hide();"
        DropShadow="true" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="CPF1" runat="server">
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="CPF2" runat="server">
</asp:Content>
