using System;

namespace SnitzCommon
{
    /// <summary>
    /// Favourites and Bookmark Url's
    /// </summary>
    [Serializable]
    public class SnitzLink
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public SnitzLink(){}

        public SnitzLink (string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
