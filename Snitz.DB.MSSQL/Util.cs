using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web.Security;
using SnitzCommon;
using SnitzConfig;

namespace SnitzData
{
    /// <summary>
    /// Utility class for returning data objects
    /// </summary>
    public static partial class Util
    {
        #region Category Utils

        /// <summary>
        /// Returns category object
        /// </summary>
        /// <param name="catid">Id of category</param>
        /// <returns>Category</returns>
        public static Category GetCategory(int catid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return dc.GetCategory(catid);
            }
        }
        /// <summary>
        /// Fetches a list of categories
        /// </summary>
        /// <returns></returns>
        public static List<Category> ListCategories()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                List<Category> result = (from cats in dc.Categories orderby cats.Order select cats).ToList();
                return result.ToList();
            }
        }
        
        public static void SetCatStatus(int catid, Enumerators.PostStatus status)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                Category cat = dc.Categories.SingleOrDefault(f => f.Id == catid);
                cat.CAT_STATUS = (short)status;
                dc.SubmitChanges();
            }
        }

        public static void DeleteCat(int catid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.Replies.DeleteAllOnSubmit(dc.Replies.Where(r => r.CatId == catid));
                dc.Topics.DeleteAllOnSubmit(dc.Topics.Where(t => t.CatId == catid));
                dc.Forums.DeleteAllOnSubmit(dc.Forums.Where(f => f.CatId == catid));
                dc.Categories.DeleteAllOnSubmit(dc.Categories.Where(c => c.Id == catid));
                dc.SubmitChanges();
                RemoveAllCategorySubscriptions(catid);
            }
        }
        
        public static int SaveCategory(Category cat)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                if (cat.Id == -1)
                {
                    cat.CAT_STATUS = (int)Enumerators.PostStatus.Open;
                    cat.Order = 99;
                    dc.Categories.InsertOnSubmit(cat);
                }
                else
                {
                    Category toUpd = dc.Categories.SingleOrDefault(c => c.Id == cat.Id);
                    toUpd.Name = cat.Name;
                    toUpd.ModerationLevel = cat.ModerationLevel;
                    toUpd.SubscriptionLevel = cat.SubscriptionLevel;
                    toUpd.Order = cat.Order;
                }
                dc.SubmitChanges();
                return cat.Id;
            }
        }

        #endregion

        #region Member related Utils

        /// <summary>
        /// Get complete member Object
        /// </summary>
        /// <param name="username">Username of Member</param>
        /// <returns></returns>
        public static Member GetMember(string username)
        {
            if (String.IsNullOrEmpty(username))
            {
                Member guest = new Member {TimeOffset = 0};
                return guest;
            }

            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                try
                {
                    var user = dc.GetMember(username) ?? new Member { TimeOffset = 0 };
                    return user;
                }
                catch (Exception)
                {
                    Member guest = new Member { TimeOffset = 0 };
                    return guest;
                }
                
            }
        }
        /// <summary>
        /// Get complete member Object
        /// </summary>
        /// <param name="userid">userId of Member</param>
        /// <returns></returns>
        private static Member GetMember(int userid)
        {
            using (var dc = new SnitzDataClassesDataContext())
            {
                return (from member in dc.Members where member.Id == userid select member).SingleOrDefault();
            }
        }
        /// <summary>
        /// Get Author Object (subset of Member)
        /// </summary>
        /// <param name="authorId">Id of post author</param>
        /// <returns></returns>
        public static List<Member> GetAuthor(int? authorId)
        {
            using (var dc = new SnitzDataClassesDataContext())
            {
                return (from author in dc.Members where author.Id == authorId select author).ToList();
            }
        }
        /// <summary>
        /// get a list of Forum moderators
        /// </summary>
        /// <returns>All Moderators</returns>
        public static List<Member> ListModerators()
        {
            //todo: should be using roles table
            using (var dc = new SnitzDataClassesDataContext())
            {
                return
                    (from u in dc.Members
                     where u.M_LEVEL == (int)Enumerators.UserLevels.Moderator && u.IsActive == 1
                     select u).ToList();
            }
        }

        #endregion

        #region Word Filters

        /// <summary>
        /// Fetches the badwordlist from the database
        /// </summary>
        /// <returns></returns>
        public static List<WordFilter> ListBadWords()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return (from bwords in dc.WordFilters orderby bwords.BadWord select bwords).ToList();
            }
        }
        /// <summary>
        /// fetches the names filter from the database
        /// </summary>
        /// <returns></returns>
        public static List<NameFilter> ListNameFilters()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                return (from names in dc.NameFilters orderby names.Name select names).ToList();
            }
        }

        public static void DeleteBadWord(int id)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.WordFilters.DeleteAllOnSubmit(dc.WordFilters.Where(w => w.Id == id));
                dc.SubmitChanges();
            }
        }

        public static void UpdateBadWord(int id, string badword, string replace)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                WordFilter word = dc.WordFilters.Where(w => w.Id == id).SingleOrDefault();
                word.BadWord = badword;
                word.Replace = replace;
                dc.SubmitChanges();
            }
        }

        public static void DeleteNameFilter(int id)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.NameFilters.DeleteAllOnSubmit(dc.NameFilters.Where(w=>w.Id == id));
                dc.SubmitChanges();
            }
        }

        public static void UpdateNameFilter(int id, string name)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                NameFilter filter = dc.NameFilters.Where(f => f.Id == id).SingleOrDefault();
                filter.Name = name;
                dc.SubmitChanges();
            }
        }
        public static int InsertNameFilter(string name)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                NameFilter filter = new NameFilter {Name = name};
                dc.NameFilters.InsertOnSubmit(filter);
                dc.SubmitChanges();
                return filter.Id;
            }

        }
        public static int InsertBadWord(string badword, string replace)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                WordFilter filter = new WordFilter {BadWord = badword, Replace = replace};
                dc.WordFilters.InsertOnSubmit(filter);
                dc.SubmitChanges();
                return filter.Id;
            }

        }
        #endregion

        /// <summary>
        /// Encrypts a string using the Cryptos Utility
        /// </summary>
        /// <param name="text">string to encrypt</param>
        /// <returns>encrypted string</returns>
        public static string Encrypt(string text)
        {
            return Cryptos.CryptosUtilities.Encrypt(text);
        }

        public static bool CastVote(object userid, int answerid)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                string sql = "INSERT INTO FORUM_POLLRESPONSE (UserID, PollAnswerID) VALUES ({0}, {1})";

                int res = dc.ExecuteCommand(sql,userid,answerid );
                return res > 0;
            }
        }

        public static void AddTopicPoll(int postid, string question, SortedList<int, string> sortedList)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                var pollid = dc.ExecuteQuery<int>("INSERT INTO FORUM_POLLS (DisplayText,TopicId) VALUES ({0}, {1});SELECT Convert(Int, @@IDENTITY);", question, postid).FirstOrDefault();

                foreach (KeyValuePair<int, string> answer in sortedList)
                {
                    dc.ExecuteCommand("INSERT INTO FORUM_POLLANSWERS (PollID,DisplayText,SortOrder) VALUES ({0},{1},{2})", pollid, answer.Value, answer.Key);
                }
            }
        }

        public static string GetTopicPoll(int? pollid)
        {
            if (pollid < 1)
                return String.Empty;
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                string sql = String.Format("SELECT P.DisplayText AS Question, A.DisplayText AS Answer, A.SortOrder FROM FORUM_POLLS P LEFT OUTER JOIN FORUM_POLLANSWERS A ON P.PollID = A.PollID WHERE (P.PollID = {0}) ORDER BY A.SortOrder", pollid);
                List<TopicPoll> poll = dc.ExecuteQuery<TopicPoll>(sql).ToList();
                string polltag = String.Format("[poll=\"{0}\"]\r\n", poll.First().Question);
                StringBuilder pollstr = new StringBuilder(polltag);
                foreach (TopicPoll topicPoll in poll)
                {
                    pollstr.AppendFormat("[*={0}]{1}[/*]\r\n",topicPoll.SortOrder, topicPoll.Answer);
                }
                pollstr.AppendLine("[/poll]");
                return pollstr.ToString();
            }
        }

        private class TopicPoll
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public int SortOrder { get; set; }
        }

        public static void UpdateTopicPoll(int? id, string question, SortedList<int, string> answers)
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.ExecuteCommand("UPDATE FORUM_POLLS SET DisplayText={0} WHERE PollID={1}", question, id);
                foreach (KeyValuePair<int, string> answer in answers)
                {
                    if (answer.Value == "" || answer.Value.ToLower() == "remove")
                    {
                        dc.ExecuteCommand("DELETE FROM FORUM_POLLANSWERS WHERE PollID={0} AND SortOrder={1}", id, answer.Key);
                    }
                    else
                    {
                        int res = 0;
                        try
                        {
                            res = dc.ExecuteCommand(
                                "UPDATE FORUM_POLLANSWERS SET DisplayText={0} WHERE PollID={1} AND SortOrder={2}",
                                answer.Value, id, answer.Key);
                            if (res == 0)
                            {
                                //didn't update anything so assume new answer and insert
                                dc.ExecuteCommand("INSERT INTO FORUM_POLLANSWERS (PollID,DisplayText,SortOrder) VALUES ({0},{1},{2})", id, answer.Value, answer.Key);
                            }
                        }
                        catch (Exception)
                        {

                        }

                    }
                    
                }
            }
        }

        public static void DeleteTopicPoll(int? pollId)
        {
            string deletePollSql = String.Format("DELETE FROM FORUM_POLLRESPONSE WHERE PollAnswerID IN (SELECT PollAnswerID FROM FORUM_POLLANSWERS WHERE PollID={0}); DELETE FROM FORUM_POLLANSWERS WHERE PollID={0}; DELETE FROM FORUM_POLLS WHERE PollID={0}", pollId);
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.ExecuteCommand(deletePollSql);
            }
        }

        public static bool UpdateForumCounts()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                dc.ExecuteCommand("snitz_updateCounts");
            }
            return true;
        }

        public static string GetDBSize()
        {
            using (SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext())
            {
                //res = dc.ExecuteQuery<int>("sp_spaceused").ToList();
            }

            return ""; //"string.Format("{0} / {1}", res[0], res[1]);
        }
    }
}