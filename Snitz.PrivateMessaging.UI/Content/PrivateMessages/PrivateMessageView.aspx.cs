/*
####################################################################################################################
##
## PrivateMessaging.Content.PrivateMessages - PrivateMessageView.aspx
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
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SnitzCommon;

namespace PrivateMessaging
{
    public partial class PrivateMessageView : PageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!IsAuthenticated)
            {
                throw new SecurityException("You must be a logged in member to view private messages");
            }
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var pmview = (PmView)Page.LoadControl("~/UserControls/PrivateMessaging/pmview.ascx");

            ViewPm.Controls.Add(pmview);
        }
    }
}