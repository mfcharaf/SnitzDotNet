
using System;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

namespace ModConfig
{
    public class EventsConfig : ISnitzModConfig
    {
        public string Name { get { return "Events Calendar"; } }
        public string Description { get { return "Events Calendar"; } }
        public Version Version { get { return new Version(1, 1); } }
        public bool ShowOnMenu { get { return true; } }

        public ModMenuItem Menu
        {
            get
            {
                string menuNode = String.Format("<siteMapNode url=\"{0}\" title=\"Events\" description=\"Events\"/>",@"~\Events");
                return new ModMenuItem()
                       {
                           InsertAfter = "Active",
                           MenuXml = menuNode
                       };
            }
        }

        [Description("boolEnableEvents")]
        public bool Enabled
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolEnableEvents"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolEnableEvents", value ? "1" : "0");
            }
        }
    }
}