using System;
using System.Web.UI;
using System.Web.UI.Adapters;
using System.Web.UI.WebControls;


namespace SnitzControlAdapters
{
    /// <summary>
    /// Fixes a bug when rendering HyperLinks
    /// </summary>
    public class HyperLinkControlAdapter : ControlAdapter
    {
        protected override void Render(HtmlTextWriter writer)
        {
            HyperLink hl = this.Control as HyperLink;
            if (hl == null)
            {
                base.Render(writer);
                return;
            }

            // This code is copied from HyperLink.RenderContents (using
            // Reflector). References to "this" have been changed to
            // "hl", and we have to render the begin and end tags.
            string imageUrl = hl.ImageUrl;
            if (imageUrl.Length > 0)
            {
                // Let the HyperLink render its begin tag
                hl.RenderBeginTag(writer);

                Image image = new Image();

                // I think the next line is the bug. The URL gets
                // resolved here, but the Image.UrlResolved property
                // doesn't get set. So another attempt to resolve the
                // URL is made in Image.AddAttributesToRender. It's in
                // the callstack above that method that the exception
                // or improperly resolved URL happens.
                //image.ImageUrl = base.ResolveClientUrl(imageUrl);
                image.ImageUrl = imageUrl;

                imageUrl = hl.ToolTip;
                if (imageUrl.Length != 0)
                {
                    image.ToolTip = imageUrl;
                }

                imageUrl = hl.Text;
                if (imageUrl.Length != 0)
                {
                    image.AlternateText = imageUrl;
                }

                image.RenderControl(writer);

                // Wrap up by letting the HyperLink render its end tag
                hl.RenderEndTag(writer);
            }
            else
            {
                // HyperLink.RenderContents handles a couple of other
                // cases if its ImageUrl property hasn't been set. We
                // delegate to that behavior here.
                base.Render(writer);
            }
        }
    }

    /// <summary>
    /// Overrides rendering of LinkButton to add Snitzbutton class
    /// and span tag to create links that look like buttons
    /// </summary>
    public class LinkButtonControlAdapter : ControlAdapter
    {
        protected override void Render(HtmlTextWriter writer)
        {

            LinkButton linkButton = this.Control as LinkButton;
            if (linkButton != null)
            {
                if (String.IsNullOrEmpty(linkButton.CssClass))
                    linkButton.CssClass = "Snitzbutton ";
                linkButton.Text = String.Concat("<span>", linkButton.Text, "</span>");
            }

            base.Render(writer);
        }
    }
}

