using System.Collections.Generic;
using System.Linq;

namespace SnitzData
{
    partial class LookupsDataContext
    {
        public Dictionary<int,string> GetForumModerators(int forumid)
        {
            return (from fm in this.ForumModerators where fm.ForumId == forumid select fm.Moderators).ToDictionary(d=>d.ModeratorId,d=>d.Name);
        }        
    }
    partial class SnitzDataClassesDataContext
    {
        public List<int> GetAllowedRoles(int forumid)
        {
            return (from role in this.ForumRoles where role.Forum_id == forumid select role.Role_Id).ToList();
        }
        public Category GetCategory(int catid)
        {
            return (from cat in this.Categories where cat.Id == catid select cat).SingleOrDefault();
        }

        public Topic GetTopic(int topicid)
        {
            return (from topic in this.Topics where topic.Id == topicid select topic).SingleOrDefault();
        }

        public Forum GetForum(int forumid)
        {
            return (from forum in this.Forums where forum.Id == forumid select forum).SingleOrDefault();
        }

        public Reply GetReply(int replyid)
        {
            return (from reply in this.Replies where reply.Id == replyid select reply).SingleOrDefault();
        }

        public Member GetAuthor(int? authorId)
        {
            if (authorId == null)
                return null;
            return (from author in this.Members where author.Id == authorId select author).SingleOrDefault();
        }
        public Member GetMember(string username)
        {
            return (from user in this.Members where user.Name == username select user).SingleOrDefault();
        }
    }
}
