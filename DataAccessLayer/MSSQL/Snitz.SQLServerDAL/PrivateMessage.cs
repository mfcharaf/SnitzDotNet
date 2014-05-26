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
    public class PrivateMessage : IPrivateMessage
    {
        private string pmSelect =
            "SELECT M_ID,M_SUBJECT,M_FROM,M_TO,M_SENT,M_MESSAGE,M_PMCOUNT,M_READ,M_MAIL,M_OUTBOX,ToMember.M_NAME AS ToMemberName, FromMember.M_NAME AS FromMemberName FROM (" +
            Config.ForumTablePrefix + "PM PM LEFT OUTER JOIN " +
            Config.MemberTablePrefix + "MEMBERS AS FromMember ON PM.M_FROM = FromMember.MEMBER_ID) LEFT OUTER JOIN " +
            Config.MemberTablePrefix + "MEMBERS AS ToMember ON PM.M_TO = ToMember.MEMBER_ID ";

        public PrivateMessageInfo GetById(int id)
        {
            PrivateMessageInfo pm = new PrivateMessageInfo();
            string strSql = "UPDATE " + Config.ForumTablePrefix + "PM SET M_READ=M_READ+1 WHERE M_ID=@MessageId; " +
                            pmSelect + "WHERE M_ID=@MessageId";

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@MessageId", SqlDbType.Int) { Value = id }))
            {
                while (rdr.Read())
                {
                    pm = BoHelper.CopyPrivateMessageToBO(rdr);
                }
            }
            return pm;
        }

        public IEnumerable<PrivateMessageInfo> GetByName(string name)
        {
            List<PrivateMessageInfo> pm = new List<PrivateMessageInfo>();
            string strSql = pmSelect + "WHERE M_SUBJECT LIKE @Subject";

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Subject", SqlDbType.Int) { Value = "%" + name + "%" }))
            {
                while (rdr.Read())
                {
                    pm.Add(BoHelper.CopyPrivateMessageToBO(rdr));
                }
            }
            return pm;
        }

        public int Add(PrivateMessageInfo message)
        {

            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "PM (M_SUBJECT,M_FROM,M_TO,M_SENT,M_MESSAGE,M_READ,M_MAIL,M_OUTBOX) VALUES " +
                                  "(@Subject,@FromUser,@ToUser,@SentDate,@Message,0,@Email,@Outbox)";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Subject", SqlDbType.NVarChar) {Value = message.Subject},
                    new SqlParameter("@FromUser", SqlDbType.Int) {Value = message.FromMemberId},
                    new SqlParameter("@ToUser", SqlDbType.Int) {Value = message.ToMemberId},
                    new SqlParameter("@SentDate", SqlDbType.NVarChar) {Value = message.SentDate},
                    new SqlParameter("@Message", SqlDbType.NVarChar) {Value = message.Message},
                    new SqlParameter("@Email", SqlDbType.NVarChar) {Value = message.Mail},
                    new SqlParameter("@Outbox", SqlDbType.Int) {Value = message.OutBox}
                };

            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            return Convert.ToInt32(res);
        }

        public void Update(PrivateMessageInfo message)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "PM SET M_SUBJECT=,M_FROM=@Subject,M_TO=@ToUser,M_SENT=@FromUser,M_MESSAGE=@Message,M_READ=M_READ+1,M_MAIL=@Email,M_OUTBOX=@Outbox " +
                                  "WHERE M_ID=@PmId";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@PmId", SqlDbType.Int) {Value = message.Id},
                    new SqlParameter("@Subject", SqlDbType.NVarChar) {Value = message.Subject},
                    new SqlParameter("@FromUser", SqlDbType.Int) {Value = message.FromMemberId},
                    new SqlParameter("@ToUser", SqlDbType.Int) {Value = message.ToMemberId},
                    new SqlParameter("@SentDate", SqlDbType.NVarChar) {Value = message.SentDate},
                    new SqlParameter("@Message", SqlDbType.NVarChar) {Value = message.Message},
                    new SqlParameter("@Email", SqlDbType.NVarChar) {Value = message.Mail},
                    new SqlParameter("@Outbox", SqlDbType.Int) {Value = message.OutBox}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Delete(PrivateMessageInfo pm)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "PM WHERE M_ID=@PmId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql,new SqlParameter("@PmId", SqlDbType.Int) {Value = pm.Id});
        }

        public void Dispose()
        {

        }

        public int MemberCount(string searchfor)
        {
            string strSql = "SELECT COUNT(M.MEMBER_ID) AS MemberCount " +
                            "FROM " + Config.MemberTablePrefix + "MEMBERS AS M LEFT OUTER JOIN " +
                            Config.MemberTablePrefix + "ProfileData AS P ON M.MEMBER_ID = P.UserId " +
                            "WHERE (M.M_STATUS = 1) AND (COALESCE (P.PMReceive, 1) <> 0) ";
            SqlParameter search = null;
            if (!String.IsNullOrEmpty(searchfor))
            {
                strSql = strSql + "AND M_NAME LIKE @SearchFor ";
                search = new SqlParameter("@SearchFor", SqlDbType.NVarChar) { Value = searchfor + "%" };
            }

            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, search);
            return Convert.ToInt32(res);
        }

        public IEnumerable<PrivateMessageInfo> GetMessages(int memberid)
        {
            string strSql = pmSelect + "WHERE M_TO=@MemberId ORDER BY M_SENT DESC";
            List<PrivateMessageInfo> pm = new List<PrivateMessageInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@MemberId",SqlDbType.Int){Value = memberid}))
            {
                while (rdr.Read())
                {
                    pm.Add(BoHelper.CopyPrivateMessageToBO(rdr));
                }
            }
            return pm;
        }

        public IEnumerable<PrivateMessageInfo> GetSentMessages(int memberid)
        {
            string strSql = pmSelect + "WHERE M_FROM=@MemberId AND M_OUTBOX=1 ORDER BY M_SENT DESC";
            List<PrivateMessageInfo> pm = new List<PrivateMessageInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@MemberId", SqlDbType.Int) { Value = memberid }))
            {
                while (rdr.Read())
                {
                    pm.Add(BoHelper.CopyPrivateMessageToBO(rdr));
                }
            }
            return pm;
        }

        public IEnumerable<MemberInfo> GetMemberListPaged(int page, string searchfor)
        {
            string SELECT_OVER = "WITH MemberEntities AS (SELECT ROW_NUMBER() OVER (ORDER BY M_NAME) AS Row, MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS LEFT OUTER JOIN " + Config.MemberTablePrefix + "ProfileData P ON P.UserId=" + Config.MemberTablePrefix + "MEMBERS.MEMBER_ID WHERE M_STATUS=1 AND COALESCE(PMEmail,1)=1 ";
            if (!String.IsNullOrEmpty(searchfor))
            {
                SELECT_OVER += " AND M_NAME LIKE @SearchFor ";
            }
            const string MEMBER_SELECT = ") SELECT M.MEMBER_ID,M.M_STATUS,M.M_NAME,M.M_USERNAME,M.M_EMAIL,M.M_COUNTRY,M.M_HOMEPAGE,M.M_SIG" +
                                         ",M.M_LEVEL,M.M_AIM,M.M_YAHOO,M.M_ICQ,M.M_MSN,M.M_POSTS,M.M_DATE,M.M_LASTHEREDATE,M.M_LASTPOSTDATE" +
                                         ",M.M_TITLE,M.M_SUBSCRIPTION,M.M_HIDE_EMAIL,M.M_RECEIVE_EMAIL,M.M_IP,M.M_VIEW_SIG,M.M_SIG_DEFAULT" +
                                         ",M.M_VOTED,M.M_ALLOWEMAIL,M.M_AVATAR,M.M_THEME,M.M_TIMEOFFSET,M.M_DOB,M_AGE,M_PASSWORD,M_KEY,M_VALID,M_LASTUPDATED " +
                                         ",M_MARSTATUS,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION,M_SEX,M_HOBBIES,M_LNEWS,M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_CITY,M_STATE ";
            string SELECT_OVER_FROM = "FROM MemberEntities ME INNER JOIN " + Config.MemberTablePrefix + "MEMBERS M on ME.MEMBER_ID = M.MEMBER_ID " +
                                            " WHERE ME.Row Between @Start AND @MaxRows ORDER BY ME.Row ASC ";
            List<MemberInfo> members = new List<MemberInfo>();
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Start", SqlDbType.Int) {Value = 1 + (page*11)},
                    new SqlParameter("@MaxRows", SqlDbType.Int) {Value = (page*11) + 11}
                };

            if (!String.IsNullOrEmpty(searchfor))
                parms.Add(new SqlParameter("@SearchFor", SqlDbType.NVarChar) { Value = searchfor + "%" });
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT_OVER + MEMBER_SELECT + SELECT_OVER_FROM, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    members.Add(BoHelper.CopyMemberToBO(rdr));
                }
            }
            return members;
        }

        public int UnreadPMCount(int memberid)
        {
            string strSql = "SELECT COUNT(M_ID) FROM " + Config.ForumTablePrefix + "PM WHERE M_TO=@MemberId AND M_READ=0";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql,new SqlParameter("@MemberId", SqlDbType.Int) {Value = memberid}));
        }
    }
}
