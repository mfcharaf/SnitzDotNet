/*
####################################################################################################################
##
## SnitzUI.Content.Forums - members.aspx
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
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;



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
            throw new NotSupportedException("You must be a logged in member to view forum users");
        }

        Page.Title = SiteMapLocalizations.MembersPageTitle;
        ucSearch.InitialLinkClick += FindInitial;
        ucSearch.SearchClick += SearchMember;
        if(Session["CurrentProfile"] != null)
            Session.Remove("CurrentProfile");
        if (!IsPostBack)
        {
            CurrentPage = 0;
            if (Session["SearchFilter"] != null)
                Session.Remove("SearchFilter"); 
        }
        BindMembers();
    }
    protected override void Page_PreRender(object sender, EventArgs e)
    {
        base.Page_PreRender(sender, e);

    }
    private void SearchMember(object sender, EventArgs e)
    {
        CurrentPage = 0;
        _memberPager.CurrentIndex = CurrentPage;
        MGV.PageIndex = CurrentPage;
        upd.Update();
    }

    protected void Page_Load()
    {
        if (Page.IsPostBack)
        {
            string postbackbtn = Request.Form["__EVENTTARGET"];
            string argument = Request.Form["__EVENTARGUMENT"];
            switch (postbackbtn)
            {
                case "LockMember":
                    LockUser(argument);
                    MGV.DataBind();
                    break;
                case "UnLockMember":
                    UnLockUser(argument);
                    MGV.DataBind();
                    break;
                case "DeleteMember":
                    DeleteUser(argument);
                    MGV.DataBind();
                    break;
            }
            
        }


    }

    private void DeleteUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.DeleteUser(user, false);
    }

    private void UnLockUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.UnlockUser(user);
    }

    private void LockUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.LockUser(user);
    }

    private void FindInitial(object sender, EventArgs e)
    {
        var letter = (LinkButton) sender;
        Session["SearchFilter"] = String.Format("Initial={0}", letter.CommandArgument);
        RowCount = Members.GetMemberCount();
        CurrentPage = 0;
        var p = (GridPager) pager.FindControl("memPager");
        if (p != null)
        {
            p.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
            p.CurrentIndex = CurrentPage;
        }

        MGV.PageIndex = CurrentPage;
        MGV.DataBind();
        upd.Update();

    }

    private void BindMembers()
    {
        pager.Controls.Clear();
        RowCount = Members.GetMemberCount();
        _memberPager = (GridPager)LoadControl("~/UserControls/GridPager.ascx");
        _memberPager.ID = "memPager";
        _memberPager.PagerStyle = Enumerators.PagerType.Linkbutton;
        _memberPager.UserControlLinkClick += PagerLinkClick;
        _memberPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
        _memberPager.CurrentIndex = CurrentPage;
        MGV.PageSize = Config.MemberPageSize;
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

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var member = (MemberInfo)e.Row.DataItem;
            var rankTitle = (Label) e.Row.FindControl("RankTitle");
            var rankStars = (Literal) e.Row.FindControl("RankStars");
            var lckUser = (ImageButton)e.Row.FindControl("lockUser");
            var unlckUser = (ImageButton)e.Row.FindControl("unlockUser");
            var delUser = (ImageButton)e.Row.FindControl("delUser");
            RankInfo rInf;
            try
            {
                string title = "";
                rInf = new RankInfo(member.Username, ref title, member.PostCount, SnitzCachedLists.GetRankings());
                rankTitle.Text = title;
                rankStars.Text = rInf.GetStars();
            }
            catch (Exception)
            {
                
                throw;
            }


            if ((!IsAdministrator))
            {
                e.Row.Cells.RemoveAt(8);
                e.Row.Cells.RemoveAt(7);
            }
            if (lckUser != null)
            {
                lckUser.Visible = (IsAdministrator) && member.Status == 1;
                lckUser.ToolTip = String.Format(webResources.lblLockUser, member.Username);
                lckUser.OnClientClick =
                    "confirmPostBack('Do you want to lock the User?','LockMember','" + member.Username + "');return false;";
            }
            if (unlckUser != null)
            {
                unlckUser.Visible = (IsAdministrator) && member.Status == 0;
                unlckUser.ToolTip = String.Format(webResources.lblUnlockUser, member.Username);
                unlckUser.OnClientClick =
                    "confirmPostBack('Do you want to unlock the User?','UnLockMember','" + member.Username + "');return false;";
            }

            if (delUser != null)
            {
                delUser.Visible = (IsAdministrator);
                delUser.ToolTip = String.Format(webResources.lblDeleteUser, member.Username);
                delUser.OnClientClick =
                    "confirmPostBack('Do you want to delete the User?','DeleteMember','" + member.Username + "');return false;";
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
        //RowCount = Members.GetMemberCount();
        //_memberPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.MemberPageSize);
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
