<%@ Control Language="C#" AutoEventWireup="true" Inherits="Admin_features" Codebehind="features.ascx.cs" %>
    <%@ Register src="AdminRadioButton.ascx" tagname="AdminRadioButton" tagprefix="uc1" %>
    <asp:Panel ID="Panel1" runat="server" CssClass="clearfix forumtable" 
    DefaultButton="btnSubmit"  
    meta:resourcekey="Panel1Resource1">
    <h2 class="category">Feature Configuration</h2>
        <asp:Panel ID="Panel2" runat="server" GroupingText="General Features" 
            CssClass="featuresform" meta:resourcekey="Panel2Resource1">
            <asp:Label ID="lblIPLogging" runat="server" Text="IP Logging" 
                AssociatedControlID="rblIPLog" meta:resourcekey="lblIPLoggingResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblIPLog" runat="server" />
            <br />
            <asp:Label ID="lblFlood" runat="server" Text="Flood Control" 
                AssociatedControlID="rblFlood" meta:resourcekey="lblFloodResource1"></asp:Label>  
            <uc1:AdminRadioButton ID="rblFlood" runat="server" />             
            <asp:DropDownList ID="ddlFlood" runat="server" 
                meta:resourcekey="ddlFloodResource1">
                <asp:ListItem Value="30" meta:resourcekey="ListItemResource5">30 seconds</asp:ListItem>
                <asp:ListItem Value="60" meta:resourcekey="ListItemResource6">60 seconds</asp:ListItem>
                <asp:ListItem Value="90" meta:resourcekey="ListItemResource7">90 seconds</asp:ListItem>
                <asp:ListItem Value="120" meta:resourcekey="ListItemResource8">120 seconds</asp:ListItem>
            </asp:DropDownList><br />
            <asp:Label ID="lblGroupCat" runat="server" Text="Group Categories" 
                Enabled="False" AssociatedControlID="rblGroups" 
                meta:resourcekey="lblGroupCatResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblGroups" runat="server" SelectedValue="0" Enabled="False" /> <br />
            <asp:Label ID="lblSubscription" runat="server" 
                Text="Highest level of Subscription" AssociatedControlID="ddlSubs" 
                meta:resourcekey="lblSubscriptionResource1"></asp:Label>
            <asp:DropDownList ID="ddlSubs" runat="server" 
                meta:resourcekey="ddlSubsResource1">
                <asp:ListItem Value="0" Text="" meta:resourcekey="ListItemResource11">No subscriptions</asp:ListItem>
                <asp:ListItem Value="1" Text="" meta:resourcekey="ListItemResource12">Subscribe to Whole Board</asp:ListItem>
                <asp:ListItem Value="2" meta:resourcekey="ListItemResource13">Subscribe by Category</asp:ListItem>
                <asp:ListItem Value="3" meta:resourcekey="ListItemResource14">Subscribe by Forum</asp:ListItem>
                <asp:ListItem Value="4" meta:resourcekey="ListItemResource15">Subscribe by Topic</asp:ListItem>
            </asp:DropDownList><br />
            <asp:Label ID="lblBadWords" runat="server" Text="Bad Word Filter" 
                AssociatedControlID="rblBadWords" meta:resourcekey="lblBadWordsResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblBadWords" runat="server" /> <br />
            <asp:Label ID="Label3" runat="server" Text="Use CAPTCHA for login/Registration" 
                AssociatedControlID="rblCaptcha" meta:resourcekey="Label3Resource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblCaptcha" runat="server" /> 
        </asp:Panel>
        <asp:Panel ID="Panel9" runat="server" GroupingText="Galleries" CssClass="featuresform" 
            meta:resourcekey="Panel9Resource1">
            <asp:Label ID="Label2" runat="server" Text="Show Public Galleries" 
                AssociatedControlID="rblShowGallery" meta:resourcekey="Label2Resource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblShowGallery" runat="server" /> <br />
            <asp:Label ID="Label1" runat="server" Text="Allow User Galleries" 
                AssociatedControlID="rblPublicGallery" meta:resourcekey="Label1Resource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblPublicGallery" runat="server" /> <br />
        </asp:Panel>
        <asp:Panel ID="Panel3" runat="server" GroupingText="Moderation Features" 
            CssClass="featuresform" meta:resourcekey="Panel3Resource1">
            <asp:Label ID="lblTopicModeration" runat="server" 
                Text="Allow Topic Moderation" AssociatedControlID="rblTopicMod" 
                meta:resourcekey="lblTopicModerationResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblTopicMod" runat="server" /> 
            <br />
            <asp:Label ID="lblShowModerators" runat="server" Enabled="False" 
                Text="Show Moderators" AssociatedControlID="rblShowMods" 
                meta:resourcekey="lblShowModeratorsResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblShowMods" runat="server" Enabled="False"/><br />
            <asp:Label ID="lblRestrictMove" runat="server" 
                Text="Restrict Moderators to moving their own topics" 
                AssociatedControlID="rblRestrictMove" 
                meta:resourcekey="lblRestrictMoveResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblRestrictMove" runat="server" /><br />
            <asp:Label ID="lblMoveNotify" runat="server" 
                Text="AutoEmail author when moving topics" 
                AssociatedControlID="rblMoveNotify" 
                meta:resourcekey="lblMoveNotifyResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblMoveNotify" runat="server" /> 
        </asp:Panel>
        <asp:Panel ID="Panel4" runat="server" GroupingText="Forum Features" 
            CssClass="featuresform" meta:resourcekey="Panel4Resource1">
            <asp:Label ID="lblArchive" runat="server" Enabled="False" 
                Text="Archive Functions" AssociatedControlID="rblArchive" 
                meta:resourcekey="lblArchiveResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblArchive" runat="server" Enabled="False"/> 
            <br />
            <asp:Label ID="lblStatistics" runat="server" Text="Show Detailed Statistics" 
                AssociatedControlID="rblStats" meta:resourcekey="lblStatisticsResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblStats" runat="server" /> 
        </asp:Panel>
        <asp:Panel ID="Panel5" runat="server" GroupingText="Topic Features" 
            CssClass="featuresform" meta:resourcekey="Panel5Resource1">
            <asp:Label ID="lblEditedBy" runat="server" Text="Show Edited By on Date" 
                AssociatedControlID="rblEditedBy" meta:resourcekey="lblEditedByResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblEditedBy" runat="server" /> 
            <br />
            <asp:Label ID="lblPrevNext" runat="server" Text="Show Prev / Next Topic" 
                AssociatedControlID="rblPrevNext" meta:resourcekey="lblPrevNextResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblPrevNext" runat="server" /> 
            <br />
            <asp:Label ID="lblSendTopic" runat="server" 
                Text="Show Send Topic to a Friend Link" AssociatedControlID="rblSend" 
                meta:resourcekey="lblSendTopicResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblSend" runat="server" /> 
            <br />
            <asp:Label ID="lblPrinter" runat="server" Text="Show Printer Friendly Link" 
                AssociatedControlID="rblPrint" meta:resourcekey="lblPrinterResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblPrint" runat="server" /> 
            <br />
            <asp:Label ID="lblHotTopics" runat="server" Enabled="False" Text="Hot Topics" 
                AssociatedControlID="rblHotTopics" 
                meta:resourcekey="lblHotTopicsResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblHotTopics" runat="server" Enabled="False"/> 
            <asp:TextBox ID="tbxHotTopics" runat="server" Width="50px"></asp:TextBox>
            <br />
            <asp:Label ID="lblPageCount" runat="server" Text="Items per page" 
                AssociatedControlID="tbxPageItems" 
                meta:resourcekey="lblPageCountResource1"></asp:Label>
            <asp:TextBox ID="tbxPageItems" runat="server" Width="50px"></asp:TextBox>
            <br />
            <asp:Label ID="lblSearchPageCount" runat="server" 
                Text="Search results per page" AssociatedControlID="tbxSearchItems" 
                meta:resourcekey="lblSearchPageCountResource1"></asp:Label>
            <asp:TextBox ID="tbxSearchItems" runat="server" Width="50px"></asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel6" runat="server" GroupingText="Posting Features" 
            CssClass="featuresform" meta:resourcekey="Panel6Resource1">
            <asp:Label ID="lblForumCode" runat="server" Text="Allow Forum Code" 
                AssociatedControlID="rblForumCode" 
                meta:resourcekey="lblForumCodeResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblForumCode" runat="server" /> 
            <br />
            <asp:Label ID="lblPostImages" runat="server" Text="Images in Posts" 
                AssociatedControlID="rblImages" meta:resourcekey="lblPostImagesResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblImages" runat="server" /> 
            <br />
            <asp:Label ID="Label30" runat="server" Text="Emoticons" 
                AssociatedControlID="rblIcons" meta:resourcekey="Label30Resource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblIcons" runat="server" /> 
            <br />
            <asp:Label ID="lblSignatures" runat="server" Text="Allow Signatures" 
                AssociatedControlID="rblSignatures" 
                meta:resourcekey="lblSignaturesResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblSignatures" runat="server" /> 
            <br />
            <asp:Label ID="lblSmilieTable" runat="server" Text="Show Smilies Table" 
                AssociatedControlID="rblSmilieTable" 
                meta:resourcekey="lblSmilieTableResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblSmilieTable" runat="server" /> 
            <br />
            <asp:Label ID="lblQuickReply" runat="server" Text="Show Quick Reply" 
                AssociatedControlID="rblQuickReply" 
                meta:resourcekey="lblQuickReplyResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblQuickReply" runat="server" /> 
            <br />
        </asp:Panel>
        <asp:Panel ID="Panel7" runat="server" GroupingText="Misc Features" 
            CssClass="featuresform" meta:resourcekey="Panel7Resource1">
            <asp:Label ID="lblTimer" runat="server" Text="Show Timer" 
                AssociatedControlID="rblPageTimer" meta:resourcekey="lblTimerResource1"></asp:Label>
            <uc1:AdminRadioButton ID="rblPageTimer" runat="server" /> 
            <br />
            <asp:Label ID="lblEmailMode" runat="server" Text="ShowRanking" 
                AssociatedControlID="ddlShowRank" meta:resourcekey="lblEmailModeResource1"></asp:Label>
            <asp:DropDownList ID="ddlShowRank" runat="server" 
                meta:resourcekey="ddlShowRankResource1">
                <asp:ListItem Value="0" Text="" meta:resourcekey="ListItemResource60">None</asp:ListItem>
                <asp:ListItem Value="1" Text="" meta:resourcekey="ListItemResource61">Rank Only</asp:ListItem>
                <asp:ListItem Value="2" meta:resourcekey="ListItemResource62">Stars Only</asp:ListItem>
                <asp:ListItem Value="3" meta:resourcekey="ListItemResource63">Rank and Stars</asp:ListItem>
            </asp:DropDownList>
        </asp:Panel>
        <asp:Panel ID="Panel8" runat="server" meta:resourcekey="Panel8Resource1">
            <asp:LinkButton ID="btnSubmit" runat="server" Text="Submit" 
                OnClick="btnSubmit_Click" meta:resourcekey="btnSubmitResource1" />&nbsp;

            <asp:LinkButton ID="btnReset" runat="server" Text="Reset" 
                meta:resourcekey="btnResetResource1" />
        </asp:Panel>

    </asp:Panel>