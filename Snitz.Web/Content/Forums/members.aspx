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
<%@ Page Language="C#" MasterPageFile="~/MasterTemplates/MainMaster.Master" AutoEventWireup="True" Inherits="MembersPage" Title="<%$ Resources:webResources, ttlMembersPage %>" Culture="auto" UICulture="auto" Codebehind="members.aspx.cs" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="SnitzCommon" %>
<%@ Import Namespace="SnitzConfig" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>
<%@ Reference Control="~/UserControls/GridPager.ascx"  %>
<%@ Register Src="~/UserControls/MemberSearch.ascx" TagName="MemberSearch" TagPrefix="uc1" %>
<%@ MasterType TypeName="BaseMasterPage" %>
<asp:Content ID="head" runat="server" ContentPlaceHolderID="CPhead">
<style type="text/css">
td a.Snitzbutton, td a.Snitzbutton span{background:none;height: auto;padding-right: 1px;padding-top: 0px;
                                            text-decoration: none;
    white-space:nowrap;
    font-size:small;
    font-weight:bold;}

    </style>
</asp:Content>
<asp:Content id="cpm" runat="server" contentplaceholderid="CPM">  

    <uc1:MemberSearch ID="ucSearch" runat="server" EnableViewState="false" />
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="upd" >
        <ProgressTemplate>
           <div style="width:800px;left:0px;top:0px;margin:auto; text-align: center;"  >
            <asp:Image ID="Image1" runat="server" ImageUrl="/images/ajax-loader.gif"/>
            </div>
        </ProgressTemplate>
        </asp:UpdateProgress>
    <ajaxtoolkit:AlwaysVisibleControlExtender ID="UpdateProgress1_AlwaysVisibleControlExtender" 
        runat="server" Enabled="True" HorizontalSide="Center" 
        TargetControlID="UpdateProgress1" VerticalSide="Middle" EnableViewState="False">
    </ajaxtoolkit:AlwaysVisibleControlExtender>
    <asp:UpdatePanel ID="upd" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <script type="text/javascript">
                    var prm = Sys.WebForms.PageRequestManager.getInstance();
                    prm.add_endRequest(function () {
                        jQuery("abbr.timeago").timeago();
                    }); 

                </script>
                <asp:GridView ID="MGV" 
                  AutoGenerateColumns="False"
                  PageSize="<%# SnitzConfig.Config.MemberPageSize %>"
                  AllowPaging="True" AllowSorting="true"
                  DataKeyNames="Id"
                  CssClass="forumtable border" 
                  runat="server" CellPadding="3" EnableViewState="False" 
                  GridLines="None" OnRowCreated="MemberGridViewRowCreated" 
                  OnRowDataBound="MgvRowDataBound" DataSourceID="MemberODS" 
                  UseAccessibleHeader="False">
                
              <Columns> 
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server" ToolTip='<%# String.Format(webResources.lblViewProfile, Eval("Name")) %>' NavigateUrl='<%# Eval("ProfileLink") %>'>
                        <asp:Image runat="server" Visible='<%# ((bool)Eval("Status")) %>' ID="ImgProfile" SkinID="Profile" AlternateText='<%$ Resources:webResources, lblProfile %>' EnableViewState="False" />
                        <asp:Image runat="server" Visible='<%# (!(bool)Eval("Status")) %>' ID="ImgProfileLocked" SkinID="ProfileLocked" AlternateText='<%$ Resources:webResources, lblProfile %>' EnableViewState="False"/></asp:HyperLink>

                    </ItemTemplate>
                    <HeaderStyle Width="20px" CssClass="sortLnk" />
                    <ItemStyle CssClass="membericon" />
                </asp:TemplateField>               
                <asp:BoundField DataField="Name" HeaderText="<%$ Resources:webResources, lblUsername %>" SortExpression="Name" HeaderStyle-CssClass="sortLnk"/>
                <asp:TemplateField HeaderText="<%$ Resources:webResources, lblTitle %>">
            		<ItemStyle ></ItemStyle>
                    <HeaderStyle CssClass="sortLnk" />
            		<ItemTemplate >
                        <%# Eval("Rank.Title") %>
        		    </ItemTemplate>
        		</asp:TemplateField>
                
                <asp:TemplateField HeaderText="<%$ Resources:webResources, lblPosts %>" sortexpression="PostCount" HeaderStyle-CssClass="sortLnk">
                	<ItemStyle HorizontalAlign="Center" Wrap="false"></ItemStyle>
            		<ItemTemplate>
                        <%# Common.TranslateNumerals(Eval("PostCount"))%>
                		<br />
                        <%# Eval("Rank.Stars") %>
                	</ItemTemplate>
        		</asp:TemplateField>
                
                <asp:TemplateField HeaderText="<%$ Resources:webResources, lblLastPost %>" sortexpression="LastPostDate">
                	<ItemStyle HorizontalAlign="Center"></ItemStyle>
            		<ItemTemplate>
                		<%# Eval("LastPostTimeAgo") %></ItemTemplate>
        		</asp:TemplateField>
                <asp:TemplateField HeaderText="<%$ Resources:webResources, lblMemberSince %>" sortexpression="Date">
                	<ItemStyle HorizontalAlign="Center"></ItemStyle>
            		<ItemTemplate>
                		<%# Eval("MemberSinceTimeAgo") %></ItemTemplate>
        		</asp:TemplateField>
                <asp:BoundField DataField="Country"   HeaderText="<%$ Resources:webResources, lblCountry %>" SortExpression="Country">
                	<ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:BoundField>
                <asp:TemplateField HeaderText="<%$ Resources:webResources, lblLastVisit %>" SortExpression="LastVisitDate">
                <ItemStyle HorizontalAlign="Center" Wrap="false"></ItemStyle>
                <ItemTemplate>
                    <%# Eval("LastVisitTimeAgo") %>
                </ItemTemplate>  
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False" >
                    <ItemTemplate>
                        <asp:HyperLink ID="hypUserLock" SkinID="UserLock" Text="<%$ Resources:webResources, lblLockUser %>" runat="server" EnableViewState="False" Visible='<%# ((Eval("Status").ToString() == "1") && IsAdministrator) %>' NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"pop_lock.aspx?lock=1&mode=M&ID={0}\")",Eval("Name")) %>' rel="NoFollow"></asp:HyperLink>
                        <asp:HyperLink ID="hypUserUnlock" SkinID="UserUnlock" Text="<%$ Resources:webResources, lblUnLockUser %>" runat="server" EnableViewState="False" Visible='<%# ((Eval("Status").ToString() != "1") && IsAdministrator) %>' NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"pop_lock.aspx?lock=0&mode=M&ID={0}\")",Eval("Name")) %>' rel="NoFollow"></asp:HyperLink>
                        <asp:HyperLink ID="hypUserEdit" SkinID="UserEdit" Text="<%$ Resources:webResources, lblEditUser %>" runat="server" EnableViewState="False" NavigateUrl='<%# "~/Account/profile.aspx?edit=Y&user=" + Eval("Name") %>' Visible='<%# IsAdministrator %>' rel="NoFollow"></asp:HyperLink>
                        <asp:HyperLink ID="hypUserDelete" SkinID="UserDelete" Text="<%$ Resources:webResources, lblDeleteUser %>" runat="server" EnableViewState="False" Visible='<%# IsAdministrator %>' NavigateUrl='<%# String.Format("javascript:openConfirmDialog(\"pop_delete.aspx?lock=1&mode=M&ID={0}\")",Eval("Name")) %>' rel="NoFollow"></asp:HyperLink>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" />
                    <ItemStyle CssClass="memberLC" />
                </asp:TemplateField>
              </Columns> 
        	<RowStyle CssClass="row" />
        	<AlternatingRowStyle CssClass="altrow" />     
            	<PagerStyle CssClass="hidden" />
                <PagerTemplate></PagerTemplate>
				<HeaderStyle CssClass="category cattitle" />                
            </asp:GridView>
            <asp:PlaceHolder ID="pager" runat="server"></asp:PlaceHolder>    
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ucSearch$btnSearch" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ucSearch" EventName="InitialLinkClick"/>
            </Triggers>
     </asp:UpdatePanel>
<br />
      <asp:ObjectDataSource 
        ID="MemberODS" 
        runat="server" 
        TypeName="SnitzData.PagedObjects" 
        EnablePaging="True" 
        SortParameterName="SortExpression"
        SelectCountMethod="SelectMemberCount"
        StartRowIndexParameterName="StartRecord"
        MaximumRowsParameterName="MaxRecords"
        SelectMethod="GetAllMembersPaged" 
        EnableViewState="False" OnSelected="MemberObjectDataSourceSelected" 
        OnSelecting="MemberObjectDataSourceSelecting">
      </asp:ObjectDataSource>
	        <!-- Profile popup -->
    <asp:Panel ID="MPanel" runat="server" Style="display: none">
        <div class="mainModalPopup mainModalBorder">
            <div class="mainModalInnerDiv mainModalInnerBorder">
            <div id="header" style="width:100%;" class="clearfix" >
                <div class="mainModalDraggablePanelDiv">
                    <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD" runat="server">
                        <span class="mainModalTitle" id="spanTitle"></span>
                    </asp:Panel>
                </div>
                <div class="mainModalDraggablePanelCloseDiv">
                    <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB" CausesValidation="false"/>
                </div>
            </div>
            <div class="mainModalContent">
                <div id="mainModalContents">
                </div>
            </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Button runat="server" ID="btnHid" Style="display: none;" />
    <ajaxtoolkit:ModalPopupExtender ID="mpeModal" runat="server" PopupControlID="MPanel"
        TargetControlID="btnHid" BehaviorID="mbMain" BackgroundCssClass="modalBackground"
        CancelControlID="clB" OnCancelScript="mainScreen.CancelModal();" DropShadow="true" />
    
</asp:Content>
<asp:Content ID="rightcol" ContentPlaceHolderID="RightCol" runat="server" >
    <snitz:SideBar runat="server" ID="sidebar" Show="Ads,Stats,Events,Active"/>

</asp:Content>
<asp:Content ID="cpf1" runat="server" ContentPlaceHolderID="CPF1">
</asp:Content>


<asp:Content ID="adOverride" runat="server" contentplaceholderid="CPAd">
</asp:Content>



