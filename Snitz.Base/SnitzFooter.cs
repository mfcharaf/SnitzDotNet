using System;
using System.IO;
using System.Reflection;
using System.Web.UI;

namespace SnitzBase
{
    public class UserControl : System.Web.UI.UserControl
    {

        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();

            string content = String.Empty;
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType(), GetType().Name + ".ascx");
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            Control userControl = Page.ParseControl(content);
            if (userControl != null) this.Controls.Add(userControl);
        }
    }
}
