
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Configuration.Provider;
using SnitzMembership;

namespace Snitz.Providers
{
    public sealed class SnitzRoleProvider : RoleProvider
    {

        /*************************************************************************
         * General settings
         *************************************************************************/

        private string _applicationName;
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }


        /*************************************************************************
         * Retrieval methods
         *************************************************************************/

        /// <summary>
        /// Gets all available user roles
        /// </summary>
        /// <returns>Array of all available roles</returns>
        public override string[] GetAllRoles()
        {
            string[] roles;
            using (var db = new MembershipDataDataContext())
            {
                roles = (from r in db.Roles
                         select r.RoleName).ToArray();
            }
            return roles;
        }

        /// <summary>
        /// Gets the assigned roles for a particular user.
        /// </summary>
        /// <param name="username">Matching username</param>
        /// <returns>Array of assigned roles</returns>
        public override string[] GetRolesForUser(string username)
        {
            string[] roles;
            using (var db = new MembershipDataDataContext())
            {
                roles = (from mg in db.MembersInRoles
                         where mg.Member.Username == username
                         select mg.Role.RoleName).ToArray();
            }
            return roles;
        }

        /// <summary>
        /// Gets all the users in a particular role
        /// </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            string[] roles;
            using (var db = new MembershipDataDataContext())
            {
                roles = (from r in db.MembersInRoles
                         where r.Role.RoleName == roleName
                         select r.Member.Username).ToArray();

            }
            return roles;
        }

        /*************************************************************************
         * Create and Delete methods
         *************************************************************************/

        /// <summary>
        /// Creates a new role
        /// </summary>
        public override void CreateRole(string roleName)
        {
            // No need to add if it already exists
            if (!RoleExists(roleName))
            {
                var r = new Role {RoleName = roleName, LoweredRoleName = roleName.ToLower()};

                using (var db = new MembershipDataDataContext())
                {
                    db.Roles.InsertOnSubmit(r);
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// public RoleProvider.CreateRoleFullInfo
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="roleDescription"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool CreateRoleFullInfo(string roleName, string roleDescription, int roleId)
        {
            if (roleName.Contains(","))
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }

            if (RoleExists(roleName))
            {
                throw new ProviderException("Role name already exists.");
            }
            var r = new Role {RoleId = roleId, RoleName = roleName, LoweredRoleName = roleName.ToLower() , Description = roleDescription};

            using (var db = new MembershipDataDataContext())
            {
                db.Roles.InsertOnSubmit(r);
                db.SubmitChanges();
            }

            return true;
        }

        /// <summary>
        /// Deletes a given role
        /// </summary>
        /// <param name="roleName">Role name to delete</param>
        /// <param name="throwOnPopulatedRole">Specifies whether the function should throw 
        /// if there are assigned users to this role</param>
        /// <returns>True if successful. Defaults to false</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            // Return status. Defaults to false.
            bool ret = false;

            // You can only delete an existing role
            if (RoleExists(roleName))
            {
                using (var db = new MembershipDataDataContext())
                {
                        int[] users = (from mr in db.MembersInRoles
                                        where mr.Role.RoleName == roleName
                                        select mr.Member.UserId).ToArray();
                    if (throwOnPopulatedRole)
                    {

                        if (users.Count() > 0)
                            throw new ProviderException("Cannot delete roles with users assigned to them");
                    }

                    Role r = (from roles in db.Roles
                                where roles.RoleName == roleName
                                select roles).FirstOrDefault();

                    db.MembersInRoles.DeleteAllOnSubmit(db.MembersInRoles.Where(mr=>mr.RoleId == r.RoleId));
                    if (r != null) db.Roles.DeleteOnSubmit(r);
                    db.SubmitChanges();

                    ret = true;
                }
            }

            return ret;
        }

        /*************************************************************************
         * Assign/Remove methods
         *************************************************************************/

        /// <summary>
        /// Adds a collection of users to a collection of corresponding roles
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            // Get the actual available roles
            string[] allRoles = GetAllRoles();

            // See if any of the given roles match the available roles
            IEnumerable<string> roles = allRoles.Intersect(roleNames);

            // There were some roles left after removing non-existent ones
            if (roles.Any())
            {
                // Cleanup duplicates first
                RemoveUsersFromRoles(usernames, roleNames);

                using (var db = new MembershipDataDataContext())
                {
                    // Get the user IDs
                    List<int> mlist = (from members in db.Members
                                       where usernames.Contains(members.Username)
                                       select members.UserId).ToList();

                    // Get the role IDs
                    List<int> rlist = (from r in db.Roles
                                       where roleNames.Contains(r.RoleName)
                                       select r.RoleId).ToList();

                    // Fresh list of user-role assignments
                    var mrlist = new List<MembersInRole>();
                    foreach (int m in mlist)
                    {
                        foreach (int r in rlist)
                        {
                            var mr = new MembersInRole {UserId = m, RoleId = r};
                            mrlist.Add(mr);
                        }
                    }

                    db.MembersInRoles.InsertAllOnSubmit(mrlist);
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Remove a collection of users from a collection of corresponding roles
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            // Get the actual available roles
            string[] allRoles = GetAllRoles();

            // See if any of the given roles match the available roles
            IEnumerable<string> roles = allRoles.Intersect(roleNames);

            // There were some roles left after removing non-existent ones
            if (roles.Any())
            {
                using (var db = new MembershipDataDataContext())
                {
                    List<MembersInRole> mg = (from members in db.MembersInRoles
                                               where usernames.Contains(members.Member.Username) &&
                                               roleNames.Contains(members.Role.RoleName)
                                               select members).ToList();

                    db.MembersInRoles.DeleteAllOnSubmit(mg);
                    db.SubmitChanges();
                }
            }
        }

        /*************************************************************************
         * Searching methods
         *************************************************************************/

        /// <summary>
        /// Checks if a given username is in a particular role
        /// </summary>
        public override bool IsUserInRole(string username, string roleName)
        {
            // Return status defaults to false
            bool ret = false;

            if (RoleExists(roleName))
            {
                using (var db = new MembershipDataDataContext())
                {
                    int c = (from m in db.MembersInRoles
                             where m.Member.Username == username &&
                             m.Role.RoleName == roleName
                             select m).Count();

                    if (c > 0)
                        ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Finds a set of users in a given role
        /// </summary>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            // Here's another function that doesn't make sense without paging
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if a given role already exists in the database
        /// </summary>
        /// <param name="roleName">Role name to search</param>
        /// <returns>True if the role exists. Defaults to false.</returns>
        public override bool RoleExists(string roleName)
        {
            bool ret = GetAllRoles().Contains(roleName);

            // If the specified role doesn't exist

            return ret;
        }

        public bool RoleExists(int roleid)
        {
            bool ret;
            using (var db = new MembershipDataDataContext())
            {
                var res = from r in db.Roles
                          where r.RoleId == roleid
                          select r;
                ret = (res.Any());
            }

            return ret;
        }

        /*************************************************************************
         * Initialization
         *************************************************************************/

        /// <summary>
        /// Initialize the RoleProvider
        /// </summary>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = "SnitzRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Snitz Role Provider");
            }

            // Initialize base class
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"],
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

        }

        /*************************************************************************
         * Forum Specific Methods
         *************************************************************************/

        public static string[] GetForumRoles(int forumId)
        {
            string[] roles;
            using (var db = new MembershipDataDataContext())
            {
                roles = (from fr in db.ForumRoles
                         where fr.ForumId == forumId
                         select fr.Roles.RoleName.Trim()).ToArray();

            }

            return roles;
        }

        public bool IsUserForumModerator(string username, int forumid)
        {
            using (var db = new MembershipDataDataContext())
            {
                if (!IsUserInRole(username, "Moderator"))
                    return false;
                var res = from fm in db.ForumModerators
                          where fm.ForumId == forumid && fm.Members.Username == username
                          select fm;
                return res.Any();
            }
        }

        //public Dictionary<int,string> ListAllRoles()
        //{
        //    Dictionary<int, string> roles = null;
        //    using (var db = new MembershipDataDataContext())
        //    {
        //        roles = (from r in db.Roles
        //                 select r).ToDictionary(d => d.RoleId, d => d.RoleName);
        //    }
        //    return roles;
        //}

        public Dictionary<int,string> ListAllRolesForUser(string username)
        {
            Dictionary<int, string> roles;
            using (var db = new MembershipDataDataContext())
            {
                roles = (from mg in db.MembersInRoles
                         where mg.Member.Username == username
                         select mg.Role).ToDictionary(d => d.RoleId, d => d.RoleName);
            }
            return roles;
        }

        //public static List<Member> ListRoleMembers(string rolename)
        //{
        //    using (var db = new MembershipDataDataContext())
        //    {
        //        var res = (from mr in db.MembersInRoles
        //                   where mr.Role.RoleName == rolename
        //                   select mr.Member).ToList();
        //        return res;
        //    }
        //}

        /*************************************************************************
         * Private helper methods
         *************************************************************************/

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }
        
        public static bool IsUserInForumRole(string username, int forumid)
        {
            if (Roles.IsUserInRole("Administrator"))
                return true;
            using (var db = new MembershipDataDataContext())
            {
                //is the forum restricted
                var forumroles = (from fr in db.ForumRoles where fr.ForumId == forumid select fr);
                if (!forumroles.Any())
                    return true;
                if (forumroles.Count() == 1)
                {
                    if (forumroles.Single().Roles.LoweredRoleName == "all")
                        return true;
                }
                var res = (from fr in db.ForumRoles
                           where fr.ForumId == forumid
                           join mr in db.MembersInRoles on fr.RoleId equals mr.RoleId
                           where mr.Member.Username == username
                           select mr.Member).Count();
                return res > 0;
            }
        }

        public static Role GetRoleFull(int roleid)
        {
            using (var db = new MembershipDataDataContext())
            {
                return (from r in db.Roles where r.RoleId == roleid select r).SingleOrDefault();
            }
        }

        public static IEnumerable<Role> GetAllRolesFull()
        {
            using (var db = new MembershipDataDataContext())
            {
                return (from r in db.Roles select r).ToList();
            }
        }

        public static void AddRolesToForum(int forumId, string[] newroles)
        {
            using (var db = new MembershipDataDataContext())
            {
                List<string> rolesToAdd = newroles.ToList();
                var existingroles = (from fr in db.ForumRoles where fr.ForumId == forumId select fr);
                var removeroles = new List<int>();
                foreach (ForumRole existingrole in existingroles)
                {
                    if (!newroles.Contains(existingrole.Roles.LoweredRoleName))
                    {
                        removeroles.Add(existingrole.Id);
                    }
                    else if(newroles.Contains(existingrole.Roles.LoweredRoleName))
                    {
                        rolesToAdd.Remove(existingrole.Roles.LoweredRoleName);
                    }
                }
                
                db.ForumRoles.DeleteAllOnSubmit(db.ForumRoles.Where(f=> removeroles.Contains(f.Id)));
                db.SubmitChanges();

                var addroles = from role in db.Roles where rolesToAdd.Contains(role.LoweredRoleName) select role;
                foreach (Role role in addroles)
                {
                    var forumRole = new ForumRole {ForumId = forumId, RoleId = role.RoleId};
                    db.ForumRoles.InsertOnSubmit(forumRole);
                }
                db.SubmitChanges();
            }
        }

        public static void UpdateRoleInfo(int roleid, string name, string description)
        {
            using (var db = new MembershipDataDataContext())
            {
                Role role = (from r in db.Roles where r.RoleId == roleid select r).Single();
                if(role != null)
                {
                    role.RoleName = name;
                    role.Description = description;
                    db.SubmitChanges();
                }
            }
        }
    }
}

