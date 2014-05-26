/*
####################################################################################################################
##
## SnitzUI.UserControls - JumpTo.ascx
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
using System.Web;
using System.Web.UI;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;


public partial class WebUserControl : UserControl
{
    protected override void OnInit(EventArgs e)
    {
        BindData();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
  
    }

    private void BindData()
    {
        GroupDropDownList1.Items.Clear();
        GroupDropDownList1.AppendDataBoundItems = true;
        GroupDropDownList1.DataSource = GetForumJumpTo();
        GroupDropDownList1.DataTextField = "Name";
        GroupDropDownList1.DataValueField = "Id";
        GroupDropDownList1.DataGroupField = "Category";
        GroupDropDownList1.DataBind();
    }

    private static List<ForumJumpto> GetForumJumpTo()
    {

        string[] rolelist = new SnitzRoleProvider().GetRolesForUser(HttpContext.Current.User.Identity.Name);
        List<ForumJumpto> result = new List<ForumJumpto>();

        foreach (ForumJumpto fo in SnitzCachedLists.GetCachedForumList(false))
        {
            fo.Roles = Forums.GetForumRoles(fo.Id).ToList();
            if (rolelist.Length == 0)
                if (fo.Roles.Count == 0)
                    result.Add(fo);
            foreach (string role in rolelist)
            {
                if(result.Contains(fo))
                    break;
                switch (role)
                {
                    case "Administrator":
                        result.Add(fo);
                        break;
                    default:
                        if (fo.Roles.Count == 0)
                            result.Add(fo);
                        else if (fo.Roles.Contains(role))
                            result.Add(fo);
                        break;
                }
            }
        }

        return result;

    }

}
