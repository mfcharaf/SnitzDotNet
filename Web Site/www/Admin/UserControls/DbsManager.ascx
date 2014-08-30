<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DbsManager.ascx.cs" Inherits="SnitzUI.Admin.UserControls.Admin_DbsManager"  %>
<style>
    #DbsFiles{float:left;width:20%;height:auto;margin-left: 5px;}
    #DbsContent{float:left;width:77%;margin-left: 5px;margin-right: 10px;height:auto;max-height: 310px;}
</style>
<asp:Panel ID="DbsMain" runat="server" CssClass="clearfix adminform" DefaultButton="btnSubmit">
    <h2 class="category">DBS Manager</h2>
    <div class="clearfix" style="width: 99%;">
        <asp:UpdatePanel runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <script type="text/javascript">
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(function () {
                        SyntaxHighlighter.autoloader(
                            'js jscript javascript  /Scripts/syntax/shBrushJScript.min.js',
                            'c# c-sharp csharp      /Scripts/syntax/shBrushCSharp.min.js',
                            'xml xhtml xslt html    /Scripts/syntax/shBrushXml.min.js'
                        );

                        SyntaxHighlighter.highlight();

                    });

                </script>
                <div id="DbsFiles" >
                    <asp:ListBox ID="lbFiles" runat="server" OnSelectedIndexChanged="SelectFile" AutoPostBack="True" Width="99%" Rows="16"></asp:ListBox>
                </div>
                <div id="DbsContent">
                    <asp:Literal ID="txtDbsFile" runat="server" ></asp:Literal>
                </div>                
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>    
    <asp:Panel ID="Panel5" runat="server" style="width:99%;">
        <asp:Label ID="lblResult" runat="server" Text=""></asp:Label><br/>
        <asp:LinkButton ID="btnSubmit" runat="server" Text="Run Script" OnClick="BtnSubmitClick" />&nbsp;
        <asp:LinkButton ID="btnReset" runat="server" Text="Reset" />
    </asp:Panel>
</asp:Panel>

