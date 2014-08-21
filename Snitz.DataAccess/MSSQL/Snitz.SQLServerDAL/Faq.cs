using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class Faq : IFaqCategory, IFaqQuestion
    {

        #region IFaqCategory Members

        public IEnumerable<FaqCategoryInfo> GetCategories(string lang)
        {
            string strSql = "SELECT FC_ID,FC_DESCRIPTION,FC_ORDER,FC_ROLES FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_LANG_ID = @Lang ORDER BY FC_ORDER,FC_DESCRIPTION";
            List<FaqCategoryInfo> faqcategories = new List<FaqCategoryInfo>();

            SqlParameter parm = new SqlParameter("@Lang", SqlDbType.VarChar,6) {Value = lang};

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    FaqCategoryInfo faqcat = new FaqCategoryInfo
                        {
                            Id=rdr.GetInt32(0),
                            Description = rdr.GetString(1),
                            Language = lang,
                            Order = rdr.GetInt32(2),
                            Roles = rdr.SafeGetString(3)
                        };
                    faqcategories.Add(faqcat);
                }
            }
            return faqcategories;
        }

        #endregion

        #region IBaseObject<FaqCategoryInfo> Members

        public FaqCategoryInfo GetById(int catid)
        {
            string strSql = "SELECT FC_ID,FC_DESCRIPTION,FC_ORDER,FC_LANG_ID,FC_ROLES FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_ID = @CatId";
            FaqCategoryInfo faqcat = null;
            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@CatId", SqlDbType.Int) { Value = catid }))
            {
                while (rdr.Read())
                {
                    faqcat = new FaqCategoryInfo
                    {
                        Id = rdr.GetInt32(0),
                        Description = rdr.SafeGetString(1),
                        Language = rdr.SafeGetString(3),
                        Order = rdr.GetInt32(2),
                        Roles = rdr.SafeGetString(4)
                    };

                }
            }
            return faqcat;
        }

        public IEnumerable<FaqCategoryInfo> GetByName(string name)
        {
            string strSql = "SELECT FC_ID,FC_DESCRIPTION,FC_ORDER,FC_LANG_ID,FC_ROLES FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_DESCRIPTION=@Name";
            List<FaqCategoryInfo> faqcat = new List<FaqCategoryInfo>();
            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name }))
            {
                while (rdr.Read())
                {
                    faqcat.Add(new FaqCategoryInfo
                    {
                        Id = rdr.GetInt32(0),
                        Description = rdr.SafeGetString(1),
                        Language = rdr.SafeGetString(3),
                        Order = rdr.GetInt32(2),
                        Roles = rdr.SafeGetString(4)
                    });

                }
            }
            return faqcat;
        }

        public int Add(FaqCategoryInfo cat)
        {
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "FAQ_CAT (FC_DESCRIPTION,FC_ORDER,FC_LANG_ID,FC_ROLES) VALUES (@Description,@Order,@Lang,@Roles); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Description", SqlDbType.VarChar) {Value = cat.Description},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = cat.Order},
                    new SqlParameter("@Lang", SqlDbType.VarChar) {Value = cat.Language},
                    new SqlParameter("@Roles", SqlDbType.VarChar) {Value = cat.Roles}
                };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Delete(FaqCategoryInfo faqcat)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_CAT=@CatId; DELETE FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_ID=@CatId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@CatId", SqlDbType.Int) { Value = faqcat.Id });
        }

        public void Update(FaqCategoryInfo faqcat)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "FAQ_CAT SET FC_DESCRIPTION=@Description,FC_ORDER=@Order,FC_LANG_ID=@Lang,FC_ROLES=@Roles WHERE FC_ID=@CatId";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = faqcat.Id},
                    new SqlParameter("@Description", SqlDbType.VarChar) {Value = faqcat.Description},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = faqcat.Order},
                    new SqlParameter("@Lang", SqlDbType.VarChar) {Value = faqcat.Language},
                    new SqlParameter("@Roles", SqlDbType.VarChar) {Value = faqcat.Roles}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void Dispose()
        {

        }

        #endregion



        #region IFaqQuestion Members

        public IEnumerable<FaqInfo> GetFaqItems(int catid, string lang)
        {
            string strSql =
                "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER  FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_CAT = @CatId AND FI_LANG_ID = @Lang ORDER BY FI_ORDER, FI_LINK_TITLE";
            List<FaqInfo> faqquestions = new List<FaqInfo>();
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Lang", SqlDbType.VarChar, 6) {Value = lang},
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = catid}
                };

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    faqquestions.Add(BoHelper.CopyFaqQuestionsToBO(rdr));
                }
            }
            return faqquestions;
        }

        public IEnumerable<FaqInfo> FindFaqItem(string searchfor, string lang)
        {
            string strSql =
                "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER  FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_LANG_ID = @Lang AND (FI_LINK_BODY LIKE @SearchFor OR FI_LINK_TITLE LIKE @SearchFor)  ORDER BY FI_ORDER, FI_LINK_TITLE";
            List<FaqInfo> faqquestions = new List<FaqInfo>();
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Lang", SqlDbType.VarChar, 6) {Value = lang},
                    new SqlParameter("@SearchFor", SqlDbType.VarChar) {Value = "%" + searchfor + "%"}
                };

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    faqquestions.Add(BoHelper.CopyFaqQuestionsToBO(rdr));
                }
            }
            return faqquestions;
        }

        public Dictionary<int, string> GetQuestions(int catid, string filter, string lang)
        {
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_CAT = @CatId AND FI_LANG_ID = @Lang ORDER BY FI_ORDER, FI_LINK_TITLE";
            Dictionary<int, string> questions = new Dictionary<int, string>();
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Lang", SqlDbType.VarChar, 6) {Value = lang},
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = catid}
                };

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    FaqInfo faq = new FaqInfo
                        {
                        Id = rdr.GetInt32(0),
                        Link = rdr.SafeGetString(1),
                        LinkTitle = rdr.SafeGetString(2)
                    };
                    if(String.IsNullOrEmpty(filter) || rdr.SafeGetString(3).Contains(filter))
                    questions.Add(faq.Id,faq.LinkTitle);
                }
            }
            return questions;
        }

        public FaqInfo GetFaqItem(int faqid, string lang)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            FaqInfo faq = null;
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID = @FaqId AND FI_LANG_ID = @Lang ORDER BY FI_ORDER, FI_LINK_TITLE";
            
            parms.Add(new SqlParameter("@Lang", SqlDbType.VarChar, 6) { Value = lang });
            parms.Add(new SqlParameter("@FaqId", SqlDbType.Int) { Value = faqid });

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    faq = BoHelper.CopyFaqQuestionsToBO(rdr);
                }
            }
            return faq;
        }

        #endregion

        #region IBaseObject<FaqInfo> Members

        FaqInfo IBaseObject<FaqInfo>.GetById(int id)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            FaqInfo faq = null;
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID = @FaqId ORDER BY FI_ORDER, FI_LINK_TITLE";

            parms.Add(new SqlParameter("@FaqId", SqlDbType.Int) { Value = id });

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    faq = BoHelper.CopyFaqQuestionsToBO(rdr);
                }
            }
            return faq;
        }

        IEnumerable<FaqInfo> IBaseObject<FaqInfo>.GetByName(string name)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            FaqInfo faq = null;
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_LINK = @FaqLink ";

            parms.Add(new SqlParameter("@FaqLink", SqlDbType.NVarChar) { Value = name });

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    faq = BoHelper.CopyFaqQuestionsToBO(rdr);

                }
            }
            return new[] {faq};
        }

        public int Add(FaqInfo faq)
        {
            string insertSql = "INSERT INTO " + Config.ForumTablePrefix + "FAQ_INFO (FI_ORDER,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID) VALUES ";
            insertSql = insertSql + "(@Order,@Link,@LinkTitle,@LinkBody,@CatId,@Lang); ";
            insertSql = insertSql + "SELECT SCOPE_IDENTITY();";

            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@Order", SqlDbType.Int) {Value = faq.Order},
                    new SqlParameter("@Link", SqlDbType.NVarChar)
                        {
                            Value = (object) faq.Link ?? DBNull.Value,
                            IsNullable = true
                        },
                    new SqlParameter("@LinkTitle", SqlDbType.NVarChar) {Value = faq.LinkTitle},
                    new SqlParameter("@LinkBody", SqlDbType.NVarChar) {Value = faq.LinkBody},
                    new SqlParameter("@Lang", SqlDbType.VarChar, 6) {Value = faq.Language},
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = faq.CatId}
                };

            int faqid = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, insertSql, parms.ToArray()));
            return faqid;
        }

        public void Update(FaqInfo faq)
        {
            StringBuilder updateFaq = new StringBuilder();
            updateFaq.AppendFormat("UPDATE {0}FAQ_INFO SET ", Config.ForumTablePrefix).AppendLine();
            updateFaq.AppendLine("FI_CAT=@CatId,");
            updateFaq.AppendLine("FI_LINK_TITLE=@LinkTitle,");
            updateFaq.AppendLine("FI_LINK_BODY=@LinkBody,");
            updateFaq.AppendLine("FI_LANG_ID=@Lang,");
            updateFaq.AppendLine("FI_ORDER=@Order ");
            updateFaq.AppendLine("WHERE FI_ID=@FaqId");

            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@CatId", SqlDbType.Int) {Value = faq.CatId},
                    new SqlParameter("@LinkTitle", SqlDbType.NVarChar) {Value = faq.LinkTitle},
                    new SqlParameter("@LinkBody", SqlDbType.NVarChar) {Value = faq.LinkBody},
                    new SqlParameter("@Lang", SqlDbType.VarChar, 6) {Value = faq.Language},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = faq.Order},
                    new SqlParameter("@FaqId", SqlDbType.Int) {Value = faq.Id}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateFaq.ToString(), parms.ToArray());
        }

        public void Delete(FaqInfo faq)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID=@FaqId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@FaqId", SqlDbType.Int) { Value = faq.Id });
        }

        #endregion
    }
}
