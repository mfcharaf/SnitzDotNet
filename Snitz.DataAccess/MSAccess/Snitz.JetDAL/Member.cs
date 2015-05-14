using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzCommon;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class Member : IMember
    {
        private int _pendingMemberCount;

        private string OVER = "FROM (" +
                              "SELECT TOP [PageSize] sub.MEMBER_ID, sub.M_POSTS " +
                              "FROM  (" +
                              " SELECT TOP [Start] MEMBER_ID, M_POSTS FROM " + Config.MemberTablePrefix + "MEMBERS [WHERECLAUSE] ORDER BY M_POSTS DESC, MEMBER_ID DESC " +
                              ") sub " +
                              " ORDER BY M_POSTS ASC, MEMBER_ID ASC " +
                              ") subordered " +
                              " INNER JOIN " + Config.MemberTablePrefix + "MEMBERS M ON subordered.MEMBER_ID = M.MEMBER_ID ";

        private const string MEMBER_SELECT =
            "SELECT M.MEMBER_ID,M.M_STATUS,M.M_NAME,M.M_USERNAME,M.M_EMAIL,M.M_COUNTRY,M.M_HOMEPAGE,M.M_SIG" +
            ",M.M_LEVEL,M.M_AIM,M.M_YAHOO,M.M_ICQ,M.M_MSN,M.M_POSTS,M.M_DATE,M.M_LASTHEREDATE,M.M_LASTPOSTDATE" +
            ",M.M_TITLE,M.M_SUBSCRIPTION,M.M_HIDE_EMAIL,M.M_RECEIVE_EMAIL,M.M_IP,M.M_VIEW_SIG,M.M_SIG_DEFAULT" +
            ",M.M_VOTED,M.M_ALLOWEMAIL,M.M_AVATAR,M.M_THEME,M.M_TIMEOFFSET,M.M_DOB,M_AGE,M_PASSWORD,M_KEY,M_VALID,M_LASTUPDATED " +
            ",M_MARSTATUS,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION,M_SEX,M_HOBBIES,M_LNEWS,M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_CITY,M_STATE,M_DAYLIGHTSAVING,M_TIMEZONE ";

        private string FROM = " FROM " + Config.MemberTablePrefix + "MEMBERS M ";
        private const string WHERE_NAME = " WHERE M.M_NAME = @Username ";
        private const string WHERE_ID = " WHERE M.MEMBER_ID = @Id ";

        public MemberInfo GetById(int id)
        {
            MemberInfo member = null;
            OleDbParameter parm = new OleDbParameter("@Id", OleDbType.Integer) { Value = id };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + WHERE_ID, parm))
            {
                while (rdr.Read())
                {
                    member = BoHelper.CopyMemberToBO(rdr);
                }
            }
            return member ?? (new MemberInfo { Username = "Guest", TimeOffset = 0.0 });
        }

        public IEnumerable<MemberInfo> GetByName(string name)
        {

            List<MemberInfo> members = new List<MemberInfo>();
            OleDbParameter parm = new OleDbParameter("@Username", OleDbType.VarChar) { Value = name };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + WHERE_NAME, parm))
            {
                if(rdr != null)
                while (rdr.Read())
                {
                    members.Add(BoHelper.CopyMemberToBO(rdr));
                }
            }
            if (members.Count < 1)
                members.Add(new MemberInfo { Username = "Guest", TimeOffset = 0.0 });
            return members;
        }

        public int Add(MemberInfo member)
        {
            List<OleDbParameter> memberparms;

            string strSql = "INSERT INTO " + Config.MemberTablePrefix + "MEMBERS " +
                                  "(M_NAME,M_STATUS,M_LEVEL,M_TITLE,M_EMAIL,M_PASSWORD,M_VALID,M_KEY,M_FIRSTNAME,M_LASTNAME,M_OCCUPATION" +
                                  ",M_SEX,M_AGE,M_DOB,M_MARSTATUS,M_CITY,M_STATE,M_COUNTRY,M_HOMEPAGE,M_SIG" +
                                  ",M_HIDE_EMAIL,M_RECEIVE_EMAIL,M_VIEW_SIG,M_SIG_DEFAULT,M_HOBBIES,M_LNEWS" +
                                  ",M_QUOTE,M_BIO,M_LINK1,M_LINK2,M_AIM,M_YAHOO,M_ICQ,M_MSN,M_LAST_IP,M_LASTUPDATED" +
                                  ",M_LASTHEREDATE,M_AVATAR,M_THEME,M_TIMEOFFSET,M_DATE,M_POSTS,M_IP,M_DEFAULT_VIEW,M_USERNAME,M_PHOTO_URL,M_NEWEMAIL,M_PWKEY,M_SHA256,M_ALLOWEMAIL,M_DAYLIGHTSAVING,M_TIMEZONE)" +
                                  " VALUES " +
                                  "(@Username,@Status,@Mlev,@Title,@Email,@Password,@IsValid,@ValidationKey,@Firstname,@Lastname" +
                                  ",@Occupation,@Gender,@Age,@Dob,@Maritalstatus,@City,@State,@Country" +
                                  ",@Homepage,@Signature,@Hidemail,@Receivemails,@Viewsignatures,@Usesignature" +
                                  ",@Hobbies,@Latestnews,@Favquote,@Bio,@Link1,@Link2,@Aim,@Yahoo,@Icq,@Skype" +
                                  ",@LastIP,@Lastupdated,@LastVisit,@Avatar,@Theme,@Timeoffset,@Created,0,@LastIP,0,'','','','',1,0,@DaylightSaving,@TimeZone)";
                                  
            try
            {
                memberparms = new List<OleDbParameter>
                {
                    new OleDbParameter("@Username", OleDbType.VarChar) {Value = member.Username},
                    new OleDbParameter("@Status", OleDbType.SmallInt) {Value = member.Status},
                    new OleDbParameter("@Created", OleDbType.VarChar) {Value = member.MemberSince.ToString("yyyyMMddHHmmss")},
                    new OleDbParameter("@Mlev", OleDbType.SmallInt) {Value = member.MemberLevel},
                    new OleDbParameter("@Title", OleDbType.VarChar) {Value = member.Title.ConvertDBNull("")},
                    new OleDbParameter("@Password", OleDbType.VarChar) {Value = member.Password},
                    new OleDbParameter("@IsValid", OleDbType.Integer) {Value = member.IsValid ? 1 : 0},
                    new OleDbParameter("@ValidationKey", OleDbType.VarChar) {Value = member.ValidationKey.ConvertDBNull("")},
                    new OleDbParameter("@Email", OleDbType.VarChar) {Value = member.Email},
                    new OleDbParameter("@Firstname", OleDbType.VarChar){Value = member.Firstname.ConvertDBNull("")},
                    new OleDbParameter("@Lastname", OleDbType.VarChar){Value = member.Lastname.ConvertDBNull("")},
                    new OleDbParameter("@Occupation", OleDbType.VarChar){Value = member.Occupation.ConvertDBNull("")},
                    new OleDbParameter("@Gender", OleDbType.VarChar){Value = member.Gender.ConvertDBNull("")}

                };
                memberparms.Add(new OleDbParameter("@Age", OleDbType.Integer) { Value = member.Age.ConvertDBNull(0)});
                memberparms.Add(new OleDbParameter("@Dob", OleDbType.VarChar){Value = member.DateOfBirth.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Maritalstatus", OleDbType.VarChar){Value = member.MaritalStatus.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@City", OleDbType.VarChar) { Value = member.City.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@State", OleDbType.VarChar) { Value = member.State.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Country", OleDbType.VarChar){Value = member.Country.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Homepage", OleDbType.VarChar){Value = member.HomePage.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Signature", OleDbType.VarChar){Value = member.Signature.ConvertDBNull("")});

                memberparms.Add(new OleDbParameter("@Hidemail", OleDbType.Integer) { Value = member.HideEmail ? 1 : 0 });
                memberparms.Add(new OleDbParameter("@Receivemails", OleDbType.Integer) { Value = member.ReceiveEmails ? 1 : 0 });
                memberparms.Add(new OleDbParameter("@Viewsignatures", OleDbType.Integer) { Value = member.ViewSignatures ? 1 : 0 });
                memberparms.Add(new OleDbParameter("@Usesignature", OleDbType.Integer) { Value = member.UseSignature ? 1 : 0 });

                memberparms.Add(new OleDbParameter("@Hobbies", OleDbType.VarChar){Value = member.Hobbies.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Latestnews", OleDbType.VarChar){Value = member.LatestNews.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Favquote", OleDbType.VarChar){Value = member.FavouriteQuote.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Bio", OleDbType.VarChar){Value = member.Biography.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Link1", OleDbType.VarChar){Value = member.FavLink1.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Link2", OleDbType.VarChar){Value = member.FavLink2.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Aim", OleDbType.VarChar) { Value = member.AIM.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Yahoo", OleDbType.VarChar) { Value = member.Yahoo.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Icq", OleDbType.VarChar) { Value = member.ICQ.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Skype", OleDbType.VarChar) { Value = member.Skype.ConvertDBNull("") });
                memberparms.Add(new OleDbParameter("@LastIP", OleDbType.VarChar) { Value = Common.GetIP4Address() });
                
                memberparms.Add(new OleDbParameter("@Lastupdated", OleDbType.VarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                });
                memberparms.Add(new OleDbParameter("@LastVisit", OleDbType.VarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                });
                memberparms.Add(new OleDbParameter("@Avatar", OleDbType.VarChar){Value = member.Avatar.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Theme", OleDbType.VarChar) { Value = member.Theme.ConvertDBNull("")});
                memberparms.Add(new OleDbParameter("@Timeoffset", OleDbType.Double) { Value = member.TimeOffset });
                memberparms.Add(new OleDbParameter("@DaylightSaving", OleDbType.Integer) { Value = member.UseDaylightSaving ? 1 : 0 });
                memberparms.Add(new OleDbParameter("@TimeZone", OleDbType.VarChar) { Value = member.TimeZone.ConvertDBNull("") });
                string query = strSql;
                foreach (OleDbParameter p in memberparms)
                {
                    if (p.OleDbType != OleDbType.VarChar)
                    {
                        query = query.Replace(p.ParameterName, p.Value.ToString() );
                    }else
                        query = query.Replace(p.ParameterName, "'" + p.Value + "'");
                }

                int res = SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, query, null);
                return res;

            }
            catch (Exception)
            {
                return 0;
                throw;
            }

        }

        public void Update(MemberInfo member)
        {
            if (member.Username.ToLower() == "guest")
                return;
            StringBuilder updateSql = new StringBuilder();
            updateSql.AppendFormat("UPDATE {0}MEMBERS SET", Config.MemberTablePrefix).AppendLine();
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
            updateSql.AppendLine(",M_AIM=@Aim,M_YAHOO=@Yahoo,M_ICQ=@Icq,M_MSN=@Skype");
            updateSql.AppendLine(",M_LAST_IP=M_IP");
            updateSql.AppendLine(",M_IP=@LastIP");
            updateSql.AppendLine(",M_LASTUPDATED=@Lastupdated,M_LASTHEREDATE=@LastVisit");
            updateSql.AppendLine(",M_AVATAR=@Avatar");
            updateSql.AppendLine(",M_THEME=@Theme");
            updateSql.AppendLine(",M_TIMEOFFSET=@Timeoffset");
            updateSql.AppendLine(",M_DAYLIGHTSAVING=@DaylightSaving");
            updateSql.AppendLine(",M_TIMEZONE=@TimeZone");
            updateSql.AppendLine(",M_STATUS=@Status");
            updateSql.AppendLine("WHERE MEMBER_ID=@MemberId");

            List<OleDbParameter> memberparms = new List<OleDbParameter>
            {
                new OleDbParameter("@Title", OleDbType.VarChar) {Value = member.Title.ConvertDBNull(),IsNullable = true},
                new OleDbParameter("@Email", OleDbType.VarChar) {Value = member.Email},
                new OleDbParameter("@Password", OleDbType.VarChar) {Value = member.Password},
                new OleDbParameter("@IsValid", OleDbType.Integer) {Value = member.IsValid?1:0},
                new OleDbParameter("@ValidationKey", OleDbType.VarChar) {Value = member.ValidationKey},
                
                new OleDbParameter("@Firstname", OleDbType.VarChar)
                {
                    Value = member.Firstname.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Lastname", OleDbType.VarChar)
                {
                    Value = member.Lastname.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Occupation", OleDbType.VarChar)
                {
                    Value = member.Occupation.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Gender", OleDbType.VarChar)
                {
                    Value = member.Gender.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Age", OleDbType.VarChar) {Value = member.Age},
                new OleDbParameter("@Dob", OleDbType.VarChar)
                {
                    Value = member.DateOfBirth.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Maritalstatus", OleDbType.VarChar)
                {
                    Value = member.MaritalStatus.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@City", OleDbType.VarChar) {Value = member.City.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@State", OleDbType.VarChar) {Value = member.State.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Country", OleDbType.VarChar)
                {
                    Value = member.Country.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Homepage", OleDbType.VarChar)
                {
                    Value = member.HomePage.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Signature", OleDbType.VarChar)
                {
                    Value = member.Signature.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Hidemail", OleDbType.Integer) {Value = member.HideEmail?1:0},
                new OleDbParameter("@Receivemails", OleDbType.Integer) {Value = member.ReceiveEmails?1:0},
                new OleDbParameter("@Viewsignatures", OleDbType.Integer) {Value = member.ViewSignatures?1:0},
                new OleDbParameter("@Usesignature", OleDbType.Integer) {Value = member.UseSignature?1:0},
                new OleDbParameter("@Hobbies", OleDbType.VarChar)
                {
                    Value = member.Hobbies.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Latestnews", OleDbType.VarChar)
                {
                    Value = member.LatestNews.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Favquote", OleDbType.VarChar)
                {
                    Value = member.FavouriteQuote.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Bio", OleDbType.VarChar)
                {
                    Value = member.Biography.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Link1", OleDbType.VarChar)
                {
                    Value = member.FavLink1.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Link2", OleDbType.VarChar)
                {
                    Value = member.FavLink2.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Aim", OleDbType.VarChar) {Value = member.AIM.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Yahoo", OleDbType.VarChar) {Value = member.Yahoo.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Icq", OleDbType.VarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Skype", OleDbType.VarChar) {Value = member.ICQ.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@LastIP", OleDbType.VarChar) {Value = Common.GetIP4Address()},
                new OleDbParameter("@Lastupdated", OleDbType.VarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                },
                new OleDbParameter("@LastVisit", OleDbType.VarChar)
                {
                    Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                },
                new OleDbParameter("@Avatar", OleDbType.VarChar)
                {
                    Value = member.Avatar.ConvertDBNull(),
                    IsNullable = true
                },
                new OleDbParameter("@Theme", OleDbType.VarChar) {Value = member.Theme.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@Timeoffset", OleDbType.Integer) {Value = member.TimeOffset},
                new OleDbParameter("@Status", OleDbType.Integer) {Value = member.Status},
                new OleDbParameter("@MemberId", OleDbType.Integer) {Value = member.Id},
                new OleDbParameter("@TimeZone", OleDbType.VarChar) {Value = member.TimeZone.ConvertDBNull(), IsNullable = true},
                new OleDbParameter("@DaylightSaving", OleDbType.Integer) { Value = member.UseDaylightSaving ? 1 : 0 }

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
                string strSql = "DELETE FROM " + Config.MemberTablePrefix + "MEMBERS WHERE MEMBER_ID=@MemberId";
                SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@MemberId", OleDbType.Integer) { Value = member.Id });
            }
        }

        public void Dispose()
        {

        }

        public int GetMemberCount(object filter)
        {
            string whereclause = "";
            string strSql = "SELECT COUNT(MEMBER_ID) FROM " + Config.MemberTablePrefix + "MEMBERS";
            List<OleDbParameter> parms = new List<OleDbParameter>();

            if (filter != null)
            {
                if (filter.ToString().StartsWith("Initial"))
                {
                    string initial = ((string)filter).Substring(((string)filter).IndexOf("=") + 1);
                    whereclause = " WHERE M_NAME LIKE @Initial ";
                    parms.Add(new OleDbParameter("@Initial", OleDbType.VarChar) { Value = initial + "%" });
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
                                parms.Add(new OleDbParameter("@Name", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "FirstName":
                                whereclause += "M_FirstName LIKE @FirstName";
                                parms.Add(new OleDbParameter("@FirstName", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "LastName":
                                whereclause += "M_LASTNAME LIKE @LastName";
                                parms.Add(new OleDbParameter("@LastName", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Country":
                                whereclause += "M_COUNTRY LIKE @Country";
                                parms.Add(new OleDbParameter("@Country", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Email":
                                whereclause += "M_EMAIL LIKE @Email";
                                parms.Add(new OleDbParameter("@Email", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                        }

                        whereclause += " OR ";
                    }
                    whereclause = whereclause.Remove(whereclause.LastIndexOf(" OR "));
                }
            }
            return (int)SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql + whereclause, parms.ToArray());
        }

        public IEnumerable<int> GetAllowedForumIds(List<int> roleList, bool isadmin)
        {
            List<int> forums = new List<int>();
            //return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
            string SqlStr = "SELECT FORUM_ID FROM " + Config.ForumTablePrefix + "FORUM";

            //Execute a query to read the products
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    forums.Add(rdr.GetInt32(0));
                }
            }
            List<int> allowedForums = new List<int>();
            foreach (int forum in forums)
            {
                ForumInfo f = new ForumInfo { Id = forum, AllowedRoles = new List<int>(new Forum().AllowedRoles(forum)) };
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
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, SqlStr, null))
            {
                while (rdr.Read())
                {
                    forums.Add(new KeyValuePair<int, string>(rdr.GetInt32(0), rdr.GetString(1)));
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
            sql.AppendFormat("((({0}TOPICS AS T LEFT OUTER JOIN", Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}REPLY AS R ON T.TOPIC_ID = R.TOPIC_ID) LEFT OUTER JOIN", Config.ForumTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID) LEFT OUTER JOIN", Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID) LEFT OUTER JOIN", Config.MemberTablePrefix).AppendLine();
            sql.AppendFormat("{0}MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID", Config.MemberTablePrefix).AppendLine();
            sql.AppendLine("WHERE");
            sql.AppendLine("T.T_LAST_POST > @SinceDate AND");
            sql.AppendLine("(T.T_AUTHOR = @UserId OR R.R_AUTHOR = @UserId) AND");
            sql.AppendLine("(R.R_STATUS < 2 OR T.T_STATUS < 2)");
            sql.AppendLine("ORDER BY T.T_LAST_POST DESC");

            TimeSpan ts = new TimeSpan(30, 0, 0, 0);
            DateTime startDate = DateTime.UtcNow - ts;
            OleDbParameter[] parms = new OleDbParameter[2];
            OleDbParameter date = new OleDbParameter("@SinceDate", OleDbType.VarChar) { Value = startDate.ToString("yyyyMMddHHmmss") };
            OleDbParameter user = new OleDbParameter("@UserId", OleDbType.VarChar) { Value = memberid };
            parms[0] = date;
            parms[1] = user;

            List<TopicInfo> topics = new List<TopicInfo>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), parms))
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
            int totalmembers = GetMemberCount(filter);
            string whereclause = "";
            List<OleDbParameter> parms = new List<OleDbParameter>();
            if (String.IsNullOrEmpty(sortExpression))
                sortExpression = "(ORDER BY M_POSTS DESC,M_DATE)";
            else
            {
                sortExpression = "(ORDER BY " + sortExpression + ")";
            }
            //SELECT_OVER = SELECT_OVER.Replace("[ORDERBY]", sortExpression);
            if (filter != null)
            {
                if (filter.ToString().StartsWith("Initial"))
                {
                    string initial = ((string)filter).Substring(((string)filter).IndexOf("=") + 1);
                    whereclause = " WHERE M_NAME LIKE @Initial ";
                    parms.Add(new OleDbParameter("@Initial", OleDbType.VarChar) { Value = initial + "%" });
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
                                parms.Add(new OleDbParameter("@Name", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "FirstName":
                                whereclause += "M_FirstName LIKE @FirstName";
                                parms.Add(new OleDbParameter("@FirstName", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "LastName":
                                whereclause += "M_LASTNAME LIKE @LastName";
                                parms.Add(new OleDbParameter("@LastName", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Country":
                                whereclause += "M_COUNTRY LIKE @Country";
                                parms.Add(new OleDbParameter("@Country", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                            case "Email":
                                whereclause += "M_EMAIL LIKE @Email";
                                parms.Add(new OleDbParameter("@Email", OleDbType.VarChar) { Value = "%" + match.Groups["searchfor"].Value + "%" });
                                break;
                        }

                        whereclause += " OR ";
                    }
                    whereclause = whereclause.Remove(whereclause.LastIndexOf(" OR "));
                }

            }

            OVER = OVER.Replace("[WHERECLAUSE]", whereclause);

            List<MemberInfo> members = new List<MemberInfo>();
            //(totalmembers - (start * maxRecords))
            OVER = OVER.Replace("[Start]", Math.Max((totalmembers - (startRecord * maxRecords)),maxRecords).ToString());
            OVER = OVER.Replace("[PageSize]", maxRecords.ToString());
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + OVER, parms.ToArray()))
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
            OleDbParameter parm = new OleDbParameter("@Email", OleDbType.VarChar) { Value = email };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + FROM + " WHERE M.M_EMAIL = @Email ", parm))
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

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    members.Add(rdr.GetString(0));
                }
            }
            return members.ToArray();
        }

        public int GetPendingMemberCount()
        {
            return _pendingMemberCount;
        }

        public IEnumerable<MemberInfo> GetPendingMembers(int startrecord, int maxrecords)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            List<MemberInfo> members = new List<MemberInfo>();

            OVER = OVER.Replace("[WHERECLAUSE]", "WHERE M_VALID=0");
            OleDbParameter start = new OleDbParameter("@Start", SqlDbType.Int) { Value = startrecord };
            OleDbParameter recs = new OleDbParameter("@MaxRows", SqlDbType.Int) { Value = maxrecords };
            parms.Add(start);
            parms.Add(recs);

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, MEMBER_SELECT + OVER, parms.ToArray()))
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
            //M_LASTHEREDATE=@LastVisit
            List<OleDbParameter> parms = new List<OleDbParameter>();
            string updateMemberSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_LASTHEREDATE=@LastVisit, M_LAST_IP=M_IP, M_IP=@LastIP WHERE MEMBER_ID=@MemberId ";
            OleDbParameter memberid = new OleDbParameter("@MemberId", OleDbType.Integer) { Value = member.Id };
            OleDbParameter lastvisit = new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = DateTime.UtcNow.ToForumDateStr() };
            OleDbParameter memberip = new OleDbParameter("@LastIP", OleDbType.VarChar) {Value = Common.GetIP4Address()};
            parms.Add(lastvisit);
            parms.Add(memberid);
            parms.Add(memberip);
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateMemberSql, parms.ToArray());
        }

        public void UpdateLastMemberPost(object post)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>();
            string updateMemberSql = "UPDATE " + Config.MemberTablePrefix + "MEMBERS SET M_POSTS=M_POSTS+1, M_LASTPOSTDATE = @PostDate WHERE MEMBER_ID=@MemberId ";

            OleDbParameter memberid = new OleDbParameter("@MemberId", OleDbType.Integer);
            OleDbParameter postdate = new OleDbParameter("@PostDate", OleDbType.VarChar);
            if (post is ReplyInfo)
            {
                ReplyInfo reply = (ReplyInfo)post;
                postdate.Value = reply.Date.ToString("yyyyMMddHHmmss");
                memberid.Value = reply.AuthorId;
            }
            else if (post is TopicInfo)
            {
                TopicInfo topic = (TopicInfo)post;
                postdate.Value = topic.Date.ToString("yyyyMMddHHmmss");
                memberid.Value = topic.AuthorId;
            }
            parms.Add(postdate);
            parms.Add(memberid);
            

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateMemberSql, parms.ToArray());
        }
    }
}
