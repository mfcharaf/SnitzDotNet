/*
####################################################################################################################
##
## ModConfig - upload.config
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
    /// File upload configuration
    /// </summary>
    /// 
    public class UploadConfig : ModConfigBase, ISnitzModConfig
    {

        public bool ShowOnMenu { get { return false; } }
        public ModMenuItem Menu { get { return null; } }

        public static bool AllowImageUpload { get; set; }
        public static bool AllowAttachments { get; set; }
        public static string FileUploadLocation { get; set; }
        public static string AllowedAttachmentTypes { get; set; }
        public static string AllowedImageTypes { get; set; }
        public static int TotalUploadLimitFileSize { get; set; }
        public static int TotalUploadLimitFileNumber { get; set; }
        public static int FileSizeLimit { get; set; }

        public UploadConfig() : base("FileUpload")
        {
            ModConfiguration.AdminControl = "FileUpload.ascx";

            //initialise mod specific properties
            AllowImageUpload = ModConfiguration.Settings["AllowImageUpload"].ToString() == "1";
            AllowAttachments = ModConfiguration.Settings["AllowAttachments"].ToString() == "1";
            FileUploadLocation = ModConfiguration.Settings["FileUploadLocation"].ToString();
            AllowedAttachmentTypes = ModConfiguration.Settings["AllowedAttachmentTypes"].ToString();
            AllowedImageTypes = ModConfiguration.Settings["AllowedImageTypes"].ToString();
            TotalUploadLimitFileSize = Convert.ToInt32(ModConfiguration.Settings["TotalUploadLimitFileSize"]);
            TotalUploadLimitFileNumber = Convert.ToInt32(ModConfiguration.Settings["TotalUploadLimitFileNumber"]);
            FileSizeLimit = Convert.ToInt32(ModConfiguration.Settings["FileSizeLimit"]);
        }

        protected override ModInfo LoadDefaultConfig(ModController controller)
        {

            Dictionary<string,string> settings = new Dictionary<string, string>
                                                 {
                                                     {"AllowFileUpload", "1"},
                                                     {"ShowFileAttach", "1"},
                                                     {
                                                         "FileUploadLocation", "/sharedFiles"
                                                     },
                                                     {
                                                         "AllowedAttachmentTypes",
                                                         "zip,pdf,txt,doc"
                                                     },
                                                     {
                                                         "AllowedImageTypes",
                                                         "jpg,jpeg,gif,png"
                                                     },
                                                     {
                                                         "TotalUploadLimitFileSize",
                                                         "1073741824"
                                                     },
                                                     {"TotalUploadLimitFileNumber", "250"},
                                                     {"FileSizeLimit", "2097152"}
                                                 };

            ModInfo modinfo = new ModInfo
            {
                Id = -1,
                Name = "FileUpload",
                Description = "File Upload Configuration",
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