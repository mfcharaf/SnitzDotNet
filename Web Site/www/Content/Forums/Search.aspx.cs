/*
####################################################################################################################
##
## SnitzUI.Content.Forums - Search.aspx
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;

using Image = System.Web.UI.WebControls.Image;

namespace SnitzUI
{
    public partial class Search : PageBase, ISiteMapResolver
    {
        private GridPager _replyPager;
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
            if(IsPostBack && currentPage != CurrentPage)
                FindPosts();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            CurrentPage = 0;
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            Page.Title = string.Format(webResources.ttlSearchPage, Config.ForumTitle);
            if (webResources.TextDirection == "rtl")
                pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/searchrtl.css");
            else
                pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/search.css");
            tbxDateCalendarExtender.Format = Config.DateFormat;
            ddlForum.DataSource = SnitzCachedLists.GetCachedForumList(true);
            ddlForum.DataTextField = "Name";
            ddlForum.DataValueField = "Id";
            ddlForum.DataBind();
            //Grid pager setup
            _replyPager = (GridPager)LoadControl("~/UserControls/GridPager.ascx");
            _replyPager.PagerStyle = Enumerators.PagerType.Linkbutton;
            _replyPager.UserControlLinkClick += PagerLinkClick;

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            lnkAdvanced.Visible = IsAuthenticated;
            if (Request.Form["__EVENTTARGET"] != null)
            {
                if (Request.Form["__EVENTTARGET"].Contains("lnkPage"))
                {
                    string target = Request.Form["__EVENTTARGET"];
                    if (target.Length > target.IndexOf("lnkPage")) target = target.Substring(target.IndexOf("lnkPage"));
                    target = target.Replace("lnkPage", "");
                    if (target.IsNumeric())
                        _replyPager.CurrentIndex = Convert.ToInt32(target) - 1;
                    else
                    {
                        switch (target.ToLower())
                        {
                            case "prev":
                                _replyPager.CurrentIndex = CurrentPage - 1;
                                break;
                            case "next":
                                _replyPager.CurrentIndex = CurrentPage + 1;
                                break;
                            case "last":
                                _replyPager.CurrentIndex = _replyPager.PageCount - 1;
                                break;
                            case "first":
                                _replyPager.CurrentIndex = 0;
                                break;

                        }
                    }
                    PopulateData(_replyPager.CurrentIndex);
                }
            }
            if(HttpContext.Current.Items["Subject"] != null)
            {
                CurrentPage = 0;
                var searchResults = Topics.GetTopicsBySubject(HttpContext.Current.Items["Subject"].ToString());
                RowCount = 0;

                SearchResults.DataSource = searchResults;
                SearchResults.PageSize = 10;
                SearchResults.DataBind();
                if (SearchResults.BottomPagerRow != null)
                    SearchResults.BottomPagerRow.Visible = true;
                SearchResults.Visible = true;
            }

            if (ViewState["Extended"] != null)
            {
                if ((bool) ViewState["Extended"])
                {
                    if(!IsPostBack)
                    ExpandSearchForm();
                    //cbxArchive.Checked = false;
                }
            }
        }

        protected void SearchForums(object sender, EventArgs eventArgs)
        {
            _replyPager.CurrentIndex = 0;
            FindPosts();
        }

        private void FindPosts()
        {
            string orderby = ddlSortBy.SelectedValue;

            var sparams = new SearchParamInfo
                                       {
                                           ForumId = Convert.ToInt32(ddlForum.SelectedValue),
                                           Match = ddlMatch.SelectedValue,
                                           AuthorPostType = ddlUserPosts.SelectedValue,
                                           Author = tbxUserName.Text.Trim(),
                                           SearchFor = searchFor.Text.Trim(),
                                           MessageAndSubject = !cbxSubjectOnly.Checked,
                                           SubjectOnly = cbxSubjectOnly.Checked,
                                           PageSize = Convert.ToInt32(ddlPageSize.SelectedValue),
                                           Archived = cbxArchive.Checked
                                       };
            if (sparams.ForumId == -99)
            {
                //no forum selected
                return;
            }
            if (!string.IsNullOrEmpty(tbxDate.Text.Trim()))
            {
                if (ddlSince.SelectedValue == "before")
                    sparams.BeforeDate = Convert.ToDateTime(tbxDate.Text);
                else
                    sparams.SinceDate = Convert.ToDateTime(tbxDate.Text);
            }
            int rowcount = 0;
            CurrentPage = _replyPager.CurrentIndex;
            var searchResults = Topics.FindTopics(sparams, CurrentPage, orderby, ref rowcount);
            RowCount = rowcount;

            SearchResults.DataSource = searchResults;
            SearchResults.PageSize = sparams.PageSize;
            SearchResults.DataBind();
            if(SearchResults.BottomPagerRow != null)
                SearchResults.BottomPagerRow.Visible = true;
            SearchResults.Visible = true;
            HideSearchForm();
        }

        protected void LinkButton1Click(object sender, EventArgs e)
        {
            lnkAdvanced.Visible = false;
            ExpandSearchForm();
            ViewState["Extended"] = true;
        }

        private void ExpandSearchForm()
        {
            extendedSearch.Attributes.Remove("style");
            Options.Attributes.Remove("style");
            pnlSearch.Attributes.Remove("style");
            btnSearch.Visible = true;
            
        }
        private void HideSearchForm()
        {
            extendedSearch.Attributes.Add("style", "display:none;");
            Options.Attributes.Add("style", "display:none;");
            pnlSearch.Attributes.Add("style","display:none;");
            lnkAdvanced.Visible = true;
            btnSearch.Visible = false;
            ViewState["Extended"] = false;
        }
        public SiteMapNode SiteMapResolve(object sender, SiteMapResolveEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
                
                currentNode.Title = webResources.lblSearch;
                var test = searchFor.Text.Trim();
                if (!String.IsNullOrEmpty(test))
                {
                    SiteMapNode tempNode = currentNode;

                    var smp = (SiteMapPath)Master.FindControl("SiteMap");
                    //set breadcrumb for search results posts
                    var url = String.Format("[url=\"/Search\"]{0}[/url]", String.Format(webResources.lblSearch));
                    tempNode.Title = string.Format("{0}{1}{2}", url, smp.PathSeparator, "Results ..");

                }
                
                return currentNode;
            }
            return null;
        }
        
        private Image GetRecentTopicIcon(Enumerators.PostStatus tStatus, int tReplies)
        {
            var image = new Image { ID = "postIcon" };

            switch (tStatus)
            {
                case Enumerators.PostStatus.Closed:
                    image.SkinID = "FolderNewLocked";

                    break;
                default:
                    image.SkinID = "FolderNew";
                    if (tReplies > Config.HotTopicNum)
                        image.SkinID = "FolderNewHot";
                    break;
            }
            image.GenerateEmptyAlternateText = true;
            image.ApplyStyleSheetSkin(Page);
            return image;
        }

        protected void ResultsRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var topic = (TopicInfo)e.Row.DataItem;
                var popuplink = e.Row.Cells[4].FindControl("popuplink") as Literal;

                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = topic.LastPostAuthorId != null ? Regex.Replace(topic.LastPostAuthorPopup, @"\[!(.*)!]", title) : "";
                }
                int replyCount = topic.ReplyCount;

                e.Row.Cells[0].Controls.Add(GetRecentTopicIcon((Enumerators.PostStatus)topic.Status, replyCount));

                //if (HttpContext.Current.User.Identity.Name == "")
                //{
                //    if (e.Row.Cells.Count > 2)
                //    {
                //        e.Row.Cells.RemoveAt(4);
                //    }

                //}
            }
            else if (e.Row.RowType == DataControlRowType.Pager)
            {
                var ph = (PlaceHolder)e.Row.FindControl("phPager");
                
                _replyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Convert.ToInt32(ddlPageSize.SelectedValue));
                PopulateObject populate = PopulateData;
                _replyPager.UpdateIndex = populate;
                ph.Controls.Add(_replyPager);
            }
        }

        private void PagerLinkClick(object sender, EventArgs e)
        {
            var lnk = sender as LinkButton;

            if (lnk != null)
            {
                if (lnk.Text.IsNumeric())
                    CurrentPage = int.Parse(lnk.Text) - 1;
                else
                {
                    if (lnk.Text.Contains("&gt;"))
                        CurrentPage += 1;
                    else if (lnk.Text.Contains("&lt;"))
                        CurrentPage -= 1;
                    else if (lnk.Text.Contains("&raquo;"))
                        CurrentPage = _replyPager.PageCount - 1;
                    else
                        CurrentPage = 0;
                }
                if (CurrentPage < 0)
                    CurrentPage = 0;
                if (CurrentPage >= _replyPager.PageCount)
                    CurrentPage = _replyPager.PageCount - 1;
            }
            _replyPager.CurrentIndex = CurrentPage;
            PopulateData(_replyPager.CurrentIndex);
        }

        protected void SearchResults_Sorting(object sender, GridViewSortEventArgs e)
        {
            SearchResults.DataBind();
        }
    }
}