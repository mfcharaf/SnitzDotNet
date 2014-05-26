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
    public class NameFilter : INameFilter
    {
        #region IBaseObject<NameFilterInfo> Members

        public NameFilterInfo GetById(int id)
        {
            string strSql = "SELECT N_ID,N_NAME FROM " + Config.FilterTablePrefix + "NAMEFILTER WHERE N_ID=@Id";
            NameFilterInfo filter = null;
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Id", OleDbType.Numeric) { Value = id }))
            {
                while (rdr.Read())
                {
                    filter = new NameFilterInfo
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.SafeGetString(1)
                    };
                }
            }
            return filter;
        }

        public IEnumerable<NameFilterInfo> GetByName(string name)
        {
            string strSql = "SELECT N_ID,N_NAME FROM " + Config.FilterTablePrefix + "NAMEFILTER WHERE N_NAME=@Name";
            List<NameFilterInfo> filters = new List<NameFilterInfo>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Name", OleDbType.VarChar) { Value = name }))
            {
                while (rdr.Read())
                {
                    filters.Add(new NameFilterInfo
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.SafeGetString(1)
                    });
                }
            }
            return filters;
        }

        public int Add(NameFilterInfo filter)
        {
            string strSql = "INSERT INTO " + Config.FilterTablePrefix + "NAMEFILTER (N_NAME) VALUES (@Name)";
            return Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Name", OleDbType.VarChar) { Value = filter.Name }));
        }

        public void Update(NameFilterInfo filter)
        {
            string strSql = "UPDATE " + Config.FilterTablePrefix + "NAMEFILTER SET M_NAME=@Name WHERE N_ID=@Id";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@Id", OleDbType.Numeric) {Value = filter.Id},
                new OleDbParameter("@NAme", OleDbType.VarChar) {Value = filter.Name}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public void Delete(NameFilterInfo filter)
        {
            string strSql = "DELETE FROM " + Config.FilterTablePrefix + "NAMEFILTER WHERE N_ID=@Id";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Id", OleDbType.VarChar) { Value = filter.Id });
        }

        public void Dispose()
        {
        }

        #endregion

        public IEnumerable<NameFilterInfo> GetAll()
        {
            string strSql = "SELECT N_ID,N_NAME FROM " + Config.FilterTablePrefix + "NAMEFILTER";
            List<NameFilterInfo> namefilters = new List<NameFilterInfo>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    namefilters.Add(new NameFilterInfo { Id = rdr.GetInt32(0), Name = rdr.SafeGetString(1) });
                }
            }
            return namefilters;
        }

    }
}
