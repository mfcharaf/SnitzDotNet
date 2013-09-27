using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;


public partial class Admin_ManageModerators : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            SnitzRoleProvider srp = new SnitzRoleProvider();
            int avIndex = Convert.ToInt32(DropDownList1.SelectedValue);
            MultiView1.ActiveViewIndex = Convert.ToInt32(DropDownList1.SelectedValue);

            if (avIndex == 0) //Moderator View
            {
                int memberId;

                ModeratorList.DataSource = Moderators.GetAll();
                ModeratorList.DataTextField = "Name";
                ModeratorList.DataValueField = "MemberId";

                ModeratorList.DataBind();


                if (ModeratorList.SelectedItem == null)
                    ModeratorList.SelectedIndex = 0;
                if (ModeratorList.Items.Count > 0)
                    memberId = Convert.ToInt32(ModeratorList.SelectedValue); 
                else
                {
                    SaveBtn.Enabled = false; 
                    memberId = 0;
                }

                Dictionary<int, string> unForumList = Moderators.GetUnModeratedForumsIdNameList(memberId);
                AvForumsList.DataSource = unForumList;
                AvForumsList.DataTextField = "Value";
                AvForumsList.DataValueField = "Key";

                Dictionary<int, string> forumList = Moderators.GetModeratedForumsIdNameList(memberId);
                MdForumsList.DataSource = forumList;
                MdForumsList.DataTextField = "Value";
                MdForumsList.DataValueField = "Key";
            }
            else  // Forums View
            {
                //DataTable fList = ForumDatasource.GetForumsIDSubject();

                ForumsList.DataSource = Forums.GetAllForums();
                ForumsList.DataTextField = "Title";
                ForumsList.DataValueField = "ForumId";

                ForumsList.DataBind();

                if (ForumsList.SelectedItem == null)
                    ForumsList.SelectedIndex = 0;

                int forumId = Convert.ToInt32(ForumsList.SelectedItem.Value);

                List<MemberInfo> avList = Moderators.GetAvailableModerators(forumId);
                AvModsList.DataSource = avList;
                AvModsList.DataTextField = "Username";
                AvModsList.DataValueField = "Id";

                CurModsList.DataSource = Forums.GetForumModerators(forumId);
                CurModsList.DataTextField = "Value";
                CurModsList.DataValueField = "Key";
                
            }

            Page.DataBind();
        }

    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        int avIndex = Convert.ToInt32(DropDownList1.SelectedValue);
        
        MultiView1.ActiveViewIndex = avIndex;

        SnitzRoleProvider srp = new SnitzRoleProvider();

        if (avIndex == 0) //Moderator View
        {
            if (ModeratorList.Items.Count == 0) // if the list has never been filled 
            {

                ModeratorList.DataSource = srp.GetUsersInRole("Moderator");
                ModeratorList.DataTextField = "u.UserName";
                ModeratorList.DataValueField = "u.UserId";
                ModeratorList.DataBind();

                if (ModeratorList.SelectedItem == null)
                    ModeratorList.SelectedIndex = 0;
            }

            
            int memberId = Convert.ToInt32(ModeratorList.SelectedItem.Value);

            Dictionary<int, string> unForumList = Moderators.GetUnModeratedForumsIdNameList(memberId);
            AvForumsList.DataSource = unForumList;
            AvForumsList.DataTextField = "Value";
            AvForumsList.DataValueField = "Key";

            Dictionary<int, string> forumList = Moderators.GetModeratedForumsIdNameList(memberId);
            MdForumsList.DataSource = forumList;
            MdForumsList.DataTextField = "Value";
            MdForumsList.DataValueField = "Key";

            
        }
        else  // Forums View
        {
            if (ForumsList.Items.Count == 0)
            {

                ForumsList.DataSource = Forums.GetAllForums();
                ForumsList.DataTextField = "Subject";
                ForumsList.DataValueField = "Id";

                ForumsList.DataBind();

                if (ForumsList.SelectedItem == null)
                    ForumsList.SelectedIndex = 0;
            }
           

            int forumId = Convert.ToInt32(ForumsList.SelectedItem.Value);

            Dictionary<int, string> avList = Moderators.GetAvailableModeratorsIdName(forumId);
            AvModsList.DataSource = avList;
            AvModsList.DataTextField = "Value";
            AvModsList.DataValueField = "Key";

            Dictionary<int, string> mList = Moderators.GetCurrentModeratorsIdName(forumId);
            CurModsList.DataSource = mList;
            CurModsList.DataTextField = "Value";
            CurModsList.DataValueField = "Key";

        }

        Page.DataBind();


    }

    protected void ModeratorList_SelectedIndexChanged(object sender, EventArgs e)
    {

        int memberId = Convert.ToInt32(ModeratorList.SelectedItem.Value);

        Dictionary<int, string> unForumList = Moderators.GetUnModeratedForumsIdNameList(memberId);
        AvForumsList.DataSource = unForumList;
        AvForumsList.DataTextField = "Value";
        AvForumsList.DataValueField = "Key";

        Dictionary<int, string> forumList = Moderators.GetModeratedForumsIdNameList(memberId);
        MdForumsList.DataSource = forumList;
        MdForumsList.DataTextField = "Value";
        MdForumsList.DataValueField = "Key";

        Page.DataBind();
        
    }
    protected void Ad1_Click(object sender, EventArgs e)
    {
        
        if (AvForumsList.SelectedItem != null)
        {
            ListItemCollection liC = new ListItemCollection();


            foreach (ListItem li in AvForumsList.Items)
                if (li.Selected)
                {
                    MdForumsList.Items.Add(li);
                    li.Selected = false;
                    liC.Add(li);
                }

            foreach (ListItem li in liC)
                   AvForumsList.Items.Remove(li);
                
                
        }
    }

    protected void Rem1_Click(object sender, EventArgs e)
    {

        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in MdForumsList.Items)
            if (li.Selected)
            {
                AvForumsList.Items.Add(li);
                li.Selected = false;
                liC.Add(li);
            }

        foreach (ListItem li in liC)
            MdForumsList.Items.Remove(li);
    }
    protected void AdAll_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in AvForumsList.Items)
            {
                MdForumsList.Items.Add(li);
                li.Selected = false;
                liC.Add(li);
            }

        foreach (ListItem li in liC)
            AvForumsList.Items.Remove(li);
            
    }
    protected void RemAll_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in MdForumsList.Items)
        {
            AvForumsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            MdForumsList.Items.Remove(li);
    }
    protected void CancelBtn_Click(object sender, EventArgs e)
    {
        Response.Redirect ("default.aspx?action=moderators");
    }

    protected void SaveBtn_Click(object sender, EventArgs e)
    {
            int memberID = Convert.ToInt32(ModeratorList.SelectedValue);
            int count = 0;
            int[] forumList = new int[MdForumsList.Items.Count];

            foreach (ListItem li in MdForumsList.Items)
                    forumList[count++] = Convert.ToInt32(li.Value);


            SnitzRoleProvider srp = new SnitzRoleProvider();
            Moderators.SetUserAsModeratorForForums(memberID, forumList);
    }

    protected void ForumsList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int forumID = Convert.ToInt32(ForumsList.SelectedItem.Value);

        var avList = Moderators.GetAvailableModeratorsIdName(forumID);
        AvModsList.DataSource = avList;
        AvModsList.DataTextField = "Value";
        AvModsList.DataValueField = "Key";

        var mList = Moderators.GetCurrentModeratorsIdName(forumID);
        CurModsList.DataSource = mList;
        CurModsList.DataTextField = "Value";
        CurModsList.DataValueField = "Key";

        Page.DataBind();
    }
    protected void CancelFBtn_Click(object sender, EventArgs e)
    {
        Response.Redirect("default.aspx?action=moderators");
    }
    protected void Ad2_Click(object sender, EventArgs e)
    {
        if (AvModsList.SelectedItem != null)
        {
            ListItemCollection liC = new ListItemCollection();


            foreach (ListItem li in AvModsList.Items)
                if (li.Selected)
                {
                    CurModsList.Items.Add(li);
                    li.Selected = false;
                    liC.Add(li);
                }

            foreach (ListItem li in liC)
                AvModsList.Items.Remove(li);


        }
    }
    protected void Ad2All_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in AvModsList.Items)
        {
            CurModsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            AvModsList.Items.Remove(li);
    }
    protected void Rem2_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in CurModsList.Items)
            if (li.Selected)
            {
                AvModsList.Items.Add(li);
                li.Selected = false;
                liC.Add(li);
            }

        foreach (ListItem li in liC)
            CurModsList.Items.Remove(li);
    }
    protected void Rem2All_Click(object sender, EventArgs e)
    {
        ListItemCollection liC = new ListItemCollection();


        foreach (ListItem li in CurModsList.Items)
        {
            AvModsList.Items.Add(li);
            li.Selected = false;
            liC.Add(li);
        }

        foreach (ListItem li in liC)
            CurModsList.Items.Remove(li);
    }
    protected void SaveFBtn_Click(object sender, EventArgs e)
    {
        //if (CurModsList.Items.Count != 0)
        //{
            int forumID = Convert.ToInt32(ForumsList.SelectedItem.Value);
            int count = 0;
            int[] userList = new int[CurModsList.Items.Count];

            foreach (ListItem li in CurModsList.Items)
                userList[count++] = Convert.ToInt32(li.Value);

            Moderators.SetForumModerators(forumID, userList);
        //}

    }
}
