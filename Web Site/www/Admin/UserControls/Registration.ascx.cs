using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Snitz.Entities;
using SnitzCommon;
using SnitzCommon.Controls;
using SnitzConfig;
using MemberInfo = Snitz.Entities.MemberInfo;

namespace SnitzUI.Admin.UserControls
{
    public struct CtlParams
    {
        public bool Show;
        public bool Required;
        public string Group;
    }
    public partial class Admin_Registration : UserControl
    {
        List<string> _systemFields = new List<string>() { "Username", "Password", "Email" };

        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControls();
        }

        protected void BtnSubmitClick(object sender, EventArgs e)
        {
            string regsettings = Server.MapPath("~/App_Data/regcontrols.xml");

            var req = new Dictionary<string, CtlParams>();
            foreach (var control in mRegControls.Controls)
            {
                if (control is Panel)
                {
                    foreach (var rctl in ((Panel)control).Controls)
                    {
                        if (rctl is AdminRegControl)
                        {
                            var properties = Helper.GetProperties<MemberInfo>(new MemberInfo());
                            var prop = properties.SingleOrDefault(p => p.Name == ((AdminRegControl)rctl).ControlID);
                            var custom = (Registration)prop.GetCustomAttributes(false)[0];
                            
                            var ctl = new CtlParams
                                      {
                                          Show = ((AdminRegControl)rctl).CbxShow.Checked,
                                          Required = ((AdminRegControl)rctl).CbxRequired.Checked,
                                          Group = custom.Group
                                      };
                            if (_systemFields.Contains(((AdminRegControl)rctl).ControlID) && ((AdminRegControl)rctl).ControlID != "DateOfBirth")
                                ctl.Show = false;
                            req.Add(((AdminRegControl)rctl).ControlID, ctl);
                        }                        
                    }
                }

            }

            XElement el2 = new XElement("root",
            req.Select(kv => new XElement(kv.Key, new XAttribute("Group", kv.Value.Group), new XAttribute("Show", kv.Value.Show), new XAttribute("Require", kv.Value.Required))));
            el2.Save(regsettings, SaveOptions.None);

            var toUpdate = new Dictionary<string, string>();
            if(txtPLength.Text != Config.PreferredPasswordLength.ToString())
                toUpdate.Add("PreferredPasswordLength".GetPropertyDescription(), txtPLength.Text);
            if(txtMinNum.Text != Config.MinimumNumericCharacters.ToString())
                toUpdate.Add("MinimumNumericCharacters".GetPropertyDescription(), txtMinNum.Text);
            if(txtMinSym.Text != Config.MinimumSymbolCharacters.ToString())
                toUpdate.Add("MinimumSymbolCharacters".GetPropertyDescription(), txtMinSym.Text);
            if(txtMinLower.Text != Config.MinimumLowerCaseCharacters.ToString())
                toUpdate.Add("MinimumLowerCaseCharacters".GetPropertyDescription(), txtMinLower.Text);
            if(txtMinUpper.Text != Config.MinimumUpperCaseCharacters.ToString())
                toUpdate.Add("MinimumUpperCaseCharacters".GetPropertyDescription(), txtMinUpper.Text);
            if (Config.MinAge != Convert.ToInt32(txtMinAge.Text))
                toUpdate.Add("MinAge".GetPropertyDescription(), txtMinAge.Text);
            if (Config.RequiresUpperAndLowerCaseCharacters != (rblUpperLower.SelectedValue == "1"))
                toUpdate.Add("RequiresUpperAndLowerCaseCharacters".GetPropertyDescription(), rblUpperLower.SelectedValue);
            if (Config.UseCaptchaReg != (rblCaptchaReg.SelectedValue == "1"))
                toUpdate.Add("UseCaptchaReg".GetPropertyDescription(), rblCaptchaReg.SelectedValue);
            if (Config.EmailValidation != (rblEmailVal.SelectedValue == "1"))
                toUpdate.Add("EmailValidation".GetPropertyDescription(), rblEmailVal.SelectedValue);
            if (Config.RestrictRegistration != (rblRestrictReg.SelectedValue == "1"))
                toUpdate.Add("RestrictRegistration".GetPropertyDescription(), rblRestrictReg.SelectedValue);
            if (Config.UniqueEmail != (rblUniqueEmail.SelectedValue == "1"))
                toUpdate.Add("UniqueEmail".GetPropertyDescription(), rblUniqueEmail.SelectedValue);

            Config.UpdateKeys(toUpdate);

        }

        private void PopulateControls()
        {
            string regsettings = Server.MapPath("~/App_Data/regcontrols.xml");
            if (File.Exists(regsettings))
            {
                LoadControls();
            }
            else
            {
                var properties = test.GetProperties<MemberInfo>(new MemberInfo());
                foreach (PropertyInfo property in properties)
                {
                    //var isBool = typeof(bool).IsAssignableFrom(property.PropertyType) ;
                    AdminRegControl ctl = new AdminRegControl()
                    {
                        LabelText = String.Format("Show {0} ", Helper.SplitCamelCase(property.Name)),
                        ControlID = property.Name,
                        Show = false,
                        Required = false,
                        CssClass = "label_not_mandatory"
                    };
                    mRegControls.Controls.Add(ctl);
                }
            }
            txtPLength.Text = Config.PreferredPasswordLength.ToString();
            txtMinNum.Text = Config.MinimumNumericCharacters.ToString();
            txtMinSym.Text = Config.MinimumSymbolCharacters.ToString();
            txtMinLower.Text = Config.MinimumLowerCaseCharacters.ToString();
            txtMinUpper.Text = Config.MinimumUpperCaseCharacters.ToString();
            txtMinAge.Text = Config.MinAge.ToString();
            rblUpperLower.SelectedValue = Config.RequiresUpperAndLowerCaseCharacters ? "1" : "0";
            rblCaptchaReg.SelectedValue = Config.UseCaptchaReg ? "1" : "0";
            rblEmailVal.SelectedValue = Config.EmailValidation ? "1" : "0";
            rblRestrictReg.SelectedValue = Config.RestrictRegistration ? "1" : "0";
            rblUniqueEmail.SelectedValue = Config.UniqueEmail ? "1" : "0";
        }

        private void LoadControls()
        {
            
            string groupname = "";
            Panel grpPanel = null;
            string regsettings = Server.MapPath("~/App_Data/regcontrols.xml");
            XDocument doc = XDocument.Load(regsettings);
            XElement root = doc.Root;
            foreach (XElement elem in root.Elements().OrderBy(e=>e.Attribute("Group").Value))
            {
                
                if (elem.Attribute("Group").Value != groupname)
                {
                    if (grpPanel != null && grpPanel.Controls.Count > 0)
                    {
                        mRegControls.Controls.Add(grpPanel);
                    }                    
                    
                    groupname = elem.Attribute("Group").Value;
                    grpPanel = new Panel {ID = "pnl" + groupname, GroupingText = groupname};
                }
                AdminRegControl ctl = new AdminRegControl()
                {
                    LabelText = Helper.SplitCamelCase(elem.Name.LocalName),
                    ControlID = elem.Name.LocalName,
                    Show = Convert.ToBoolean(elem.Attribute("Show").Value),
                    Required = Convert.ToBoolean(elem.Attribute("Require").Value),
                    CssClass = "label_not_mandatory"
                };
                if(Config.MinAge > 0)
                    _systemFields.Add("DateOfBirth");

                if (_systemFields.Contains(ctl.ControlID))
                {
                    ctl.CssClass += " label_sys";
                    ctl.Show = true;
                    ctl.Required = true;
                    ctl.Enabled = false;                    
                }

                if (grpPanel != null) grpPanel.Controls.Add(ctl);
            }
            if (grpPanel != null && grpPanel.Controls.Count > 0)
            {
                mRegControls.Controls.Add(grpPanel);
            }
        }

    }
    public static class Helper
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
            return propertyList;
        }
    }
}