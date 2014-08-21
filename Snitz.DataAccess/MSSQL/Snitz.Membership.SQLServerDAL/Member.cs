using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.Membership.IDal;
using SnitzConfig;

namespace Snitz.Membership.SQLServerDAL
{
    public class Member : IMember
    {
        public bool ActivateUser(string username)
        {
            string strSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_VALID=1,M_LEVEL=1 WHERE M_NAME=@Username";
            int res = SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Username", SqlDbType.VarChar){Value = username});
            return res > 0;
        }

        public bool ChangeEmail(string username, bool valid, string email)
        {
            string newemail;
            string strSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_EMAIL=@Email WHERE M_NAME=@Username";
            if (!valid)
            {
                newemail = (ConfigurationManager.AppSettings["boolEncrypt"] == "1") ? Cryptos.CryptosUtilities.Encrypt(email) : email;
            }
            else
            {
                newemail = email;
            }
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = newemail });
            parms.Add(new SqlParameter("@Username", SqlDbType.VarChar) { Value = username });
            int result = SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            return (result == 1);
        }

        public int UnApprovedMemberCount()
        {
            string strSql = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_VALID=0";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, null));
        }

        public int MemberCount()
        {
            string strSql = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, null));
        }

        public string[] OnlineUsers(TimeSpan userIsOnlineTimeWindow)
        {
            List<string> users = new List<string>();
            string strSql = "SELECT M_NAME FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_LASTHEREDATE > @LastActivity";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@LastActivity", SqlDbType.VarChar) { Value = DateTime.UtcNow.Add(-userIsOnlineTimeWindow).ToString("yyyyMMddHHmmss") });

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql,parms.ToArray()))
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
            string strSql = "DELETE FROM " + Config.MemberTablePrefix + "ProfileData WHERE UserId=@Userid";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Userid",SqlDbType.Int){Value = member.Id});

        }

    }
}
