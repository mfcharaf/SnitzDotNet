using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class Faq : IFaqCategory, IFaqQuestion
    {

        #region IFaqCategory Members

        public IEnumerable<FaqCategoryInfo> GetCategories(string lang)
        {
            string strSql = "SELECT FC_ID,FC_DESCRIPTION,FC_ORDER,FC_ROLES FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_LANG_ID = @Lang ORDER BY FC_ORDER,FC_DESCRIPTION";
            List<FaqCategoryInfo> faqcategories = new List<FaqCategoryInfo>();

            OleDbParameter parm = new OleDbParameter("@Lang", OleDbType.VarChar) { Value = lang };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parm))
            {
                while (rdr.Read())
                {
                    FaqCategoryInfo faqcat = new FaqCategoryInfo
                        {
                            Id = rdr.GetInt32(0),
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
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@CatId", OleDbType.Integer) { Value = catid }))
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
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Name", OleDbType.VarChar) { Value = name }))
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
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "FAQ_CAT (FC_DESCRIPTION,FC_ORDER,FC_LANG_ID,FC_ROLES) VALUES (@Description,@Order,@Lang,@Roles)";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@Description", OleDbType.VarChar) {Value = cat.Description},
                    new OleDbParameter("@Order", OleDbType.Integer) {Value = cat.Order},
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = cat.Language},
                    new OleDbParameter("@Roles", OleDbType.VarChar) {Value = cat.Roles}
                };

            return Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));
        }

        public void Delete(FaqCategoryInfo faqcat)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_CAT=@CatId; DELETE FROM " + Config.ForumTablePrefix + "FAQ_CAT WHERE FC_ID=@CatId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@CatId", OleDbType.Integer) { Value = faqcat.Id });
        }

        public void Update(FaqCategoryInfo faqcat)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "FAQ_CAT SET FC_DESCRIPTION=@Description,FC_ORDER=@Order,FC_LANG_ID=@Lang,FC_ROLES=@Roles WHERE FC_ID=@CatId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@Description", OleDbType.VarChar) {Value = faqcat.Description},
                    new OleDbParameter("@Order", OleDbType.Integer) {Value = faqcat.Order},
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = faqcat.Language},
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = faqcat.Id},
                    new OleDbParameter("@Roles", OleDbType.Integer) {Value = faqcat.Roles}
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
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = catid},
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = lang}
                };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
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
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = lang},
                    new OleDbParameter("@SearchFor", OleDbType.VarChar) {Value = "%" + searchfor + "%"}
                };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
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
            string strSql = "SELECT FI_ID,FI_CAT,FI_ORDER,FI_LANG_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY  FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_CAT = @CatId AND FI_LANG_ID = @Lang ORDER BY FI_ORDER, FI_LINK_TITLE";
            Dictionary<int, string> questions = new Dictionary<int, string>();
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = catid},
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = lang}
                };

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    FaqInfo faq = new FaqInfo
                        {
                            Id = rdr.SafeGetInt32("FI_ID"),
                            Link = rdr.SafeGetString("FI_LINK"),
                            LinkTitle = rdr.SafeGetString("FI_LINK_TITLE")
                        };
                    questions.Add(faq.Id, faq.LinkTitle);
                }
            }
            return questions;
        }

        public FaqInfo GetFaqItem(int faqid, string lang)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            FaqInfo faq = null;
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER  FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID = @FaqId AND FI_LANG_ID = @Lang ORDER BY FI_ORDER, FI_LINK_TITLE";

            
            parms.Add(new OleDbParameter("@FaqId", OleDbType.Integer) { Value = faqid });
            parms.Add(new OleDbParameter("@Lang", OleDbType.VarChar) { Value = lang });
            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
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
            List<OleDbParameter> parms = new List<OleDbParameter>();
            FaqInfo faq = null;
            string strSql = "SELECT FI_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY,FI_CAT,FI_LANG_ID,FI_ORDER  FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID = @FaqId ORDER BY FI_ORDER, FI_LINK_TITLE";

            parms.Add(new OleDbParameter("@FaqId", OleDbType.Integer) { Value = id });

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
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
            throw new NotImplementedException();
        }

        public int Add(FaqInfo faq)
        {
            string insertSql = "INSERT INTO " + Config.ForumTablePrefix + "FAQ_INFO (FI_ORDER,FI_CAT,FI_LANG_ID,FI_LINK,FI_LINK_TITLE,FI_LINK_BODY) VALUES ";
            insertSql = insertSql + "(@Order,@CatId,@Lang,@Link,@LinkTitle,@LinkBody) ";

            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@Order", OleDbType.Integer) {Value = faq.Order},
                                        
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = faq.CatId},
                    new OleDbParameter("@Lang", OleDbType.VarChar) {Value = faq.Language},
                    new OleDbParameter("@Link", OleDbType.VarChar)
                        {
                            Value = (object) faq.Link ?? DBNull.Value,
                            IsNullable = true
                        },
                    new OleDbParameter("@LinkTitle", OleDbType.VarChar) {Value = faq.LinkTitle},
                    new OleDbParameter("@LinkBody", OleDbType.VarChar) {Value = faq.LinkBody}

                };

            int faqid = Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, insertSql, parms.ToArray()));
            return faqid;
        }

        public void Update(FaqInfo faq)
        {
            StringBuilder updateFaq = new StringBuilder("");
            updateFaq.AppendFormat("UPDATE {0}FAQ_INFO SET ",Config.ForumTablePrefix).AppendLine();
            updateFaq.AppendLine("FI_CAT=@CatId,");
            updateFaq.AppendLine("FI_LINK_TITLE=@LinkTitle,");
            updateFaq.AppendLine("FI_LINK_BODY=@LinkBody,");
            updateFaq.AppendLine("FI_LANG_ID=@Lang,");
            updateFaq.AppendLine("FI_ORDER=@Order ");
            updateFaq.AppendLine("WHERE FI_ID=@FaqId");

            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = faq.CatId},
                    new OleDbParameter("@LinkTitle", OleDbType.VarChar) {Value = faq.LinkTitle},
                    new OleDbParameter("@LinkBody", OleDbType.VarChar) {Value = faq.LinkBody},
                    new OleDbParameter("@Lang", OleDbType.VarChar, 6) {Value = faq.Language},
                    new OleDbParameter("@Order", OleDbType.Integer) {Value = faq.Order},
                    new OleDbParameter("@FaqId", OleDbType.Integer) {Value = faq.Id}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateFaq.ToString(), parms.ToArray());
        }

        public void Delete(FaqInfo faq)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "FAQ_INFO WHERE FI_ID=@FaqId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@FaqId", OleDbType.Integer) { Value = faq.Id });
        }

        #endregion
    }
}
