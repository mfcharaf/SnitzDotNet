using System.Configuration;
using SnitzConfig;


namespace ModConfig
{
    /// <summary>
    /// Summary description for newmod
    /// </summary>
    public class SampleModConfig : SnitzMod
    {
        public string Name
        {
            get { return "SampleModConfig"; }
        }

        public string Description
        {
            get { return "SampleModConfig"; }
        }

        public static string SampleModSomeValue
        {
            get
            {
                return ConfigurationManager.AppSettings["boolShowFileUpload"];
            }
            set
            {
                Config.UpdateConfig("boolShowFileUpload", value);
            }
        }


    } 
}
