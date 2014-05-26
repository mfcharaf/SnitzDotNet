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
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;
using SnitzConfig;

namespace Snitz.Entities
{
    public class RankInfo
    {
        public static string[] rankColor
        {
            //array containing rank images, number after the | denotes the repeat value for the image
            get
            {
                string[] arrColor = new string[8];
                arrColor[0] = "icon_star_gold.gif|5"; //Administrators
                arrColor[1] = "icon_star_bronze.gif|5"; //Moderators
                arrColor[2] = "icon_star_green.gif|1";
                arrColor[3] = "icon_star_cyan.gif|2";
                arrColor[4] = "icon_star_blue.gif|3";
                arrColor[5] = "icon_star_purple.gif|4";
                arrColor[6] = "icon_star_silver.gif|5";
                arrColor[7] = "icon_star_orange.gif|6";
                return arrColor;
            }
        }
        // number of posts for each rank level
        public enum RankLevel
        {
            [Description("New Member")]
            New = 0,
            [Description("Starting Member")]
            Starting = 50,
            [Description("Junior Member")]
            Junior = 100,
            [Description("Average Member")]
            Average = 500,
            [Description("Senior Member")]
            Senior = 1000,
            [Description("Advanced Member")]
            Advanced = 2000,
            [Description("Extreme Addict")]
            Extreme = 10000
        }

        private int _Level;
        public string Title { get; set; }
        private readonly int? _Posts;
        private int _RankVal;
        private readonly string _User;
        public string Stars {
            get { return GetStars(); }
        }   
        //private string _path;

        public RankInfo(string username, ref string title, int? posts)
        {
            _User = username;
            _Posts = posts;
            //_path = apppath;
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
            StringBuilder ImageString = new StringBuilder("");

            string[] rank = rankColor[_Level + 1].Split('|');
            int imageRepeat = Int32.Parse(rank[1]);

            string rankImage = rank[0];
            if (Roles.IsUserInRole(_User, "Administrator"))
            {
                rank = rankColor[0].Split('|');
                imageRepeat = Int32.Parse(rank[1]);
                rankImage = rank[0]; //Admin;
            }
            if (Roles.IsUserInRole(_User, "Moderator"))
            {
                rank = rankColor[1].Split('|');
                imageRepeat = Int32.Parse(rank[1]);

                rankImage = rank[0]; //Moderator;
            }
            //if (_path == "/")
            //    _path = "";

            if (rankImage != "")
            {
                for (int ii = 1; ii <= imageRepeat; ii++)
                {
                    string clientpath = VirtualPathUtility.ToAbsolute(Config.ImageDirectory + "ranking/");
                    ImageString.AppendFormat("<img src='{0}{1}' alt=''/>", clientpath, rankImage);
                }
            }
            return ImageString.ToString() + "<br/>";
        }

        private void SetLevel()
        {
            string rankTitle = "";
            _Level = -1;
            foreach (string rank in Enum.GetNames(typeof(RankLevel)))
            {
                if (_Posts >= (int)Enum.Parse(typeof(RankLevel), rank))
                {
                    //rankTitle = rank;

                    _RankVal = (int)Enum.Parse(typeof(RankLevel), rank);
                    RankLevel thisLevel = (RankLevel)_RankVal;
                    rankTitle = GetDescription(thisLevel);
                    _Level++;
                }
                if (_Posts < (int)Enum.Parse(typeof(RankLevel), rank))
                    break;
                if (Roles.IsUserInRole(_User, "Administrator"))
                {
                    rankTitle = "Forum Administrator";
                }
                if (Roles.IsUserInRole(_User, "Moderator"))
                {
                    rankTitle = "Forum Moderator";
                }
            }
            Title = rankTitle;
        }
        
        private static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
