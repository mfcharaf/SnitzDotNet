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
using System.Web;
using System.Web.Security;
using Snitz.BLL;
using Snitz.BLL.modconfig;
using Snitz.Entities;


namespace ModConfig
{

    /// <summary>
    /// Summary description for SampleMod
    /// </summary>
    public class ThanksConfig : ModConfigBase, ISnitzModConfig
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

        public bool NeedsSetup { get { return false; } }

        #endregion

        public ThanksConfig() : base("Thanks")
        {
            ModConfiguration.AdminControl = "ThanksMod.ascx";
        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>
                                                  {
                                                      {"Member Likes","1"},
                                                      {"Topic Likes","1"},
                                                      {"Reply Likes","1"}
                                                  };

            ModInfo modinfo = new ModInfo
            {
                Id = -1,
                Name = "Thanks",
                Description = "Thanks mod",
                Version = new Version(1, 0),
                Enabled = false,
                AdminControl = "ThanksMod.ascx",
                Settings = new Hashtable(settings)
            };

            controller.ModInfo = modinfo;
            controller.InstallMod();
            return modinfo;
        }

        protected override bool SetupMod(ModController controller)
        {

            DbsFileProcessor dbsUpgrade = new DbsFileProcessor(HttpContext.Current.Server.MapPath("thanksSetup.xml"));
            if (!dbsUpgrade.Applied)
            {
                var restext = dbsUpgrade.Process();
                if (restext == "success")
                    return true;
            }
            return false;
        }
    }

}