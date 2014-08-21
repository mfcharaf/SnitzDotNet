using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


public partial class Admin_Subscriptions : System.Web.UI.UserControl
{
    private IEnumerable<SubscriptionInfo> _subscriptions;
        protected void Page_Load(object sender, EventArgs e)
        {
            _subscriptions = Admin.GetAllSubscriptions();
            if(!Page.IsPostBack || rbAll.Checked)
            {
                
                grdSubs.DataSource = _subscriptions.OrderBy(c => c.CategoryId).ThenBy(f => f.ForumId).ThenByDescending(t => t.TopicId);
                grdSubs.DataBind();
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            CatForumGrouping();

            base.Render(writer);
        }

        private void CatForumGrouping()
        {
            if (grdSubs.Controls.Count == 0)
                return;
            const int catColumnIndex = 0;
            const int forumColumnIndex = 1;

            // Reference the Table the GridView has been rendered into        
            var gridTable = (Table)grdSubs.Controls[0];
            // Enumerate each TableRow, adding a sorting UI header if        
            // the sorted value has changed        
            string lastCat = string.Empty;
            string lastForum = string.Empty;
            foreach (GridViewRow gvr in grdSubs.Rows)
            {
                string currentCat = gvr.Cells[catColumnIndex].Text.Replace("&nbsp;","");
                string currentForum = gvr.Cells[forumColumnIndex].Text.Replace("&nbsp;", "");

                if (lastCat.CompareTo(currentCat) != 0 && !String.IsNullOrEmpty(currentCat))
                {
                    CategoryInfo category = Categories.GetCategoryByName(currentCat).First();
                    var subscriptions =
                        Enumerators.GetEnumDescription((Enumerators.Subscription) category.SubscriptionLevel);
                    string catLink = String.Format("{0} ({1})", category.Name,
                                                   subscriptions);

                    // there's been a change in value in the category column                
                    int rowIndex = gridTable.Rows.GetRowIndex(gvr);
                    // Add a new category header row                
                    var sortRow = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
                    var sortCell = new TableCell
                    {
                        Text = catLink,
                        CssClass = "tableheader"
                    };
                    sortCell.ColumnSpan = grdSubs.Columns.Count;
                    sortRow.Cells.Add(sortCell);
                    gridTable.Controls.AddAt(rowIndex, sortRow);
                    // Update lastValue                
                    lastCat = currentCat;
                }
                if (lastForum.CompareTo(currentForum) != 0 && !String.IsNullOrEmpty(currentForum))
                {
                    ForumInfo forum = Forums.GetForumBySubject(currentForum.Trim()).First();
                    var subscriptions =
                        Enumerators.GetEnumDescription((Enumerators.Subscription)forum.SubscriptionLevel);
                    string forumLink = String.Format("{0} ({1})",forum.Subject,subscriptions);
                    // there's been a change in value in the forum column                
                    int rowIndex = gridTable.Rows.GetRowIndex(gvr);
                    // Add a new forum header row                
                    var sortRow = new GridViewRow(rowIndex, rowIndex, DataControlRowType.DataRow, DataControlRowState.Normal);
                    
                    var sortCell = new TableCell
                    {

                        Text = forumLink,
                        CssClass = "ForumHeaderRow"
                    };
                    sortCell.ColumnSpan = grdSubs.Columns.Count;
                    //sortRow.Cells.Add(spacer);
                    sortRow.Cells.Add(sortCell);
                    gridTable.Controls.AddAt(rowIndex, sortRow);
                    // Update lastValue                
                    lastForum = currentForum;
                }

                gvr.Cells.RemoveAt(1);
                gvr.Cells.RemoveAt(0);
                gvr.Cells[0].ColumnSpan = 3;

            }

        }

        
        protected void CheckedChanged(object sender, EventArgs e)
        {
            IEnumerable<SubscriptionInfo> filtered = _subscriptions;

            if (rbBoard.Checked)
            {
                filtered = _subscriptions.Where(s => s.CategoryId == 0);
            }
            if (rbCategory.Checked)
            {
                filtered = _subscriptions.Where(s => s.CategoryId > 0 && s.ForumId == 0);
            }
            if (rbForum.Checked)
            {
                filtered = _subscriptions.Where(s => s.ForumId > 0 && s.TopicId == 0);
            }
            if (rbTopic.Checked)
            {
                filtered = _subscriptions.Where(s => s.TopicId > 0);
            }
            grdSubs.DataSource = filtered.OrderBy(c => c.CategoryId).ThenBy(f => f.ForumId).ThenByDescending(t => t.TopicId);
            grdSubs.DataBind();

        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grdSubs.Rows)
            {
                int topicid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["TopicId"]);
                int forumid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["ForumId"]);
                int categoryid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["CategoryId"]);
                int memberid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["MemberId"]);

                if (topicid > 0)
                    Subscriptions.RemoveTopicSubscription(memberid, topicid);
                else if (forumid > 0)
                    Subscriptions.RemoveForumSubscription(memberid, forumid);
                else if (categoryid > 0)
                    Subscriptions.RemoveCategorySubscription(memberid, categoryid);
            }
        }
    }
