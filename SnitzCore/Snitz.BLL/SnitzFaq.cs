/*
####################################################################################################################
##
## Snitz.BLL - SnitzFaq
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
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
using System.Linq;
using Snitz.Entities;
using Snitz.IDAL;

namespace Snitz.BLL
{
    public static class SnitzFaq
    {

        public static IEnumerable<FaqCategoryInfo> GetFaqCategories(string lang)
        {
                IFaqCategory dal = Factory<IFaqCategory>.Create("Faq");
                return dal.GetCategories(lang);
        }

        public static IEnumerable<FaqInfo> GetFaqQuestionsByCategory(int catid, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.GetFaqItems(catid, lang);
        }

        public static IEnumerable<FaqInfo> FindFaqQuestion(string searchfor, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.FindFaqItem(searchfor, lang);
        }

        public static Dictionary<int, string> GetQuestions(int catid, string filter, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.GetQuestions(catid, filter, lang);
        }

        public static FaqInfo GetFaqQuestion(int faqid, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.GetFaqItem(faqid, lang);
        }

        public static IEnumerable<FaqInfo> GetQuestion(string path, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.GetByName(path);
        }

        public static int AddFaqCategory(FaqCategoryInfo category)
        {
            IFaqCategory dal = Factory<IFaqCategory>.Create("Faq");
            return dal.Add(category);
        }

        public static int AddFaqQuestion(FaqInfo question)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            return dal.Add(question);
        }

        public static void DeleteFaqCategory(FaqCategoryInfo category)
        {
            IFaqCategory dal = Factory<IFaqCategory>.Create("Faq");
            dal.Delete(category);
        }

        public static void DeleteFaqQuestion(FaqInfo question)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            dal.Delete(question);
        }

        public static void UpdateFaqCategory(FaqCategoryInfo category)
        {
            IFaqCategory dal = Factory<IFaqCategory>.Create("Faq");
            dal.Update(category);
        }

        public static void UpdateFaqQuestion(FaqInfo question)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            dal.Update(question);
        }

        public static void Dispose()
        {
            
        }

        public static void UpdateFaqQuestion(int id, string question, string answer, string lang)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            FaqInfo faq = dal.GetById(id);
            faq.LinkTitle = question;
            faq.LinkBody = answer;
            faq.Language = lang;
            dal.Update(faq);

        }

        public static void DeleteFaqQuestion(int question)
        {
            IFaqQuestion dal = Factory<IFaqQuestion>.Create("Faq");
            dal.Delete(dal.GetById(question));
        }

        public static FaqCategoryInfo GetCategory(string name)
        {
            IFaqCategory dal = Factory<IFaqCategory>.Create("Faq");
            return dal.GetByName(name).FirstOrDefault();
        }
    }
}
