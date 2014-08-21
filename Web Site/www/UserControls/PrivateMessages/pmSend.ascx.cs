/*
####################################################################################################################
##
## PrivateMessaging.UserControls.PrivateMessaging - pmSend.ascx
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
using SnitzCommon;

namespace SnitzUI.UserControls.PrivateMessaging
{

    public partial class PmSend : TemplateUserControl
    {

        public int ToUser { get; set; }
        public string Layout { get { return _layout; } set { _layout = value; } }

        private string _layout;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                ToUser = Convert.ToInt32(Data);
            }
            this.StartupScript = "$('.PMMsg').markItUp(miniSettings)";

            //var pmcookies = SnitzCookie.GetPMCookie();

            //if (!pmcookies.ContainsKey("outbox"))
            //{
            //    pmcookies["outbox"] = "double";
            //    _layout = "double";
            //}
            //else
            //{
            //    _layout = pmcookies["outbox"];
            //}
        }

        protected void SendPm(object sender, EventArgs e)
        {

 
        }
    }

}
