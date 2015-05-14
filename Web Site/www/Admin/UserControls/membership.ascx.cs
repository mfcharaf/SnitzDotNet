using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;

public partial class Admin_Membership : UserControl
{
    int pageSize = 25;
    int totalUsers;
    int totalPages;
    int currentPage = 1;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            string postbackbtn = Request.Form["__EVENTTARGET"];
            string argument = Request.Form["__EVENTARGUMENT"];
            switch (postbackbtn)
            {
                case "LockMember":
                    LockUser(argument);
                    break;
                case "UnLockMember":
                    UnLockUser(argument);
                    break;
                case "DeleteMember":
                    DeleteUser(argument);
                    break;
            }
            
        } 

        if (!IsPostBack)
        {
            Session["SearchFilter"] = "";
            GetUsers();
            ucSearch.InitialLinkClick += FindInitial;
            ucSearch.SearchClick += SearchMember;
        }

    }

    private void SearchMember(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void FindInitial(object sender, EventArgs e)
    {
        var letter = (LinkButton)sender;
        Session["SearchFilter"] = String.Format("Initial={0}", letter.CommandArgument);

    }

    private void GetUsers()
    {
        UsersOnlineLabel.Text = Membership.GetNumberOfUsersOnline().ToString();
        var members = Membership.GetAllUsers(currentPage - 1, pageSize, out totalUsers);
        if (!String.IsNullOrEmpty(Session["SearchFilter"].ToString()))
        {
            var filter = Session["SearchFilter"].ToString();
            var parms = filter.Split('=');
            if (parms[0] == "Initial")
            {
                members = Membership.FindUsersByName(parms[1], currentPage - 1, pageSize, out totalUsers);
            }
            else
            {

            }
        }
        UserGrid.DataSource = members;
        totalPages = ((totalUsers - 1) / pageSize) + 1;

        // Ensure that we do not navigate past the last page of users.

        if (currentPage > totalPages)
        {
            currentPage = totalPages;
            GetUsers();
            return;
        }

        UserGrid.DataBind();
        CurrentPageLabel.Text = currentPage.ToString();
        TotalPagesLabel.Text = totalPages.ToString();

        if (currentPage == totalPages)
            NextButton.Visible = false;
        else
            NextButton.Visible = true;

        if (currentPage == 1)
            PreviousButton.Visible = false;
        else
            PreviousButton.Visible = true;

        if (totalUsers <= 0)
            NavigationPanel.Visible = false;
        else
            NavigationPanel.Visible = true;
    }

    private void DeleteUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.DeleteUser(user, false);
        GetUsers();
    }

    private void UnLockUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.UnlockUser(user);
        GetUsers();
    }

    private void LockUser(string user)
    {
        SnitzMembershipProvider smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
        if (!String.IsNullOrEmpty(user))
            if (smp != null) smp.LockUser(user);
        GetUsers();
    }

    public void NextButton_OnClick(object sender, EventArgs args)
    {
        currentPage = Convert.ToInt32(CurrentPageLabel.Text);
        currentPage++;
        GetUsers();
    }

    public void PreviousButton_OnClick(object sender, EventArgs args)
    {
        currentPage = Convert.ToInt32(CurrentPageLabel.Text);
        currentPage--;
        GetUsers();
    }

    protected void BindGrid(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var member = (SnitzMembershipUser)e.Row.DataItem;
            var rankTitle = (Label)e.Row.FindControl("RankTitle");
            var rankStars = (Literal)e.Row.FindControl("RankStars");
            var lckUser = (ImageButton)e.Row.FindControl("lockUser");
            var unlckUser = (ImageButton)e.Row.FindControl("unlockUser");
            var delUser = (ImageButton)e.Row.FindControl("delUser");

            string title = "";
            RankInfo rInf = new RankInfo(member.UserName, ref title, member.Posts, SnitzCachedLists.GetRankings());
            rankTitle.Text = title;
            rankStars.Text = rInf.GetStars();


            if (lckUser != null)
            {
                lckUser.Visible = !member.IsLockedOut;
                lckUser.ToolTip = String.Format(webResources.lblLockUser, member.UserName);
                lckUser.OnClientClick =
                    "confirmPostBack('Do you want to lock the User?','LockMember','" + member.UserName +
                    "');return false;";
                if (unlckUser != null)
                {
                    unlckUser.Visible = member.IsLockedOut;
                    unlckUser.ToolTip = String.Format(webResources.lblUnlockUser, member.UserName);
                    unlckUser.OnClientClick =
                        "confirmPostBack('Do you want to unlock the User?','UnLockMember','" + member.UserName +
                        "');return false;";
                }

                if (delUser != null)
                {
                    delUser.Visible = true;
                    delUser.ToolTip = String.Format(webResources.lblDeleteUser, member.UserName);
                    delUser.OnClientClick =
                        "confirmPostBack('Do you want to delete the User?','DeleteMember','" + member.UserName +
                        "');return false;";
                }
            }
        }
    }
}
