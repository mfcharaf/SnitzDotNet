using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;


    public partial class Admin_Subscriptions : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grdSubs.DataSource = Admin.GetAllSubscriptions();
            grdSubs.DataBind();
        }

        protected void grdSubs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName == "DeleteAll")
            //{
            //    Util.RemoveMemberSubscription(_user.Id);
            //}
            //else
            //{
            //    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            //    int topicid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["TopicId"]);
            //    int forumid = Convert.ToInt32(grdSubs.DataKeys[row.RowIndex]["ForumId"]);

            //    if (topicid > 0)
            //        Util.RemoveTopicSubscription(_user.Id, topicid);
            //    else if (forumid > 0)
            //        Util.RemoveForumSubscription(_user.Id, forumid);
            //}
        }

        protected void Rowdeleting(object sender, GridViewDeleteEventArgs e)
        {

            int topicid = Convert.ToInt32(grdSubs.DataKeys[e.RowIndex]["TopicId"]);
            int forumid = Convert.ToInt32(grdSubs.DataKeys[e.RowIndex]["ForumId"]);
            int memberid = Convert.ToInt32(grdSubs.DataKeys[e.RowIndex]["MemberId"]);

            if (topicid > 0)
                Subscriptions.RemoveTopicSubscription(memberid, topicid);
            else if (forumid > 0)
                Subscriptions.RemoveForumSubscription(memberid, forumid);
        }
    }
