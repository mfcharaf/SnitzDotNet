using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                DataSet forumList, unForumList;
                int memberID;

                ModeratorList.DataSource = SnitzRoleProvider.ListRoleMembers("Moderator");
                ModeratorList.DataTextField = "UserName";
                ModeratorList.DataValueField = "UserId";

                ModeratorList.DataBind();


                if (ModeratorList.SelectedItem == null)
                    ModeratorList.SelectedIndex = 0;
                if (ModeratorList.Items.Count > 0) 
                    memberID = Convert.ToInt32(ModeratorList.SelectedItem.Value); 
                else
                {
                    SaveBtn.Enabled = false; 
                    memberID = 0;
                }

                unForumList = srp.ListUnModeratedForums(memberID);
                AvForumsList.DataSource = unForumList;
                AvForumsList.DataTextField = "F_SUBJECT";
                AvForumsList.DataValueField = "FORUM_ID";

                forumList  = srp.ListModeratedForums(memberID);
                MdForumsList.DataSource = forumList;
                MdForumsList.DataTextField = "F_SUBJECT";
                MdForumsList.DataValueField = "FORUM_ID";
            }
            else  // Forums View
            {

                int forumID;

                //DataTable fList = ForumDatasource.GetForumsIDSubject();

                ForumsList.DataSource = SnitzData.Util.ListForums();
                ForumsList.DataTextField = "Title";
                ForumsList.DataValueField = "ForumId";

                ForumsList.DataBind();

                if (ForumsList.SelectedItem == null)
                    ForumsList.SelectedIndex = 0;

                forumID = Convert.ToInt32(ForumsList.SelectedItem.Value);

                DataTable avList = srp.GetAvailableModerators(forumID);
                AvModsList.DataSource = avList;
                AvModsList.DataTextField = "M_NAME";
                AvModsList.DataValueField = "MEMBER_ID";

                CurModsList.DataSource = SnitzData.Util.GetForum(forumID).Moderators;
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
            DataSet forumList, unForumList;
            int memberID;

            if (ModeratorList.Items.Count == 0) // if the list has never been filled 
            {

                ModeratorList.DataSource = SnitzRoleProvider.GetRoleMembers("Moderator");
                ModeratorList.DataTextField = "u.UserName";
                ModeratorList.DataValueField = "u.UserId";
                ModeratorList.DataBind();

                if (ModeratorList.SelectedItem == null)
                    ModeratorList.SelectedIndex = 0;
            }

            
            memberID = Convert.ToInt32(ModeratorList.SelectedItem.Value);

            unForumList = srp.GetUnModeratedForumsIdNameList(memberID);
            AvForumsList.DataSource = unForumList;
            AvForumsList.DataTextField = "F_SUBJECT";
            AvForumsList.DataValueField = "FORUM_ID";

            forumList = srp.GetModeratedForumsIdNameList(memberID);
            MdForumsList.DataSource = forumList;
            MdForumsList.DataTextField = "F_SUBJECT";
            MdForumsList.DataValueField = "FORUM_ID";

            
        }
        else  // Forums View
        {
            DataTable avList, mList;
            int forumID;

            if (ForumsList.Items.Count == 0)
            {

                ForumsList.DataSource = SnitzData.Util.ListForums();
                ForumsList.DataTextField = "Subject";
                ForumsList.DataValueField = "Id";

                ForumsList.DataBind();

                if (ForumsList.SelectedItem == null)
                    ForumsList.SelectedIndex = 0;
            }
           

            forumID = Convert.ToInt32(ForumsList.SelectedItem.Value);

            avList = srp.GetAvailableModeratorsIdName(forumID);
            AvModsList.DataSource = avList;
            AvModsList.DataTextField = "M_NAME";
            AvModsList.DataValueField = "MEMBER_ID";

            mList = srp.GetCurrentModeratorsIdName(forumID);
            CurModsList.DataSource = mList;
            CurModsList.DataTextField = "M_NAME";
            CurModsList.DataValueField = "MEMBER_ID";

        }

        Page.DataBind();


    }

    protected void ModeratorList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SnitzRoleProvider srp = new SnitzRoleProvider();
        DataSet forumList, unForumList;
        int memberID;


        memberID = Convert.ToInt32(ModeratorList.SelectedItem.Value);

        unForumList = srp.GetUnModeratedForumsIdNameList(memberID);
        AvForumsList.DataSource = unForumList;
        AvForumsList.DataTextField = "F_SUBJECT";
        AvForumsList.DataValueField = "FORUM_ID";

        forumList = srp.GetModeratedForumsIdNameList(memberID);
        MdForumsList.DataSource = forumList;
        MdForumsList.DataTextField = "F_SUBJECT";
        MdForumsList.DataValueField = "FORUM_ID";

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
            srp.SetUserAsModeratorForForums(memberID, forumList);
    }

    protected void ForumsList_SelectedIndexChanged(object sender, EventArgs e)
    {
        SnitzRoleProvider srp = new SnitzRoleProvider();
        DataTable avList, mList;
        int forumID;


        forumID = Convert.ToInt32(ForumsList.SelectedItem.Value);

        avList = srp.GetAvailableModeratorsIdName(forumID);
        AvModsList.DataSource = avList;
        AvModsList.DataTextField = "M_NAME";
        AvModsList.DataValueField = "MEMBER_ID";

        mList = srp.GetCurrentModeratorsIdName(forumID);
        CurModsList.DataSource = mList;
        CurModsList.DataTextField = "M_NAME";
        CurModsList.DataValueField = "MEMBER_ID";

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


            SnitzRoleProvider srp = new SnitzRoleProvider();
            srp.SetForumModerators(forumID, userList);
        //}

    }
}
