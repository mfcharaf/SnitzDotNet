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
    public class ShareItConfig : ISnitzModConfig
    {
        #region ISnitzModConfig Implementation

        public string Name { get { return "Share It"; }}

        public string Description { get { return "Share topic on Social Media sites"; } }

        public Version Version { get { return new Version(1, 0); } }
        public bool ShowOnMenu { get { return false; } }

        public ModMenuItem Menu
        {
            get
            {
                return null;
             }
        }


        [Description("boolShareItEnabled")]
        public bool Enabled
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShareItEnabled"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolShareItEnabled", value ? "1" : "0");
            }
        }

        #endregion
        
        #region Mod specific Properties

        [Description("strShareItItems")]
        public static string MediaItems
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
