using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Snitz.Entities;
using SnitzCommon.Controls;
using MemberInfo = Snitz.Entities.MemberInfo;

namespace SnitzUI.Admin
{
    public partial class PropertyTest : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
 	         base.OnInit(e);
             pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/regwizard.css");
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateControls();
        }

        private void PopulateControls()
        {
            string regsettings = Server.MapPath("~/App_Data/regcontrols.xml");
            if (File.Exists(regsettings))
            {
                LoadControls(regsettings);
            }

        }

        private void LoadControls(string regsettings)
        {
            var properties = test.GetProperties(new MemberInfo());
            string groupname = "";
            Panel grpPanel = null;
            XDocument doc = XDocument.Load(regsettings);
            XElement root = doc.Root;
            foreach (XElement elem in root.Elements().OrderBy(e=>e.Attribute("Group").Value))
            {
                if (elem.Attribute("Group").Value != groupname)
                {
                    if (grpPanel != null && grpPanel.Controls.Count > 0)
                    {
                        mControls.Controls.Add(grpPanel);
                    }

                    groupname = elem.Attribute("Group").Value;
                    grpPanel = new Panel { ID = "pnl" + groupname, GroupingText = test.SplitCamelCase(groupname) };
                }

                var prop = properties.SingleOrDefault(p => p.Name == elem.Name.LocalName);
                Registration pr = (Registration)prop.GetCustomAttributes(false)[0];

                var isVisible = Convert.ToBoolean(elem.Attribute("Show").Value);
                var isRequired = Convert.ToBoolean(elem.Attribute("Require").Value);
                if (isVisible)
                {
                    RegControl ctl = new RegControl()
                    {
                        LabelText = test.SplitCamelCase(elem.Name.LocalName),
                        ControlID = elem.Name.LocalName,
                        ControlType = pr.Control,
                        InvalidMessage = isRequired ? elem.Name.LocalName + " Is required" : "",
                        Show = true,
                        Required = isRequired,
                        ClientScript = "false",
                        CssClass = isRequired ? "label_mandatory" : "label_not_mandatory"
                    };
                    if (grpPanel != null) grpPanel.Controls.Add(ctl);
                    
                }
               
            }
            if (grpPanel != null && grpPanel.Controls.Count > 0)
            {
                mControls.Controls.Add(grpPanel);
            }
        }

    }

    public static class test
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
        public static IEnumerable<PropertyInfo> GetProperties<T>(T item) where T : new()
        {
            var type = item.GetType();
            var properties = type.GetProperties();
            //Console.WriteLine("Finding PK for {0}", type.Name);
            // This replaces all the iteration above:
            var propertyList = properties
                .Where(p => p.GetCustomAttributes(false).Any(a => a.GetType() == typeof(Registration) && ((Registration)a).Display));

            var res = from info in propertyList
                select new {info.Name, Required = false, Show = false};
            return propertyList;
        }
    }
}