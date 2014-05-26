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
    public class Topic : ITopic
    {
        public const string TopicCols =
            " T.TOPIC_ID,T.CAT_ID,T.FORUM_ID,T.T_STATUS,T.T_SUBJECT,T.T_AUTHOR,T.T_REPLIES,T.T_VIEW_COUNT,T.T_LAST_POST" +
            ",T.T_DATE,T.T_IP,T.T_LAST_POST_AUTHOR,T.T_STICKY,T.T_LAST_EDIT,T.T_LAST_EDITBY,T.T_SIG,T.T_LAST_POST_REPLY_ID" +
            ",T.T_UREPLIES,T.T_MESSAGE,A.M_NAME AS [Author], LPA.M_NAME AS [LastPostAuthor], EM.M_NAME AS [Editor],A.M_VIEW_SIG, A.M_SIG ";
        public static string TopicFrom =
            "FROM ((" + Config.ForumTablePrefix + "TOPICS T " +
            "LEFT JOIN " + Config.MemberTablePrefix + "MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID) " +
            "LEFT JOIN " + Config.MemberTablePrefix + "MEMBERS A ON T.T_AUTHOR = A.MEMBER_ID )" +
            "LEFT JOIN " + Config.MemberTablePrefix + "MEMBERS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID ";

        #region ITopic Members

        public IEnumerable<ReplyInfo> GetReplies(TopicInfo topic, int startrec, int maxrecs)
        {
            return new Reply().GetByParent(topic, startrec, maxrecs);
        }

        public TopicInfo GetNextPrevTopic(int topicid, string which)
        {
            string strSql = "SELECT TOP 1 " + TopicCols + TopicFrom;
            TopicInfo topic = GetById(topicid);


            if (which == "next")
            {
                strSql = strSql + " WHERE T.FORUM_ID=@ForumId AND T.T_LAST_POST > @TopicDate ORDER BY T.T_LAST_POST";
            }
            else
            {
                strSql = strSql + " WHERE T.FORUM_ID=@ForumId AND T.T_LAST_POST < @TopicDate ORDER BY T.T_LAST_POST DESC";

            }
            TopicInfo newtopic = null;
            List<OleDbParameter> parms = new List<OleDbParameter>
                                       {
                                           new OleDbParameter("@ForumId", OleDbType.Integer){Value=topic.Id}
                                       };
            parms.Add(topic.LastPostDate.HasValue
                ? new OleDbParameter("@TopicDate", OleDbType.Integer) { Value = topic.LastPostDate.Value.ToString("yyyyMMddHHmmss") }
                : new OleDbParameter("@TopicDate", OleDbType.Integer) { Value = topic.Date.ToString("yyyyMMddHHmmss") });
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    newtopic = BoHelper.CopyTopicToBO(rdr);
                }
            }
            return newtopic;
        }

        public void UpdateLastTopicPost(ReplyInfo reply)
        {
            string updateTopicSql = "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_REPLIES=T_REPLIES+1, T_LAST_POST_REPLY_ID=@ReplyId, T_LAST_POST=@ReplyDate, T_LAST_POST_AUTHOR=@ReplyAuthor WHERE TOPIC_ID=@TopicId ";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    
                    new OleDbParameter("@ReplyId", OleDbType.Integer) {Value = reply.Id},
                    new OleDbParameter("@ReplyDate", OleDbType.VarChar) {Value = reply.Date.ToString("yyyyMMddHHmmss")},
                    new OleDbParameter("@ReplyAuthor", OleDbType.Integer) {Value = reply.AuthorId},
                    new OleDbParameter("@TopicId", OleDbType.Integer) {Value = reply.TopicId}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateTopicSql, parms.ToArray());
        }

        public void SetTopicStatus(int topicid, int status)
        {
            string updateSql = "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_STATUS=@Status WHERE TOPIC_ID=@TopicId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@TopicId", OleDbType.Integer) {Value = topicid},
                    new OleDbParameter("@Status", OleDbType.Integer) {Value = status}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateSql, parms.ToArray());
        }

        public void MakeSticky(int topicid, bool sticky)
        {
            string updateSql = "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_STICKY=@Sticky WHERE TOPIC_ID=@TopicId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@TopicId", OleDbType.Integer) {Value = topicid},
                    new OleDbParameter("@Sticky", OleDbType.SmallInt) {Value = sticky}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateSql, parms.ToArray());
        }

        public void ChangeTopicForum(int topicid, int forumid)
        {
            ForumInfo newforum = new Forum().GetById(forumid);

            string updateSql = "UPDATE " + Config.ForumTablePrefix + "TOPICS SET FORUM_ID=@ForumId, CAT_ID=@CatId WHERE TOPIC_ID=@TopicId ";
            if (newforum.Status == (int)Enumerators.PostStatus.Closed) //forum locked so lock all posts
            {
                updateSql = updateSql + "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_STATUS=0 WHERE TOPIC_ID=@TopicId ";
                updateSql = updateSql + "UPDATE " + Config.ForumTablePrefix + "REPLY SET R_STATUS=0 WHERE TOPIC_ID=@TopicId ";
            }
            else if (newforum.ModerationLevel == (int)Enumerators.Moderation.UnModerated)
            {
                //change status of posts if coming from moderated forum
                updateSql = updateSql + "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_STATUS=1 WHERE TOPIC_ID=@TopicId AND T_STATUS > 1 ";
                updateSql = updateSql + "UPDATE " + Config.ForumTablePrefix + "REPLY SET R_STATUS=1 WHERE TOPIC_ID=@TopicId AND R_STATUS > 1 ";

            }
            updateSql = updateSql + "UPDATE " + Config.ForumTablePrefix + "REPLY SET FORUM_ID=@ForumId, CAT_ID=@CatId WHERE TOPIC_ID=@TopicId ";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@TopicId", OleDbType.Integer) {Value = topicid},
                    new OleDbParameter("@ForumId", OleDbType.Integer) {Value = forumid},
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = newforum.CatId}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateSql, parms.ToArray());
        }

        public void UpdateViewCount(int topicid)
        {
            OleDbParameter parm = new OleDbParameter("@TopicId", OleDbType.Integer) { Value = topicid };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, "UPDATE " + Config.ForumTablePrefix + "TOPICS SET T_VIEW_COUNT = T_VIEW_COUNT + 1 WHERE TOPIC_ID=@TopicId", parm);
        }

        public IEnumerable<TopicInfo> GetTopicsForSiteMap(int maxRecords)
        {
            List<TopicInfo> topics = new List<TopicInfo>();

            string strSql = "SELECT TOP " + maxRecords + " " + TopicCols + TopicFrom + " WHERE T.T_STATUS<2 ORDER BY T.T_LAST_POST DESC";
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyTopicToBO(rdr));
                }
            }

            return topics;
        }

        public IEnumerable<TopicInfo> GetLatestTopics(int topicCount, string lastvisit)
        {
            List<TopicInfo> topics = new List<TopicInfo>();
            List<OleDbParameter> parms = new List<OleDbParameter>();
            string strSql = "";
            string where = "";
            const string @orderby = " ORDER BY T.T_LAST_POST DESC";

            if (topicCount > 0)
            {
                //parms.Add(new OleDbParameter("@RecCount", OleDbType.Integer) { Value = topicCount });
                strSql = "SELECT TOP " + topicCount;
            }
            if (!String.IsNullOrEmpty(lastvisit))
            {
                parms.Add(new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = lastvisit });
                strSql = "SELECT ";
                where = "WHERE T_STATUS=1 AND T.T_LAST_POST > @LastVisit";
            }

            strSql = strSql + TopicCols + TopicFrom + where + orderby;
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyTopicToBO(rdr));
                }
            }

            return topics;
        }

        public int? GetPollId(int topicid)
        {
            string strSql = "SELECT PollId FROM " + Config.ForumTablePrefix + "POLLS WHERE TopicId=@TopicId";

            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@TopicId", OleDbType.Integer) { Value = topicid });
            int? pollid;
            if (res != null)
                pollid = Convert.ToInt32(res);
            else
            {
                pollid = null;
            }
            return pollid;
        }

        public IEnumerable<SearchResult> FindTopics(SearchParamInfo sparams, int currentPage, string orderby, ref int rowcount)
        {
            string whereClause = "";
            string orderfield = "";
            string whereOperator = "WHERE";
            string topictable = sparams.Archived ? "FORUM_A_TOPICS" : "FORUM_TOPICS";
            string replytable = sparams.Archived ? "FORUM_A_REPLY" : "FORUM_REPLY";

            List<OleDbParameter> param = new List<OleDbParameter>();

            switch (orderby)
            {
                case "Subject":
                    orderfield = "TT.T_SUBJECT";
                    break;
                case "Replies":
                    orderfield = "TT.T_REPLIES";
                    break;
                case "Views":
                    orderfield = "TT.T_VIEW_COUNT";
                    break;
                case "Date":
                    orderfield = "TT.T_DATE";
                    break;
                case "Author":
                    orderfield = "TM.M_NAME";
                    break;
                case "ForumOrder":
                    orderfield = "F.F_SUBJECT";
                    break;
                default:
                    orderfield = "TT.T_LAST_POST, TOPIC_ID";
                    break;
            }

            #region Build WHERE clause

            if (sparams.ForumId > 0)
            {
                param.Add(new OleDbParameter("@ForumId", OleDbType.Integer) { Value = sparams.ForumId });
                whereClause = whereClause + whereOperator + " TT.FORUM_ID=@ForumId ";
                whereOperator = "AND";
               
            }

            if (sparams.SinceDate != DateTime.MinValue)
            {
                param.Add(new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = sparams.SinceDate });
                whereClause = whereClause + whereOperator + " IIF(TT.T_LAST_POST IS NULL,T.T_DATE,TT.T_LAST_POST) > @LastVisit ";
                whereOperator = "AND";

            }
            if (sparams.BeforeDate != DateTime.MinValue)
            {
                param.Add(new OleDbParameter("@BeforeDate", OleDbType.VarChar) { Value = sparams.SinceDate });
                whereClause = whereClause + whereOperator + " IIF(TT.T_LAST_POST IS NULL,T.T_DATE,TT.T_LAST_POST) < @BeforeDate ";
                whereOperator = "AND";

            }
            if (!String.IsNullOrEmpty(sparams.Author))
            {
                if (sparams.AuthorPostType == "topic")
                {
                    param.Add(new OleDbParameter("@Author", OleDbType.VarChar) { Value = sparams.Author });
                    whereClause = whereClause + whereOperator + " TM.M_NAME=@Author ";

                }
                else if (sparams.AuthorPostType == "any")
                {
                    param.Add(new OleDbParameter("@Author", OleDbType.VarChar) { Value = sparams.Author });
                    whereClause = whereClause + whereOperator + " (TM.M_NAME=@Author OR TRA.M_NAME=@Author) ";

                }
                whereOperator = "AND";
            }
            

            if (!String.IsNullOrEmpty(sparams.SearchFor))
            {
                whereClause = whereClause + whereOperator + " (";

                string[] searchTerms;
                switch (sparams.Match)
                {
                    case "any":
                        whereOperator = "";
                        searchTerms = sparams.SearchFor.Split(new[] { ' ' });
                        if (sparams.MessageAndSubject)
                        {
                            for (int i = 0; i < searchTerms.Length; i++)
                            {
                                param.Add(new OleDbParameter("@SearchFor" + i, OleDbType.VarChar) { Value = "%" + searchTerms[i] + "%" });
                                whereClause = whereClause + whereOperator + "(TT.T_SUBJECT LIKE @SearchFor" + i + " OR TT.T_MESSAGE LIKE @SearchFor" + i + ")";
                                whereOperator = "OR";
                            }
                        }
                        else
                        {
                            for (int i = 0; i < searchTerms.Length; i++)
                            {
                                param.Add(new OleDbParameter("@SearchFor" + i, OleDbType.VarChar) { Value = "%" + searchTerms[i] + "%" });
                                whereClause = whereClause + whereOperator + "TT.T_SUBJECT LIKE @SearchFor" + i + " ";
                                whereOperator = "OR";
                            }
                        }

                        break;
                    case "all":
                        whereOperator = " (";
                        searchTerms = sparams.SearchFor.Split(new[] { ' ' });
                        if (sparams.MessageAndSubject)
                        {
                            for (int i = 0; i < searchTerms.Length; i++)
                            {
                                param.Add(new OleDbParameter("@SearchFor" + i, OleDbType.VarChar) { Value = "%" + searchTerms[i] + "%" });
                                whereClause = whereClause + whereOperator + " TT.T_SUBJECT LIKE @SearchFor" + i + " ";
                                whereOperator = "AND";
                            }
                            whereOperator = ") OR (";
                            for (int i = 0; i < searchTerms.Length; i++)
                            {
                                param.Add(new OleDbParameter("@SearchFor" + (i + searchTerms.Length), OleDbType.VarChar) { Value = "%" + searchTerms[i] + "%" });
                                whereClause = whereClause + whereOperator + " TT.T_MESSAGE LIKE @SearchFor" + (i + searchTerms.Length) + " ";
                                whereOperator = "AND";
                            }
                            whereClause = whereClause + ") ";
                        }
                        else
                        {
                            whereOperator = " (";
                            for (int i = 0; i < searchTerms.Length; i++)
                            {
                                param.Add(new OleDbParameter("@SearchFor" + i, OleDbType.VarChar) { Value = "%" + searchTerms[i] + "%" });
                                whereClause = whereClause + whereOperator + " TT.T_SUBJECT LIKE @SearchFor" + i + " ";
                                whereOperator = "AND";
                            }
                            whereClause = whereClause + ") ";
                        }
                        break;
                    default:
                        if (sparams.MessageAndSubject)
                        {
                            param.Add(new OleDbParameter("@SearchFor", OleDbType.VarChar) { Value = "%" + sparams.SearchFor + "%" });
                            whereClause = whereClause + " TT.T_SUBJECT LIKE @SearchFor OR TT.T_MESSAGE LIKE @SearchFor";
                           
                        }
                        else
                        {
                            param.Add(new OleDbParameter("@SearchFor", OleDbType.VarChar) { Value = "%" + sparams.SearchFor + "%" });
                            whereClause = whereClause + " TT.T_SUBJECT LIKE @SearchFor";
                        }
                        break;
                }
                whereClause = whereClause + ") ";
            }
            #endregion

            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("SELECT DISTINCT ");
            strSql.AppendLine("C.CAT_STATUS, C.CAT_SUBSCRIPTION, C.CAT_NAME,");
            strSql.AppendLine("F.F_SUBJECT, F.F_SUBSCRIPTION, F.F_STATUS, F.F_PRIVATEFORUMS, F.F_PASSWORD_NEW,");
            strSql.AppendLine("T.CAT_ID, T.FORUM_ID, T.TOPIC_ID, T.T_AUTHOR, T.T_SUBJECT, T.T_STATUS, T.T_LAST_POST,");
            strSql.AppendLine("T.T_LAST_POST_AUTHOR, T.T_LAST_POST_REPLY_ID, T.T_REPLIES, T.T_UREPLIES, T.T_VIEW_COUNT,");
            strSql.AppendLine("TA.MEMBER_ID, TA.M_NAME, LRA.M_NAME AS LAST_POST_AUTHOR_NAME ");
            strSql.AppendLine("FROM");
            strSql.AppendLine("((((((");

            strSql.AppendFormat("(SELECT TOP {0} TOPIC_ID, T_LAST_POST FROM  ", sparams.PageSize).AppendLine();
            strSql.AppendFormat("(SELECT TOP {0} TOPIC_ID, T_LAST_POST FROM {1} TT ", Math.Max((rowcount - (currentPage * sparams.PageSize)), sparams.PageSize), topictable).AppendLine();
            strSql.Append(whereClause).AppendLine();
            strSql.AppendLine("ORDER BY TT.T_LAST_POST DESC, TT.TOPIC_ID DESC) AS foo ");
            strSql.AppendLine("ORDER BY T_LAST_POST ASC, TOPIC_ID ASC) as bar ");

            strSql.AppendFormat("INNER JOIN {0} T ON bar.TOPIC_ID = T.TOPIC_ID) ", topictable).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0} R ON T.TOPIC_ID = R.TOPIC_ID) ", replytable).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0}CATEGORY C ON T.CAT_ID = C.CAT_ID) ",Config.ForumTablePrefix).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0}FORUM F ON T.FORUM_ID = F.FORUM_ID) ", Config.ForumTablePrefix).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0}MEMBERS TA ON T.T_AUTHOR = TA.MEMBER_ID) ", Config.MemberTablePrefix).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0}MEMBERS RA ON R.R_AUTHOR = RA.MEMBER_ID) ", Config.MemberTablePrefix).AppendLine();
            strSql.AppendFormat("LEFT JOIN {0}MEMBERS LRA ON T.T_LAST_POST_AUTHOR = LRA.MEMBER_ID ", Config.MemberTablePrefix).AppendLine();

            strSql = strSql.Replace("[WHERECLAUSE]", whereClause);

            List<SearchResult> topics = new List<SearchResult>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql.ToString(), param.ToArray()))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopySearchResultToBO(rdr, ref rowcount));
                }
            }
            return topics;

        }

        public IEnumerable<int> GetReplyIdList(int topicid)
        {
            string strSql = "SELECT REPLY_ID FROM " + Config.ForumTablePrefix + "REPLY WHERE TOPIC_ID=@TopicId ORDER BY R_DATE DESC";
            List<int> replies = new List<int>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@TopicId", OleDbType.Integer) { Value = topicid }))
            {
                while (rdr.Read())
                {
                    replies.Add(rdr.GetInt32(0));
                }
            }
            return replies;
        }

        public IEnumerable<TopicInfo> GetTopics(string lastHereDate, int start, int maxrecs, int? forumid, bool isAdminOrModerator, int? topicstatus,bool stickytopics)
        {
            List<OleDbParameter> param = new List<OleDbParameter>();
            int totalrecs = GetTopicCount(lastHereDate, start, maxrecs, forumid, isAdminOrModerator, topicstatus,stickytopics);
            string where = String.Empty;

            StringBuilder over = new StringBuilder();
            over.AppendLine("FROM ((((");
            over.AppendFormat("SELECT Top {0} sub.TOPIC_ID ", maxrecs);
            over.AppendLine("FROM (");
            over.AppendFormat("SELECT TOP {0} TOPIC_ID ", Math.Max((totalrecs - (start * maxrecs)),maxrecs));
            over.AppendFormat("FROM {0}TOPICS",Config.ForumTablePrefix).AppendLine();
            if(!stickytopics)
                over.AppendLine("WHERE T_STICKY=0");
            else
            {
                over.AppendLine("WHERE T_STICKY>-1 ");
            }
            over.AppendLine("ORDER BY FORUM_ID,T_LAST_POST DESC,T_SUBJECT");
            over.AppendLine(") sub");
            over.AppendFormat(") TE INNER JOIN {0}TOPICS T on TE.TOPIC_ID = T.TOPIC_ID ) ", Config.ForumTablePrefix).AppendLine();
            over.AppendFormat("LEFT OUTER JOIN {0}MEMBERS LPA ON T.T_LAST_POST_AUTHOR = LPA.MEMBER_ID )", Config.MemberTablePrefix).AppendLine();
            over.AppendFormat("LEFT OUTER JOIN {0}MEMBERS AS A ON T.T_AUTHOR = A.MEMBER_ID )", Config.MemberTablePrefix).AppendLine();
            over.AppendFormat("LEFT OUTER JOIN {0}MEMBERS AS EM ON T.T_LAST_EDITBY = EM.MEMBER_ID ", Config.MemberTablePrefix).AppendLine();

            if (topicstatus.HasValue)
            {
                switch (topicstatus.Value)
                {
                    case 0:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        if (!String.IsNullOrEmpty(where))
                            where += " AND ";
                        where = where + "T_STATUS=@Status";
                        break;
                    case 1:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        if (!String.IsNullOrEmpty(where))
                            where += " AND ";
                        where = where + "T_STATUS=@Status";
                        break;
                    case 2:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        if (!String.IsNullOrEmpty(where))
                            where += " AND ";
                        where = where + "T_STATUS=@Status";
                        break;
                }
            }
            else
            {
                if (!isAdminOrModerator)
                {
                    if (!String.IsNullOrEmpty(where))
                        where += " AND ";
                    where = where + "T_STATUS < 2";
                }
            }
            if (forumid.HasValue)
            {
                param.Add(new OleDbParameter("@ForumId", OleDbType.Integer) { Value = forumid });
                if (!String.IsNullOrEmpty(where))
                    where += " AND ";
                where = where + "FORUM_ID=@ForumId";
            }

            if (!String.IsNullOrEmpty(lastHereDate))
            {
                param.Add(new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = lastHereDate });
                if (!String.IsNullOrEmpty(where))
                    where += " AND "; 
                where = where + "IIF(T_LAST_POST IS NULL,T_DATE,T_LAST_POST) > @LastVisit";
            }

            List<TopicInfo> topics = new List<TopicInfo>();

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + TopicCols + over.ToString() + " WHERE " + where, param.ToArray()))
            {
                while (rdr.Read())
                {
                    topics.Add(BoHelper.CopyTopicToBO(rdr));
                }
            }
            return topics;
        }

        public int GetTopicCount(string lastHereDate, int start, int maxrecs, int? forumid, bool isAdminOrModerator, int? topicstatus, bool stickytopics)
        {
            string strSql = "SELECT COUNT(TOPIC_ID) FROM " + Config.ForumTablePrefix + "TOPICS  " + (stickytopics ? "WHERE T_STICKY>-1 " : "WHERE T_STICKY=0 ");

            List<OleDbParameter> param = new List<OleDbParameter>();
            if (topicstatus.HasValue)
            {
                //+ (isAdminOrModerator ? "" : "AND T_STATUS=1")
                switch (topicstatus.Value)
                {
                    case 0:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        strSql = strSql + " AND T_STATUS=@Status";
                        break;
                    case 1:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        strSql = strSql + " AND T_STATUS=@Status";
                        break;
                    case 2:
                        param.Add(new OleDbParameter("@Status", OleDbType.Integer) { Value = topicstatus.Value });
                        strSql = strSql + " AND T_STATUS=@Status";
                        break;
                }
            }
            else
            {
                if (!isAdminOrModerator)
                {
                    strSql = strSql + " AND T_STATUS < 2";
                }
            }
            if (forumid.HasValue)
            {
                param.Add(new OleDbParameter("@ForumId", OleDbType.Integer) { Value = forumid });
                strSql = strSql + " AND FORUM_ID=@ForumId";
            }
            if (!String.IsNullOrEmpty(lastHereDate))
            {
                strSql = strSql + " AND T_LAST_POST > @LastVisit";
                param.Add(new OleDbParameter("@LastVisit", OleDbType.VarChar) { Value = lastHereDate });
            }

            var count = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, param.ToArray());
            return Convert.ToInt32(count);
        }

        #endregion

        #region IBaseObject<TopicInfo> Members

        public TopicInfo GetById(int id)
        {
            TopicInfo topic = null;

            OleDbParameter parm = new OleDbParameter("@TopicId", OleDbType.Integer) { Value = id };

            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, "SELECT" + TopicCols + TopicFrom + "WHERE T.TOPIC_ID = @TopicId", parm))
            {
                while (rdr.Read())
                {
                    topic = BoHelper.CopyTopicToBO(rdr);
                }
            }
            return topic;
        }

        public IEnumerable<TopicInfo> GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public int Add(TopicInfo topic)
        {
            StringBuilder insertSql = new StringBuilder();
            insertSql.AppendFormat("INSERT INTO {0}TOPICS",Config.ForumTablePrefix).AppendLine();
            insertSql.Append("(CAT_ID,FORUM_ID,T_STATUS,T_SUBJECT,T_MESSAGE,T_AUTHOR,T_LAST_POST_AUTHOR,T_REPLIES,T_VIEW_COUNT,T_DATE,T_LAST_POST,T_IP,T_STICKY,T_SIG,T_UREPLIES) VALUES ( ");
            insertSql.AppendLine("@CatId,");
            insertSql.AppendLine("@ForumId,");
            insertSql.AppendLine("@Status,");
            insertSql.AppendLine("@Subject,");
            insertSql.AppendLine("@Message,");
            insertSql.AppendLine("@AuthorId,");
            insertSql.AppendLine("@AuthorId,");
            insertSql.AppendLine("0,0,"); //reply + view count
            insertSql.AppendLine("@Date,");
            insertSql.AppendLine("@Date,");
            insertSql.AppendLine("@PostersIP,");
            insertSql.AppendLine("@Sticky,");
            insertSql.AppendLine("@UseSig,");
            insertSql.AppendLine("0 )"); //unmoderated post count


            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@CatId", OleDbType.Integer) {Value = topic.CatId},
                    new OleDbParameter("@ForumId", OleDbType.Integer) {Value = topic.ForumId},
                    new OleDbParameter("@Status", OleDbType.Integer) {Value = topic.Status},
                    new OleDbParameter("@Subject", OleDbType.VarChar) {Value = topic.Subject},
                    new OleDbParameter("@Message", OleDbType.VarChar) {Value = topic.Message},
                    new OleDbParameter("@AuthorId", OleDbType.Integer) {Value = topic.AuthorId},
                    new OleDbParameter("@Date", OleDbType.VarChar) {Value = topic.Date.ToString("yyyyMMddHHmmss")},
                    new OleDbParameter("@PostersIP", OleDbType.VarChar) {Value = topic.PosterIp},
                    new OleDbParameter("@Sticky", OleDbType.SmallInt) {Value = topic.IsSticky},
                    new OleDbParameter("@UseSig", OleDbType.SmallInt) {Value = topic.UseSignatures}
                };

            int topicid = Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, insertSql.ToString(), parms.ToArray()));
            topic.Id = topicid;
            new Forum().UpdateLastForumPost(topic);
            new Member().UpdateLastMemberPost(topic);
            return topicid;
        }

        public void Update(TopicInfo topic)
        {
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    
                    new OleDbParameter("@Status", OleDbType.Integer) {Value = topic.Status},
                    new OleDbParameter("@Subject", OleDbType.VarChar) {Value = topic.Subject},
                    new OleDbParameter("@Message", OleDbType.VarChar) {Value = topic.Message},
                    new OleDbParameter("@Sticky", OleDbType.SmallInt) {Value = topic.IsSticky},
                    new OleDbParameter("@UseSig", OleDbType.SmallInt) {Value = topic.UseSignatures},
                    new OleDbParameter("@TopicId", OleDbType.Integer) {Value = topic.Id}
                };

            StringBuilder updateSql = new StringBuilder(" ");
            updateSql.AppendFormat("UPDATE {0}TOPICS SET",Config.ForumTablePrefix).AppendLine();
            updateSql.AppendLine("T_STATUS=@Status,");
            updateSql.AppendLine("T_SUBJECT=@Subject,");
            updateSql.AppendLine("T_MESSAGE=@Message,");
            if (topic.LastEditedById.HasValue)
            {
                updateSql.AppendLine("T_LAST_EDITBY=@EditedBy,");
                updateSql.AppendLine("T_LAST_EDIT=@EditDate,");
                parms.Add(new OleDbParameter("@EditedBy", OleDbType.Integer) { Value = topic.LastEditedById.Value });
                parms.Add(new OleDbParameter("@EditDate", OleDbType.VarChar) { Value = DateTime.UtcNow.ToString("yyyyMMddHHmmss") });

            }
            updateSql.AppendLine("T_STICKY=@Sticky,");
            updateSql.AppendLine("T_SIG=@UseSig ");
            updateSql.AppendLine("WHERE TOPIC_ID=@TopicId");

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, updateSql.ToString(), parms.ToArray());

        }

        public void Delete(TopicInfo topic)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "REPLY WHERE TOPIC_ID=@TopicId; DELETE FROM " + Config.ForumTablePrefix + "TOPICS WHERE TOPIC_ID=@TopicId";

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@TopicId", OleDbType.Integer) { Value = topic.Id });
            new AdminFunctions().UpdateForumCounts();
        }

        public void Dispose()
        {

        }

        #endregion

    }
}
