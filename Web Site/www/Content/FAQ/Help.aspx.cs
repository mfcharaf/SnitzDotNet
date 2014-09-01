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

        public int currentfaq
        {
            get
            {
                if (ViewState["currentfaq"] != null)
                    return (int)ViewState["currentfaq"];
                return -1;
            }
            set
            {
                //Filter = value;
                ViewState.Add("currentfaq", value);
            }
        }
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
            ViewState.Add("currentfaq", currentfaq);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            Page.Title = SiteMapLocalizations.FAQPageTitle;
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
            shTheme.Attributes.Add("href","/css/" + Page.Theme + "/shThemeDefault.css");
            if (webResources.TextDirection == "rtl")
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faqrtl.css");
            }
            else
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faq.css");
            }
            addTopic.Visible = IsAuthenticated && (IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin"));
            manageCats.Visible = IsAuthenticated && (IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin"));
            if (!IsPostBack)
            BindFaqNav();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                SetDefaultView();
            }
            if (Page.IsPostBack)
            {
                string postbackbtn = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];
                switch (postbackbtn)
                {
                    case "DeleteFaq":
                        int id = Convert.ToInt32(argument);
                        DeleteFaq(id);
                        break;
                    case "ctl00$CPM$ddlCategoryEdit" :
                        //get round bug if selecting index 0
                        if (ddlCategoryEdit.SelectedIndex == 0)
                            SelectCategory(sender, null);
                        break;
                }
                
            }

        }

        private void BindFaqNav()
        {
            var faqcats = SnitzCachedLists.GetCachedHelpCategories();
            FaqNav.DataSource = faqcats;
            FaqNav.DataBind();
            ddlCategoryEdit.DataSource = faqcats;
            ddlCategoryEdit.DataBind();
            ddlCategoryEdit.Items.Insert(0, "[New]");
            ddlCategory.DataSource = faqcats;
            ddlCategory.DataBind();
        }

        private void SetDefaultView()
        {
            FaqViews.ActiveViewIndex = 0;
        }

        protected void ManageCategories(object sender, EventArgs e)
        {
            if (!(IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin")))
                return;
           
            FaqViews.ActiveViewIndex = 2;
        }

        protected void NewTopic(object sender, EventArgs e)
        {
            if (!(IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin")))
                return;
            FaqViews.ActiveViewIndex = 1;
            tbxQuestion.Text = "";
            tbxAnswer.Text = "";
        }

        protected void SearchFaq(object sender, ImageClickEventArgs e)
        {
            Filter = searchFor.Text;
            FaqNav.DataSource = null;
            BindFaqNav();
        }

        protected void BindQuestions(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FaqCategoryInfo cat = (FaqCategoryInfo) e.Item.DataItem;
                if (!String.IsNullOrEmpty(cat.Roles) && !Roles.IsUserInRole(cat.Roles) && !IsAdministrator)
                {
                    e.Item.Visible = false;
                    return;
                }
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
            faqQuestion.Text = "<h1>" + faq.LinkTitle + "</h1>";
            faqAnswer.Text = faq.LinkBody.ReplaceNoParseTags().ParseVideoTags().ParseWebUrls();
            btnDeleteFaq.OnClientClick =
"confirmPostBack('Do you want to delete Question and answer?','DeleteFaq'," + faqId.Value + ");return false;";
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
            tbxQorder.Text = faq.Order.ToString();
            ddlCategory.SelectedValue = faq.CatId.ToString();
        }

        protected void SelectCategory(object sender, EventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(ddlCategoryEdit.SelectedItem.Text);
            

            if (cat != null)
            {
                if (currentfaq == cat.Id)
                {
                    return;
                }
                currentfaq = cat.Id;
                catDescription.Text = cat.Description;
                catLang.Text = cat.Language;
                catOrder.Text = cat.Order.ToString();
                catRole.Text = cat.Roles;
            }
            else
            {
                currentfaq = -1;
                catDescription.Text = "";
                catLang.Text = "en";
                catOrder.Text = "99";
                catRole.Text = "";                
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
            cat.Roles = catRole.Text;
            cat.Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if(cat.Id > 0)
                SnitzFaq.UpdateFaqCategory(cat);
            else
                SnitzFaq.AddFaqCategory(cat);
            SetDefaultView();
            //refresh the category cache
            Cache.Remove("faqcatlist");
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
                Order = tbxQorder.Text == "" ? 0 : Convert.ToInt32(tbxQorder.Text)
            };
            SnitzFaq.AddFaqQuestion(faq);
            Response.Redirect(this.Request.RawUrl);
        }
        private void SaveFAQ(object sender, EventArgs eventArgs)
        {
            int id = Convert.ToInt32(hdnEditFaq.Value);
            string category = ddlCategory.SelectedValue;
            string question = tbxQuestion.Text;
            string answer = tbxAnswer.Text;
            int order = tbxQorder.Text == "" ? 0 : Convert.ToInt32(tbxQorder.Text);

            FaqInfo faq = SnitzFaq.GetFaqQuestion(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faq.Order = order;
            faq.CatId = Convert.ToInt32(category);
            faq.LinkTitle = question;
            faq.LinkBody = answer;

            SnitzFaq.UpdateFaqQuestion(faq);

            Response.Redirect(this.Request.RawUrl);

        }

        protected void CategoryDataBound(object sender, EventArgs e)
        {
            
        }
    }
}