/*
####################################################################################################################
##
## Snitz.BLL - Profile
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
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class CustomProfile
    {
        public static bool AddColumn(ProfileColumn column)
        {
            ICustomProfile dal = Factory<ICustomProfile>.Create("CustomProfile");
            return dal.AddColumn(column);
        }
        public static bool DropColumn(string column)
        {
            ICustomProfile dal = Factory<ICustomProfile>.Create("CustomProfile");
            return dal.DropColumn(column);
        }

        public static List<ProfileColumn> GetColumns()
        {
            ICustomProfile dal = Factory<ICustomProfile>.Create("CustomProfile");
            List<ProfileColumn> cols = dal.GetColumns();
            foreach (ProfileColumn col in cols)
            {
                if (!String.IsNullOrEmpty(col.DefaultValue))
                    col.DefaultValue = col.DefaultValue.Replace("(", "").Replace(")", "");
                if (col.Precision == "-1")
                    col.Precision = "Max";
            }
            return cols;
        }

    }
}
