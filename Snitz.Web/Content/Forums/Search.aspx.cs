#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
#endregion
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Resources;
using SnitzCommon;
using SnitzConfig;
using SnitzData;
using Image = System.Web.UI.WebControls.Image;

namespace SnitzUI
{
    public partial class Search : PageBase
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
            if (Session["CurrentProfile"] != null)
                Session.Remove("CurrentProfile");
            Page.Title = string.Format(webResources.ttlSearchPage, Config.ForumTitle);
            if (webResources.TextDirection == "rtl")
                pageCSS.Attributes.Add("href","/css/" + Page.StyleSheetTheme + "/searchrtl.css");
            else
                pageCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/search.css");
            tbxDateCalendarExtender.Format = Config.DateFormat;
            ddlForum.DataSource = SnitzCachedLists.GetForumListItems();
            ddlForum.DataTextField = "Name";
            ddlForum.DataValueField = "Id";
            ddlForum.DataBind();
            //Grid pager setup
            _replyPager = (GridPager)LoadControl("~/UserControls/GridPager.ascx");
            _replyPager.PagerStyle = PagerType.Lnkbutton;
            _replyPager.UserControlLinkClick += PagerLinkClick;

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateObject populate = PopulateData;
            _replyPager.UpdateIndex = populate;
            lnkAdvanced.Visible = IsAuthenticated;
            if (Request.Form["__EVENTTARGET"] != null)
            {
                if (Request.Form["__EVENTTARGET"].Contains("lnkPage"))
                {
                    string target = Request.Form["__EVENTTARGET"];
                    if (target.Length > target.IndexOf("lnkPage")) target = target.Substring(target.IndexOf("lnkPage"));
                    target = target.Replace("lnkPage", "");
                    if (target.IsNumeric())
                        _replyPager.CurrentIndex = Convert.ToInt32(target)-1;
                    else
                    {
                        switch (target.ToLower())
                        {
                            case "prev" :
                                _replyPager.CurrentIndex = CurrentPage - 1;
                                break;
                            case "next" :
                                _replyPager.CurrentIndex = CurrentPage + 1;
                                break;
                            case "last" :
                                _replyPager.CurrentIndex = RowCount - 1;
                                break;
                            case "first" :
                                _replyPager.CurrentIndex = 0;
                                break;

                        }
                    }
                    //ReplyPager.CurrentIndex = CurrentPage;
                }
            }
            if(HttpContext.Current.Items["Subject"] != null)
            {
                int rowcount = 0;
                CurrentPage = 0;
                var searchResults = Util.FindTopics(HttpContext.Current.Items["Subject"].ToString());
                RowCount = rowcount;

                SearchResults.DataSource = searchResults;
                //GroupByCategoryForum();
                SearchResults.PageSize = 10;
                SearchResults.DataBind();
                if (SearchResults.BottomPagerRow != null)
                    SearchResults.BottomPagerRow.Visible = true;
                SearchResults.Visible = true;
            }
        }

        protected void SearchForums(object sender, EventArgs eventArgs)
        {
            FindPosts();
        }

        private void FindPosts()
        {
            var sparams = new SearchParams
                                       {
                                           ForumId = Convert.ToInt32(ddlForum.SelectedValue),
                                           Match = ddlMatch.SelectedValue,
                                           AuthorPostType = ddlUserPosts.SelectedValue,
                                           Author = tbxUserName.Text.Trim(),
                                           SearchFor = searchFor.Text.Trim(),
                                           MessageAndSubject = true,
                                           PageSize = Convert.ToInt32(ddlPageSize.SelectedValue)
                                       };

            if (!string.IsNullOrEmpty(tbxDate.Text.Trim()))
            {
                if (ddlSince.SelectedValue == "before")
                    sparams.BeforeDate = Convert.ToDateTime(tbxDate.Text);
                else
                    sparams.SinceDate = Convert.ToDateTime(tbxDate.Text);
            }
            int rowcount = 0;
            CurrentPage = _replyPager.CurrentIndex;
            var searchResults = SnitzData.Search.FindTopics(sparams, CurrentPage, ref rowcount);
            RowCount = rowcount;

            SearchResults.DataSource = searchResults;
            //GroupByCategoryForum();
            SearchResults.PageSize = sparams.PageSize;
            SearchResults.DataBind();
            if(SearchResults.BottomPagerRow != null)
                SearchResults.BottomPagerRow.Visible = true;
            SearchResults.Visible = true;

            
        }

        protected void LinkButton1Click(object sender, EventArgs e)
        {
            extendedSearch.Visible = true;
            Options.Visible = true;
            lnkAdvanced.Visible = false;
        }

        protected override SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
                SiteMapNode tempNode = currentNode;

                tempNode.Title = webResources.lblSearch;
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
                var topic = (Topic)e.Row.DataItem;
                var popuplink = e.Row.Cells[5].FindControl("popuplink") as Literal;

                if (popuplink != null)
                {
                    string title = String.Format(webResources.lblViewProfile, "$1");
                    popuplink.Text = topic.LastPostAuthor != null ? Regex.Replace(topic.LastPostAuthor.ProfilePopup, @"\[!(.*)!]", title) : "";
                }
                int replyCount = topic.ReplyCount;

                e.Row.Cells[0].Controls.Add(GetRecentTopicIcon(topic.Status, replyCount));

                if (HttpContext.Current.User.Identity.Name == "")
                {
                    if (e.Row.Cells.Count > 2)
                    {
                        e.Row.Cells.RemoveAt(6);
                    }

                }
            }
            else if (e.Row.RowType == DataControlRowType.Pager)
            {
                var ph = (PlaceHolder)e.Row.FindControl("phPager");
                
                _replyPager.PageCount = Common.CalculateNumberOfPages(RowCount, Convert.ToInt32(ddlPageSize.SelectedValue));
                
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
        }
    }
}