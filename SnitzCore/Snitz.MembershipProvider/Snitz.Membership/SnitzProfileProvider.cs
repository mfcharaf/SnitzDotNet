//------------------------------------------------------------------------------
// <copyright file="SqlProfileProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
/*
####################################################################################################################
##
## SnitzMembership - SnitzProfileProvider
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

    using System;
    using System.Web.Profile;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration.Provider;
    using System.Configuration;
    using Snitz.Membership.IDal;

public class SnitzProfileProvider : ProfileProvider
{
        public string OldValues { get; set; }
        private string _appName;
        private Guid _appId;
        private bool _appIdSet;
        private string _sqlConnectionString;
        private string _connStr = ConfigurationManager.ConnectionStrings["SnitzConnectionString"].ConnectionString;
        private int _commandTimeout;
        private string _table;

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");
            if (String.IsNullOrEmpty(name))
                name = "SqlTableProfileProvider";
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "SqlTableProfileProvider");
            }
            base.Initialize(name, config);

            string temp = config["connectionStringName"];
            if (String.IsNullOrEmpty(temp))
                throw new ProviderException("connectionStringName not specified");
            _connStr = ConfigurationManager.ConnectionStrings["SnitzConnectionString"].ConnectionString;
            if (String.IsNullOrEmpty(_connStr))
            {
                throw new ProviderException("connectionStringName not specified");
            }

            _appName = config["applicationName"];
            if (string.IsNullOrEmpty(_appName))
                _appName = "/";

            if (_appName.Length > 256)
            {
                throw new ProviderException("Application name too long");
            }

            _table = config["table"];
            if (string.IsNullOrEmpty(_table))
            {
                throw new ProviderException("No table specified");
            }
            EnsureValidTableOrColumnName(_table);

            string timeout = config["commandTimeout"];
            if (string.IsNullOrEmpty(timeout) || !Int32.TryParse(timeout, out _commandTimeout))
            {
                _commandTimeout = 30;
            }

            config.Remove("commandTimeout");
            config.Remove("connectionStringName");
            config.Remove("applicationName");
            config.Remove("table");
            if (config.Count > 0)
            {
                string attribUnrecognized = config.GetKey(0);
                if (!String.IsNullOrEmpty(attribUnrecognized))
                    throw new ProviderException("Unrecognized config attribute:" + attribUnrecognized);
            }
        }

        public override string ApplicationName
        {
            get { return _appName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("ApplicationName");
                if (value.Length > 256)
                {
                    throw new ProviderException("Application name too long");
                }
                _appName = value;
                _appIdSet = false;
            }
        }

        private Guid AppId
        {
            get
            {
                return new Guid();
            }
        }

        private const string S_LEGAL_CHARS = "_@#$";

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            SettingsPropertyValueCollection vc =  dal.GetPropertyValues(context, collection);
            return vc;
        }
        
        // Container struct for use in aggregating columns for queries
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

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            dal.SetPropertyValues(context, collection);
        }

        // Mangement APIs from ProfileProvider class

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.DeleteProfiles(profiles);
        }

        public override int DeleteProfiles(string[] usernames)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.DeleteProfiles(usernames);
        }

        public static string getQueryFromCommand(SqlCommand cmd)
        {
            string CommandTxt = cmd.CommandText;


            foreach (SqlParameter parms in cmd.Parameters)
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
        
        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.DeleteInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.GetNumberOfInactiveProfiles(authenticationOption, userInactiveSinceDate);
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.GetAllProfiles(authenticationOption, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.GetAllInactiveProfiles(authenticationOption, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.FindProfilesByUserName(authenticationOption, usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            IProfile dal = Snitz.Membership.Helpers.Factory<IProfile>.Create("Profile");
            dal.TableName = _table;
            return dal.FindInactiveProfilesByUserName(authenticationOption, usernameToMatch, userInactiveSinceDate, pageIndex, pageSize, out totalRecords);
        }

        // Private methods
        private static void EnsureValidTableOrColumnName(string name)
        {
            for (int i = 0; i < name.Length; ++i)
            {
                if (!Char.IsLetterOrDigit(name[i]) && S_LEGAL_CHARS.IndexOf(name[i]) == -1)
                    throw new ProviderException("Table and column names cannot contain: " + name[i]);
            }
        }

    }
