using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SnitzMembership;

namespace SnitzData
{
    public class ProfileColumn
    {
        public string ColumnName { get; set; }
        public bool AllowNull { get; set; }
        public string DataType { get; set; }
        public string DefaultValue { get; set; }
        public string Precision { get; set; }
    }
    public static class Profile
    {
        private static string connstr = ConfigurationManager.ConnectionStrings["ForumConnectionString"].ConnectionString;

        public static bool AddColumn(ProfileColumn column)
        {
            using (DbConnection connection = new SqlConnection(connstr))
            {
                string cmd = String.Format("alter table [ProfileData] add {0} {1} default {2} {3}",column.ColumnName,column.DataType,column.DefaultValue,column.AllowNull ? "NULL" : "NOT NULL");
                connection.Open();
                using (DbCommand command = new SqlCommand(cmd))
                {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }
        public static bool DropColumn(string column)
        {
            using (DbConnection connection = new SqlConnection(connstr))
            {
                connection.Open();
                using (DbCommand command = new SqlCommand("alter table [ProfileData] drop column " + column))
                {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static List<ProfileColumn> GetColumns()
        {
            List<ProfileColumn> profileCols = new List<ProfileColumn>();
            List<sp_columnsResult> cols = new SnitzMembership.MembershipDataDataContext().GetProfileColumns();
            foreach (sp_columnsResult col in cols)
            {
                ProfileColumn prof = new ProfileColumn();
                prof.ColumnName = col.COLUMN_NAME;
                prof.DataType = col.TYPE_NAME;
                prof.AllowNull = col.NULLABLE == 1;
                if(!String.IsNullOrEmpty(col.COLUMN_DEF))
                    prof.DefaultValue = col.COLUMN_DEF.Replace("(","").Replace(")","");
                
                if(col.TYPE_NAME == "nvarchar")
                    prof.Precision = col.PRECISION.ToString();
                profileCols.Add(prof);
            }
            return profileCols;
        }
    }
}
