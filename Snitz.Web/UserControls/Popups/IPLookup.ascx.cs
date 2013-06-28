using System;
using System.Net;
using SnitzCommon;

namespace SnitzUI.UserControls
{
    public partial class IPLookup : TemplateUserControl
    {
        public string IPAdress { get; set; }  
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Data != null)
            {
                IPAdress = ((string)Data);

                Literal1.Text = @"<strong>" + Resources.webResources.lblIP + @"</strong>&nbsp;" + IPAdress;
                Literal1.Text += @"<br />";
                try
                {
                    if (IPAdress != "")
                    {
                        IPHostEntry myIP = Dns.GetHostEntry(IPAdress);
                        Literal1.Text += @"<strong>" + Resources.webResources.lblHost + @"</strong><br />" + myIP.HostName;
                    }
                }
                catch (Exception exc)
                {
                    Literal1.Text += @"<br/>" + exc.Message;
                }
            }


        }
    }
}