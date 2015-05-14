/*
####################################################################################################################
##
## Snitz.Entities - Rank
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
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using SnitzConfig;

namespace Snitz.Entities
{
    public class RankInfo
    {
        private Dictionary<int, Ranking> _ranking;

        private int _Level;
        public string Title { get; set; }
        private readonly int? _Posts;
        private readonly string _User;
        public string Stars {
            get { return GetStars(); }
        }   

        public RankInfo(string username, ref string title, int? posts,Dictionary<int,Ranking> rankings)
        {
            _ranking = rankings;
            _User = username;
            _Posts = posts;
            SetLevel();
            if (String.IsNullOrEmpty(title))
                title = Title;
            else
            {
                Title = title;
            }

        }

        public string GetStars()
        {
            if (_Level == 0) return "";
            StringBuilder imageString = new StringBuilder("");

            int imageRepeat = _ranking[_Level + 1].Repeat;

            string rankImage = _ranking[_Level + 1].Image;
            if (Roles.IsUserInRole(_User, "Administrator"))
            {
                imageRepeat = _ranking[0].Repeat;
                rankImage = _ranking[0].Image; //Admin;
            }
            if (Roles.IsUserInRole(_User, "Moderator"))
            {
                imageRepeat = _ranking[1].Repeat;
                rankImage = _ranking[1].Image;
            }

            if (rankImage != "")
            {
                for (int ii = 1; ii <= imageRepeat; ii++)
                {
                    string clientpath = VirtualPathUtility.ToAbsolute(Config.ImageDirectory + "ranking/");
                    imageString.AppendFormat("<img src='{0}{1}' alt=''/>", clientpath, rankImage);
                }
            }
            return imageString.ToString() + "<br/>";
        }

        private void SetLevel()
        {
            string rankTitle = "";
            _Level = -1;

            foreach (KeyValuePair<int, Ranking> ranking in _ranking)
            {
                if (ranking.Key < 2)
                    continue;
                if (_Posts >= ranking.Value.RankLevel)
                {
                    rankTitle = ranking.Value.Title;
                    _Level++;
                }
                if (_Posts < ranking.Value.RankLevel)
                    break;
            }
            if (Roles.IsUserInRole(_User, "Administrator"))
            {
                rankTitle = "Forum Administrator";
            }
            if (Roles.IsUserInRole(_User, "Moderator"))
            {
                rankTitle = "Forum Moderator";
            }            
            Title = rankTitle;
        }
        
    }

    public struct Ranking
    {
        public string Title;
        public string Image;
        public int RankLevel;
        public int Repeat;
    }
}
