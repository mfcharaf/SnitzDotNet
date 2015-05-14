using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;


namespace SnitzUI.Content.FAQ
{
    public partial class Faq : PageBase, IRoutablePage, ISiteMapResolver
    {
        private List<FaqCategoryInfo> _faqcats;

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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Page.Title = SiteMapLocalizations.FAQPageTitle;
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
            shTheme.Attributes.Add("href", "/css/" + Page.Theme + "/shThemeDefault.css");
            if (webResources.TextDirection == "rtl")
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faqrtl.css");
            }
            else
            {
                faqCSS.Attributes.Add("href", "/css/" + Page.Theme + "/faq.css");
            }
            BindFaqNav();
            ImageButton addTopic = (ImageButton)FaqNav.Controls[0].Controls[0].FindControl("addTopic");
            ImageButton manageCats = (ImageButton)FaqNav.Controls[0].Controls[0].FindControl("manageCats");
            addTopic.Visible = IsAuthenticated && (IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin"));
            manageCats.Visible = IsAuthenticated && (IsAdministrator || IsModerator || Roles.IsUserInRole("FaqAdmin"));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CheckUrlParams();
            if (Page.IsPostBack)
            {
                string postbackbtn = Request.Form["__EVENTTARGET"];
                string argument = Request.Form["__EVENTARGUMENT"];
                switch (postbackbtn)
                {
                    case "ctl00_CPM_btn":
                        //get round bug if selecting index 0
                        if (fcCategory.SelectedIndex == 0)
                            SelectCategory(sender, null);
                        break;
                }

            }
        }
        public SiteMapNode SiteMapResolve(object sender,SiteMapResolveEventArgs e)
        {
            string category = "";
            string question = "";

            SiteMapNode currentNode = null;
            if (SiteMap.CurrentNode == null)
            {
                var routable = e.Context.CurrentHandler as IRoutablePage;

                if (routable != null)
                {
                    var rc = routable.Routing.RequestContext;
                    var route = rc.RouteData.Route;
                    var segments = route.GetVirtualPath(rc, null).VirtualPath.Split('/');
                    var path = "~/" + string.Join("/", segments.Take(segments.Length - rc.RouteData.Values.Count).ToArray());
                    category = segments[1];
                    question = segments[2];
                    var findSiteMapNodeFromKey = SiteMap.Provider.FindSiteMapNodeFromKey(@"~\question");
                    if (findSiteMapNodeFromKey != null)
                        currentNode = findSiteMapNodeFromKey.Clone(true);
                }
            }
            if (SiteMap.CurrentNode != null)
            {
                currentNode = SiteMap.CurrentNode.Clone(true);
                currentNode.Url = @"~\Faq";
            }
            
            SiteMapNode tempNode = currentNode;
            if(category != "")
                tempNode.Title = category + " : " + question.Replace("_"," ");

            //tempNode = tempNode.ParentNode;
            ////tempNode.Title = question;
            //tempNode.Url = tempNode.Url;

            return currentNode;

        }

        private void CheckUrlParams()
        {
            string[] path = Request.Path.Split('/');
            if (path.Length == 4)
            {
                string category = path[2].Replace("_", " ");
                string question = path[3].Replace("_", " ");
                faqView.SetActiveView(vQuestion);

                var faq = SnitzFaq.GetQuestion(String.Format("/{0}/{1}", category, question), "").ToList();
                if (faq.Count() == 1 && faq[0] != null)
                {
                    fvQuestion.Text = string.Format("<h1>{0}?</h1>", question);
                    fvAnswer.Text = faq[0].LinkBody.ReplaceNoParseTags();
                    hdnFaqId.Value = faq[0].Id.ToString();
                }
                btnDeleteFaq.OnClientClick =
                    "confirmPostBack('Do you want to delete Question and answer?','Delete'," + hdnFaqId.Value + ");return false;";
                btnEdit.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");
                btnDeleteFaq.Visible = IsAdministrator || Roles.IsUserInRole("FAQEditor");
            }
            else
            {
                faqView.SetActiveView(vDefault);
                hdnFaqId.Value = "-1";
            }
        }

        private void BindFaqNav()
        {
            _faqcats = SnitzCachedLists.GetCachedHelpCategories();
            FaqNav.DataSource = _faqcats;
            FaqNav.DataBind();
            feCategory.DataSource = _faqcats;
            feCategory.DataBind();
            fcCategory.DataSource = _faqcats;
            fcCategory.DataBind();
            fcCategory.Items.Insert(0, "[New]");
            fnCategory.DataSource = _faqcats;
            fnCategory.DataBind();
        }

        protected void BindQuestions(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                FaqCategoryInfo cat = (FaqCategoryInfo)e.Item.DataItem;
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
                        if(!String.IsNullOrEmpty(Filter))
                            fqr.DataSource = SnitzFaq.FindFaqQuestion( Filter, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                        else
                            fqr.DataSource = SnitzFaq.GetFaqQuestionsByCategory(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                        fqr.DataBind();
                    }
                }
            }
        }

        protected void NewTopic(object sender, EventArgs e)
        {
            faqView.SetActiveView(vNew);
            fnTitle.Text = "";
            fnBody.Text = "";
            fnOrder.Text = "1";
            BindFaqNav();
        }

        protected void ManageCategories(object sender, EventArgs e)
        {
            fcTitle.Text = "";
            fcLang.Text = "en";
            fcOrder.Text = "99";
            fcRoles.Text = "";
            BindFaqNav();

            faqView.SetActiveView(vCategory);

        }

        protected void ToggleEdit(object sender, ImageClickEventArgs e)
        {
            faqView.SetActiveView(vEdit);
            
            string[] path = Request.Path.Split('/');

            if (path.Length == 4)
            {
                string category = path[2].Replace("_", " ");
                string question = path[3].Replace("_", " ");
                
                var faq = SnitzFaq.GetQuestion(String.Format("/{0}/{1}", category, question), "").ToList();
                if (faq.Count() == 1)
                {
                    feCategory.SelectedValue = faq[0].CatId.ToString();
                    feTitle.Text = question;
                    feOrder.Text = faq[0].Order.ToString();
                    feBody.Text = faq[0].LinkBody;
                    hdnFaqId.Value = faq[0].Id.ToString();
                }

            }
        }

        protected void SearchFaq(object sender, ImageClickEventArgs e)
        {
            TextBox searchfor = (TextBox)FaqNav.Controls[0].Controls[0].FindControl("searchFor");
            Filter = searchfor.Text;
            FaqNav.DataSource = null;
            BindFaqNav();
        }

        protected void Delete(object sender, ImageClickEventArgs e)
        {
            int id = Convert.ToInt32(hdnFaqId.Value);
            if (id > 0)
            {
                SnitzFaq.DeleteFaqQuestion(id);
                Response.Redirect("/Faq");
            }
        }

        protected void Save(object sender, ImageClickEventArgs e)
        {
            int id = Convert.ToInt32(hdnFaqId.Value);
            string category = feCategory.SelectedValue;
            string question = feTitle.Text;
            string answer = feBody.Text;
            int order = feOrder.Text == "" ? 0 : Convert.ToInt32(feOrder.Text);

            FaqInfo faq = SnitzFaq.GetFaqQuestion(id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            faq.Order = order;
            faq.CatId = Convert.ToInt32(category);
            faq.Link = String.Format("/{0}/{1}", feCategory.SelectedItem.Text, question);
            faq.LinkTitle = question;
            faq.LinkBody = answer;

            SnitzFaq.UpdateFaqQuestion(faq);

            Response.Redirect(this.Request.RawUrl);
        }

        protected void Add(object sender, ImageClickEventArgs e)
        {
            if (fnTitle.Text == "" || fnBody.Text == "")
                return;

            string category = fnCategory.SelectedValue;
            string question = fnTitle.Text;
            string answer = fnBody.Text;
            FaqInfo faq = new FaqInfo
            {
                CatId = Convert.ToInt32(category),
                Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                Link = String.Format("/{0}/{1}",fnCategory.SelectedItem.Text,question),
                LinkTitle = question,
                LinkBody = answer,
                Order = fnOrder.Text == "" ? 0 : Convert.ToInt32(fnOrder.Text)
            };
            SnitzFaq.AddFaqQuestion(faq);
            Response.Redirect(this.Request.RawUrl);
        }

        protected void SaveCat(object sender, ImageClickEventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(fcTitle.Text) ?? new FaqCategoryInfo();
            cat.Description = fcTitle.Text;
            cat.Order = Convert.ToInt32(fcOrder.Text);
            cat.Roles = fcRoles.Text;
            cat.Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            if (cat.Id > 0)
                SnitzFaq.UpdateFaqCategory(cat);
            else
                SnitzFaq.AddFaqCategory(cat);

            //refresh the category cache
            Cache.Remove("faqcatlist");
            Response.Redirect(this.Request.RawUrl);
        }

        protected void DeleteCat(object sender, ImageClickEventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(fcCategory.SelectedItem.Text);
            if(cat != null)
            {
                var questions = SnitzFaq.GetFaqQuestionsByCategory(cat.Id, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                if (!questions.Any())
                {
                    SnitzFaq.DeleteFaqCategory(cat);
                    //refresh the category cache
                    Cache.Remove("faqcatlist");
                    Response.Redirect(this.Request.RawUrl);
                }
            }
        }

        protected void SelectCategory(object sender, EventArgs e)
        {
            FaqCategoryInfo cat = SnitzFaq.GetCategory(fcCategory.SelectedItem.Text);

            if (cat != null)
            {
                fcTitle.Text = cat.Description;
                fcRoles.Text = cat.Roles;
                fcLang.Text = cat.Language;
                fcOrder.Text = cat.Order.ToString();
            }
            else
            {
                fcTitle.Text = "";
                fcLang.Text = "en";
                fcOrder.Text = "99";
                fcRoles.Text = ""; 
            }
        }

        protected void btn_Click(object sender, EventArgs e)
        {
            //dummy event for fcCategory ddl postbaack
        }

        public RoutingHelper Routing { get; set; }
    }
}