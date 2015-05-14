using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class ForumMod : IForumMod
    {
        public ModInfo GetById(int id)
        {
            string strSql = "SELECT MOD_ID,MOD_NAME,MOD_DESCRIPTION,MOD_VERSION,MOD_ENABLED,MOD_ROLES FROM " + Config.ForumTablePrefix + "MODS_CONFIG WHERE MOD_ID = @ModId";
            ModInfo modInfo = null;
            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@ModId", SqlDbType.Int) { Value = id }))
            {
                while (rdr.Read())
                {
                    modInfo = new ModInfo
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.SafeGetString(1),
                        Description = rdr.SafeGetString(2),
                        Version = new Version(rdr.SafeGetString(3)),
                        Enabled = rdr.GetBoolean(4),
                        Roles = rdr.SafeGetString(5),
                        Settings = GetModSettings(id)
                    };

                }
            }
            return modInfo;
        }

        private Hashtable GetModSettings(int modid)
        {
            Dictionary<string,string> settings = new Dictionary<string, string>();
            string strSql = "SELECT MOD_SETTING,MOD_VALUE FROM " + Config.FilterTablePrefix + "MODS_SETTING WHERE MOD_ID=@ModId";
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql,
                    new SqlParameter("@ModId", SqlDbType.Int) {Value = modid}))
            {
                while (rdr.Read())
                {
                    settings.Add(rdr.SafeGetString(0),rdr.SafeGetString(1));
                }

            }
            return new Hashtable(settings);
        }

        public IEnumerable<ModInfo> GetByName(string name)
        {
            string strSql = "SELECT MOD_ID,MOD_NAME,MOD_DESCRIPTION,MOD_VERSION,MOD_ENABLED,MOD_ROLES FROM " + Config.ForumTablePrefix + "MODS_CONFIG WHERE MOD_Name = @ModName";
            ModInfo modInfo = null;
            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@ModName", SqlDbType.VarChar) { Value = name }))
            {
                while (rdr.Read())
                {
                    int modid = rdr.GetInt32(0);
                    modInfo = new ModInfo
                    {
                        Id = modid,
                        Name = rdr.SafeGetString(1),
                        Description = rdr.SafeGetString(2),
                        Version = new Version(rdr.SafeGetString(3)),
                        Enabled = rdr.SafeGetInt16(4) == 1,
                        Roles = rdr.SafeGetString(5),
                        Settings = GetModSettings(modid)
                    };
                }
            }
            return new[] {modInfo};
        }

        public int Add(ModInfo modinfo)
        {
            string insertSql = "INSERT INTO " + Config.FilterTablePrefix +
                "MODS_CONFIG (MOD_NAME,MOD_DESCRIPTION,MOD_VERSION,MOD_ENABLED,MOD_ROLES) VALUES (@Name,@Description,@Version,@Enabled,@Roles ); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = modinfo.Name},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = modinfo.Description},
                    new SqlParameter("@Version", SqlDbType.NVarChar) {Value = modinfo.Version.ToString()},
                    new SqlParameter("@Enabled", SqlDbType.Bit) {Value = modinfo.Enabled},
                    new SqlParameter("@Roles", SqlDbType.NVarChar) {Value = (object)modinfo.Roles??DBNull.Value}
                };
            int modid = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, insertSql, parms.ToArray()));

            foreach (DictionaryEntry setting in modinfo.Settings)
            {
                string insSetting = "INSERT INTO " + Config.FilterTablePrefix + "MODS_SETTING (MOD_ID,MOD_SETTING,MOD_VALUE) VALUES (@Id,@Setting,@Value);";
                List<SqlParameter> sparms = new List<SqlParameter>
                {
                    new SqlParameter("@Id", SqlDbType.Int) {Value = modid},
                    new SqlParameter("@Setting", SqlDbType.NVarChar) {Value = setting.Key},
                    new SqlParameter("@Value", SqlDbType.NVarChar) {Value = setting.Value}
                };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, insSetting, sparms.ToArray());
            }

            return modid;
        }

        public void Update(ModInfo modinfo)
        {
            StringBuilder updSql = new StringBuilder("UPDATE ");
            updSql.AppendFormat("{0}MODS_CONFIG SET",Config.FilterTablePrefix).AppendLine();
            updSql.AppendLine("MOD_NAME=@Name,");
            updSql.AppendLine("MOD_DESCRIPTION=@Description,");
            updSql.AppendLine("MOD_VERSION=@Version,");
            updSql.AppendLine("MOD_ENABLED=@Enabled,");
            updSql.AppendLine("MOD_ROLES=@Roles");
            updSql.AppendLine("WHERE MOD_ID=@ModId;");
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@ModId", SqlDbType.Int) {Value = modinfo.Id},
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = modinfo.Name},
                    new SqlParameter("@Description", SqlDbType.NVarChar) {Value = modinfo.Description},
                    new SqlParameter("@Version", SqlDbType.NVarChar) {Value = modinfo.Version.ToString()},
                    new SqlParameter("@Enabled", SqlDbType.Bit) {Value = modinfo.Enabled},
                    new SqlParameter("@Roles", SqlDbType.NVarChar) {Value = (object)modinfo.Roles??DBNull.Value}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updSql.ToString(), parms.ToArray());

            foreach (DictionaryEntry setting in modinfo.Settings)
            {
                updSql.Length = 0;
                updSql.AppendFormat("UPDATE {0}MODS_SETTING SET", Config.FilterTablePrefix).AppendLine();
                updSql.AppendLine("MOD_Value=@Value");
                updSql.AppendLine("WHERE MOD_ID=@ModId AND MOD_SETTING=@Setting;");
                List<SqlParameter> sparms = new List<SqlParameter>
                {
                    new SqlParameter("@ModId", SqlDbType.Int) {Value = modinfo.Id},
                    new SqlParameter("@Value", SqlDbType.NVarChar) {Value = setting.Value},
                    new SqlParameter("@Setting", SqlDbType.NVarChar) {Value = setting.Key}
                };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updSql.ToString(), sparms.ToArray());
            }

            

        }

        public void Delete(ModInfo modinfo)
        {
            int modid = modinfo.Id;

            string delSql = "DELETE FROM " + Config.FilterTablePrefix + "MODS_SETTING WHERE MOD_ID=@ModId; " +
                            "DELETE FROM " + Config.FilterTablePrefix + "MODS_CONFIG WHERE MOD_ID=@ModId; ";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, delSql, new SqlParameter("@Id", SqlDbType.Int) { Value = modid });
        }

        public void Dispose()
        {

        }

        public object GetModValue(int modid, string settingname)
        {
            string strSql = "SELECT MOD_VALUE FROM " + Config.FilterTablePrefix + "MODS_SETTING WHERE MOD_ID=@ModId AND MOD_SETTING=@Setting; ";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@ModId", SqlDbType.Int) {Value = modid},
                    new SqlParameter("@Setting", SqlDbType.NVarChar) {Value = settingname}
                };

            return SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

    }
}
