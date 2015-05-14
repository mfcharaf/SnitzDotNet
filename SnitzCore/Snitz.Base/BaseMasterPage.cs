/*
####################################################################################################################
##
## SnitzBase - BaseMasterPage
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
using System.Security.Authentication;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SnitzBase;
using SnitzConfig;

public abstract class BaseMasterPage : MasterPage
{
    public abstract string PageTimer
    {
        get;
        set;
    }

    public abstract string ForumUrl { get; set; }
    public abstract string ForumTitle { get; set; }

    protected override void OnInit(EventArgs e)
    {

        base.OnInit(e);
        
        var cph = (ContentPlaceHolder)this.FindControl("CPF2");
        if (cph != null)
        {
            var footer = new SnitzPageFooter();
            cph.Controls.Add(footer);
        }
        else
        {
            throw new InvalidCredentialException("Copyright Violation error");
        }
        var meta = new HtmlMeta();
        meta.Attributes.Add("name", "copyright");
        meta.Attributes.Add("content", "The base forum code is Copyright (C) 2012 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser, Non-Forum Related code is Copyright (C) " + Config.ForumTitle);
        Page.Header.Controls.Add(meta);

    }

} 
