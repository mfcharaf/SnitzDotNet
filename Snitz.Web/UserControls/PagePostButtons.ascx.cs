/*
####################################################################################################################
##
## SnitzUI.UserControls - PagePostButtons.ascx
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
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI.UserControls
{
    public partial class PagePostButtons : UserControl
    {
        private TopicInfo _topic;

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
                _topic = Topics.GetTopic(page.TopicId.Value);
                NewTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}", _topic.ForumId, _topic.CatId);
                ReplyTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=reply&TOPIC={0}", _topic.Id);
                if (_topic.AllowSubscriptions)
                {
                    SubscribeTopic.Visible = page.IsAuthenticated;
                    SubscribeTopic.CommandName = "topicsub";
                    SubscribeTopic.CommandArgument = page.TopicId.Value.ToString();

                    if (Members.IsSubscribedToTopic(page.TopicId.Value, page.Member.Id))
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
                if((_topic.Forum.Status == (int)Enumerators.PostStatus.Closed ||
                    _topic.Status == (int)Enumerators.PostStatus.Closed) && 
                    !(page.IsAdministrator))
                {
                    ReplyTopic.Visible = false;
                    NewTopic.Visible = _topic.Forum.Status != (int)Enumerators.PostStatus.Closed;
                }
                ReplyTopic.Visible = ReplyTopic.Visible && _topic.Status != (int)Enumerators.PostStatus.Closed;
                NewTopic.Visible = _topic.Forum.Status == (int)Enumerators.PostStatus.Open;

                ReplyTopic.Visible = ReplyTopic.Visible || Members.IsTopicAuthor(page.Member.Id,_topic.Id);

                NewTopic.Visible = NewTopic.Visible && page.IsAuthenticated;
                ReplyTopic.Visible = ReplyTopic.Visible && page.IsAuthenticated;

            }else
            {
                if(page.ForumId.HasValue)
                {
                    ForumInfo forum = Forums.GetForum(page.ForumId.Value);
                    NewTopic.PostBackUrl = string.Format("~/Content/Forums/post.aspx?method=topic&FORUM={0}&CAT={1}", forum.Id, forum.CatId);
                    NewTopic.Visible = page.IsAuthenticated;
                    NewTopic.Visible = NewTopic.Visible && forum.Status != (int)Enumerators.PostStatus.Closed || (page.IsAdministrator || ((ForumPage)Page).IsForumModerator);
                    if (forum.SubscriptionLevel == (int)Enumerators.Subscription.ForumSubscription)
                    {
                        SubscribeTopic.Visible = page.IsAuthenticated;
                        SubscribeTopic.CommandName = "forumsub";
                        SubscribeTopic.CommandArgument = page.ForumId.Value.ToString();

                        if (Members.IsSubscribedToForum(page.Member.Id,page.ForumId.Value))
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
                    Subscriptions.AddTopicSubscription(page.Member.Id, id);
                    break;
                case "topicunsub":
                    Subscriptions.RemoveTopicSubscription(page.Member.Id, id);
                    break;
                case "forumsub" :
                    Subscriptions.AddForumSubscription(page.Member.Id,id);
                    break;
                case "forumunsub" :
                    Subscriptions.RemoveForumSubscription(page.Member.Id,id);
                    break;
            }

        }
    }
}