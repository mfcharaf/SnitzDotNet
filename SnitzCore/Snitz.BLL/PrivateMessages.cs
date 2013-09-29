/*
####################################################################################################################
##
## Snitz.BLL - PrivateMessages
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
using Snitz.IDAL;
using SnitzMembership;

namespace Snitz.BLL
{
    public static class PrivateMessages
    {
        public static List<MemberInfo> GetMemberListPaged(int page, string searchfor)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return new List<MemberInfo>(dal.GetMemberListPaged(page,searchfor));
        }

        public static List<PrivateMessageInfo> GetMessages(int memberid)
        {

            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return new List<PrivateMessageInfo>(dal.GetMessages(memberid));
        }

        public static List<PrivateMessageInfo> GetSentMessages(int memberid)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return new List<PrivateMessageInfo>(dal.GetSentMessages(memberid));
        }

        public static void SendPrivateMessage(PrivateMessageInfo pm)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            dal.Add(pm);
        }

        public static void DeletePrivateMessage(int pmid)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            dal.Delete(GetMessage(pmid));
        }

        public static ProfileCommon GetPreferences(string user)
        {
            return ProfileCommon.GetUserProfile(user);           
        }

        public static void SavePreferences(object user, string enabled, string notify, string layout)
        {
            ProfileCommon _profile = ProfileCommon.GetUserProfile((string) user);

            _profile.PMEmail = Convert.ToInt16(notify);
            _profile.PMReceive = Convert.ToInt16(enabled);
            _profile.PMLayout = layout;
            _profile.Save();

        }

        public static int GetUnreadPMCount(int userid)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return dal.UnreadPMCount(userid);
        }

        public static PrivateMessageInfo GetMessage(int pmid)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return dal.GetById(pmid);
        }

        public static void RemoveFromOutBox(int currentPmId)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            PrivateMessageInfo pm = GetMessage(currentPmId);
            pm.OutBox = 0;
            dal.Update(pm);

        }

        public static int GetMemberCount(string searchfor)
        {
            IPrivateMessage dal = Factory<IPrivateMessage>.Create("PrivateMessage");
            return dal.MemberCount(searchfor);
        }


    }
}
