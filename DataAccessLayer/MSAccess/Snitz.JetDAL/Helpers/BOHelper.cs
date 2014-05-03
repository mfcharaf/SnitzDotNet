using System;
using System.Data.OleDb;
using Snitz.Entities;
using SnitzCommon;

namespace Snitz.OLEDbDAL.Helpers
{
    /// <summary>
    /// Helper methods to copy datareader to entity objects
    /// </summary>
    internal static class BoHelper
    {
        public static CategoryInfo CopyCategoryToBO(OleDbDataReader rdr)
        {
            //CAT_ID, CAT_NAME, CAT_STATUS, CAT_MODERATION, CAT_SUBSCRIPTION, CAT_ORDER
            CategoryInfo category = new CategoryInfo
            {
                Id = rdr.GetInt32(0),
                Name = rdr.SafeGetString(1),
                Status = rdr.GetInt16(2),
                ModerationLevel = rdr.SafeGetInt32(3),
                SubscriptionLevel = rdr.SafeGetInt32(4),
                Order = rdr.GetInt32(5)
            };
            return category;
        }

        public static ForumInfo CopyForumToBO(OleDbDataReader rdr)
        {
            //"F.FORUM_ID,F.CAT_ID,F.F_STATUS,F.F_SUBJECT,F.F_URL,F.F_TOPICS" +
            //",F.F_COUNT,F.F_LAST_POST,F.F_PRIVATEFORUMS,F.F_TYPE,F.F_LAST_POST_AUTHOR,F.F_A_TOPICS,F.F_A_COUNT,F.F_MODERATION" +
            //",F.F_SUBSCRIPTION,F.F_ORDER, F.F_COUNT_M_POSTS,F.F_LAST_POST_TOPIC_ID,F.F_LAST_POST_REPLY_ID,F.F_POLLS,F.F_DESCRIPTION" +
            //",F.F_L_ARCHIVE,F.F_ARCHIVE_SCHED,T.T_SUBJECT,M.M_NAME ";
            ForumInfo forum = new ForumInfo
            {
                Id = rdr.GetInt32(0),
                CatId = rdr.GetInt32(1),
                Status = rdr.GetInt16(2),
                Subject = rdr.SafeGetString(3),
                Url = rdr.SafeGetString(4),
                TopicCount = rdr.GetInt32(5),
                PostCount = rdr.GetInt32(6),
                LastPostDate = rdr.GetSnitzDate(7),
                AuthType = rdr.SafeGetInt32(8),
                Type = rdr.GetInt16(9),
                LastPostAuthorId = rdr.SafeGetInt32(10),
                ArchivedTopicCount = rdr.SafeGetInt32(11),
                ArchivedPostCount = rdr.SafeGetInt32(12),
                ModerationLevel = rdr.SafeGetInt32(13),
                SubscriptionLevel = rdr.SafeGetInt32(14),
                Order = rdr.GetInt32(15),
                UpdatePostCount = rdr.GetInt16(16) == 1,
                LastPostTopicId = rdr.SafeGetInt32(17),
                LastPostReplyId = rdr.SafeGetInt32(18),
                AllowPolls = rdr.GetInt32(19) == 1,
                Description = rdr.SafeGetString(20),
                LastArchived = rdr.GetSnitzDate(21),
                ArchiveFrequency = rdr.SafeGetInt32(22),
                LastPostSubject = rdr.SafeGetString(23),
                LastPostAuthorName = rdr.SafeGetString(24)
            };

            return forum;
        }

        public static MemberInfo CopyMemberToBO(OleDbDataReader rdr)
        {
            MemberInfo member = null;
            try
            {
                member = new MemberInfo();

                member.Id = rdr.GetInt32(0);
                member.Status = rdr.GetInt16(1);
                member.Username = rdr.GetString(2);
                member.NTUsername = rdr.SafeGetString(3);
                member.Email = rdr.SafeGetString(4);
                member.Country = rdr.SafeGetString(5);
                member.HomePage = rdr.SafeGetString(6);
                member.Signature = rdr.SafeGetString(7);
                member.MemberLevel = rdr.GetInt16(8);
                member.AIM = rdr.SafeGetString(9);
                member.Yahoo = rdr.SafeGetString(10);
                member.ICQ = rdr.SafeGetString(11);
                member.MSN = rdr.SafeGetString(12);
                member.PostCount = rdr.SafeGetInt32(13) ?? 0;
                member.MemberSince = rdr.GetSnitzDate(14) == null ? DateTime.UtcNow : rdr.GetSnitzDate(14).Value;
                member.LastVisitDate = rdr.GetSnitzDate(15);
                member.LastPostDate = rdr.GetSnitzDate(16);
                member.Title = rdr.SafeGetString(17); //member.MemberTitle(rdr.SafeGetString(17));
                member.AllowSubscriptions = rdr.SafeGetInt16(18) == 1;
                member.HideEmail = rdr.SafeGetInt16(19) == 1;
                member.ReceiveEmails = rdr.SafeGetInt16(20) == 1;
                member.MembersIP = rdr.SafeGetString(21);
                member.ViewSignatures = rdr.SafeGetInt16(22) == 1;
                member.UseSignature = rdr.SafeGetInt16(23) == 1;
                member.Voted = rdr.SafeGetInt32(24) == 1;
                member.AllowEmail = rdr.SafeGetInt16(25) == 1;
                member.Avatar = rdr.SafeGetString(26);
                member.Theme = rdr.SafeGetString(27);
                member.TimeOffset = rdr.SafeGetInt32(28) == null ? 0 : rdr.SafeGetInt32(28).Value;
                member.DateOfBirth = rdr.SafeGetString(29);
                member.Age = rdr.SafeGetString(30);
                member.Password = rdr.GetString(31);
                member.ValidationKey = rdr.SafeGetString(32);
                member.IsValid = rdr.SafeGetInt32(33) == 1;
                member.LastUpdateDate = rdr.GetSnitzDate(34);
                member.MaritalStatus = rdr.SafeGetString(35);
                member.Firstname = rdr.SafeGetString(36);
                member.Lastname = rdr.SafeGetString(37);
                member.Occupation = rdr.SafeGetString(38);
                member.Gender = rdr.SafeGetString(39);
                member.Hobbies = rdr.SafeGetString(40);
                member.LatestNews = rdr.SafeGetString(41);
                member.FavouriteQuote = rdr.SafeGetString(42);
                member.Biography = rdr.SafeGetString(43);
                member.FavLink1 = rdr.SafeGetString(44);
                member.FavLink2 = rdr.SafeGetString(45);
                member.City = rdr.SafeGetString(46);
                member.State = rdr.SafeGetString(47);


            }
            catch (Exception ex)
            {
                throw new Exception("Error copying reader to BO");
                OleDbDataReader rdrtest = rdr;
                string test = ex.Message;
            }

            string title = member.Title;
            member.Rank = new RankInfo(member.Username, ref title, member.PostCount);
            member.Title = title;
            return member;
        }

        public static TopicInfo CopyTopicToBO(OleDbDataReader rdr)
        {
            TopicInfo topic;
            try
            {
                topic = new TopicInfo
                {
                    Id = rdr.GetInt32(0),
                    CatId = rdr.GetInt32(1),
                    ForumId = rdr.GetInt32(2),
                    Status = rdr.GetInt16(3),
                    Subject = rdr.SafeGetString(4),
                    AuthorId = rdr.GetInt32(5),
                    ReplyCount = rdr.GetInt32(6),
                    Views = rdr.GetInt32(7),
                    LastPostDate = rdr.GetSnitzDate(8),
                    Date = rdr.GetSnitzDate(9).Value,
                    PosterIp = rdr.SafeGetString(10),
                    LastPostAuthorId = rdr.SafeGetInt32(11),
                    IsSticky = rdr.GetInt16(12) == 1,
                    LastEditDate = rdr.GetSnitzDate(13),
                    LastEditedById = rdr.SafeGetInt32(14),
                    UseSignatures = rdr.GetInt16(15) == 1,
                    LastReplyId = rdr.SafeGetInt32(16),
                    UnModeratedReplies = rdr.GetInt32(17),
                    Message = rdr.SafeGetString(18)

                };

                if (rdr.FieldCount > 19)
                {
                    topic.AuthorName = rdr.SafeGetString(19);
                    topic.LastPostAuthorName = rdr.SafeGetString(20);

                    topic.EditorName = rdr.SafeGetString(21);
                    if (rdr.FieldCount > 22)
                    {
                        topic.AuthorViewSig = rdr.GetInt16(22) == 1;
                        topic.AuthorSignature = rdr.SafeGetString(23);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return topic;
        }

        public static ReplyInfo CopyReplyToBO(OleDbDataReader rdr)
        {

            ReplyInfo reply = new ReplyInfo
                                  {
                                      Id = rdr.GetInt32(0),
                                      CatId = rdr.GetInt32(1),
                                      ForumId = rdr.GetInt32(2),
                                      TopicId = rdr.GetInt32(3),
                                      AuthorId = rdr.GetInt32(4),
                                      Date = rdr.GetSnitzDate(5).Value,
                                      PosterIp = rdr.SafeGetString(6),
                                      Status = rdr.GetInt16(7),
                                      LastEditDate = rdr.GetSnitzDate(8),
                                      LastEditedById = rdr.SafeGetInt32(9),
                                      UseSignatures = rdr.GetInt16(10) == 1,
                                      Message = rdr.SafeGetString(11)
                                  };

            if (rdr.FieldCount > 12)
            {
                reply.AuthorName = rdr.SafeGetString(12);
                reply.EditorName = rdr.SafeGetString(13);
                reply.AuthorViewSig = rdr.GetInt16(14) == 1;
                reply.AuthorSignature = rdr.SafeGetString(15).Replace(">", "]").Replace("<", "[");
            }
            return reply;
        }

        public static PrivateMessageInfo CopyPrivateMessageToBO(OleDbDataReader rdr)
        {
            PrivateMessageInfo pm = new PrivateMessageInfo
            {
                Id = rdr.GetInt32(0),
                Subject = rdr.SafeGetString(1),
                FromMemberId = rdr.GetInt32(2),
                ToMemberId = rdr.GetInt32(3),
                SentDate = rdr.SafeGetString(4),
                Message = rdr.SafeGetString(5),
                Count = rdr.SafeGetString(6),
                Read = rdr.GetInt32(7),
                //Mail = Convert.ToInt32(rdr.SafeGetString(8)),
                OutBox = rdr.GetInt32(9),
                ToMemberName = rdr.SafeGetString(10),
                FromMemberName = rdr.SafeGetString(11)
            };
            pm.Sent = pm.SentDate.ToDateTime();
            return pm;
        }

        public static SearchResult CopySearchResultToBO(OleDbDataReader rdr, ref int rowcount)
        {
            SearchResult result = new SearchResult
            {
                CategoryStatus = rdr.GetInt16(0),
                CategorySubscriptionLevel = rdr.GetInt32(1),
                CategoryTitle = rdr.SafeGetString(2),
                ForumSubject = rdr.SafeGetString(3),
                ForumSubscriptionLevel = rdr.GetInt32(4),
                ForumStatus = rdr.GetInt16(5),
                ForumAccessType = rdr.GetInt32(6),
                ForumPassword = rdr.SafeGetString(7),
                CatId = rdr.GetInt32(8),
                ForumId = rdr.GetInt32(9),
                Id = rdr.GetInt32(10),
                AuthorId = rdr.GetInt32(11),
                Subject = rdr.SafeGetString(12),
                Status = rdr.GetInt16(13),
                LastPostDate = rdr.GetSnitzDate(14),
                LastPostAuthorId = rdr.SafeGetInt32(15),
                LastReplyId = rdr.SafeGetInt32(16),
                ReplyCount = rdr.GetInt32(17),
                UnModeratedReplies = rdr.GetInt32(18),
                Views = rdr.GetInt32(19),
                AuthorName = rdr.SafeGetString(21),
                LastPostAuthorName = rdr.SafeGetString(22)
            };
            //rowcount = rdr.GetInt32(24);
            return result;
        }

        public static FaqInfo CopyFaqQuestionsToBO(OleDbDataReader rdr)
        {
            FaqInfo faqquestion = new FaqInfo
                {
                    Id = rdr.GetInt32(0),
                    Link = rdr.SafeGetString(1),
                    LinkTitle = rdr.SafeGetString(2),
                    LinkBody = rdr.SafeGetString(3),
                    CatId = rdr.GetInt32(4),
                    Language = rdr.SafeGetString(5),
                    Order = rdr.GetInt32(6)
                };
            return faqquestion;
        }
    }
}
