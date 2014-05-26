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
    public class BadWordFilter : IBadWordFilter
    {
        private string SELECT = "SELECT B_ID,B_BADWORD,B_REPLACE FROM " + Config.FilterTablePrefix + "BADWORDS ";

        #region IBaseObject<BadwordInfo> Members

        public BadwordInfo GetById(int id)
        {
            BadwordInfo badword = null;

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT + "WHERE B_ID=@BadwordId", new SqlParameter("@BadwordId", SqlDbType.Int) { Value = id }))
            {
                while (rdr.Read())
                {
                    badword = new BadwordInfo { Id = rdr.GetInt32(0), Badword = rdr.SafeGetString(1), Replace = rdr.SafeGetString(2) };
                }
            }
            return badword;
        }

        public IEnumerable<BadwordInfo> GetByName(string name)
        {
            List<BadwordInfo> badwords = new List<BadwordInfo>();

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT + "WHERE B_BADWORD=@Badword", new SqlParameter("@Badword",SqlDbType.VarChar){Value = name}))
            {
                while (rdr.Read())
                {
                    badwords.Add(new BadwordInfo { Id = rdr.GetInt32(0), Badword = rdr.SafeGetString(1), Replace = rdr.SafeGetString(2) });
                }
            }
            return badwords;
        }

        public int Add(BadwordInfo badword)
        {
            string strSql = "INSERT INTO " + Config.FilterTablePrefix + "BADWORDS (B_BADWORD,B_REPLACE) VALUES (@Badword,@Replaceword); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Badword", SqlDbType.NVarChar) {Value = badword.Badword},
                    new SqlParameter("@Replaceword", SqlDbType.NVarChar) {Value = badword.Replace}
                };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, SELECT + strSql, parms.ToArray()));
        }

        public void Update(BadwordInfo badword)
        {
            string strSql = "UPDATE " + Config.FilterTablePrefix + "BADWORDS SET B_BADWORD=@Badword,B_REPLACE=@Replaceword WHERE B_ID=@BadwordId";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@BadwordId", SqlDbType.Int) {Value = badword.Id},
                    new SqlParameter("@Badword", SqlDbType.NVarChar) {Value = badword.Badword},
                    new SqlParameter("@Replaceword", SqlDbType.NVarChar) {Value = badword.Replace}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Delete(BadwordInfo badword)
        {
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "DELETE FROM " + Config.FilterTablePrefix + "BADWORDS WHERE B_ID=@BadwordId", new SqlParameter("@BadwordId", SqlDbType.Int) { Value = badword.Id });
        }

        public IEnumerable<BadwordInfo> GetAll()
        {
            List<BadwordInfo> badwords = new List<BadwordInfo>();

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT, null))
            {
                while (rdr.Read())
                {
                    badwords.Add(new BadwordInfo { Id = rdr.GetInt32(0), Badword = rdr.SafeGetString(1), Replace = rdr.SafeGetString(2) });
                }
            }
            return badwords;
        }

        public void DeleteAll()
        {
            string strSql = "TRUNCATE TABLE " + Config.FilterTablePrefix + "BADWORDS ";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, null);
        }

        public void Dispose()
        {

        }

        #endregion
    }
}
