using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;
using SnitzConfig;

namespace SnitzUI.Setup
{
    public partial class Process : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                adminUsername.Value = Request.QueryString["n"];
                adminEmail.Value = Request.QueryString["e"];
                adminPassword.Value = Request.QueryString["p"];
                updateType.Value = Request.QueryString["u"];

            }
            StringBuilder SB = new StringBuilder();
            // Padding to circumvent IE's buffer.
            Response.Write(new string('*', 256));
            Response.Flush();

            // Initialization
            UpdateProgress(0, "Initializing task.");


            try
            {

                UpdateDatabase(updateType.Value);

                // All finished!
                UpdateProgress(100, "Database upgrade Completed!");
                Config.UpdateConfig("RunSetup", "false");
            }
            catch (Exception ex)
            {
                UpdateProgress(0, "Exception: " + ex.Message.Replace("'",""));

                SB.Append("Back Up Failed!");
                SB.Append("<br/>");
                SB.Append("Failed DataBase: " + "test");
                SB.Append("<br/>");
                SB.Append("Exception: " + ex.Message);

            }

        }

        private void StoredProcedures()
        {
            Snitz.BLL.Admin.ExecuteScript(UPDATECOUNTS);
        }

        private void UpdateDatabase(string updatetype)
        {
            DbsFileProcessor dbsUpgrade = new DbsFileProcessor(HttpContext.Current.Server.MapPath("update.xml"));
            switch (updatetype)
            {
                case "new":
                    UpdateProgress(0,"Creating database<br/>");
                    Thread.Sleep(500);
                    Snitz.BLL.Admin.ExecuteScript(CREATE_DATABASE);
                    foreach (string basetable in _basecreateStrings)
                    {
                        string res = Snitz.BLL.Admin.ExecuteScript(basetable);
                        if (String.IsNullOrEmpty(res))
                        {
                            UpdateProgress(0,string.Format("<br/> {0}", basetable));
                        }
                        else
                        {
                            UpdateProgress(0,string.Format("<br/><span style='color:red'>{0}:{1}</span>",
                                                                 basetable, res));
                        }

                    }
                    StoredProcedures();
                    //Add some default data
                    UpdateProgress(0,"Adding default data<br/>");
                    AddRoles();
                    AddDefaultData();
                    break;
                case "empty":
                    StoredProcedures();
                    UpdateProgress(0,"Creating base forum tables<br/>");
                    Thread.Sleep(500);
                    //create base forum tables
                    foreach (string basetable in _basecreateStrings)
                    {
                        string res = Snitz.BLL.Admin.ExecuteScript(basetable);
                        if (String.IsNullOrEmpty(res))
                        {
                            UpdateProgress(0,string.Format("<br/> {0}", basetable));
                        }
                        else
                        {
                            UpdateProgress(0,string.Format("<br/><span style='color:red'>{0}:{1}</span>",
                                                                 basetable, res));
                        }
                        Thread.Sleep(200);
                    }
                    //upgrade to latest version
                    dbsUpgrade.Process();
                    //Add some default data
                    AddRoles();
                    AddDefaultData();
                    break;
                case "upgrade":
                    dbsUpgrade.Process();
                    StoredProcedures();
                    AddRoles();
                    UpdateUserRoles();
                    UpdateForumAllowedRoles();
                    UpdateMembersTable();
                    UpdateMembersRole();
                    break;
                case "upgradeadmin":

                    dbsUpgrade.Process();
                    AddRoles();
                    AddDefaultData();
                    UpdateForumAllowedRoles();
                    break;
            }

            Config.UpdateConfig("RunSetup", "false");

        }

        #region Creating

        private void AddDefaultData()
        {
            UpdateProgress(0,"Adding default data<br/>");
            //Add Admin Account
            string aUsername = adminUsername.Value ?? "Admin";
            string adminpassword = adminPassword.Value ?? "P@ssword01!!";
            string adminemail = adminEmail.Value ?? "Admin@admin.com";

            MembershipCreateStatus status;
            MembershipUser admin = Membership.GetUser(aUsername);
            if (admin == null || admin.UserName != aUsername)
                admin = Membership.CreateUser(aUsername, adminpassword, adminemail, ".", ".", true, out status);
            Membership.UpdateUser(admin);
            Roles.AddUserToRoles(admin.UserName, new string[] { "Administrator", "Member" });

            var newadmin = "UPDATE FORUM_MEMBERS SET M_TITLE='Forum Administrator', M_SUBSCRIPTION=1,M_LEVEL=3,M_STATUS=1,M_HIDE_EMAIL = 1,M_RECEIVE_EMAIL = 1,M_VOTED = 0 WHERE M_NAME='" + aUsername + "'";
            Snitz.BLL.Admin.ExecuteScript(newadmin);
            Snitz.BLL.Admin.ExecuteScript("INSERT INTO FORUM_TOTALS (P_COUNT,P_A_COUNT,T_COUNT,T_A_COUNT,U_COUNT) VALUES (0,0,0,0,1)");
            CategoryInfo cat = new CategoryInfo();
            ForumInfo forum = new ForumInfo();
            TopicInfo topic = new TopicInfo();
            cat.Id = -1;
            cat.Name = "Snitz .Net Forums";
            cat.Order = 0;
            cat.Status = (int)Enumerators.PostStatus.Open;
            cat.SubscriptionLevel = 0;
            cat.ModerationLevel = 0;

            int catid = Categories.AddCategory(cat);

            forum.CatId = catid;
            forum.Id = -1;
            forum.Status = (int)Enumerators.PostStatus.Open;
            forum.AllowPolls = false;
            forum.Description = "This forum gives you a chance to become more familiar with how this product responds to different features and keeps testing in one place instead of posting tests all over. Happy Posting! [:)]";
            forum.Subject = "Testing Forum";
            forum.SubscriptionLevel = 0;
            forum.ModerationLevel = 0;
            forum.Order = 0;
            forum.PostCount = 0;
            forum.UpdatePostCount = true;

            int forumid = Forums.SaveForum(forum);

            topic.CatId = catid;
            topic.ForumId = forumid;
            topic.Status = 1;
            topic.Message = "Thank you for downloading Snitz Forums 2000. We hope you enjoy this great tool to support your organization!" + 0x13 + 0x10 + 0x13 + 0x10 + "Many thanks go out to John Penfold &lt;asp@asp-dev.com&gt; and Tim Teal &lt;tteal@tealnet.com&gt; for the original source code and to all the people of Snitz Forums 2000 at http://forum.snitz.com for continued support of this product.";
            topic.Subject = "Welcome to Snitz .Net Forums";
            topic.AuthorId = (int)admin.ProviderUserKey;
            topic.Date = DateTime.UtcNow;
            topic.PosterIp = "0.0.0.0";

            Topics.Add(topic);

        }

        private void AddRoles()
        {
            UpdateProgress(0,"Adding default Roles<br/>");
            SnitzRoleProvider roles = new SnitzRoleProvider();
            if(!roles.RoleExists("Administrator"))
                roles.CreateRoleFullInfo("Administrator", "Forum Administrator", 99);
            if (!roles.RoleExists("Member"))
                roles.CreateRoleFullInfo("Member", "Snitz Forum member", 1);
            if (!roles.RoleExists("Moderator"))
                roles.CreateRoleFullInfo("Moderator", "Snitz Forum Moderator", 10);
            if (!roles.RoleExists("All"))
                roles.CreateRoleFullInfo("All", "All Visitors", 0);
            UpdateProgress(0,"New Forum Roles Added</br>");
        }

        #endregion

        #region Upgrading

        private void UpgradeDatabase()
        {
            //Needs an upgrade
            Thread.Sleep(500);
            UpdateProgress(0,"Uprading database<br/>");
            UpdateProgress(0,"Adding New Tables<br/>");
            foreach (string newTable in _newTableStrings)
            {
                try
                {
                    var res = Snitz.BLL.Admin.ExecuteScript(newTable);
                    if (res != null)
                        UpdateProgress(0, string.Format("<br/><span style=\"color:red\">{0}:{1}</span>",
                                                         newTable, res));
                    else
                        UpdateProgress(0, string.Format("<br/> {0}", newTable));
                }
                catch (Exception ex)
                {
                    UpdateProgress(0, string.Format("<br/><span style=\"color:red\">{0}:{1}</span>",
                                                         newTable, ex.Message));
                }
                Thread.Sleep(200);
            }
            UpdateProgress(0,"Updating existing Tables<br/>");
            Thread.Sleep(500);
            foreach (string table in _updateTableStrings)
            {
                try
                {
                    var res = Snitz.BLL.Admin.ExecuteScript(table);
                    if(res != null)
                        UpdateProgress(0, string.Format("<br/><span style=\"color:red\">{0}:{1}</span>",
                                                         table, res));
                    else
                    UpdateProgress(0,string.Format("<br/> {0}", table));
                }
                catch (Exception ex)
                {
                    UpdateProgress(0, string.Format("<br/><span style=\"color:red\">{0}:{1}</span>",
                                                         table, ex.Message));
                }
                Thread.Sleep(200);
            }
        }

        private void UpdateMembersTable()
        {
            Snitz.BLL.Admin.ExecuteScript("UPDATE FORUM_MEMBERS SET M_VALID=1 WHERE M_STATUS=1");
            UpdateProgress(0,"Members Table Updated</br>");
            Thread.Sleep(500);
        }

        private void UpdateUserRoles()
        {
            UpdateProgress(0,"Adding adninistrator/moderators to role table<br/>");
            string[] admins = Moderators.GetForumAdmins();
            var mods = Moderators.GetAll();

            SnitzRoleProvider roles = new SnitzRoleProvider();
            roles.AddUsersToRoles(admins, new string[] { "Administrator" });
            roles.AddUsersToRoles(mods.Select(m => m.Name).ToArray(), new string[] { "Moderator" });
            UpdateProgress(0,"UserRoles Table Updated</br>");
            Thread.Sleep(500);
        }

        private void UpdateForumAllowedRoles()
        {
            //1, 6 '## Allowed Users
            //2 '## password
            //3 '## Either Password or Allowed
            //7 '## members or password
            //4, 5 '## members only
            UpdateProgress(0,"Updating allowed forum lists<br/>");
            Thread.Sleep(500);
            SnitzRoleProvider roles = new SnitzRoleProvider();
            var privateforums = Snitz.BLL.Admin.PrivateForums();
            int newroleid = 500;

            foreach (ForumInfo forum in privateforums)
            {
                int forumid = forum.Id;
                string description = forum.Subject;

                switch (forum.PrivateForum)
                {
                    case 4: //## members
                    case 5:
                    case 7:
                        string role = SnitzMembership.Helpers.BusinessUtil.GetRoleFull(1).LoweredRolename;
                        SnitzMembership.Helpers.BusinessUtil.AddRolesToForum(forumid, new[] { role });
                        break;
                    case 1: //## Allowed Users
                    case 3:
                    case 6:
                        //create a Role for this forum
                        string rolename = "Forum_" + forumid;
                        roles.CreateRoleFullInfo(rolename, description, newroleid);
                        SnitzMembership.Helpers.BusinessUtil.AddRolesToForum(forumid, new[] { rolename });
                        //get the allowed members for this forum and add to the new role
                        string[] allowedmembers = Snitz.BLL.Admin.AllowedMembers(forumid);
                        roles.AddUsersToRoles(allowedmembers, new[] { rolename });

                        newroleid++;
                        break;
                }
            }
            UpdateProgress(0,"ForumAllowedRoles Updated</br>");
            Thread.Sleep(500);
        }

        private void UpdateMembersRole()
        {
            const string update = "INSERT INTO aspnet_UsersInRoles (UserId,RoleId) SELECT MEMBER_ID, 1 FROM FORUM_MEMBERS WHERE M_LEVEL = 1 AND M_STATUS = 1";
            Snitz.BLL.Admin.ExecuteScript(update);
            UpdateProgress(0,"Members Role Updated</br>");
            Thread.Sleep(500);
        }

        #endregion
        protected void UpdateProgress(double PercentComplete, string Message)
        {
            // Write out the parent script callback.
            Response.Write(String.Format("<script type=\"text/javascript\">parent.UpdateProgress({0}, '{1}');</script>", PercentComplete, Message + "</br>"));
            // To be sure the response isn't buffered on the server.    
            Response.Flush();
        }
    }
}