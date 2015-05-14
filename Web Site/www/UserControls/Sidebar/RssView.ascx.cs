/*
####################################################################################################################
##
## SnitzUI.UserControls.Sidebar - RssView.ascx
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzUI.UserControls
{
    public partial class RssView : UserControl
    {
        public string RssUrl { get; set; } 
       
        protected System.Xml.XmlDocument Doc;
        protected string ImgUrl;

        public override void DataBind()
        {
            Doc = new System.Xml.XmlDocument();
            Doc.Load(RssUrl);
            var link = Doc.SelectSingleNode("/rss/channel/link");
            if (link != null)
                ImgUrl = link.InnerText;
            base.DataBind();
        }

        public void Page_Load(Object s, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(RssUrl))
                {
                    noFeed.Text = "<b>No Url defined</b>";
                    return;
                }
                DataBind();
                var data = Doc.SelectNodes("/rss/channel/item[position()<=4]");
                RSSViewer.DataSource = data;
                RSSViewer.DataBind();
            }
            catch (Exception)
            {
                //swallow any errors
            }

        }

        protected void CheckData(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var data = e.Item.DataItem;
            }
        }
    }
}