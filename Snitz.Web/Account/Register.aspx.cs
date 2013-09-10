/*
####################################################################################################################
##
## SnitzUI.Account - Register.aspx
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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Profile;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Snitz.BLL;
using SnitzUI.UserControls;
using Resources;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;


namespace SnitzUI
{
    public partial class RegisterPage : PageBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.Title = SiteMapLocalizations.RegisterPageTitle;
            pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/regwizard.css");
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Config.ProhibitNewMembers) { Response.Redirect("~/default.aspx"); }
            //set visibility and data requirements
            SetVisiblity();
            //load the forum policy file
            var file = new StreamReader(Server.MapPath(Config.CultureSpecificDataDirectory + "policy.txt"));
            string content = file.ReadToEnd();
            policy.Text = content.Replace("[MinAge]", Config.MinAge.ToString());
            //Set the newly created user disabled until they validate their email
            CreateUserWizard1.DisableCreatedUser = Config.RestrictRegistration || Config.EmailValidation;
            if(Config.EmailValidation)
                CreateUserWizard1.CompleteSuccessText = webResources.EmailValMessage;
            var ltl = (Literal)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("domain");
            string[] emailDom = Config.AdminEmail.Split('@');
            ltl.Text = emailDom[1];
            if (!Page.IsPostBack)
            {
                CreateUserWizard1.MailDefinition.From = Config.AdminEmail;
                if(Config.EmailValidation)
                    CreateUserWizard1.MailDefinition.BodyFileName = Config.CultureSpecificDataDirectory + "ValidationMail.html";
                else
                    CreateUserWizard1.MailDefinition.BodyFileName = Config.CultureSpecificDataDirectory +"RegisterMail.html";
                CreateUserWizard1.MailDefinition.Subject = "Welcome";

                var smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
                //we set the .Net password question to a random string for use as the activation key.
                if (smp != null) CreateUserWizard1.Question = smp.GeneratePassword(12);
                BindCountry();
            }

        }

        #region CreateUserWizard Methods

        protected void CreateUserWizard1CreatingUser(object sender, LoginCancelEventArgs e)
        {
            e.Cancel = false; 

            //generate a random password and update the hiden password control
            var smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
            var tb = (TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Password");
            if (smp != null) tb.Text = smp.GeneratePassword(12);

            //check UserName filter
            if (Config.FilterUsernames)
            {
                e.Cancel = e.Cancel || !IsNameAllowed(CreateUserWizard1.UserName);
            }

            if (Config.UseSpamService)
                if (!StopForumSpamCheck(3))
                {
                    var ltl = (Literal)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Literal1");
                    ltl.Text = extras.ErrSpamCheck;
                    e.Cancel = true;
                }
            //check wwe are the requird age
            if(Config.MinAge > 0)
            {
                var dp = (DatePicker)CreateUserWizard1.WizardSteps[1].FindControl("DatePicker1");

                 string dateOfBirth = dp.DOBStr;
                 int age = Convert.ToInt32(Common.GetAgeFromDOB(dateOfBirth));
                 if (age < Config.MinAge)
                     e.Cancel = true;
            }
            //check the visible captcha if used, if it fails cancel registration
            var ct = (SnitzCaptchaControl)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("CaptchaControl1");
            if (ct != null)
                if (!ct.IsValid)
                    e.Cancel = true;
        }

        protected void CreateUserWizard1SendingMail(object sender, MailMessageEventArgs e)
        {
            //Send the new member his password and security key
            //Message Body is in /App_Data/RegisterMail.txt
            var mp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
            //string msg;
            var cuw = (CreateUserWizard)sender;
            //string validationCode = cuw.Question;
            //Get the MembershipUser that we just created.
            var newUser = (SnitzMembershipUser)Membership.GetUser(cuw.UserName);
            //Create the validation code
            if (mp != null)
            {
                string validationCode = SnitzMembershipProvider.CreateValidationCode(newUser);
                mp.ChangePasswordQuestionAndAnswer(cuw.UserName, "Registration", validationCode, "validationcode");
                //And build the url for the validation page. 
                var builder = new UriBuilder("http",
                                                    Request.Url.DnsSafeHost,
                                                    Request.Url.Port, Page.ResolveUrl("~/Account/activate.aspx"), "?C=" + validationCode);
                //Add the values to the mail message. 
                e.Message.Body = e.Message.Body.Replace("<%OurForum%>", Config.ForumTitle);
                e.Message.Body = e.Message.Body.Replace("<%activationURL%>", builder.Uri.ToString().Replace(" ", "%20"));
                e.Message.Body = e.Message.Body.Replace("<%activationKey%>", validationCode);
            }

        }

        protected void CreateUserWizard1CreatedUser(object sender, EventArgs e)
        {
            var newMember = Members.GetMember(CreateUserWizard1.UserName);
            string[] fname = ((TextBox)CreateUserWizard1.WizardSteps[1].FindControl("fullname")).Text.Split(' ');

            newMember.Firstname = fname[0];
            newMember.Lastname = "";
            if (fname.Length > 1)
            {
                for (int i = 1; i < fname.Length; i++)
                {
                    newMember.Lastname += fname[i] + " ";
                }
                
            }
            newMember.City = ((TextBox)CreateUserWizard1.WizardSteps[1].FindControl("city")).Text;
            newMember.State = ((TextBox)CreateUserWizard1.WizardSteps[1].FindControl("state")).Text;
            newMember.Country = ((DropDownList)CreateUserWizard1.WizardSteps[1].FindControl("Country")).SelectedValue;
            newMember.Gender = ((DropDownList)CreateUserWizard1.WizardSteps[1].FindControl("Gender")).SelectedValue;
            
            var dp = (DatePicker)CreateUserWizard1.WizardSteps[1].FindControl("DatePicker1");
            newMember.DateOfBirth = dp.DOBStr;
            newMember.Age = Common.GetAgeFromDOB(newMember.DateOfBirth);

            newMember.Occupation = ((TextBox)CreateUserWizard1.WizardSteps[2].FindControl("occupation")).Text;
            newMember.Biography = ((TextBox)CreateUserWizard1.WizardSteps[2].FindControl("biography")).Text;
            newMember.Hobbies = ((TextBox)CreateUserWizard1.WizardSteps[2].FindControl("hobbies")).Text;
            newMember.LatestNews = ((TextBox)CreateUserWizard1.WizardSteps[2].FindControl("lnews")).Text;
            newMember.FavouriteQuote = ((TextBox)CreateUserWizard1.WizardSteps[2].FindControl("quote")).Text;

            newMember.ViewSignatures = bool.Parse(((RadioButtonList)CreateUserWizard1.WizardSteps[3].FindControl("viewsig")).SelectedValue);
            newMember.UseSignature = bool.Parse(((RadioButtonList)CreateUserWizard1.WizardSteps[3].FindControl("usesig")).SelectedValue);
            newMember.ReceiveEmails = bool.Parse(((RadioButtonList)CreateUserWizard1.WizardSteps[3].FindControl("recemail")).SelectedValue);

            newMember.Signature = ((TextBox)CreateUserWizard1.WizardSteps[3].FindControl("signature")).Text;
            newMember.MembersIP = Common.GetIPAddress();
            newMember.LastIP = newMember.MembersIP;
            // Save the profile - must be done since we explicitly created this profile instance
            Members.SaveMember(newMember);

            Membership.GetUser(newMember.UseSignature, true);
        }

        protected void CreateUserWizard1CreateUserError(object sender, CreateUserErrorEventArgs e)
        {
            string sMessage = "";
            switch (e.CreateUserError)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    sMessage = CreateUserWizard1.DuplicateEmailErrorMessage;
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    sMessage = CreateUserWizard1.DuplicateUserNameErrorMessage;
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    sMessage = CreateUserWizard1.InvalidAnswerErrorMessage;
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    sMessage = CreateUserWizard1.InvalidEmailErrorMessage;
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    sMessage = string.Format(CreateUserWizard1.InvalidPasswordErrorMessage, Membership.Provider.MinRequiredPasswordLength, Membership.Provider.MinRequiredNonAlphanumericCharacters);
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    sMessage = CreateUserWizard1.InvalidQuestionErrorMessage;
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    sMessage = "Username is not valid";
                    break;
                case MembershipCreateStatus.ProviderError:
                    sMessage = "Something went wrong in the provider";
                    break;
                case MembershipCreateStatus.UserRejected:
                    sMessage = "Administrator said NO WAY!";
                    break;
            }

            var phStuff = (PlaceHolder) CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("phCustomStuff");
            var cvError = new CustomValidator
                                          {
                                              ValidationGroup = "CreateUserWizard1",
                                              ErrorMessage = sMessage,
                                              IsValid = false
                                          };
            phStuff.Controls.Add(cvError);

        }
        
        #endregion

        #region Validation Methods

        private bool StopForumSpamCheck(int faillimit)
        {
            const string stopforumspamurl = "http://www.stopforumspam.com/api";
            string email = "email=" + CreateUserWizard1.Email;
            string username = "username=" + CreateUserWizard1.UserName;
            string memberIP = "ip=" + Common.GetIPAddress();

            var client = new WebClient();
            byte[] data = client.DownloadData(string.Format("{0}?{1}&{2}&{3}", stopforumspamurl, email, username, memberIP));
            string strPageData = Encoding.ASCII.GetString(data);
            var xml = new XmlDocument();
            xml.LoadXml(strPageData);
            XmlNodeList nodelist = xml.SelectNodes("/response/appears");
            int failcount = 0;
            if (nodelist != null)
                failcount = nodelist.Cast<XmlNode>().Count(node => node.InnerText == "yes");
            return (failcount < faillimit);
        }

        private static bool IsNameAllowed(string username)
        {

            if (Filters.GetAllNameFilters().Any(name => name.Name == username))
            {
                return false;
            }
            return username.ReplaceBadWords() == username;
        }

        private void SetVisiblity()
        {
            SettingsPropertyCollection spc = ProfileBase.Properties;

            if(Config.MinAge > 0)
            {
                var rfv = (RequiredFieldValidator)CreateUserWizard1.WizardSteps[1].FindControl("RFV_Bio_DOB");
                rfv.Enabled = true;
                var dp = (UserControl)CreateUserWizard1.WizardSteps[1].FindControl("DatePicker1");
                var lbl = (Label)dp.FindControl("lbl_Bio_DOB");
                lbl.CssClass = rfv.Enabled ? "label_mandatory" : "label_not_mandatory";
            }

            foreach (SettingsProperty prop in spc)
            {
                var persistenceData = prop.Attributes["CustomProviderData"] as string;
                // If we can't find the table/column info we will ignore this data
                if (String.IsNullOrEmpty(persistenceData))
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                string[] chunk = persistenceData.Split(new char[] { ';' });
                if (chunk.Length < 3)
                {
                    // REVIEW: Perhaps we should throw instead?
                    continue;
                }
                //prop.Attributes.Values;
                var rfv = (RequiredFieldValidator)CreateUserWizard1.WizardSteps[1].FindControl("RFV_" + prop.Name.Replace(".", "_"));
                var lbl = (Label)CreateUserWizard1.WizardSteps[1].FindControl("lbl_" + prop.Name.Replace(".", "_"));
                if (rfv != null)
                {
                    rfv.Enabled = (chunk[0] == "1");
                    if(lbl != null)
                    lbl.CssClass = rfv.Enabled ? "label_mandatory" : "label_not_mandatory";
                }
            }
        }

        private void BindCountry()
        {
            var doc = new XmlDocument();
            doc.Load(Server.MapPath(Config.CultureSpecificDataDirectory + "countries.xml"));
            var xmlNodeList = doc.SelectNodes("//country");
            if (xmlNodeList != null)
                foreach (XmlNode node in xmlNodeList)
                {
                    Country.Items.Add(new ListItem(node.InnerText, node.InnerText));
                }
        }

        #endregion

        #region Page methods for Ajax Name and Email checks

        [WebMethod]
        public static bool CheckUserName(string userName)
        {
            System.Threading.Thread.Sleep(2000);
            if ((Membership.GetUser(userName) != null))
            {
                return true;
            }
            if (!IsNameAllowed(userName))
                return true;
            return false;
        }

        [WebMethod]
        public static bool CheckEmail(string email)
        {
            System.Threading.Thread.Sleep(2000);
            if (!String.IsNullOrEmpty(Membership.GetUserNameByEmail(email)))
            {
                return true;
            }
            return false;
        } 

        #endregion
    }
}