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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using AjaxControlToolkit;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon.Controls;
using Resources;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;
using MemberInfo = Snitz.Entities.MemberInfo;


namespace SnitzUI
{
    public partial class RegisterPage : PageBase
    {
        private string _regsettings;
        private XDocument _memberFields;

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
            PasswordStrength passwordStrength = (PasswordStrength)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Password_PasswordStrength");
            if (passwordStrength != null)
            {
                passwordStrength.PreferredPasswordLength = Config.PreferredPasswordLength;
                passwordStrength.MinimumNumericCharacters = Config.MinimumNumericCharacters;
                passwordStrength.MinimumSymbolCharacters = Config.MinimumSymbolCharacters;
                if (Config.RequiresUpperAndLowerCaseCharacters)
                {
                    passwordStrength.RequiresUpperAndLowerCaseCharacters = Config.RequiresUpperAndLowerCaseCharacters;
                    passwordStrength.MinimumUpperCaseCharacters = Config.MinimumUpperCaseCharacters;
                    passwordStrength.MinimumLowerCaseCharacters = Config.MinimumLowerCaseCharacters;
                }
            }
            string[] emailDom = Config.AdminEmail.Split('@');
            ltl.Text = emailDom[1];
            if (!Page.IsPostBack)
            {
                var smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
                //we set the .Net password question to a random string for use as the activation key.
                if (smp != null) CreateUserWizard1.Question = smp.GeneratePassword(12);
            }
            CreateUserWizard1.MailDefinition.From = Config.AdminEmail;
            if (Config.EmailValidation)
                CreateUserWizard1.MailDefinition.BodyFileName = Config.CultureSpecificDataDirectory + "ValidationMail.html";
            else
                CreateUserWizard1.MailDefinition.BodyFileName = Config.CultureSpecificDataDirectory + "RegisterMail.html";

        }

        #region CreateUserWizard Methods

        protected void CreateUserWizard1CreatingUser(object sender, LoginCancelEventArgs e)
        {
            e.Cancel = false; 

            
            var smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
            var tb = (TextBox)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("Password");
            //generate a random password and update the hiden password control
            if (smp != null && String.IsNullOrEmpty(tb.Text)) tb.Text = smp.GeneratePassword(12);

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
            if (Config.MinAge > 0 && !e.Cancel)
            {
                e.Cancel = !IsMinAge();
            }
            //check the visible captcha if used, if it fails cancel registration
            if (!e.Cancel)
            {
                var ct = (SnitzCaptchaControl)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("CaptchaControl1");
                if (ct != null)
                    if (!ct.IsValid)
                        e.Cancel = true;                
            }

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
                mp.ChangePasswordQuestionAndAnswer(cuw.UserName, cuw.Password, validationCode, "validationcode");
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

            XElement root = _memberFields.Root;
            var properties = RegHelper.GetProperties(newMember);
            var fields = root.Elements().Where(a => a.Attribute("Show").Value == "true" && a.Attribute("Group").Value == "Member");
            foreach (XElement field in fields)
            {
                var prop = properties.SingleOrDefault(p => p.Name == field.Name);
                Registration attr = (Registration)prop.GetCustomAttributes(false)[0];
                switch (attr.Control)
                {
                    case "Text" :
                    case "LongText":
                        var tfinder = new ControlFinder<TextBox>();
                        tfinder.FindChildControlsRecursive(CreateUserWizard1.CreateUserStep.ContentTemplateContainer);
                        var tinfo = tfinder.FoundControls.ToList();
                        
                        if (tinfo.Any(t => t.ID == field.Name))
                            prop.SetValue(newMember, Convert.ChangeType(tinfo.Single(t => t.ID == field.Name).Text, prop.PropertyType), null);
                        break;
                    case "DatePicker" :
                        var dfinder = new ControlFinder<DobPicker>();
                        dfinder.FindChildControlsRecursive(CreateUserWizard1.CreateUserStep.ContentTemplateContainer);
                        var dinfo = dfinder.FoundControls.ToList();
                        if (dinfo.Any(t => t.ID == field.Name))
                        {
                            newMember.DateOfBirth = dinfo.Single(t => t.ID == field.Name).DOBStr;
                            newMember.Age = Common.GetAgeFromDOB(newMember.DateOfBirth);
                        }                        
                        break;
                    case "GenderLookup" :
                        var cfinder = new ControlFinder<GenderDropDownList>();
                        cfinder.FindChildControlsRecursive(CreateUserWizard1.CreateUserStep.ContentTemplateContainer);
                        var cinfo = cfinder.FoundControls.ToList();
                        if (cinfo.Any(t => t.ID == field.Name))
                            newMember.Gender = cinfo.Single(t => t.ID == field.Name).SelectedValue;
                        break;
                }
            }

            newMember.MembersIP = Common.GetIP4Address();
            newMember.LastIP = newMember.MembersIP;
            // Save the profile - must be done since we explicitly created this profile instance
            Members.SaveMember(newMember);

            Membership.GetUser(newMember.Username,true);
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
                    sMessage = String.Format(CreateUserWizard1.InvalidPasswordErrorMessage, Membership.Provider.MinRequiredPasswordLength, Membership.Provider.MinRequiredNonAlphanumericCharacters);
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

            var cvError = new CustomValidator
                            {
                                ValidationGroup = "CreateUserWizard1",
                                ErrorMessage = sMessage,
                                IsValid = false
                            };
            CreateUserWizard1.CreateUserStep.ContentTemplateContainer.Controls.Add(cvError);
        }
        
        protected void SaveOptions(object sender, EventArgs e)
        {
            var newMember = Members.GetMember(CreateUserWizard1.UserName);
            XElement root = _memberFields.Root;
            var properties = RegHelper.GetProperties(newMember);
            var fields = root.Elements().Where(a => a.Attribute("Show").Value == "true" && a.Attribute("Group").Value != "Member");

            foreach (XElement field in fields)
            {
                var prop = properties.SingleOrDefault(p => p.Name == field.Name);
                Registration attr = (Registration)prop.GetCustomAttributes(false)[0];
                switch (attr.Control)
                {
                    case "Text":
                    case "LongText" :
                        var tfinder = new ControlFinder<TextBox>();
                        tfinder.FindChildControlsRecursive(CreateUserWizard1.WizardSteps[2]);
                        var tinfo = tfinder.FoundControls.ToList();
                        if (tinfo.Any(t => t.ID == field.Name))
                            prop.SetValue(newMember, Convert.ChangeType(tinfo.Single(t => t.ID == field.Name).Text, prop.PropertyType), null);
                        break;   
                    case "CheckBox" :
                        var cfinder = new ControlFinder<CheckBox>();
                        cfinder.FindChildControlsRecursive(CreateUserWizard1.WizardSteps[2]);
                        var cinfo = cfinder.FoundControls.ToList();
                        if (cinfo.Any(t => t.ID == field.Name))
                            prop.SetValue(newMember, Convert.ChangeType(cinfo.Single(t => t.ID == field.Name).Checked, prop.PropertyType), null);
                        break;
                    case "CountryLookup" :
                        var dfinder = new ControlFinder<CountryDropDownList>();
                        dfinder.FindChildControlsRecursive(CreateUserWizard1.WizardSteps[2]);
                        var dinfo = dfinder.FoundControls.ToList();
                        if (dinfo.Any(t => t.ID == field.Name))
                            prop.SetValue(newMember, Convert.ChangeType(dinfo.Single(t => t.ID == field.Name).SelectedValue, prop.PropertyType), null);
                        break;
                    case "TimeZoneLookup" :
                        var tzfinder = new ControlFinder<TimeZoneListBox>();
                        tzfinder.FindChildControlsRecursive(CreateUserWizard1.WizardSteps[2]);
                        var tzinfo = tzfinder.FoundControls.ToList();
                        if (tzinfo.Any(t => t.ID == field.Name))
                        {
                            string tZone = tzinfo.Single(t => t.ID == field.Name).SelectedValue;
                            prop.SetValue(newMember, Convert.ChangeType(tZone, prop.PropertyType), null);

                            TimeZoneInfo tzoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tZone);
                            newMember.TimeOffset = tzoneInfo.BaseUtcOffset.TotalHours;
                        }
                        break;
                    case "Lookup":
                        var lfinder = new ControlFinder<DropDownList>();
                        lfinder.FindChildControlsRecursive(CreateUserWizard1.WizardSteps[2]);
                        var linfo = lfinder.FoundControls.ToList();
                        if (linfo.Any(t => t.ID == field.Name))
                            prop.SetValue(newMember, Convert.ChangeType(linfo.Single(t => t.ID == field.Name).SelectedValue, prop.PropertyType), null);
                        break;
                }
                
            }

            Members.SaveMember(newMember);
        }        

        #endregion

        #region Validation Methods

        private bool StopForumSpamCheck(int faillimit)
        {
            const string stopforumspamurl = "http://www.stopforumspam.com/api";
            string email = "email=" + CreateUserWizard1.Email;
            string username = "username=" + CreateUserWizard1.UserName;
            string memberIP = "ip=" + Common.GetIP4Address();

            var client = new WebClient();
            byte[] data = client.DownloadData(String.Format("{0}?{1}&{2}&{3}", stopforumspamurl, email, username, memberIP));
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

        private bool IsMinAge()
        {
            if (Config.MinAge > 0)
            {
                //find the Date of birth control
                var cfinder = new ControlFinder<DobPicker>();
                cfinder.FindChildControlsRecursive(CreateUserWizard1.CreateUserStep.ContentTemplateContainer);

                var vfinder = new ControlFinder<RequiredFieldValidator>();
                vfinder.FindChildControlsRecursive(CreateUserWizard1.CreateUserStep.ContentTemplateContainer);

                var dobCtl = cfinder.FoundControls.SingleOrDefault(c => c.ID == "DateOfBirth");
                if (dobCtl != null)
                {
                    if (Convert.ToInt32(Common.GetAgeFromDOB(dobCtl.DOBStr)) < Config.MinAge)
                    {
                        RequiredFieldValidator req = new RequiredFieldValidator
                                                     {
                                                         ValidationGroup = CreateUserWizard1.ID,
                                                         ErrorMessage =
                                                             String.Format(
                                                                 webResources.lblMinAge,
                                                                 Config.MinAge),
                                                         IsValid = false
                                                     };
                        Page.Validators.Add(req);
                        //req.Visible = false;

                        return false;
                    }
                }
                return Convert.ToInt32(Common.GetAgeFromDOB(dobCtl.DOBStr)) > Config.MinAge;
            }
            return true;
        }

        #endregion

        private void SetVisiblity()
        {
            _regsettings = Server.MapPath("~/App_Data/regcontrols.xml");
            if (File.Exists(_regsettings))
            {
                LoadControls();
            }
        }
        private void LoadControls()
        {
            // Member, Member:Info, Member:Other, Member:Social Media, Settings
            Panel grpPanel = LoadGroupControls("Member");
            grpPanel.GroupingText = String.Empty;
            var phStuff = (PlaceHolder)CreateUserWizard1.CreateUserStep.ContentTemplateContainer.FindControl("phCustomStuff");
            phStuff.Controls.Add(grpPanel);

            mInfoControls.Controls.Add(LoadGroupControls("Member:Social Media"));
            mInfoControls.Controls.Add(LoadGroupControls("Member:Info"));
            mPostingControls.Controls.Add(LoadGroupControls("Settings"));
        }
        private Panel LoadGroupControls(string groupname)
        {
            var properties = RegHelper.GetProperties(new MemberInfo());
            Panel grpPanel = null;
            _memberFields = XDocument.Load(_regsettings);
            XElement root = _memberFields.Root;

            //get all the controls to be shown and order by groupname so we can arrange on the wizard
            grpPanel = new Panel { ID = "pnl" + groupname, GroupingText = RegHelper.SplitCamelCase(groupname) };
            foreach (XElement elem in root.Elements().Where(a => a.Attribute("Show").Value == "true" && a.Attribute("Group").Value == groupname))
            {
                //get the custom attributes for the current property so we can get the control to use
                var prop = properties.SingleOrDefault(p => p.Name == elem.Name.LocalName);
                Registration attr = (Registration)prop.GetCustomAttributes(false)[0];

                //var isVisible = Convert.ToBoolean(elem.Attribute("Show").Value);
                var isRequired = Convert.ToBoolean(elem.Attribute("Require").Value);
                RegControl ctl = new RegControl()
                {
                    ValidationGroup = CreateUserWizard1.ID,
                    LabelText = RegHelper.SplitCamelCase(elem.Name.LocalName),
                    ControlID = elem.Name.LocalName,
                    ControlType = attr.Control,
                    InvalidMessage = isRequired ? elem.Name.LocalName + " Is required" : "",
                    Show = true,
                    Required = isRequired,
                    ClientScript = "true",
                    CssClass = isRequired ? "label_mandatory" : "label_not_mandatory"
                };
                //add the new control to the panel
                grpPanel.Controls.Add(ctl);

            }

            return grpPanel;
        }
        
        protected void CheckContinue(object sender, WizardNavigationEventArgs e)
        {
            //check we are the requird age
            //if(e.CurrentStepIndex == 1)

        }

        private class ControlFinder<T> where T : Control
        {
            private readonly List<T> _foundControls = new List<T>();
            public IEnumerable<T> FoundControls
            {
                get { return _foundControls; }
            }

            public void FindChildControlsRecursive(Control control)
            {
                foreach (Control childControl in control.Controls)
                {
                    if (childControl.GetType() == typeof(T))
                    {
                        _foundControls.Add((T)childControl);
                    }
                    else
                    {
                        FindChildControlsRecursive(childControl);
                    }
                }
            }
        }
    }

    public static class RegHelper
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static IEnumerable<PropertyInfo> GetProperties<T>(T item) where T : new()
        {
            var type = item.GetType();
            var properties = type.GetProperties();

            var propertyList = properties
                .Where(p => p.GetCustomAttributes(false).Any(a => a.GetType() == typeof(Registration) && ((Registration)a).Display));

            var res = from info in propertyList
                      select new { info.Name, Required = false, Show = false };
            return propertyList;
        }
    }

}