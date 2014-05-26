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
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using Snitz.Entities;
using SnitzConfig;


namespace ModConfig
{
    /// <summary>
    /// File upload configuration
    /// </summary>
    /// 
    public class UploadConfig : ISnitzModConfig
    {
        public string Name
        {
            get { return "File Upload"; }
        }

        public string Description
        {
            get { return "File Upload Configuration"; }
        }

        public Version Version { get { return new Version(1, 0); } }
        public bool ShowOnMenu { get { return false; } }
        public ModMenuItem Menu { get { return null; } }

        [Description("boolEnableUploads")]
        public bool Enabled
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolEnableUploads"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolEnableUploads", value ? "1" : "0");
            }
        }


        [Description("boolAllowFileUpload")]
        public static bool AllowFileUpload
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolAllowFileUpload"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolAllowFileUpload", value ? "1" : "0");
            }
        }
        [Description("boolShowFileAttach")]
        public static bool ShowFileAttach
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowFileAttach"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolShowFileAttach", value ? "1" : "0");
            }
        }
        [Description("strFileUploadLocation")]
        public static string FileUploadLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["strFileUploadLocation"];
            }

            set
            {
                Config.UpdateConfig("strFileUploadLocation", value);
            }
        }
        [Description("strAllowedFileTypes")]
        public static string AllowedFileTypes
        {
            get
            {
                return ConfigurationManager.AppSettings["strAllowedFileTypes"];
            }

            set
            {
                Config.UpdateConfig("strAllowedFileTypes", value);
            }
        }
        [Description("intTotalUploadLimitFileSize")]
        public static int TotalUploadLimitFileSize
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["intTotalUploadLimitFileSize"]);
            }
            set
            {
                Config.UpdateConfig("intTotalUploadLimitFileSize", value.ToString());
            }
        }
        [Description("intTotalUploadLimitFileNumber")]
        public static int TotalUploadLimitFileNumber
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["intTotalUploadLimitFileNumber"]);
            }
            set
            {
                Config.UpdateConfig("intTotalUploadLimitFileNumber", value.ToString());
            }
        }
        [Description("intUploadLimitFileSize")]
        public static int FileSizeLimit
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["intUploadLimitFileSize"]);
            }
            set
            {
                Config.UpdateConfig("intUploadLimitFileSize", value.ToString());
            }
        }

    }

}