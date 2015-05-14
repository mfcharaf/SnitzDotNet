﻿/*
####################################################################################################################
##
## SnitzUI.UserControls - adrotation.ascx
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
using SnitzConfig;

namespace SnitzUI.UserControls
{
    
    public partial class adrotation : System.Web.UI.UserControl
    {
        public string Filter { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        private const string Iframe =
            "<iframe id=\"iframe1\" onload='javascript:resizeIframe(this);'  frameborder=\"0\" style=\"background-color:#ffffff;\" scrolling=\"no\" src=\"/Content/AdRotator.aspx?f={0}\" name=\"iframe1\"></iframe>";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Config.ShowHeaderAds)
            {
                GoogleAd.Visible = Config.ShowGoogleAds && Config.GoogleAdCode != "";
                if (!Config.ShowGoogleAds)
                    Literal1.Text = String.Format(Iframe, Filter);
            }
            else
            {
                GoogleAd.Visible = false;
            }
        }
    }
}