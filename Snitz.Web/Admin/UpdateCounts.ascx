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
                            <asp:Label ID="lblProgress" runat="server" Text="Updating Forum Counts..." Width="200px" BackColor="#FFFF80"
                                ForeColor="Maroon" Font-Bold="True" Style="padding: 5px"></asp:Label>
                        </ProgressTemplate>
                    </asp:UpdateProgress>
               
                </p>
</asp:Panel> 