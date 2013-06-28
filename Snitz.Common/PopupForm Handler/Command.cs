using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using SnitzCommon.Account.Devarchive_Net.Navigation;

//code for handling popup forms

namespace SnitzCommon
{
    public class Command
    {
        #region Command functionality

        private string m_CommandName = "";

        public Command(string commandName)
        {
            m_CommandName = commandName;
        }

        public static Command Create(string commandName)
        {
            return new Command(commandName);
        }

        public object Execute(object data)
        {
            Type type = this.GetType();
            MethodInfo method = type.GetMethod(m_CommandName);
            var args = new object[] { data };
            try
            {
                return method.Invoke(this, args);
            }
            catch
            {
                // TODO: Add logging functionality
                throw;
            }
        }

        #endregion

        #region Public execution commands

        /// <summary>
        /// returns rendered control's string representation.
        /// object "data" should be passed from javascript method 
        /// as array of objects consisting of two objects,
        /// first - pageID - integer identificator by which we will
        /// lookup real control path; second object may be some data
        /// that the control needs.
        /// </summary>
        public object GetWizardPage(object data)
        {
            bool errorLogged = false;
            try
            {
                var param =
                    (Dictionary<string, object>)data;
                var pageId = (int)param["pageID"];
                var customData = param["data"];

                var controlPath = m_NavigationData.Find(x => x.Key == pageId).Value;

                if (!String.IsNullOrEmpty(controlPath))
                {
                    if(
                        controlPath.ToLower()
                        .EndsWith(".htm") 
                        ||
                        controlPath.ToLower()
                        .EndsWith(".html") 
                        ||
                        controlPath.ToLower()
                        .EndsWith(".txt"))
                    {
                        var result = "";
                        using (
                                TextReader tr = 
                                    new StreamReader(
                                        HttpContext.Current.Server.MapPath(controlPath)
                                        )
                                    )
                        {
                            result = tr.ReadToEnd();
                        }
                        return new ContentsResponse(result, string.Empty, string.Empty);
                    }
                    else
                    {
                        return TemplateViewManager.RenderView(controlPath, customData);
                    }
                }
            }
            catch
            {
                // Log error
                errorLogged = true;
            }
            if (!errorLogged)
            {
                // Log custom error saying 
                // we did not find the page
            }
            return ContentsResponse.Empty;
        }

        #endregion

        #region User Controls

        private static List<KeyValuePair<int, string>>
            m_NavigationData = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(1,Pages.Controls.Modules.Profile.SnitzProfile),
                new KeyValuePair<int, string>(2,Pages.Controls.Modules.Profile.RetrievePassword),
                 new KeyValuePair<int, string>(3,Pages.Controls.Common.Confirmation),
                 new KeyValuePair<int, string>(4,Pages.Controls.Modules.Profile.IPLookup),
                 new KeyValuePair<int, string>(5,Pages.Controls.Common.EmailTopic),
                 new KeyValuePair<int, string>(6,Pages.Controls.Modules.Admin.SplitTopic),
                 new KeyValuePair<int, string>(7,Pages.Controls.Modules.Admin.Moderate),
                 new KeyValuePair<int, string>(8,Pages.Controls.Modules.Admin.EditForum),
                 new KeyValuePair<int, string>(9,Pages.Controls.Modules.Admin.EditCategory),
                 new KeyValuePair<int, string>(10,Pages.Controls.Common.EmailMember),
                 new KeyValuePair<int, string>(11,Pages.Controls.Common.ViewEvent)
            };

        #endregion
    }
}