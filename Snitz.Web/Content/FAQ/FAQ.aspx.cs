/*
####################################################################################################################
##
## SnitzUI.Content.FAQ - FAQ.aspx
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
using System.Globalization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snitz.BLL;
using Snitz.Entities;
using SnitzUI.UserControls;
using Resources;
using SnitzCommon;
using SnitzConfig;


namespace SnitzUI
{
    public partial class FAQPage : PageBase
    {

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
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
            if (webResources.TextDirection == "rtl")
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faqrtl.css");
            }
            else
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faq.css");
            }

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
                        pnlRead.Visible = false;
                        pnlEdit.Visible = false;
                        Category.Visible = true;
                        ddlCategoryEdit.DataSource = SnitzFaq.GetFaqCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                        ddlCategoryEdit.DataBind();
                        LinkButton submit = new LinkButton { ID = "newCat" };
                        submit.Click += AddNewCategory;
                        submit.Text = Resources.webResources.btnSubmit;

                        MainMaster m = (MainMaster)Master;
                        if (m != null)
                            m.rootScriptManager.RegisterAsyncPostBackControl(submit);
                        PlaceHolder1.Controls.Add(submit);

                    }
                }
            }
            ddlCategory.DataSource = SnitzFaq.GetFaqCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            ddlCategory.DataBind();
        }

        private void BindFaqNav()
        {

            FaqNav.DataSource = SnitzFaq.GetFaqCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
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
                        fqr.DataSource = SnitzFaq.GetQuestions(id, Filter, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
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
            FaqInfo faq = SnitzFaq.GetFaqQuestion(question, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqQuestion.Text = faq.LinkTitle;
            faqAnswer.Text = faq.LinkBody.ParseTags();
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
            FaqInfo faq = SnitzFaq.GetFaqQuestion(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqId.Value = id.ToString();
            tbxQuestion.Text = faq.LinkTitle;
            tbxAnswer.Text = faq.LinkBody;
        }

        private void SaveFAQ(object sender, EventArgs eventArgs)
        {
            int id = Convert.ToInt32(faqId.Value);

            SnitzFaq.UpdateFaqQuestion(id, tbxQuestion.Text, tbxAnswer.Text, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            pnlRead.Visible = true;
            pnlEdit.Visible = false;

            FaqInfo faq = SnitzFaq.GetFaqQuestion(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqQuestion.Text = faq.LinkTitle;
            faqAnswer.Text = faq.LinkBody.ParseTags();

        }

        protected void DeleteFaq(object sender, ImageClickEventArgs e)
        {
            int question = Convert.ToInt32(faqId.Value);
            SnitzFaq.DeleteFaqQuestion(question);
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
                                  LinkTitle = question,
                                  LinkBody = answer,
                                  Order = 99
                              };
            SnitzFaq.AddFaqQuestion(faq);
            UpdatePanel1.Update();
        }

        protected void ManageCategories(object sender, EventArgs e)
        {

        }

        private void AddNewCategory(object sender, EventArgs e)
        {
            FaqCategoryInfo cat;

            cat = SnitzFaq.GetCategory(catDescription.Text);
            if(cat == null)
                cat = new FaqCategoryInfo();
            cat.Description = catDescription.Text;
            cat.Order = Convert.ToInt32(catOrder.Text);

            cat.Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            SnitzFaq.AddFaqCategory(cat);
        }

        protected void SelectCategory(object sender, EventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(ddlCategoryEdit.SelectedItem.Text);
            if (cat != null)
            {
                catDescription.Text = cat.Description;
                catLang.Text = cat.Language;
                catOrder.Text = cat.Order.ToString();
            }
        }
    }
}