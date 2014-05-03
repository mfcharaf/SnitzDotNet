using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.EventsCalendar.IDAL;
using Snitz.OLEDbDAL;
using Snitz.OLEDbDAL.Helpers;


namespace Snitz.EventsCalendar.OLEDbDAL
{
    public class ForumEvent : IForumEvent
    {
        public IEnumerable<EventInfo> GetEvents(string startdate, string enddate)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();

            string strSql = "SELECT Id,Title,Type,Audience,Author,EventDate,Description FROM FORUM_EVENT ";
            if (String.IsNullOrEmpty(enddate))
            {
                strSql = strSql + "WHERE EventDate=@Date";
                parms.Add(new OleDbParameter("@Date", SqlDbType.VarChar) { Value = startdate });
            }
            else
            {
                strSql = strSql + "WHERE EventDate>=@Date AND EventDate<=@EndDate";
                parms.Add(new OleDbParameter("@Date", SqlDbType.VarChar) { Value = startdate });
                parms.Add(new OleDbParameter("@EndDate", SqlDbType.VarChar) { Value = enddate });
            }
            List<EventInfo> events = new List<EventInfo>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
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
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@EventId", SqlDbType.VarChar) { Value = eventid }))
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
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Name", SqlDbType.VarChar) { Value = name }))
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
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@EventId", SqlDbType.VarChar) { Value = forumevent.Id });
        }

        public void Update(EventInfo forumevent)
        {
            const string strSql = "UPDATE FORUM_EVENT SET Title=@Title,Type=@Type,Author=@Author,EventDate=@Date,Description=@Description WHERE Id=@EventId";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@EventId", SqlDbType.Int) {Value = forumevent.Id},
                new OleDbParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new OleDbParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new OleDbParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new OleDbParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new OleDbParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public int Add(EventInfo forumevent)
        {
            const string strSql =
                "INSERT INTO FORUM_EVENT (Title,Type,Author,EventDate,Description) VALUES " +
                "(@Title,@Type,@Author,@Date,@Description); SELECT SCOPE_IDENTITY();";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@Title", SqlDbType.VarChar) {Value = forumevent.Title},
                new OleDbParameter("@Type", SqlDbType.Int) {Value = forumevent.Type},
                new OleDbParameter("@Author", SqlDbType.VarChar) {Value = forumevent.MemberId},
                new OleDbParameter("@Date", SqlDbType.VarChar) {Value = forumevent.Date.ToString("yyyyMMddHHmmss")},
                new OleDbParameter("@Description", SqlDbType.VarChar) {Value = forumevent.Description}
            };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Dispose()
        {

        }



        private EventInfo CopyEventToBO(OleDbDataReader rdr)
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
