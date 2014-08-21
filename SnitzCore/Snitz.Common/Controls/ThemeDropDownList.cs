using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using SnitzConfig;

namespace SnitzCommon.Controls
{
    public class ThemeDropDownList : DropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/App_Themes"));
            var themes = dir.GetDirectories();
            foreach (DirectoryInfo theme in themes)
            {
                this.Items.Add(new ListItem(theme.Name,theme.Name));
            }
            if(this.AutoPostBack)
                this.SelectedIndexChanged += ChangeTheme;
        }

        private void ChangeTheme(object sender, EventArgs e)
        {
            Config.UserTheme = this.SelectedValue;
            HttpContext.Current.Session.Add("PageTheme", Config.UserTheme);
            HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
        }
    }
}
