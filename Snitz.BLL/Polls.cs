/*
####################################################################################################################
##
## Snitz.BLL - Polls
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System.Collections.Generic;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Polls
    {
        public static bool CastVote(object memberid, int answerid)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return dal.CastVote(memberid, answerid);
        }

        public static PollInfo GetTopicPoll(int pollid)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            
            return dal.GetById(pollid);
        }

        public static void AddTopicPoll(int postid, string question, SortedList<int, string> choices)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            PollInfo poll = new PollInfo {TopicId = postid, DisplayText = question};

            foreach (var choice in choices)
            {
                PollChoiceInfo pollchoice = new PollChoiceInfo {DisplayText = choice.Value, Order = choice.Key};
                poll.AddChoice(pollchoice);
            }
            dal.Add(poll);
        }

        public static void DeleteTopicPoll(int pollid)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            dal.Delete(GetTopicPoll(pollid));
        }


        public static void UpdateTopicPoll(int pollId, string question, SortedList<int, string> choices)
        {
            PollInfo poll = new PollInfo {Id = pollId, DisplayText = question, Choices = new List<PollChoiceInfo>()};
            foreach (var choice in choices)
            {
                poll.Choices.Add(new PollChoiceInfo {DisplayText = choice.Value,Order = choice.Key,PollId = pollId});
            }
            IPoll dal = Factory<IPoll>.Create("Poll");
            dal.Update(poll);
        }

        public static bool CanUserTakePoll(int pollId, int userId)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return dal.CanUserVote(pollId, userId);
        }

        public static int GetTotalVotes(int pollId)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return dal.TotalVotes(pollId);

        }

        public static List<PollChoiceInfo> GetPollChoices(int pollId)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return new List<PollChoiceInfo>(dal.GetPollChoices(pollId));
        }

        public static List<PollResponse> GetResults(int pollId)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return new List<PollResponse>(dal.GetPollResponses(pollId));
        }

        public static string GetTopicPollString(int? pollId)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return dal.GetPollTopicString(pollId);
        }

        public static List<PollInfo> GetPolls()
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            return new List<PollInfo>(dal.GetPolls());
        }

        public static void AddChoice(PollChoiceInfo choice)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            dal.AddChoice(choice);
        }

        public static void UpdatePollAnswer(PollChoiceInfo choice)
        {
            IPoll dal = Factory<IPoll>.Create("Poll");
            dal.UpdateChoice(choice);
        }
    }
}
