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
using System.Linq;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;

namespace SnitzUI.UserControls
{
    public partial class MiniActiveTopics : System.Web.UI.UserControl
    {
        protected PageBase ThisPage;
        public int TopicCount { get; set; }
        public bool Hide { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ThisPage = (PageBase)Page;
            var activetopics = Topics.GetLatestTopics(TopicCount);
            if (activetopics == null || !activetopics.Any())
            {
                Hide = true;
            }
            else
            {
                Hide = false;
                DataList1.DataSource = activetopics;
                DataList1.DataBind();
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            ThisPage = (PageBase)Page;

        }


        protected void BindRepeater(object sender, RepeaterItemEventArgs e)
        {
            Literal test = (Literal)e.Item.FindControl("lastposttime");
            if (test != null)
            {
                test.Text = Common.TimeAgoTag(((TopicInfo)e.Item.DataItem).LastPostDate,
                    ThisPage.IsAuthenticated, ThisPage.Member == null ? 0 : ThisPage.Member.TimeOffset);
            }
        }
    }
}