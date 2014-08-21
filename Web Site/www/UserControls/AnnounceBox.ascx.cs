/*
####################################################################################################################
##
## SnitzUI.UserControls - AnnounceBox.ascx
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
using System.Web.UI.WebControls;
using Snitz.BLL;
using SnitzCommon;
using SnitzConfig;

namespace SnitzUI.UserControls
{
    
    public partial class AnnounceBox : System.Web.UI.UserControl
    {
        public string AnonMessage { get; set; }
        public string AuthMessage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthMessage = Config.AuthMessage;
            AnonMessage = Config.AnonMessage;
            Visible = Config.Announcement && (!String.IsNullOrEmpty(AuthMessage) || !String.IsNullOrEmpty(AnonMessage));
            if (Visible)
            {
                var auth = (Literal) LoginView1.FindControl("authText");
                var anon = (Literal) LoginView1.FindControl("anonText");
                if (auth != null && !String.IsNullOrEmpty(AuthMessage))
                    auth.Text = AuthMessage.StripCData().ParseTags();
                if (anon != null && !String.IsNullOrEmpty(AnonMessage))
                    anon.Text = AnonMessage.StripCData().ParseTags();
            }
        }
    }
}