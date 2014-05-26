using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzCommon;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class Group : IGroup
    {

        #region IGroup Members

        public void AddCategory(int groupid, int categoryid)
        {
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "GROUPS (GROUP_ID, GROUP_CATID) VALUES (@GroupId,@CatId)";
            List<OleDbParameter> parms = new List<OleDbParameter>();
            parms.Add(new OleDbParameter("@GroupId", OleDbType.Numeric) { Value = groupid });
            parms.Add(new OleDbParameter("@CatId", OleDbType.Numeric) { Value = categoryid });


            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public IEnumerable<GroupInfo> GetAll()
        {
            string strSql = "SELECT GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE FROM " + Config.ForumTablePrefix + "GROUP_NAMES";
            List<GroupInfo> groups = new List<GroupInfo>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    groups.Add(new GroupInfo
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.GetString(1),
                        Description = rdr.SafeGetString(2),
                        Icon = rdr.SafeGetString(3),
                        Image = rdr.SafeGetString(4)
                    });
                }
            }
            return groups;
        }

        public IEnumerable<CategoryInfo> GetCategories(int groupid)
        {
            return new Category().GetByParent(groupid);
        }

        #endregion

        #region IBaseObject<GroupInfo> Members

        public GroupInfo GetById(int id)
        {
            string strSql = "SELECT GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE FROM " + Config.ForumTablePrefix + "GROUP_NAMES WHERE GROUP_ID=@Group";
            GroupInfo group = null;

            OleDbParameter parm = new OleDbParameter("@Group", OleDbType.Numeric) { Value = id };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    group = new GroupInfo
                    {
                        Id = rdr.GetInt32(0),
                        Name = rdr.GetString(1),
                        Description = rdr.SafeGetString(2),
                        Icon = rdr.SafeGetString(3),
                        Image = rdr.SafeGetString(4)
                    };
                }
            }
            return group;
        }

        public IEnumerable<GroupInfo> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public int Add(GroupInfo group)
        {
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "GROUP_NAMES (GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE) VALUES (@Name,@Description,@Icon,@Image)";
            List<OleDbParameter> parms = new List<OleDbParameter>();
            parms.Add(new OleDbParameter("@Name", OleDbType.VarChar) { Value = group.Name });
            parms.Add(new OleDbParameter("@Description", OleDbType.VarChar) { Value = group.Name });
            parms.Add(new OleDbParameter("@Icon", OleDbType.VarChar) { Value = group.Icon.ConvertDBNull(), IsNullable = true });
            parms.Add(new OleDbParameter("@Image", OleDbType.VarChar) { Value = group.Name.ConvertDBNull(), IsNullable = true });

            return Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Update(GroupInfo group)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "GROUP_NAMES SET GROUP_NAME=@Name,GROUP_DESCRIPTION=@Description,GROUP_ICON=@Icon,GROUP_IMAGE=@Image) WHERE GROUP_ID=@GroupId";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@GroupId", OleDbType.Numeric) {Value = @group.Id},
                new OleDbParameter("@Name", OleDbType.VarChar) {Value = @group.Name},
                new OleDbParameter("@Description", OleDbType.VarChar) {Value = @group.Name},
                new OleDbParameter("@Icon", OleDbType.VarChar) {Value = @group.Icon.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Image", OleDbType.VarChar) {Value = @group.Name.ConvertDBNull(), IsNullable = true}
            };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Delete(GroupInfo group)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "GROUPS WHERE GROUP_ID=@GroupId; DELETE FROM FORUM_GROUP_NAMES WHERE GROUP_ID=@GroupId";
            List<OleDbParameter> parms = new List<OleDbParameter>
            {
                new OleDbParameter("@GroupId", OleDbType.Numeric) {Value = @group.Id}
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
