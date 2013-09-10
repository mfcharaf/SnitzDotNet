<%@ Control CodeBehind="pmView.ascx.cs" Inherits="PrivateMessaging.PmView" Language="C#" AutoEventWireup="True" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>
<br />

<asp:Panel ID="PMControls" runat="server" Visible="<%# !PmViewMessage.Visible %>" style=" text-align:right" >
    <asp:ImageButton ID="ButtonOutBox" runat="server" CommandArgument="outbox" 
        OnClick="btnOutBox_Click" SkinID="Outbox" AlternateText="Outbox"  />
    <asp:ImageButton ID="ButtonInBox" runat="server" CommandArgument="inbox" 
        OnClick="btnOutBox_Click" SkinID="Inbox" AlternateText="Inbox" />
    <asp:ImageButton ID="ButtonNew" runat="server" SkinID="PMNew" 
        OnClick="btnNew_Click" AlternateText="New PM" />
    <asp:ImageButton ID="ButtonReceive" runat="server" SkinID="PMReceive" 
        OnClick="btnReceive_Click" AlternateText="Send/Receive" />
    <asp:ImageButton ID="ButtonMembers" runat="server" SkinID="PMMembers" 
        AlternateText="Member list" />
    <asp:ImageButton ID="ButtonOptions" runat="server" SkinID="PMOPtions" 
        OnClick="btnOptions_Click" AlternateText="Options" />

    <asp:ImageButton ID="ButtonReply" runat="server" SkinID="PMReply" 
        AlternateText="Reply" onclick="ButtonReply_Click" />
    <asp:ImageButton ID="ButtonReplyQuote" runat="server" SkinID="PMReplyQuote" 
        AlternateText="Reply with Quote" onclick="ButtonReplyQuote_Click" />
    <asp:ImageButton ID="ButtonForward" runat="server" SkinID="PMForward" 
        AlternateText="Forward" onclick="ButtonForward_Click" />
    <asp:ImageButton ID="ButtonDelete" runat="server" SkinID="PMDelete" 
        AlternateText="Delete" onclick="ButtonDelete_Click"/>
    </asp:Panel>
<br />
<asp:MultiView runat="server" ID="PMViews" ActiveViewIndex="0">
    <asp:View runat="server" ID="view1">
        <asp:Panel ID="InBox" runat="server" GroupingText="InBox">
            <asp:GridView ID="grdInBox" runat="server" EmptyDataText="You have no Private Messages in your Inbox"
                AutoGenerateColumns="False" EnableModelValidation="True" OnRowDataBound="InboxBound"
                CellPadding="4" GridLines="None" 
                HorizontalAlign="Center" CssClass="forumtable" DataKeyNames="Id">
                <AlternatingRowStyle CssClass="altrow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="pmImgRead" runat="server" SkinID="pmRead" Visible="false"></asp:Image>
                            <asp:Image ID="pmImgUnread" runat="server" SkinID="pmUnread" Visible="false"></asp:Image>
                        </ItemTemplate>
                        <HeaderStyle Width="40px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Subject">
                        <ItemTemplate>
                            <asp:LinkButton CssClass="pmLnk" ID="LinkButton1" Text='<%# Bind("Subject") %>' runat="server" CommandArgument='<%# Bind("Id") %>'
                                OnClick="ViewMessage"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="From">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("FromMemberName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" />
                    </asp:TemplateField>
            
                    <asp:BoundField DataField="Sent" HeaderText="Date" DataFormatString="{0:dd MMM yyyy HH:mm}"
                        HtmlEncode="False">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle Width="150px" />
                    </asp:BoundField>
                    <asp:TemplateField>
                             <HeaderTemplate>
                                <asp:CheckBox runat="server" ID="chkAll" />
                             </HeaderTemplate>
                             <HeaderStyle Width="40px" />
                        <ItemTemplate>
                            <asp:CheckBox ID="cbxDel" runat="server" CssClass="pmDelete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle BackColor="#999999" />
                <FooterStyle CssClass="" />
                <HeaderStyle CssClass="category cattitle" />
                <PagerStyle CssClass="" />
                <RowStyle CssClass="row" />
                <SelectedRowStyle ForeColor="#333333" />
            </asp:GridView>
            <br />
            <asp:LinkButton ID="btnDelMessage" runat="server" Text="Delete Selected" 
                onclick="btnDelMessage_Click" />
        </asp:Panel>
        <br />
        <asp:Panel ID="OutBox" runat="server" GroupingText="Sent Items">
            <asp:GridView ID="grdOutBox" runat="server" AutoGenerateColumns="False" EmptyDataText="You have no Private Messages in your Sent Items"
                EnableModelValidation="True" OnRowDataBound="InboxBound" CellPadding="4" ForeColor="#333333"
                GridLines="None" DataKeyNames="Id" CssClass="forumtable">
                <AlternatingRowStyle CssClass="altrow" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image ID="pmImgRead" runat="server" SkinID="pmRead" Visible="false"></asp:Image>
                            <asp:Image ID="pmImgUnread" runat="server" SkinID="pmUnread" Visible="false"></asp:Image>
                        </ItemTemplate>
                        <HeaderStyle Width="20px" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Subject">
                        <ItemTemplate>
                            <asp:LinkButton CssClass="pmLnk" ID="LinkButton1" Text='<%# Bind("Subject") %>' runat="server" CommandArgument='<%# Bind("Id") %>'
                                OnClick="ViewSentMessage"></asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="To">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Bind("ToMemberName") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Left"/>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Sent" DataFormatString="{0:dd MMM yyyy HH:mm}" HeaderText="Date"
                        HtmlEncode="False">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle Width="150px" />
                    </asp:BoundField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:CheckBox ID="cbxDel" runat="server" CssClass="pmRemove" />
                        </ItemTemplate>
                        <HeaderStyle Width="40px" />
                    </asp:TemplateField>
                </Columns>
                <EditRowStyle CssClass="" />
                <FooterStyle CssClass="" />
                <HeaderStyle CssClass="category cattitle" />
                <PagerStyle CssClass="" />
                <RowStyle CssClass="row" />
                <SelectedRowStyle ForeColor="#333333" />
            </asp:GridView>
            <br />
            <asp:LinkButton ID="btnRemMessage" runat="server" Text="Remove Selected" 
                onclick="btnRemMessage_Click" />
        </asp:Panel>        
    </asp:View>    
    <asp:View runat="server" ID="view2">
        <asp:Panel ID="PmMessage" runat="server" GroupingText="New private message" DefaultButton="btnSend">
            <asp:Label ID="lblRecipient" runat="server" Text="Send to" 
                AssociatedControlID="tbxRecipient"></asp:Label>
            <asp:TextBox ID="tbxRecipient" runat="server"></asp:TextBox>&nbsp;
            <asp:Label ID="lblMultiple" runat="server" Text="Separate multiple recipients using ; "></asp:Label>
            <br />
            <asp:Label ID="lblSubject" runat="server" Text="Subject" 
                AssociatedControlID="tbxSubject"></asp:Label>
            <asp:TextBox ID="tbxSubject" runat="server"></asp:TextBox>
            <br />
            <asp:TextBox ID="qrMessage" runat="server" TextMode="MultiLine" Rows="6" CssClass="QRMsgArea"></asp:TextBox>
            <br />
            <asp:Label ID="pmSuccess" runat="server" Text=""></asp:Label>
            <br />
            <asp:LinkButton ID="btnSend" runat="server" AlternateText="Send Message" OnClick="btnSend_Click"
                Text="<%$ Resources:webResources,btnSend %>" />
            <asp:LinkButton ID="btnCancel" runat="server" AlternateText="Cancel" OnClick="btnCancel_Click"
                Text="<%$ Resources:webResources,btnCancel %>" />
        </asp:Panel>        
    </asp:View>
    <asp:View runat="server" ID="view3">
        <asp:Panel ID="PmViewMessage" runat="server" class="clearfix">

            <div class="ReplyDiv clearfix">
                <div class="category cattitle">
                    <asp:Label ID="pmSubject" runat="server" Text="Label"></asp:Label>
                </div>
                <div class="leftColumn smalltext">
                    <asp:Literal ID="pmFrom" runat="server" Text="Username"></asp:Literal><br />
                    <asp:PlaceHolder ID="phAvatar" runat="server"></asp:PlaceHolder>
                    <br />
                    <asp:Label ID="pmTitle" runat="server" Text="Username"></asp:Label><br />
                    <asp:Label ID="pmCountry" runat="server" Text="Username"></asp:Label><br />
                    <asp:Label ID="pmPostcount" runat="server" Text="Username"></asp:Label><br />
                </div>
                <div class="MessageDIV">
                    <div class="buttonbar smalltext">
                        <asp:Label ID="pmDate" runat="server" Text="Label"></asp:Label>
                    </div>
                    <div class="mContent">
                        <asp:Label ID="pmBody" runat="server" Text='Message'></asp:Label>
                    </div>
                </div>
            </div>
        </asp:Panel>        
    </asp:View>
    <asp:View runat="server" ID="view4">
        <asp:Panel ID="PmOptions" runat="server" DefaultButton="btnPmOptions"
            CssClass="pmoptions">
            <asp:Panel ID="Panel1" runat="server" GroupingText="Enable/Disable Private Messages">
                <asp:Label ID="Label1" runat="server" Text="private messages are currently turned OFF"></asp:Label>.<br />
                <asp:Literal ID="Label2" runat="server" Text="You may turn it <b>ON</b> below.  By doing so you will no longer be able to receive private messages"></asp:Literal><br />
                <asp:RadioButtonList ID="rblEnabled" runat="server">
                    <asp:ListItem Value="1">Enable private messages</asp:ListItem>
                    <asp:ListItem Value="0">Disable private messages</asp:ListItem>
                </asp:RadioButtonList>
                <br />
                <asp:Label ID="Label3" runat="server" Text="You may turn Private Messages back on by returning to this page"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="Panel2" runat="server" GroupingText="Email Notification">
                <asp:Label ID="Label4" runat="server" Text="can send you an e-mail notification whenever you receive a private message."></asp:Label>
                <asp:Label ID="Label6" runat="server" Text="Please select your preference."></asp:Label>
                <asp:RadioButtonList ID="rblNotify" runat="server">
                    <asp:ListItem Value="1">Receive e-mail notification of private messages</asp:ListItem>
                    <asp:ListItem Value="0">Do not receive notification</asp:ListItem>
                </asp:RadioButtonList>
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server" GroupingText="InBox/OutBox Preferences">
                <asp:Label ID="Label5" runat="server" Text="Please select your layout preference."></asp:Label>
                <asp:RadioButtonList ID="rblLayout" runat="server">
                    <asp:ListItem Value="single">Single page layout :  both inbox and outbox appear on the same page</asp:ListItem>
                    <asp:ListItem Value="double">2 page layout :  inbox and outbox appear on seperate pages</asp:ListItem>
                    <asp:ListItem Value="none">No Outbox :  No outbox. Sent messages are not saved</asp:ListItem>
                </asp:RadioButtonList>
            </asp:Panel>
            <asp:Label ID="lblResult" runat="server" Text=""></asp:Label><br />
            <asp:LinkButton ID="btnPmOptions" Text="<%$ Resources:webResources,btnUpdate %>" runat="server" OnClick="SaveOptions"
                AlternateText="Save Options" />
        </asp:Panel>        
    </asp:View>

</asp:MultiView>









<!-- Modal popup --> 
    <asp:Panel ID="FindMember" runat="server" Style="display: none;width:360px" EnableViewState="false">
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
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2" >
                            <ProgressTemplate>
                                <div style="position:fixed;top:0px;left:0px; width:100%;height:100%;background:#666;filter: alpha(opacity=80);-moz-opacity:.8; opacity:.8;"  >
                                    <img src="/images/ajax-loader.gif" style="position:relative; top:45%;left:45%;" />
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
                                        <asp:HyperLink Width="200px" ID="lnkMember" runat="server" Target="_blank" NavigateUrl='<%# String.Format("~/Account/profile.aspx?user={0}", Eval("Username")) %>' Text='<%#Eval("Username") %>' ></asp:HyperLink>
                                        <asp:ImageButton AlternateText="pm" ToolTip='<%# Eval("Username") %>' CommandName="select" Width="16" Height="16" ID="sendpm" runat="server" SkinID="PMSend" CommandArgument='<%# Eval("Username") %>' OnClick="SendPM" /></ItemTemplate>
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
                            <asp:AsyncPostBackTrigger ControlID="Members" EventName="ItemCommand" />
                            </Triggers>
                         </asp:UpdatePanel> 
                    <br />
                    <asp:LinkButton ID="btnCancelSearch" runat="server" Text="<%$ Resources:webResources,btnClose %>" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>

<ajax:ModalPopupExtender Drag="true" PopupDragHandleControlID="MPD2" ID="Panel1_ModalPopupExtender" runat="server" BackgroundCssClass="modalBackground"
    Enabled="True" TargetControlID="ButtonMembers" PopupControlID="FindMember" CancelControlID="btnCancelSearch"></ajax:ModalPopupExtender>






