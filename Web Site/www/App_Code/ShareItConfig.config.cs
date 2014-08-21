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
using System.Collections;
using System.Collections.Generic;
using Snitz.BLL.modconfig;
using Snitz.Entities;


namespace ModConfig
{
    /// <summary>
    /// Summary description for SampleMod
    /// </summary>
    public class ShareItConfig : ModConfigBase, ISnitzModConfig
    {
        #region ISnitzModConfig Implementation

        public bool ShowOnMenu { get { return false; } }

        public ModMenuItem Menu
        {
            get
            {
                return null;
             }
        }

        #endregion
        
        #region Mod specific Properties
        /// <summary>
        /// comma seperated list of items to hide
        /// Available items: messenger,facebook,delicious,stumbleupon,googlebuzz,digg,twitter,reddit,linkedin
        /// </summary>
        public static string MediaItems { get; set; }

        #endregion

        public ShareItConfig() : base("ShareIt")
        {
            ModConfiguration.AdminControl = "ShareIt.ascx";

            MediaItems = ModConfiguration.Settings["MediaItems"].ToString();
        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>
                                                  {
                                                      {
                                                          "MediaItems",
                                                          "messenger,googlebuzz"
                                                      }
                                                  };

            ModInfo modinfo = new ModInfo
            {
                Id = -1,
                Name = "ShareIt",
                Description = "Share topic on Social Media sites",
                Version = new Version(1, 0),
                Enabled = true,
                Settings = new Hashtable(settings)
            };

            controller.ModInfo = modinfo;
            controller.InstallMod();
            return modinfo;
        }

    } 
}
