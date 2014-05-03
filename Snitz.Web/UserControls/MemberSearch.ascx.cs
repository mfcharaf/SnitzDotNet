/*
####################################################################################################################
##
## SnitzUI.UserControls - MemberSearch.ascx
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
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
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
    public event EventHandler SearchClick;

    private void CreateHyperlinks()
    {
        PageBase page = (PageBase) Page;

        string[] letters = { "", "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z" };
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
        if (InitialLinkClick != null)
        {
            InitialLinkClick(sender, e);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        StringBuilder SearchFilter = new StringBuilder("");

        if (Session["SearchFilter"] != null)
            Session.Remove("SearchFilter");

        if (cbxCountry.Checked)
        {
            SearchFilter.Append(String.Format("Country,{0}", tbxSearchFor.Text));
        }
        if (cbxEmail.Checked)
        {
            SearchFilter.Append(Config.Encrypt
                                    ? String.Format("Email,{0}", Admin.Encrypt(tbxSearchFor.Text))
                                    : String.Format("Email,{0}", tbxSearchFor.Text));
        }
        else
        {

            if (cbxUserName.Checked)
            {
                if (cbxCountry.Checked)
                    SearchFilter.Append(" OR ");
                SearchFilter.Append(String.Format("Name,{0}", tbxSearchFor.Text));
            }
            if (cbxFirstName.Checked)
            {
                if (cbxUserName.Checked || cbxCountry.Checked)
                    SearchFilter.Append(" OR ");
                SearchFilter.Append(String.Format("FirstName,{0}", tbxSearchFor.Text));
            }
            if (cbxLastName.Checked)
            {
                if (cbxUserName.Checked || cbxFirstName.Checked || cbxCountry.Checked)
                    SearchFilter.Append(" OR ");
                SearchFilter.Append(String.Format("LastName,{0}", tbxSearchFor.Text));
            }
        }

        Session.Add("SearchFilter", SearchFilter.ToString());
        if (SearchClick != null)
        {
            SearchClick(sender, e);
        }
    }
}
