using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Profile;
using Snitz.Entities;
using Snitz.Membership.IDal;
using Snitz.SQLServerDAL;
using SnitzConfig;
using ProfileInfo = System.Web.Profile.ProfileInfo;

namespace Snitz.Membership.SQLServerDAL
{
    public class Profile : IProfile
    {
        public string TableName { get; set; }
        private int _commandTimeout;
        private const string S_LEGAL_CHARS = "_@#$";
        private int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        private struct ProfileColumnData
        {
            public string ColumnName;
            public SettingsProperty PropertyValue;
            public object Value;
            public SqlDbType DataType;

            public ProfileColumnData(string col, SettingsProperty pv, object val, SqlDbType type)
            {
                EnsureValidTableOrColumnName(col);
                ColumnName = col;
                PropertyValue = pv;
                Value = val;
                DataType = type;
            }
        }
        private static void EnsureValidTableOrColumnName(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                if (!Char.IsLetterOrDigit(name[i]) && S_LEGAL_CHARS.IndexOf(name[i]) == -1)
                    throw new ProviderException("Table and column names cannot contain: " + name[i]);
            }
        }
        private void GetProfileDataFromTable(SettingsPropertyCollection properties, SettingsPropertyValueCollection svc, string username, SqlConnection conn)
        {
            List<ProfileColumnData> columnData = new List<ProfileColumnData>(properties.Count);
            StringBuilder commandText = new StringBuilder("SELECT u.MEMBER_ID");
            SqlCommand cmd = new SqlCommand(String.Empty, conn);

            int columnCount = 0;
            foreach (SettingsProperty prop in properties)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(prop);
                if (prop.PropertyType == typeof(List<SnitzLink>))
                {
                    prop.ThrowOnErrorDeserializing = true;
                    prop.SerializeAs = SettingsSerializeAs.Xml;
                    value.Deserialized = false;
                }
                svc.Add(value);
                string persistenceData = prop.Attributes["CustomProviderData"] as string;
                // If we can't find the table/column info we will ignore this data
                if (String.IsNullOrEmpty(persistenceData))
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string[] chunk = persistenceData.Split(new char[] { ';' });
                if (chunk.Length != 2)
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string columnName = chunk[0];
                // REVIEW: Should we ignore case?
                SqlDbType datatype = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

                columnData.Add(new ProfileColumnData(columnName, prop, null /* not needed for get */, datatype));
                commandText.Append(", ");
                commandText.Append("t." + columnName);
                ++columnCount;
            }

            commandText.AppendFormat(" FROM {0} t, {1}MEMBERS u WHERE ",TableName,Config.MemberTablePrefix).AppendLine();
            commandText.Append("u.M_NAME = @Username AND t.UserID = u.MEMBER_ID");
            cmd.CommandText = commandText.ToString();
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);
            SqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
                //If no row exists in the database, then the default Profile values
                //from configuration are used.
                if (reader.Read())
                {
                    svc.Clear();
                    int userId = reader.GetInt32(0);
                    for (int i = 0; i < columnData.Count; ++i)
                    {
                        object val = reader.GetValue(i + 1);
                        ProfileColumnData colData = columnData[i];
                        SettingsPropertyValue propValue = new SettingsPropertyValue(colData.PropertyValue);

                        //Only initialize a SettingsPropertyValue for non-null values
                        //if (!(val is DBNull || val == null))
                        //{
                        propValue.IsDirty = false;
                        if (propValue.Property.SerializeAs == SettingsSerializeAs.Xml)
                        {
                            propValue.Deserialized = false;
                            object test = "";
                            if (!val.Equals(test))
                                propValue.SerializedValue = val;
                        }
                        else
                        {
                            propValue.PropertyValue = val;
                            propValue.Deserialized = true;
                        }


                        svc.Add(propValue);
                        //}
                    }

                    // need to close reader before we try to update the user
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }

                    //UpdateLastActivityDate(conn, userId);
                }
                else
                {
                    object val = GetBookMarkModValues(username);
                    ProfileColumnData colData = columnData.Find(c => c.ColumnName == "BookMarks");
                    SettingsPropertyValue propValue = new SettingsPropertyValue(colData.PropertyValue);
                    propValue.IsDirty = false;
                    if (propValue.Property.SerializeAs == SettingsSerializeAs.Xml)
                    {
                        if (propValue.Name == "BookMarks")
                        {
                            svc.Remove("BookMarks");
                            propValue.Deserialized = false;
                            object test = "";
                            if (!val.Equals(test))
                                propValue.SerializedValue = val;

                            svc.Add(propValue);
                        }
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }
        private ProfileInfoCollection GetProfilesForQuery(SqlParameter[] insertArgs, int pageIndex, int pageSize, StringBuilder insertQuery, out int totalRecords)
        {
            if (pageIndex < 0)
                throw new ArgumentException("pageIndex");
            if (pageSize < 1)
                throw new ArgumentException("pageSize");

            long lowerBound = (long)pageIndex * pageSize;
            long upperBound = lowerBound + pageSize - 1;
            if (upperBound > Int32.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize");
            }

            SqlConnection conn = null;
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(SqlHelper.ConnString);
                conn.Open();

                StringBuilder cmdStr = new StringBuilder(200);
                // Create a temp table TO store the select results
                cmd = new SqlCommand("CREATE TABLE #PageIndexForProfileUsers(IndexId int IDENTITY (0, 1) NOT NULL, UserId int)", conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                insertQuery.Append(" ORDER BY UserName");
                cmd = new SqlCommand(insertQuery.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;
                if (insertArgs != null)
                {
                    foreach (SqlParameter arg in insertArgs)
                        cmd.Parameters.Add(arg);
                }

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                cmdStr = new StringBuilder(200);
                cmdStr.AppendFormat("SELECT u.M_NAME, u.M_LASTHEREDATE, p.M_LASTUPDATED FROM {0}MEMBERS u, ",Config.MemberTablePrefix).AppendLine(TableName);
                cmdStr.Append(" p, #PageIndexForProfileUsers i WHERE u.MEMBER_ID = p.UserId AND p.UserId = i.UserId AND i.IndexId >= ");
                cmdStr.Append(lowerBound).Append(" AND i.IndexId <= ").Append(upperBound);
                cmd = new SqlCommand(cmdStr.ToString(), conn);
                cmd.CommandTimeout = CommandTimeout;

                reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                ProfileInfoCollection profiles = new ProfileInfoCollection();
                while (reader.Read())
                {
                    string username;
                    DateTime dtLastActivity, dtLastUpdated = DateTime.UtcNow;
                    bool isAnon;

                    username = reader.GetString(0);
                    isAnon = reader.GetBoolean(1);
                    dtLastActivity = DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc);
                    dtLastUpdated = DateTime.SpecifyKind(reader.GetDateTime(3), DateTimeKind.Utc);
                    profiles.Add(new ProfileInfo(username, isAnon, dtLastActivity, dtLastUpdated, 0));
                }
                totalRecords = profiles.Count;

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                cmd.Dispose();

                // Cleanup, REVIEW: should move to finally?
                cmd = new SqlCommand("DROP TABLE #PageIndexForProfileUsers", conn);
                cmd.ExecuteNonQuery();

                return profiles;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();

                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }
        private static SqlParameter CreateInputParam(string paramName, SqlDbType dbType, object objValue)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            if (objValue == null)
                objValue = String.Empty;
            param.Value = objValue;
            return param;
        }

        private static SqlParameter CreateOutputParam(string paramName, SqlDbType dbType, int size)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            param.Direction = ParameterDirection.Output;
            param.Size = size;
            return param;
        }
        private StringBuilder GenerateTempInsertQueryForGetProfiles(ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            cmdStr.Append("INSERT INTO #PageIndexForProfileUsers (UserId) ");
            cmdStr.AppendFormat("SELECT u.MEMBER_ID FROM {0}MEMBERS u, ",Config.MemberTablePrefix).AppendLine(TableName);
            cmdStr.Append(" p WHERE ");
            cmdStr.Append("u.MEMBER_ID = p.UserId");
            return cmdStr;
        }

        private string GenerateQuery(bool delete, ProfileAuthenticationOption authenticationOption)
        {
            StringBuilder cmdStr = new StringBuilder(200);
            cmdStr.Append(delete ? "DELETE FROM " : "SELECT COUNT(*) FROM ");
            cmdStr.Append(TableName);
            cmdStr.AppendFormat(" WHERE UserId IN (SELECT u.MEMBER_ID FROM {0}MEMBERS u WHERE ",Config.MemberTablePrefix).AppendLine();
            cmdStr.Append(" (u.M_LASTHEREDATE <= @InactiveSinceDate)");
            cmdStr.Append(")");
            return cmdStr.ToString();
        }


        public SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection svc = new SettingsPropertyValueCollection();

            if (collection == null || collection.Count < 1 || context == null)
                return svc;

            string username = (string)context["UserName"];
            if (String.IsNullOrEmpty(username))
                return svc;

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(SqlHelper.ConnString);
                conn.Open();

                GetProfileDataFromTable(collection, svc, username, conn);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return svc;
        }

        public void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            string username = (string)context["UserName"];
            bool userIsAuthenticated = (bool)context["IsAuthenticated"];

            if (String.IsNullOrEmpty(username) || collection.Count < 1)
                return;

            SqlConnection conn = null;
            SqlDataReader reader = null;
            SqlCommand cmd = null;
            try
            {
                bool anyItemsToSave = false;

                // First make sure we have at least one item to save
                foreach (SettingsPropertyValue pp in collection)
                {
                    if (pp.IsDirty)
                    {
                        if (!userIsAuthenticated)
                        {
                            bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                            if (!allowAnonymous)
                                continue;
                        }
                        anyItemsToSave = true;
                        break;
                    }
                }

                if (!anyItemsToSave)
                    return;

                conn = new SqlConnection(SqlHelper.ConnString);
                conn.Open();

                List<ProfileColumnData> columnData = new List<ProfileColumnData>(collection.Count);

                foreach (SettingsPropertyValue pp in collection)
                {
                    if (!userIsAuthenticated)
                    {
                        bool allowAnonymous = (bool)pp.Property.Attributes["AllowAnonymous"];
                        if (!allowAnonymous)
                            continue;
                    }

                    //Normal logic for original SQL provider
                    //if (!pp.IsDirty && pp.UsingDefaultValue) // Not fetched from DB and not written to

                    //Can eliminate unnecessary updates since we are using a table though
                    if (!pp.IsDirty)
                        continue;

                    string persistenceData = pp.Property.Attributes["CustomProviderData"] as string;
                    // If we can't find the table/column info we will ignore this data
                    if (String.IsNullOrEmpty(persistenceData))
                    {
                        // REVIEW: Perhaps we should throw instead?
                        continue;
                    }
                    string[] chunk = persistenceData.Split(new char[] { ';' });
                    if (chunk.Length != 2)
                    {
                        // REVIEW: Perhaps we should throw instead?
                        continue;
                    }
                    string columnName = chunk[0];
                    // REVIEW: Should we ignore case?
                    SqlDbType datatype = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

                    object value = null;

                    // REVIEW: Is this handling null case correctly?
                    if (pp.Deserialized && pp.PropertyValue == null)
                    { // is value null?
                        value = DBNull.Value;
                    }
                    else
                    {
                        if (pp.Deserialized && (pp.Property.PropertyType != typeof(List<SnitzLink>)))
                            value = pp.PropertyValue;
                        else
                            value = pp.SerializedValue ?? pp.PropertyValue;
                    }

                    // REVIEW: Might be able to ditch datatype
                    columnData.Add(new ProfileColumnData(columnName, null, value, datatype));
                }

                // Figure out userid, if we don't find a userid, go ahead and create a user in the aspnetUsers table
                int userId = 0;
                cmd = new SqlCommand("SELECT u.MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS u WHERE LOWER(u.M_NAME) = LOWER(@Username)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Username", username);
                try
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userId = reader.GetInt32(0);
                    }
                    else
                    {
                        reader.Close();
                        cmd.Dispose();
                        reader = null;

                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader = null;
                    }
                    cmd.Dispose();
                }

                // Figure out if the row already exists in the table and use appropriate SELECT/UPDATE
                cmd = new SqlCommand(String.Empty, conn);
                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(TableName);
                sqlCommand.Append(" WHERE UserId = @UserId) ");
                cmd.Parameters.AddWithValue("@UserId", userId);

                // Build up strings used in the query
                StringBuilder columnStr = new StringBuilder();
                StringBuilder valueStr = new StringBuilder();
                StringBuilder setStr = new StringBuilder();
                int count = 0;
                foreach (ProfileColumnData data in columnData)
                {
                    columnStr.Append(", ");
                    valueStr.Append(", ");
                    columnStr.Append(data.ColumnName);
                    string valueParam = "@Value" + count;
                    valueStr.Append(valueParam);
                    cmd.Parameters.AddWithValue(valueParam, data.Value);

                    // REVIEW: Can't update Timestamps?
                    if (data.DataType != SqlDbType.Timestamp)
                    {
                        if (count > 0)
                        {
                            setStr.Append(",");
                        }
                        setStr.Append(data.ColumnName);
                        setStr.Append("=");
                        setStr.Append(valueParam);
                    }

                    ++count;
                }

                sqlCommand.Append("BEGIN UPDATE ").Append(TableName).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = '").Append(userId).Append("'");

                sqlCommand.Append("END ELSE BEGIN INSERT ").Append(TableName).Append(" (UserId").Append(columnStr.ToString());
                sqlCommand.Append(") VALUES ('").Append(userId).Append("'").Append(valueStr.ToString()).Append(") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();

            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (cmd != null)
                    cmd.Dispose();
                if (conn != null)
                    conn.Close();
            }
        }

        public int DeleteProfiles(string[] usernames)
        {
            if (usernames == null || usernames.Length < 1)
            {
                return 0;
            }

            int numProfilesDeleted = 0;
            bool beginTranCalled = false;
            try
            {
                SqlConnection conn = null;
                try
                {
                    conn = new SqlConnection(SqlHelper.ConnString);
                    conn.Open();

                    SqlCommand cmd;
                    int numUsersRemaing = usernames.Length;
                    while (numUsersRemaing > 0)
                    {
                        cmd = new SqlCommand(String.Empty, conn);
                        cmd.Parameters.AddWithValue("UserName0", usernames[usernames.Length - numUsersRemaing]);
                        StringBuilder allUsers = new StringBuilder("@UserName0");
                        numUsersRemaing--;

                        int userIndex = 1;
                        for (int iter = usernames.Length - numUsersRemaing; iter < usernames.Length; iter++)
                        {
                            // REVIEW: Should we check length of command string instead of parameter lengths?
                            if (allUsers.Length + usernames[iter].Length + 3 >= 4000)
                                break;
                            string userNameParam = "UserName" + userIndex;
                            allUsers.Append(",");
                            allUsers.Append("@" + userNameParam);
                            cmd.Parameters.AddWithValue(userNameParam, usernames[iter]);
                            numUsersRemaing--;
                            ++userIndex;
                        }

                        // We don't need to start a transaction if we can finish this in one sql command
                        if (!beginTranCalled && numUsersRemaing > 0)
                        {
                            SqlCommand beginCmd = new SqlCommand("BEGIN TRANSACTION", conn);
                            beginCmd.ExecuteNonQuery();
                            beginTranCalled = true;
                        }


                        cmd.CommandText = "DELETE FROM " + TableName + " WHERE UserId IN ( SELECT u.MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS u WHERE  u.M_NAME IN (" + allUsers.ToString() + "))";
                        cmd.CommandTimeout = CommandTimeout;
                        cmd.CommandText = GetQueryFromCommand(cmd);
                        cmd.Parameters.Clear();
                        numProfilesDeleted += cmd.ExecuteNonQuery();
                    }

                    if (beginTranCalled)
                    {
                        cmd = new SqlCommand("COMMIT TRANSACTION", conn);
                        cmd.ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                }
                catch
                {
                    if (beginTranCalled)
                    {
                        SqlCommand cmd = new SqlCommand("ROLLBACK TRANSACTION", conn);
                        cmd.ExecuteNonQuery();
                        beginTranCalled = false;
                    }
                    throw;
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
            catch
            {
                throw;
            }
            return numProfilesDeleted;
        }

        public string GetQueryFromCommand(object cmd)
        {
            string CommandTxt = ((SqlCommand)cmd).CommandText;


            foreach (SqlParameter parms in ((SqlCommand)cmd).Parameters)
            {
                string val = String.Empty;
                if (parms.DbType.Equals(DbType.String) || parms.DbType.Equals(DbType.DateTime))
                    val = "'" + Convert.ToString(parms.Value).Replace(@"\", @"\\").Replace("'", @"\'") + "'";
                if (parms.DbType.Equals(DbType.Int16) || parms.DbType.Equals(DbType.Int32) || parms.DbType.Equals(DbType.Int64) || parms.DbType.Equals(DbType.Decimal) || parms.DbType.Equals(DbType.Double))
                    val = Convert.ToString(parms.Value);
                string paramname = "@" + parms.ParameterName;
                CommandTxt = CommandTxt.Replace(paramname, val);
            }
            return (CommandTxt);
        }

        public ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND LOWER(u.M_NAME) LIKE LOWER(@UserName) AND u.M_LASTHEREDATE <= @InactiveSinceDate");
            SqlParameter[] args = new SqlParameter[2];
            args[0] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            args[1] = CreateInputParam("@UserName", SqlDbType.NVarChar, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);

        }

        public ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch,
            int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND LOWER(u.M_NAME) LIKE LOWER(@UserName)");
            SqlParameter[] args = new SqlParameter[1];
            args[0] = CreateInputParam("@UserName", SqlDbType.NVarChar, usernameToMatch);
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);

        }

        public ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption,
            DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            insertQuery.Append(" AND u.M_LASTHEREDATE <= @InactiveSinceDate");
            SqlParameter[] args = new SqlParameter[1];
            args[0] = CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime());
            return GetProfilesForQuery(args, pageIndex, pageSize, insertQuery, out totalRecords);

        }

        public ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize,
            out int totalRecords)
        {
            StringBuilder insertQuery = GenerateTempInsertQueryForGetProfiles(authenticationOption);
            return GetProfilesForQuery(null, pageIndex, pageSize, insertQuery, out totalRecords);

        }

        public int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(SqlHelper.ConnString);
                conn.Open();

                cmd = new SqlCommand(GenerateQuery(false, authenticationOption), conn);
                cmd.CommandTimeout = CommandTimeout;
                cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime()));

                object o = cmd.ExecuteScalar();
                if (o == null || !(o is int))
                    return 0;
                return (int)o;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
        }

        public int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            try
            {
                SqlConnection conn = null;
                SqlCommand cmd = null;
                try
                {
                    conn = new SqlConnection(SqlHelper.ConnString);
                    conn.Open();

                    cmd = new SqlCommand(GenerateQuery(true, authenticationOption), conn);
                    cmd.CommandTimeout = CommandTimeout;
                    cmd.Parameters.Add(CreateInputParam("@InactiveSinceDate", SqlDbType.DateTime, userInactiveSinceDate.ToUniversalTime()));

                    return cmd.ExecuteNonQuery();
                }
                finally
                {
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                    if (conn != null)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public int DeleteProfiles(ProfileInfoCollection profiles)
        {
            if (profiles == null)
            {
                throw new ArgumentNullException("profiles");
            }

            if (profiles.Count < 1)
            {
                throw new ArgumentException("Profiles collection is empty");
            }

            string[] usernames = new string[profiles.Count];

            int iter = 0;
            foreach (ProfileInfo profile in profiles)
            {
                usernames[iter++] = profile.UserName;
            }

            return DeleteProfiles(usernames);
        }

        public string GetBookMarkModValues(string username)
        {

            string returnvalue = "";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT");
            sql.AppendLine("ROW_NUMBER() OVER (ORDER BY Bookmark.B_MEMBERID)-1 AS ID,");
            sql.AppendLine("SnitzLink.T_SUBJECT AS Name,'~/Content/Forums/topic.aspx?TOPIC=' + CAST(SnitzLink.TOPIC_ID AS varchar) AS Url");
            sql.AppendLine("FROM FORUM_BOOKMARKS Bookmark INNER JOIN");
            sql.AppendLine("FORUM_TOPICS SnitzLink ON Bookmark.B_TOPICID = SnitzLink.TOPIC_ID");
            sql.AppendLine("WHERE Bookmark.B_MEMBERID = (SELECT MEMBER_ID FROM FORUM_MEMBERS WHERE M_NAME=@Username)");
            sql.AppendLine("FOR XML PATH('SnitzLink')");

            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), new SqlParameter("@Username", SqlDbType.VarChar) { Value = username }))
            {
                while (rdr.Read())
                {
                    returnvalue = rdr.SafeGetString(0, "");
                }
            }
            if (returnvalue != "")
            {
                returnvalue = "<?xml version=\"1.0\" encoding=\"utf-16\"?><ArrayOfSnitzLink xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + returnvalue + "</ArrayOfSnitzLink>";
            }
            return returnvalue.Replace("><",">" + Environment.NewLine + "<");
        }
    }
}
