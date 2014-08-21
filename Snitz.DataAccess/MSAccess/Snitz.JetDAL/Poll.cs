using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.OLEDbDAL.Helpers;
using SnitzConfig;

namespace Snitz.OLEDbDAL
{
    public class Poll : IPoll
    {

        #region IPoll Members

        public bool CastVote(object userid, int answerid)
        {
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "POLLRESPONSE (UserID, PollAnswerID) VALUES (@User, @Answer)";

            List<OleDbParameter> parms = new List<OleDbParameter>(new OleDbParameter[2])
                {
                    new OleDbParameter("@User", OleDbType.Numeric) {Value = userid},
                    new OleDbParameter("@Answer", OleDbType.Numeric) {Value = answerid}
                };

            int res = Convert.ToInt32(SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray()));

            return res > 0;
        }

        public bool CanUserVote(int pollId, int userId)
        {
            string strSql = "SELECT COUNT(*) FROM " + Config.ForumTablePrefix + "POLLRESPONSE r INNER JOIN " + Config.ForumTablePrefix + "POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID AND r.UserID = @UserID";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@PollID", OleDbType.Numeric) {Value = pollId},
                    new OleDbParameter("@UserID", OleDbType.Numeric) {Value = userId}
                };

            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray())) == 0;
        }

        public int TotalVotes(int pollId)
        {
            string strSql = "SELECT COUNT(*) FROM " + Config.ForumTablePrefix + "POLLRESPONSE r INNER JOIN " + Config.ForumTablePrefix + "POLLANSWERS a ON r.PollAnswerID = a.PollAnswerID WHERE a.PollID = @PollID";
            // Calculate the total # of votes
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@PollID", OleDbType.Numeric) { Value = pollId }));
        }

        public IEnumerable<PollChoiceInfo> GetPollChoices(int pollId)
        {
            string strSql = "SELECT PollAnswerId,DisplayText,SortOrder FROM " + Config.ForumTablePrefix + "POLLANSWERS WHERE PollId=@PollId ORDER BY SortOrder";
            List<PollChoiceInfo> answers = new List<PollChoiceInfo>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@PollID", OleDbType.Numeric) { Value = pollId }))
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
            string strSql = "SELECT a.PollAnswerID, a.DisplayText,COUNT(r.UserID) AS Votes FROM " + Config.ForumTablePrefix + "POLLANSWERS AS a LEFT OUTER JOIN " +
                                  Config.ForumTablePrefix + "POLLRESPONSE AS r ON a.PollAnswerID = r.PollAnswerID WHERE a.PollId=@PollId " +
                                  "GROUP BY a.PollID, a.DisplayText, a.PollAnswerID,a.SortOrder " +
                                  "ORDER BY a.SortOrder";
            List<PollResponse> responses = new List<PollResponse>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@PollID", OleDbType.Numeric) { Value = pollId }))
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
            string strSql = "SELECT P.DisplayText AS Question, A.DisplayText AS Answer, A.SortOrder FROM " + Config.ForumTablePrefix + "POLLS P LEFT OUTER JOIN " + Config.ForumTablePrefix + "POLLANSWERS A ON P.PollID = A.PollID WHERE (P.PollID = @PollId) ORDER BY A.SortOrder";

            TopicPoll poll = new TopicPoll { Answers = new Dictionary<int, string>() };
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@PollID", OleDbType.Numeric) { Value = pollId }))
            {
                while (rdr.Read())
                {
                    poll.Question = rdr.GetString(0);
                    poll.Answers.Add(rdr.GetInt32(2), rdr.GetString(1));
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
            string strSql = "INSERT INTO " + Config.ForumTablePrefix + "POLLANSWERS (PollID,DisplayText,SortOrder) VALUES (@PollId,@Answer,@Order)";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@PollID", OleDbType.Numeric) {Value = pollChoice.PollId},
                    new OleDbParameter("@Order", OleDbType.Numeric) {Value = pollChoice.Order},
                    new OleDbParameter("@Answer", OleDbType.VarChar) {Value = pollChoice.DisplayText}
                };
            SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());
        }

        public void UpdateChoice(PollChoiceInfo pollChoice)
        {
            string strSql = "UPDATE " + Config.ForumTablePrefix + "POLLANSWERS SET PollID = @PollId,DisplayText = @Answer,SortOrder = @Order WHERE PollAnswerID=@Id";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@PollID", OleDbType.Numeric) {Value = pollChoice.PollId},
                    new OleDbParameter("@Order", OleDbType.Numeric) {Value = pollChoice.Order},
                    new OleDbParameter("@Answer", OleDbType.VarChar) {Value = pollChoice.DisplayText},
                    new OleDbParameter("@Id", OleDbType.Numeric) {Value = pollChoice.Id}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, parms.ToArray());

        }

        public void DeleteChoice(int pollChoiceId)
        {
            string strSql = "DELETE FROM " + Config.ForumTablePrefix + "POLLANSWERS WHERE PollAnswerID=@Id";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@Id", OleDbType.Numeric) { Value = pollChoiceId });
        }

        public IEnumerable<PollInfo> GetPolls()
        {
            string strSql = "SELECT PollId,DisplayText,TopicId FROM " + Config.ForumTablePrefix + "POLLS";
            List<PollInfo> polls = new List<PollInfo>();
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, null))
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
            string strSql = "SELECT PollId,DisplayText,TopicId FROM " + Config.ForumTablePrefix + "POLLS WHERE PollId=@PollId";
            PollInfo poll = null;
            using (OleDbDataReader rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, strSql, new OleDbParameter("@PollID", OleDbType.Numeric) { Value = id }))
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
            string pollSql = "INSERT INTO " + Config.ForumTablePrefix + "POLLS (DisplayText,TopicId) VALUES (@Question,@TopicId)";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@TopicId", OleDbType.Numeric) {Value = poll.TopicId},
                    new OleDbParameter("@Question", OleDbType.VarChar) {Value = poll.DisplayText}
                };

            int pollid = Convert.ToInt32(SqlHelper.ExecuteInsertQuery(SqlHelper.ConnString, CommandType.Text, pollSql, parms.ToArray()));

            StringBuilder choices = new StringBuilder();
            foreach (var choice in poll.Choices)
            {
                choices.AppendFormat("INSERT INTO {0}POLLANSWERS (PollID,DisplayText,SortOrder) VALUES (@PollId,'{1}',{2})",Config.ForumTablePrefix, choice.DisplayText, choice.Order);
            }
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, choices.ToString(), new OleDbParameter("@PollId", OleDbType.Numeric) { Value = pollid });

            return pollid;
        }

        public void Update(PollInfo poll)
        {
            string pollSql = "UPDATE " + Config.ForumTablePrefix + "POLLS SET DisplayText=@Question WHERE PollId=@PollId";
            List<OleDbParameter> parms = new List<OleDbParameter>
                {
                    new OleDbParameter("@PollId", OleDbType.Numeric) {Value = poll.Id},
                    new OleDbParameter("@Question", OleDbType.VarChar) {Value = poll.DisplayText}
                };

            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, pollSql, parms.ToArray());
            if (poll.Choices == null)
                return;
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
            string pollSql = "DELETE FROM " + Config.ForumTablePrefix + "POLLS WHERE PollId=@PollId";
            SqlHelper.ExecuteNonQuery(SqlHelper.ConnString, CommandType.Text, pollSql, new OleDbParameter("@PollId", OleDbType.Numeric) { Value = poll.Id });
        }

        public void Dispose()
        {

        }

        #endregion
    }
}
