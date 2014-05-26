namespace SnitzUI.Setup
{
    public partial class Process
    {
        private string[] _basecreateStrings = new string[] { 
            FORUM_A_REPLY, FORUM_A_TOPIC, FORUM_BADWORDS,
            FORUM_CATEGORY, FORUM_FORUM, FORUM_GROUP_NAMES, FORUM_GROUPS,
            FORUM_MEMBERS, FORUM_MODERATOR, FORUM_NAMEFILTER, FORUM_REPLY,
            FORUM_SUBSCRIPTIONS, FORUM_TOPICS, FORUM_TOTALS
        };

        const string  FORUM_A_REPLY = "CREATE TABLE [FORUM_A_REPLY]( " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] NOT NULL, " +
        "	[TOPIC_ID] [int] NOT NULL, " +
        "	[REPLY_ID] [int] NOT NULL, " +
        "	[R_STATUS] [smallint] NULL, " +
        "	[R_MAIL] [smallint] NULL, " +
        "	[R_AUTHOR] [int] NULL, " +
        "	[R_MESSAGE] [nvarchar](max) NULL, " +
        "	[R_DATE] [nvarchar](14) NULL, " +
        "	[R_IP] [nvarchar](50) NULL, " +
        "	[R_LAST_EDIT] [nvarchar](14) NULL, " +
        "	[R_LAST_EDITBY] [int] NULL, " +
        "	[R_SIG] [smallint] NULL " +
        ") ON [PRIMARY];";


        const string FORUM_A_TOPIC = "CREATE TABLE [FORUM_A_TOPICS]( " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] NOT NULL, " +
        "	[TOPIC_ID] [int] NOT NULL, " +
        "	[T_STATUS] [smallint] NULL, " +
        "	[T_MAIL] [smallint] NULL, " +
        "	[T_SUBJECT] [nvarchar](100) NULL, " +
        "	[T_MESSAGE] [nvarchar](max) NULL, " +
        "	[T_AUTHOR] [int] NULL, " +
        "	[T_REPLIES] [int] NULL, " +
        "	[T_UREPLIES] [int] NULL, " +
        "	[T_VIEW_COUNT] [int] NULL, " +
        "	[T_LAST_POST] [nvarchar](14) NULL, " +
        "	[T_DATE] [nvarchar](14) NULL, " +
        "	[T_LAST_POSTER] [int] NULL, " +
        "	[T_IP] [nvarchar](50) NULL, " +
        "	[T_LAST_POST_AUTHOR] [int] NULL, " +
        "	[T_LAST_POST_REPLY_ID] [int] NULL, " +
        "	[T_LAST_EDIT] [nvarchar](14) NULL, " +
        "	[T_LAST_EDITBY] [int] NULL, " +
        "	[T_STICKY] [smallint] NULL, " +
        "	[T_SIG] [smallint] NULL " +
        ") ON [PRIMARY];";

        const string FORUM_BADWORDS = "CREATE TABLE [FORUM_BADWORDS]( " +
        "	[B_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[B_BADWORD] [nvarchar](50) NULL, " +
        "	[B_REPLACE] [nvarchar](50) NULL, " +
        " CONSTRAINT [PK_FORUM_BADWORDS] PRIMARY KEY CLUSTERED  " +
        "( [B_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_CATEGORY = "CREATE TABLE [FORUM_CATEGORY]( " +
        "	[CAT_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[CAT_STATUS] [smallint] NULL, " +
        "	[CAT_NAME] [nvarchar](100) NULL, " +
        "	[CAT_MODERATION] [int] NULL, " +
        "	[CAT_SUBSCRIPTION] [int] NULL, " +
        "	[CAT_ORDER] [int] NULL, " +
        " CONSTRAINT [PK_FORUM_CATEGORY] PRIMARY KEY CLUSTERED  " +
        "( [CAT_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";


        const string FORUM_FORUM = "CREATE TABLE [FORUM_FORUM]( " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[F_STATUS] [smallint] NULL, " +
        "	[F_MAIL] [smallint] NULL, " +
        "	[F_SUBJECT] [nvarchar](100) NULL, " +
        "	[F_URL] [nvarchar](255) NULL, " +
        "	[F_DESCRIPTION] [nvarchar](max) NULL, " +
        "	[F_TOPICS] [int] NULL, " +
        "	[F_COUNT] [int] NULL, " +
        "	[F_LAST_POST] [nvarchar](14) NULL, " +
        "	[F_PASSWORD_NEW] [nvarchar](255) NULL, " +
        "	[F_PRIVATEFORUMS] [int] NULL, " +
        "	[F_TYPE] [smallint] NULL, " +
        "	[F_IP] [nvarchar](50) NULL, " +
        "	[F_LAST_POST_AUTHOR] [int] NULL, " +
        "	[F_LAST_POST_TOPIC_ID] [int] NULL, " +
        "	[F_LAST_POST_REPLY_ID] [int] NULL, " +
        "	[F_A_TOPICS] [int] NULL, " +
        "	[F_A_COUNT] [int] NULL, " +
        "	[F_MODERATION] [int] NOT NULL, " +
        "	[F_SUBSCRIPTION] [int] NOT NULL, " +
        "	[F_ORDER] [int] NOT NULL, " +
        "	[F_DEFAULTDAYS] [int] NULL, " +
        "	[F_COUNT_M_POSTS] [smallint] NULL, " +
        "	[F_L_ARCHIVE] [nvarchar](14) NULL , " +
        "	[F_ARCHIVE_SCHED] [int] NOT NULL DEFAULT 30, " +
        "	[F_L_DELETE] [nvarchar](14) NULL, " +
        "	[F_DELETE_SCHED] [int] NOT NULL DEFAULT 365, " +
        " CONSTRAINT [PK_FORUM_FORUM] PRIMARY KEY CLUSTERED  " +
        "( [FORUM_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_GROUP_NAMES = "CREATE TABLE [FORUM_GROUP_NAMES]( " +
        "	[GROUP_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[GROUP_NAME] [nvarchar](50) NULL, " +
        "	[GROUP_DESCRIPTION] [nvarchar](255) NULL, " +
        "	[GROUP_ICON] [nvarchar](255) NULL, " +
        "	[GROUP_IMAGE] [nvarchar](255) NULL, " +
        " CONSTRAINT [PK_FORUM_GROUP_NAMES] PRIMARY KEY CLUSTERED  " +
        "( [GROUP_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";


        const string FORUM_GROUPS = "CREATE TABLE [FORUM_GROUPS]( " +
        "	[GROUP_KEY] [int] IDENTITY(1,1) NOT NULL, " +
        "	[GROUP_ID] [int] NULL, " +
        "	[GROUP_CATID] [int] NULL, " +
        " CONSTRAINT [PK_FORUM_GROUPS] PRIMARY KEY CLUSTERED  " +
        "( [GROUP_KEY] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";


        const string FORUM_MEMBERS = "CREATE TABLE [FORUM_MEMBERS]( " +
        "	[MEMBER_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[M_STATUS] [smallint] NULL, " +
        "	[M_NAME] [nvarchar](75) NULL, " +
        "	[M_USERNAME] [nvarchar](150) NULL, " +
        "	[M_PASSWORD] [nvarchar](65) NULL, " +
        "	[M_EMAIL] [nvarchar](50) NULL, " +
        "	[M_COUNTRY] [nvarchar](50) NULL, " +
        "	[M_HOMEPAGE] [nvarchar](255) NULL, " +
        "	[M_SIG] [nvarchar](max) NULL, " +
        "	[M_VIEW_SIG] [smallint] NULL, " +
        "	[M_SIG_DEFAULT] [smallint] NULL, " +
        "	[M_DEFAULT_VIEW] [int] NULL, " +
        "	[M_LEVEL] [smallint] NULL, " +
        "	[M_AIM] [nvarchar](150) NULL, " +
        "	[M_ICQ] [nvarchar](150) NULL, " +
        "	[M_MSN] [nvarchar](150) NULL, " +
        "	[M_YAHOO] [nvarchar](150) NULL, " +
        "	[M_POSTS] [int] NULL, " +
        "	[M_DATE] [nvarchar](14) NULL, " +
        "	[M_LASTHEREDATE] [nvarchar](14) NULL, " +
        "	[M_LASTPOSTDATE] [nvarchar](14) NULL, " +
        "	[M_TITLE] [nvarchar](50) NULL, " +
        "	[M_SUBSCRIPTION] [smallint] NULL, " +
        "	[M_HIDE_EMAIL] [smallint] NULL, " +
        "	[M_RECEIVE_EMAIL] [smallint] NULL, " +
        "	[M_LAST_IP] [nvarchar](50) NULL, " +
        "	[M_IP] [nvarchar](50) NULL, " +
        "	[M_FIRSTNAME] [nvarchar](100) NULL, " +
        "	[M_LASTNAME] [nvarchar](100) NULL, " +
        "	[M_OCCUPATION] [nvarchar](255) NULL, " +
        "	[M_SEX] [nvarchar](50) NULL, " +
        "	[M_AGE] [nvarchar](10) NULL, " +
        "	[M_DOB] [nvarchar](8) NULL, " +
        "	[M_HOBBIES] [nvarchar](max) NULL, " +
        "	[M_LNEWS] [nvarchar](max) NULL, " +
        "	[M_QUOTE] [nvarchar](max) NULL, " +
        "	[M_BIO] [nvarchar](max) NULL, " +
        "	[M_MARSTATUS] [nvarchar](100) NULL, " +
        "	[M_LINK1] [nvarchar](255) NULL, " +
        "	[M_LINK2] [nvarchar](255) NULL, " +
        "	[M_CITY] [nvarchar](100) NULL, " +
        "	[M_STATE] [nvarchar](100) NULL, " +
        "	[M_PHOTO_URL] [nvarchar](255) NULL, " +
        "	[M_KEY] [nvarchar](32) NULL, " +
        "	[M_NEWEMAIL] [nvarchar](50) NULL, " +
        "	[M_PWKEY] [nvarchar](32) NULL, " +
        "	[M_SHA256] [smallint] NULL, " +
        "	[M_ALLOWEMAIL] [smallint] NULL, " +
        " CONSTRAINT [PK_FORUM_MEMBERS] PRIMARY KEY CLUSTERED  " +
        "( [MEMBER_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_MODERATOR = "CREATE TABLE [FORUM_MODERATOR]( " +
        "	[MOD_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[FORUM_ID] [int] NULL, " +
        "	[MEMBER_ID] [int] NULL, " +
        "	[MOD_TYPE] [smallint] NULL, " +
        " CONSTRAINT [PK_FORUM_MODERATOR] PRIMARY KEY CLUSTERED  " +
        "( [MOD_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_NAMEFILTER = "CREATE TABLE [FORUM_NAMEFILTER]( " +
        "	[N_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[N_NAME] [nvarchar](75) NULL, " +
        " CONSTRAINT [PK_FORUM_NAMEFILTER] PRIMARY KEY CLUSTERED  " +
        "( " +
        "	[N_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_REPLY = "CREATE TABLE [FORUM_REPLY]( " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] NOT NULL, " +
        "	[TOPIC_ID] [int] NOT NULL, " +
        "	[REPLY_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[R_MAIL] [smallint] NULL, " +
        "	[R_AUTHOR] [int] NULL, " +
        "	[R_MESSAGE] [nvarchar](max) NULL, " +
        "	[R_DATE] [nvarchar](14) NULL, " +
        "	[R_IP] [nvarchar](50) NULL, " +
        "	[R_STATUS] [smallint] NULL, " +
        "	[R_LAST_EDIT] [nvarchar](14) NULL, " +
        "	[R_LAST_EDITBY] [int] NULL, " +
        "	[R_SIG] [smallint] NULL, " +
        " CONSTRAINT [PK_FORUM_REPLY] PRIMARY KEY CLUSTERED  " +
        "( [REPLY_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_SUBSCRIPTIONS = "CREATE TABLE [FORUM_SUBSCRIPTIONS]( " +
        "	[SUBSCRIPTION_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[MEMBER_ID] [int] NOT NULL, " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[TOPIC_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] NOT NULL, " +
        " CONSTRAINT [PK_FORUM_SUBSCRIPTIONS] PRIMARY KEY CLUSTERED  " +
        "( [SUBSCRIPTION_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_TOPICS = "CREATE TABLE [FORUM_TOPICS]( " +
        "	[CAT_ID] [int] NOT NULL, " +
        "	[FORUM_ID] [int] NOT NULL, " +
        "	[TOPIC_ID] [int] IDENTITY(1,1) NOT NULL, " +
        "	[T_STATUS] [smallint] NULL, " +
        "	[T_MAIL] [smallint] NULL, " +
        "	[T_SUBJECT] [nvarchar](100) NULL, " +
        "	[T_MESSAGE] [nvarchar](max) NULL, " +
        "	[T_AUTHOR] [int] NULL, " +
        "	[T_REPLIES] [int] NULL, " +
        "	[T_UREPLIES] [int] NULL, " +
        "	[T_VIEW_COUNT] [int] NULL, " +
        "	[T_LAST_POST] [nvarchar](14) NULL, " +
        "	[T_DATE] [nvarchar](14) NULL, " +
        "	[T_LAST_POSTER] [int] NULL, " +
        "	[T_IP] [nvarchar](50) NULL, " +
        "	[T_LAST_POST_AUTHOR] [int] NULL, " +
        "	[T_LAST_POST_REPLY_ID] [int] NULL, " +
        "	[T_ARCHIVE_FLAG] [int] NULL, " +
        "	[T_LAST_EDIT] [nvarchar](14) NULL, " +
        "	[T_LAST_EDITBY] [int] NULL, " +
        "	[T_STICKY] [smallint] NULL, " +
        "	[T_SIG] [smallint] NULL, " +
        " CONSTRAINT [PK_FORUM_TOPICS] PRIMARY KEY CLUSTERED  " +
        "( [TOPIC_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";

        const string FORUM_TOTALS = "CREATE TABLE [FORUM_TOTALS]( " +
        "	[COUNT_ID] [smallint] IDENTITY(1,1) NOT NULL, " +
        "	[P_COUNT] [int] NULL, " +
        "	[P_A_COUNT] [int] NULL, " +
        "	[T_COUNT] [int] NULL, " +
        "	[T_A_COUNT] [int] NULL, " +
        "	[U_COUNT] [int] NULL, " +
        " CONSTRAINT [PK_FORUM_TOTALS] PRIMARY KEY CLUSTERED  " +
        "( [COUNT_ID] ASC " +
        ")WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY] " +
        ") ON [PRIMARY];";
    }
}