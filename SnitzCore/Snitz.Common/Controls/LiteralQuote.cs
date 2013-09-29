using System;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;


namespace SnitzCommon
{
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public sealed class LiteralQuote : LiteralControl
    {
        public string CssClass { get; set; }
        protected override void Render(HtmlTextWriter writer)
        {
            // Write out some literal text.
            writer.Write(String.Format("<div id=\"outerQ\" class=\"{0}\">",this.CssClass));
            writer.Write(String.Format("<div id=\"innerQ\" class=\"{0}\">", this.CssClass));
            base.Render(writer);
            writer.Write("</div>");
            writer.Write("</div>");

        }
    }

}
