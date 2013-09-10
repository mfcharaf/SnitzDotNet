using System;
using System.Data.SqlClient;
using Snitz.Entities;
using SnitzCommon;

namespace Snitz.SQLServerDAL
{
    /// <summary>
    /// Helper methods to copy datareader to entity objects
    /// </summary>
    public static class BOHelper
    {
        public static CategoryInfo CopyCategoryToBO(SqlDataReader rdr)
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

        public static ForumInfo CopyForumToBO(SqlDataReader rdr)
        {
            ForumInfo forum = new ForumInfo
            {
                Id = rdr.GetInt32(0),
                CatId = rdr.GetInt32(1),
                Status = rdr.GetInt16(2),
                Subject = rdr.SafeGetString(3),
                Url = rdr.SafeGetString(4),
                TopicCount = rdr.GetInt32(5),
                PostCount = rdr.GetInt32(6),
                LastPostDate = rdr.SafeGetString(7).ToDateTime(),
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
                LastPostSubject = rdr.SafeGetString(21),
                LastPostAuthorName = rdr.SafeGetString(22)
            };

            //if (forum.LastPostAuthorId != null)
            //{
            //    var author = new PostAuthor().GetAuthor(forum.LastPostAuthorId.Value);
            //    forum.LastPostAuthor = author;
            //}
            return forum;
        }

        public static MemberInfo CopyMemberToBO(SqlDataReader rdr)
        {
            MemberInfo member = null;
            try
            {
                member = new MemberInfo
                                        {
                                            Id = rdr.GetInt32(0),
                                            Status = rdr.GetInt16(1),
                                            Username = rdr.GetString(2),
                                            NTUsername = rdr.SafeGetString(3),
                                            Email = rdr.SafeGetString(4),
                                            Country = rdr.SafeGetString(5),
                                            HomePage = rdr.SafeGetString(6),
                                            Signature = rdr.SafeGetString(7),
                                            MemberLevel = rdr.GetInt16(8),
                                            AIM = rdr.SafeGetString(9),
                                            Yahoo = rdr.SafeGetString(10),
                                            ICQ = rdr.SafeGetString(11),
                                            MSN = rdr.SafeGetString(12),
                                            PostCount = rdr.SafeGetInt32(13) ?? 0,
                                            MemberSince = rdr.SafeGetString(14).ToDateTime().Value,
                                            LastVisitDate = rdr.SafeGetString(15).ToDateTime(),
                                            LastPostDate = rdr.SafeGetString(16).ToDateTime(),
                                            Title = rdr.SafeGetString(17), //MemberTitle(rdr.SafeGetString(17)),
                                            AllowSubscriptions = (rdr["M_SUBSCRIPTION"] as int?) == 1,
                                            HideEmail = (rdr["M_HIDE_EMAIL"] as int?) == 1,
                                            ReceiveEmails = (rdr["M_RECEIVE_EMAIL"] as int?) == 1,
                                            MembersIP = rdr.SafeGetString(21),
                                            ViewSignatures = (rdr["M_VIEW_SIG"] as int?) == 1,
                                            UseSignature = (rdr["M_SIG_DEFAULT"] as int?) == 1,
                                            Voted = (rdr["M_VOTED"] as int?) == 1,
                                            AllowEmail = (rdr["M_ALLOWEMAIL"] as int?) == 1,
                                            Avatar = rdr.SafeGetString(26),
                                            Theme = rdr.SafeGetString(27),
                                            TimeOffset = rdr.GetInt32(28),
                                            DateOfBirth = rdr.SafeGetString(29)
                                        };
            }
            catch(Exception ex)
            {
                SqlDataReader rdrtest = rdr;
                string test = ex.Message;
            }

            string title = member.Title;
            member.Rank = new RankInfo(member.Username,ref title,member.PostCount);
            member.Title = title;
            return member;
        }

        public static TopicInfo CopyTopicToBO(SqlDataReader rdr)
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
                LastPostDate = rdr.SafeGetString(8).ToDateTime(),
                Date = rdr.SafeGetString(9).ToDateTime().Value,
                PosterIp = rdr.SafeGetString(10),
                LastPostAuthorId = rdr.SafeGetInt32(11),
                IsSticky = rdr.GetInt16(12) == 1,
                LastEditDate = rdr.SafeGetString(13).ToDateTime(),
                LastEditedById = rdr.SafeGetInt32(14),
                UseSignatures = rdr.GetInt16(15) == 1,
                LastReplyId = rdr.SafeGetInt32(16),
                UnModeratedReplies = rdr.GetInt32(17),
                Message = rdr.SafeGetString(18)

            };
                if (rdr.FieldCount > 19)
                {
                    topic.LastPostAuthorName = rdr.SafeGetString(19);
                    topic.AuthorName = rdr.SafeGetString(20);
                    topic.EditorName = rdr.SafeGetString(21);
                    if (rdr.FieldCount > 22)
                    {
                        topic.AuthorViewSig = rdr.GetInt16(22) == 1;
                        topic.AuthorSignature = rdr.SafeGetString(23);      
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }
            
            return topic;
        }

        public static ReplyInfo CopyReplyToBO(SqlDataReader rdr)
        {

            ReplyInfo reply = new ReplyInfo
                                  {
                                      Id = rdr.GetInt32(0),
                                      CatId = rdr.GetInt32(1),
                                      ForumId = rdr.GetInt32(2),
                                      TopicId = rdr.GetInt32(3),
                                      AuthorId = rdr.GetInt32(4),
                                      Date = rdr.GetString(5).ToDateTime().Value,
                                      PosterIp = rdr.SafeGetString(6),
                                      Status = rdr.GetInt16(7),
                                      LastEditDate = rdr.SafeGetString(8).ToDateTime(),
                                      LastEditedById = rdr.SafeGetInt32(9),
                                      UseSignatures = rdr.GetInt16(10) == 1,
                                      Message = rdr.SafeGetString(11)
                                  };
            if (rdr.FieldCount > 12)
            {
                reply.AuthorName = rdr.SafeGetString(12);
                reply.EditorName = rdr.SafeGetString(13);
                reply.AuthorViewSig = rdr.GetInt16(14) == 1;
                reply.AuthorSignature = rdr.SafeGetString(15);
            }
            return reply;
        }

        public static PrivateMessageInfo CopyPrivateMessageToBO(SqlDataReader rdr)
        {
            //M_ID,M_SUBJECT,M_FROM,M_TO,M_SENT,M_MESSAGE,M_PMCOUNT,M_READ,M_MAIL,M_OUTBOX

            PrivateMessageInfo pm = new PrivateMessageInfo()
                                        {
                                            Id = rdr.GetInt32(0),
                                            Subject = rdr.SafeGetString(1),
                                            FromMemberId = rdr.GetInt32(2),
                                            ToMemberId = rdr.GetInt32(3),
                                            SentDate = rdr.SafeGetString(4),
                                            Message = rdr.SafeGetString(5),
                                            Count = rdr.SafeGetString(6),
                                            Read = rdr.GetInt32(7),
                                            Mail = rdr.SafeGetString(8),
                                            OutBox = rdr.GetInt32(9),
                                            ToMemberName = rdr.SafeGetString(10),
                                            FromMemberName = rdr.SafeGetString(11)
                                        };
            pm.Sent = pm.SentDate.ToDateTime();
            return pm;
        }

        public static SearchResult CopySearchResultToBO(SqlDataReader rdr, ref int rowcount)
        {
            SearchResult result = new SearchResult()
                                      {
                                          CategoryStatus = rdr.GetInt16(1),
                                          CategorySubscriptionLevel = rdr.GetInt32(2),
                                          CategoryTitle = rdr.SafeGetString(3),
                                          ForumSubject = rdr.SafeGetString(4),
                                          ForumSubscriptionLevel = rdr.GetInt32(5),
                                          ForumStatus = rdr.GetInt16(6),
                                          ForumAccessType = rdr.GetInt32(7),
                                          ForumPassword = rdr.SafeGetString(8),
                                          CatId = rdr.GetInt32(9),
                                          ForumId = rdr.GetInt32(10),
                                          Id = rdr.GetInt32(11),
                                          AuthorId = rdr.GetInt32(12),
                                          Subject = rdr.SafeGetString(13),
                                          Status = rdr.GetInt16(14),
                                          LastPostDate = rdr.GetSnitzDate(15),
                                          LastPostAuthorId = rdr.SafeGetInt32(16),
                                          LastReplyId = rdr.SafeGetInt32(17),
                                          ReplyCount = rdr.GetInt32(18),
                                          UnModeratedReplies = rdr.GetInt32(19),
                                          Views = rdr.GetInt32(20),
                                          AuthorName = rdr.SafeGetString(22),
                                          LastPostAuthorName = rdr.SafeGetString(23)
                                      };
            rowcount = rdr.GetInt32(24);
            return result;
        }

        public static FaqInfo CopyFaqQuestionsToBO(SqlDataReader rdr)
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
