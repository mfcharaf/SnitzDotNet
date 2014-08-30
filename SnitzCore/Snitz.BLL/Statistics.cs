/*
####################################################################################################################
##
## Snitz.BLL - Statistics
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		01/08/2013
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

using System.Web;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Statistics
    {
        public static StatsInfo GetStatistics()
        {
            if (HttpContext.Current.Session["_LastVisit"] != null)
            {
                IForumStats dal = Factory<IForumStats>.Create("ForumStats");
                return dal.GetStatistics(HttpContext.Current.Session["_LastVisit"].ToString());
                
            }
            else
            {
                return null;
            }
        }

        public static int FileDownloaded(string file)
        {
            IForumStats dal = Factory<IForumStats>.Create("ForumStats");
            return dal.LogDownload(file);
        }
    }
}
