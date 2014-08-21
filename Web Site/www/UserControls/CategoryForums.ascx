<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CategoryForums.ascx.cs" Inherits="SnitzUI.UserControls.CategoryForums" EnableViewState="false" EnableTheming="true" %>
<%@ Import Namespace="SnitzCommon" %>
<%@ Import Namespace="SnitzConfig" %>

<asp:Repeater ID="repForum" runat="server" OnItemDataBound="RepForumItemDataBound" EnableViewState="False" OnItemCreated="repForum_ItemCreated">
                <HeaderTemplate>
                    <table id="defaultTopicTable" style="table-layout: fixed; width: 100%;">
                    <tr style="padding: 0px;">
                        <td style="padding: 0px;vertical-align: top;" colspan="3">
                            <table class="forumtable" cellpadding="3" cellspacing="0" style="width: 100%; margin: 0px; table-layout: fixed;" >
                                <thead runat="server" ID="fTableHeader">
                                <tr>
                                <th class="tableheader iconCol" ></th>
                                <th class="tableheader nowrap forumCol">
                                    <asp:Label ID="LF" runat="server" Text="<%$ Resources:webResources, lblForum %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader nowrap countCol">
                                    <asp:Label ID="Label1" runat="server" Text="<%$ Resources:webResources, lblTopics %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader nowrap countCol">
                                    <asp:Label ID="Label2" runat="server" Text="<%$ Resources:webResources, lblPosts %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader lastpostCol">
                                    <asp:Label ID="Label3" runat="server" Text="<%$ Resources:webResources, lblLastPost %>" EnableViewState="False"></asp:Label>
                                </th>
                                <th class="tableheader buttonCol"></th>
                            </tr>
                            </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="row">
                        <td valign="top" class="iconCol">
                            <asp:PlaceHolder  ID="Ticons" runat="server" EnableViewState="False"></asp:PlaceHolder>
                        </td>
                        <td valign="top" runat="server" id="linkCol" class="forumCol">
                            <asp:HyperLink CssClass="forumlink bbcode" NavigateUrl='<%# String.Format("/Content/Forums/forum.aspx?FORUM={0}",Eval("Id")) %>' runat="server" ID="forumLink"><%# Eval("Subject") %></asp:HyperLink><br />
                            <span class="smallText bbcode"><%# Eval("Description") %></span>
                        </td>
                        <td class="nowrap countCol" align="center" valign="top" runat="server" id="tCount">
                            <%# Common.TranslateNumerals(Eval("TopicCount"))%>
                        </td>
                        <td class="nowrap countCol" style="text-align: center" valign="top" runat="server" id="pCount">
                            <%# Common.TranslateNumerals(Eval("PostCount"))%>&nbsp;
                        </td>
                        <td class="smallText lastpostCol" runat="server" id="lastpost">
                            <asp:HyperLink ID="hypTopic" runat="server" Visible='<%# (Eval("LastPostTopicId") != null) %>' NavigateUrl='<%# String.Format("/Content/Forums/topic.aspx?TOPIC={0}",Eval("LastPostTopicId")) %>' ToolTip='<%# Eval("LastPostSubject") %>'>
                                <span class="bbcode"><%# Eval("LastPostSubject") %></span><br /></asp:HyperLink>
                            <asp:Literal ID="lDate" runat="server" EnableViewState="False" />
                            <br />
                            <asp:Label ID="Label4" runat="server" Text="by:" Visible='<%# Convert.ToInt32(Eval("TopicCount")) > 0 %>' EnableViewState="False"></asp:Label>
                            <asp:Literal ID="popuplink" runat="server" Text='<%# Eval("LastPostAuthorPopup") %>' EnableViewState="False"></asp:Literal>

                            &nbsp;<a href='/Content/Forums/topic.aspx?TOPIC=<%# Eval("LastPostTopicId") %>&amp;whichpage=-1#<%# Eval("LastPostReplyId") %>'><asp:Image
                                ID="imgLastPost" Visible='<%# (Config.ShowLastPostLink && (Eval("LastPostTopicId") != null && (int)Eval("LastPostTopicId") > 0)) %>'
                                ImageAlign="Bottom" runat="server" AlternateText='<%$ Resources:webResources, lblLastPostJump %>'
                                ToolTip='<%$ Resources:webResources, lblLastPostJump  %>' EnableViewState="False" /></a>
                        </td>
                        <td id="adminBtn" runat="server" valign="top" class="buttonCol">
                            <asp:HyperLink ID="hypNewTopic" runat="server" Text="<%$ Resources:webResources, lblNewTopic %>"
                                ToolTip="<%$ Resources:webResources, lblNewTopic %>" EnableViewState="False"></asp:HyperLink>
                            <asp:ImageButton ID="ForumEdit" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                runat="server" ToolTip="<%$ Resources:webResources, lblEditForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ForumLock" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lbllockForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ForumUnLock" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblUnlockForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ForumDelete" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblDelForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False"  />
                            <asp:ImageButton ID="ForumEmpty" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>'
                                    runat="server" ToolTip="<%$ Resources:webResources, lblEmptyForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ArchiveForum" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>' 
                                    runat="server" ToolTip="Archive" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            
                            <asp:ImageButton ID="ForumSub" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>' 
                                    runat="server" ToolTip="<%$ Resources:webResources, lblSubscribeForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:ImageButton ID="ForumUnSub" Visible='<%# IsAdministrator %>' CommandArgument='<%# Eval("Id")%>' 
                                    runat="server" ToolTip="<%$ Resources:webResources, lblUnSubscribeForum %>" OnClientClick="" CausesValidation="False" EnableViewState="False" />
                            <asp:HyperLink ID="hypViewArchive" runat="server" Text="A"
                                    ToolTip="Show Archived topics" EnableViewState="False"></asp:HyperLink>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table></td></tr></table>
                </FooterTemplate>
            </asp:Repeater>
