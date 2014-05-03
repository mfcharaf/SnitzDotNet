using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.Membership.IDal;
using Snitz.OLEDbDAL.Helpers;

namespace Snitz.Membership.OLEDbDAL
{
    public class Member : IMember
    {
        public bool ActivateUser(string username)
        {
            const string strSql = "UPDATE FORUM_MEMBERS SET M_VALID=1 WHERE M_NAME=@Username";
            int res = SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Username", SqlDbType.VarChar) { Value = username });
            return res > 0;
        }

        public bool ChangeEmail(string username, bool valid, string email)
        {
            string newemail;
            const string strSql = "UPDATE FORUM_MEMBERS SET M_EMAIL=@Email WHERE M_NAME=@Username";
            if (!valid)
            {
                newemail = (ConfigurationManager.AppSettings["boolEncrypt"] == "1") ? Cryptos.CryptosUtilities.Encrypt(email) : email;
            }
            else
            {
                newemail = email;
            }
            List<OleDbParameter> parms = new List<OleDbParameter>();
            parms.Add(new OleDbParameter("@Email", SqlDbType.VarChar) { Value = newemail });
            parms.Add(new OleDbParameter("@Username", SqlDbType.VarChar) { Value = username });
            int result = SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            return (result == 1);
        }

        public int UnApprovedMemberCount()
        {
            const string strSql = "SELECT COUNT(MEMBER_ID) FROM FORUM_MEMBERS WHERE M_VALID=0";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, null));
        }

        public int MemberCount()
        {
            const string strSql = "SELECT COUNT(MEMBER_ID) FROM FORUM_MEMBERS";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, null));
        }

        public string[] OnlineUsers(TimeSpan userIsOnlineTimeWindow)
        {
            List<string> users = new List<string>();
            const string strSql = "SELECT M_NAME FROM FORUM_MEMBERS WHERE M_LASTHEREDATE > @LastActivity";
            List<OleDbParameter> parms = new List<OleDbParameter>();
            parms.Add(new OleDbParameter("@LastActivity", SqlDbType.VarChar) { Value = DateTime.UtcNow.Add(-userIsOnlineTimeWindow).ToString("yyyyMMddHHmmss") });

            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    users.Add(rdr.GetString(0));
                }
            }
            return users.ToArray();
        }

        public void DeleteProfile(MemberInfo member)
        {
            const string strSql = "DELETE FROM ProfileData WHERE UserId=@Userid";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Userid", SqlDbType.Int) { Value = member.Id });

        }

    }
}
