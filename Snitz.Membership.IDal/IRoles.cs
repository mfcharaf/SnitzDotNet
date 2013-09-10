using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.Membership.IDal
{
    public interface IRoles
    {
        bool IsUserInRole(string username, string roleName);
        bool RoleExists(string roleName);
        bool RoleExists(int roleid);
        bool IsUserForumModerator(string username, int forumid);
        bool IsUserInForumRole(string username, int forumid);

        string[] GetAllRoles();
        string[] GetRolesForUser(string username);
        string[] GetUsersInRole(string roleName);
        string[] GetForumRoles(int forumId);
        Dictionary<int, string> ListAllRolesForUser(string username);

        IEnumerable<RoleInfo> GetAllRolesFull();
        RoleInfo GetRole(int roleid);
        void AddRolesToForum(int forumId, string[] newroles);
        void CreateRole(RoleInfo roleInfo);
        void CreateRole(string roleName);
        void DeleteRole(string roleName);
        void UpdateRoleInfo(int roleid, string name, string description);
        void AddUsersToRoles(string[] usernames, string[] roleNames);
        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);

    }
}
