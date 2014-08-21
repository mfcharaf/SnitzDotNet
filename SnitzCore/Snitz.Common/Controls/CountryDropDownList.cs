using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using SnitzConfig;

namespace SnitzCommon.Controls
{
    public class CountryDropDownList : DropDownList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BindCountry();
        }

        private void BindCountry()
        {
            var doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath(Config.CultureSpecificDataDirectory + "countries.xml"));
            var xmlNodeList = doc.SelectNodes("//country");
            if (xmlNodeList != null)
                foreach (XmlNode node in xmlNodeList)
                {
                    this.Items.Add(new ListItem(node.InnerText, node.InnerText));
                }
            this.Items.Insert(0,new ListItem("[Select Country]",""));
        }
    }
}
