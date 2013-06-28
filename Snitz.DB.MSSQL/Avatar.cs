using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SnitzData
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
