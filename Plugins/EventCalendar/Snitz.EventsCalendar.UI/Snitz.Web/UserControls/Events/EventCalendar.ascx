<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="EventCalendar.ascx.cs" Inherits="EventsCalendar.UserControls.EventCalendar" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Register Src="~/UserControls/Events/ExtendedCalendar.ascx" TagName="ExtendedCalendar" TagPrefix="uc1" %>

<asp:Panel ID="pnlButtons" runat="server" EnableViewState="false" Visible="false" style="padding:3px;">
<asp:Button ID="btnChangeView" runat="server" Text="Show Full Year" 
    OnClick="ChangeView" EnableViewState="False" />
</asp:Panel>
<asp:Panel ID="pnlYearPick" runat="server" EnableViewState="false" Visible="true" style="padding:3px;">
    <asp:Label ID="lblYearPick" runat="server" Text="Choose a year to display" AssociatedControlID="ddlYear"></asp:Label>
<asp:DropDownList ID="ddlYear" runat="server" 
    AutoPostBack="True" EnableViewState="true" 
    onselectedindexchanged="ddlYear_SelectedIndexChanged">
</asp:DropDownList>
</asp:Panel>

<asp:Panel ID="calMonth" runat="server" Visible="false">
<asp:Calendar OnDayRender="Calendar1_DayRender" 
    OnSelectionChanged="Calendar1_SelectionChanged"
    OnVisibleMonthChanged="MonthChanged"
    Runat="server" id="Calendar1"  CssClass="largeCal"
    SelectionMode="Day" SkinID="lgeCal" EnableViewState="False" >
    <TitleStyle CssClass="calYearLabel" />
</asp:Calendar>
</asp:Panel>
            <asp:UpdatePanel ID="udpOutterUpdatePanel" runat="server"> 
             <ContentTemplate>
<asp:Panel ID="calYear" runat="server" Visible="true">
    <asp:Repeater ID="rptMonths" runat="server" OnItemDataBound="rptMonths_ItemDataBound" >
        <HeaderTemplate> 
        <div style="width:100%;"> 
    </HeaderTemplate> 
    <SeparatorTemplate> 
        
        </div><div style="width:100%;"> 
    </SeparatorTemplate> 

    <ItemTemplate>
    <div style="float:left;width:auto;padding:2px;">
    <uc1:ExtendedCalendar ID="ExtendedCalendar1" runat="server" TodaysDate='<%# Eval("Date") %>' />
    </div>
    </ItemTemplate>
    <FooterTemplate> 
        <div style="clear:both;"></div></div></FooterTemplate>
    </asp:Repeater>
<br />
</asp:Panel>
    <!-- popup -->

    <asp:Panel ID="MPanel" runat="server" Style="display: none" EnableViewState="False">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
            <div id="header" style="width:100%;" class="clearfix" >
                <div class="mainModalDraggablePanelDiv">
                    <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server" EnableViewState="False">
                        <span class="mainModalTitle" id="spanTitle">Details for <span id="selectedday" runat="server"></span></span>
                    </asp:Panel>
                </div>
                <div class="mainModalDraggablePanelCloseDiv">
                    <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false" EnableViewState="False" />
                </div>
            </div>
            <div class="mainModalContent">
                <div id="mainModalContents">
                <span id="daydetail_render" runat="server" ></span>
                </div>
            </div>
            </div>
        </div>
    </asp:Panel>
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="MPanel"
     Drag="true" PopupDragHandleControlID="MPD"
        TargetControlID="btnHid" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="mainScreen.CancelModal();" DropShadow="true" EnableViewState="False" />
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
                         </ContentTemplate>
             </asp:UpdatePanel>