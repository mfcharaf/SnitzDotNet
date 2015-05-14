﻿using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Snitz.Entities;
using Snitz.Membership.IDal;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.Membership.OLEDbDAL
{
    public class Role : IRoles
    {
        public void CreateRole(RoleInfo role)
        {
            if (!RoleExists(role.RoleName))
            {
                const string strSql = "INSERT INTO snitz_Roles (ApplicationId,RoleName, LoweredRoleName,Description,RoleId) VALUES (@AppId,@Name,@LoweredName,@Description,@RoleId)";
                List<OleDbParameter> parms = new List<OleDbParameter>
                    {
                        new OleDbParameter("@AppId", SqlDbType.VarChar){Value= new Guid().ToString()},
                        new OleDbParameter("@Name", SqlDbType.VarChar){Value=role.RoleName},
                        new OleDbParameter("@LoweredName", SqlDbType.VarChar){Value=role.LoweredRolename},
                        new OleDbParameter("@Description", SqlDbType.VarChar){Value=role.Description},
                        new OleDbParameter("@RoleId", SqlDbType.Int){Value=role.Id}
                    };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

            }
        }

        public void CreateRole(string roleName)
        {
            if (!RoleExists(roleName))
            {
                const string strSql = "INSERT INTO snitz_Roles (RoleName, LoweredRoleName,Description) VALUES (@Name,@LoweredName,@Description)";
                List<OleDbParameter> parms = new List<OleDbParameter>
                    {
                        new OleDbParameter("@Name", SqlDbType.VarChar){Value=roleName},
                        new OleDbParameter("@LoweredName", SqlDbType.VarChar){Value=roleName.ToLower()},
                        new OleDbParameter("@Description", SqlDbType.VarChar){Value=roleName}
                    };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            }
        }

        public void DeleteRole(string roleName)
        {
            const string strSql = "DELETE FROM snitz_Roles WHERE RoleName=@Name";
            List<OleDbParameter> parms = new List<OleDbParameter>
                    {
                        new OleDbParameter("@Name", SqlDbType.VarChar){Value=roleName}
                    };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public void DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            const string strSql = "DELETE FROM snitz_Roles WHERE RoleName=@Name";
            string[] users = GetUsersInRole(roleName);
            if (throwOnPopulatedRole)
            {

                if (users.Length > 0)
                    throw new ProviderException("Cannot delete roles with users assigned to them");
            }
            else
            {
                RemoveUsersFromRoles(users, new[] { roleName });

                List<OleDbParameter> parms = new List<OleDbParameter>
                        {
                            new OleDbParameter("@Name", SqlDbType.VarChar){Value=roleName}
                        };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
            }

        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            string[] allRoles = GetAllRoles();

            // See if any of the given roles match the available roles
            List<string> roles = new List<string>(allRoles.Intersect(roleNames));
            if (roles.Any())
            {
                // Cleanup duplicates first
                RemoveUsersFromRoles(usernames, roleNames);
                foreach (string role in roles)
                {
                    var roleinfo = GetRole(role);
                    string sql = "INSERT INTO snitz_UsersInRoles (RoleId,UserId) SELECT @RoleId,MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_NAME=@Name";

                    foreach (string username in usernames)
                    {
                        List<OleDbParameter> parms = new List<OleDbParameter>();
                        parms.Add(new OleDbParameter("@RoleId", SqlDbType.Int) { Value = roleinfo.Id });
                        parms.Add(new OleDbParameter("@Name", SqlDbType.VarChar) { Value = username });
                        SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sql, parms.ToArray());

                    }
                }
            }

        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            string[] allRoles = GetAllRoles();

            // See if any of the given roles match the available roles
            List<string> roles = new List<string>(allRoles.Intersect(roleNames));
            // There were some roles left after removing non-existent ones
            if (roles.Any())
            {
                foreach (string role in roles)
                {
                    string sql = "DELETE FROM snitz_UsersInRoles WHERE " +
                                       "RoleId=(SELECT RoleId FROM snitz_Roles WHERE LoweredRolename=@Rolename) AND " +
                                       "UserId = (SELECT MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_NAME=@Name)";
                    foreach (string username in usernames)
                    {
                        List<OleDbParameter> parms = new List<OleDbParameter>();
                        parms.Add(new OleDbParameter("@Rolename", SqlDbType.VarChar) { Value = role.ToLower() });
                        parms.Add(new OleDbParameter("@Name", SqlDbType.VarChar) { Value = username });
                        SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, sql, parms.ToArray());
                    }
                }
            }
        }

        public bool IsUserInRole(string username, string roleName)
        {
            string strSql = "SELECT COUNT(M.MEMBER_ID) " +
                      "FROM (snitz_UsersInRoles AS UR INNER JOIN " +
                      "snitz_Roles AS R ON UR.RoleId = R.RoleId) INNER JOIN " +
                      Config.MemberTablePrefix + "MEMBERS AS M ON UR.UserId = M.MEMBER_ID " +
                      "WHERE M.M_NAME=@Username AND R.RoleName=@Rolename";
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@Username", SqlDbType.VarChar)
                                           {
                                               Value=username
                                           },
                                           new OleDbParameter("@Rolename", SqlDbType.VarChar)
                                           {
                                               Value=roleName
                                           }
                                       };

            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public bool RoleExists(string roleName)
        {
            const string strSql = "SELECT COUNT(RoleId) " +
                      "FROM snitz_Roles " +
                      "WHERE LoweredRoleName=@LoweredRole";
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@LoweredRole", SqlDbType.VarChar)
                                           {
                                               Value = roleName.ToLower()
                                           }
                                       };

            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public bool RoleExists(int roleid)
        {
            const string strSql = "SELECT COUNT(RoleId) " +
                      "FROM snitz_Roles " +
                      "WHERE RoleId=@RoleId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@RoleId", SqlDbType.Int)
                                           {
                                               Value = roleid
                                           }
                                       };

            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public string[] GetAllRoles()
        {
            const string strSql = "SELECT RoleName FROM snitz_Roles";
            List<string> rolenames = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    rolenames.Add(rdr.GetString(0));
                }
            }
            return rolenames.ToArray();
        }

        public string[] GetRolesForUser(string username)
        {
            List<string> rolenames = new List<string>();
            string strSql = "SELECT R.RoleName " +
                                  "FROM (snitz_UsersInRoles AS UR INNER JOIN " +
                                  "snitz_Roles AS R ON UR.RoleId = R.RoleId) INNER JOIN " +
                                  Config.MemberTablePrefix + "MEMBERS AS M ON UR.UserId = M.MEMBER_ID " +
                                  "WHERE M.M_NAME=@Username";
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Username", SqlDbType.VarChar) { Value = username }))
            {
                while (rdr.Read())
                {
                    rolenames.Add(rdr.GetString(0));
                }
            }
            return rolenames.ToArray();
        }

        public string[] GetUsersInRole(string roleName)
        {
            List<string> usernames = new List<string>();
            string strSql = "SELECT M.M_NAME " +
                                  "FROM (snitz_UsersInRoles AS UR INNER JOIN " +
                                  "snitz_Roles AS R ON UR.RoleId = R.RoleId) INNER JOIN " +
                                  Config.MemberTablePrefix + "MEMBERS AS M ON UR.UserId = M.MEMBER_ID " +
                                  "WHERE R.Rolename=@Rolename";

            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Rolename", SqlDbType.VarChar) { Value = roleName }))
            {
                while (rdr.Read())
                {
                    usernames.Add(rdr.GetString(0));
                }
            }
            return usernames.ToArray();
        }

        public string[] GetForumRoles(int forumId)
        {
            string strForumRolesSql = "SELECT snitz_Roles.RoleName FROM " + Config.ForumTablePrefix + "ROLES FR INNER JOIN snitz_Roles " +
                                            "ON FR.Role_Id = snitz_Roles.RoleId WHERE (FR.Forum_id=@ForumId)";

            List<string> currentroles = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strForumRolesSql, new OleDbParameter("@ForumId", SqlDbType.Int) { Value = forumId }))
            {
                while (rdr.Read())
                {
                    currentroles.Add(rdr.GetString(0));
                }
            }
            return currentroles.ToArray();
        }

        public bool IsUserForumModerator(string username, int forumid)
        {
            string strSql = "SELECT COUNT(MOD_ID) " +
                                  "FROM " + Config.ForumTablePrefix + "MODERATOR FM INNER JOIN " + Config.MemberTablePrefix + "MEMBERS M ON FM.MEMBER_ID = M.MEMBER_ID " +
                                  "WHERE FM.FORUM_ID=@ForumId AND M.M_NAME=@Username";
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@ForumId", SqlDbType.Int)
                                           {
                                               Value
                                                   =
                                                   forumid
                                           },
                                           new OleDbParameter("@Username", SqlDbType.VarChar)
                                           {
                                               Value
                                                   =
                                                   username
                                           }
                                       };

            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public Dictionary<int, string> ListAllRolesForUser(string username)
        {
            string strSql = "SELECT snitz_Roles.RoleId, snitz_Roles.RoleName " +
                                  "FROM (snitz_Roles INNER JOIN " +
                                  "snitz_UsersInRoles ON snitz_Roles.RoleId = snitz_UsersInRoles.RoleId) INNER JOIN " +
                                  Config.MemberTablePrefix + "MEMBERS M ON snitz_UsersInRoles.UserId = M.MEMBER_ID " +
                                  "WHERE (M.M_NAME = @Username)";

            Dictionary<int, string> roles = new Dictionary<int, string>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Username", SqlDbType.VarChar) { Value = username }))
            {
                while (rdr.Read())
                {
                    roles.Add(rdr.GetInt32(0), rdr.GetString(1));
                }
            }
            return roles;
        }

        public bool IsUserInForumRole(string username, int forumid)
        {
            return IsUserInRole(username, "Forum_" + forumid);
        }

        public void UpdateRoleInfo(int roleid, string name, string description)
        {
            const string strSql = "UPDATE snitz_Roles SET Rolename=@RoleName,Description=@Description,LoweredRolename=@loweredname WHERE RoleId=@RoleId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@RoleId", SqlDbType.Int)
                                           {
                                               Value =roleid
                                           },
                                           new OleDbParameter("@RoleName", SqlDbType.VarChar)
                                           {
                                               Value=name
                                           },
                                           new OleDbParameter("@loweredname", SqlDbType.VarChar)
                                           {
                                               Value=name.ToLower()
                                           },
                                           new OleDbParameter("@Description", SqlDbType.VarChar)
                                           {
                                               Value=description
                                           }
                                       };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public IEnumerable<RoleInfo> GetAllRolesFull()
        {
            const string strSql = "SELECT RoleId,RoleName,Description FROM snitz_Roles";
            List<RoleInfo> roles = new List<RoleInfo>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    roles.Add(new RoleInfo
                              {
                                  Id = rdr.GetInt32(0),
                                  RoleName = rdr.GetString(1),
                                  Description = rdr.GetString(2)
                              });
                }
            }
            return roles;
        }

        public RoleInfo GetRole(int roleid)
        {
            const string strSql = "SELECT RoleId,RoleName,Description FROM snitz_Roles WHERE RoleId=@Roleid";
            RoleInfo role = null;
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Roleid", SqlDbType.Int) { Value = roleid }))
            {
                while (rdr.Read())
                {
                    role = new RoleInfo
                           {
                               Id = rdr.GetInt32(0),
                               RoleName = rdr.GetString(1),
                               Description = rdr.GetString(2)
                           };
                }
            }
            return role;
        }
        public RoleInfo GetRole(string rolename)
        {
            const string strSql = "SELECT RoleId,RoleName,Description FROM snitz_Roles WHERE RoleName=@RoleName";
            RoleInfo role = null;
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@RoleName", SqlDbType.VarChar) { Value = rolename }))
            {
                while (rdr.Read())
                {
                    role = new RoleInfo
                    {
                        Id = rdr.GetInt32(0),
                        RoleName = rdr.GetString(1),
                        Description = rdr.GetString(2)
                    };
                }
            }
            return role;
        }
        public void AddRolesToForum(int forumId, string[] newroles)
        {
            string strForumRolesSql = "SELECT snitz_Roles.RoleName FROM " + Config.ForumTablePrefix + "ROLES FR INNER JOIN snitz_Roles " +
                                            "ON FR.Role_Id = snitz_Roles.RoleId WHERE (FR.Forum_id=@ForumId)";

            string strRemoveSql = "DELETE FROM " + Config.ForumTablePrefix + "ROLES WHERE FORUM_ID=@ForumId AND ROLE_ID NOT IN " +
                               "(SELECT ROLE_ID FROM snitz_Roles WHERE RoleName IN ([newroles]))";
            string newrolelist = "";
            foreach (string role in newroles)
            {
                if (String.IsNullOrEmpty(newrolelist))
                    newrolelist = "'" + role + "'";
                else
                    newrolelist += ",'" + role + "'";
            }
            strRemoveSql = strRemoveSql.Replace("[newroles]", newrolelist);
            //remove roles
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strRemoveSql, new OleDbParameter("@ForumId", SqlDbType.Int) { Value = forumId });

            //fetch current roles
            List<string> currentroles = new List<string>();
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strForumRolesSql, new OleDbParameter("@ForumId", SqlDbType.Int) { Value = forumId }))
            {
                while (rdr.Read())
                {
                    currentroles.Add(rdr.GetString(0));
                }
            }
            List<string> rolestoadd = new List<string>();
            foreach (string role in newroles)
            {
                //if not in the current list then it needs adding
                if (!currentroles.Contains(role))
                    rolestoadd.Add(role);
            }

            foreach (string role in rolestoadd)
            {
                string strInsertSql = "INSERT INTO " + Config.ForumTablePrefix + "ROLES (FORUM_ID,ROLE_ID) SELECT @ForumId, RoleId FROM snitz_Roles WHERE RoleName=@Rolename";
                List<OleDbParameter> parms = new List<OleDbParameter>
                                           {
                                               new OleDbParameter("@ForumId", SqlDbType.Int)
                                               {
                                                   Value=forumId
                                               },
                                               new OleDbParameter("@Rolename", SqlDbType.VarChar)
                                               {
                                                   Value=role
                                               }
                                           };
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strInsertSql, parms.ToArray());
            }
        }
    }
}
