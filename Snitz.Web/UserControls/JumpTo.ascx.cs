/*'
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Snitz.Providers;
using SnitzData;

public partial class WebUserControl : UserControl
{
    protected override void OnInit(EventArgs e)
    {
        BindData();
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            Session.Remove("LastJump");
        }
            
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
        
        if (Session["LastJump"] != null)
        {
            GroupDropDownList1.SelectedIndex = (int)Session["LastJump"];
        }
    }

    private static List<ForumJumpto> GetForumJumpTo()
    {

        string[] rolelist = new SnitzRoleProvider().GetRolesForUser(HttpContext.Current.User.Identity.Name);
        List<ForumJumpto> result = new List<ForumJumpto>();

        foreach (ForumJumpto fo in SnitzCachedLists.GetForumListItems())
        {
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

    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (GroupDropDownList1.SelectedIndex == 0)
            return;
        if ((Session["LastJump"] == null) || (Session["LastJump"].ToString() != GroupDropDownList1.SelectedIndex.ToString()))
        {
            Page.MaintainScrollPositionOnPostBack = false;
            Session.Add("LastJump", GroupDropDownList1.SelectedIndex);
            //int catid = forum[DropDownList1.SelectedIndex-1].CatId;
            Page.Response.Redirect(string.Format("/Content/Forums/forum.aspx?FORUM={0}", GroupDropDownList1.SelectedValue));
        }
        else
        {
            Session.Remove("LastJump");
        }
    }
}
