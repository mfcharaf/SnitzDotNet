/*
####################################################################################################################
##
## SnitzUI.UserControls - ForumLogin.ascx
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
using System.Web;
using System.Web.UI.WebControls;
using Snitz.Entities;


namespace SnitzUI.UserControls
{
    public partial class ForumLogin : System.Web.UI.UserControl
    {
        public ForumInfo forum { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            login.UserName = HttpContext.Current.User.Identity.Name;
            Page.Form.DefaultButton = login.FindControl("LoginButton").UniqueID;
        }

        protected void OnAuthenticate(object sender, AuthenticateEventArgs e)
        {
            if (String.Compare(login.Password, forum.Password, false) == 0)
            {
                popup.Hide();
                Session.Add("FORUM" + forum.Id, "true");
                Response.Redirect(Request.RawUrl);
            }

        }

    }
}