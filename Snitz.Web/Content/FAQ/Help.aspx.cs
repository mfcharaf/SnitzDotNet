/*
####################################################################################################################
##
## SnitzUI - help.aspx
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
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;

namespace SnitzUI.Content.FAQ
{
    public partial class Help : PageBase
    {
        private int currentfaq = -1;
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
            base.Page_PreRender(sender, e);
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
            if (!IsPostBack)
            {
                SetDefaultView();
            }
            if (Page.IsPostBack)
            {
                string postbackbtn = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];
                int id;
                switch (postbackbtn)
                {
                    case "DeleteFaq":
                    id = Convert.ToInt32(argument);
                    DeleteFaq(id);
                        break;
                }
            }
        }

        private void SetDefaultView()
        {
            FaqViews.ActiveViewIndex = 0;
        }

        protected void ManageCategories(object sender, EventArgs e)
        {
            ddlCategoryEdit.DataSource = SnitzFaq.GetFaqCategories(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            ddlCategoryEdit.DataBind();
            FaqViews.ActiveViewIndex = 2;
        }

        protected void NewTopic(object sender, EventArgs e)
        {
            FaqViews.ActiveViewIndex = 1;
            tbxQuestion.Text = "";
            tbxAnswer.Text = "";
        }

        protected void SearchFaq(object sender, ImageClickEventArgs e)
        {
            Filter = searchFor.Text;
            BindFaqNav();
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
                    if (fqr != null)
                    {
                        fqr.DataSource = SnitzFaq.GetQuestions(id, Filter, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                        fqr.DataBind();
                    }
                }
            }
        }

        protected void ViewAnswer(object source, RepeaterCommandEventArgs e)
        {
            int question = Convert.ToInt32(e.CommandArgument);
            faqId.Value = question.ToString();
            FaqInfo faq = SnitzFaq.GetFaqQuestion(question, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqQuestion.Text = faq.LinkTitle;
            faqAnswer.Text = faq.LinkBody.ParseTags();
            btnDeleteFaq.OnClientClick =
"setArgAndPostBack('Do you want to delete QUestion and answer?','DeleteFaq'," + faqId.Value + ");return false;";
            btnEdit.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");
            btnDeleteFaq.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");
        }

        private void DeleteFaq(int id)
        {
            int question = id;
            SnitzFaq.DeleteFaqQuestion(question);
            BindFaqNav();
        }

        protected void ToggleEdit(object sender, ImageClickEventArgs e)
        {
            int id = Convert.ToInt32(faqId.Value);

            FaqInfo faq = SnitzFaq.GetFaqQuestion(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faqId.Value = id.ToString();
            tbxQuestion.Text = faq.LinkTitle;
            tbxAnswer.Text = faq.LinkBody;
            FaqViews.ActiveViewIndex = 1;
            hdnEditFaq.Value = id.ToString();
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

        protected void CancelEdit(object sender, EventArgs e)
        {
            SetDefaultView();
        }


        protected void AddNewCategory(object sender, EventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(catDescription.Text) ?? new FaqCategoryInfo();
            cat.Description = catDescription.Text;
            cat.Order = Convert.ToInt32(catOrder.Text);

            cat.Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            SnitzFaq.AddFaqCategory(cat);
            SetDefaultView();
            Response.Redirect(this.Request.RawUrl);
        }
        protected void AddNewQuestion(object sender, EventArgs e)
        {
            if (tbxQuestion.Text == "" || tbxAnswer.Text == "")
                return;
            if (hdnEditFaq.Value != "")
            {
                currentfaq = Convert.ToInt32(hdnEditFaq.Value);
                if (currentfaq > 0)
                    SaveFAQ(sender, e);
            }

            string category = ddlCategory.SelectedValue;
            string question = tbxQuestion.Text;
            string answer = tbxAnswer.Text;
            FaqInfo faq = new FaqInfo
            {
                CatId = Convert.ToInt32(category),
                Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                Link = String.Empty,
                LinkTitle = question,
                LinkBody = answer,
                Order = 99
            };
            SnitzFaq.AddFaqQuestion(faq);
            Response.Redirect(this.Request.RawUrl);
        }
        private void SaveFAQ(object sender, EventArgs eventArgs)
        {
            int id = Convert.ToInt32(hdnEditFaq.Value);

            SnitzFaq.UpdateFaqQuestion(id, tbxQuestion.Text, tbxAnswer.Text, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

            Response.Redirect(this.Request.RawUrl);

        }
    }
}