using System;
using System.Collections;
using System.Collections.Generic;
using Snitz.BLL.modconfig;
using Snitz.Entities;

namespace ModConfig
{
    public class RSSFeedConfig : ModConfigBase, ISnitzModConfig
    {
        public bool ShowOnMenu { get { return false; } }
        public ModMenuItem Menu { get { return null; } }

        #region Mod specific Properties

        public static string Url { get; set; }

        #endregion

        public RSSFeedConfig() : base("RSSFeed")
        {
            Url = ModConfiguration.Settings["Url"].ToString();
            ModConfiguration.AdminControl = "RSSFeed.ascx";
        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>
                                                  {
                                                      {
                                                          "Url",
                                                          "http://forum.snitz.com/forum/rssfeed2.asp"
                                                      }
                                                  };

            ModInfo modinfo = new ModInfo
            {
                Id = -1,
                Name = "RSSFeed",
                Description = "Displays an RSS feed",
                Version = new Version(1, 1),
                Enabled = true,
                AdminControl = "RSSFeed.ascx",
                Settings = new Hashtable(settings)
            };

            controller.ModInfo = modinfo;
            controller.InstallMod();
            return modinfo;
        }

    }
}