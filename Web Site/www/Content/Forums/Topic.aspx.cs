/*
####################################################################################################################
##
## SnitzUI.Content.Forums - Topic.aspx
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
using System.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.BLL.modconfig;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzUI.UserControls.Post_Templates;


namespace SnitzUI
{
    public partial class TopicPage : PageBase, IRoutablePage, ISiteMapResolver
    {
        private TopicInfo _topic;
        private ForumInfo _forum;
        protected int ArchiveView;
        protected internal GridPager ReplyPager;
        public RoutingHelper Routing { get; set; }
        private int RowCount
        {
            get
            {
                return (int)ViewState["RowCount"];
            }
            set
            {
                ViewState["RowCount"] = value;
            }
        }

        public delegate void PopulateObject(int myInt);
        
        private void PopulateData(int currentPage)
        {
            FetchData(currentPage);
        }

        private void FetchData(int pageIndex)
        {
            if (replyFilter.SelectedIndex > -1)
            {
                var topics = new List<TopicInfo> {_topic};
                TopicView.DataSource = topics;
                TopicView.DataBind();

            }
            int pageSize = Config.TopicPageSize;
            
            
            var replies = Topics.GetRepliesForTopic(_topic, pageIndex, pageSize);
            if(replyFilter.SelectedIndex > 0)
            {
                switch (replyFilter.SelectedValue)
                {
                    case "Asc" :
                        replies = replies.OrderBy(r => r.Date);
                        break;
                    case "Desc" :
                        replies = replies.OrderByDescending(r => r.Date);
                        break;
                    case "Last" :
                        replies = replies.Where(r => r.Date >= _topic.LastPostDate);
                        break;
                    case "New" :
                        replies = replies.Where(r => r.Date > ((PageBase) Page).LastVisitDateTime);
                        break;
                }
            }
            var page = new PagedDataSource
            {
                AllowCustomPaging = true,
                AllowPaging = true,
                DataSource = replies,
                PageSize = pageSize
            };
            TopicReplies.DataSource = page;
            TopicReplies.DataBind();
            
        }
     
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PageScriptManager.Services.Add(new ServiceReference("~/CommonFunc.asmx"));
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
            jsshareCSS.Attributes.Add("href", "/css/" + Page.Theme + "/jsShare.css");

            if (TopicId == null)
                throw new HttpException(404, "Topic not found");

            if (Request.QueryString["ARCHIVE"] != null)
            {
                if (Request.QueryString["ARCHIVE"] == "1")
                {
                    ArchiveView = 1;
                }
            }
            else
            {
                ArchiveView = 0;
            }
            try
            {
                if (TopicId != null)
                {
                    if (Session["TOPIC"] == null)
                        Session.Add("TOPIC", TopicId);
                    else
                        Session["TOPIC"] = TopicId;

                    string skip = "";
                    if (!String.IsNullOrEmpty(Request.Params["dir"]))
                    {
                        skip = Request.Params["dir"];
                    }
                    _topic = Topics.GetTopic(TopicId.Value);
                    _forum = Forums.GetForum(_topic.ForumId);
                    if (_forum.Type == (int) Enumerators.ForumType.BlogPosts)
                    {
                        MinWeblog.MemberId = _topic.AuthorId;
                        MinWeblog.ForumId = _topic.ForumId;
                        MinWeblog.Visible = true;
                    }
                    else
                    {
                        MinWeblog.Visible = false;
                    }
                    if (skip != "")
                    {
                        _topic = Topics.GetNextPrevTopic(_topic.Id, skip);
                        TopicId = _topic.Id;
                    }
                    _topic.Author = Members.GetAuthor(_topic.AuthorId);
                    //Grid pager setup
                    ReplyPager = (GridPager) LoadControl("~/UserControls/GridPager.ascx");
                    ReplyPager.PagerStyle = Enumerators.PagerType.Linkbutton;
                    ReplyPager.UserControlLinkClick += PagerLinkClick;
                    RowCount = _topic.ReplyCount;
                    ReplyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Config.TopicPageSize);

                    Page.Title = string.Format(webResources.ttlTopicPage, _topic.Subject.CleanForumCodeTags(), Config.ForumTitle);
                    string pagedescription = _topic.Message.CleanForumCodeTags();
                    metadescription.Text = String.Format("<meta name=\"description\" content=\"{0}\">", HttpUtility.HtmlEncode(pagedescription.Substring(0, Math.Min(160, pagedescription.Length))));

                }
            }
            catch (Exception)
            {
                throw new HttpException(404, "Topic not found");
            }

            var meta = new HtmlMeta();
            meta.Attributes.Add("name", "description");
            meta.Attributes.Add("content", _topic.Subject);
            Page.Header.Controls.Add(meta);

            if (User.Identity.IsAuthenticated)
            {
                if ((Config.ShowQuickReply && _topic.Status != (int)Enumerators.PostStatus.Closed && _topic.Forum.Status != (int)Enumerators.PostStatus.Closed) || IsAdministrator)
                {
                    var qr = (QuickReply)Page.LoadControl("~/UserControls/QuickReply.ascx"); //loading the user control dynamically
                    qr.thisTopic = _topic;
                    QRPlaceHolder.Controls.Add(qr);
                }
            }
            PopulateObject populate = PopulateData;
            ReplyPager.UpdateIndex = populate;
            pager.Controls.Add(ReplyPager);

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentPage == -1)
                CurrentPage = 0;


            if (Request.Form["__EVENTTARGET"] != null)
            {
                //let's check what async call posted back and see if we need to refresh the page
                string target = Request.Form["__EVENTTARGET"];
                var listOfStrings = new List<string> { "TopicSend", "DeleteTopic", "DeleteReply"};
                bool refreshAfterPostback = listOfStrings.Any(target.EndsWith);
                if (refreshAfterPostback)
                {
                    ReplyPager.CurrentIndex = CurrentPage;
                }
            }

            if (!Page.IsPostBack)
            {
                TopicInfo topic = _topic;
                string session = "FORUM" + topic.ForumId;

                if (IsAuthenticated)
                {

                    //do we have access to this forum
                    if (!Forums.IsUserInForumRole(Member.Username, topic.Forum.Id))
                    {
                        if (Session[session] == null || Session[session].ToString() != "true")
                        {

                            if (topic.Forum.Password != null &&
                                !String.IsNullOrEmpty(topic.Forum.Password.Trim()))
                            {
                                if (Session[session] == null || Session[session].ToString() == "")
                                {
                                    Response.Redirect("~/Content/Forums/forum.aspx?FORUM=" + topic.ForumId);
                                }
                                else
                                {
                                    if (Session[session].ToString() != "true")
                                        throw new SecurityException("You are not authorised to view this forum");
                                }
                            }
                        }
                        if (topic.Forum.Roles.Contains("All") || topic.Forum.Roles.Count == 0)
                        {
                            if (String.IsNullOrEmpty(topic.Forum.Password))
                            {
                                WriteShareItScriptTags();
                            }
                        }
                    }
                    else
                    {
                        Session[session] = "true";
                    }
                }
                else if (topic.Forum.Roles.Contains("All") || topic.Forum.Roles.Count == 0)
                {
                    Session[session] = "true";
                    WriteShareItScriptTags();
                }
                else if (topic.Forum.Roles.Count > 0 && !topic.Forum.Roles.Contains("All"))
                {
                    if (Session[session] == null || Session[session].ToString() != "true")
                        throw new SecurityException("You must be logged in to view this forum");
                }

                Topics.UpdateViewCount(_topic.Id);
                int pagejump = 0;

                if (Request.QueryString["LastVisit"] != null)
                {
                    pagejump = SnitzCookie.LastTopicPage(topic.Id);

                }
                if (pagejump == 0)
                {
                    if ((Request.Params["whichpage"] != null))
                    {
                        pagejump = Int32.Parse(Request.Params["whichpage"]);
                    }
                }
                if (pagejump != 0)
                {
                    if (pagejump == -1)
                    {
                        //jump to last page
                        int pagenum = topic.ReplyCount/Config.TopicPageSize;
                        if (topic.ReplyCount%Config.TopicPageSize == 0)
                            if (topic.ReplyCount > 0)
                                pagenum -= 1;
                        CurrentPage = pagenum;
                        ReplyPager.CurrentIndex = pagenum;
                    }
                    else
                    {
                        int pagenum = pagejump - 1;
                        CurrentPage = pagenum;
                        ReplyPager.CurrentIndex = pagenum;
                    }
                }
                else
                {
                    ReplyPager.CurrentIndex = CurrentPage;
                }
                SnitzCookie.TrackIt(_topic.Id, CurrentPage);
            }

            //TopicView.Visible = ReplyPager.CurrentIndex == 0;

            if (Request.Params["reply"] != null)
            {
                string reply = Request.Params["reply"];
                if (reply != "0")
                    JumpToReply(reply);
            }

        }

        protected override void Page_PreRender(object sender, EventArgs e)
        {
            pnlTopic.Visible = ReplyPager.CurrentIndex == 0;
            //if (ReplyPager.CurrentIndex != 0)
            //{
            //    pnlTopic.Attributes.Add("style","display:none;");
            //}

            base.Page_PreRender(sender, e);
        }

        private void WriteShareItScriptTags()
        {
            StringBuilder shareit = new StringBuilder();
            shareit.AppendLine("<script type=\"text/javascript\">");
            shareit.AppendFormat("var forumTitle = '{0}'", Config.ForumTitle).AppendLine();
            shareit.AppendFormat("var forumUrl = '{0}'", Config.ForumUrl).AppendLine();
            shareit.AppendFormat("var forumName = '{0}'", Config.ForumTitle).AppendLine();
            shareit.AppendFormat("var forumDesc = '{0}'", Config.ForumDescription).AppendLine();
            if(IsAuthenticated)
                shareit.AppendFormat("var urltarget = '{0}'", Profile.LinkTarget).AppendLine();
            else
                shareit.AppendLine("var urltarget = '_blank'");
            shareit.AppendLine("</script>");
            shareit.AppendLine("<link href=\"/css/" + Page.Theme + "/shThemeDefault.css\" rel=\"stylesheet\" type=\"text/css\" />");
            shareItScripts.Mode = LiteralMode.PassThrough;
            shareItScripts.Text = shareit.ToString();
            ModController modcontroler = new ModController("ShareIt");
            if (modcontroler.ModInfo.Enabled)
            {
                string[] hideitems = modcontroler.ModInfo.Settings["MediaItems"].ToString().Split(',');
                StringBuilder shareitscript = new StringBuilder();
                shareitscript.AppendLine("$('#buttons').jsShare({ maxwidth: 240 });");
                shareitscript.AppendLine("$('#buttons-expanded').jsShare({ initialdisplay: 'expanded', maxwidth: 240 ");
                foreach (string hideitem in hideitems)
                {
                    shareitscript.AppendFormat(",{0}: false", hideitem);
                }
                shareitscript.AppendLine("});");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "shareit", shareitscript.ToString(), true);
            }
        }

        private void JumpToReply(string reply)
        {
            int replyPage = Replies.FindReplyPage(Convert.ToInt32(reply));
            CurrentPage = replyPage;
            ReplyPager.CurrentIndex = CurrentPage;
        }

        private void PagerLinkClick(object sender, EventArgs e)
        {
            var lnk = sender as LinkButton;

            if (lnk != null)
            {
                if (lnk.Text.IsNumeric())
                    CurrentPage = int.Parse(lnk.Text)-1;
                else
                {
                    if (lnk.Text.Contains("&gt;"))
                        CurrentPage += 1;
                    else if (lnk.Text.Contains("&lt;"))
                        CurrentPage -= 1;
                    else if (lnk.Text.Contains("&raquo;"))
                        CurrentPage = ReplyPager.PageCount-1;
                    else
                        CurrentPage = 0;
                }
                if (CurrentPage < 0)
                    CurrentPage = 0;
                if (CurrentPage >= ReplyPager.PageCount)
                    CurrentPage = ReplyPager.PageCount - 1;
            }
            SnitzCookie.TrackIt(_topic.Id, CurrentPage);
            ReplyPager.CurrentIndex = CurrentPage;
            

        }

        protected void ReplyFilterSelectedIndexChanged(object sender, EventArgs e)
        {
            ReplyPager.CurrentIndex = 0;
        }

        protected void TopicViewItemCommand(object sender, FormViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {

                case "prev" :
                    _topic = Topics.GetNextPrevTopic(_topic.Id, "prev");
                    if (Session["TOPIC"].ToString() != _topic.Id.ToString())
                        Response.Redirect("~/Content/Forums/topic.aspx?TOPIC=" + _topic.Id, true);
                    break;
                case "next" :
                    _topic = Topics.GetNextPrevTopic(_topic.Id, "next");
                    if (Session["TOPIC"].ToString() != _topic.Id.ToString())
                        Response.Redirect("~/Content/Forums/topic.aspx?TOPIC=" + _topic.Id, true);
                    break;
            }
        }

        //private void ReplyDeleteClicked(object sender, EventArgs e)
        //{
        //    Response.Redirect(Request.RawUrl);
        //}

        private void TopicDeleted(object sender, EventArgs e)
        {
            ReplyPager.CurrentIndex = CurrentPage;
        }

        protected void TopicBound(object sender, EventArgs e)
        {
            var frm = (FormView)sender;
            var currentTopic = ((TopicInfo) frm.DataItem);

            if (!Config.ShowTopicNav || currentTopic.IsArchived || (currentTopic.Forum.Type == (int)Enumerators.ForumType.BlogPosts))
            {
                frm.HeaderRow.Visible = false;
            }
        
            currentTopic.PollId = Topics.GetTopicPollId(currentTopic.Id);

            var poll = (PollTemplate)frm.FindControl("pollTemplate");
            var topic = (TopicTemplate)frm.FindControl("topicTemplate");
            var blog = (BlogTemplate)frm.FindControl("blogTemplate");
            if (currentTopic.Forum.Type == (int) Enumerators.ForumType.BlogPosts)
            {   //Blog Post
                topic.Visible = false;
                poll.Visible = false;
                blog.Visible = true;
                pbTop.DisplaySettings = new[] {"NewTopic"};
                blog.Post = currentTopic;
            }
            else if (currentTopic.PollId != null && currentTopic.PollId.Value > 0)
            {   //Poll
                topic.Visible = false;
                blog.Visible = false;
                poll.Visible = true;
                pbTop.DisplaySettings = new[] { "" }; //to hide all buttons
                poll.Post = currentTopic;
                var bBar = poll.FindControl("buttonBar");
                if (bBar != null)
                {
                    ((MessageButtonBar) bBar).ReplyDeleteClicked += TopicDeleted;
                }
                poll.PollId = currentTopic.PollId.Value;
            }
            else
            {
                Page.IncludeStyles("#rightcolumn,.rightcolumn{ display: none;width: 0px;} #contentwrapper{ margin-right: 0px;} .maincolumn{ width: 99%;}");
                poll.Visible = false;
                blog.Visible = false;
                topic.Visible = true;
                topic.Post = currentTopic;
                var bBar = topic.FindControl("buttonBar");
                if (bBar != null)
                {
                    ((MessageButtonBar)bBar).ReplyDeleteClicked += TopicDeleted;
                }
                if (currentTopic != null) topic.Post = currentTopic;             
            }

        }


        protected void BindReply(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //ReplyInfo reply = (ReplyInfo) e.Item.DataItem;

                var postctrl = e.Item.FindControl("PostHolder");
                if (_topic != null)
                {
                    
                    if (_topic.Forum.Type == (int) Enumerators.ForumType.BlogPosts)
                    {
                        var btemplate =
                            (BlogReplyTemplate) LoadControl("~/UserControls/Post Templates/BlogReplyTemplate.ascx");
                        btemplate.Post = e.Item.DataItem;
                        
                        if (postctrl != null)
                            postctrl.Controls.Add(btemplate);
                    }
                    else if (_topic.PollId > 0)
                    {
                        var btemplate =
                            (BlogReplyTemplate)LoadControl("~/UserControls/Post Templates/BlogReplyTemplate.ascx");
                        btemplate.Post = e.Item.DataItem;

                        if (postctrl != null)
                            postctrl.Controls.Add(btemplate);                        
                    }
                    else
                    {
                        var template = (ReplyTemplate)LoadControl("~/UserControls/Post Templates/ReplyTemplate.ascx");

                        template.CssClass = e.Item.ItemType == ListItemType.Item ? "ReplyDiv clearfix" : "AltReplyDiv clearfix";
                        template.Alternate = e.Item.ItemType == ListItemType.AlternatingItem;
                        template.Post = e.Item.DataItem;
                        if (postctrl != null)
                            postctrl.Controls.Add(template);    
                    }            
                }

            }
        }

        public SiteMapNode SiteMapResolve(object sender, SiteMapResolveEventArgs e)
        {

            SiteMapNode currentNode = null;

            if (SiteMap.CurrentNode == null)
            {
                var routable = e.Context.CurrentHandler as IRoutablePage;

                if (routable != null)
                {
                    var rc = routable.Routing.RequestContext;
                    var route = rc.RouteData.Route;
                    var segments = route.GetVirtualPath(rc, null).VirtualPath.Split('/');
                    var path = "~/" + string.Join("/", segments.Take(segments.Length - rc.RouteData.Values.Count).ToArray());
                    path = path.Replace("MyWebLog", "Topic").Replace("WebLog", "Topic");
                    var findSiteMapNodeFromKey = SiteMap.Provider.FindSiteMapNodeFromKey(path);
                    if (findSiteMapNodeFromKey != null)
                        currentNode = findSiteMapNodeFromKey.Clone(true);
                }
            }
            else
            {
                currentNode = SiteMap.CurrentNode.Clone(true);
            }

            SiteMapNode tempNode = currentNode;

            string strStatus = "";
            if (_topic.Status == 0)
                strStatus = " (locked) ";
            if (_topic.IsArchived)
            {
                strStatus = " (Archived)";
                _topic.Status = 0;
            }
            tempNode.Title = HttpUtility.HtmlDecode(_topic.Subject) + strStatus;
            var smp = (SiteMapPath)Master.FindControl("SiteMap");
            //set breadcrumb for Blog posts
            if (_forum.Type == (int)Enumerators.ForumType.BlogPosts)
            {
                var url = String.Format("[url=\"/WebLog/{0}\"]{1}[/url]", _topic.AuthorName, String.Format(webResources.lblBlogTitle, _topic.AuthorName));
                tempNode.Title = string.Format("{0}{1}{2}", url,smp.PathSeparator,HttpUtility.HtmlDecode(_topic.Subject));
            }
            if (_topic.PollId > 0)
            {
                tempNode.Title = String.Format("POLL - {0}", tempNode.Title);
            }

            tempNode = tempNode.ParentNode;
            tempNode.Title = HttpUtility.HtmlDecode(_topic.Forum.Subject);
            tempNode.Url = tempNode.Url + "?FORUM=" + _topic.ForumId;
            if (_topic.IsArchived)
                tempNode.Url += "&ARCHIVE=1";

            return currentNode;
        }
    }
}