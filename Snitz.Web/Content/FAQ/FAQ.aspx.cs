#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
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
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzUI.UserControls;
using Resources;
using SnitzCommon;
using SnitzConfig;
using SnitzData;

namespace SnitzUI
{
    public partial class FAQPage : PageBase
    {
        private FaqUtil _faqUtil;
        public string Filter
        {
            get
            {
                if (ViewState["faqFilter"] != null)
                    return ViewState["faqFilter"].ToString();
                return null;
            }
            set
            {
                //Filter = value;
                ViewState.Add("faqFilter", value);
            }
        }

        protected override void Page_PreRender(object sender, EventArgs e)
        {
            base.Page_PreRender(sender,e);
            // Save PageArrayList before the page is rendered.
            ViewState.Add("faqFilter", Filter);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.Title = SiteMapLocalizations.FAQPageTitle;
            markitupCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/markitup.css");
            if (webResources.TextDirection == "rtl")
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/faqrtl.css");
            }
            else
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.StyleSheetTheme + "/faq.css");
            }

            _faqUtil = new FaqUtil();
            if (IsPostBack)
            {
                if (!String.IsNullOrEmpty(Request.Form[customPostBack.UniqueID]))
                {
                    string CtrlID = Request.Form[customPostBack.UniqueID];
                    if (CtrlID.EndsWith("addTopic"))
                    {
                        LinkButton submit = new LinkButton { ID = "newQ" };
                        submit.Click += AddNewQuestion;
                        submit.Text = Resources.webResources.btnSubmit;
                        
                        MainMaster m = (MainMaster)Master;
                        if (m != null)
                            m.rootScriptManager.RegisterAsyncPostBackControl(submit);
                        phSubmit.Controls.Add(submit);

                    }
                    else if (CtrlID.EndsWith("btnEdit"))
                    {
                        LinkButton submit = new LinkButton { ID = "saveQ" };
                        submit.Click += SaveFAQ;
                        submit.Text = Resources.webResources.btnUpdate;

                        MainMaster m = (MainMaster)Master;
                        if (m != null)
                            m.rootScriptManager.RegisterAsyncPostBackControl(submit);
                        phSubmit.Controls.Add(submit);

                    }
                    else if (CtrlID.EndsWith("manageCats"))
                    {
                        LinkButton submit = new LinkButton { ID = "newCat" };
                        submit.Click += AddNewCategory;
                        submit.Text = Resources.webResources.btnSubmit;

                        MainMaster m = (MainMaster)Master;
                        if (m != null)
                            m.rootScriptManager.RegisterAsyncPostBackControl(submit);
                        phSubmit.Controls.Add(submit);

                    }
                }
            }
            ddlCategory.DataSource = _faqUtil.GetCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            ddlCategory.DataBind();
        }

        private void BindFaqNav()
        {
            
            FaqNav.DataSource = _faqUtil.GetCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            FaqNav.DataBind();

        }

        protected void Page_Load(object sender, EventArgs e)
        {
                BindFaqNav();
                this.PageScriptManager.RegisterAsyncPostBackControl(addTopic);
        }

        protected void BindQuestions(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int id = 0;
                if (e.Item.FindControl("hdnCatId") != null)
                    id = Convert.ToInt32(((HiddenField)e.Item.FindControl("hdnCatId")).Value);
                if (e.Item.FindControl("FaqQuestions") != null)
                {
                    Repeater fqr = ((Repeater)e.Item.FindControl("FaqQuestions"));
                    if(fqr != null)
                    {
                        fqr.DataSource = _faqUtil.GetFaqQuestions(id, Filter, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                        fqr.DataBind();
                    }
                }
            }
        }

        protected void ViewAnswer(object source, RepeaterCommandEventArgs e)
        {
            pnlRead.Visible = true;
            pnlEdit.Visible = false;
            int question =  Convert.ToInt32(e.CommandArgument);
            faqId.Value = question.ToString();
            FaqInfo faq = _faqUtil.GetAnswer(question, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqQuestion.Text = faq.Question;
            faqAnswer.Text = faq.Answer.ParseTags();
            btnEdit.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");
            btnDelete.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");

        }

        protected void SearchFaq(object sender, ImageClickEventArgs e)
        {
            Filter = searchFor.Text;
            BindFaqNav();
        }

        protected void ToggleEdit(object sender, ImageClickEventArgs e)
        {
            int id = Convert.ToInt32(faqId.Value);

            pnlRead.Visible = false;
            pnlEdit.Visible = true;
            FaqInfo faq = _faqUtil.GetAnswer(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqId.Value = id.ToString();
            tbxQuestion.Text = faq.Question;
            tbxAnswer.Text = faq.Answer;
        }

        private void SaveFAQ(object sender, EventArgs eventArgs)
        {
            int id = Convert.ToInt32(faqId.Value);

            _faqUtil.SaveFaq(id, tbxQuestion.Text, tbxAnswer.Text,CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            pnlRead.Visible = true;
            pnlEdit.Visible = false;
            
            FaqInfo faq = _faqUtil.GetAnswer(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqQuestion.Text = faq.Question;
            faqAnswer.Text = faq.Answer.ParseTags();

        }

        protected void DeleteFaq(object sender, ImageClickEventArgs e)
        {
            int question = Convert.ToInt32(faqId.Value);
            _faqUtil.Delete(question);
            BindFaqNav();
        }

        protected void NewTopic(object sender, EventArgs eventArgs)
        {
            pnlRead.Visible = false;
            pnlEdit.Visible = true;
            pnlCategory.Visible = true;

            tbxQuestion.Text = "";
            tbxAnswer.Text = "";
        }

        private void AddNewQuestion(object sender, EventArgs eventArgs)
        {
            if (tbxQuestion.Text == "" || tbxAnswer.Text == "")
                return;
            string category = ddlCategory.SelectedValue;
            string question = tbxQuestion.Text;
            string answer = tbxAnswer.Text;
            FaqInfo faq = new FaqInfo
                              {
                                  CatId = Convert.ToInt32(category),
                                  Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                                  Question = question,
                                  Answer = answer,
                                  Order = 99
                              };
            _faqUtil.AddFaq(faq);
        }

        protected void ManageCategories(object sender, EventArgs e)
        {
            pnlRead.Visible = false;
            pnlEdit.Visible = false;
            Category.Visible = true;
            DropDownList1.DataSource = _faqUtil.GetCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            DropDownList1.DataBind();

            LinkButton submit = new LinkButton { ID = "newCat" };
            submit.Click += AddNewCategory;
            submit.Text = Resources.webResources.btnSubmit;

            MainMaster m = (MainMaster)Master;
            if (m != null)
                m.rootScriptManager.RegisterAsyncPostBackControl(submit);
            PlaceHolder1.Controls.Add(submit);
        }

        private void AddNewCategory(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}