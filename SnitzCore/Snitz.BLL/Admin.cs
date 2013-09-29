/*
####################################################################################################################
##
## Snitz.BLL - Admin
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

using System.Collections.Generic;
using System.Web;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class Admin
    {
        public static IEnumerable<SubscriptionInfo> GetAllSubscriptions()
        {
            ISubscription dal = Factory<ISubscription>.Create("Subscription");
            return dal.GetAllSubscriptions();
        }

        public static bool UpdateForumCounts()
        {
            IAdmin dal = Factory<IAdmin>.Create("AdminFunctions");
            dal.UpdateForumCounts();
            return true;
        }

        public static string GetDBSize()
        {
            IAdmin dal = Factory<IAdmin>.Create("AdminFunctions");
            return dal.GetDbSize().ToString();
        }

        public static string Encrypt(string text)
        {
            return Cryptos.CryptosUtilities.Encrypt(text);
        }

        public static int MemberCount
        {
            get
            {
                IMember dal = Factory<IMember>.Create("Member");
                return dal.GetMemberCount(HttpContext.Current.Session["SearchFilter"]);
            }
        }

        public static string ExecuteScript(string script)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            return dal.ExecuteScript(script);
        }

        public static bool DoesTableExist(string table)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            return dal.TableExists(table);
        }

        public static void CheckDBOwner()
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            dal.ChangeDbOwner();
        }

        public static bool DatabaseExists()
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            return dal.DatabaseExists();
        }

        public static IEnumerable<ForumInfo> PrivateForums()
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            return dal.PrivateForums();
        }

        public static string[] AllowedMembers(int forumid)
        {
            ISetup dal = Factory<ISetup>.Create("SnitzSetup");
            return dal.AllowedMembers(forumid);
        }
    }
}
