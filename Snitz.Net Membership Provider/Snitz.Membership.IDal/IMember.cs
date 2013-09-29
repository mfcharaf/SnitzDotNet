using System;
using Snitz.Entities;

namespace Snitz.Membership.IDal
{
    public interface IMember
    {
        bool ActivateUser(string username);
        bool ChangeEmail(string userName, bool valid, string email);
        int UnApprovedMemberCount();
        int MemberCount();
        string[] OnlineUsers(TimeSpan userIsOnlineTimeWindow);
        void DeleteProfile(MemberInfo memberInfo);
    }
}
