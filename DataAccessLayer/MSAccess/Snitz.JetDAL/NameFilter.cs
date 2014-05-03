using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.OLEDbDAL
{
    public class NameFilter : INameFilter
    {
        #region IBaseObject<NameFilterInfo> Members

        public NameFilterInfo GetById(int id)
        {
            const string strSql = "SELECT N_ID,N_NAME FROM FORUM_NAMEFILTER WHERE N_ID=@Id";
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
            const string strSql = "SELECT N_ID,N_NAME FROM FORUM_NAMEFILTER WHERE N_NAME=@Name";
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
            const string strSql = "INSERT INTO FORUM_NAMEFILTER (N_NAME) VALUES (@Name)";
            return Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Name", OleDbType.VarChar) { Value = filter.Name }));
        }

        public void Update(NameFilterInfo filter)
        {
            const string strSql = "UPDATE FORUM_NAMEFILTER SET M_NAME=@Name WHERE N_ID=@Id";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@Id", OleDbType.Numeric) {Value = filter.Id},
                new OleDbParameter("@NAme", OleDbType.VarChar) {Value = filter.Name}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public void Delete(NameFilterInfo filter)
        {
            const string strSql = "DELETE FROM FORUM_NAMEFILTER WHERE N_ID=@Id";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Id", OleDbType.VarChar) { Value = filter.Id });
        }

        public void Dispose()
        {
        }

        #endregion

        public IEnumerable<NameFilterInfo> GetAll()
        {
            const string strSql = "SELECT N_ID,N_NAME FROM FORUM_NAMEFILTER";
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
