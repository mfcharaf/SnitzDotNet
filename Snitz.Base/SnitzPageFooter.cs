/*
####################################################################################################################
##
## SnitzBase - SnitzPageFooter
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


using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzBase
{
    public partial class SnitzPageFooter : UserControl
    {
        protected HyperLink LnkForumFooter;

        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();
            LnkForumFooter = this.FindControl("lnkForumFooter") as HyperLink;

            if (LnkForumFooter != null)
            {
                LnkForumFooter.ToolTip = @"Powered By: Snitz Forums.Net v2.0";
                LnkForumFooter.Text = @"Snitz Forums.Net";
                LnkForumFooter.NavigateUrl = "http://forum.snitz.com";
                if (!Config.ShowPoweredByImage) LnkForumFooter.ImageUrl = "";
            }
        }
    }
}
