/*
####################################################################################################################
##
## Snitz.BLL - Replies
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
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
using System.Linq;
using System.Web;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzCommon;
using SnitzMembership;

namespace Snitz.BLL
{
    public static class Replies
    {
        public static string LastEditTimeAgo(object reply)
        {
            ReplyInfo thisreply = (ReplyInfo) reply;
            ProfileCommon profile = ProfileCommon.GetUserProfile(HttpContext.Current.User.Identity.Name);
            if (thisreply.LastEditDate.HasValue && !(thisreply.LastEditDate.Value == DateTime.MinValue))
                return String.Format("<abbr class='timeago' title='{0}'>{1}</abbr>",
                                     thisreply.LastEditDate.Value.ToISO8601Date(HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset),
                                     thisreply.LastEditDate.Value.ToForumDateDisplay(" ", true, HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset));
            else
                return String.Empty;
        }

        public static string DateTimeAgo(object reply)
        {
            ReplyInfo thisreply = (ReplyInfo)reply;
            ProfileCommon profile = ProfileCommon.GetUserProfile(HttpContext.Current.User.Identity.Name);
            return String.Format("<abbr class='timeago' title='{0}'>{1}</abbr>",
                                 thisreply.Date.ToISO8601Date(HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset),
                                 thisreply.Date.ToForumDateDisplay(" ", true, HttpContext.Current.User.Identity.IsAuthenticated, profile.TimeOffset));
        }

        public static void MoveReplies(int newtopicid, List<int> replyIDs)
        {
            ITopic topicdal = Factory<ITopic>.Create("Topic");
            TopicInfo topic = topicdal.GetById(newtopicid);

            IReply dal = Factory<IReply>.Create("Reply");
            dal.MoveReplies(topic, replyIDs);
            Admin.UpdateForumCounts();
        }
        public static ReplyInfo GetReply(int replyid)
        {
            IReply dal = Factory<IReply>.Create("Reply");
            return dal.GetById(replyid);
        }

        public static int AddReply(ReplyInfo reply)
        {
            IReply dal = Factory<IReply>.Create("Reply");
            int replyid = dal.Add(reply);
            return replyid;
        }

        public static void UpdateReply(int replyid, string message, MemberInfo author, bool isAdministrator, bool showsig)
        {
            ReplyInfo reply = GetReply(replyid);
            reply.Message = message;
            if (!isAdministrator)
            {
                reply.LastEditDate = DateTime.UtcNow;
                reply.LastEditedById = author.Id;
            }
            reply.UseSignatures = showsig;

            IReply dal = Factory<IReply>.Create("Reply");
            dal.Update(reply);
        }

        public static int FindReplyPage(int replyid)
        {
            bool found = false;
            int pagesize = SnitzConfig.Config.TopicPageSize;
            int page = 0;

            ReplyInfo reply = GetReply(replyid);
            ITopic topicdal = Factory<ITopic>.Create("Topic");
            List<int> replies = new List<int>(topicdal.GetReplyIdList(reply.TopicId));

            while (!found)
            {
                List<int> sublist = (replies.Skip(page * pagesize).Take(pagesize)).ToList();
                if (sublist.Contains(replyid))
                {
                    found = true;
                    page += 1;
                    continue;
                }
                page += 1;
                if (sublist.Count < pagesize)
                {
                    found = true;
                    page = -1;
                }
            }
            return page;

        }

        public static void SetReplyStatus(int replyid, int status)
        {
            IReply dal = Factory<IReply>.Create("Reply");
            dal.SetReplyStatus(replyid,status);
        }
        public static void DeleteReply(int id)
        {
            ReplyInfo reply = GetReply(id);
            IReply dal = Factory<IReply>.Create("Reply");
            dal.Delete(reply);
            Admin.UpdateForumCounts();
        }
    }
}
