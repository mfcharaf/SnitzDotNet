using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzData;

namespace SnitzUI.UserControls
{
    public partial class ForumLogin : System.Web.UI.UserControl
    {
        public Forum forum { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            login.UserName = HttpContext.Current.User.Identity.Name;
            Page.Form.DefaultButton = login.FindControl("LoginButton").UniqueID;
        }

        protected void OnAuthenticate(object sender, AuthenticateEventArgs e)
        {
            if (String.Compare(login.Password, forum.Password, false) == 0)
            {
                popup.Hide();
                Session.Add("FORUM" + forum.Id, "true");
                Response.Redirect(Request.RawUrl);
            }

        }

    }
}