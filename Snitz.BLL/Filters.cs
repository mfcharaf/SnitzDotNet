/*
####################################################################################################################
##
## Snitz.BLL - Filters
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Snitz.Entities;
using Snitz.IDAL;
using SnitzConfig;

namespace Snitz.BLL
{
    public static class Filters
    {

        #region Badword Filter

        public static BadwordInfo GetBadword(int id)
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            return dal.GetById(id);
        }

        public static BadwordInfo GetBadword(string name)
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            return dal.GetByName(name).FirstOrDefault();
        }

        public static List<BadwordInfo> GetAllBadwords()
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            return new List<BadwordInfo>(dal.GetAll());
        }

        public static int AddBadword(string badword, string replace)
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            BadwordInfo bw = new BadwordInfo();
            bw.Badword = badword;
            bw.Replace = replace;
            return dal.Add(bw);
        }

        public static void DeleteBadword(int Id)
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            BadwordInfo badword = dal.GetById(Id);
            dal.Delete(badword);
        }

        public static void DeleteAllBadwords()
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            dal.DeleteAll();
        }

        public static void UpdateBadword(int Id, string badword, string replace)
        {
            IBadWordFilter dal = Factory<IBadWordFilter>.Create("BadWordFilter");
            BadwordInfo bw = dal.GetById(Id);
            bw.Badword = badword;
            bw.Replace = replace;

            dal.Update(bw);
        }

        public static string ReplaceBadWords(this string text)
        {
            const RegexOptions matchOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

            if (!Config.FilterBadWords || String.IsNullOrEmpty(text))
                return text;

            List<BadwordInfo> bWords = SnitzCachedLists.GetCachedBadWordList();


            string[] ReplaceText = new string[bWords.Count];
            string[] BadWords = new string[bWords.Count];

            int ii = 0;
            foreach (BadwordInfo dr in bWords)
            {
                string bw = dr.Badword;
                BadWords[ii] = Regex.Escape(bw);
                ReplaceText[ii] = dr.Replace;
                ++ii;
            }
            string strBadWords = String.Join("|", BadWords);
            try
            {
                Regex regexObj = new Regex(@"\b" + strBadWords, matchOptions);
                Match matchObj = regexObj.Match(text);
                while (matchObj.Success)
                {
                    int pos = Array.IndexOf(BadWords, Regex.Escape(matchObj.Value));
                    string rText = ReplaceText[pos];
                    text = Regex.Replace(text, matchObj.Value, rText, matchOptions);
                    matchObj = matchObj.NextMatch();
                }
            }
            catch
            {
                // Most likely cause is a syntax error in the regular expression
                // throw (ex);
                return text;
            }

            return text;
        }

        #endregion

        #region Name Filter

        public static NameFilterInfo GetNameFilter(int filterid)
        {
            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            return dal.GetById(filterid);
        }

        public static NameFilterInfo GetNameFilter(string name)
        {
            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            return dal.GetByName(name).FirstOrDefault();
        }

        public static List<NameFilterInfo> GetAllNameFilters() //Cache
        {
            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            return dal.GetAll() as List<NameFilterInfo>;
        }

        public static int AddNameFilter(string name)
        {
            NameFilterInfo namefilter = new NameFilterInfo();
            namefilter.Name = name;
            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            return dal.Add(namefilter);
        }

        public static void DeleteNameFilter(int Id)
        {

            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            NameFilterInfo namefilter = dal.GetById(Id);
            dal.Delete(namefilter);
        }

        public static void UpdateNameFilter(int Id, string name)
        {
            INameFilter dal = Factory<INameFilter>.Create("NameFilter");
            NameFilterInfo namefilter = dal.GetById(Id);
            namefilter.Name = name;
            dal.Update(namefilter);
        }

        #endregion
    }
}
