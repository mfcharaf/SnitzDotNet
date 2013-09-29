/*
####################################################################################################################
##
## Snitz.BLL - Avatars
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
using System.IO;
using System.Web;

namespace Snitz.BLL
{
    public class Avatar
    {
        public string Path { get; set; }

        public string Name { get; set; }

        public Avatar()
        {

        }

        private Avatar(string filename)
        {
            Name = filename;
            Path = System.IO.Path.Combine("~/Avatars/", filename);
        }

        public static List<Avatar> GetAvatars()
        {
            DirectoryInfo dInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Avatars/"));
            FileInfo[] fArrInfo = dInfo.GetFiles("*.jpg");
            List<Avatar> list = new List<Avatar>();
            foreach (FileInfo sFileName in fArrInfo)
            {
                Avatar temp = new Avatar(sFileName.Name);
                list.Add(temp);
            }
            return list;
        }

        public static bool Delete(string filename)
        {
            DirectoryInfo dInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/Avatars/"));
            FileInfo[] fArrInfo = dInfo.GetFiles(filename);
            fArrInfo[0].Delete();
            return true;
        }
    }

}
