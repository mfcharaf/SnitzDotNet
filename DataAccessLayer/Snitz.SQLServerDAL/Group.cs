using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzCommon;

namespace Snitz.SQLServerDAL
{
    public class Group : IGroup
    {

        #region IGroup Members

        public void AddCategory(int groupid, int categoryid)
        {
            const string strSql = "INSERT INTO FORUM_GROUPS (GROUP_ID, GROUP_CATID) VALUES (@GroupId,@CatId)";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@GroupId", SqlDbType.Int) { Value = groupid });
            parms.Add(new SqlParameter("@CatId", SqlDbType.Int) { Value = categoryid });


            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public IEnumerable<GroupInfo> GetAll()
        {
            const string strSql = "SELECT GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE FROM FORUM_GROUP_NAMES";
            List<GroupInfo> groups = new List<GroupInfo>();

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
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
            const string strSql = "SELECT GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE FROM FORUM_GROUP_NAMES WHERE GROUP_ID=@Group";
            GroupInfo group = null;

            SqlParameter parm = new SqlParameter("@Group", SqlDbType.Int) { Value = id };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    group = new GroupInfo
                    {
                        Id=rdr.GetInt32(0),
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
            const string strSql = "INSERT INTO FORUM_GROUP_NAMES (GROUP_NAME,GROUP_DESCRIPTION,GROUP_ICON,GROUP_IMAGE) VALUES (@Name,@Description,@Icon,@Image); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>();
            parms.Add(new SqlParameter("@Name",SqlDbType.NVarChar){Value = group.Name});
            parms.Add(new SqlParameter("@Description", SqlDbType.NVarChar) { Value = group.Name });
            parms.Add(new SqlParameter("@Icon", SqlDbType.NVarChar) { Value = group.Icon.ConvertDBNull(), IsNullable = true });
            parms.Add(new SqlParameter("@Image", SqlDbType.NVarChar) { Value = group.Name.ConvertDBNull(), IsNullable = true });

            return  Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Update(GroupInfo group)
        {
            const string strSql = "UPDATE FORUM_GROUP_NAMES SET GROUP_NAME=@Name,GROUP_DESCRIPTION=@Description,GROUP_ICON=@Icon,GROUP_IMAGE=@Image) WHERE GROUP_ID=@GroupId";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@GroupId", SqlDbType.Int) {Value = @group.Id},
                new SqlParameter("@Name", SqlDbType.NVarChar) {Value = @group.Name},
                new SqlParameter("@Description", SqlDbType.NVarChar) {Value = @group.Name},
                new SqlParameter("@Icon", SqlDbType.NVarChar) {Value = @group.Icon.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Image", SqlDbType.NVarChar) {Value = @group.Name.ConvertDBNull(), IsNullable = true}
            };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Delete(GroupInfo group)
        {
            const string strSql = "DELETE FROM FORUM_GROUPS WHERE GROUP_ID=@GroupId; DELETE FROM FORUM_GROUP_NAMES WHERE GROUP_ID=@GroupId;";
            List<SqlParameter> parms = new List<SqlParameter>
            {
                new SqlParameter("@GroupId", SqlDbType.Int) {Value = @group.Id}
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
