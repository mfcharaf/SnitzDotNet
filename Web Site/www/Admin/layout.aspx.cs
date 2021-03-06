#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
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
using System.Web;
using Snitz;

public partial class layout : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session.Add("_theme", DropDownList1.SelectedValue);
        Response.Redirect("~/ThemeRedirect.aspx?uri=" + HttpUtility.UrlEncode(Request.Url.ToString()));
    }

    protected void DropDownList1_DataBound(object sender, EventArgs e)
    {
        DropDownList1.SelectedValue = Page.StyleSheetTheme;
    }
    protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session.Add("_MasterPage", "~/Master_Templates/" + DropDownList2.SelectedValue);
        Response.Redirect("~/ThemeRedirect.aspx?uri=" + HttpUtility.UrlEncode(Request.Url.ToString()));
    }

    protected void DropDownList2_DataBound(object sender, EventArgs e)
    {
        string mPage = MasterPageFile.Substring(MasterPageFile.LastIndexOf("/")+1);
        DropDownList2.SelectedValue = mPage;
    }


}
