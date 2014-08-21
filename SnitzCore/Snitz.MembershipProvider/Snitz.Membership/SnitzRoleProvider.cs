/*
####################################################################################################################
##
## SnitzMembership - SnitzRoleProvider
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
using System.Web.Security;
using System.Configuration.Provider;
using Snitz.Entities;
using SnitzMembership.Helpers;

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
            return BusinessUtil.GetAllRoles();
        }

        public int GetRoleCount()
        {
            return BusinessUtil.TotalRoles;
        }
        /// <summary>
        /// Gets the assigned roles for a particular user.
        /// </summary>
        /// <param name="username">Matching username</param>
        /// <returns>Array of assigned roles</returns>
        public override string[] GetRolesForUser(string username)
        {
            return BusinessUtil.GetRolesForUser(username);
        }

        /// <summary>
        /// Gets all the users in a particular role
        /// </summary>
        public override string[] GetUsersInRole(string roleName)
        {
            return BusinessUtil.GetUsersInRole(roleName);
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
                var r = new RoleInfo { RoleName = roleName, LoweredRolename = roleName.ToLower(), Description = roleName };

                BusinessUtil.AddRole(r);

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
            var r = new RoleInfo {Id = roleId, RoleName = roleName, LoweredRolename = roleName.ToLower() , Description = roleDescription};

            BusinessUtil.AddRole(r);

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
                if (throwOnPopulatedRole)
                {

                    if (BusinessUtil.GetUsersInRole(roleName).Any())
                    {
                        throw new ProviderException("Cannot delete roles with users assigned to them");
                    }
                }

                BusinessUtil.DeleteRole(roleName);
                ret = true;
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
            BusinessUtil.AddUsersToRoles(usernames, roleNames);
        }

        /// <summary>
        /// Remove a collection of users from a collection of corresponding roles
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            BusinessUtil.RemoverUsersFromRoles(usernames, roleNames);
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
            return BusinessUtil.IsUserInRole(username, roleName);
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
            return BusinessUtil.RoleExists(roleid);
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


        public Dictionary<int,string> ListAllRolesForUser(string username)
        {
            return BusinessUtil.GetRoleListForUser(username);
        }

        /*************************************************************************
         * Private helper methods
         *************************************************************************/

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        public static RoleInfo GetRoleFull(int roleid)
        {
            return BusinessUtil.GetRoleFull(roleid);
        }

        public static IEnumerable<RoleInfo> GetAllRolesFull(int maximumRows, int startRowIndex)
        {
            return BusinessUtil.GetAllRolesFull().Skip(startRowIndex).Take(maximumRows);
        }

        public static void AddRolesToForum(int forumId, string[] newroles)
        {
            BusinessUtil.AddRolesToForum(forumId, newroles);
        }

        public static void UpdateRoleInfo(int roleid, string name, string description)
        {
            BusinessUtil.UpdateRoleInfo(roleid, name, description);
        }

    }
}

