<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="true" CodeBehind="faq.aspx.cs" Inherits="SnitzUI.Content.FAQ.Faq" %>
<%@ Reference Control = "~/UserControls/SideColumn.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="CPhead" runat="server">
    <link rel="stylesheet" type="text/css" runat="server" id="editorCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="faqCSS"/>
    <link rel="stylesheet" type="text/css" runat="server" id="shTheme"/>
    <script src="/scripts/help.js" type="text/javascript"></script>

    <link href="/css/shCore.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var urltarget = '<%# Profile.LinkTarget %>';

        function CategoryChange() {
            __doPostBack("<%= btn.ClientID %>", "");
        }

        $(document).ready(function () {
            var prm = Sys.WebForms.PageRequestManager.getInstance();

            prm.add_endRequest(function () {
                $(".QRMsgArea").markItUp(miniSettings);
            });

            $(".bbcode").each(function () {
                $(this).html(parseBBCode(parseEmoticon($(this).text(), '<%= Page.Theme %>')));
            });
            SyntaxHighlighter.autoloader(
                'js jscript javascript  /Scripts/syntax/shBrushJScript.min.js',
                'c# c-sharp csharp      /Scripts/syntax/shBrushCSharp.min.js',
                'css                    /Scripts/syntax/shBrushCss.min.js',
                'text plain             /Scripts/syntax/shBrushPlain.min.js',
                'sql                    /Scripts/syntax/shBrushSql.min.js',
                'vb vbnet               /Scripts/syntax/shBrushVb.min.js',
                'xml xhtml xslt html    /Scripts/syntax/shBrushXml.min.js'
            );
            SyntaxHighlighter.all();
            $(".QRMsgArea").markItUp(miniSettings);
        });


    </script>
</asp:Content>
<asp:Content ID="topleft" ContentPlaceHolderID="CPHL" runat="server">

</asp:Content>
<asp:Content ID="cpMain" ContentPlaceHolderID="CPM" runat="server">
    <asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>   
            <div id="body">
                <div id="sidebar">
                    <asp:Repeater ID="FaqNav" runat="server" OnItemDataBound="BindQuestions">
                        <HeaderTemplate>
                            <div class="clearfix">
                                <asp:ImageButton ID="manageCats" runat="server" SkinID="Folder" OnClick="ManageCategories" ToolTip="Manage Categories" EnableViewState="False" />
                                <asp:ImageButton ID="addTopic" runat="server" SkinID="Document" OnClick="NewTopic" ToolTip="Add new help Topic" EnableViewState="False" />
                        
                                <asp:TextBox ID="searchFor" runat="server" Width="175px" CssClass="floatright" ></asp:TextBox>
                                <asp:ImageButton ID="btnSearch" runat="server" SkinID="Search" OnClick="SearchFaq" style="float:right"  EnableViewState="False" />              
                            </div>                                
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="tableheader">
                                <asp:Label ID="lnkCat" runat="server" Text='<%# Eval("Description") %>' EnableViewState="False" />
                                <asp:HiddenField ID="hdnCatId" runat="server" Value='<%# Eval("Id") %>' /><br />
                            </div>
                            <div class="faqQuestion">
                                <asp:Repeater ID="FaqQuestions" runat="server">
                                    <ItemTemplate>
                                        <asp:HyperLink runat="server" ID="qLink" NavigateUrl='<%# String.Format("/Faq{0}", Eval("Link").ToString().Replace(" ","_")) %>'><%# Eval("LinkTitle") %></asp:HyperLink><br />
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <div id="main">
                    <asp:HiddenField ID="hdnFaqId" runat="server" />
                    <asp:Button ID="btn" runat="server" OnClick="btn_Click" style="display:none;" />
                    <asp:UpdatePanel runat="server" ID="cPanel" ChildrenAsTriggers="True">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btn" EventName="Click" />
                        </Triggers>
                        <ContentTemplate>      
                            <asp:MultiView ID="faqView" runat="server">
                                <asp:View runat="server" ID="vDefault">
                                    <asp:Literal ID="faqMsg" runat="server" Text="Here you can find answers to questions about how the forum works. Use the links on the left or the search box to find what you are looking for. " EnableViewState="False" Mode="Encode"></asp:Literal>                             
                                </asp:View>
                                <asp:View runat="server" ID="vQuestion">
                                    <div class="categorybuttons">
                                        <asp:ImageButton ID="btnDeleteFaq" SkinID="DeleteMessage" runat="server" OnClick="Delete" Visible="false" EnableViewState="False" />
                                        <asp:ImageButton ID="btnEdit" SkinID="Edit" runat="server" OnClick="ToggleEdit" Visible="false" EnableViewState="False" />
                                    </div>
                                    <div class="faqtable bbcode" style="padding:4px;">
                                        <asp:Literal runat="server" ID="fvQuestion"  Mode="Encode"></asp:Literal>
                                        <asp:Literal runat="server" ID="fvAnswer"  Mode="Encode"></asp:Literal>
                                    </div>
                                </asp:View>
                                <asp:View runat="server" ID="vEdit">
                                    <div class="categorybuttons">
                                        <asp:ImageButton ID="veDelete" SkinID="DeleteMessage" runat="server" OnClick="Delete" EnableViewState="False" />
                                        <asp:ImageButton ID="veSave" SkinID="Save" runat="server" OnClick="Save" EnableViewState="False" />
                                    </div>
                                    <div class="faqtable" style="padding:4px;">
                                        <asp:Label ID="Label4" runat="server" Text="Category" AssociatedControlID="fnCategory"></asp:Label><asp:DropDownList ID="feCategory" runat="server" DataValueField="Id" DataTextField="Description"></asp:DropDownList><br/>
                                        <asp:Label ID="Label2" runat="server" Text="Question" AssociatedControlID="feTitle"></asp:Label><asp:TextBox ID="feTitle" runat="server" Width="350"></asp:TextBox><br/>
                                        <asp:Label ID="Label13" runat="server" Text="Order" AssociatedControlID="feOrder" ></asp:Label><asp:TextBox ID="feOrder" runat="server" Width="20px"></asp:TextBox><br />
                                        <asp:Label ID="Label3" runat="server" Text="Answer" AssociatedControlID="feBody"></asp:Label><asp:TextBox CssClass="QRMsgArea" ID="feBody" runat="server" TextMode="MultiLine" Rows="4" Width="90%"></asp:TextBox><br/>
                                    </div>
                                </asp:View>
                                <asp:View runat="server" ID="vCategory">
                                    <div class="categorybuttons">
                                        <asp:ImageButton ID="fcDelete" SkinID="DeleteMessage" runat="server" OnClick="DeleteCat" EnableViewState="False" />
                                        <asp:ImageButton ID="fcSave" SkinID="Save" runat="server" OnClick="SaveCat" EnableViewState="False" />
                                    </div>                                 
                                    <div class="faqtable" style="padding:4px;">
                                        <h1>Manage Categories</h1>
                                        <asp:Label ID="Label1" runat="server" Text="Category" AssociatedControlID="fcCategory"></asp:Label><asp:DropDownList onchange="CategoryChange();"  ID="fcCategory" runat="server" DataValueField="Id" DataTextField="Description" OnSelectedIndexChanged="SelectCategory" ></asp:DropDownList><br/>
                                        <asp:Label ID="Label8" runat="server" Text="Title" AssociatedControlID="fcTitle"></asp:Label><asp:TextBox ID="fcTitle" runat="server" Width="300"></asp:TextBox><br/>
                                        <asp:Label ID="Label10" runat="server" Text="Language code" AssociatedControlID="fcLang" ></asp:Label><asp:TextBox ID="fcLang" runat="server" Width="30px" ReadOnly="True"></asp:TextBox><br />
                                        <asp:Label ID="Label11" runat="server" Text="Order" AssociatedControlID="fcOrder" ></asp:Label><asp:TextBox ID="fcOrder" runat="server" Width="20px"></asp:TextBox><br />
                                        <asp:Label ID="Label9" runat="server" Text="Roles" AssociatedControlID="fcRoles"></asp:Label><asp:TextBox ID="fcRoles" runat="server" Width="300"></asp:TextBox><br/>
                                    </div>                                
                                </asp:View>
                                <asp:View runat="server" ID="vNew">
                                    <div class="categorybuttons">
                                        <asp:ImageButton ID="fnSave" SkinID="Save" runat="server" OnClick="Add" EnableViewState="False" />
                                    </div>                                 
                                    <div class="faqtable" style="padding:4px;">
                                        <h1>New Help Topic</h1>
                                        <asp:Label ID="Label7" runat="server" Text="Category" AssociatedControlID="fnCategory"></asp:Label><asp:DropDownList ID="fnCategory" runat="server" DataValueField="Id" DataTextField="Description"></asp:DropDownList><br/>
                                        <asp:Label ID="Label5" runat="server" Text="Question" AssociatedControlID="fnTitle"></asp:Label><asp:TextBox ID="fnTitle" runat="server" Width="350"></asp:TextBox><br/>
                                        <asp:Label ID="Label12" runat="server" Text="Order" AssociatedControlID="fnOrder" ></asp:Label><asp:TextBox ID="fnOrder" runat="server" Width="20px"></asp:TextBox><br />
                                        <asp:Label ID="Label6" runat="server" Text="Answer" AssociatedControlID="fnBody"></asp:Label><asp:TextBox CssClass="QRMsgArea" ID="fnBody" runat="server" TextMode="MultiLine" Rows="4" Width="90%"></asp:TextBox><br/>
                                    </div>                                
                                </asp:View>
                            </asp:MultiView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>    
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:sidebar runat="server" ID="sidebar1" Show="Poll,Events,Ads,Active,Rss"/>
</asp:Content>