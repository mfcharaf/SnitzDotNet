
using System.Collections.Generic;
using System.Linq;
using SnitzCommon;


namespace SnitzData
{
    public partial class Category
    {
        public Enumerators.PostStatus Status
        {
            get { return (Enumerators.PostStatus)this.CAT_STATUS; }
            set { this.CAT_STATUS = (short) value; }
        }
        public List<Forum> Forums { get { return GetForums(this.Id); } }

        private static List<Forum> GetForums(int catid)
        {
            SnitzDataClassesDataContext dc = new SnitzDataClassesDataContext();
            var result = from forums in dc.Forums where forums.CatId == catid orderby forums.Order select forums;
            return result.ToList();
        }
    }
}
