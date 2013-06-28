using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace SnitzData
{
    public class FaqUtil
    {
        private SnitzFaqDataDataContext dc;

        public FaqUtil()
        {
            dc = new SnitzFaqDataDataContext();
        }

        public List<FaqCategory> GetCategories(string lang)
        {
            return (from fc in dc.FaqCategories where fc.Language == lang orderby fc.Order select fc).ToList();
        }

        public Dictionary<int,string> GetFaqQuestions(int catid, string filter, string lang)
        {
            var res =
                (from fi in dc.FaqInfos
                 where fi.CatId == catid && fi.Language == lang
                 select fi);
            if (!String.IsNullOrEmpty(filter))
                res = res.Where(fi => (fi.Question + fi.Answer).Contains(filter));
            return res.OrderBy(fi => fi.Order).ToDictionary(d => d.Id, d => d.Question);
        }

        //public List<FaqInfo> GetFaqInfo(int catid, string lang)
        //{
        //    return (from fi in dc.FaqInfos where fi.CatId == catid && fi.Language == lang select fi).ToList();
        //}

        public FaqInfo GetAnswer(int faqid, string  lang)
        {
            return (from fa in dc.FaqInfos where fa.Id == faqid && fa.Language == lang select fa).SingleOrDefault();
        }

        public void SaveFaq(int faqid, string question, string answer, string lang)
        {
            FaqInfo faq = (from fa in dc.FaqInfos where fa.Id == faqid && fa.Language == lang select fa).SingleOrDefault();
            faq.Question = question;
            faq.Answer = answer;

            dc.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        //public List<FaqInfo> FindFaq(string searchfor, string lang)
        //{
        //    return (from fi in dc.FaqInfos where fi.Language == lang && (fi.Question + fi.Answer).Contains(searchfor) select fi).ToList();
        //}

        public void Delete(int question)
        {
            dc.FaqInfos.DeleteAllOnSubmit(dc.FaqInfos.Where(f=>f.Id==question));
            dc.SubmitChanges();
        }

        public void AddFaq(FaqInfo faq)
        {
            dc.FaqInfos.InsertOnSubmit(faq);
            dc.SubmitChanges();
        }
    }
}
