<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_UpdateCounts" Codebehind="UpdateCounts.ascx.cs" %>
<asp:Panel ID="Panel1" runat="server" CssClass="forumtable clearfix">
        <h2 class="category">Forum Counts</h2>
                <p align="center">
                    <asp:UpdatePanel ID="UpdateCounts" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
                        <ContentTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" Text="Update Counts" 
                                onclick="LinkButton1_Click"></asp:LinkButton>
                <br />
                <br />
                <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                <br />
                <br />

                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdateCounts">
                        <ProgressTemplate>
                            <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;"  >
                                <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
               
                </p>
</asp:Panel> 