CREATE TABLE [dbo].[aspnet_Applications](
	[ApplicationName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[LoweredApplicationName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](256) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[aspnet_Paths](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[PathId] [uniqueidentifier] NOT NULL,
	[Path] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[LoweredPath] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[aspnet_Roles](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[RoleName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[LoweredRoleName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[Description] [nvarchar](256) COLLATE Latin1_General_CI_AS NULL,
	[RoleId] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[aspnet_SchemaVersions](
	[Feature] [nvarchar](128) COLLATE Latin1_General_CI_AS NOT NULL,
	[CompatibleSchemaVersion] [nvarchar](128) COLLATE Latin1_General_CI_AS NOT NULL,
	[IsCurrentVersion] [bit] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[aspnet_Users](
	[ApplicationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[LoweredUserName] [nvarchar](256) COLLATE Latin1_General_CI_AS NOT NULL,
	[MobileAlias] [nvarchar](16) COLLATE Latin1_General_CI_AS NULL,
	[IsAnonymous] [bit] NOT NULL,
	[LastActivityDate] [datetime] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[aspnet_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_EVENT](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[Type] [int] NOT NULL,
	[Audience] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[Author] [int] NOT NULL,
	[EventDate] [nvarchar](50) COLLATE Latin1_General_CI_AS NOT NULL,
	[Description] [nvarchar](400) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_FAQ_CAT](
	[FC_ID] [int] IDENTITY(9,1) NOT NULL,
	[FC_DESCRIPTION] [nvarchar](255) COLLATE Latin1_General_CI_AS NOT NULL,
	[FC_LANG_ID] [varchar](6) COLLATE Latin1_General_CI_AS NULL,
	[FC_ORDER] [int] NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_FAQ_INFO](
	[FI_ID] [int] IDENTITY(1,1) NOT NULL,
	[FI_ORDER] [int] NOT NULL,
	[FI_LINK] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL,
	[FI_LINK_TITLE] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL,
	[FI_LINK_BODY] [nvarchar](max) COLLATE Latin1_General_CI_AS NULL,
	[FI_CAT] [int] NOT NULL,
	[FI_LANG_ID] [varchar](6) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_PM](
	[M_ID] [int] IDENTITY(1,1) NOT NULL,
	[M_SUBJECT] [nvarchar](100) COLLATE Latin1_General_CI_AS NULL,
	[M_FROM] [int] NOT NULL,
	[M_TO] [int] NOT NULL,
	[M_SENT] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL,
	[M_MESSAGE] [nvarchar](max) COLLATE Latin1_General_CI_AS NULL,
	[M_PMCOUNT] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL,
	[M_READ] [int] NOT NULL,
	[M_MAIL] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL,
	[M_OUTBOX] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_POLLANSWERS](
	[PollAnswerID] [int] IDENTITY(1,1) NOT NULL,
	[PollID] [int] NOT NULL,
	[DisplayText] [nvarchar](500) COLLATE Latin1_General_CI_AS NOT NULL,
	[SortOrder] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_POLLRESPONSE](
	[UserID] [int] NOT NULL,
	[PollAnswerID] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_POLLS](
	[PollID] [int] IDENTITY(1,1) NOT NULL,
	[DisplayText] [nvarchar](500) COLLATE Latin1_General_CI_AS NOT NULL,
	[TopicId] [int] NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[FORUM_ROLES](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Forum_id] [int] NOT NULL,
	[Role_Id] [int] NOT NULL
) ON [PRIMARY];

CREATE TABLE [dbo].[ProfileData](
	[UserId] [int] NOT NULL,
	[Skype] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL,
	[HideAge] [smallint] NOT NULL,
	[LinkTarget] [nvarchar](20) COLLATE Latin1_General_CI_AS NULL,
	[FavLinks] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[Gravatar] [smallint] NOT NULL,
	[BookMarks] [nvarchar](max) COLLATE Latin1_General_CI_AS NOT NULL,
	[PublicGallery] [smallint] NOT NULL,
	[PMEmail] [smallint] NOT NULL,
	[PMReceive] [smallint] NOT NULL,
	[PMLayout] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL
) ON [PRIMARY];



ALTER TABLE FORUM_FORUM ADD [F_USERLIST] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL;
ALTER TABLE FORUM_FORUM ADD [F_POLLS] [int] NOT NULL;


ALTER TABLE FORUM_MEMBERS ADD [M_LASTUPDATED] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL;
ALTER TABLE FORUM_MEMBERS ADD [M_AVATAR] [nvarchar](255) COLLATE Latin1_General_CI_AS NULL;
ALTER TABLE FORUM_MEMBERS ADD [M_THEME] [nvarchar](50) COLLATE Latin1_General_CI_AS NULL;
ALTER TABLE FORUM_MEMBERS ADD [M_TIMEOFFSET] [int] NULL;
ALTER TABLE FORUM_MEMBERS ADD [M_VALID] [smallint] NOT NULL;

