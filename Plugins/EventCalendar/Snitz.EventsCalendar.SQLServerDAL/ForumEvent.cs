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
        public IEnumerable<IEvent> GetEvents(string startdate, string enddate)
        {
            List<SqlParameter> parms = new List<SqlParameter>();

            string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description,Recur FROM FORUM_EVENT ";
            if (String.IsNullOrEmpty(enddate))
            {
                strSql = strSql + "WHERE Enabled=1 AND (EventDate=@Date OR Recur > 0)";
                parms.Add(new SqlParameter("@Date",SqlDbType.VarChar){Value = startdate});
            }
            else
            {
                strSql = strSql + "WHERE Enabled=1 AND ((EventDate>=@Date AND EventDate<=@EndDate) OR (Recur > 0))";
                parms.Add(new SqlParameter("@Date", SqlDbType.VarChar) { Value = startdate });
                parms.Add(new SqlParameter("@EndDate", SqlDbType.VarChar) { Value = enddate });
            }
            List<IEvent> events = new List<IEvent>();
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
            const string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description,Recur FROM FORUM_EVENT WHERE Id=@EventId";
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
            const string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description,Recur FROM FORUM_EVENT WHERE Title=@Name";
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
            const string strSql = "UPDATE FORUM_EVENT SET Title=@Title,Type=@Type,Author=@Author,EventDate=@Date,Description=@Description,Recur=@Recur,Enabled=@Enabled WHERE Id=@EventId";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@EventId", SqlDbType.Int) {Value = forumevent.Id},
                new SqlParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new SqlParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new SqlParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new SqlParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new SqlParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description},
                new SqlParameter("@Recur", SqlDbType.Int) {Value = forumevent.RecurringFrequency},
                new SqlParameter("@Enabled", SqlDbType.SmallInt) {Value = forumevent.Enabled}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public int Add(EventInfo forumevent)
        {
            const string strSql =
                "INSERT INTO FORUM_EVENT (Title,Type,Author,EventDate,Description,Recur,Enabled) VALUES " +
                "(@Title,@Type,@Author,@Date,@Description,@Recur,1); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new SqlParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new SqlParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new SqlParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new SqlParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description},
                new SqlParameter("@Recur", SqlDbType.Int) {Value = forumevent.RecurringFrequency}
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
                                           Audience = rdr.SafeGetString(3),
                                           MemberId = rdr.GetInt32(4),
                                           Date = rdr.GetSnitzDate(5).Value,
                                           Description = rdr.SafeGetString(6),
                                           RecurringFrequency = (RecurringFrequencies) rdr.SafeGetInt32(7)
                                       };
            var mem = new Member();
            forumevent.Author = new AuthorInfo(mem.GetById(forumevent.MemberId));
            return forumevent;
        }
    }
}
