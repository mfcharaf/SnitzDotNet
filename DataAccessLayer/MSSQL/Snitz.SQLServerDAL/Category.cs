using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;

namespace Snitz.SQLServerDAL
{
    public class Category : ICategory
    {

        #region ICategory Members

        //public IEnumerable<ForumInfo> GetForums(int catid)
        //{
        //    return new Forum().GetByParent(catid,0,1000);
        //}

        public void SetStatus(int catid, int status)
        {
            const string strSql = "UPDATE FORUM_CATEGORY SET CAT_STATUS=@Status WHERE CAT_ID=@CatId";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = catid},
                    new SqlParameter("@Status", SqlDbType.Int) {Value = status}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public bool HasPosts(int catid)
        {
            const string strSql = "SELECT  SUM(Posts) totalPosts FROM " +
                                  "( " +
                                  "    SELECT COUNT(CAT_ID) AS Posts FROM FORUM_REPLY WHERE CAT_ID=@CatId" +
                                  "    UNION ALL" +
                                  "    SELECT COUNT(CAT_ID) AS Posts FROM FORUM_TOPICS WHERE CAT_ID=@CatId" +
                                  ") s";
            int res =
                Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql,new SqlParameter("@CatId", SqlDbType.Int) {Value = catid}));
            return res > 0;
        }

        public IEnumerable<CategoryInfo> GetByParent(int groupid)
        {
            const string strSql = "SELECT CAT_ID, CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER " +
                                  "FROM FORUM_CATEGORY ORDER BY CAT_ORDER, CAT_NAME " +
                                  "WHERE CAT_ID IN (SELECT GROUP_CATID FROM FORUM_GROUPS WHERE GROUP_ID=@GroupId";
            List<CategoryInfo> categories = new List<CategoryInfo>();
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@GroupId", SqlDbType.Int) {Value = groupid}
                };
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    categories.Add(BoHelper.CopyCategoryToBO(rdr));
                }
            }
            return categories;
        }

        #endregion

        #region IBaseObject<CategoryInfo> Members

        public CategoryInfo GetById(int catid)
        {
            CategoryInfo category = null;
            const string strSql = "SELECT CAT_ID, CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER FROM FORUM_CATEGORY WHERE CAT_ID = @CatId ORDER BY CAT_ORDER, CAT_NAME";
            SqlParameter parm = new SqlParameter("@CatId", SqlDbType.Int) { Value = catid };

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    category = BoHelper.CopyCategoryToBO(rdr);
                }
            }
            return category;
        }

        public IEnumerable<CategoryInfo> GetByName(string name)
        {
            List<CategoryInfo> categories = new List<CategoryInfo>();
            const string strSql = "SELECT CAT_ID, CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER FROM FORUM_CATEGORY WHERE CAT_NAME = @Name ORDER BY CAT_ORDER, CAT_NAME";
            SqlParameter parm = new SqlParameter("@Name", SqlDbType.Int) { Value = name };

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    categories.Add(BoHelper.CopyCategoryToBO(rdr));
                }
            }
            return categories;
        }

        public int Add(CategoryInfo cat)
        {
            const string strSql = "INSERT INTO FORUM_CATEGORY (CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER) VALUES (@Name,@Status,@ModLevel,@SubsLevel,@Order); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = cat.Name},
                    new SqlParameter("@Status", SqlDbType.Int) {Value = cat.Status},
                    new SqlParameter("@ModLevel", SqlDbType.Int) {Value = cat.ModerationLevel},
                    new SqlParameter("@SubsLevel", SqlDbType.Int) {Value = cat.SubscriptionLevel},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = cat.Order}
                };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Update(CategoryInfo cat)
        {
            const string strSql = "UPDATE FORUM_CATEGORY SET " +
                                  "CAT_NAME=@Name, CAT_STATUS=@Status, CAT_MODERATION=@ModLevel, CAT_SUBSCRIPTION=@SubsLevel, CAT_ORDER=@Order " +
                                  "WHERE CAT_ID=@CatId";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = cat.Id},
                    new SqlParameter("@Name", SqlDbType.NVarChar) {Value = cat.Name},
                    new SqlParameter("@Status", SqlDbType.Int) {Value = cat.Status},
                    new SqlParameter("@ModLevel", SqlDbType.Int) {Value = cat.ModerationLevel},
                    new SqlParameter("@SubsLevel", SqlDbType.Int) {Value = cat.SubscriptionLevel},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = cat.Order}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Delete(CategoryInfo cat)
        {
            const string strSql =
                "DELETE FROM FORUM_A_REPLY WHERE CAT_ID = @CatId; " +
                "DELETE FROM FORUM_A_TOPICS WHERE CAT_ID = @CatId; " +
                "DELETE FROM FORUM_REPLY WHERE CAT_ID = @CatId; " +
                "DELETE FROM FORUM_TOPICS WHERE CAT_ID=@CatId; " +
                "DELETE FROM FORUM_FORUM WHERE CAT_ID=@CatId; " +
                "DELETE FROM FORUM_CATEGORY WHERE CAT_ID=@CatId";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@CatId",SqlDbType.Int){Value = cat.Id});
        }

        public void Dispose()
        {
            
        }

        #endregion

        public IEnumerable<CategoryInfo> GetAll()
        {
            const string strSql =
                "SELECT CAT_ID, CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER FROM FORUM_CATEGORY ORDER BY CAT_ORDER, CAT_NAME";
            List<CategoryInfo> categories = new List<CategoryInfo>();

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    categories.Add(BoHelper.CopyCategoryToBO(rdr));
                }
            }
            return categories;
        }

    }
}
