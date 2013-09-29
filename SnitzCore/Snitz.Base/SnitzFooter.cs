/*
####################################################################################################################
##
## SnitzBase - SnitzFooter
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
