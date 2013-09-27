<%@ Control Language="C#" AutoEventWireup="True"  CodeBehind="ResXEditor.ascx.cs" Inherits="ResxWebEditor.Editor.ResXEditor" %>
<%@ Register TagPrefix="resx" Namespace="Snitz.ThirdParty" Assembly="Snitz.Controls" %>


<div style="margin: auto;">
    <div class="EditorHeader" style="width: auto;">
        <asp:Literal ID="Literal2" runat="server" EnableViewState="False">Resource Translation</asp:Literal>
    </div>
    <div id="EditorPanel" class=" forumtable">
        <asp:Panel ID="Panel1" runat="server" CssClass="forumtable clearfix">
            <h2 style="width: 100%; padding-right: 0px; padding-left: 0px;">
                <asp:Label ID="lblFileName" runat="server" meta:resourcekey="lblFileNameResource1" />
            </h2>
            <asp:Panel ID="Panel2" runat="server" GroupingText="Resource files" Width="220px" Style="float: left; padding: 4px;" EnableViewState="True">
                <p>
                    <asp:Label ID="Label1" runat="server" Text="Select language" EnableViewState="False"></asp:Label>
                    <asp:DropDownList ID="ddlEditLang" runat="server"
                        AutoPostBack="True" OnSelectedIndexChanged="ddlEditLang_SelectedIndexChanged" EnableViewState="False">
                        <asp:ListItem Selected="True">[Default]</asp:ListItem>
                        <asp:ListItem Value="fr">French</asp:ListItem>
                        <asp:ListItem Value="de">German</asp:ListItem>
                        <asp:ListItem Value="en-ie">Irish</asp:ListItem>
                        <asp:ListItem Value="it">Italian</asp:ListItem>
                        <asp:ListItem Value="ja">Japanese</asp:ListItem>
                        <asp:ListItem Value="fa">Persian</asp:ListItem>
                    </asp:DropDownList>
                </p>
                <asp:ListBox ID="lstResX" runat="server" AutoPostBack="True" Height="150px" CssClass="resXlist"
                    OnSelectedIndexChanged="lstResX_SelectedIndexChanged" Width="200px"
                    />
                <div class="clearfix">
                    <p>
                        <asp:LinkButton ID="btSave" runat="server" Text="<%$ Resources:webResources,btnSave %>" Visible="False" EnableViewState="False" />
                    </p>
                    <asp:Panel ID="pnlMsg" runat="server" EnableViewState="false" Visible="false" meta:resourcekey="pnlMsgResource1">
                        <asp:MultiView ID="MultiViewMsg" runat="server">
                            <asp:View ID="View1" runat="server">
                                <asp:Image ID="imgResult" runat="server" ImageUrl="~/ResourceEditor/images/tick.png" meta:resourcekey="imgResultResource1" EnableViewState="False" />
                            </asp:View>
                            <asp:View ID="View2" runat="server">
                                <asp:Image ID="Image1" runat="server" ImageUrl="~/ResourceEditor/images/exclamation.png" meta:resourcekey="Image1Resource1" EnableViewState="False" />
                            </asp:View>
                        </asp:MultiView>
                        <asp:Label ID="lblMsg" runat="server" Style="padding-left: 10px; position: relative; top: -5px;" meta:resourcekey="lblMsgResource1" EnableViewState="False" />
                    </asp:Panel>
                </div>
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" Style="margin-left: 225px; padding-top: 4px;">
                <asp:Panel ID="pnlAddLang" runat="server" Visible="False" GroupingText="Add Language File"
                    Style="margin: auto; width: 99%;" EnableViewState="True">
                    <asp:Literal ID="Literal1" runat="server" Text="Language:" meta:resourcekey="Literal1Resource1" EnableViewState="False" /><br />
                    <asp:DropDownList ID="ddLanguage" runat="server" meta:resourcekey="ddLanguageResource1" />
                    <asp:Button ID="btAddLang" runat="server" OnClick="btAddLang_Click" OnClientClick="this.disable=true;" Enabled="true" Text="Add" meta:resourcekey="btAddLangResource1" EnableViewState="False" />
                    <br />
                    <asp:CheckBox Visible="false" ID="chShowEmpty" runat="server" Text="Show empty strings" Checked="false" AutoPostBack="true" OnCheckedChanged="OnShowEmpty" EnableViewState="False" />
                </asp:Panel>
                <resx:BulkEditGridViewEx ID="gridView" runat="server" GridLines="Both"
                    Width="99%" AutoGenerateColumns="False"
                    EnableInsert="False" OnRowDataBound="GridView_RowDataBound"
                    SaveButtonID="btSave" DataKeyNames="Key" OnRowUpdating="gridView_RowUpdating"
                    OnSaved="gridView_Saved" InsertRowCount="1"
                    meta:resourcekey="gridViewResource1" CellPadding="1" EnableViewState="False">
                    <PagerSettings Position="TopAndBottom" />
                </resx:BulkEditGridViewEx>
            </asp:Panel>

        </asp:Panel>

    </div>
</div>
