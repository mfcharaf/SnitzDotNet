using System;
using System.Reflection;
using System.Resources;

namespace Snitz.LanguageResources
{
    public class ResourceText
    {
        private ResourceText()
        { }
        public static void InitializeResources()
        {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();
            _resourceManager = new ResourceManager(
                 "Snitz.LanguageResources.webResources", resourceAssembly);
            _resourceManager.IgnoreCase = true;
            //This is my preference. You can change this...
        }
        private static ResourceManager _resourceManager;


        //ResourceText.GetString("DELETE_EMPLOYEE_CONFIRMATION");
        public static string GetString(string key)
        {
            try
            {
                string s = _resourceManager.GetString(key);
                if (null == s) throw (new Exception());
                return s;
            }
            catch
            {
                return String.Format("[?:{0}]", key);
            }
        }
    }
}