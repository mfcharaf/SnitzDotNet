
      
using System;
using System.Data;
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
    }
}
