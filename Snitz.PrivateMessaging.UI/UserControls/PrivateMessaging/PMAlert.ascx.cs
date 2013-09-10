/*
####################################################################################################################
##
## PrivateMessaging.UserControls.PrivateMessaging - PMAlert.ascx
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
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Snitz.BLL;

namespace PrivateMessaging
{
    public partial class PmAlert : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser(HttpContext.Current.User.Identity.Name);
            if (currentUser != null)
            {
                if (PrivateMessages.GetUnreadPMCount((int) currentUser.ProviderUserKey) > 0)
                {
                    var alert = new StringBuilder();
                    alert.Append("<div style=\"z-index:10000;float:left;\">");
                    alert.Append("<a href='/PrivateMessages' title=''>");
                    alert.AppendFormat("<img src='/App_Themes/{0}/images/icon_pmgotmail.gif' />",Page.Theme);
                    alert.Append("</a>");
                    alert.Append("</div>");
                    pmAlert.Text = alert.ToString();
                }
            }
        }
    }

}
