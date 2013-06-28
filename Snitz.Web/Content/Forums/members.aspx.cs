#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
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
#endregion

using System;
using System.Security;
using System.Web.UI.WebControls;
using Resources;
using SnitzCommon;
using SnitzConfig;
using SnitzData;


public partial class MembersPage : PageBase
{
    private GridPager _memberPager;
    private int RowCount
    {
        get
        {
            return (int)ViewState["RowCount"];
        }
        set
        {
            ViewState["RowCount"] = value;
        }
    }
    public delegate void PopulateObject(int myInt);

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!IsAuthenticated)
        {
            throw new SecurityException("You must be a logged in member to view users profiles");
        }

        Page.Title = SiteMapLocalizations.MembersPageTitle;
        ucSearch.InitialLinkClick += FindInitial;
        if(Session["CurrentProfile"] != null)
            Session.Remove("CurrentProfile");

    }

    protected void Page_Load()
    {
        BindMembers();

    }

    private void FindInitial(object sender, EventArgs e)
    {
        var letter = (LinkButton) sender;
        Session["SearchFilter"] = String.Format("Name.StartsWith(\"{0}\")", letter.CommandArgument);
        CurrentPage = 0;
        _memberPager.CurrentIndex = CurrentPage;
        MGV.PageIndex = CurrentPage;
        upd.Update();
    }

    private void BindMembers()
    {
        RowCount = PagedObjects.SelectMemberCount();
        _memberPager = (GridPager)LoadControl("~/UserControls/GridPager.ascx");
        _memberPager.PagerStyle = PagerType.Lnkbutton;
        _memberPager.UserControlLinkClick += PagerLinkClick;
        _memberPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
        PopulateObject populate = PopulateData;
        _memberPager.UpdateIndex = populate;
        pager.Controls.Add(_memberPager);
    }


    protected void MemberGridViewRowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            for (int index = 0; index < MGV.Columns.Count; index++)
            {
                DataControlField col = MGV.Columns[index];
                if (MGV.SortExpression != "")
                    if (col.SortExpression.Equals(MGV.SortExpression))
                    {
                        var image = new Image
                                          {
                                              SkinID =
                                                  MGV.SortDirection == SortDirection.Ascending ? "SortAsc" : "SortDesc"
                                          };
                        image.ApplyStyleSheetSkin(Page);
                        image.EnableViewState = false;
                        e.Row.Cells[index].Controls.Add(image);
                    }
            }
        }
    }
    protected void MgvRowDataBound(object sender, GridViewRowEventArgs e)
    {

        //if (e.Row.RowType == DataControlRowType.Pager)
        //{
        //    _memberPager = (GridPager)e.Row.FindControl("pager");
        //    _memberPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
        //    _memberPager.CurrentIndex = CurrentPage;
        //    _memberPager.UserControlLinkClick += PagerLinkClick;
        //    PopulateObject populate = PopulateData;
        //    _memberPager.UpdateIndex = populate;
        //}
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var member = (Member)e.Row.DataItem;
            var hyp = (HyperLink)e.Row.FindControl("hypUserLock");
            if (hyp != null)
                hyp.Text = string.Format(webResources.lblLockUser, member.Name);
            hyp = (HyperLink)e.Row.FindControl("hypUserUnLock");
            if (hyp != null)
                hyp.Text = string.Format(webResources.lblUnlockUser, member.Name);
            if ((!IsAdministrator))
            {
                e.Row.Cells.RemoveAt(8);
                e.Row.Cells.RemoveAt(7);
            }
        }
        if(e.Row.RowType == DataControlRowType.Header)
        {
            if ((!IsAdministrator))
            {
                e.Row.Cells.RemoveAt(8);
                e.Row.Cells.RemoveAt(7);
            }
        }

    }

    private void PagerLinkClick(object sender, EventArgs e)
    {
        var lnk = sender as LinkButton;

        if (lnk != null)
        {
            if (lnk.Text.IsNumeric())
                CurrentPage = int.Parse(lnk.Text) - 1;
            else
            {
                if (lnk.Text.Contains("&gt;"))
                    CurrentPage += 1;
                else if (lnk.Text.Contains("&lt;"))
                    CurrentPage -= 1;
                else if (lnk.Text.Contains("&raquo;"))
                    CurrentPage = _memberPager.PageCount - 1;
                else
                    CurrentPage = 0;
            }
            if (CurrentPage < 0)
                CurrentPage = 0;
            if (CurrentPage >= _memberPager.PageCount)
                CurrentPage = _memberPager.PageCount - 1;
        }
        _memberPager.CurrentIndex = CurrentPage;
    }

    private void PopulateData(int myint)
    {
        CurrentPage = myint;
        MGV.PageIndex = CurrentPage;
        //MemberODS.Select();
        upd.Update();
    }

    private bool _bGetSelectCount;
    protected void MemberObjectDataSourceSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        _bGetSelectCount = e.ExecutingSelectCount;
    }

    protected void MemberObjectDataSourceSelected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (_bGetSelectCount)
        {
            RowCount = (int)e.ReturnValue;
            if (CurrentPage != MGV.PageIndex)
                CurrentPage = MGV.PageIndex;
        }
    }

}
