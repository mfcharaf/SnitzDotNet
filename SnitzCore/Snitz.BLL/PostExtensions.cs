/*
####################################################################################################################
##
## Snitz.BLL - PostExtensions
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		30/07/2013
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using ModConfig;
using Snitz.Entities;
using SnitzConfig;


namespace Snitz.BLL
{
    /// <summary>
    /// Extension methods for parsing posted data
    /// </summary>
    public static class PostExtensions
    {
        private const RegexOptions matchOptions = RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Singleline;

        public static string ReplaceSmileTags(this string text)
        {
            if (!Config.AllowIcons || String.IsNullOrEmpty(text))
                return text;
            foreach (Emoticon emoticon in Emoticons.GetEmoticons())
            {
                if (text.Contains(emoticon.Code))
                    text = Regex.Replace(text, Regex.Escape(emoticon.Code), GetSmileImage(emoticon.Image, emoticon.Description, "align='middle'"), matchOptions);
            }
            return text;
        }

        private static string GetSmileImage(string fIconName, string fAltText, string fOtherTags)
        {

            string strImageUrl = VirtualPathUtility.ToAbsolute(Config.ImageDirectory + "emoticons/");
            if (fIconName != "")
            {
                if (fOtherTags != "") { fOtherTags = " " + fOtherTags; }

                string strTempImageUrl = fIconName.IndexOf("http://") >= 0 ? "" : strImageUrl;
                string[] tmpicons = Regex.Split(fIconName, Regex.Escape("|"), RegexOptions.Singleline);
                return "<img src='" + strTempImageUrl + tmpicons[0] + "' class='NoBorder' alt='" + fAltText + "' title='" + fAltText + "'" + fOtherTags + " />";
            }
            return "";
        }
        
        private static string GetImage(string fIconName, string fAltText, string fOtherTags)
        {

            string strImageUrl = VirtualPathUtility.ToAbsolute(Config.ImageDirectory);
            if (fIconName != "")
            {
                if (fOtherTags != "") { fOtherTags = " " + fOtherTags; }

                string strTempImageUrl = fIconName.IndexOf("http://") >= 0 ? "" : strImageUrl;
                string[] tmpicons = Regex.Split(fIconName, Regex.Escape("|"), RegexOptions.Singleline);
                return "<img src='" + strTempImageUrl + tmpicons[0] + "' class='NoBorder' alt='" + fAltText + "' title='" + fAltText + "'" + fOtherTags + " />";
            }
            return "";
        }
             
        public static string ReplaceURLs(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            text = text.Replace("&quot;", "\x22");
            text = text.Replace("&amp;", "\x26");
            string matchstring;

            matchstring = @"((\[mail]|\[mail]mailto:)(?<email>\b[A-z0-9a-z._%-]+@[A-Z0-9a-z.-]+\.[A-Za-z]{2,4}\b|\[)(\[/mail]))";
            text = Regex.Replace(text, matchstring, "<a href='mailto:${email}'>${email}</a> ", matchOptions);
            matchstring = @"((\[mail=\x22|\[mail=\x22mailto:)(?<email>\b[A-z0-9a-z._%-]+@[A-Z0-9a-z.-]+\.[A-Za-z]{2,4})(\b|\x22)](?<linkText>.*)\[/mail])";
            text = Regex.Replace(text, matchstring, "<a href='mailto:${email}'>${linkText}</a> ", matchOptions);
            matchstring = @"((\[url=\x22)(?:https?://|telnet://|file://|ftp://)(?<url>[\w/#~:.?+=&%@;!\-]+?)\x22](?<linkText>.+?)(\[/url]))";
            text = Regex.Replace(text, matchstring, "<a href='http://${url}' rel='nofollow' target='_blank'>${linkText}</a> ", matchOptions);
            matchstring = @"((\[url=\x22)(?<url>([A-Z0-9a-z.-]+\.)([\w/#~:.?+=&%@;!\-]+?))\x22](?<linkText>.+?)(\[/url]))";
            text = Regex.Replace(text, matchstring, "<a href='http://${url}' rel='nofollow' target='_blank'>${linkText}</a> ", matchOptions);
            matchstring = @"(\[url])(((https?://)(?<url>[\w/#~:.?+=&%@;!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@;!\-\[]|$|\[)))|(?<url>([a-zA-Z0-9]+[a-zA-Z0-9\-\.]+?\.[com|uk|org|net|mil|edu|gov]).*))(\[/url])";
            text = Regex.Replace(text, matchstring, "<a href='http://${url}' rel='nofollow' target='_blank'>${url}</a>", matchOptions);

            matchstring = @"((\[rurl=\x22)(?<url>([/A-Z0-9a-z.\-_+]+\.)([\w/#~:.?+=&%@;!\-]+?))\x22](?<linkText>.+?)(\[/rurl]))";
            text = Regex.Replace(text, matchstring, "<a href='${url}' rel='follow' target='_blank'>${linkText}</a> ", matchOptions);
            matchstring = @"((\[rurl])(?<url>([/A-Z0-9a-z.-_+]+\.)([\w/#~:.?+=&%@;!\-]+?))(\[/rurl]))";
            text = Regex.Replace(text, matchstring, "<a href='${url}' rel='follow' target='_blank'>${url}</a> ", matchOptions);

            return text;
        }

        public static string ReplaceCodeTags(this string text, int postId, string type)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            if (type == null || !Config.AllowForumCode)
                return text;

            string strCodeText;
            string strTempString = text;
            string strResultString = "";

            string DownloadLink = "";

            string[] strArray;

            const string oTag = "[code]";
            const string roTag = "<div class='codebox'>";
            const string cTag = "[/code]";
            const string rcTag = "</code></li></ol></div></div>";

            int oTagPos = strTempString.IndexOf(oTag, 0, StringComparison.CurrentCultureIgnoreCase);
            int cTagPos = strTempString.IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase);
            int TagCount = 1;
            if ((oTagPos >= 0) && (cTagPos > 0))
            {
                strArray = Regex.Split(strTempString, Regex.Escape(oTag), matchOptions);
                for (int counter2 = 0; counter2 < strArray.Length; counter2++)
                {
                    if (strArray[counter2].IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        string[] strArray2 = Regex.Split(strArray[counter2], Regex.Escape(cTag), matchOptions);
                        strCodeText = HttpUtility.HtmlDecode(strArray2[0]);
                        if (strCodeText != null)
                        {
                            strCodeText.Trim();
                            strCodeText = Regex.Replace(strCodeText, @"<br>", Environment.NewLine, matchOptions);
                            strCodeText = Regex.Replace(strCodeText, @"<br />", Environment.NewLine, matchOptions);
                            if (type != "")
                            {
                                DownloadLink = "<div class='codehead'>Code: [<a href='/handlers/code_download.ashx?id=" + postId +
                                               "&amp;type=" + type + "&amp;codenum=" + TagCount +
                                               "'>download</a>]</div><div class='scrollcode'><ol><li><code>";
                            }
                        }
                        strCodeText = HttpUtility.HtmlEncode(strCodeText);
                        strCodeText = Regex.Replace(strCodeText, @"\r\n", "</code></li><li><code>", matchOptions);
                        strResultString = strResultString + roTag + DownloadLink + strCodeText + rcTag + strArray2[1];
                        TagCount++;
                    }
                    else
                    {
                        strResultString += strArray[counter2];
                    }
                }

                strTempString = strResultString;
            }

            return strTempString;
        }

        private static string ReplaceCodeTags(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            string strCodeText;
            string strTempString = text;
            string strResultString = "";

            string DownloadLink = "";

            string[] strArray;

            const string oTag = "[code]";
            const string roTag = "<div class='codebox'><div class='scrollcode'><ol><li><code>";
            const string cTag = "[/code]";
            const string rcTag = "</code></li></ol></div></div>";

            int oTagPos = strTempString.IndexOf(oTag, 0, StringComparison.CurrentCultureIgnoreCase);
            int cTagPos = strTempString.IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase);
            int TagCount = 1;
            if ((oTagPos >= 0) && (cTagPos > 0))
            {
                strArray = Regex.Split(strTempString, Regex.Escape(oTag), matchOptions);
                for (int counter2 = 0; counter2 < strArray.Length; counter2++)
                {
                    if (strArray[counter2].IndexOf(cTag, 0, StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        string[] strArray2 = Regex.Split(strArray[counter2], Regex.Escape(cTag), matchOptions);
                        strCodeText = HttpUtility.HtmlDecode(strArray2[0]);
                        if (strCodeText != null)
                        {
                            strCodeText.Trim();
                            strCodeText = Regex.Replace(strCodeText, @"<br>", Environment.NewLine, matchOptions);
                            strCodeText = Regex.Replace(strCodeText, @"<br />", Environment.NewLine, matchOptions);
                        }
                        strCodeText = HttpUtility.HtmlEncode(strCodeText);
                        strCodeText = Regex.Replace(strCodeText, @"\n", "</code></li><li><code>", matchOptions);
                        strResultString = strResultString + roTag + DownloadLink + strCodeText + rcTag + strArray2[1];
                        TagCount++;
                    }
                    else
                    {
                        strResultString += strArray[counter2];
                    }
                }

                strTempString = strResultString;
            }

            return strTempString;
        }

        public static string ReplaceImageTags(this string text)
        {
            if (!Config.AllowForumCode || !Config.AllowImages || String.IsNullOrEmpty(text))
                return text;

            Regex imgregex = new Regex(@"\[img]|\[image]|\[img=right]|\[image=right]|\[img=left]|\[image=left]");
            MatchCollection MatchList = imgregex.Matches(text);
            foreach (Match m in MatchList)
            {
                string thismatch = m.Groups[0].Value;
                switch (thismatch)
                {
                    case "[img]":
                        text = Regex.Replace(text, @"(\[img])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/img])", "<a href='$2' title='Click to open full size Image' target='_blank'><img src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                    case "[image]":
                        text = Regex.Replace(text, @"(\[image])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/image])", "<a href='$2' title='Click to open full size Image' target='_blank'><img src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                    case "[img=right]":
                        text = Regex.Replace(text, @"(\[img=right])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/img=right])", "<a href='$2' title='Click to open full size Image' target='_blank'><img align='right' src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                    case "[image=right]":
                        text = Regex.Replace(text, @"(\[image=right])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/image=right])", "<a href='$2' title='Click to open full size Image' target='_blank'><img align='right' src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                    case "[img=left]":
                        text = Regex.Replace(text, @"(\[img=left])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/img=left])", "<a href='$2' title='Click to open full size Image' target='_blank'><img align='left' src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                    case "[image=left]":
                        text = Regex.Replace(text, @"(\[image=left])([\w/~:.?+=5@ \(\)!\-]+?)(?=[.:?\-]*(?:[^\w/#~:.?+=&%@!\-]|$))(\[\/image=left])", "<a href='$2' title='Click to open full size Image' target='_blank'><img align='left' src='$2' class='mImg NoBorder' alt='Click to open full size Image' /></a>", matchOptions);
                        break;
                }
            }
            return text;
        }

        public static string ReplaceFileTags(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            const string startTag = "[file]";
            const string endTag = "[/file]";
            while (text.Contains(endTag))
            {
                int begStr = text.IndexOf(startTag) + startTag.Length;
                int endStr = text.IndexOf(endTag);
                if (endStr <= begStr) return text;
                string leftPart = text.Substring(0, begStr - startTag.Length);
                string rightPart = text.Substring(endStr + endTag.Length);
                string arrfileInfo = text.Substring(begStr, endStr - begStr);
                string[] arrFileInfo = arrfileInfo.Split(',');
                string fileName = arrFileInfo[0];
                string fileNameExt = fileName.Substring(fileName.LastIndexOf('.'));
                string memberFldrID = arrFileInfo[1];

                StringBuilder midPart = new StringBuilder();

                midPart.AppendLine("<table border='0' cellpadding='4' cellspacing='1' class='attachment'>\n");
                midPart.AppendLine("<tr>");
                midPart.AppendLine("<td class='row' colspan='2'>");

                string fileUrlLocation = UploadConfig.fileUploadLocation.Replace("~/", "") + memberFldrID + "/";
                switch (fileNameExt.ToUpper())
                {
                    //                        case ".JPG", ".JPEG", ".GIF", ".PNG"
                    case ".JPG":
                    case ".JPEG":
                    case ".GIF":
                    case ".PNG":
                        midPart.AppendFormat("<strong>Image Attachment:</strong> {0}", fileName);
                        midPart.AppendFormat("<br/><a href='{0}{1}'><img src='{2},{3}' /></a>", fileUrlLocation, fileName, fileUrlLocation, fileName);
                        break;
                    case ".SWF":
                        midPart.AppendLine("<strong>SWF files no longer supported</strong><br/>");
                        break;
                    default:
                        midPart.AppendLine("<br/><strong>File Attachment:</strong> ");
                        midPart.AppendFormat("<a href='{0}{1}'>{2}</a>", fileUrlLocation, fileName, fileName);
                        break;
                } // End switch statement
                midPart.AppendLine("</td></tr></table>");

                text = leftPart + midPart + rightPart;
            } // end of while(text.Contains(startTag))
            return text;
        }

        public static string ReplaceTableTags(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            return Regex.Replace(text, @"(?:\[)(?<tabletag>(?:table|/table|tr|/tr|td|/td|th|/th)([^\[])*)(?:\])", "<${tabletag}>", RegexOptions.Multiline);
        }
        
        public static string ReplaceTags(this string text)
        {
            if (!Config.AllowForumCode || String.IsNullOrEmpty(text))
                return text;

            text = Regex.Replace(text, @"\[pencil\]", GetImage("admin/write.png", "edit icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[subscribe\]", GetImage("admin/subscribe.png", "subscribe icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[unsubscribe\]", GetImage("admin/unsubscribe.png", "unsubscribe icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[edit\]", GetImage("message/edit.png", "edit icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[lock\]", GetImage("admin/lock.png", "edit icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[delete\]", GetImage("admin/trash.png", "trash icon", "align='middle'"), matchOptions);
            text = Regex.Replace(text, @"\[quote\]", "<blockquote class='quoteMessage'><span>", matchOptions);
            text = Regex.Replace(text, @"\[quote=\x22([\w/#~:.?+=&%@!\-]+?)(?=[\x22]+])\x22\]", "<blockquote class='quoteMessage'><span><em>Originally posted by $1</em><br/>", matchOptions);
            text = Regex.Replace(text, @"\[\/quote\]", "</span></blockquote>", matchOptions);

            Regex colourregex = new Regex(@"\[(#[a-fA-F0-9]{2,6})\]");
            MatchCollection MatchList = colourregex.Matches(text);

            foreach (Match m in MatchList)
            {
                string thismatch = m.Groups[1].Value;
                text = Regex.Replace(text, @"\[(" + thismatch + @")\](.*?)\[\/" + thismatch + @"\]", "<span style='color:$1'>$2</span>", matchOptions);
            }

            text = Regex.Replace(text, @"\[(strike|del|s)\](.*?)\[\/(strike|del|s)]", "<del>$2</del>", matchOptions);

            text = Regex.Replace(text, @"\[(red|green|blue|white|purple|yellow|violet|brown|black|pink|orange|gold|beige|teal|navy|maroon|limegreen)](.*?)\[\/\1]", "<span style='color:$1'>$2</span>", matchOptions);

            text = Regex.Replace(text, @"\[size=1](.*?)\[\/size=1]", "<span style='font-size:xx-small'>$1</span>", matchOptions);
            text = Regex.Replace(text, @"\[size=2](.*?)\[\/size=2]", "<span style='font-size:x-small'>$1</span>", matchOptions);
            text = Regex.Replace(text, @"\[size=3](.*?)\[\/size=3]", "<span style='font-size:small'>$1</span>", matchOptions);
            text = Regex.Replace(text, @"\[size=4](.*?)\[\/size=4]", "<span style='font-size:large'>$1</span>", matchOptions);
            text = Regex.Replace(text, @"\[size=5](.*?)\[\/size=5]", "<span style='font-size:x-large'>$1</span>", matchOptions);
            text = Regex.Replace(text, @"\[size=6](.*?)\[\/size=6]", "<span style='font-size:xx-large'>$1</span>", matchOptions);

            text = Regex.Replace(text, @"\[list]", "<ul>", matchOptions);
            text = Regex.Replace(text, @"\[\/list]", "</ul>", matchOptions);
            text = Regex.Replace(text, @"\[list=(?<type>[1iIaA])(,(?<start>[0-9]*))*\]", "<ol type='${type}' start='${start}'>", matchOptions);
            text = Regex.Replace(text, @"\[\/list=[1iIaAo]\]", "</ol>", matchOptions);
            text = Regex.Replace(text, @"\[\*]", "<li>", matchOptions);
            text = Regex.Replace(text, @"\[/\*]", "</li>", matchOptions);
            text = Regex.Replace(text, @"\[left](.*?)\[\/left]", "<div align='left'>$1</div>", matchOptions);
            text = Regex.Replace(text, @"\[center](.*?)\[\/center]", "<div align='center'>$1</div>", matchOptions);
            text = Regex.Replace(text, @"\[right](.*?)\[\/right]", "<div align='right'>$1</div>", matchOptions);
            text = Regex.Replace(text, @"\[br]", "<br />", matchOptions);
            text = Regex.Replace(text, @"\[hr]", "<hr class='message'/>", matchOptions);
            text = Regex.Replace(text, @"\[ltr](.*?)\[\/ltr]", "<div dir='ltr'>$1</div>", matchOptions);
            text = Regex.Replace(text, @"\[rtl](.*?)\[\/rtl]", "<div dir='rtl'>$1</div>", matchOptions);
            //[i],[b] etc
            text = Regex.Replace(text, @"\[((?>(?!\[|\]).|\[(?<Depth>)|\] (?<-Depth>))*(?(Depth)(?!)))\]", "<$1>", matchOptions);

            // cleanup
            text = Regex.Replace(text, Regex.Escape(Environment.NewLine), "<br />", matchOptions);
            text = Regex.Replace(text, Regex.Escape("\n"), "<br />", matchOptions);
            text = Regex.Replace(text, @"<ul><br \/>", "<ul>", matchOptions);
            text = Regex.Replace(text, @"<\/li><br \/>", "</li>", matchOptions);

            return text;
        }

        public static string ParseTags(this string text)
        {
            return text.ReplaceNoParseTags().ReplaceBadWords().ReplaceSmileTags().ReplaceImageTags().ReplaceURLs().ReplaceTableTags()
                .ReplaceFileTags().ReplaceCodeTags().ReplaceTags();
        }


    }
}
