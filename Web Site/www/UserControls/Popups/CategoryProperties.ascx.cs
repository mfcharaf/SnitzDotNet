/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - CategoryProperties.ascx
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
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI.UserControls.Popups
{
    public partial class CategoryProperties : TemplateUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(Data != null)
            {
                CategoryInfo cat = Categories.GetCategory(Convert.ToInt32(Data));
                SetupForm(cat);
            }
            else
            {
                var cat = new CategoryInfo();
                SetupForm(cat);
            }
        }

        private void SetupForm(CategoryInfo cat)
        {
            hdnCatId.Value = cat.Id.ToString();

            tbxSubject.Text = cat.Name;
            tbxOrder.Text = cat.Order.ToString();
            if (cat.ModerationLevel.HasValue) ddlMod.SelectedValue = cat.ModerationLevel.Value.ToString();
            #region Subscriptions
            if (Config.SubscriptionLevel > 0)
            {
                ddlSub.Items.Clear();
                ddlSub.Items.Add(new ListItem("No Subscriptions Allowed", "0"));
                if (Config.SubscriptionLevel < Enumerators.SubscriptionLevel.Forum)
                {
                    ddlSub.Items.Add(new ListItem("Category Subscriptions Allowed", "1"));
                }
                if (cat.SubscriptionLevel < (int)Enumerators.SubscriptionLevel.Topic)
                {
                    ddlSub.Items.Add(new ListItem("Forum Subscriptions Allowed", "2"));
                }
                ddlSub.Items.Add(new ListItem("Topic Subscriptions Allowed", "3"));
                ddlSub.SelectedValue = cat.SubscriptionLevel.ToString();

            }
            else
            {
                lblSub.Enabled = false;
                ddlSub.Enabled = false;
                ddlSub.Items.Clear();
                ddlSub.Items.Add(new ListItem("No Subscriptions Allowed", "0"));
                ddlSub.SelectedValue = "0";
            }
            #endregion

        }
    }
}