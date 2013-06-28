namespace SnitzCommon
{
    /// <summary>
    /// Summary description for ContentsResponse
    /// </summary>
    public class ContentsResponse
    {
        public ContentsResponse(string _html, string _script, string _customStyle)
        {
            html = _html;
            script = _script;
            customStyle = _customStyle;
        }

        public static ContentsResponse Empty
        {
            get
            {
                return new ContentsResponse(string.Empty, string.Empty, string.Empty);
            }
        }

        public string html = "";
        public string script = "";
        public string customStyle = "";
    }
}