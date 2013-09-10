/*
####################################################################################################################
##
## SnitzUI.UserControls - GridPager.ascx
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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using SnitzConfig;

public partial class GridPager : UserControl
{
    private static int _intCurrentIndex;
    private static int _pageCount;
    private Delegate _delUpdateIndex;

    #region Public Properties

    public Enumerators.PagerType PagerStyle { get; set; }
    public int PageCount
    {
        get { return _pageCount; }
        set
        {
            _pageCount = value;
            Visible = _pageCount > 1;
        }
    }
    public int CurrentIndex
    {
        get { return _intCurrentIndex; }
        set
        {
            _intCurrentIndex = value;
            SetCurrentIndex();
        }
    }
    public Delegate UpdateIndex
    {
        set { _delUpdateIndex = value; }
    }

    #endregion

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        dropdownPager.Visible = false;
        buttonPager.Visible = false;
        linkPager.Visible = false;
        textPager.Visible = false;

        switch (PagerStyle)
        {
            case Enumerators.PagerType.Button:
                buttonPager.Visible = true;
                break;
            case Enumerators.PagerType.Dropdown:
                dropdownPager.Visible = true;
                break;
            case Enumerators.PagerType.Text:
                textPager.Visible = true;
                string url = Request.RawUrl;
                if (url.Contains("&"))
                    url = url.Substring(0, Request.RawUrl.IndexOf("&"));
                litPager.Text = CreatePagerLinks(url);
                break;
            case Enumerators.PagerType.Lnkbutton:
                linkPager.Visible = true;
                CreatePagingControl();
                break;
            default:
                linkPager.Visible = true;
                CreatePagingControl();
                break;
        }
    }
    
    private void UpdateControls()
    {

        Visible = _pageCount > 1;
        if (!Visible)
            return;
        if(buttonPager.Visible)
            lblCurrentPage.Text = string.Format(webResources.lblPagerButton, _intCurrentIndex + 1, _pageCount);
        if (dropdownPager.Visible)
        {
            pagingLabel2.Text = string.Format(webResources.lblPagerDropdown, _pageCount);
            ddlPageSelector.Items.Clear();
            for (int ii = 1; ii <= _pageCount; ii++)
                ddlPageSelector.Items.Add(new ListItem(ii.ToString(), ii.ToString()));
            if (ddlPageSelector.Items.Count > 0)
                ddlPageSelector.SelectedIndex = _intCurrentIndex;
        }
        if(linkPager.Visible)
            CreatePagingControl();
        
        
    }
    
    private void SetCurrentIndex()
    {
        object[] aObj = new object[1];
        aObj[0] = _intCurrentIndex;
        if(_delUpdateIndex != null)
            _delUpdateIndex.DynamicInvoke(aObj);
        UpdateControls();
    }

    #region dropdown pager controls

    protected void cmdPrev_Click(object sender, EventArgs e)
    {
        _intCurrentIndex -= 1;
        ((Button)sender).Enabled = (_intCurrentIndex != 0);
        SetCurrentIndex();
    }
    protected void cmdNext_Click(object sender, EventArgs e)
    {
        _intCurrentIndex += 1;
        ((Button)sender).Enabled = (_intCurrentIndex < _pageCount);
        SetCurrentIndex();
    }

    protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
    {
        _intCurrentIndex = ddlPageSelector.SelectedIndex;
        SetCurrentIndex();
    }

    #endregion

    #region textlink pager control

    public string CreatePagerLinks(string BaseUrl)
    {
        
        //const string pageurl = "<a href=\"{0}\" class=\"pagelink\" >{1}</a>";

        StringBuilder sbPager = new StringBuilder();
        sbPager.AppendFormat("<span id=\"pagecounter\">{0} pages</span> ", _pageCount);
        if (_intCurrentIndex != 0)
        {
            // first page link

            sbPager.Append("<a href=\"");
            sbPager.Append(BaseUrl);
            sbPager.Append("\" class=\"pagelink\"><span>&laquo;</span></a> ");

            // previous page link
            sbPager.Append("<a href=\"");
            sbPager.Append(BaseUrl);
            sbPager.Append("&whichpage=");
            sbPager.Append(_intCurrentIndex.ToString());
            sbPager.Append("\" alt=\"Previous Page\" class=\"pagelink\"><span>&lt;</span></a>  ");

        }
        // calc low and high limits for numeric links
        int intLow = _intCurrentIndex - 1;
        int intHigh = _intCurrentIndex + 3;
        if (intLow < 1) intLow = 1;
        if (intHigh > _pageCount) intHigh = _pageCount;
        if (intHigh - intLow < 5) while ((intHigh < intLow + 4) && intHigh < _pageCount) intHigh++;
        if (intHigh - intLow < 5) while ((intLow > intHigh - 4) && intLow > 1) intLow--;
        for (int x = intLow; x < intHigh + 1; x++)
        {
            // numeric links
            if (x == _intCurrentIndex + 1) sbPager.Append("<span class=\"currentpagelink\">" + x + "</span>  ");
            else
            {
                sbPager.Append("<a href=\"");
                sbPager.Append(BaseUrl);
                sbPager.Append("&whichpage=");
                sbPager.Append(x.ToString());
                sbPager.Append("\" class=\"pagelink\"><span>");
                sbPager.Append(x.ToString());
                sbPager.Append("</span></a>&nbsp;");
            }
        }
        if (_intCurrentIndex != _pageCount - 1)
        {
            if ((_intCurrentIndex) < _pageCount - 1)
            {
                // next page link
                sbPager.Append("<a href=\"");
                sbPager.Append(BaseUrl);
                sbPager.Append("&whichpage=");
                sbPager.Append(Convert.ToString(_intCurrentIndex + 2));
                sbPager.Append("\" class=\"pagelink\"><span>&gt;</span></a>&nbsp;");
            }
            // last page link
            sbPager.Append("<a href=\"");
            sbPager.Append(BaseUrl);
            sbPager.Append("&whichpage=");
            sbPager.Append(_pageCount.ToString());
            sbPager.Append("\" class=\"pagelink\"><span>&raquo;</span></a>");
        }
        // conver the final links to a string and assign to labels

        return sbPager.ToString();

    }

    #endregion

    #region linkbutton pager controls

    public event EventHandler UserControlLinkClick;
    protected void CreatePagingControl()
    {
        plcPaging.Controls.Clear();
        Literal pagecount = new Literal
                                {
                                    Text = String.Format("<span class=\"pagecounter\">{0} pages</span>&nbsp;", _pageCount)
                                };
        plcPaging.Controls.Add(pagecount);

        if (_intCurrentIndex != 0)
        {
            // first page link
            LinkButton first = new LinkButton { ID = "lnkPageFirst", CommandName = "Page", CommandArgument = "First", Text = @"&laquo;", CssClass = "pagelinkfirst" };
            first.Click += PagerClick;
            first.CausesValidation = false;
            plcPaging.Controls.Add(first);
            Literal spacer = new Literal { Text = @"&nbsp;" };
            plcPaging.Controls.Add(spacer);

            // previous page link
            LinkButton prev = new LinkButton { ID = "lnkPagePrev", CommandName = "Page", CommandArgument = "Prev", Text = @"&lt;", CssClass = "pagelink" };
            prev.Click += PagerClick;
            prev.CausesValidation = false;
            plcPaging.Controls.Add(prev);
            Literal spacer2 = new Literal { Text = @"&nbsp;" };
            plcPaging.Controls.Add(spacer2);
        }

        // calc low and high limits for numeric links
        int intLow = _intCurrentIndex - 1;
        int intHigh = _intCurrentIndex + 3;
        if (intLow < 1) intLow = 1;
        if (intHigh > _pageCount) intHigh = _pageCount;
        if (intHigh - intLow < 5) while ((intHigh < intLow + 4) && intHigh < _pageCount) intHigh++;
        if (intHigh - intLow < 5) while ((intLow > intHigh - 4) && intLow > 1) intLow--;
        for (int x = intLow; x < intHigh + 1; x++)
        {
            if (x == _intCurrentIndex + 1)
            {
                //sbPager.Append("<span class=\"currentpagelink\">" + x + "</span>  ")
                Literal current = new Literal
                                      {
                                          Text = String.Format("<span class=\"currentpagelink\">{0}</span>&nbsp;", x)
                                      };
                plcPaging.Controls.Add(current);

            }
            else
            {
                LinkButton lnk = new LinkButton { ID = "lnkPage" + (x), CommandName = "Page", CommandArgument = (x).ToString(), Text = (x).ToString(), CssClass = "pagelink" };
                lnk.Click += PagerClick;
                lnk.CausesValidation = false;
                plcPaging.Controls.Add(lnk);
                Literal spacer = new Literal { Text = @"&nbsp;" };
                plcPaging.Controls.Add(spacer);
            }

        }
        if (_intCurrentIndex != _pageCount - 1)
        {
            if ((_intCurrentIndex) < _pageCount - 1)
            {
                // next page link
                LinkButton next = new LinkButton { ID = "lnkPageNext", CommandName = "Page", CommandArgument = "Next", Text = @"&gt;", CssClass = "pagelink" };
                next.Click += PagerClick;
                next.CausesValidation = false;
                plcPaging.Controls.Add(next);
                Literal spacer = new Literal { Text = @"&nbsp;" };
                plcPaging.Controls.Add(spacer);
            }
            // last page link
            LinkButton last = new LinkButton { ID = "lnkPageLast", CommandName = "Page", CommandArgument = "Last", Text = @"&raquo;", CssClass = "pagelinklast" };
            last.Click += PagerClick;
            last.CausesValidation = false;
            plcPaging.Controls.Add(last);

        }

    }

    protected void PagerClick(object sender, EventArgs e)
    {
        if (this.UserControlLinkClick != null)
        {
            this.UserControlLinkClick(sender, e);
        }
    }

    #endregion

}
