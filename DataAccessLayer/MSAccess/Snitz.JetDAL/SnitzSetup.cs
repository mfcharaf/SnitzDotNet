using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzCommon;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class SnitzSetup : ISetup
    {
        public string CheckVersion()
        {
            return "access";
        }

        public string ExecuteScript(string script)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, script, null);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

        public bool ChangeDbOwner()
        {
            return true;

        }

        /// <summary>
        /// Checks to see if a table exists in Database or not.
        /// </summary>
        /// <param name="tblName">Table name to check</param>
        /// <returns>Works with Access or SQL</returns>
        /// <remarks></remarks>
        public bool TableExists(string tblName)
        {
            return SqlHelper.TableExists(SqlHelper.ConnString, tblName);
        }

        /// <summary>
        /// Checks to see if a field exists in table or not.
        /// </summary>
        /// <param name="tblName">Table name to check in</param>
        /// <param name="fldName">Field name to check</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool FieldExists(string tblName, string fldName)
        {
            return SqlHelper.FieldExists(SqlHelper.ConnString, tblName, fldName);
        }

        public bool DatabaseExists()
        {

            const string strSql = "SELECT DB_ID(@Database) ";
            bool result = false;
            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder
                                                                   {
                                                                       ConnectionString = SqlHelper.ConnString
                                                                   };
            if (!builder.ContainsKey("initial catalog"))
                return true;

            string database = builder["initial catalog"] as string;
            builder["initial catalog"] = "master";

            var res = SqlHelper.ExecuteScalar(builder.ConnectionString, CommandType.Text, strSql, new OleDbParameter("@Database", OleDbType.VarChar) { Value = database });

            result = (res.ConvertDBNull() != DBNull.Value);

            return result;
        }

        public IEnumerable<ForumInfo> PrivateForums()
        {
            List<ForumInfo> forumlist = new List<ForumInfo>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT F.FORUM_ID,F.F_PRIVATEFORUMS,F_SUBJECT FROM " + Config.ForumTablePrefix + "FORUM F WHERE F.F_PRIVATEFORUMS > 0", null))
            {
                while (rdr.Read())
                {
                    ForumInfo forum = new ForumInfo { Id = rdr.GetInt32(0), PrivateForum = rdr.GetInt32(1), Subject = rdr.GetString(2) };
                    forumlist.Add(forum);
                }
                return forumlist;
            }

        }

        public string[] AllowedMembers(int forumid)
        {
            List<string> forumlist = new List<string>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT M.M_NAME FROM " + Config.ForumTablePrefix + "ALLOWED_MEMBERS FM LEFT OUTER JOIN " + Config.MemberTablePrefix + "MEMBERS M ON FM.MEMBER_ID = M.MEMBER_ID WHERE FM.FORUM_ID=@ForumId", new OleDbParameter("@ForumId", OleDbType.Numeric) { Value = forumid }))
            {
                while (rdr.Read())
                {
                    forumlist.Add(rdr.GetString(0));
                }
                return forumlist.ToArray();
            }
        }
    }
}
