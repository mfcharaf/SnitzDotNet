using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;
using SnitzCommon;

namespace Snitz.SQLServerDAL
{
    public class Member : IMember
    {
        private string SELECT_OVER = "WITH MemberEntities AS (SELECT ROW_NUMBER() OVER [ORDERBY] AS Row, MEMBER_ID FROM FORUM_MEMBERS [WHERE]) ";
        private const string SELECT_OVER_FROM = "FROM MemberEntities ME INNER JOIN FORUM_MEMBERS M on ME.MEMBER_ID = M.MEMBER_ID " +
            " WHERE ME.Row Between @Start AND @MaxRows ORDER BY ME.Row ASC ";

        private const string MEMBER_SELECT =
            "SELECT M.MEMBER_ID,M.M_STATUS,M.M_NAME,M.M_USERNAME,M.M_EMAIL,M.M_COUNTRY,M.M_HOMEPAGE,M.M_SIG" +
            ",M.M_LEVEL,M.M_AIM,M.M_YAHOO,M.M_ICQ,M.M_MSN,M.M_POSTS,M.M_DATE,M.M_LASTHEREDATE,M.M_LASTPOSTDATE" +
            ",M.M_TITLE,M.M_SUBSCRIPTION,M.M_HIDE_EMAIL,M.M_RECEIVE_EMAIL,M.M_IP,M.M_VIEW_SIG,M.M_SIG_DEFAULT" +
            ",M.M_VOTED,M.M_ALLOWEMAIL,M.M_AVATAR,M.M_THEME,M.M_TIMEOFFSET,M.M_DOB,M_AGE,M_PASSWORD,M_KEY,M_VALID,M_LASTUPDATED " +
            ",M_MARSTATUS,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION,M_SEX,M_HOBBIES,M_LNEWS,M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_CITY,M_STATE ";

        private const string FROM = " FROM FORUM_MEMBERS M ";
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

        public int Add(MemberInfo obj)
        {
            //Member object added through membership providor
            return -1;
        }

        public void Update(MemberInfo member)
        {
            if (member.Username.ToLower() == "guest")
                return;
            const string strSql = "UPDATE dbo.FORUM_MEMBERS SET " +
                                  "M_TITLE=@Title" +
                                  ",M_EMAIL=@Email" +
                                  ",M_PASSWORD=@Password" +
                                  ",M_VALID=@IsValid" +
                                  ",M_KEY=@ValidationKey" +

                                  ",M_FIRSTNAME=@Firstname" +
                                  ",M_LASTNAME=@Lastname" +
                                  ",M_OCCUPATION=@Occupation" +
                                  ",M_SEX=@Gender" +
                                  ",M_AGE=@Age" +
                                  ",M_DOB=@Dob" +
                                  ",M_MARSTATUS=@Maritalstatus" +
                                  ",M_CITY=@City" +
                                  ",M_STATE=@State" +
                                  ",M_COUNTRY= @Country" +
                                  ",M_HOMEPAGE=@Homepage" +
                                  ",M_SIG=@Signature" +
                                  ",M_HIDE_EMAIL=@Hidemail" +
                                  ",M_RECEIVE_EMAIL=@Receivemails" +
                                  ",M_VIEW_SIG=@Viewsignatures" +
                                  ",M_SIG_DEFAULT=@Usesignature" +
                                  ",M_HOBBIES=@Hobbies" +
                                  ",M_LNEWS=@Latestnews" +
                                  ",M_QUOTE=@Favquote" +
                                  ",M_BIO=@Bio" +
                                  ",M_LINK1=@Link1" +
                                  ",M_LINK2=@Link2" +
                                  ",M_AIM=@Aim,M_YAHOO=@Yahoo,M_ICQ=@Icq,M_MSN=@Msn" +
                                  ",M_LAST_IP=@LastIP" +
                                  ",M_LASTUPDATED=@Lastupdated" +
                                  ",M_LASTHEREDATE=@LastVisit" +
                                  ",M_AVATAR=@Avatar" +
                                  ",M_THEME=@Theme" +
                                  ",M_TIMEOFFSET=@Timeoffset" +
                                  " WHERE MEMBER_ID=@MemberId";
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
                new SqlParameter("@Age", SqlDbType.Int) {Value = member.Age},
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
                new SqlParameter("@LastIP", SqlDbType.NVarChar) {Value = Common.GetIPAddress()},
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

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, memberparms.ToArray());
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
                member.MSN = null;
                member.LatestNews = null;
                member.HomePage = null;
                member.FavLink1 = null;
                member.FavLink2 = null;
                member.ReceiveEmails = false;
                Update(member);
            }
            else
            {
                const string strSql = "DELETE FROM FORUM_MEMBERS WHERE MEMBER_ID=@Member";
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@MemberId", SqlDbType.Int) { Value = member.Id });
            }
        }

        public void Dispose()
        {

        }

        public int GetMemberCount(object filter)
        {
            string whereclause = "";
            const string strSql = "SELECT COUNT(MEMBER_ID) FROM FORUM_MEMBERS";
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
            const string SqlStr = "SELECT FORUM_ID FROM FORUM_FORUM WHERE F_STATUS=1";

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
            const string SqlStr = "SELECT FORUM_ID,F_SUBJECT FROM FORUM_FORUM ORDER BY F_SUBJECT";

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
            const string strSql =
            "SELECT DISTINCT TOP (20)  " +
            " T.TOPIC_ID,T.CAT_ID,T.FORUM_ID,T.T_STATUS,T.T_SUBJECT,T.T_AUTHOR,T.T_REPLIES" +
            ",T.T_VIEW_COUNT,T.T_LAST_POST,T.T_DATE,T.T_IP,T.T_LAST_POST_AUTHOR,T.T_STICKY,T.T_LAST_EDIT,T.T_LAST_EDITBY" +
            ",T.T_SIG,T.T_LAST_POST_REPLY_ID,T.T_UREPLIES,T.T_MESSAGE,A.M_NAME AS Author, LPA.M_NAME AS LastPostAuthor, EM.M_NAME AS Editor " +            
            
            "FROM " +
            "FORUM_TOPICS AS T LEFT OUTER JOIN " +
            "FORUM_REPLY AS R ON T.TOPIC_ID = R.TOPIC_ID LEFT OUTER JOIN " +
            "FORUM_MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID LEFT OUTER JOIN " +
            "FORUM_MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID LEFT OUTER JOIN " +
            "FORUM_MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID " +
            "WHERE        " +
            "T.T_LAST_POST > @SinceDate AND " +
            "(T.T_AUTHOR = @UserId OR R.R_AUTHOR = @UserId) AND " +
            "(R.R_STATUS < 2 OR T.T_STATUS < 2) " +
            "ORDER BY T.T_LAST_POST DESC";

            TimeSpan ts = new TimeSpan(30, 0, 0, 0);
            DateTime startDate = DateTime.UtcNow - ts;
            SqlParameter[] parms = new SqlParameter[2];
            SqlParameter date = new SqlParameter("@SinceDate", SqlDbType.VarChar) { Value = startDate.ToString("yyyyMMddHHmmss") };
            SqlParameter user = new SqlParameter("@UserId", SqlDbType.VarChar) { Value = memberid };
            parms[0] = date;
            parms[1] = user;

            List<TopicInfo> topics = new List<TopicInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms))
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
            SqlParameter start = new SqlParameter("@Start", SqlDbType.Int) { Value = startRecord };
            SqlParameter recs = new SqlParameter("@MaxRows", SqlDbType.Int) { Value = maxRecords };
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

        public void UpdateLastMemberPost(object post)
        {
            List<SqlParameter> parms = new List<SqlParameter>();
            const string updateMemberSql = "UPDATE FORUM_MEMBERS SET M_POSTS=M_POSTS+1, M_LASTPOSTDATE = @PostDate WHERE MEMBER_ID=@MemberId ";
            
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
    }
}
