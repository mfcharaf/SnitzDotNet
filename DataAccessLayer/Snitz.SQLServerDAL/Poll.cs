using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;

namespace Snitz.SQLServerDAL
{
    public class Poll : IPoll
    {

        #region IPoll Members

        public bool CastVote(object userid, int answerid)
        {
            const string strSql = "INSERT INTO FORUM_POLLRESPONSE (UserID, PollAnswerID) VALUES (@User, @Answer);";
            
            List<SqlParameter> parms = new List<SqlParameter>(new SqlParameter[2])
                {
                    new SqlParameter("@User", SqlDbType.Int) {Value = userid},
                    new SqlParameter("@Answer", SqlDbType.Int) {Value = answerid}
                };

            int res = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public bool CanUserVote(int pollId, int userId)
        {
            const string strSql = "SELECT COUNT(*) FROM FORUM_POLLRESPONSE r INNER JOIN FORUM_POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID AND r.UserID = @UserID";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@PollID", SqlDbType.Int) {Value = pollId},
                    new SqlParameter("@UserID", SqlDbType.Int) {Value = userId}
                };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray())) == 0;
        }

        public int TotalVotes(int pollId)
        {
            const string strSql = "SELECT COUNT(*) FROM FORUM_POLLRESPONSE r INNER JOIN FORUM_POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID";
            // Calculate the total # of votes
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql,new SqlParameter("@PollID", SqlDbType.Int) {Value = pollId}));
        }

        public IEnumerable<PollChoiceInfo> GetPollChoices(int pollId)
        {
            const string strSql = "SELECT PollAnswerId,DisplayText,SortOrder FROM FORUM_POLLANSWERS WHERE PollId=@PollId ORDER BY SortOrder";
            List<PollChoiceInfo> answers = new List<PollChoiceInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@PollID", SqlDbType.Int) { Value = pollId }))
            {
                while (rdr.Read())
                {
                   PollChoiceInfo answer = new PollChoiceInfo
                       {
                           Id = rdr.GetInt32(0),
                           DisplayText = rdr.SafeGetString(1),
                           Order = rdr.GetInt32(2),
                           PollId = pollId
                       };
                    answers.Add(answer);
                }
            }
            return answers;
        }

        public IEnumerable<PollResponse> GetPollResponses(int pollId)
        {
            const string strSql = "SELECT a.PollAnswerID, a.DisplayText,COUNT(r.UserID) AS Votes FROM FORUM_POLLANSWERS AS a LEFT OUTER JOIN " +
                                  "FORUM_POLLRESPONSE AS r ON a.PollAnswerID = r.PollAnswerID WHERE a.PollId=@PollId " + 
                                  "GROUP BY a.PollID, a.DisplayText, a.PollAnswerID,a.SortOrder " +
                                  "ORDER BY a.SortOrder";
            List<PollResponse> responses = new List<PollResponse>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@PollID", SqlDbType.Int) { Value = pollId }))
            {
                while (rdr.Read())
                {
                    PollResponse response = new PollResponse
                    {
                        Id = rdr.GetInt32(0),
                        Answer = rdr.GetString(1),
                        Votes = rdr.GetInt32(2)
                    };
                    responses.Add(response);
                }
            }
            return responses;
        }

        public string GetPollTopicString(int? pollId)
        {
            const string strSql = "SELECT P.DisplayText AS Question, A.DisplayText AS Answer, A.SortOrder FROM FORUM_POLLS P LEFT OUTER JOIN FORUM_POLLANSWERS A ON P.PollID = A.PollID WHERE (P.PollID = @PollId) ORDER BY A.SortOrder";
            
            TopicPoll poll = new TopicPoll {Answers = new Dictionary<int, string>()};
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@PollID", SqlDbType.Int) { Value = pollId }))
            {
                while (rdr.Read())
                {
                    poll.Question = rdr.GetString(0);
                    poll.Answers.Add(rdr.GetInt32(2),rdr.GetString(1));
                }
            }
            string polltag = String.Format("[poll=\"{0}\"]\r\n", poll.Question);
            StringBuilder pollstr = new StringBuilder(polltag);
            foreach (var answer in poll.Answers)
            {
                pollstr.AppendFormat("[*={0}]{1}[/*]\r\n", answer.Key, answer.Value);
            }
            pollstr.AppendLine("[/poll]");
            return pollstr.ToString();
        }

        public void AddChoice(PollChoiceInfo pollChoice)
        {
            const string strSql = "INSERT INTO FORUM_POLLANSWERS (PollID,DisplayText,SortOrder) VALUES (@PollId,@Answer,@Order); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@PollID", SqlDbType.Int) {Value = pollChoice.PollId},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = pollChoice.Order},
                    new SqlParameter("@Answer", SqlDbType.NVarChar) {Value = pollChoice.DisplayText}
                };
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void UpdateChoice(PollChoiceInfo pollChoice)
        {
            const string strSql = "UPDATE FORUM_POLLANSWERS SET PollID = @PollId,DisplayText = @Answer,SortOrder = @Order WHERE PollAnswerID=@Id";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@PollID", SqlDbType.Int) {Value = pollChoice.PollId},
                    new SqlParameter("@Order", SqlDbType.Int) {Value = pollChoice.Order},
                    new SqlParameter("@Answer", SqlDbType.NVarChar) {Value = pollChoice.DisplayText},
                    new SqlParameter("@Id", SqlDbType.Int) {Value = pollChoice.Id}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public void DeleteChoice(int pollChoiceId)
        {
            const string strSql = "DELETE FROM FORUM_POLLANSWERS WHERE PollAnswerID=@Id";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@Id", SqlDbType.Int) { Value = pollChoiceId });
        }

        public IEnumerable<PollInfo> GetPolls()
        {
            const string strSql = "SELECT PollId,DisplayText,TopicId FROM FORUM_POLLS";
            List<PollInfo> polls = new List<PollInfo>();
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
            {
                while (rdr.Read())
                {
                    polls.Add(new PollInfo
                    {
                        Id = rdr.GetInt32(0),
                        DisplayText = rdr.GetString(1),
                        TopicId = rdr.SafeGetInt32(2)
                    });
                }
            }
            return polls;
        }

        #endregion

        #region IBaseObject<PollInfo> Members

        public PollInfo GetById(int id)
        {
            const string strSql = "SELECT PollId,DisplayText,TopicId FROM FORUM_POLLS WHERE PollId=@PollId";
            PollInfo poll = null;
            using (SqlDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new SqlParameter("@PollID", SqlDbType.Int) { Value = id }))
            {
                while (rdr.Read())
                {
                    poll = new PollInfo
                        {
                            Id = rdr.GetInt32(0),
                            DisplayText = rdr.SafeGetString(1),
                            TopicId = rdr.GetInt32(2)
                        };
                }
            }
            return poll;
        }

        public IEnumerable<PollInfo> GetByName(string name)
        {
            //not relevent for polls
            return null;
        }

        public int Add(PollInfo poll)
        {
            const string pollSql = "INSERT INTO FORUM_POLLS (DisplayText,TopicId) VALUES (@Question,@TopicId); SELECT SCOPE_IDENTITY();";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@TopicId", SqlDbType.Int) {Value = poll.TopicId},
                    new SqlParameter("@Question", SqlDbType.NVarChar) {Value = poll.DisplayText}
                };

            int pollid = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, pollSql, parms.ToArray()));

            StringBuilder choices = new StringBuilder();
            foreach (var choice in poll.Choices)
            {
                choices.AppendFormat("INSERT INTO FORUM_POLLANSWERS (PollID,DisplayText,SortOrder) VALUES (@PollId,'{0}',{1});",choice.DisplayText, choice.Order);
            }
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, choices.ToString(),new SqlParameter("@PollId", SqlDbType.Int) {Value = pollid});

            return pollid;
        }

        public void Update(PollInfo poll)
        {
            const string pollSql = "UPDATE FORUM_POLLS SET DisplayText=@Question WHERE TopicId=@TopicId;";
            List<SqlParameter> parms = new List<SqlParameter>
                {
                    new SqlParameter("@TopicId", SqlDbType.Int) {Value = poll.TopicId},
                    new SqlParameter("@Question", SqlDbType.NVarChar) {Value = poll.DisplayText}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, pollSql, parms.ToArray());

            var choices = GetPollChoices(poll.Id).ToArray();
            int currentchoicecount = choices.Length;
            int newchoicecount = poll.Choices.Count;

            for (int ch = 0; ch < choices.Count(); ch++)
            {
                if (ch < newchoicecount)
                {
                    choices[ch].DisplayText = poll.Choices[ch].DisplayText;
                    choices[ch].Order = poll.Choices[ch].Order;
                    UpdateChoice(choices[ch]);
                }
                else
                {
                    break;
                }
            }
            if (newchoicecount > currentchoicecount)
            {
                for (int i = currentchoicecount; i < newchoicecount; i++)
                {
                    PollChoiceInfo choice = new PollChoiceInfo
                        {
                            DisplayText = poll.Choices[i].DisplayText,
                            PollId = poll.Id,
                            Order = poll.Choices[i].Order
                        };
                    AddChoice(choice);
                }
            }
            else if (newchoicecount < currentchoicecount)
            {
                for (int i = newchoicecount; i < currentchoicecount; i++)
                {
                    DeleteChoice(choices[i].Id);
                }                
            }
        }

        public void Delete(PollInfo poll)
        {
            const string pollSql = "DELETE FROM FORUM_POLLS WHERE PollId=@PollId;";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, pollSql, new SqlParameter("@PollId",SqlDbType.Int){Value = poll.Id});
        }

        public void Dispose()
        {

        }

        #endregion
    }
}
