<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PmViewAlt.ascx.cs" Inherits="SnitzUI.UserControls.PrivateMessages.PmViewAlt" %>
<%@ Register TagPrefix="ajax" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit, Version=3.5.7.1213, Culture=neutral, PublicKeyToken=28f01b0e84b6d53e" %>

<script type="text/javascript">

    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_pageLoaded(pageLoaded);

    function pageLoaded(sender, args) {
        var panels = args.get_panelsUpdated();

        if (panels.length > 0) {
            for (i = 0; i < panels.length; i++) {
                var panelID = panels[i].id;
                
                if (panelID == '<%= upd.ClientID %>') {
                    $('.pmMsgArea').markItUp(miniSettings);
                    $("#<%= PMTreeView.ClientID %> input[type=checkbox]").click(function () {
                        updatePanel();
                    });
                }
            }

        }
    }


    function updatePanel() {
        __doPostBack('<%= upd.ClientID %>');
	    return false;
	}
    function ShowMemberList() {
        $('.popub-bkg').show();
    }
</script>
<div style="position: relative;">
    <asp:UpdateProgress ID="UpdatePanelProgressExtender2" runat="server" AssociatedUpdatePanelID="upd">
        <ProgressTemplate>
            <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:transparent;"  >
                <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="upd" ChildrenAsTriggers="True" UpdateMode="Conditional" >
        <Triggers>
          <asp:AsyncPostBackTrigger ControlID="buttonCheck" EventName="click" />
        </Triggers>    
        <ContentTemplate><asp:button ID="buttonCheck" runat="server" CausesValidation="false" CssClass="hidden" />
            <div class="pm-inner clearfix" >
                <div class="pmbutton clearfix">
                    <div style="width:40%;float:left;">
                    <asp:ImageButton ID="ButtonNew" runat="server" SkinID="PMNewAlt"
                        OnClick="btnNew_Click" ToolTip="<%$ Resources:PrivateMessage, PmNew %>" AlternateText="New PM" BorderWidth="1" />
                    <asp:ImageButton ID="ButtonReceive" runat="server" SkinID="PMReceiveAlt"
                        OnClick="btnReceive_Click" ToolTip="<%$ Resources:PrivateMessage, PmSendReceive %>" AlternateText="Send/Receive" BorderWidth="1"/>
                    <asp:ImageButton ID="ButtonMembers" runat="server" SkinID="PMMembersAlt"
                        AlternateText="Member list" ToolTip="<%$ Resources:PrivateMessage, PmMembers %>" BorderWidth="1" OnClick="ButtonMembersClick" />
                    <asp:ImageButton ID="ButtonOptions" runat="server" SkinID="PMOPtionsAlt"
                        OnClick="btnOptions_Click" ToolTip="<%$ Resources:PrivateMessage, PmSettings %>" AlternateText="Options" BorderWidth="1"/>
                    <asp:ImageButton ID="ButtonDelete" runat="server" SkinID="PMDeleteAlt"
                        AlternateText="Delete Checked" ToolTip="<%$ Resources:PrivateMessage, PmDelete %>" OnClick="ButtonDelete_Click" BorderWidth="1"/>                
                    
                    </div>
                    <div style="width:40%;float:right;text-align:right;">
                    <asp:ImageButton ID="ButtonReply" runat="server" SkinID="PMReplyAlt"
                        AlternateText="Reply" ToolTip="<%$ Resources:PrivateMessage, PmReply %>" OnClick="ButtonReply_Click" BorderWidth="1"/>
                    <asp:ImageButton ID="ButtonReplyQuote" runat="server" SkinID="PMReplyQuoteAlt"
                        AlternateText="Reply with Quote" ToolTip="<%$ Resources:PrivateMessage, PmQuote %>" OnClick="ButtonReplyQuote_Click" BorderWidth="1"/>
                    <asp:ImageButton ID="ButtonForward" runat="server" SkinID="PMForwardAlt"
                        AlternateText="Forward" ToolTip="<%$ Resources:PrivateMessage, PmFwd %>" OnClick="ButtonForward_Click" BorderWidth="1"/>
                    <asp:ImageButton ID="ButtonDelMsg" runat="server" SkinID="PMTrash"
AlternateText="Delete" ToolTip="<%$ Resources:PrivateMessage, PmDelete %>" OnClick="ButtonDelMsg_Click" BorderWidth="1"/> 
                    </div>
                </div>
                <div style="width:25%;float:left;background-color: white;height: 260px;overflow-y: scroll;">
                      <asp:TreeView id="PMTreeView"
                        Font-Name= "Arial"              
                        ShowExpandCollapse="True"
                        ForeColor="Blue"
                        EnableClientScript="true"
                        PopulateNodesFromClient="true"  
                        OnTreeNodePopulate="PopulateNode"
                        OnSelectedNodeChanged="NodeSelected"
                        OnTreeNodeCheckChanged="NodeChecked"
                        runat="server"
                        SkinID="Explorer">
                        <Nodes>
                          <asp:TreeNode Text="Private Messages" 
                            SelectAction="Expand"  
                            PopulateOnDemand="true"/>
                        </Nodes>
                      </asp:TreeView>
                </div>
                <div style="width: 74%;border: 1px solid gray;height: auto;background-color: white;margin-left:26%;" class="clearfix">
                    <asp:Literal runat="server" ID="dummy"></asp:Literal>
                    <asp:Panel runat="server" ID="pnlMessage" Visible="False" CssClass="pm-new-message clearfix">
                        <asp:Label ID="lblRecipient" runat="server" Text="<%$ Resources:webResources, lblTo %>" AssociatedControlID="newTo"></asp:Label><asp:TextBox ID="newTo" runat="server" Width="40%"></asp:TextBox><asp:Label ID="lblMultiple" runat="server" Text="<%$ Resources:PrivateMessage, PmRecipientNote %>" ></asp:Label><br/>
                        <asp:Label ID="lblSubject" runat="server" Text="<%$ Resources:webResources, lblSubject %>" AssociatedControlID="newSubject"></asp:Label><asp:TextBox ID="newSubject" runat="server" Width="60%"></asp:TextBox><br/>
                        <asp:Label ID="Label2" runat="server" Text="<%$ Resources:webResources, lblMessage %>" AssociatedControlID="newMessage" style="vertical-align: top;"></asp:Label>
                        <asp:TextBox ID="newMessage" CssClass="pmMsgArea" runat="server" TextMode="MultiLine" Rows="4"></asp:TextBox><br/>
                        <asp:ImageButton ID="pmSend" runat="server" SkinID="PMSendAlt"
                        OnClick="btnReceive_Click" ToolTip="<%$ Resources:PrivateMessage, PmSendReceive %>" AlternateText="Send/Receive" BorderWidth="1"/>

                    </asp:Panel>
                    <asp:Panel ID="PmOptions" runat="server" CssClass="pmoptions clearfix" Visible="False">
                        <asp:Panel ID="Panel1" runat="server" GroupingText="<%$ Resources:PrivateMessage, PmOptGroup1 %>">
                            <asp:RadioButtonList ID="rblEnabled" runat="server">
                                <asp:ListItem Value="1" Text='<%$ Resources:PrivateMessage, PmEnable %>'></asp:ListItem>
                                <asp:ListItem Value="0" Text='<%$ Resources:PrivateMessage, PmDisable %>'></asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <asp:Label ID="Label4" runat="server" Text="<%$ Resources:PrivateMessage, PmOptGroup1Text %>"></asp:Label>
                        </asp:Panel>
                        <asp:Panel ID="Panel2" runat="server" GroupingText="<%$ Resources:PrivateMessage, PmOptGroup2 %>">
                            <asp:Label ID="Label5" runat="server" Text="<%$ Resources:PrivateMessage, PmNote %>"></asp:Label>
                            <asp:Label ID="Label6" runat="server" Text="<%$ Resources:PrivateMessage, PmPrefLbl %>"></asp:Label>
                            <asp:RadioButtonList ID="rblNotify" runat="server">
                                <asp:ListItem Value="1" Text="<%$ Resources:PrivateMessage, PmNotify1 %>"></asp:ListItem>
                                <asp:ListItem Value="0" Text="<%$ Resources:PrivateMessage, PmNotify2 %>"></asp:ListItem>
                            </asp:RadioButtonList>
                        </asp:Panel>
                        <asp:Panel ID="Panel3" runat="server" GroupingText="<%$ Resources:PrivateMessage, PmOptGroup3 %>">
                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:PrivateMessage, PmPrefLbl %>"></asp:Label>
                            <asp:RadioButtonList ID="rblLayout" runat="server">
                                <asp:ListItem Value="double" Text="<%$ Resources:PrivateMessage, PmLayout1 %>"></asp:ListItem>
                                <asp:ListItem Value="none" Text="<%$ Resources:PrivateMessage, PmLayout2 %>"></asp:ListItem>
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </asp:Panel>
                </div>
                <div class="pm-footer">
                    <asp:Literal runat="server" ID="statusTxt"></asp:Literal>
                </div>
            </div>
        
    <ajax:ModalPopupExtender Drag="true" PopupDragHandleControlID="MPD2" ID="Panel1_ModalPopupExtender" runat="server" BackgroundCssClass="modalBackground"
      BehaviorID="popup"  Enabled="True" TargetControlID="buttonCheck" PopupControlID="FindMember" CancelControlID="btnCancelSearch">
    </ajax:ModalPopupExtender> 
            <!-- Modal popup -->


        </ContentTemplate>
    </asp:UpdatePanel>
</div>
<asp:Panel ID="FindMember" runat="server" Style="width: 360px;height:600px;" EnableViewState="false">
    <div class="memberModalPopup mainModalBorder">
        <div class="mainModalInnerDiv mainModalInnerBorder">
            <div id="header" style="width: 100%;" class="clearfix">
                <div class="mainModalDraggablePanelDiv">
                    <asp:Panel CssClass="mainModalDraggablePanel" ID="MPD2" runat="server" EnableViewState="false">
                        <span class="mainModalTitle" id="spanTitle1">Send private message</span>
                    </asp:Panel>
                </div>
                <div class="mainModalDraggablePanelCloseDiv">
                    <asp:ImageButton SkinID="CloseModal" runat="server" ID="clB2" CausesValidation="false" EnableViewState="false" />
                </div>
            </div>
            <div class="mainModalContent">
                <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2">
                    <ProgressTemplate>
                        <div style="position: fixed; top: 0px; left: 0px; width: 100%; height: 100%; background: #666; filter: alpha(opacity=80); -moz-opacity: .8; opacity: .8;">
                            <img src="/images/ajax-loader.gif" style="position: relative; top: 0%; left: 0%;" />
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
                <div id="Div3" class="clearfix">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="true">
                        <ContentTemplate>
                            <asp:TextBox ID="tbxFind" runat="server"></asp:TextBox><asp:ImageButton ID="ImageButton1"
                                runat="server" SkinID="Search" OnClick="SearchMember" />
                            <asp:DataList ID="Members" runat="server" ItemStyle-Wrap="False" DataKeyField="Id" GridLines="Horizontal" CssClass="forumtable smalltext" ShowHeader="False"
                                OnItemCommand="Members_ItemCommand" OnItemDataBound="MembersDataBound">
                                <HeaderTemplate>Member List</HeaderTemplate>
                                <ItemTemplate>
                                    <asp:HyperLink Width="200px" ID="lnkMember" runat="server" Target="_blank" NavigateUrl='<%# String.Format("~/Account/profile.aspx?user={0}", Eval("Username")) %>' Text='<%#Eval("Username") %>'></asp:HyperLink>
                                    <asp:ImageButton  AlternateText="pm" ToolTip='<%# Eval("Username") %>' CommandName="select" Width="16" Height="16" ID="sendpm" runat="server" SkinID="PMSend" CommandArgument='<%# Eval("Username") %>' />
                                </ItemTemplate>
                            </asp:DataList>
                            <br />
                            <asp:Panel ID="pnlControls" runat="server">
                                <asp:Label ID="Label7" runat="server" Text="Page"></asp:Label>
                                <asp:DropDownList ID="ddlCurrentPage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ChangePage">
                                </asp:DropDownList>
                                <asp:Label ID="Label8" runat="server" Text="of"></asp:Label>
                                <asp:Label ID="numPages" runat="server" Text="Label"></asp:Label>

                            </asp:Panel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlCurrentPage" EventName="SelectedIndexChanged"></asp:AsyncPostBackTrigger>
                            <asp:AsyncPostBackTrigger ControlID="Members" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <br />
                    <asp:LinkButton ID="btnCancelSearch" runat="server" Text="<%$ Resources:webResources,btnClose %>" />
                </div>
            </div>
        </div>
    </div>
</asp:Panel>

 