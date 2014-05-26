/*
####################################################################################################################
##
## ModConfig - newmod.config
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
using System.ComponentModel;
using System.Configuration;
using System.Text;
using System.Xml;
using Snitz.Entities;
using SnitzConfig;


namespace ModConfig
{
    /// <summary>
    /// Summary description for SampleMod
    /// </summary>
    public class SampleModConfig : ISnitzModConfig
    {
        #region ISnitzModConfig Implementation

        public string Name { get { return "Sample Mod"; }}

        public string Description { get { return "Sample Mod Description"; } }

        public Version Version { get { return new Version(1, 0); } }
        public bool ShowOnMenu { get { return true; } }

        public ModMenuItem Menu
        {
            get
            {
                StringBuilder menuXml = new StringBuilder();
                menuXml.AppendLine("<siteMapNode roles=\"Member\" title=\"Sample Menu\">");
                menuXml.AppendFormat("<siteMapNode url=\"{0}\" title=\"Sample Mod Settings\" description=\"Mod Administration page\" Roles=\"Administrator\"/>", @"~\modfolder\default.aspx").AppendLine();
                menuXml.AppendFormat("<siteMapNode url=\"{0}\" title=\"Sample Mod Page\" description=\"The mod page\" />", @"~\modfolder\modpage.aspx").AppendLine();
                menuXml.AppendLine("</siteMapNode>");
                return new ModMenuItem()
                {
                    //Parent = "Account",
                    MenuXml = menuXml.ToString()
                };
            }
        }


        [Description("boolSampleModEnabled")]
        public bool Enabled
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolSampleModEnabled"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolSampleModEnabled", value ? "1" : "0");
            }
        }

        #endregion
        
        #region Mod specific Properties

        [Description("strSampleModProperty")]
        public static string SampleModProperty
        {
            get
            {
                return ConfigurationManager.AppSettings["strSampleModProperty"];
            }
            set
            {
                Config.UpdateConfig("strSampleModProperty", value);
            }
        }

        #endregion
    } 
}
