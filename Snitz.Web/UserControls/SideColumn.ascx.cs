﻿using System;
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
                        var events = Page.LoadControl("~/UserControls/Events/UpComingEvents.ascx");
                        events.EnableViewState = false;
                        Container.Controls.Add(events);
                        break;
                    case "Stats":
                        var stats = Page.LoadControl("~/UserControls/SideBar/MiniStatistics.ascx");
                        stats.EnableViewState = false;
                        Container.Controls.Add(stats);
                        break;
                    case "Rss":
                        var rss = Page.LoadControl("~/UserControls/SideBar/rssview.ascx");
                        rss.EnableViewState = false;
                        Container.Controls.Add(rss);
                        break;
                    case "Ads" :
                        var ads = (GoogleAdCode)Page.LoadControl("~/UserControls/googleadcode.ascx");
                        ads.Visible = Config.ShowSideAds && Config.ShowGoogleAds;
                        ads.AdSlot = "7801400008";
                        ads.AdHeight = 250;
                        ads.AdWidth = 300;
                        ads.EnableViewState = false;
                        Container.Controls.Add(ads);
                        break;
                    case "Active" :
                        var active = (MiniActiveTopics)Page.LoadControl("~/UserControls/SideBar/MiniActiveTopics.ascx");
                        active.TopicCount = 5;
                        active.EnableViewState = false;
                        Container.Controls.Add(active);
                        active.Visible = !active.Hide;
                        break;
                    case "Poll":
                        var poll = (Poll)Page.LoadControl("~/UserControls/Polls/Poll.ascx");
                        poll.PollId = Config.ActivePoll;
                        poll.EnableViewState = false;
                        if (poll.PollId > 0)
                        {
                            Container.Controls.Add(poll);
                        }
                        break;
                }
            }
        }
    }
}