/*
####################################################################################################################
##
## Snitz.BLL - Archive
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
using System.Web;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Archive
    {
        public static void ArchiveTopic(int forumid,int topicid)
        {
            IArchiveForum dal = Factory<IArchiveForum>.Create("ArchiveForums");
            dal.ArchiveTopic(forumid,topicid);
        }
        public static void ArchiveForumTopics(int forumid, List<int> topicids)
        {
            IArchiveForum dal = Factory<IArchiveForum>.Create("ArchiveForums");
            foreach (int topicid in topicids)
            {
                dal.ArchiveTopic(forumid, topicid);
            }
            
        }

        public static void DeleteArchivedTopic(int topicid)
        {
            throw new NotImplementedException();
        }

        public static void DeleteArchivedTopics(List<int> topicids)
        {
            throw new NotImplementedException();
        }

        public static int ArchiveForums(int[] forumIdList, string date)
        {
            if (date == null)
            {
                
            }
            try
            {
                IArchiveForum dal = Factory<IArchiveForum>.Create("ArchiveForums");
                foreach (int id in forumIdList)
                {
                    dal.ArchiveForum(id, date);
                }
                return 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static List<TopicInfo> GetForumTopicsSince(bool isAdminOrModerator, int? forumid, int? topicstatus, string fromdate, int startRowIndex, int maximumRows)
        {
            if (!forumid.HasValue)
                throw new HttpException(404,"Forum not found");
            if (maximumRows == 0)
                return new List<TopicInfo>();

            IArchiveForum dal = Factory<IArchiveForum>.Create("ArchiveForums");
            return new List<TopicInfo>(dal.GetTopics(startRowIndex, maximumRows, forumid.Value, isAdminOrModerator));
        }
        public static int GetForumTopicsSinceCount(bool isAdminOrModerator, int forumid, int? topicstatus, string fromdate, int startRowIndex, int maximumRows)
        {
            if (maximumRows == 0)
                return 0;
            IArchiveForum dal = Factory<IArchiveForum>.Create("ArchiveForums");
            return dal.GetTopicCount(startRowIndex, maximumRows, forumid, isAdminOrModerator);
        }
    }
}
