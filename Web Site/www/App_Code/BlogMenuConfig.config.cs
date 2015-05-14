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
    public class BlogMenuConfig : ModConfigBase,ISnitzModConfig
    {
        #region ISnitzModConfig Implementation

        public bool ShowOnMenu { get { return true; } }       

        public ModMenuItem Menu
        {
            get
            {
                string menuNode = String.Format("<siteMapNode url=\"{0}\" title=\"My Blog\" description=\"Blog\"/>", @"~\MyWebLog");
                return new ModMenuItem()
                {
                    Parent = "Account",
                    InsertAfter = "Profile",
                    MenuXml = menuNode
                };
            }
        }

        public bool NeedsSetup { get { return false; } }

        #endregion

        public BlogMenuConfig() : base("BlogMenu")
        {
            //Mod may be enabled, but only show it when required
            Enabled = Enabled && IsViewable();

            //initialise mod specific properties

        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {
            //todo: load mod config data from xml file etc 
           
            ModInfo modinfo = new ModInfo
                              {
                                  Id = -1,
                                  Name = "BlogMenu",
                                  Description = "Blog Menu Config",
                                  Version = new Version(1, 0),
                                  Enabled = true,
                                  Settings = null
                              };

            controller.ModInfo = modinfo;
            controller.InstallMod();
            return modinfo;

        }

        protected override bool SetupMod(ModController controller)
        {
            throw new NotImplementedException();
        }

        private bool IsViewable()
        {
            var m = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            if (m == null || m.ProviderUserKey == null)
                return false;
            if (Forums.GetUserBlogTopics(-1, (int)m.ProviderUserKey).Count > 0)
                return true;
            return false;
        }
    }
}
