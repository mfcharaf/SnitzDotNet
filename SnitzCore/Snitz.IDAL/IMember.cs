/*
####################################################################################################################
##
## Snitz.IDAL - IMember
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

using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.IDAL
{
    public interface IMember : IBaseObject<MemberInfo>
    {
        int GetMemberCount(object o);
        IEnumerable<int> GetAllowedForumIds(MemberInfo member, List<int> roleList, bool isadmin);
        IEnumerable<KeyValuePair<int, string>> GetAllowedForumList(MemberInfo member, List<int> roleList, bool isadmin);
        IEnumerable<TopicInfo> GetRecentTopics(int memberid, MemberInfo member);
        IEnumerable<MemberInfo> GetMembers(int startRecord, int maxRecords, string sortExpression, object filter);

        MemberInfo GetByEmail(string email);
        string[] ForumAdministrators();
        int GetPendingMemberCount();
        IEnumerable<MemberInfo> GetPendingMembers(int startRecord, int maxRecords);
        void UpdateVisit(MemberInfo member);
    }
}
