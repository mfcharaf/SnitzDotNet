using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzCommon;
using SnitzConfig;
using SnitzData;

namespace SnitzUI.UserControls
{
    public partial class PagePostButtons : UserControl
    {
        private Topic _topic;

        protected void Page_Load(object sender, EventArgs e)
        {
            SendTopic.Visible = false;
            PrintTopic.Visible = false;
            NewTopic.Visible = false;
            ReplyTopic.Visible = false;
            SubscribeTopic.Visible = false;
            UnSubscribeTopic.Visible = false;
            
            var page = (PageBase) this.Page;
            if(page.TopicId.HasValue)
            {
                _topic = Util.GetTopic(page.TopicId.Value);
                NewTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}", _topic.ForumId, _topic.CatId);
                ReplyTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=reply&TOPIC={0}", _topic.Id);
                if (_topic.AllowSubscriptions)
                {
                    SubscribeTopic.Visible = page.IsAuthenticated;
                    SubscribeTopic.CommandName = "topicsub";
                    SubscribeTopic.CommandArgument = page.TopicId.Value.ToString();

                    if (page.Member.IsSubscribedToTopic(page.TopicId.Value))
                    {
                        UnSubscribeTopic.CommandName = "topicunsub";
                        UnSubscribeTopic.CommandArgument = page.TopicId.Value.ToString();
                        UnSubscribeTopic.Visible = page.IsAuthenticated;
                        SubscribeTopic.Visible = false;
                    }
                }
                SendTopic.Visible = page.IsAuthenticated && Config.SendTopic;
                PrintTopic.Visible = page.IsAuthenticated && Config.PrintTopic;
                SendTopic.Attributes.Add("onclick",
                                     string.Format(
                                         "mainScreen.LoadServerControlHtml('Send Topic',{{'pageID':5,'data':{0}}},'methodHandlers.BeginRecieve');return false;",
                                         _topic.Id));
                PrintTopic.Attributes.Add("onclick",string.Format("javascript:PrintThisPage({0});return false;", _topic.Id));
                if((_topic.Forum.Status == Enumerators.PostStatus.Closed || 
                    _topic.Status == Enumerators.PostStatus.Closed) && 
                    !(page.IsAdministrator))
                {
                    ReplyTopic.Visible = false;
                    NewTopic.Visible = _topic.Forum.Status != Enumerators.PostStatus.Closed;
                }
                ReplyTopic.Visible = ReplyTopic.Visible && _topic.Status != Enumerators.PostStatus.Closed;
                NewTopic.Visible = _topic.Forum.Status == Enumerators.PostStatus.Open;

                ReplyTopic.Visible = ReplyTopic.Visible || page.Member.IsTopicAuthor(_topic);

                NewTopic.Visible = NewTopic.Visible && page.IsAuthenticated;
                ReplyTopic.Visible = ReplyTopic.Visible && page.IsAuthenticated;

            }else
            {
                if(page.ForumId.HasValue)
                {
                    Forum forum = Util.GetForum(page.ForumId.Value);
                    NewTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}", forum.Id, forum.CatId);
                    NewTopic.Visible = page.IsAuthenticated;
                    NewTopic.Visible = NewTopic.Visible && forum.Status != Enumerators.PostStatus.Closed || (page.IsAdministrator || ((ForumPage)Page).IsForumModerator);
                    if (forum.SubscriptionLevel == (int)Enumerators.Subscription.ForumSubscription)
                    {
                        SubscribeTopic.Visible = page.IsAuthenticated;
                        SubscribeTopic.CommandName = "forumsub";
                        SubscribeTopic.CommandArgument = page.ForumId.Value.ToString();

                        if (page.Member.IsSubscribedToForum(page.ForumId.Value))
                        {
                            UnSubscribeTopic.CommandName = "forumunsub";
                            UnSubscribeTopic.CommandArgument = page.ForumId.Value.ToString();
                            UnSubscribeTopic.Visible = page.IsAuthenticated;
                            SubscribeTopic.Visible = false;
                        }
                    }
                }

            }
        }

        protected void TopicSubscribe(object sender, EventArgs eventArgs)
        {
            var btn = (ImageButton)sender;
            var page = (PageBase)this.Page;
            int id = Convert.ToInt32(btn.CommandArgument);
            switch (btn.CommandName)
            {
                case "topicsub":
                    Util.AddTopicSubscription(page.Member.Id, id);
                    break;
                case "topicunsub":
                    Util.RemoveTopicSubscription(page.Member.Id, id);
                    break;
                case "forumsub" :
                    Util.AddForumSubscription(page.Member.Id,id);
                    break;
                case "forumunsub" :
                    Util.RemoveForumSubscription(page.Member.Id,id);
                    break;
            }

        }
    }
}