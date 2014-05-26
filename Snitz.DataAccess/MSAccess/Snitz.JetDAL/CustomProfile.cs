using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class CustomProfile : ICustomProfile
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
            List<ProfileColumn> columns = new List<ProfileColumn>();

            using (var con = new OleDbConnection(SqlHelper.ConnString))
            {
                con.Open();
                using (var cmd = new OleDbCommand("select * from " + Config.MemberTablePrefix + "ProfileData", con))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    var table = reader.GetSchemaTable();
                    var nameCol = table.Columns["ColumnName"];

                    foreach (DataRow row in table.Rows)
                    {
                        var test = row[nameCol];
                        columns.Add(new ProfileColumn()
                        {
                            ColumnName = row[nameCol].ToString(),
                            AllowNull =(bool) row["AllowDBNull"],
                            DataType = row["DataType"].ToString(),
                            DefaultValue = nameCol.DefaultValue.ToString(),
                            Precision = row["NumericPrecision"].ToString()
                        });
                    }
                }
            }

            return columns;
        }
    }
}
