/*
####################################################################################################################
##
## SnitzUI.UserControls.Popups - UserProfile.ascx
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
using System.Web;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;
using SnitzMembership;


namespace SnitzUI.UserControls
{
    public partial class UserProfile : TemplateUserControl
    {
        public int UserId { get; set; }

        protected bool IsAuthenticated { get; set; }

        protected MemberInfo CurrentUser { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                UserId = ((int)Data);
                CurrentUser = Members.GetMember(HttpContext.Current.User.Identity.Name);
                IsAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
                List<MemberInfo> authrsrc = new List<MemberInfo> {Members.GetMember(UserId)};
                rpt.DataSource = authrsrc;
                rpt.DataBind();
            }
        }

        protected void RptItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;

            AuthorInfo author;
            if (Cache["M" + UserId] == null)
            {
                author = Members.GetAuthor(UserId);
                Cache.Insert("M" + UserId, author, null, DateTime.Now.AddMinutes(10d),
                                System.Web.Caching.Cache.NoSlidingExpiration);
            }
            else
            {
                author = (AuthorInfo)Cache["M" + UserId];
            }
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                if (author != null)
                {
                    Label litRank = (Label)item.FindControl("MemberTitleLabel");
                    if (litRank != null)
                    {
                        string title = "";
                        RankInfo rInf = new RankInfo(author.Username, ref title, author.PostCount, SnitzCachedLists.GetRankings());
                        if (Config.ShowRankTitle)
                            litRank.Text = title;

                    }
                    ProfileCommon prof = ProfileCommon.GetUserProfile(author.Username);
                    if (prof.Gravatar)
                    {
                        var avatar = (Literal)item.FindControl("AvatarLabel");
                        avatar.Visible = false;
                        var ph = (PlaceHolder) item.FindControl("phAvatar");
                        ph.Visible = true;
                        var grav = new Gravatar {Email = author.Email};
                        if (author.AvatarUrl != "" && author.AvatarUrl.StartsWith("http:"))
                            grav.DefaultImage = author.AvatarUrl;
                        ph.Controls.Add(grav);

                    }
                    else
                    {
                        var avatar = (Literal)item.FindControl("AvatarLabel");
                        var ph = (PlaceHolder)item.FindControl("phAvatar");
                        avatar.Text = author.AvatarImg;
                        avatar.Visible = true;
                        ph.Visible = false;
                    }
                }
            }

        }
 
    }
}