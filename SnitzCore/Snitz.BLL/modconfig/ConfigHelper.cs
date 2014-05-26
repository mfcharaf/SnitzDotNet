using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;
using Snitz.Entities;
using SnitzCommon;

namespace ModConfig
{
    public static class ConfigHelper
    {
        private const string NAMESPACE = "ModConfig.";
        private const string ASSEMBLY = ",App_Code";

        public static List<ModMenuItem> GetMenuItems()
        {
            List<ModMenuItem> menuitems = new List<ModMenuItem>();

            foreach (string modconfig in GetModConfigs())
            {
                Type modclass = Type.GetType("ModConfig." + modconfig + ",App_Code", true);
                if (modclass != null)
                {
                    object modInstance = Activator.CreateInstance(modclass);
                    var enabled = modclass.GetProperty("Enabled").GetValue(modInstance, null);
                    var showmenu = modclass.GetProperty("ShowOnMenu").GetValue(modInstance, null);
                    if (Convert.ToBoolean(enabled) && Convert.ToBoolean(showmenu))
                    {
                        ModMenuItem menu = (ModMenuItem) modclass.GetProperty("Menu").GetValue(modInstance, null);

                        menuitems.Add(menu);
                    }
                }
            }
            return menuitems;
        }
        public static List<string> GetModConfigs()
        {
            var mods = new List<string>();
            string path = HttpContext.Current.Server.MapPath("~/App_Code");

            var res = Directory.GetFiles(path, "*.config.cs");
            foreach (string file in res)
            {
                mods.Add(GetFileNameWithoutExtensions(file));
            }
            return mods;
        } 
        /// <summary>
        /// Fetches a boolean property value determining if the MOD is enabled
        /// </summary>
        /// <param name="modconfigname">ModConfigClass e.g. SampleModConfig </param>
        /// <returns>The property value as a Boolean</returns>
        public static bool IsModEnabled(string modconfigname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty("Enabled").GetValue(modInstance, null);
                return Convert.ToBoolean(res);
            }
            return false;
        }
        public static string ModVersion(string modconfigname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty("Version").GetValue(modInstance, null);
                return ((Version)res).ToString();
            }
            return "";
        }
        public static string ModDisplayName(string modconfigname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty("Name").GetValue(modInstance, null);
                return res.ToString();
            }
            return "Unknown";
        }
        public static string ModDescription(string modconfigname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty("Description").GetValue(modInstance, null);
                return res.ToString();
            }
            return "Unknown";
        }

        /// <summary>
        /// Fetches a string property value for a Mod Configuration
        /// </summary>
        /// <param name="modconfigname">ModConfigClass e.g. SampleModConfig </param>
        /// <param name="propertyname">Name of the Property to retreive</param>
        /// <returns>The property value as a string</returns>
        public static string GetStringValue(string modconfigname, string propertyname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);
            
            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty(propertyname, BindingFlags.Public | BindingFlags.Static).GetValue(modInstance, null);
                return res.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// Fetches a boolean property value for a Mod Configuration
        /// </summary>
        /// <param name="modconfigname">ModConfigClass e.g. SampleModConfig </param>
        /// <param name="propertyname">Name of the Property to retreive</param>
        /// <returns>The property value as a Boolean</returns>
        public static bool GetBoolValue(string modconfigname, string propertyname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty(propertyname, BindingFlags.Public | BindingFlags.Static).GetValue(modInstance, null);
                return Convert.ToBoolean(res);
            }
            return false;
        }

        /// <summary>
        /// Fetches an integer property value for a Mod Configuration
        /// </summary>
        /// <param name="modconfigname">ModConfigClass e.g. SampleModConfig </param>
        /// <param name="propertyname">Name of the Property to retreive</param>
        /// <returns>The property value as an Integer</returns>
        public static int GetIntValue(string modconfigname, string propertyname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty(propertyname, BindingFlags.Public | BindingFlags.Static).GetValue(modInstance, null);
                return (int)res;
            }
            return -1;
        }

        /// <summary>
        /// Fetches a numeric property value for a Mod Configuration
        /// </summary>
        /// <param name="modconfigname">ModConfigClass e.g. SampleModConfig </param>
        /// <param name="propertyname">Name of the Property to retreive</param>
        /// <returns>The property value as a Double</returns>
        public static double GetNumericValue(string modconfigname, string propertyname)
        {
            if (!modconfigname.Contains(","))
                modconfigname += ASSEMBLY;
            Type modclass = Type.GetType(NAMESPACE + modconfigname, true);

            if (modclass != null)
            {
                object modInstance = Activator.CreateInstance(modclass);
                var res = modclass.GetProperty(propertyname, BindingFlags.Public | BindingFlags.Static).GetValue(modInstance, null);
                return Convert.ToDouble(res);
            }
            return -1D;
        }

        private static string GetFileNameWithoutExtensions(string path)
        {
            string result = path;
            while (result.Contains("."))
            {
                result = Path.GetFileNameWithoutExtension(result);
            }
            return result;
        }
    }
}
