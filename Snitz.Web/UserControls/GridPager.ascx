<%@ Control Language="C#" AutoEventWireup="true" Inherits="GridPager" CodeBehind="GridPager.ascx.cs" %>
<div id="PagerPanel">
    <div id="buttonPager" runat="server" visible="false">
        <asp:Button ID="cmdPrev" runat="server" CausesValidation="False" CommandArgument="prev"
            EnableViewState="False" OnClick="cmdPrev_Click" Text="<%$ Resources:webResources, btnPrev %>" />
        <asp:Label ID="lblCurrentPage" runat="server"></asp:Label>
        <asp:Button ID="cmdNext" runat="server" CausesValidation="False" CommandArgument="next"
            EnableViewState="False" OnClick="cmdNext_Click" Text="<%$ Resources:webResources, btnNext %>" />
    </div>
    <div id="dropdownPager" runat="server" visible="false">
        &nbsp;&nbsp;&nbsp;<asp:Label ID="pagingLabel1" runat="server" EnableViewState="False"
            Text='<%$ Resources:webResources, lblPage %>'></asp:Label>&nbsp;&nbsp;
        <asp:DropDownList runat="server" ID="ddlPageSelector" AutoPostBack="True" OnSelectedIndexChanged="ddlPageSelector_SelectedIndexChanged"
            Height="20px">
        </asp:DropDownList>
        &nbsp;<asp:Label ID="pagingLabel2" runat="server" EnableViewState="False"></asp:Label>
    </div>
    <div id="textPager" runat="server" visible="false" class="snitzpager">
        &nbsp;&nbsp;&nbsp;<asp:Literal ID="litPager" runat="server"></asp:Literal>
    </div>
    <div id="linkPager" runat="server" visible="false" class="snitzpager">
        <asp:PlaceHolder ID="plcPaging" runat="server"></asp:PlaceHolder>
    </div>
</div>
