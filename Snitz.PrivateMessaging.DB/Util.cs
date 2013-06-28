using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;


namespace PrivateMessaging.Data
{
    public class Util
    {
        public static List<MemberSearch> GetMemberListPaged(int page, string searchfor)
        {
            int skip = page*15;
            using (var dc = new PMDataDataContext())
            {
                var allmembers = (from member in dc.MemberSearch
                 join profile in dc.ProfileDatas on member.Id equals profile.UserId into totalmembers
                 from members in totalmembers.DefaultIfEmpty()
                 where member.Status == 1 && (members.PMReceive ?? 1) !=  0
                 orderby member.Name
                 select member);
                if (!String.IsNullOrEmpty(searchfor))
                {
                    allmembers = searchfor.Length == 1 ? allmembers.Where(m => m.Name.StartsWith(searchfor)) : allmembers.Where(m => m.Name.Contains(searchfor));
                }
                return allmembers.Skip(skip).Take(15).ToList();
            }
        }

        public static List<PrivateMessage> GetMessages(int memberid)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.DeferredLoadingEnabled = false;
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<PrivateMessage>(t => t.ToMembers);
                loadOptions.LoadWith<PrivateMessage>(t => t.FromMembers);
                dc.LoadOptions = loadOptions;
                return (from pm in dc.PrivateMessages where pm.ToMemberId == memberid orderby pm.SentDate descending select pm).ToList();
            }
        }

        public static List<PrivateMessage> GetSentMessages(int memberid)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.DeferredLoadingEnabled = false;
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<PrivateMessage>(t => t.ToMembers);
                loadOptions.LoadWith<PrivateMessage>(t => t.FromMembers);
                dc.LoadOptions = loadOptions;
                return (from pm in dc.PrivateMessages where pm.FromMemberId == memberid && pm.OutBox == 1 orderby pm.SentDate descending select pm).ToList();
            }
        }

        public static void SendPrivateMessage(PrivateMessage pm)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.PrivateMessages.InsertOnSubmit(pm);
                dc.SubmitChanges();
            }
        }

        public static void DeletePrivateMessage(int pmid)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.PrivateMessages.DeleteAllOnSubmit(dc.PrivateMessages.Where(p => p.Id == pmid));
                dc.SubmitChanges();
            }
        }

        public static ProfileData GetPreferences(object userid)
        {
            using (var dc = new PMDataDataContext())
            {
                return (from pref in dc.ProfileDatas where pref.UserId == (int)userid select pref).SingleOrDefault();
            }
        }

        public static void SavePreferences(object userid, string enabled, string notify, string layout)
        {
            using (var dc = new PMDataDataContext())
            {
                var pf = dc.ProfileDatas.SingleOrDefault(pref => pref.UserId == (int)userid);
                if (pf == null)
                {
                    dc.ExecuteCommand("INSERT INTO ProfileData (UserId,PMEmail,PMReceive,PMLayout) Values ({0},{1},{2},{3})", userid, Convert.ToInt16(notify), Convert.ToInt16(enabled),layout);

                }
                else
                {
                    dc.ExecuteCommand("UPDATE ProfileData SET PMEmail = {0},PMReceive={1}, PMLayout = {2} WHERE UserId={3}", Convert.ToInt16(notify), Convert.ToInt16(enabled), layout,userid);

                }

            }
        }

        public static int GetPMCount(object userid)
        {
            using (var dc = new PMDataDataContext())
            {
                var result = (from pm in dc.PrivateMessages where pm.ToMemberId == (int)userid && pm.Read == 0 select pm).ToList();
                return result.Count;
            }
        }

        public static PrivateMessage GetMessage(string pmid)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.DeferredLoadingEnabled = false;
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<PrivateMessage>(t => t.ToMembers);
                loadOptions.LoadWith<PrivateMessage>(t => t.FromMembers);
                dc.LoadOptions = loadOptions;
                var privm = (from pm in dc.PrivateMessages where pm.Id == Convert.ToInt32(pmid) select pm).SingleOrDefault();
                privm.Read = 1;
                dc.SubmitChanges();
                return privm;
            }
        }
        public static PrivateMessage GetSentMessage(string pmid)
        {
            using (var dc = new PMDataDataContext())
            {
                dc.DeferredLoadingEnabled = false;
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<PrivateMessage>(t => t.ToMembers);
                loadOptions.LoadWith<PrivateMessage>(t => t.FromMembers);
                dc.LoadOptions = loadOptions;
                var privm = (from pm in dc.PrivateMessages where pm.Id == Convert.ToInt32(pmid) select pm).SingleOrDefault();
                return privm;
            }
        }

        public static void RemoveFromOutBox(int currentPmId)
        {
            using (var dc = new PMDataDataContext())
            {
                var privm = (from pm in dc.PrivateMessages where pm.Id == currentPmId select pm).SingleOrDefault();
                privm.OutBox = 0;
                dc.SubmitChanges();
            }
        }

        public static int GetMemberCount(string searchfor)
        {
            using (var dc = new PMDataDataContext())
            {
                var allmembers = (from member in dc.MemberSearch
                              join profile in dc.ProfileDatas on member.Id equals profile.UserId into totalmembers
                              from members in totalmembers.DefaultIfEmpty()
                                  where member.Status == 1 && (members.PMReceive ?? 1) != 0
                              select member);
                if (!String.IsNullOrEmpty(searchfor))
                {
                    allmembers = searchfor.Length == 1 ? allmembers.Where(m => m.Name.StartsWith(searchfor)) : allmembers.Where(m => m.Name.Contains(searchfor));
                }
                return allmembers.Count();
            }
        }
    }
    
}