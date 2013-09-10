using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using Snitz.BLL;
using Snitz.Entities;
using Snitz.Providers;
using SnitzCommon;
using SnitzConfig;



namespace SnitzUI.Setup
{
    public partial class Setup : System.Web.UI.Page
    {
        const string AspnetApplication = "CREATE TABLE [dbo].[aspnet_Applications](" +
                                           "[ApplicationName] [nvarchar](256)  NOT NULL," +
                                           "[LoweredApplicationName] [nvarchar](256)  NOT NULL," +
                                           "[ApplicationId] [uniqueidentifier] NOT NULL PRIMARY KEY," +
                                           "[Description] [nvarchar](256)  NULL" +
                                           ") ON [PRIMARY];";

        const string AspnetPaths = "CREATE TABLE [dbo].[aspnet_Paths](" +
                        "[ApplicationId] [uniqueidentifier] NOT NULL PRIMARY KEY," +
                        "[PathId] [uniqueidentifier] NOT NULL," +
                        "[Path] [nvarchar](256)  NOT NULL," +
                        "[LoweredPath] [nvarchar](256)  NOT NULL" +
                        ") ON [PRIMARY]";

        const string AspnetSchema = "CREATE TABLE [dbo].[aspnet_SchemaVersions](" +
                                       "[Feature] [nvarchar](128)  NOT NULL," +
                                       "[CompatibleSchemaVersion] [nvarchar](128)  NOT NULL," +
                                       "[IsCurrentVersion] [bit] NOT NULL" +
                                       ") ON [PRIMARY]";

        const string AspnetRoles =   "CREATE TABLE [dbo].[aspnet_Roles](" +
                                "[ApplicationId] [uniqueidentifier] NOT NULL," +
                                "[RoleName] [nvarchar](256)  NOT NULL," +
                                "[LoweredRoleName] [nvarchar](256)  NOT NULL," +
                                "[Description] [nvarchar](256)  NULL," +
                                "[RoleId] [int] NOT NULL PRIMARY KEY" +
                                ") ON [PRIMARY]";

        const string AspnetUsers = "CREATE TABLE [dbo].[aspnet_Users](" +
                                      "[ApplicationId] [uniqueidentifier] NOT NULL," +
                                      "[UserId] [uniqueidentifier] NOT NULL PRIMARY KEY," +
                                      "[UserName] [nvarchar](256)  NOT NULL," +
                                      "[LoweredUserName] [nvarchar](256)  NOT NULL," +
                                      "[MobileAlias] [nvarchar](16)  NULL," +
                                      "[IsAnonymous] [bit] NOT NULL," +
                                      "[LastActivityDate] [datetime] NOT NULL" +
                                      ") ON [PRIMARY]";

        const string AspnetUserRoles = "CREATE TABLE [dbo].[aspnet_UsersInRoles](" +
                                          "[UserId] [int] NOT NULL," +
                                          "[RoleId] [int] NOT NULL," +
                                          "CONSTRAINT pk_UsersInRoles PRIMARY KEY (UserId,RoleId)" +
                                          ") ON [PRIMARY]";

        const string ForumEvents = "CREATE TABLE [dbo].[FORUM_EVENT](" +
                                      "[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                      "[Title] [nvarchar](50)  NOT NULL," +
                                      "[Type] [int] NOT NULL," +
                                      "[Audience] [nvarchar](255)  NULL," +
                                      "[Author] [int] NOT NULL," +
                                      "[EventDate] [nvarchar](50)  NOT NULL," +
                                      "[Description] [nvarchar](400)  NULL" +
                                      ") ON [PRIMARY]";

        const string ForumFaqCat = "CREATE TABLE [dbo].[FORUM_FAQ_CAT](" +
                                      "[FC_ID] [int] IDENTITY(9,1) NOT NULL PRIMARY KEY," +
                                      "[FC_DESCRIPTION] [nvarchar](255)  NOT NULL," +
                                      "[FC_LANG_ID] [varchar](6)  NULL," +
                                      "[FC_ORDER] [int] NULL" +
                                      ") ON [PRIMARY]";

        const string ForumFaqInfo = "CREATE TABLE [dbo].[FORUM_FAQ_INFO](" +
                                       "[FI_ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                       "[FI_ORDER] [int] NOT NULL," +
                                       "[FI_LINK] [nvarchar](50)  NULL," +
                                       "[FI_LINK_TITLE] [nvarchar](255)  NULL," +
                                       "[FI_LINK_BODY] [nvarchar](max)  NULL," +
                                       "[FI_CAT] [int] NOT NULL," +
                                       "[FI_LANG_ID] [varchar](6)  NULL" +
                                       ") ON [PRIMARY]";

        const string ForumPm = "CREATE TABLE [dbo].[FORUM_PM](" +
                                  "[M_ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                  "[M_SUBJECT] [nvarchar](100)  NULL," +
                                  "[M_FROM] [int] NOT NULL," +
                                  "[M_TO] [int] NOT NULL," +
                                  "[M_SENT] [nvarchar](50)  NULL," +
                                  "[M_MESSAGE] [nvarchar](max)  NULL," +
                                  "[M_PMCOUNT] [nvarchar](50)  NULL," +
                                  "[M_READ] [int] NOT NULL," +
                                  "[M_MAIL] [nvarchar](50)  NULL," +
                                  "[M_OUTBOX] [int] NOT NULL" +
                                  ") ON [PRIMARY]";

        const string ForumPollAnswers = "CREATE TABLE [dbo].[FORUM_POLLANSWERS](" +
                                           "[PollAnswerID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                           "[PollID] [int] NOT NULL," +
                                           "[DisplayText] [nvarchar](500)  NOT NULL," +
                                           "[SortOrder] [int] NOT NULL" +
                                           ") ON [PRIMARY]";

        const string ForumPollResponse = "CREATE TABLE [dbo].[FORUM_POLLRESPONSE](" +
                                            "[UserID] [int] NOT NULL," +
                                            "[PollAnswerID] [int] NOT NULL," +
                                            "CONSTRAINT pk_POLLRESPONSE PRIMARY KEY (UserID,PollAnswerID)" +
                                            ") ON [PRIMARY]";

        const string ForumPolls = "CREATE TABLE [dbo].[FORUM_POLLS](" +
                                     "[PollID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                     "[DisplayText] [nvarchar](500)  NOT NULL," +
                                     "[TopicId] [int] NULL" +
                                     ") ON [PRIMARY]";

        const string ForumRoles = "CREATE TABLE [dbo].[FORUM_ROLES](" +
                                     "[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                     "[Forum_id] [int] NOT NULL," +
                                     "[Role_Id] [int] NOT NULL" +
                                     ") ON [PRIMARY]";

        const string ForumProfile = "CREATE TABLE [dbo].[ProfileData](" +
                                       "[UserId] [int] NOT NULL PRIMARY KEY," +
                                       "[Skype] [nvarchar](50)  NULL," +
                                       "[HideAge] [smallint] NOT NULL," +
                                       "[LinkTarget] [nvarchar](20)  NULL," +
                                       "[FavLinks] [nvarchar](max)  NOT NULL," +
                                       "[Gravatar] [smallint] NOT NULL," +
                                       "[BookMarks] [nvarchar](max)  NOT NULL," +
                                       "[PublicGallery] [smallint] NOT NULL," +
                                       "[PMEmail] [smallint] NOT NULL," +
                                       "[PMReceive] [smallint] NOT NULL," +
                                       "[PMLayout] [nvarchar](50)  NULL" +
                                       ") ON [PRIMARY]";

        private string[] _newTableStrings = new string[] { 
            //AspnetApplication, AspnetPaths, AspnetSchema,  
            AspnetRoles, AspnetUsers, AspnetUserRoles,
            ForumEvents,ForumFaqCat,ForumFaqInfo,ForumPm,
            ForumPollAnswers,ForumPollResponse,ForumPolls,
            ForumRoles,ForumProfile};

        private string[] _updateTableStrings = new string[] { 
                "ALTER TABLE FORUM_FORUM DROP COLUMN [F_USERLIST];",
                 "ALTER TABLE FORUM_FORUM ADD [F_POLLS] [int] NOT NULL DEFAULT 0;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_VOTED] [smallint] NOT NULL DEFAULT 0;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_LASTUPDATED] [nvarchar](50)  NULL;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_AVATAR] [nvarchar](255)  NULL;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_THEME] [nvarchar](50)  NULL;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_TIMEOFFSET] [int] NOT NULL DEFAULT 0;",
                 "ALTER TABLE FORUM_MEMBERS ADD [M_VALID] [smallint] NOT NULL DEFAULT 0;",
                 "ALTER TABLE FORUM_TOPICS ALTER COLUMN [T_MESSAGE] [nvarchar](MAX)",
                 "ALTER TABLE FORUM_REPLY ALTER COLUMN [R_MESSAGE] [nvarchar](MAX)"
                };
        const string ChangeDbOwner = 
            "declare @sql varchar(8000); " +
            "select @sql = coalesce( @sql, ';', '') + 'alter schema dbo transfer [' + s.name + '].[' + t.name + '];' " +
            "from  sys.tables t inner join sys.schemas s on t.[schema_id] = s.[schema_id] where  s.name <> 'dbo' ;" +
            "exec( @sql );";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            using (var dc = new SnitzSetupDataContext())
            {
                if (!dc.DatabaseExists())
                {
                    //no database so lets create one based on the entity diagram
                    updateType.Value = "new";
                    AdminUserRequired.Visible = true;
                    UpdateDB.Text = "Create Database";
                }else
                {
                    //make sure that dbo is the table owner otherwise we will have problems
                    dc.ExecuteCommand(ChangeDbOwner);
                    // ok let's check if the Members table exist or not
                    if (!DoesTableExist("FORUM_MEMBERS", ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString))
                    {
                        //no members table so let's create the base forum tables
                        updateType.Value = "empty";
                        UpdateDB.Text = "Create Tables";
                        AdminUserRequired.Visible = true;
                    }
                    else
                    {
                        AdminUserRequired.Visible = false;
                        UpdateDB.Text = "Upgrade Database";
                        UpdateDatabase("upgrade");

                    }
                }
            }
        }

        private void UpdateMembersTable()
        {
            using(var dc = new SnitzSetupDataContext())
            {
                dc.ExecuteCommand("UPDATE FORUM_MEMBERS SET M_VALID=1 WHERE M_STATUS=1");
                litSetupResult.Text += "Members Table Updated</br>";
            }
        }

        private void UpdateUserRoles()
        {
            litSetupResult.Text += "Adding adninistrator/moderators to role table<br/>";
            using(var dc = new SnitzSetupDataContext())
            {
                var admins = from m in dc.FORUM_MEMBERs
                             where m.M_LEVEL == 3
                             select m.M_NAME;
                var mods = from m in dc.FORUM_MEMBERs
                           where m.M_LEVEL == 2
                           select m.M_NAME;

                SnitzRoleProvider roles = new SnitzRoleProvider();
                roles.AddUsersToRoles(admins.ToArray(),new string[]{"Administrator"});
                roles.AddUsersToRoles(mods.ToArray(), new string[] { "Moderator" });
                litSetupResult.Text += "UserRoles Table Updated</br>";
            }
        }

        private void UpdateForumAllowedRoles()
        {
            //1, 6 '## Allowed Users
            //2 '## password
            //3 '## Either Password or Allowed
            //7 '## members or password
            //4, 5 '## members only
            litSetupResult.Text += "Updating allowed forum lists<br/>";
            using(var dc = new SnitzSetupDataContext())
            {
                var privateforums = from f in dc.FORUM_FORUMs
                                    where f.F_PRIVATEFORUMS > 0
                                    select f;
                int newroleid = 500;

                foreach (FORUM_FORUM forum in privateforums)
                {
                    int forumid = forum.FORUM_ID;
                    string description = forum.F_SUBJECT;

                    switch (forum.F_PRIVATEFORUMS)
                    {
                        case 4 : //## members
                        case 5 :
                        case 7:
                            dc.FORUM_ROLEs.InsertOnSubmit(new FORUM_ROLE() { Forum_id = forumid, Role_Id = 1 });
                            dc.SubmitChanges();
                            break;
                        case 1 : //## Allowed Users
                        case 3 :
                        case 6 :
                            //create a Role for this forum
                            string rolename = "Forum_" + forumid;
                            SnitzRoleProvider roles = new SnitzRoleProvider();
                            roles.CreateRoleFullInfo(rolename, description, newroleid);
                            
                            dc.FORUM_ROLEs.InsertOnSubmit(new FORUM_ROLE() { Forum_id = forumid, Role_Id = newroleid });
                            dc.SubmitChanges();
                            //get the allowed members for this forum and add to the new role
                            var allowedmembers = from m in dc.FORUM_ALLOWED_MEMBERs
                                                 where m.FORUM_ID == forumid
                                                 select m;

                            foreach (FORUM_ALLOWED_MEMBER allowedmember in allowedmembers)
                            {
                                dc.aspnet_UsersInRoles.InsertOnSubmit(new aspnet_UsersInRole() { UserId = allowedmember.MEMBER_ID, RoleId = newroleid });
                            }
                            dc.SubmitChanges();
                            newroleid++;
                            break;
                    }
                }
                litSetupResult.Text += "ForumAllowedRoles Updated</br>";
            }
        }

        private void AddRoles()
        {
            litSetupResult.Text += "Adding default Roles<br/>";
            SnitzRoleProvider roles = new SnitzRoleProvider();
            roles.CreateRoleFullInfo("Administrator", "Forum Administrator", 99);
            roles.CreateRoleFullInfo("Member", "Snitz Forum member", 1);
            roles.CreateRoleFullInfo("Moderator", "Snitz Forum Moderator", 10);
            roles.CreateRoleFullInfo("All", "All Visitors", 0);
            litSetupResult.Text += "New Forum Roles Added</br>";
        }
        private void AddDefaultData()
        {
            litSetupResult.Text += "Adding default data<br/>";
            //Add Admin Account
            string adminUsername = adminUser.Text ?? "Admin";
            string adminpassword = adminPassword.Text ?? "P@ssword01!!";
            string adminemail = adminEmail.Text ?? "Admin@admin.com";

            MembershipCreateStatus status;
            MembershipUser admin = Membership.CreateUser(adminUsername, adminpassword, adminemail,".",".",true, out status);

            Membership.UpdateUser(admin);
            Roles.AddUserToRoles(admin.UserName, new string[] { "Administrator", "Member" });

            var dc = new SnitzSetupDataContext();
            var newadmin =
                "UPDATE FORUM_MEMBERS SET M_SUBSCRIPTION=1,M_STATUS=1,M_HIDE_EMAIL = 1,M_RECEIVE_EMAIL = 1,M_VOTED = 0 WHERE M_NAME='" +
                adminUsername + "'";
            dc.ExecuteCommand(newadmin);

            CategoryInfo cat = new CategoryInfo();
            ForumInfo forum = new ForumInfo();
            cat.Id = -1;
            cat.Name = "Default Category";
            cat.Order = 0;
            cat.Status = (int)Enumerators.PostStatus.Open;
            cat.SubscriptionLevel = 0;
            cat.ModerationLevel = 0;

            int catid = Categories.AddCategory(cat);

            forum.CatId = catid;
            forum.Id = -1;
            forum.Status = (int)Enumerators.PostStatus.Open;
            forum.AllowPolls = false;
            forum.Description = "Default forum";
            forum.Subject = "Snitz .Net Forum";
            forum.SubscriptionLevel = 0;
            forum.ModerationLevel = 0;
            forum.Order = 0;

            int forumid = Forums.SaveForum(forum);

        }

        /// <summary>
        /// Checks to see if a table exists in Database or not.
        /// </summary>
        /// <param name="tblName">Table name to check</param>
        /// <param name="cnnStr">Connection String to connect to</param>
        /// <returns>Works with Access or SQL</returns>
        /// <remarks></remarks>

        public bool DoesTableExist(string tblName, string cnnStr)
        {
            bool functionReturnValue = false;
            // For Access Connection String,
            // use "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" &
            // accessFilePathAndName

            // Open connection to the database
            SqlConnection dbConn = new SqlConnection(cnnStr);
            dbConn.Open();

            // Specify restriction to get table definition schema
            // For reference on GetSchema see:
            // http://msdn2.microsoft.com/en-us/library/ms254934(VS.80).aspx

            string[] restrictions = new string[4];
            restrictions[2] = tblName;
            DataTable dbTbl = dbConn.GetSchema("Tables", restrictions);

            if (dbTbl.Rows.Count == 0)
            {
                //Table does not exist
                functionReturnValue = false;
            }
            else
            {
                //Table exists
                functionReturnValue = true;
            }

            dbTbl.Dispose();
            dbConn.Close();
            dbConn.Dispose();
            return functionReturnValue;
        }


        /// <summary>
        /// Checks to see if a field exists in table or not.
        /// </summary>
        /// <param name="tblName">Table name to check in</param>
        /// <param name="fldName">Field name to check</param>
        /// <param name="cnnStr">Connection String to connect to</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool DoesFieldExist(string tblName, string fldName, string cnnStr)
        {
            bool functionReturnValue = false;
            // For Access Connection String,
            // use "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" &
            // accessFilePathAndName

            // Open connection to the database
            SqlConnection dbConn = new SqlConnection(cnnStr);
            dbConn.Open();
            DataTable dbTbl = new DataTable();

            // Get the table definition loaded in a table adapter
            string strSql = "Select TOP 1 * from " + tblName;
            SqlDataAdapter dbAdapater = new SqlDataAdapter(strSql, dbConn);
            dbAdapater.Fill(dbTbl);

            // Get the index of the field name
            int i = dbTbl.Columns.IndexOf(fldName);

            if (i == -1)
            {
                //Field is missing
                functionReturnValue = false;
            }
            else
            {
                //Field is there
                functionReturnValue = true;
            }

            dbTbl.Dispose();
            dbConn.Close();
            dbConn.Dispose();
            return functionReturnValue;
        }

        protected void UpdateDB_Click(object sender, EventArgs e)
        {
            UpdateDatabase(updateType.Value);
        }

        private void UpdateDatabase(string updatetype)
        {
            switch (updatetype)
            {
                case "new":
                    litSetupResult.Text = "Creating database<br/>";
                    using (var dc = new SnitzSetupDataContext())
                        dc.CreateDatabase();
                    //Add some default data
                    litSetupResult.Text += "Adding default data<br/>";
                    AddRoles();
                    AddDefaultData();
                    break;
                case "empty":
                    litSetupResult.Text = "Creating base forum tables<br/>";
                    //create base forum tables
                    using (var dc = new SnitzSetupDataContext())
                    {
                        foreach (string basetable in _basecreateStrings)
                        {
                            try
                            {
                                dc.ExecuteCommand(basetable);
                                litSetupResult.Text += string.Format("<br/> {0}", basetable);
                            }
                            catch (Exception ex)
                            {
                                litSetupResult.Text += string.Format("<br/><span style='color:red'>{0}:{1}</span>",
                                                                     basetable, ex.Message);
                            }

                        }
                    }
                    //upgrade to latest version
                    UpgradeDatabase();
                    //Add some default data
                    AddRoles();
                    AddDefaultData();
                    break;
                case "upgrade":
                    litSetupResult.Text = "";
                    //upgrade to latest version
                    UpgradeDatabase();
                    AddRoles();
                    UpdateUserRoles();
                    UpdateForumAllowedRoles();
                    UpdateMembersTable();
                    UpdateMembersRole();
                    break;
            }

            Config.UpdateConfig("RunSetup", "false");
            lnkReturn.Visible = true;
            //Response.Redirect("~/default.aspx", true);
        }

        private void UpdateMembersRole()
        {
            using (var dc = new SnitzSetupDataContext())
            {
                string update = "INSERT INTO aspnet_UsersInRoles (UserId,RoleId) SELECT MEMBER_ID, 1 FROM FORUM_MEMBERS WHERE M_LEVEL = 1 AND M_STATUS = 1";
                dc.ExecuteCommand(update);
                litSetupResult.Text += "Members Role Updated</br>";
            }
        }

        private void UpgradeDatabase()
        {
            //Needs an upgrade
            litSetupResult.Text += "Uprading database<br/>";
            using (var dc = new SnitzSetupDataContext())
            {
                litSetupResult.Text += "Adding New Tables<br/>";
                foreach (string newTable in _newTableStrings)
                {
                    try
                    {
                        dc.ExecuteCommand(newTable);
                        litSetupResult.Text += string.Format("<br/> {0}", newTable);
                    }
                    catch (Exception ex)
                    {
                        litSetupResult.Text += string.Format("<br/><span style='color:red'>{0}:{1}</span>",
                                                             newTable, ex.Message);
                    }

                }
                litSetupResult.Text += "Updating existing Tables<br/>";
                foreach (string table in _updateTableStrings)
                {
                    try
                    {
                        dc.ExecuteCommand(table);
                        litSetupResult.Text += string.Format("<br/> {0}", table);
                    }
                    catch (Exception ex)
                    {
                        litSetupResult.Text += string.Format("<br/><span style='color:red'>{0}:{1}</span>",
                                                             table, ex.Message);
                    }

                }

            }
        }
    }
}