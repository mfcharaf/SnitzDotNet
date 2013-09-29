/*
####################################################################################################################
##
## Snitz.Entities - PollsInfo
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

namespace Snitz.Entities
{
    public class PollInfo
    {
        public int Id { get; set; }
        public string DisplayText { get; set; }
        public int? TopicId { get; set; }
        public int? CloseAfterDays { get; set; }
        public bool ShowResultsBeforeClose { get; set; }
        public TopicInfo TopicInfo { get; set; }
        public IList<PollChoiceInfo> Choices { get; set; }

        public PollInfo()
        {
            Choices = new List<PollChoiceInfo>();
        }
        public void AddChoice(PollChoiceInfo choiceInfo)
        {
            choiceInfo.PollInfo = this;
            Choices.Add(choiceInfo);
        }
        public bool IsActivePoll { get; set; }
    }
    public class PollChoiceInfo
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string DisplayText { get; set; }
        public int Order { get; set; }

        public PollInfo PollInfo { get; set; }
    }
    public class PollResponseInfo
    {
        public int MemberId { get; set; }
        public int PollAnswerId { get; set; }
        public AuthorInfo Respondent { get; set; }
        public PollChoiceInfo Response { get; set; }
    }

    public class TopicPoll
    {
        public string Question { get; set; }
        public Dictionary<int, string> Answers { get; set; } 
    }

    public class PollResponse
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public int Votes { get; set; }
    }
}
