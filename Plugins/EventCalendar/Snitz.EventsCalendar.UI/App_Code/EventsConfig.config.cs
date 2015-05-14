
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using Snitz.BLL.modconfig;
using Snitz.Entities;

namespace ModConfig
{
    public class EventsConfig : ModConfigBase, ISnitzModConfig
    {
        public static bool EventShowHolidays { get; set; }
        public static string[] EventAdminRoles { get; set; }

        public bool ShowOnMenu { get { return true; } }

        public ModMenuItem Menu
        {
            get
            {
                string menuNode = String.Format("<siteMapNode url=\"{0}\" title=\"Events\" description=\"Events\"/>",@"~\Events");
                return new ModMenuItem()
                       {
                           InsertAfter = "Topics",
                           MenuXml = menuNode
                       };
            }
        }

        public EventsConfig() : base("Events")
        {
            string allowedRoles = ModConfiguration.Settings["EventAdminRoles"] != null ? ModConfiguration.Settings["EventAdminRoles"].ToString() : String.Empty;
            EventAdminRoles = Regex.Split(allowedRoles, ",", RegexOptions.Singleline);
            ModConfiguration.AdminControl = "EventsAdmin.ascx";
            EventShowHolidays = ModConfiguration.Settings["EventShowHolidays"] == null || ModConfiguration.Settings["EventShowHolidays"].ToString() == "1";
        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>
                                                  {
                                                      {
                                                          "EventAdminRoles",
                                                          "Administrator,Moderator"
                                                      },
                                                      {
                                                          "EventShowHolidays","1"
                                                      }
                                                  };
            ModInfo modinfo = new ModInfo
            {
                Id = -1,
                Name = "Events",
                Description = "Events Calendar",
                Version = new Version(1, 0),
                Enabled = true,
                AdminControl = "EventsAdmin.ascx",
                Settings = new Hashtable(settings)
            };

            controller.ModInfo = modinfo;
            controller.InstallMod();
            return modinfo;
        }

    }
}