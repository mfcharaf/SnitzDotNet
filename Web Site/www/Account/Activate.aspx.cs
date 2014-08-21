/*
####################################################################################################################
##
## SnitzUI.Account - Activate.aspx
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
using System.Web.Security;
using System.Web.UI.WebControls;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;

namespace SnitzUI
{
    public partial class Activate : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.Params["C"] != "")
                    ActivationCode.Text = Request.Params["C"];
            }
        }

        protected void ActivationCodeValidatorServerValidate(object source, ServerValidateEventArgs args)
        {
            MembershipUser mu = Membership.GetUser(username.Text, false);
            if (mu != null) args.IsValid = mu.PasswordQuestion == ActivationCode.Text;
        }

        protected void BtnActivateClick(object sender, EventArgs e)
        {
            MembershipUser mu = Membership.GetUser(username.Text, false);
            Page.Validate();
            if (Page.IsValid)
            {

                ActivationPanel.Visible = false;
                ActivatedPanel.Visible = true;
                if (string.IsNullOrEmpty(Request.Params["E"]))
                {
                    //we are already approved so lets quit
                    if(mu != null && mu.IsApproved)
                    {
                        litAccepted.Visible = false;
                        litValidated.Visible = false;
                        litAlreadyValid.Visible = true;
                        return;
                    }

                    if (Config.RestrictRegistration)
                    {
                        litAccepted.Visible = false;
                        litValidated.Visible = true;
                        litAlreadyValid.Visible = false;

                        if (mu != null) mu.UnlockUser();
                    }
                    else
                    {
                        litAccepted.Visible = true;
                        litValidated.Visible = false;
                        litAlreadyValid.Visible = false;
                        if (mu != null) mu.UnlockUser();
                        if (SnitzMembershipProvider.ActivateUser(username.Text)) //we need to activate in order to validate login
                        {
                            //we passed so lets unlock and add to member role
                            
                            Roles.AddUserToRole(username.Text, "Member");
                        }

                    }
                }
                else if (Request.Params["E"] == "T")
                {
                    if (SnitzMembershipProvider.ChangeEmail(mu, true, null))
                        litValidated.Visible = true;
                    else
                    {
                        throw new Exception("Email change failed");
                    }
                }
            }
        }
    }
}