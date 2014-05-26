using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class CustomProfile :ICustomProfile
    {
        public bool AddColumn(ProfileColumn column)
        {
            string strSql = String.Format("ALTER TABLE {0}ProfileData ADD {1} {2} DEFAULT {3} {4}",Config.MemberTablePrefix, column.ColumnName, column.DataType, column.DefaultValue, column.AllowNull ? "NULL" : "NOT NULL");
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql + column, null);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool DropColumn(string column)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "ALTER TABLE " + Config.MemberTablePrefix + "ProfileData DROP COLUMN " + column, null);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<ProfileColumn> GetColumns()
        {
            string strSql = "SELECT COLUMN_NAME,IS_NULLABLE,DATA_TYPE,COLUMN_DEFAULT,COALESCE(CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProfileData'";
            List<ProfileColumn> columns = new List<ProfileColumn>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    columns.Add(new ProfileColumn(){ColumnName = rdr.GetString(0),
                        AllowNull = rdr.GetString(1) == "YES",
                        DataType = rdr.GetString(2),
                        DefaultValue = rdr.SafeGetString(3),
                        Precision = rdr.GetInt32(4).ToString()
                    });

                }
            }
            return columns;
        }
    }
}
