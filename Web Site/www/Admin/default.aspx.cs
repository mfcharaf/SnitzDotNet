using System;
using SnitzUI.Admin;
using SnitzCommon;
using SnitzUI.Admin.UserControls;

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
            CP1.Controls.Clear();
            switch (action)
            {
                case "system":
                    var adminSys = (Admin_system)Page.LoadControl("UserControls/system.ascx");
                    CP1.Controls.Add(adminSys);
                    break;
                case "features":
                    var adminFeatures = (Admin_features)Page.LoadControl("UserControls/features.ascx");
                    CP1.Controls.Add(adminFeatures);
                    break;
                case "time":
                    var adminTime = (Admin_dateTime)Page.LoadControl("UserControls/dateTime.ascx");
                    CP1.Controls.Add(adminTime);
                    break;
                case "email":
                    var adminEmail = (Admin_email)Page.LoadControl("UserControls/email.ascx");
                    CP1.Controls.Add(adminEmail);
                    break;
                case "emoticons":
                    var adminEmoticon = (EmoticonAdmin)Page.LoadControl("UserControls/emoticonadmin.ascx");
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
                    Admin_ManageCategories AdminManageCategories = (Admin_ManageCategories)Page.LoadControl("UserControls/ManageCategories.ascx");
                    CP1.Controls.Add(AdminManageCategories);
                    break;
                case "moderators":
                    Admin_ManageModerators AdminManageModerators = (Admin_ManageModerators)Page.LoadControl("UserControls/ManageModerators.ascx");
                    CP1.Controls.Add(AdminManageModerators);
                    break;
                case "managesubscriptions":
                    var adminManageSubscriptions = (Admin_Subscriptions)Page.LoadControl("UserControls/Subscriptions.ascx");
                    CP1.Controls.Add(adminManageSubscriptions);
                    break;
                case "adduser":
                    var newMember = (Admin_NewMember)Page.LoadControl("UserControls/NewMember.ascx");
                    CP1.Controls.Add(newMember);
                    break;
                case "manageusers":
                    var manageUsers = (Admin_Membership)Page.LoadControl("UserControls/Membership.ascx");
                    CP1.Controls.Add(manageUsers);

                    break;
                case "registration":
                    var manageReg = (Admin_Registration)Page.LoadControl("UserControls/Registration.ascx");
                    CP1.Controls.Add(manageReg);

                    break;
                case "counts":
                    var adminUpdateCounts = (Admin_UpdateCounts)Page.LoadControl("UserControls/UpdateCounts.ascx");
                    CP1.Controls.Add(adminUpdateCounts);
                    break;
                case "archive":
                    Admin_ArchiveForums adminArchiveForums = (Admin_ArchiveForums)Page.LoadControl("UserControls/ArchiveForums.ascx");
                    CP1.Controls.Add(adminArchiveForums);
                    break;
                //case "unarchive":
                //    Admin_UnArchiveForums AdminUnArchiveForums = (Admin_UnArchiveForums)Page.LoadControl("UnArchiveForums.ascx");
                //    CP1.Controls.Add(AdminUnArchiveForums);
                //    break;
                case "badwords":
                    var adminFilterB = (Admin_filters)Page.LoadControl("UserControls/filters.ascx");
                    adminFilterB.FilterType = "badwords";
                    CP1.Controls.Add(adminFilterB);
                    break;
                case "userfilter":
                    var adminFilterN = (Admin_filters)Page.LoadControl("UserControls/filters.ascx");
                    adminFilterN.FilterType = "usernames";
                    CP1.Controls.Add(adminFilterN);
                    break;
                case "validate":
                    var adminValidate = (Admin_PendingMembers)Page.LoadControl("UserControls/Pendingmembers.ascx");
                    CP1.Controls.Add(adminValidate);
                    break;
                case "avatar":
                    var adminAvatars = (Admin_ManageAvatars)Page.LoadControl("UserControls/ManageAvatars.ascx");
                    CP1.Controls.Add(adminAvatars);
                    break;
                case "roles":
                    var adminManageRoles = (Admin_ManageRoles)Page.LoadControl("UserControls/ManageRoles.ascx");
                    CP1.Controls.Add(adminManageRoles);
                    break;
                case "profile" :
                    var adminManageProfile = (Admin_ManageProfile)Page.LoadControl("UserControls/ManageProfile.ascx");
                    CP1.Controls.Add(adminManageProfile);
                    break;
                case "database":
                    var dbsmanager = (Admin_DbsManager)Page.LoadControl("UserControls/DbsManager.ascx");
                    CP1.Controls.Add(dbsmanager);
                    break;
            }
        }
    }

}
