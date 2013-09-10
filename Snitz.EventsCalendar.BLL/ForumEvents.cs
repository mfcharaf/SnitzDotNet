/*
####################################################################################################################
##
## Snitz.BLL - ForumEvents
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System;
using System.Collections.Generic;
using Snitz.Entities;
using Snitz.EventsCalendar.IDAL;
using Snitz.IDAL;


namespace Snitz.BLL
{
    public static class ForumEvents
    {

        public static List<EventInfo> GetEvents(DateTime start, DateTime end)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return new List<EventInfo>(dal.GetEvents(start.ToString("yyyyMMddHHmmss"), end.ToString("yyyyMMddHHmmss")));

        }

        public static List<EventInfo> GetEventsForToday(string today)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return new List<EventInfo>(dal.GetEvents(today, null));
        }

        public static void AddEvent(string title, string description, int type, DateTime eventDate, int userid)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            EventInfo forumevent = new EventInfo
                                       {
                                           Title = title,
                                           Description = description,
                                           Type = type,
                                           Date = eventDate,
                                           MemberId = userid
                                       };
            dal.Add(forumevent);
        }

        public static EventInfo GetEvent(int id)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            return dal.GetById(id);
        }

        public static void DeleteEvent(int id)
        {
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            dal.Delete(GetEvent(id));
        }

        public static void UpdateEvent(int id, string title, string description, int type, DateTime eventDate)
        {
            EventInfo forumevent = GetEvent(id);
            forumevent.Title = title;
            forumevent.Description = description;
            forumevent.Type = type;
            forumevent.Date = eventDate;
            IForumEvent dal = Factory<IForumEvent>.Create("ForumEvent", "EventsDAL");
            dal.Update(forumevent);
        }
    }
}
