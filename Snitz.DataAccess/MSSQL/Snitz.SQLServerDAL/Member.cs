using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzCommon;
using SnitzConfig;

namespace Snitz.SQLServerDAL
{
    public class Member : IMember
    {
        private int _pendingMemberCount;
        private string SELECT_OVER = "WITH MemberEntities AS (SELECT ROW_NUMBER() OVER [ORDERBY] AS Row, MEMBER_ID FROM " + Config.MemberTablePrefix + "MEMBERS [WHERE]) ";
        private string SELECT_OVER_FROM = "FROM MemberEntities ME INNER JOIN " + Config.MemberTablePrefix + "MEMBERS M on ME.MEMBER_ID = M.MEMBER_ID " +
            " WHERE ME.Row Between @Start AND @MaxRows ORDER BY ME.Row ASC ";

        private const string MEMBER_SELECT =
            "SELECT M.MEMBER_ID,M.M_STATUS,M.M_NAME,M.M_USERNAME,M.M_EMAIL,M.M_COUNTRY,M.M_HOMEPAGE,M.M_SIG" +
            ",M.M_LEVEL,M.M_AIM,M.M_YAHOO,M.M_ICQ,M.M_MSN,M.M_POSTS,M.M_DATE,M.M_LASTHEREDATE,M.M_LASTPOSTDATE" +
            ",M.M_TITLE,M.M_SUBSCRIPTION,M.M_HIDE_EMAIL,M.M_RECEIVE_EMAIL,M.M_IP,M.M_VIEW_SIG,M.M_SIG_DEFAULT" +
            ",M.M_VOTED,M.M_ALLOWEMAIL,M.M_AVATAR,M.M_THEME,M.M_TIMEOFFSET,M.M_DOB,M_AGE,M_PASSWORD,M_KEY,M_VALID,M_LASTUPDATED " +
            ",M_MARSTATUS,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION,M_SEX,M_HOBBIES,M_LNEWS,M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_CITY,M_STATE ";

        private string FROM = " FROM " + Config.MemberTablePrefix + "MEMBERS M ";
        private const string WHERE_NAME = " WHERE M.M_NAME = @Username ";
        private const string WHERE_ID = " WHERE M.MEMBER_ID = @Id ";

        public MemberInfo GetById(int id)
        {
            MemberInfo member = null;
            SqlParameter parm = new SqlParameter("@Id", SqlDbType.Int) { Value = id };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + WHERE_ID, parm))
            {
                while (rdr.Read())
                {
                    member = BoHelper.CopyMemberToBO(rdr);
                }
            }
            return member ?? (new MemberInfo {Username = "Guest", TimeOffset = 0});
        }

        public IEnumerable<MemberInfo> GetByName(string name)
        {
            List<MemberInfo> members = new List<MemberInfo>();
            SqlParameter parm = new SqlParameter("@Username", SqlDbType.VarChar) { Value = name };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + WHERE_NAME, parm))
            {
                while (rdr.Read())
                {
                    members.Add(BoHelper.CopyMemberToBO(rdr));
                }
            }
            if(members.Count < 1)
                members.Add(new MemberInfo {Username = "Guest", TimeOffset = 0});
            return members;
        }

        public int Add(MemberInfo member)
        {
            List<SqlParameter> memberparms;

            string strSql = "INSERT INTO " + Config.MemberTablePrefix + "MEMBERS " +
                                  "(M_NAME,M_STATUS,M_LEVEL,M_TITLE,M_EMAIL,M_PASSWORD,M_VALID,M_KEY,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION" +
                                  ",M_SEX,M_AGE,M_DOB,M_MARSTATUS,M_CITY,M_STATE,M_COUNTRY,M_HOMEPAGE,M_SIG" +
                                  ",M_HIDE_EMAIL,M_RECEIVE_EMAIL,M_VIEW_SIG,M_SIG_DEFAULT,M_HOBBIES,M_LNEWS" +
                                  ",M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_AIM,M_YAHOO,M_ICQ,M_MSN,M_LAST_IP,M_LASTUPDATED" +
                                  ",M_LASTHEREDATE,M_AVATAR,M_THEME,M_TIMEOFFSET,M_DATE)" +
                                  " VALUES " +
                                  "(@Username,@Status,@Mlev,@Title,@Email,@Password,@IsValid,@ValidationKey,@Firstname,@Lastname" +
                                  ",@Occupation,@Gender,@Age,@Dob,@Maritalstatus,@City,@State,@Country" +
                                  ",@Homepage,@Signature,@Hidemail,@Receivemails,@Viewsignatures,@Usesignature" +
                                  ",@Hobbies,@Latestnews,@Favquote,@Bio,@Link1,@Link2,@Aim,@Yahoo,@Icq,@Msn" +
                                  ",@LastIP,@Lastupdated,@LastVisit,@Avatar,@Theme,@Timeoffset,@Created); " +
                                  "SELECT SCOPE_IDENTITY();";
            try
            {
                memberparms = new List<SqlParameter>
                              {
                    new SqlParameter("@Username", SqlDbType.NVarChar) {Value = member.Username},
                    new SqlParameter("@Status", SqlDbType.SmallInt) {Value = member.Status},
                    new SqlParameter("@Created", SqlDbType.VarChar) {Value = member.MemberSince.ToString("yyyyMMddHHmmss")},
                    new SqlParameter("@Mlev", SqlDbType.SmallInt) {Value = member.MemberLevel},
                    new SqlParameter("@Title", SqlDbType.NVarChar) {Value = member.Title.ConvertDBNull(),IsNullable = true},
                    new SqlParameter("@Password", SqlDbType.NVarChar) {Value = member.Password},
                    new SqlParameter("@IsValid", SqlDbType.Int) {Value = member.IsValid},
                    new SqlParameter("@ValidationKey", SqlDbType.NVarChar) {Value = member.ValidationKey.ConvertDBNull(),IsNullable = true},
                    new SqlParameter("@Email", SqlDbType.NVarChar) {Value = member.Email},
                    new SqlParameter("@Firstname", SqlDbType.NVarChar)
                    {
                        Value = member.Firstname.ConvertDBNull(),
                        IsNullable = true
                    },
                    new SqlParameter("@Lastname", SqlDbType.NVarChar)
                    {
                        Value = member.Lastname.ConvertDBNull(),
                        IsNullable = true
                    },
                    new SqlParameter("@Occupation", SqlDbType.NVarChar)
                    {
                        Value = member.Occupation.ConvertDBNull(),
                        IsNullable = true
                    },
                    new SqlParameter("@Gender", SqlDbType.NVarChar)
                    {
                        Value = member.Gender.ConvertDBNull(),
                        IsNullable = true
                    }

                };
                memberparms.Add(new SqlParameter("@Age", SqlDbType.NVarChar) { Value = member.Age.ConvertDBNull(), IsNullable = true });
                memberparms.Add(new SqlParameter("@Dob", SqlDbType.NVarChar)
                    {
                        Value = member.DateOfBirth.ConvertDBNull(),
                        IsNullable = true
                    });
                memberparms.Add(new SqlParameter("@Maritalstatus", SqlDbType.NVarChar)
                {
                    Value = member.MaritalStatus.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@City", SqlDbType.NVarChar) {Value = member.City.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@State", SqlDbType.NVarChar) {Value = member.State.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@Country", SqlDbType.NVarChar)
                {
                    Value = member.Country.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Homepage", SqlDbType.NVarChar)
                {
                    Value = member.HomePage.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Signature", SqlDbType.NVarChar)
                {
                    Value = member.Signature.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Hidemail", SqlDbType.Int) {Value = member.HideEmail});
                memberparms.Add(new SqlParameter("@Receivemails", SqlDbType.Int) {Value = member.ReceiveEmails});
                memberparms.Add(new SqlParameter("@Viewsignatures", SqlDbType.Int) {Value = member.ViewSignatures});
                memberparms.Add(new SqlParameter("@Usesignature", SqlDbType.Int) {Value = member.UseSignature});
                memberparms.Add(new SqlParameter("@Hobbies", SqlDbType.NVarChar)
                {
                    Value = member.Hobbies.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Latestnews", SqlDbType.NVarChar)
                {
                    Value = member.LatestNews.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Favquote", SqlDbType.NVarChar)
                {
                    Value = member.FavouriteQuote.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Bio", SqlDbType.NVarChar)
                {
                    Value = member.Biography.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Link1", SqlDbType.NVarChar)
                {
                    Value = member.FavLink1.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Link2", SqlDbType.NVarChar)
                {
                    Value = member.FavLink2.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Aim", SqlDbType.NVarChar) {Value = member.AIM.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@Yahoo", SqlDbType.NVarChar) {Value = member.Yahoo.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@Icq", SqlDbType.NVarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@Msn", SqlDbType.NVarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@LastIP", SqlDbType.NVarChar) {Value = Common.GetIP4Address()});
                memberparms.Add(new SqlParameter("@Lastupdated", SqlDbType.NVarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                });
                memberparms.Add(new SqlParameter("@Lastvisit", SqlDbType.NVarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                });
                memberparms.Add(new SqlParameter("@Avatar", SqlDbType.NVarChar)
                {
                    Value = member.Avatar.ConvertDBNull(),
                    IsNullable = true
                });
                memberparms.Add(new SqlParameter("@Theme", SqlDbType.NVarChar) {Value = member.Theme.ConvertDBNull(), IsNullable = true});
                memberparms.Add(new SqlParameter("@Timeoffset", SqlDbType.Int) { Value = member.TimeOffset });

            
            }
            catch (Exception)
            {
                return 0;
                throw;
            }

            int res = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, memberparms.ToArray()));
            return res;
        }

        public void Update(MemberInfo member)
        {
            if (member.Username.ToLower() == "guest")
                return;
            StringBuilder updateSql = new StringBuilder();
            updateSql.AppendFormat("UPDATE {0}MEMBERS SET",Config.MemberTablePrefix).AppendLine();
            updateSql.AppendLine("M_TITLE=@Title");
            updateSql.AppendLine(",M_EMAIL=@Email");
            updateSql.AppendLine(",M_PASSWORD=@Password");
            updateSql.AppendLine(",M_VALID=@IsValid");
            updateSql.AppendLine(",M_KEY=@ValidationKey");
            updateSql.AppendLine(",M_FIRSTNAME=@Firstname");
            updateSql.AppendLine(",M_LASTNAME=@Lastname");
            updateSql.AppendLine(",M_OCCUPATION=@Occupation");
            updateSql.AppendLine(",M_SEX=@Gender");
            updateSql.AppendLine(",M_AGE=@Age");
            updateSql.AppendLine(",M_DOB=@Dob");
            updateSql.AppendLine(",M_MARSTATUS=@Maritalstatus");
            updateSql.AppendLine(",M_CITY=@City");
            updateSql.AppendLine(",M_STATE=@State");
            updateSql.AppendLine(",M_COUNTRY= @Country");
            updateSql.AppendLine(",M_HOMEPAGE=@Homepage");
            updateSql.AppendLine(",M_SIG=@Signature");
            updateSql.AppendLine(",M_HIDE_EMAIL=@Hidemail");
            updateSql.AppendLine(",M_RECEIVE_EMAIL=@Receivemails");
            updateSql.AppendLine(",M_VIEW_SIG=@Viewsignatures");
            updateSql.AppendLine(",M_SIG_DEFAULT=@Usesignature");
            updateSql.AppendLine(",M_HOBBIES=@Hobbies");
            updateSql.AppendLine(",M_LNEWS=@Latestnews");
            updateSql.AppendLine(",M_QUOTE=@Favquote");
            updateSql.AppendLine(",M_BIO=@Bio");
            updateSql.AppendLine(",M_LINK1=@Link1");
            updateSql.AppendLine(",M_LINK2=@Link2");
            updateSql.AppendLine(",M_AIM=@Aim,M_YAHOO=@Yahoo,M_ICQ=@Icq,M_MSN=@Msn");
            updateSql.AppendLine(",M_LAST_IP=@LastIP");
            updateSql.AppendLine(",M_LASTUPDATED=@Lastupdated,M_LASTHEREDATE=@LastVisit");
            updateSql.AppendLine(",M_AVATAR=@Avatar");
            updateSql.AppendLine(",M_THEME=@Theme");
            updateSql.AppendLine(",M_TIMEOFFSET=@Timeoffset");
            updateSql.AppendLine("WHERE MEMBER_ID=@MemberId");

            List<SqlParameter> memberparms = new List<SqlParameter>
                                             {
                new SqlParameter("@Title", SqlDbType.NVarChar) {Value = member.Title.ConvertDBNull(),IsNullable = true},
                new SqlParameter("@Password", SqlDbType.NVarChar) {Value = member.Password},
                new SqlParameter("@IsValid", SqlDbType.Int) {Value = member.IsValid},
                new SqlParameter("@ValidationKey", SqlDbType.NVarChar) {Value = member.ValidationKey},
                new SqlParameter("@Email", SqlDbType.NVarChar) {Value = member.Email},
                new SqlParameter("@Firstname", SqlDbType.NVarChar)
                {
                    Value = member.Firstname.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Lastname", SqlDbType.NVarChar)
                {
                    Value = member.Lastname.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Occupation", SqlDbType.NVarChar)
                {
                    Value = member.Occupation.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Gender", SqlDbType.NVarChar)
                {
                    Value = member.Gender.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Age", SqlDbType.VarChar) {Value = member.Age},
                new SqlParameter("@Dob", SqlDbType.NVarChar)
                {
                    Value = member.DateOfBirth.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Maritalstatus", SqlDbType.NVarChar)
                {
                    Value = member.MaritalStatus.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@City", SqlDbType.NVarChar) {Value = member.City.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@State", SqlDbType.NVarChar) {Value = member.State.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Country", SqlDbType.NVarChar)
                {
                    Value = member.Country.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Homepage", SqlDbType.NVarChar)
                {
                    Value = member.HomePage.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Signature", SqlDbType.NVarChar)
                {
                    Value = member.Signature.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Hidemail", SqlDbType.Int) {Value = member.HideEmail},
                new SqlParameter("@Receivemails", SqlDbType.Int) {Value = member.ReceiveEmails},
                new SqlParameter("@Viewsignatures", SqlDbType.Int) {Value = member.ViewSignatures},
                new SqlParameter("@Usesignature", SqlDbType.Int) {Value = member.UseSignature},
                new SqlParameter("@Hobbies", SqlDbType.NVarChar)
                {
                    Value = member.Hobbies.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Latestnews", SqlDbType.NVarChar)
                {
                    Value = member.LatestNews.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Favquote", SqlDbType.NVarChar)
                {
                    Value = member.FavouriteQuote.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Bio", SqlDbType.NVarChar)
                {
                    Value = member.Biography.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Link1", SqlDbType.NVarChar)
                {
                    Value = member.FavLink1.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Link2", SqlDbType.NVarChar)
                {
                    Value = member.FavLink2.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Aim", SqlDbType.NVarChar) {Value = member.AIM.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Yahoo", SqlDbType.NVarChar) {Value = member.Yahoo.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Icq", SqlDbType.NVarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Msn", SqlDbType.NVarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@LastIP", SqlDbType.NVarChar) {Value = Common.GetIP4Address()},
                new SqlParameter("@Lastupdated", SqlDbType.NVarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                },
                new SqlParameter("@Lastvisit", SqlDbType.NVarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                },
                new SqlParameter("@Avatar", SqlDbType.NVarChar)
                {
                    Value = member.Avatar.ConvertDBNull(),
                    IsNullable = true
                },
                new SqlParameter("@Theme", SqlDbType.NVarChar) {Value = member.Theme.ConvertDBNull(), IsNullable = true},
                new SqlParameter("@Timeoffset", SqlDbType.Int) {Value = member.TimeOffset},
                new SqlParameter("@MemberId", SqlDbType.Int) {Value = member.Id}
            };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateSql.ToString(), memberparms.ToArray());
        }

        public void Delete(MemberInfo member)
        {
            if (member.PostCount > 0)
            {
                member.Status = 0;
                //set the password to a random string
                member.Password = new Guid().ToString();
                member.HideEmail = true;
                member.Firstname = null;
                member.Lastname = null;
                member.DateOfBirth = null;
                member.Biography = null;
                member.Yahoo = null;
                member.AIM = null;
                member.Signature = null;
                member.ICQ = null;
                member.Skype = null;
                member.LatestNews = null;
                member.HomePage = null;
                member.FavLink1 = null;
                member.FavLink2 = null;
                member.ReceiveEmails = false;
                Update(member);
            }
            else
            {
                string strSql = "DELETE FROM " + Config.MemberTablePrefix + "MEMBERS WHERE MEMBER_ID=@Member";
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@MemberId", SqlDbType.Int) { Value = member.Id });
            }
        }

        public void Dispose()
        {

        }

        public int GetMemberCount(object filter)
        {
            string whereclause = "";
            string strSql = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS";
            List<SqlParameter> parms = new List<SqlParameter>();

            if (filter != null)
            {
                if (filter.ToString().StartsWith("Initial"))
                {
                    string initial = ((string)filter).Substring(((string)filter).IndexOf("=") + 1);
                    whereclause = " WHERE M_NAME LIKE @Initial ";
                    parms.Add(new SqlParameter("@Initial", SqlDbType.VarChar) { Value = initial + "%" });
                }
                else
                {
                    whereclause = " WHERE ";
                    Regex regex = new Regex(@"(?<field>[^,]+),(?<searchfor>[^,]+)(?: OR |$)");
                    MatchCollection matches = regex.Matches(filter.ToString());
                    foreach (Match match in matches)
                    {
                        switch (match.Groups["field"].Value)
                        {
                            case "Name":
                                whereclause += "M_NAME LIKE @Name";
                                parms.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "FirstName":
                                whereclause += "M_FirstName LIKE @FirstName";
                                parms.Add(new SqlParameter("@FirstName", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "LastName":
                                whereclause += "M_LASTNAME LIKE @LastName";
                                parms.Add(new SqlParameter("@LastName", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Country":
                                whereclause += "M_COUNTRY LIKE @Country";
                                parms.Add(new SqlParameter("@Country", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Email":
                                whereclause += "M_EMAIL LIKE @Email";
                                parms.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                        }

                        whereclause += " OR ";
                    }
                    whereclause = whereclause.Remove(whereclause.LastIndexOf(" OR "));
                }
            }
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql + whereclause, parms.ToArray());
        }

        public IEnumerable<int> GetAllowedForumIds(MemberInfo member, List<int> roleList, bool isadmin)
        {
            List<int> forums = new List<int>();
            //return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
            string SqlStr = "SELECT FORUM_ID FROM " + Config.ForumTablePrefix + "FORUM";

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    forums.Add(rdr.GetInt32(0));
                }
            }
            List<int> allowedForums = new List<int>();
            foreach (int forum in forums)
            {
                ForumInfo f = new ForumInfo {Id = forum, AllowedRoles = new List<int>(new Forum().AllowedRoles(forum))};
                if (f.AllowedRoles.Count == 0)
                    allowedForums.Add(forum);
                else
                {
                    if (f.AllowedRoles.Any(role => roleList.Contains(role) || isadmin))
                    {
                        allowedForums.Add(f.Id);
                    }                    
                }
            }

            return allowedForums;
        }

        public IEnumerable<KeyValuePair<int, string>> GetAllowedForumList(MemberInfo member, List<int> roleList, bool isadmin)
        {
            List<KeyValuePair<int, string>> forums = new List<KeyValuePair<int, string>>();
            //return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
            string SqlStr = "SELECT FORUM_ID,F_SUBJECT FROM " + Config.ForumTablePrefix + "FORUM ORDER BY F_SUBJECT";

            //Execute a query to read the products
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    forums.Add(new KeyValuePair<int, string>(rdr.GetInt32(0),rdr.GetString(1)));
                }
            }

            List<KeyValuePair<int, string>> allowedForums = new List<KeyValuePair<int, string>>();
            foreach (var forum in forums)
            {
                ForumInfo f = new ForumInfo
                {
                    Id = forum.Key,
                    AllowedRoles = new List<int>(new Forum().AllowedRoles(forum.Key))
                };
                if (f.AllowedRoles.Count == 0)
                    allowedForums.Add(forum);
                else
                {
                    if (f.AllowedRoles.Any(role => roleList.Contains(role) || isadmin))
                    {
                        allowedForums.Add(forum);
                    }
                }
            }

            return allowedForums;
        }

        public IEnumerable<TopicInfo> GetRecentTopics(int memberid, MemberInfo member)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT TOP (20)  ");
            sql.AppendLine("T.TOPIC_ID,T.CAT_ID,T.FORUM_ID,T.T_STATUS,T.T_SUBJECT,T.T_AUTHOR,T.T_REPLIES");
            sql.AppendLine(",T.T_VIEW_COUNT,T.T_LAST_POST,T.T_DATE,T.T_IP,T.T_LAST_POST_AUTHOR,T.T_STICKY,T.T_LAST_EDIT,T.T_LAST_EDITBY");
            sql.AppendLine(",T.T_SIG,T.T_LAST_POST_REPLY_ID,T.T_UREPLIES,T.T_MESSAGE,A.M_NAME AS Author, LPA.M_NAME AS LastPostAuthor, EM.M_NAME AS Editor ");
            sql.AppendLine("FROM");
            sql.AppendFormat("{0}TOPICS AS T LEFT OUTER JOIN",Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}REPLY AS R ON T.TOPIC_ID = R.TOPIC_ID LEFT OUTER JOIN", Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID LEFT OUTER JOIN", Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID LEFT OUTER JOIN", Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID", Config.MemberTablePrefix).AppendLine();
            sql.AppendLine("WHERE");
            sql.AppendLine("T.T_LAST_POST > @SinceDate AND");
            sql.AppendLine("(T.T_AUTHOR = @UserId OR R.R_AUTHOR = @UserId) AND");
            sql.AppendLine("(R.R_STATUS < 2 OR T.T_STATUS < 2)");
            sql.AppendLine("ORDER BY T.T_LAST_POST DESC");

            TimeSpan ts = new TimeSpan(30, 0, 0, 0);
            DateTime startDate = DateTime.UtcNow - ts;
            SqlParameter[] parms = new SqlParameter[2];
            SqlParameter date = new SqlParameter("@SinceDate", SqlDbType.VarChar) { Value = startDate.ToString("yyyyMMddHHmmss") };
            SqlParameter user = new SqlParameter("@UserId", SqlDbType.VarChar) { Value = memberid };
            parms[0] = date;
            parms[1] = user;

            List<TopicInfo> topics = new List<TopicInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parms))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyTopicToBO(rdr));
                }
            }

            int[] allowed = member.AllowedForums;

            return topics.Where(t => allowed.Contains(t.ForumId)).Take(10).ToList(); 
        }

        public IEnumerable<MemberInfo> GetMembers(int startRecord, int maxRecords, string sortExpression, object filter)
        {
            //(ORDER BY M_POSTS DESC,M_DATE)
            string whereclause = "";
            List<SqlParameter> parms = new List<SqlParameter>();
            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "(ORDER BY M_POSTS DESC,M_DATE)";
            else
            {
                sortExpression = "(ORDER BY " + sortExpression + ")";
            }
            SELECT_OVER = SELECT_OVER.Replace("[ORDERBY]", sortExpression);
            if (filter != null)
            {
                if (filter.ToString().StartsWith("Initial"))
                {
                    string initial = ((string) filter).Substring(((string) filter).IndexOf("=")+1);
                    whereclause = " WHERE M_NAME LIKE @Initial ";
                    parms.Add(new SqlParameter("@Initial", SqlDbType.VarChar) { Value = initial + "%" });
                }
                else
                {
                    whereclause = "WHERE ";
                    Regex regex = new Regex(@"(?<field>[^,]+),(?<searchfor>[^,]+)(?: OR |$)");
                    MatchCollection matches = regex.Matches(filter.ToString());
                    foreach (Match match in matches)
                    {
                        switch (match.Groups["field"].Value)
                        {
                            case "Name":
                                whereclause += "M_NAME LIKE @Name";
                                parms.Add(new SqlParameter("@Name", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "FirstName":
                                whereclause += "M_FirstName LIKE @FirstName";
                                parms.Add(new SqlParameter("@FirstName", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "LastName":
                                whereclause += "M_LASTNAME LIKE @LastName";
                                parms.Add(new SqlParameter("@LastName", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Country":
                                whereclause += "M_COUNTRY LIKE @Country";
                                parms.Add(new SqlParameter("@Country", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Email":
                                whereclause += "M_EMAIL LIKE @Email";
                                parms.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                        }

                        whereclause += " OR ";
                    }
                    whereclause = whereclause.Remove(whereclause.LastIndexOf(" OR "));
                }

            }

            SELECT_OVER = SELECT_OVER.Replace("[WHERE]", whereclause);
            List<MemberInfo> members = new List<MemberInfo>();
            SqlParameter start = new SqlParameter("@Start", SqlDbType.Int) { Value = startRecord + 1 };
            SqlParameter recs = new SqlParameter("@MaxRows", SqlDbType.Int) { Value = startRecord + maxRecords };
            parms.Add(start);
            parms.Add(recs);

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT_OVER + MEMBER_SELECT + SELECT_OVER_FROM, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    members.Add(BoHelper.CopyMemberToBO(rdr));
                }
            }
            return members;
        }

        public MemberInfo GetByEmail(string email)
        {
            MemberInfo member = null;
            SqlParameter parm = new SqlParameter("@Email", SqlDbType.VarChar) { Value = email };

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + " WHERE M.M_EMAIL = @Email ", parm))
            {
                while (rdr.Read())
                {
                    member = BoHelper.CopyMemberToBO(rdr);
                }
            }
            return member;
        }

        public string[] ForumAdministrators()
        {
            List<string> members = new List<string>();
            string strSql = "SELECT M_NAME FROM " + Config.MemberTablePrefix + "MEMBERS WHERE M_LEVEL=3 AND M_STATUS=1";

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    members.Add(rdr.GetString(0));
                }
            }
            return members.ToArray();
        }

        public void UpdateLastMemberPost(object post)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            string updateMemberSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_POSTS=COALESCE(M_POSTS,0)+1, M_LASTPOSTDATE = @PostDate WHERE MEMBER_ID=@MemberId ";
            
            SqlParameter memberid = new SqlParameter("@MemberId", SqlDbType.Int);
            SqlParameter postdate = new SqlParameter("@PostDate", SqlDbType.VarChar);
            if (post is ReplyInfo)
            {
                ReplyInfo reply = (ReplyInfo)post;
                postdate.Value = reply.Date.ToString("yyyyMMddHHmmss");
                memberid.Value = reply.AuthorId;
            }else if (post is TopicInfo)
            {
                TopicInfo topic = (TopicInfo) post;
                postdate.Value = topic.Date.ToString("yyyyMMddHHmmss");
                memberid.Value = topic.AuthorId;
            }

            parms.Add(memberid);
            parms.Add(postdate);
          
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateMemberSql, parms.ToArray());
        }

        public IEnumerable<MemberInfo> GetPendingMembers(int startrecord, int maxrecords)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            List<MemberInfo> members = new List<MemberInfo>();

            SELECT_OVER = SELECT_OVER.Replace("[WHERE]", "WHERE M.M_VALID=0");
            SqlParameter start = new SqlParameter("@Start", SqlDbType.Int) { Value = startrecord };
            SqlParameter recs = new SqlParameter("@MaxRows", SqlDbType.Int) { Value = maxrecords };
            parms.Add(start);
            parms.Add(recs);

            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SELECT_OVER + MEMBER_SELECT + SELECT_OVER_FROM , parms.ToArray()))
            {
                while (rdr.Read())
                {
                    members.Add(BoHelper.CopyMemberToBO(rdr));
                }
            }
            _pendingMemberCount = members.Count;
            return members;
        }

        public void UpdateVisit(MemberInfo member)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            string updateMemberSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_LASTHEREDATE=@LastVisit WHERE MEMBER_ID=@MemberId ";
            SqlParameter memberid = new SqlParameter("@MemberId", SqlDbType.Int){Value = member.Id};
            SqlParameter lastvisit = new SqlParameter("@LastVisit", SqlDbType.VarChar){Value = DateTime.UtcNow.ToForumDateStr()};
            parms.Add(lastvisit);
            parms.Add(memberid);
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateMemberSql, parms.ToArray());

        }

        public int GetPendingMemberCount()
        {
            return _pendingMemberCount;
        }
    }
}
