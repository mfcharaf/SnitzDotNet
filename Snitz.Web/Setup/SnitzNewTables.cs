namespace SnitzUI.Setup
{
    public partial class Process
    {
        private string[] _updateTableStrings = new string[] { 
                " IF COL_LENGTH('FORUM_FORUM', 'F_USERLIST') IS NOT NULL BEGIN ALTER TABLE FORUM_FORUM DROP COLUMN [F_USERLIST] END",
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

        private string[] _newTableStrings = new string[] { 
            ASPNET_ROLES,ASPNET_USERS,ASPNET_USERSINROLES,FORUM_FAQ_CAT,
            FORUM_FAQ_INFO,FORUM_PM,FORUM_POLLANSWERS,FORUM_POLLRESPONSE,
            FORUM_POLLS,FORUM_ROLES,PROFILEDATA, FORUM_EVENTS
        };

        private const string CREATE_DATABASE = "";

        private const string ASPNET_ROLES = "CREATE TABLE [aspnet_Roles]( " +
                                            "    [ApplicationId] [uniqueidentifier] NOT NULL," +
                                            "    [RoleName] [nvarchar](256) NOT NULL," +
                                            "    [LoweredRoleName] [nvarchar](256) NOT NULL," +
                                            "    [Description] [nvarchar](256) NULL," +
                                            "    [RoleId] [int] NOT NULL" +
                                            ") ON [PRIMARY]";

        private const string ASPNET_USERS = "CREATE TABLE [aspnet_Users](" +
                                            "    [ApplicationId] [uniqueidentifier] NOT NULL," +
                                            "    [UserId] [uniqueidentifier] NOT NULL," +
                                            "    [UserName] [nvarchar](256) NOT NULL," +
                                            "    [LoweredUserName] [nvarchar](256) NOT NULL," +
                                            "    [MobileAlias] [nvarchar](16) NULL," +
                                            "    [IsAnonymous] [bit] NOT NULL," +
                                            "[LastActivityDate] [datetime] NOT NULL" +
                                            ") ON [PRIMARY]";

        private const string ASPNET_USERSINROLES = "CREATE TABLE [aspnet_UsersInRoles](" +
                                                   "    [UserId] [int] NOT NULL," +
                                                   "    [RoleId] [int] NOT NULL," +
                                                   "CONSTRAINT pk_UsersInRoles PRIMARY KEY (UserId,RoleId)" +
                                                    ") ON [PRIMARY]";

        private const string FORUM_FAQ_CAT = "CREATE TABLE [FORUM_FAQ_CAT](" +
                                             "    [FC_ID] [int] IDENTITY(9,1) NOT NULL," +
                                             "    [FC_DESCRIPTION] [nvarchar](255) NOT NULL," +
                                             "    [FC_LANG_ID] [varchar](6) NULL," +
                                             "    [FC_ORDER] [int] NULL" +
                                             ") ON [PRIMARY]";

        private const string FORUM_FAQ_INFO = "CREATE TABLE [FORUM_FAQ_INFO](" +
                                              "    [FI_ID] [int] IDENTITY(1,1) NOT NULL," +
                                              "    [FI_ORDER] [int] NOT NULL," +
                                              "    [FI_LINK] [nvarchar](50) NULL," +
                                              "    [FI_LINK_TITLE] [nvarchar](255) NULL," +
                                              "    [FI_LINK_BODY] [ntext] NULL," +
                                              "    [FI_CAT] [int] NOT NULL," +
                                              "    [FI_LANG_ID] [varchar](6)  NULL" +
                                              ") ON [PRIMARY]";

        private const string FORUM_PM = "CREATE TABLE [FORUM_PM](" +
                                        "    [M_ID] [int] IDENTITY(1,1) NOT NULL," +
                                        "    [M_SUBJECT] [nvarchar](100) NULL," +
                                        "    [M_FROM] [int] NOT NULL," +
                                        "    [M_TO] [int] NOT NULL," +
                                        "    [M_SENT] [nvarchar](50) NULL," +
                                        "    [M_MESSAGE] [ntext] NULL," +
                                        "    [M_PMCOUNT] [nvarchar](50) NULL," +
                                        "    [M_READ] [int] NOT NULL," +
                                        "    [M_MAIL] [nvarchar](50) NULL," +
                                        "    [M_OUTBOX] [int] NOT NULL" +
                                        ") ON [PRIMARY]";

        private const string FORUM_POLLANSWERS = "CREATE TABLE [FORUM_POLLANSWERS](" +
                                                 "    [PollAnswerID] [int] IDENTITY(1,1) NOT NULL," +
                                                 "    [PollID] [int] NOT NULL," +
                                                 "    [DisplayText] [nvarchar](500) NOT NULL," +
                                                 "    [SortOrder] [int] NOT NULL" +
                                                 ") ON [PRIMARY]";

        private const string FORUM_POLLRESPONSE = "CREATE TABLE [FORUM_POLLRESPONSE](" +
                                                  "    [UserID] [int] NOT NULL," +
                                                  "    [PollAnswerID] [int] NOT NULL," +
                                                  " CONSTRAINT pk_POLLRESPONSE PRIMARY KEY (UserID,PollAnswerID)" +
                                                  ") ON [PRIMARY]";

        private const string FORUM_POLLS = "CREATE TABLE [FORUM_POLLS](" +
                                           "    [PollID] [int] IDENTITY(1,1) NOT NULL," +
                                           "    [DisplayText] [nvarchar](500) NOT NULL," +
                                           "    [TopicId] [int] NULL," +
                                           "    [CloseAfterDays] [int] NULL," +
                                           "    [ShowResultsBeforeClose] [smallint] NOT NULL" +
                                           ") ON [PRIMARY]";

        private const string FORUM_ROLES = "CREATE TABLE [FORUM_ROLES](" +
                                           "    [Id] [int] IDENTITY(1,1) NOT NULL," +
                                           "    [Forum_id] [int] NOT NULL," +
                                           "    [Role_Id] [int] NOT NULL" +
                                           ") ON [PRIMARY]";

        private const string PROFILEDATA = "CREATE TABLE [ProfileData](" +
                                           "    [UserId] [int] NOT NULL," +
                                           "    [Skype] [nvarchar](50) NULL," +
                                           "    [HideAge] [smallint] NOT NULL," +
                                           "    [LinkTarget] [nvarchar](20) NULL," +
                                           "    [FavLinks] [ntext] NULL," +
                                           "    [Gravatar] [smallint] NOT NULL," +
                                           "    [BookMarks] [ntext] NULL," +
                                           "    [PublicGallery] [smallint] NOT NULL," +
                                           "    [PMEmail] [smallint] NULL," +
                                           "    [PMReceive] [smallint] NULL," +
                                           "    [PMLayout] [nvarchar](50) NULL," +
                                           "    [TimeOffset] [int] NOT NULL" +
                                           ") ON [PRIMARY]";

        const string FORUM_EVENTS = "CREATE TABLE [dbo].[FORUM_EVENT](" +
                                      "[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                      "[Title] [nvarchar](50)  NOT NULL," +
                                      "[Type] [int] NOT NULL," +
                                      "[Audience] [nvarchar](255)  NULL," +
                                      "[Author] [int] NOT NULL," +
                                      "[EventDate] [nvarchar](50)  NOT NULL," +
                                      "[Description] [nvarchar](400)  NULL" +
                                      ") ON [PRIMARY]";
    }
}