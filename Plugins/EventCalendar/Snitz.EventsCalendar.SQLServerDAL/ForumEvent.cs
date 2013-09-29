using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.EventsCalendar.IDAL;
using Snitz.SQLServerDAL;
using Snitz.SQLServerDAL.Helpers;


namespace Snitz.EventsCalendar.SQLServerDAL
{
    public class ForumEvent : IForumEvent
    {
        public IEnumerable<EventInfo> GetEvents(string startdate, string enddate)
        {
            List<SqlParameter> parms = new List<SqlParameter>();

            string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description FROM FORUM_EVENT ";
            if (String.IsNullOrEmpty(enddate))
            {
                strSql = strSql + "WHERE EventDate=@Date";
                parms.Add(new SqlParameter("@Date",SqlDbType.VarChar){Value = startdate});
            }
            else
            {
                strSql = strSql + "WHERE EventDate>=@Date AND EventDate<=@EndDate";
                parms.Add(new SqlParameter("@Date", SqlDbType.VarChar) { Value = startdate });
                parms.Add(new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = enddate });
            }
            List<EventInfo> events = new List<EventInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    events.Add(CopyEventToBO(rdr));
                }
            }
            return events;
        }

        public EventInfo GetById(int eventid)
        {
            const string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description FROM FORUM_EVENT WHERE Id=@EventId";
            EventInfo calevent = null;
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@EventId", SqlDbType.VarChar) { Value = eventid }))
            {
                while (rdr.Read())
                {
                    calevent = CopyEventToBO(rdr);
                }
            }
            return calevent;
        }

        public IEnumerable<EventInfo> GetByName(string name)
        {
            const string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description FROM FORUM_EVENT WHERE Title=@Name";
            List<EventInfo> calevents = new List<EventInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Name", SqlDbType.VarChar) { Value = name }))
            {
                while (rdr.Read())
                {
                    calevents.Add(CopyEventToBO(rdr));
                }
            }
            return calevents;
        }

        public void Delete(EventInfo forumevent)
        {
            const string strSql = "DELETE FROM FORUM_EVENT WHERE Id=@EventId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql,new SqlParameter("@EventId", SqlDbType.VarChar) {Value = forumevent.Id});
        }

        public void Update(EventInfo forumevent)
        {
            const string strSql = "UPDATE FORUM_EVENT SET Title=@Title,Type=@Type,Author=@Author,EventDate=@Date,Description=@Description WHERE Id=@EventId";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@EventId", SqlDbType.Int) {Value = forumevent.Id},
                new SqlParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new SqlParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new SqlParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new SqlParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new SqlParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public int Add(EventInfo forumevent)
        {
            const string strSql =
                "INSERT INTO FORUM_EVENT (Title,Type,Author,EventDate,Description) VALUES " +
                "(@Title,@Type,@Author,@Date,@Description); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new SqlParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new SqlParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new SqlParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new SqlParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description}
            };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Dispose()
        {

        }



        private EventInfo CopyEventToBO(SqlDataReader rdr)
        {
            //Id,Title,Type,Audience,Author,EventDate,Description
            EventInfo forumevent = new EventInfo
            {
                                           Id = rdr.GetInt32(0),
                                           Title = rdr.SafeGetString(1),
                                           Type = rdr.GetInt32(2),
                                           MemberId = rdr.GetInt32(4),
                                           Date = rdr.GetSnitzDate(5).Value,
                                           Description = rdr.SafeGetString(6)
                                       };
            return forumevent;
        }
    }
}
