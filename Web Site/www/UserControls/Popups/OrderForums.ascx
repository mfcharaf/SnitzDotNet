<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderForums.ascx.cs" Inherits="SnitzUI.UserControls.Popups.OrderForums" %>
<style>
    #divSetOrder span{ width: 300px;display: inline-block;margin: auto;}
</style>
<div id="divSetOrder" style="width: 99%;height: 500px;">
        <div style="max-height: 95%;overflow: auto;width:98%;">
            <asp:Repeater ID="rptCatOrder" runat="server" OnItemDataBound="BindCategories">
                <ItemTemplate>
                    <div class="category cattitle clearfix" style="width: 95%;vertical-align: middle;padding:2px;">
                        <asp:HiddenField runat="server" ID="hdnCatOrderId"/>
                        <asp:Label runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                        <asp:DropDownList ID="cOrder" runat="server" style="float:right;"></asp:DropDownList> 
                    </div>
                    <div style="width:95%">
                        <asp:Repeater ID="rptForumOrder" runat="server" OnItemDataBound="BindForums">
                            <ItemTemplate>
                                <div style="padding: 2px;" class="clearfix">
                                    <asp:HiddenField runat="server" ID="hdnForumOrderId"/>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                                    <asp:DropDownList ID="fOrder" runat="server" style="float:right;"></asp:DropDownList><br/> 
                                </div>               
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <asp:Panel ID="Panel5" runat="server" CssClass="popup-button-panel">
        <button onclick="SaveForumOrder();return false;" type="button">
            <%= Resources.webResources.btnSubmit %></button>&nbsp;
        <button onclick="mainScreen.CancelModal();return false;" type="button">
                <%= Resources.webResources.btnCancel %></button>
    </asp:Panel>
</div>