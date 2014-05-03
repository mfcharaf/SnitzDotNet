using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.OLEDbDAL
{
    public class CustomProfile : ICustomProfile
    {
        public bool AddColumn(ProfileColumn column)
        {
            string strSql = String.Format("ALTER TABLE [ProfileData] ADD {0} {1} DEFAULT {2} {3}", column.ColumnName, column.DataType, column.DefaultValue, column.AllowNull ? "NULL" : "NOT NULL");
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
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "ALTER TABLE [ProfileData] DROP COLUMN " + column, null);
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
                using (var cmd = new OleDbCommand("select * from ProfileData", con))
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
            //string strSql = "SELECT COLUMN_NAME,IS_NULLABLE,DATA_TYPE,COLUMN_DEFAULT,COALESCE(CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProfileData'";
            
            //using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            //{
            //    while (rdr.Read())
            //    {
            //        columns.Add(new ProfileColumn()
            //        {
            //            ColumnName = rdr.GetString(0),
            //            AllowNull = rdr.GetString(1) == "YES",
            //            DataType = rdr.GetString(2),
            //            DefaultValue = rdr.SafeGetString(3),
            //            Precision = rdr.GetInt32(4).ToString()
            //        });

            //    }
            //}
            return columns;
        }
    }
}
