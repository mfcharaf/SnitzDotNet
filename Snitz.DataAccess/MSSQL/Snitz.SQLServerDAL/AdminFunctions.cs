
      
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Snitz.Entities;
using Snitz.IDAL;
using Snitz.SQLServerDAL.Helpers;

namespace Snitz.SQLServerDAL
{
    public class AdminFunctions : IAdmin
    {
        public int GetDbSize()
        {
            int size;

            try
            {
                size = Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.StoredProcedure, "sp_spaceused", null));
            }
            catch (Exception)
            {
                size = 0;
            } 

            return size;
        }

        public bool UpdateForumCounts()
        {
            var res = SqlHelper.ExecuteScalar(SqlHelper.ConnString, CommandType.StoredProcedure, "snitz_updateCounts", null);
            return true;
        }


        public int GetMemberCount()
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.GetMemberCount(null);
        }

        public Dictionary<int, Ranking> GetRankings()
        {
            Dictionary<int, Ranking> rankings = new Dictionary<int, Ranking>();
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT RANK_ID,R_TITLE,R_IMAGE,R_POSTS,R_IMG_REPEAT FROM FORUM_RANKING ORDER BY RANK_ID");
            using (var rdr = SqlHelper.ExecuteReader(SqlHelper.ConnString, CommandType.Text, sql.ToString(), null))
            {
                while (rdr.Read())
                {
                    Ranking ranking = new Ranking
                                      {
                                          Title = rdr.SafeGetString(1),
                                          Image = rdr.SafeGetString(2),
                                          RankLevel = rdr.GetInt32(3),
                                          Repeat = rdr.GetInt32(4)
                                      };
                    rankings.Add(rdr.GetInt32(0),ranking);
                }
            }
            return rankings;
        }
    }
}
