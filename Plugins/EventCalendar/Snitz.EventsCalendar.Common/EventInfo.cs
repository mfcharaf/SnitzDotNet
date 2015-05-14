﻿/*
####################################################################################################################
##
## Snitz.Entities - EventInfo
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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

using System;

namespace Snitz.Entities
{
    [Serializable]
    public class EventInfo : IEvent
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int Type { get; set; }

        public string Audience { get; set; }

        public int MemberId { get; set; }

        public DateTime Date { get; set; }

        public bool Enabled { get; set; }

        public RecurringFrequencies RecurringFrequency { get; set; }

        public string Description { get; set; }

        public AuthorInfo Author { get; set; }

        public bool ThisDayForwardOnly { get; set; }

        public CustomRecurringFrequenciesHandler CustomRecurringFunction { get; set; }

        /// <summary>
        /// EventInfo Constructor
        /// </summary>
        public EventInfo()
        {
            Enabled = true;
            ThisDayForwardOnly = true;
            RecurringFrequency = RecurringFrequencies.None;
        }

        public IEvent Clone()
        {
            return new EventInfo
            {
                CustomRecurringFunction = CustomRecurringFunction,
                Date = Date,
                Enabled = Enabled,
                Title = Title,
                Type = Type,
                RecurringFrequency = RecurringFrequency,
                ThisDayForwardOnly = ThisDayForwardOnly
            };
        }

    }
}
