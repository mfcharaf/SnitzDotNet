using System;
using SnitzUI.Admin;
using SnitzCommon;

public partial class AdminHome : PageBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        pageCSS.Attributes.Add("href", "css/" + Page.Theme + "/admin.css");
        menuCSS.Attributes.Add("href", "css/" + Page.Theme + "/menu.css");
        string action = "";

        if (Request.Params["action"] != null)
            action = Request.Params["action"];
        if (action != "")
        {
            Panel1.Visible = false;
            switch (action)
            {
                case "system":
                    var adminSys = (Admin_system)Page.LoadControl("system.ascx");
                    CP1.Controls.Add(adminSys);
                    break;
                case "features":
                    var adminFeatures = (Admin_features)Page.LoadControl("features.ascx");
                    CP1.Controls.Add(adminFeatures);
                    break;
                case "time":
                    var adminTime = (Admin_dateTime)Page.LoadControl("dateTime.ascx");
                    CP1.Controls.Add(adminTime);
                    break;
                case "email":
                    var adminEmail = (Admin_email)Page.LoadControl("email.ascx");
                    CP1.Controls.Add(adminEmail);
                    break;
                case "emoticons":
                    var adminEmoticon = (EmoticonAdmin)Page.LoadControl("emoticonadmin.ascx");
                    CP1.Controls.Add(adminEmoticon);
                    break;
                case "pollconfig":
                    var pollAdmin = (PollAdmin)Page.LoadControl("~/UserControls/Polls/polladmin.ascx");
                    CP1.Controls.Add(pollAdmin);
                    break;
                case "editpoll":
                    var editpoll = (EditPoll)Page.LoadControl("~/UserControls/Polls/polledit.ascx");
                    CP1.Controls.Add(editpoll);
                    break;
                case "pollresults":
                    var poll = (Poll)Page.LoadControl("~/UserControls/Polls/Poll.ascx");
                    poll.PollId = Convert.ToInt32(Request.Params["pid"]);
                    CP1.Controls.Add(poll);
                    break;
                case "managecategories":
                    Admin_ManageCategories AdminManageCategories = (Admin_ManageCategories)Page.LoadControl("ManageCategories.ascx");
                    CP1.Controls.Add(AdminManageCategories);
                    break;
                case "moderators":
                    Admin_ManageModerators AdminManageModerators = (Admin_ManageModerators)Page.LoadControl("ManageModerators.ascx");
                    CP1.Controls.Add(AdminManageModerators);
                    break;
                case "managesubscriptions":
                    var adminManageSubscriptions = (Admin_Subscriptions)Page.LoadControl("Subscriptions.ascx");
                    CP1.Controls.Add(adminManageSubscriptions);
                    break;
                case "adduser":
                    var newMember = (Admin_NewMember)Page.LoadControl("NewMember.ascx");
                    CP1.Controls.Add(newMember);
                    break;
                case "manageusers":
                    var manageUsers = (Admin_Membership)Page.LoadControl("Membership.ascx");
                    CP1.Controls.Add(manageUsers);

                    break;
                case "counts":
                    var adminUpdateCounts = (Admin_UpdateCounts)Page.LoadControl("UpdateCounts.ascx");
                    CP1.Controls.Add(adminUpdateCounts);
                    break;
                case "archive":
                    Admin_ArchiveForums adminArchiveForums = (Admin_ArchiveForums)Page.LoadControl("ArchiveForums.ascx");
                    CP1.Controls.Add(adminArchiveForums);
                    break;
                //case "unarchive":
                //    Admin_UnArchiveForums AdminUnArchiveForums = (Admin_UnArchiveForums)Page.LoadControl("UnArchiveForums.ascx");
                //    CP1.Controls.Add(AdminUnArchiveForums);
                //    break;
                case "badwords":
                    var adminFilterB = (Admin_filters)Page.LoadControl("filters.ascx");
                    adminFilterB.FilterType = "badwords";
                    CP1.Controls.Add(adminFilterB);
                    break;
                case "userfilter":
                    var adminFilterN = (Admin_filters)Page.LoadControl("filters.ascx");
                    adminFilterN.FilterType = "usernames";
                    CP1.Controls.Add(adminFilterN);
                    break;
                case "validate":
                    var adminValidate = (Admin_PendingMembers)Page.LoadControl("Pendingmembers.ascx");
                    CP1.Controls.Add(adminValidate);
                    break;
                case "avatar":
                    var adminAvatars = (Admin_ManageAvatars)Page.LoadControl("ManageAvatars.ascx");
                    CP1.Controls.Add(adminAvatars);
                    break;
                case "roles":
                    var adminManageRoles = (Admin_ManageRoles)Page.LoadControl("ManageRoles.ascx");
                    CP1.Controls.Add(adminManageRoles);
                    break;
                case "profile" :
                    var adminManageProfile = (Admin_ManageProfile)Page.LoadControl("ManageProfile.ascx");
                    CP1.Controls.Add(adminManageProfile);
                    break;
            }
        }
    }

}
