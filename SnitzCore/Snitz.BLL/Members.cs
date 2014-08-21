/*
####################################################################################################################
##
## Snitz.BLL - Members
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
using SnitzMembership;

namespace Snitz.BLL
{
    public static class Members
    {
        public static string LinkTarget(AuthorInfo author)
        {
            ProfileCommon prof = ProfileCommon.GetUserProfile(author.Username);
            if (String.IsNullOrEmpty(prof.LinkTarget))
                return "_blank";
            return prof.LinkTarget;
        }

        public static string MemberSinceTimeAgo(object member)
        {
            MemberInfo m = (MemberInfo) member;

            return SnitzTime.TimeAgoTag(m.MemberSince, HttpContext.Current.User.Identity.IsAuthenticated, m);
        }

        public static string LastVisitTimeAgo(object member)
        {
            MemberInfo m = (MemberInfo)member;
            return !m.LastVisitDate.HasValue ? "" : SnitzTime.TimeAgoTag(m.LastVisitDate.Value, HttpContext.Current.User.Identity.IsAuthenticated, m);
        }

        public static string LastPostTimeAgo(object member)
        {
            MemberInfo m = (MemberInfo)member;

            return !m.LastPostDate.HasValue ? "" : SnitzTime.TimeAgoTag(m.LastPostDate.Value, HttpContext.Current.User.Identity.IsAuthenticated, m);
        }

        public static MemberInfo GetMember(string username)
        {
            if (String.IsNullOrEmpty(username))
                return null;

            List<int> roleList = new List<int> { 0 };
            roleList.AddRange(SnitzCachedLists.UserRoles().Select(role => role.Key));
            
            IMember dal = Factory<IMember>.Create("Member");
            var member = dal.GetByName(username).FirstOrDefault();
            if (!string.IsNullOrEmpty(member.Signature))
            {
                member.ParsedSignature = member.Signature.ParseTags();
            }
            member.AllowedForums = Forums.AllowedForums(member).ToArray();
            // Run a search against the data store
            return member;
        }

        public static bool IsSubscribedToTopic(int topicid, int memberid)
        {
            var res = Subscriptions.GetTopicSubscriptions(topicid).Where(s => s.MemberId == memberid);
            return res.Any();
        }

        public static bool IsSubscribedToForum(int memberid, int forumid)
        {
            var res = Subscriptions.GetForumSubscriptions(forumid).Where(s => s.MemberId == memberid);
            return res.Any();

        }

        public static void SaveMember(MemberInfo member)
        {
            IMember dal = Factory<IMember>.Create("Member");
            dal.Update(member);
        }

        public static AuthorInfo GetAuthor(int memberid)
        {
            var member = GetMember(memberid);
            AuthorInfo author = new AuthorInfo(member);
            return author;
        }

        public static bool IsTopicAuthor(int memberid, int topicid)
        {
            ITopic dal = Factory<ITopic>.Create("Topic");
            TopicInfo topic = dal.GetById(topicid);
            return topic.AuthorId == memberid;
        }

        public static List<TopicInfo> GetRecentTopics(int memberid, MemberInfo member)
        {
            IMember dal = Factory<IMember>.Create("Member");
            return new List<TopicInfo>(dal.GetRecentTopics(memberid,member));
        }

        public static MemberInfo GetMember(int userid)
        {
            List<int> roleList = new List<int> { 0 };
            roleList.AddRange(SnitzCachedLists.UserRoles().Select(role => role.Key));
            
            IMember dal = Factory<IMember>.Create("Member");
            var member = dal.GetById(userid);
            if (!string.IsNullOrEmpty(member.Signature))
            {
                member.ParsedSignature = member.Signature.ParseTags();
            }
            member.AllowedForums = Forums.AllowedForums(member).ToArray();
            // Run a search against the data store
            return member;
        }

        public static List<MemberInfo> GetAllMembers(string sortExpression, int startRecord, int maxRecords)
        {
            IMember dal = Factory<IMember>.Create("Member");
            return new List<MemberInfo>(dal.GetMembers(startRecord, maxRecords, sortExpression, HttpContext.Current.Session["SearchFilter"]));
        }

        public static int GetMemberCount()
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.GetMemberCount(HttpContext.Current.Session["SearchFilter"]);
        }
    }
}
