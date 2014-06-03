<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MinWeblog.ascx.cs" Inherits="SnitzUI.UserControls.MiniWebLog" EnableViewState="False" %>
<%@ Register TagPrefix="cc1" Assembly="Snitz.ThirdParty" Namespace="GroupedRepeater.Controls" %>
<br />
<div class="blogList">
    <cc1:GroupingRepeater ID="blogYears" runat="server" EnableViewState="False" OnItemDataBound="bindYears">
        <GroupTemplate>
            <b><%# Eval("Date", "{0:yyyy}")  %></b><br/>
        </GroupTemplate>
        <ItemTemplate>
            <div style="padding-left: 8px;">
                <cc1:GroupingRepeater ID="blogposts" runat="server" EnableViewState="False">
                    <GroupTemplate>
                        <b><%# Eval("Date", "{0:MMMM}")  %></b>
                    </GroupTemplate>
                    <ItemTemplate>
                        <div class="ovHidden" style="padding-left: 16px;">
                            <a class="" href='/Content/Forums/topic.aspx?TOPIC=<%# Eval("Id") %>'
                                target='<%# Eval("Author.LinkTarget") %>' title="<%# Eval("Subject") %>">
                                <span class="minibbcode mImg"><%# HttpUtility.HtmlDecode(Eval("Subject").ToString()) %></span></a>
                        </div>
                    </ItemTemplate>
                </cc1:GroupingRepeater>
            </div>
        </ItemTemplate>
    </cc1:GroupingRepeater>

</div>
<br class="seperator" />
