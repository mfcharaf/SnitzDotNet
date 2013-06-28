/*'
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
using System;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzCommon;
using SnitzConfig;

public partial class User_Controls_MemberSearch : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        cbxEmail.Visible = Roles.IsUserInRole("Administrator");
        CreateHyperlinks();
    }
    public event EventHandler InitialLinkClick;
    private void CreateHyperlinks()
    {
        PageBase page = (PageBase) Page;

        string[] letters = new string[] { "", "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
        int id = 1;
        foreach (string letter in letters)
        {
            LinkButton lnk = new LinkButton
                                 {
                                     ID = "lnk" + id++,
                                     Text = letter == "" ? "All" : letter,
                                     CssClass = "initialLnk",
                                     CausesValidation = false,
                                     CommandArgument = letter,
                                     EnableViewState = false,
                                     CommandName = "initial"
                                 };
            //lnk.CssClass = "ForumLnk";
            lnk.Click += InitialClick;
            
            plcLink.Controls.Add(lnk);
            plcLink.Controls.Add(new Literal { Text = @"&nbsp;" });

            //AsyncPostBackTrigger asyncPostBackTrigger = new AsyncPostBackTrigger();
            //asyncPostBackTrigger.ControlID = lnk.ClientID;
            //asyncPostBackTrigger.EventName = "Click";
            page.PageScriptManager.RegisterAsyncPostBackControl(lnk);

        }
    }

    private void InitialClick(object sender, EventArgs e)
    {
        if (this.InitialLinkClick != null)
        {
            this.InitialLinkClick(sender, e);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        StringBuilder SearchFilter = new StringBuilder("");

        if (Session["SearchFilter"] != null)
            Session.Remove("SearchFilter");

        if (cbxCountry.Checked)
        {
            SearchFilter.Append(String.Format("Country.Contains(\"{0}\" )", tbxSearchFor.Text));
        }
        if (cbxEmail.Checked)
        {
            SearchFilter.Append(Config.Encrypt
                                    ? String.Format("Email.Contains(\"{0}\")", SnitzData.Util.Encrypt(tbxSearchFor.Text))
                                    : String.Format("Email.Contains(\"{0}\")", tbxSearchFor.Text));
        }
        else
        {

            if (cbxUserName.Checked)
            {
                if (cbxCountry.Checked)
                    SearchFilter.Append(" or ");
                SearchFilter.Append(String.Format("Name.Contains(\"{0}\")", tbxSearchFor.Text));
            }
            if (cbxFirstName.Checked)
            {
                if (cbxUserName.Checked || cbxCountry.Checked)
                    SearchFilter.Append(" or ");
                SearchFilter.Append(String.Format("FirstName.Contains(\"{0}\")", tbxSearchFor.Text));
            }
            if (cbxLastName.Checked)
            {
                if (cbxUserName.Checked || cbxFirstName.Checked || cbxCountry.Checked)
                    SearchFilter.Append(" or ");
                SearchFilter.Append(String.Format("LastName.Contains(\"{0}\")", tbxSearchFor.Text));
            }
        }

        Session.Add("SearchFilter", SearchFilter.ToString());

    }
}
