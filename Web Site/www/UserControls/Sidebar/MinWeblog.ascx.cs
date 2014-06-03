/*
####################################################################################################################
##
## SnitzUI.UserControls.Sidebar - MiniActiveTopics.ascx
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using GroupedRepeater.Controls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzUI.UserControls.Post_Templates;

namespace SnitzUI.UserControls
{
    public partial class MiniWebLog : System.Web.UI.UserControl
    {
        protected PageBase ThisPage;
        public int TopicCount { get; set; }
        public bool Hide { get; set; }
        public int MemberId { get; set; }
        public int ForumId { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ThisPage = (PageBase)Page;
            

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ThisPage = (PageBase)Page;
            blogYears.DataSource = Forums.GetUserBlogTopics(ForumId, MemberId);
            blogYears.Comparer = new YearComparer();
            blogYears.DataBind();
        }

        private bool headerchange = false;
        private int currentyear = 0;
        protected void bindYears(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Separator)
            {
                headerchange = true;
                currentyear = ((TopicInfo) e.Item.DataItem).Date.Year;

            }
            if (e.Item.ItemType == ListItemType.Item && headerchange)
            {
                var blogposts = (GroupingRepeater)e.Item.FindControl("blogposts");
                blogposts.DataSource = Forums.GetUserBlogTopics(ForumId, MemberId).Where(t=>t.Date.Year==currentyear);
                blogposts.Comparer = new MonthComparer();
                blogposts.DataBind();
                headerchange = false;
            }

        }

        private class YearComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                    return -1;

                TopicInfo v1 = x as TopicInfo;
                TopicInfo v2 = y as TopicInfo;
                if (v1 == null && v2 == null)
                {
                    return 0;
                }
                string month1 = v1 == null ? "0" : v1.Date.ToString("yyyy");
                string month2 = v2 == null ? "0" : v2.Date.ToString("yyyy");

                return month1.CompareTo(month2);
            }
        }
        private class MonthComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null || y == null)
                    return -1;

                TopicInfo v1 = x as TopicInfo;
                TopicInfo v2 = y as TopicInfo;
                if (v1 == null && v2 == null)
                {
                    return 0;
                }
                string month1 = v1 == null ? "0" : v1.Date.ToString("MM");
                string month2 = v2 == null ? "0" : v2.Date.ToString("MM");

                return month1.CompareTo(month2);
            }
        }
    }
}