/*
####################################################################################################################
##
## SnitzUI.UserControls - SideColumn.ascx
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
using ModConfig;
using SnitzConfig;

namespace SnitzUI.UserControls
{
    public partial class SideColumn : System.Web.UI.UserControl
    {
        public string Show { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var controlsToShow = Show.Split(',');

            foreach (string control in controlsToShow)
            {
                switch (control)
                {
                    case "Events" :
                        if (ConfigHelper.IsModEnabled("EventsConfig"))
                        {
                            var events = Page.LoadControl("~/UserControls/Events/UpComingEvents.ascx");
                            events.EnableViewState = false;
                            Container.Controls.Add(events);
                        }
                        break;
                    case "Stats":
                        if (Config.ShowStats)
                        {
                            var stats = Page.LoadControl("~/UserControls/SideBar/MiniStatistics.ascx");
                            stats.EnableViewState = false;
                            Container.Controls.Add(stats);
                        }
                        break;
                    case "Rss":
                        if (ConfigHelper.IsModEnabled("RSSFeedConfig"))
                        {
                            var rss = (RssView)Page.LoadControl("~/UserControls/SideBar/rssview.ascx");
                            rss.RssUrl = ConfigHelper.GetStringValue("RSSFeedConfig", "Url");
                            rss.EnableViewState = false;
                            Container.Controls.Add(rss);
                        }
                        break;
                    case "Ads" :
                        if (Config.ShowSideAds)
                        {
                            var ads = (GoogleAdCode) Page.LoadControl("~/UserControls/googleadcode.ascx");
                            ads.Visible = Config.ShowSideAds && Config.ShowGoogleAds;
                            ads.AdSlot = "7801400008";
                            ads.AdHeight = 250;
                            ads.AdWidth = 300;
                            ads.EnableViewState = false;
                            Container.Controls.Add(ads);
                        }
                        break;
                    case "Active" :
                        var active = (MiniActiveTopics)Page.LoadControl("~/UserControls/SideBar/MiniActiveTopics.ascx");
                        active.TopicCount = 5;
                        active.EnableViewState = false;
                        Container.Controls.Add(active);
                        active.Visible = !active.Hide;
                        break;
                    case "Poll":
                        if (Config.ActivePoll > 0)
                        {
                            var poll = (Poll) Page.LoadControl("~/UserControls/Polls/Poll.ascx");
                            poll.PollId = Config.ActivePoll;
                            poll.EnableViewState = false;
                            Container.Controls.Add(poll);
                        }
                        break;
                }
            }
        }
    }
}