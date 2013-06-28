using System;
using System.Collections.Generic;
using System.Configuration;


namespace PMConfig
{
    public class PMConfig : SnitzConfig.Config
    {
        // IsNumeric Function
        public static int PMLimit
        {
            get { return ConfigurationManager.AppSettings["intPMLimit"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intPMLimit"]); }
            set { UpdateConfig("intPMLimit", value.ToString()); }
        }

        public static bool PMStatus
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolPMStatus"] == "1");
            }
            set
            {
                UpdateConfig("boolPMStatus", value ? "1" : "0");
            }
        }



    }
}