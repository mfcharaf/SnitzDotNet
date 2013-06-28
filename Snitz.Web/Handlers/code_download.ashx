<%@ WebHandler Language="C#" Class="code_download" %>

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SnitzData;

public class code_download : IHttpHandler
{

    private static int codeNum;

    public void ProcessRequest(HttpContext context)
    {
        if (String.IsNullOrEmpty(context.Request.Params["codenum"]))
            return;

        if (!Int32.TryParse(context.Request.Params["codenum"], out codeNum))
            return;

        string txtMessage = "";
        switch (context.Request.Params["type"])
        {
            case "T":
                txtMessage = HttpUtility.HtmlDecode(Util.GetTopic(Convert.ToInt32(context.Request.Params["id"])).Message);
                break;
            case "R":
                txtMessage = HttpUtility.HtmlDecode(Util.GetReply(Convert.ToInt32(context.Request.Params["id"])).Message);
                break;
        }

        MemoryStream m = new MemoryStream(Encoding.Default.GetBytes(DownloadCodeTags(txtMessage)));

        byte[] byteArray = m.ToArray();
        //Clean up the memory stream
        m.Flush();
        m.Close();
        // Clear all content output from the buffer stream
        context.Response.Clear();
        // Add a HTTP header to the output stream that specifies the default filename
        // for the browser's download dialog
        context.Response.AddHeader("Content-Disposition", "attachment; filename=code.txt");
        // Add a HTTP header to the output stream that contains the 
        // content length(File Size). This lets the browser know how much data is being transfered
        context.Response.AddHeader("Content-Length", byteArray.Length.ToString());
        // Set the HTTP MIME type of the output stream
        context.Response.ContentType = "application/octet-stream";
        // Write the data out to the client.
        context.Response.BinaryWrite(byteArray);
    }
    private static string DownloadCodeTags(string fString)
    {
        //int nTagPos;

        string strCodeText;
        int Tagcount = 1;


        string strResultString = "";
        fString = Regex.Replace(fString, @"\[noparse\]", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        string strTempString = Regex.Replace(fString, @"\[/noparse\]", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        const string oTag = "[code]";
        string roTag = "// Begin code Snippet" + Environment.NewLine;
        const string cTag = "[/code]";
        string rcTag = Environment.NewLine + "// End code snippet";


        int oTagPos = strTempString.IndexOf(oTag, 0, StringComparison.CurrentCultureIgnoreCase);
        int cTagPos = strTempString.IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase);
        if ((oTagPos >= 0) && (cTagPos > 0))
        {
            string[] strArray = Regex.Split(strTempString, Regex.Escape(oTag));
            for (int counter = 1; counter < strArray.Length; counter++)
            {
                if (strArray[counter].IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    string[] strArray2 = Regex.Split(strArray[counter], Regex.Escape(cTag));
                    strCodeText = strArray2[0];
                    strCodeText.Trim();
                    strCodeText = Regex.Replace(strCodeText, @"<br>|<br />", Environment.NewLine, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (codeNum == Tagcount)
                        strResultString = strResultString + roTag + Environment.NewLine + strCodeText + Environment.NewLine + rcTag;
                    Tagcount++;
                }
                else
                {
                    strResultString += strArray[counter];
                }
            }
            string pattern = @"\[(.|\n)*?\]";
            strTempString = Regex.Replace(strResultString, pattern, string.Empty);
            //strTempString = strResultString;
        }

        return strTempString;
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}