using System;
using System.Configuration;
using SnitzConfig;


namespace ModConfig
{
    /// <summary>
    /// Summary description for upload_config
    /// </summary>
    /// 
public  class UploadConfig : SnitzMod
    {
        public string Name
        {
            get { return "UploadConfig"; }
        }

        public string Description
        {
            get { return "Upload Config"; }
        }

        // Guy Change start 28 Aug 2008
        

        public static bool showFileUpload
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowFileUpload"] == "1");
            }
            set
            {
                Config.UpdateConfig("boolShowFileUpload", value ? "1" : "0");
            }
        }
        public static bool showFileAttach
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
        public static string fileUploadLocation
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
        public static string allowedFileTypes
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
        public static int totalUploadLimitFileSize
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
        public static int totalUploadLimitFileNumber
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
        // Guy Change end 28 Aug 2008
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