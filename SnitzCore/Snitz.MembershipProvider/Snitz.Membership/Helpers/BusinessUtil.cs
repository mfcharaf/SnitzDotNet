using System.Collections.Generic;
using System.Linq;
using Snitz.Entities;
using Snitz.Membership.IDal;

namespace SnitzMembership.Helpers
{
    public class BusinessUtil
    {
        public static string[] GetAllRoles()
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            string[] roles = dal.GetAllRoles();
            TotalRoles = roles.Length;
            return roles;
        }

        public static string[] GetRolesForUser(string username)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.GetRolesForUser(username);
        }

        public static string[] GetUsersInRole(string rolename)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.GetUsersInRole(rolename);
        }

        public static bool IsUserInRole(string username, string roleName)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.IsUserInRole(username, roleName);
        }

        public static bool RoleExists(int roleid)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.RoleExists(roleid);
        }

        public static void UpdateRoleInfo(int roleid, string name, string description)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.UpdateRoleInfo(roleid, name, description);
        }

        public static IEnumerable<RoleInfo> GetAllRolesFull()
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            var roles = dal.GetAllRolesFull();
            TotalRoles = roles.Count();
            return roles;
        }

        public static RoleInfo GetRoleFull(int roleid)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.GetRole(roleid);
        }

        public static void AddRolesToForum(int forumId, string[] newroles)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.AddRolesToForum(forumId, newroles);
        }

        public static Dictionary<int, string> GetRoleListForUser(string username)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            return dal.ListAllRolesForUser(username);
        }

        public static void RemoverUsersFromRoles(string[] usernames, string[] roleNames)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.RemoveUsersFromRoles(usernames,roleNames);
        }

        public static void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.AddUsersToRoles(usernames,roleNames);
        }

        public static void AddRole(RoleInfo roleInfo)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.CreateRole(roleInfo);
        }

        public static void DeleteRole(string roleName)
        {
            IRoles dal = Snitz.Membership.Helpers.Factory<IRoles>.Create("Role");
            dal.DeleteRole(roleName);
        }

        public static int TotalRoles { get; set; }
    }
}
