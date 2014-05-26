using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.MobileControls;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzMembership;

namespace SnitzUI
{
    public partial class Bookmark : PageBase
    {
        private string _userProfile;
        private ProfileCommon _profile;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (HttpContext.Current.Items["user"] != null)
            {
                _userProfile = HttpContext.Current.Items["user"].ToString();
            }
            else
                _userProfile = !String.IsNullOrEmpty(Request.Params["user"]) ? Request.Params["user"] : Member.Username;
            _profile = ProfileCommon.GetUserProfile(_userProfile);
            
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_profile != null)
            {
                repBookMarks.DataSource = _profile.BookMarks;
                repBookMarks.DataBind();                
            }

        }

        protected void DeleteBookMark(object sender, ImageClickEventArgs e)
        {
            PageBase page = (PageBase)Page;
            var id = ((ImageButton) sender).CommandArgument;

            List<SnitzLink> bookmarks = page.Profile.BookMarks;
            var todelete = bookmarks.Find(i => i.ID == Convert.ToInt32(id));
            bookmarks.Remove(todelete);
            _profile.BookMarks = bookmarks;
            _profile.Save();
            repBookMarks.DataSource = _profile;
            repBookMarks.DataBind();
        }
    }
}