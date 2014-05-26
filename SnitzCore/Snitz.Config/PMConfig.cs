/*
####################################################################################################################
##
## SnitzConfig - PMConfig
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
using System.Configuration;

namespace SnitzConfig
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
