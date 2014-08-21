using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzMembership;


namespace SnitzUI.UserControls.PrivateMessages
{
    public partial class PmViewAlt : System.Web.UI.UserControl
    {
        public int CurrentPage
        {
            get
            {
                // look for current page in ViewState
                object o = ViewState["_CurrentPage"];
                if (o == null)
                    return 1;
                return (int)o;
            }

            set
            {
                ViewState["_CurrentPage"] = value;
            }
        }
        public int CurrentMessageId
        {
            get
            {
                // look for current page in ViewState
                object o = ViewState["_CurrentPM"];
                if (o == null)
                    return 1;
                return (int)o;
            }

            set
            {
                ViewState["_CurrentPM"] = value;
            }
        }
        private int _pages;
        private string _layout;
        private bool _showOutBox;
        private readonly string _username = HttpContext.Current.User.Identity.Name;
        private string _redirectionScript;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            int totalmembers = Snitz.BLL.PrivateMessages.GetMemberCount(Session["_searchfor"] != null ? Session["_searchfor"].ToString() : String.Empty);
            _pages = Common.CalculateNumberOfPages(totalmembers, 11);
            SetupPager();
            var profile = Snitz.BLL.PrivateMessages.GetPreferences(_username);
            _layout = profile == null ? "double" : profile.PMLayout;

            switch (_layout)
            {
                case "none":
                    _showOutBox = false;
                    break;
                default:
                    _layout = "double";
                    _showOutBox = true;
                    break;
            }
            _redirectionScript =
             "function Delayer(){" +
             "setTimeout('Redirection()', 3000);" +
             "};" +
             "function Redirection(){" +
             " __doPostBack('" + upd.ClientID + "');" +
             "};" +
             "Delayer();";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
 
            }
            else
            {
                buttonCheck.Attributes.CssStyle["visibility"] = "hidden";
                
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "pmMsg","$('.pmMsgArea').markItUp(miniSettings);", true);
                //$("#ctl00_CPM_ctl00_PMTreeView input[type=checkbox]").click(function () {updatePanel();});
            }
            ScriptManager.RegisterStartupScript(Page,Page.GetType(), "pmCbx", "$('#ctl00_CPM_ctl00_PMTreeView input[type=checkbox]').click(function () {updatePanel();});", true);
            BindMemberList(Session["_searchfor"] != null ? Session["_searchfor"].ToString() : String.Empty);
            SetButtonDisplay();
        }

        #region Treeview Methods

        protected void PopulateNode(object sender, TreeNodeEventArgs e)
        {
            // Call the appropriate method to populate a node at a particular level.
            //PMTreeView.ShowCheckBoxes = TreeNodeTypes.Leaf;
            switch (e.Node.Depth)
            {
                case 0:
                    // Populate the first-level nodes.
                    PopulateCategories(e.Node);
                    break;
                case 1:
                    // Populate the second-level nodes.
                    PopulateProducts(e.Node);
                    break;
                default:
                    // Do nothing.
                    break;
            }

        }

        private void PopulateProducts(TreeNode node)
        {
            MembershipUser currentUser = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            List<PrivateMessageInfo> ResultSet = node.Value == "Inbox" ? Snitz.BLL.PrivateMessages.GetMessages((int)currentUser.ProviderUserKey) : Snitz.BLL.PrivateMessages.GetSentMessages((int)currentUser.ProviderUserKey);
            // Create the third-level nodes.
            if (ResultSet.Count > 0)
            {

                // Iterate through and create a new node for each row in the query results.
                // Notice that the query results are stored in the table of the DataSet.
                foreach (var row in ResultSet)
                {

                    // Create the new node.
                    string nodetext;
                    nodetext = node.Value == "Inbox" ? row.Subject + " (" + row.FromMemberName + ")" : row.Subject + " (" + row.ToMemberName + ")";
                    TreeNode NewNode = new TreeNode(nodetext)
                                       {
                                           PopulateOnDemand = false,
                                           ShowCheckBox = true,
                                           SelectAction = TreeNodeSelectAction.Select,
                                           Value = row.Id.ToString()
                                       };

                    if(row.Read > 0)
                        NewNode.ImageUrl = String.Format("/App_Themes/{0}/pm/icon_pm_old.gif",Page.Theme);
                    // Add the new node to the ChildNodes collection of the parent node.
                    node.ChildNodes.Add(NewNode);

                }

            }

        }

        private void PopulateCategories(TreeNode node)
        {

            TreeNode inbox = new TreeNode();
            inbox.Text = "Inbox";
            inbox.Value = "Inbox";
            inbox.PopulateOnDemand = true;

            // Set additional properties for the node.
            inbox.SelectAction = TreeNodeSelectAction.Expand;
            inbox.ImageUrl = String.Format("/App_Themes/{0}/pm/inbox.png", Page.Theme);
            // Add the new node to the ChildNodes collection of the parent node.
            node.ChildNodes.Add(inbox);
            if (_showOutBox)
            {
                TreeNode sent = new TreeNode();
                sent.Text = "Sent Items";
                sent.Value = "SentItems";
                sent.PopulateOnDemand = true;
                //sent.Expanded = false;
                // Set additional properties for the node.
                sent.SelectAction = TreeNodeSelectAction.Expand;
                sent.ImageUrl = String.Format("/App_Themes/{0}/pm/sent.png", Page.Theme);
                // Add the new node to the ChildNodes collection of the parent node.
                node.ChildNodes.Add(sent);
            }
        }

        protected void NodeSelected(object sender, EventArgs e)
        {
            String value = PMTreeView.SelectedNode.Value;
            String path = PMTreeView.SelectedNode.ValuePath;
            CurrentMessageId = Convert.ToInt32(value);
            ShowMessage(path);
        }

        protected void NodeChecked(object sender, TreeNodeEventArgs e)
        {
            bool check = false;
            //if any nodes checked then show delete button
            check = CheckAllNodes(PMTreeView.Nodes[0]);
            ButtonDelete.Visible = check;
        }

        private bool CheckAllNodes(TreeNode treeNode)
        {
            foreach (TreeNode node in treeNode.ChildNodes)
            {
                if (node.Checked)
                    return true;
                if (node.ChildNodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    if (this.CheckAllNodes(node))
                        return true;
                }
            }
            return false;
        }
        
        private void RefreshTreeView()
        {
            TreeNode parent = PMTreeView.Nodes[0];
            parent.ChildNodes.Clear();
            TreeNodeEventArgs ea = new TreeNodeEventArgs(parent);
            PopulateNode(this, ea);
        }

        #endregion

        #region Button Click Events

        protected void btnNew_Click(object sender, ImageClickEventArgs e)
        {
            pnlMessage.Visible = true;
            PmOptions.Visible = false;
            dummy.Text = String.Empty;
            dummy.Visible = false;
            lblMultiple.Visible = true;
            SetButtonDisplay();
        }

        protected void ButtonReply_Click(object sender, ImageClickEventArgs e)
        {
            var pm = Snitz.BLL.PrivateMessages.GetMessage(CurrentMessageId);
            pnlMessage.Visible = true;
            dummy.Text = String.Empty;
            dummy.Visible = false;
            SetButtonDisplay();
            newTo.Text = pm.FromMemberName;
            newTo.Enabled = false;
            lblRecipient.Visible = true;
            lblMultiple.Visible = false;
            newSubject.Text = Resources.PrivateMessage.PmViewRE + pm.Subject;
            newSubject.Enabled = false;
            lblSubject.Visible = true;

        }

        protected void ButtonReplyQuote_Click(object sender, ImageClickEventArgs e)
        {
            var pm = Snitz.BLL.PrivateMessages.GetMessage(CurrentMessageId);
            pnlMessage.Visible = true;
            dummy.Text = String.Empty;
            dummy.Visible = false;
            SetButtonDisplay();
            newTo.Text = pm.FromMemberName;
            newTo.Enabled = false;
            lblRecipient.Visible = true;
            lblMultiple.Visible = false;
            newMessage.Text = string.Format(@"[quote]{0}[/quote]", pm.Message);
            newSubject.Text = Resources.PrivateMessage.PmViewRE + pm.Subject;
            newSubject.Enabled = true;
            lblSubject.Visible = true;

        }

        protected void ButtonForward_Click(object sender, ImageClickEventArgs e)
        {
            PrivateMessageInfo pm = Snitz.BLL.PrivateMessages.GetMessage(CurrentMessageId);
            pnlMessage.Visible = true;
            dummy.Text = String.Empty;
            dummy.Visible = false;
            SetButtonDisplay();
            newTo.Text = "";
            newTo.Enabled = true;
            newMessage.Text = Resources.PrivateMessage.PmForwardedMessage + Environment.NewLine + pm.Message;
            newSubject.Text = Resources.PrivateMessage.PmViewFwd + pm.Subject;
            newSubject.Enabled = false;
            lblSubject.Visible = true;

        }

        protected void ButtonDelete_Click(object sender, ImageClickEventArgs e)
        {
            //are any nodes checked
            if (CheckAllNodes(PMTreeView.Nodes[0]))
            {
                foreach (TreeNode checkedNode in PMTreeView.CheckedNodes)
                {
                    int toRemove = Convert.ToInt32(checkedNode.Value);
                    if (checkedNode.ValuePath.Contains("SentItems"))
                    {
                        //Remove node from sent items
                        Snitz.BLL.PrivateMessages.RemoveFromOutBox(toRemove);
                        statusTxt.Text = Resources.PrivateMessage.PmMessageRemoved;
                    }
                    if (checkedNode.ValuePath.Contains("Inbox"))
                    {
                        Snitz.BLL.PrivateMessages.DeletePrivateMessage(toRemove);
                        statusTxt.Text = Resources.PrivateMessage.PmMessageRemoved;
                    }
                }
                RefreshTreeView();
                PMTreeView.ExpandAll();
                SetButtonDisplay();
            }
            
        }

        protected void btnReceive_Click(object sender, ImageClickEventArgs e)
        {
            if (pnlMessage.Visible)
            {
                SendPM();
            }
            RefreshTreeView();
            PMTreeView.ExpandAll();
            SetButtonDisplay();
        }

        protected void btnOptions_Click(object sender, ImageClickEventArgs e)
        {
            LoadUserPrefs();
        }
        protected void ButtonMembersClick(object sender, ImageClickEventArgs e)
        {
            pnlMessage.Visible = true;
            Panel1_ModalPopupExtender.Show();
        }
        protected void ButtonDelMsg_Click(object sender, ImageClickEventArgs e)
        {
            Snitz.BLL.PrivateMessages.DeletePrivateMessage(CurrentMessageId);
            dummy.Text = String.Empty;
            statusTxt.Text = Resources.PrivateMessage.PmMessageRemoved;
            CurrentMessageId = -1;
            RefreshTreeView();
            PMTreeView.ExpandAll();
            SetButtonDisplay();
        }

        protected void PopupSendPM(object sender, EventArgs e)
        {
            ImageButton lnk = (ImageButton)sender;
            pnlMessage.Visible = true;
            newTo.Text = lnk.CommandArgument;
            SetButtonDisplay();
        }
        #endregion


        #region Member List Methods

        private void SetupPager()
        {
            ddlCurrentPage.Items.Clear();
            for (int i = 1; i <= _pages; i++)
            {
                ddlCurrentPage.Items.Add(i.ToString());
            }
            ddlCurrentPage.SelectedValue = CurrentPage.ToString();
            numPages.Text = _pages.ToString();
        }
        private void BindMemberList(string searchfor)
        {
            if (Session["_searchfor"] != null)
            {
                searchfor = Session["_searchfor"].ToString();
                Session["SearchFilter"] = "Name," + searchfor;
            }
            Members.DataSource = Snitz.BLL.Members.GetAllMembers("", CurrentPage, 15);
            Members.DataBind();
            int totalmembers = Snitz.BLL.PrivateMessages.GetMemberCount(searchfor);
            _pages = Common.CalculateNumberOfPages(totalmembers, 11);

            numPages.Text = _pages.ToString();
        }
        protected void ChangePage(object sender, EventArgs e)
        {
            CurrentPage = Convert.ToInt32(ddlCurrentPage.SelectedValue);
            BindMemberList(tbxFind.Text);
        }
        protected void SearchMember(object sender, ImageClickEventArgs e)
        {
            CurrentPage = 1;
            Session["_searchfor"] = tbxFind.Text;
            Members.DataSource = null;
            BindMemberList(tbxFind.Text);

            SetupPager();
        }
        protected void MembersDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                ImageButton ib = e.Item.FindControl("sendpm") as ImageButton;
                var scriptManager = ScriptManager.GetCurrent(Page);
                if (scriptManager != null && ib != null) scriptManager.RegisterPostBackControl(ib);
            }
        }
        protected void Members_ItemCommand(object source, DataListCommandEventArgs e)
        {
            newTo.Text = (string)e.CommandArgument;
            upd.Update();
            //Panel1_ModalPopupExtender.Hide();
        }

        #endregion

        private void ShowMessage(string path)
        {
            PrivateMessageInfo pm = Snitz.BLL.PrivateMessages.GetMessage(CurrentMessageId);
            string display = "";
            display += "<div class='pm-message-header'><b>" + pm.Subject + "</b><br/>";
            display += String.Format("<a href=\"/Account/profile.aspx?user={0}\">{0}</a>", pm.FromMemberName);
            display += " " + pm.Sent + "</div>";
            display += "<p>" + pm.Message.ParseTags() + "</p>";

            dummy.Text = display;
            pnlMessage.Visible = false;
            PmOptions.Visible = false;
            dummy.Visible = true;

            SetButtonDisplay();
            if (path.Contains("SentItems"))
            {
                ButtonDelMsg.Visible = false;
                ButtonReply.Visible = false;
                ButtonReplyQuote.Visible = false;
            }
        }

        private void SendPM()
        {
            MembershipUser currentUser = Membership.GetUser(_username);

            if (currentUser == null || currentUser.ProviderUserKey == null)
                return;

            string[] toMembers = Regex.Split(newTo.Text, ";");
            foreach (string member in toMembers)
            {
                ProfileCommon profile = ProfileCommon.GetUserProfile(member);
                MembershipUser recipient = Membership.GetUser(member, false);
                if (recipient != null && recipient.ProviderUserKey != null)
                {
                    var pm = new PrivateMessageInfo
                    {
                        FromMemberId = (int)currentUser.ProviderUserKey,
                        Read = 0,
                        Subject = newSubject.Text,
                        Message = newMessage.Text,
                        OutBox = _layout != "none" ? 1 : 0,
                        SentDate = DateTime.UtcNow.ToForumDateStr(),
                        ToMemberId = (int)recipient.ProviderUserKey,
                        Mail = profile.PMEmail == null ? 0 : profile.PMEmail.Value
                    };

                    Snitz.BLL.PrivateMessages.SendPrivateMessage(pm);
                }

            }
            //TODO: Send notify if required
            statusTxt.Text = Resources.PrivateMessage.PmSent;
            pnlMessage.Visible = false;
        }

        private void LoadUserPrefs()
        {
            MembershipUser currentUser = Membership.GetUser(_username);
            if (currentUser != null)
            {
                var userprefs = Snitz.BLL.PrivateMessages.GetPreferences(currentUser.UserName);
                rblLayout.SelectedValue = _layout;
                if (userprefs != null)
                {
                    rblEnabled.SelectedValue = userprefs.PMReceive.HasValue ? userprefs.PMReceive.Value.ToString() : "1";
                    rblNotify.SelectedValue = userprefs.PMEmail.ToString();
                }
                else
                {
                    rblEnabled.SelectedValue = "1";
                    rblNotify.SelectedValue = "1";
                }
            }
            else
            {
                Response.Write("couldn't get your preferences");
            }
            PmOptions.Visible = true;
            pnlMessage.Visible = false;
            dummy.Text = String.Empty;
            dummy.Visible = false;
            SetButtonDisplay();
        }

        private void SetButtonDisplay()
        {
            ButtonOptions.Visible = true;
            ButtonMembers.Visible = true;
            ButtonNew.Visible = true;
            statusTxt.Text = "";
            ButtonDelete.Visible = CheckAllNodes(PMTreeView.Nodes[0]);
            ButtonReceive.Visible = true;

            ButtonReply.Visible = !String.IsNullOrEmpty(dummy.Text);
            ButtonReplyQuote.Visible = !String.IsNullOrEmpty(dummy.Text);
            ButtonForward.Visible = !String.IsNullOrEmpty(dummy.Text);
            ButtonDelMsg.Visible = !String.IsNullOrEmpty(dummy.Text);
        }


    }
}