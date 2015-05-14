/*
####################################################################################################################
##
## SnitzConfig - Config
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;


namespace SnitzConfig
{
    public abstract class Config
    {
        static readonly XmlDocument XmlDoc = new XmlDocument();

        #region Config Properties

        [Description("strAnonMembers")]
        public static List<string> AnonMembers
        {
            get
            {
                return (ConfigurationManager.AppSettings["strAnonMembers"].Split(',').ToList());
            }
            set { UpdateConfig("strAnonMembers", String.Join(",", value.ToArray())); }

        }
        [Description("boolAllowThemeChange")]
        public static bool AllowThemeChange
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolAllowThemeChange"] == "1");
            }
            set { UpdateConfig("boolAllowThemeChange", value ? "1" : "0"); }

        }
        [Description("boolMoveTopicMode")]
        public static bool RestrictModeratorMove
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolMoveTopicMode"] == "1");
            }
            set { UpdateConfig("boolMoveTopicMode", value ? "1" : "0"); }

        }

        [Description("boolMailAuth")]
        public static bool EmailAuthenticate //Email requires SMTP authentication
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolMailAuth"] == "1");
            }
            set
            {
                UpdateConfig("boolMailAuth", value ? "1" : "0");
            }
        }

        [Description("smtpUserName")]
        public static string EmailAuthUser
        {
            get
            {
                SmtpSection smtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                return smtpSec.Network.UserName;
            }
        }

        [Description("smtpPassword")]
        public static string EmailAuthPwd
        {
            get
            {
                SmtpSection smtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                return smtpSec.Network.Password;
            }
        }

        [Description("smtpServer")]
        public static string EmailHost
        {
            get
            {
                SmtpSection smtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                return smtpSec.Network.Host;
            }
        }

        [Description("smtpPort")]
        public static int EmailPort
        {
            get
            {
                SmtpSection smtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                return smtpSec.Network.Port;
            }
        }

        [Description("CultureDir")]
        public static string CultureSpecificDataDirectory
        {
            get
            {
                string datapath = String.Format("~/App_Data/{0}/", CultureInfo.CurrentCulture.ToString().Trim());
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(datapath)))
                {
                    datapath = "~/App_Data/";
                }
                return datapath;
            }
        }

        [Description("imageDirectory")]
        public static string ImageDirectory
        {
            get
            {
                return String.Format("~/App_Themes/{0}/Images/", UserTheme);
            }
        }
        
        [Description("CultureSpecificImageDirectory")]
        public static string CultureSpecificImageDirectory
        {
            get
            {
                string imagepath = String.Format("~/App_Themes/{0}/Images/{1}/", HttpContext.Current.Session["_theme"], CultureInfo.CurrentCulture.ToString().Trim());
                if (!Directory.Exists(HttpContext.Current.Server.MapPath(imagepath)))
                {
                    imagepath = ImageDirectory;
                }
                return imagepath;
            }
        }

        [Description("strForumTitle")]
        public static string ForumTitle
        {
            get
            {
                return ConfigurationManager.AppSettings["strForumTitle"];
            }
            set
            {
                UpdateConfig("strForumTitle", value);
            }
        }

        [Description("strForumUrl")]
        public static string ForumUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["strForumUrl"];
                if (!url.EndsWith("/"))
                    url += "/";
                return url;
            }
            set
            {
                UpdateConfig("strForumUrl", value);
            }
        }

        [Description("defaultTheme")]
        public static string DefaultTheme
        {
            get { return ConfigurationManager.AppSettings["defaultTheme"] ?? "BlueGray"; }
            set { UpdateConfig("defaultTheme", value); }
        }
        [Description("User Theme")]
        public static string UserTheme
        {
            get
            {
                HttpCookie themecookie = HttpContext.Current.Request.Cookies.Get("theme");
                if (themecookie != null)
                {
                    return themecookie.Value;
                }
                return DefaultTheme;
            }
            set
            {
                HttpCookie themecookie = HttpContext.Current.Request.Cookies.Get("theme") ?? new HttpCookie("theme");
                themecookie.Value = value;
                themecookie.Path = "/";
                themecookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(themecookie);
            }
        }

        [Description("boolBadWordFilter")]
        public static bool FilterBadWords
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolBadWordFilter"] == "1");
            }
            set
            {
                UpdateConfig("boolBadWordFilter", value ? "1" : "0");
            }
        }

        [Description("boolIcons")]
        public static bool AllowIcons
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolIcons"] == "1");
            }
            set
            {
                UpdateConfig("boolIcons", value ? "1" : "0");
            }
        }

        [Description("boolAllowForumCode")]
        public static bool AllowForumCode
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolAllowForumCode"] == "1");
            }
            set
            {
                UpdateConfig("boolAllowForumCode", value ? "1" : "0");
            }
        }

        [Description("boolImgInPosts")]
        public static bool AllowImages
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolImgInPosts"] == "1");
            }
            set
            {
                UpdateConfig("boolImgInPosts", value ? "1" : "0");
            }
        }

        [Description("boolSignatures")]
        public static bool AllowSignatures
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolSignatures"] == "1");
            }
            set
            {
                UpdateConfig("boolSignatures", value ? "1" : "0");
            }
        }

        [Description("boolProhibitNewMembers")]
        public static bool ProhibitNewMembers
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolProhibitNewMembers"] == "1");
            }
            set
            {
                UpdateConfig("boolProhibitNewMembers", value ? "1" : "0");
            }
        }

        [Description("boolRequireReg")]
        public static bool RequireRegistration
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolRequireReg"] == "1");
            }
            set
            {
                UpdateConfig("boolRequireReg", value ? "1" : "0");
            }
        }

        [Description("boolCaptchaReg")]
        public static bool UseCaptchaReg
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolCaptchaReg"] == "1");
            }
            set
            {
                UpdateConfig("boolCaptchaReg", value ? "1" : "0");
            }
        }

        [Description("boolCaptchaLogin")]
        public static bool UseCaptchaLogin
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolCaptchaLogin"] == "1");
            }
            set
            {
                UpdateConfig("boolCaptchaLogin", value ? "1" : "0");
            }
        }
        [Description("boolShowTimer")]
        public static bool ShowTimer
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowTimer"] == "1");
            }
            set
            {
                UpdateConfig("boolShowTimer", value ? "1" : "0");
            }
        }

        [Description("strCopyright")]
        public static string Copyright
        {
            get { return ConfigurationManager.AppSettings["strCopyright"]; }
            set { UpdateConfig("strCopyright", value); }
        }

        [Description("boolShowImagePoweredBy")]
        public static bool ShowPoweredByImage
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowImagePoweredBy"] == "1");
            }
            set
            {
                UpdateConfig("boolShowImagePoweredBy", value ? "1" : "0");
            }
        }

        [Description("boolShowStatistics")]
        public static bool ShowStats 
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowStatistics"] == "1");
            }
            set
            {
                UpdateConfig("boolShowStatistics", value ? "1" : "0");
            }
        }

        [Description("boolShowLastPostLink")]
        public static bool ShowLastPostLink
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowLastPostLink"] == "1");
            }
            set
            {
                UpdateConfig("boolShowLastPostLink", value ? "1" : "0");
            }
        }

        [Description("boolShowGroups")]
        public static bool ShowGroups
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowGroups"] == "1");
            }
            set
            {
                UpdateConfig("boolShowGroups", value ? "1" : "0");
            }
        }

        [Description("strDateType")]
        public static string DateFormat
        {
            get { return ConfigurationManager.AppSettings["strDateType"] ?? "dd MMMM yyyy"; }
            set { UpdateConfig("strDateType", value); }
        }

        [Description("strTimeType")]
        public static string TimeFormat
        {
            get { return ConfigurationManager.AppSettings["strTimeType"] ?? "HH:mm:ss"; }
            set { UpdateConfig("strTimeType", value); }
        }

        [Description("dblTimeAdjust")]
        public static double TimeAdjust
        {
            get { return ConfigurationManager.AppSettings["dblTimeAdjust"] == null ? 0 : Convert.ToDouble(ConfigurationManager.AppSettings["dblTimeAdjust"]); }
            set { UpdateConfig("intTimeAdjust", value.ToString()); }
        }
        [Description("strTimeZone")]
        public static string TimeZoneString
        {
            get { return ConfigurationManager.AppSettings["strTimeZone"] ?? "00:00:00"; }
            set { UpdateConfig("strTimeZone", value.ToString()); }
        }
        //DayLightSavingAdjust
        [Description("boolDayLightSavingAdjust")]
        public static bool DayLightSavingAdjust
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolDayLightSavingAdjust"] == "1");
            }
            set
            {
                UpdateConfig("boolDayLightSavingAdjust", value ? "1" : "0");
            }
        }
        public static bool IsDayLightSaving
        {
            get
            {
                DaylightTime dtt = TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year);
                return (dtt.Start < DateTime.Now || dtt.End > DateTime.Now);
            }

        }
        [Description("intHotTopicNum")]
        public static int HotTopicNum
        {
            get { return ConfigurationManager.AppSettings["intHotTopicNum"] == null ? 25 : Convert.ToInt32(ConfigurationManager.AppSettings["intHotTopicNum"]); }
            set { UpdateConfig("intHotTopicNum", value.ToString()); }
        }

        [Description("intTopicPageSize")]
        public static int TopicPageSize
        {
            get { return ConfigurationManager.AppSettings["intTopicPageSize"] == null ? 25 : Convert.ToInt32(ConfigurationManager.AppSettings["intTopicPageSize"]); }
            set { UpdateConfig("intTopicPageSize", value.ToString()); }
        }

        [Description("intSearchPageSize")]
        public static int SearchPageSize
        {
            get { return ConfigurationManager.AppSettings["intSearchPageSize"] == null ? 25 : Convert.ToInt32(ConfigurationManager.AppSettings["intTopicPageSize"]); }
            set { UpdateConfig("intSearchPageSize", value.ToString()); }
        }

        [Description("intPagerType")]
        public static Enumerators.PagerType PagerStyle
        {
            get
            {
                return (Enumerators.PagerType)Int32.Parse(ConfigurationManager.AppSettings["intPagerType"]);
            }
            set
            {
                UpdateConfig("intPagerType", value.ToString());
            }
        }

        [Description("boolStickyTopic")]
        public static bool StickyTopics //boolStickyTopic
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolStickyTopic"] == "1");
            }
            set
            {
                UpdateConfig("boolStickyTopic", value ? "1" : "0");
            }
        }

        //[Description("strEventAdminRoles")]
        //public static List<string> EventAdminRoles
        //{
        //    get
        //    {
        //        string allowedRoles = ConfigurationManager.AppSettings["strEventAdminRoles"] ?? "Administrator,Moderator";
        //        return new List<string>(Regex.Split(allowedRoles, ",", RegexOptions.Singleline));
        //    }
        //    set
        //    {
        //        UpdateConfig("strEventAdminRoles", String.Join(",", value.ToArray()));
        //    }
        //}

        [Description("showRankStars")]
        public static bool ShowRankStars
        {
            get
            {
                return ((ShowRankType == Enumerators.RankType.StarsOnly) || (ShowRankType == Enumerators.RankType.Both));
            }
        }

        [Description("showRankTitle")]
        public static bool ShowRankTitle
        {
            get
            {
                return ((ShowRankType == Enumerators.RankType.RankOnly) || (ShowRankType == Enumerators.RankType.Both));
            }
        }

        [Description("boolShowPaging")]
        public static bool ShowPaging
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowPaging"] == "1");
            }
            set
            {
                UpdateConfig("boolShowPaging", value ? "1" : "0");
            }
        }

        [Description("intShowRankType")]
        public static Enumerators.RankType ShowRankType
        {
            get { return ConfigurationManager.AppSettings["intShowRankType"] == null ? Enumerators.RankType.Both : (Enumerators.RankType)Convert.ToInt32(ConfigurationManager.AppSettings["intShowRankType"]); }
            set { UpdateConfig("intShowRankType", ((int)value).ToString()); }
        }

        [Description("intMinAge")]
        public static int MinAge
        {
            get
            {

                return Int32.Parse(ConfigurationManager.AppSettings["intMinAge"]);
            }
            set
            {
                UpdateConfig("intMinAge", value.ToString());
            }
        }

        [Description("boolRestrictReg")]
        public static bool RestrictRegistration
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolRestrictReg"] == "1");
            }
            set
            {
                UpdateConfig("boolRestrictReg", value ? "1" : "0");
            }
        }

        [Description("smtpAdminEmail")]
        public static string AdminEmail
        {
            get
            {
                SmtpSection smtpSec = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                return smtpSec.From;
            }
        }

        [Description("boolUserNameFilter")]
        public static bool FilterUsernames
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolUserNameFilter"] == "1");
            }
            set
            {
                UpdateConfig("boolUserNameFilter", value ? "1" : "0");
            }
        }

        [Description("boolUseSpamService")]
        public static bool UseSpamService
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolUseSpamService"] == "1");
            }
            set
            {
                UpdateConfig("boolUseSpamService", value ? "1" : "0");
            }
        }

        [Description("intMemberPageSize")]
        public static int MemberPageSize
        {
            get { return ConfigurationManager.AppSettings["intMemberPageSize"] == null ? 20 : Convert.ToInt32(ConfigurationManager.AppSettings["intMemberPageSize"]); }
            set { UpdateConfig("intMemberPageSize", value.ToString()); }
        }

        [Description("boolEncrypt")]
        public static bool Encrypt
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolEncrypt"] == "1");
            }
            set
            {
                UpdateConfig("boolEncrypt", value ? "1" : "0");
            }
        }

        [Description("boolFloodCheck")]
        public static bool FloodCheck
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolFloodCheck"] == "1");
            }
            set
            {
                UpdateConfig("boolFloodCheck", value ? "1" : "0");
            }
        }

        [Description("intFloodCheckTime")]
        public static int FloodTimeout
        {
            get { return ConfigurationManager.AppSettings["intFloodCheckTime"] == null ? 20 : Convert.ToInt32(ConfigurationManager.AppSettings["intFloodCheckTime"]); }
            set { UpdateConfig("intFloodCheckTime", value.ToString()); }

        }

        [Description("boolShowQuickReply")]
        public static bool ShowQuickReply
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowQuickReply"] == "1");
            }
            set
            {
                UpdateConfig("boolShowQuickReply", value ? "1" : "0");
            }
        }

        [Description("boolShowFileUpload")]
        public static bool ShowFileUpload
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowFileUpload"] == "1");
            }
            set
            {
                UpdateConfig("boolShowFileUpload", value ? "1" : "0");
            }
        }

        [Description("strHomeUrl")]
        public static string HomeUrl
        {
            get
            {
                string url = ConfigurationManager.AppSettings["strHomeUrl"];
                if (!url.EndsWith("/"))
                    url += "/";
                return url;
            }
            set
            {
                UpdateConfig("strHomeUrl", value);
            }
        }

        [Description("boolShowTopicNav")]
        public static bool ShowTopicNav
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowTopicNav"] == "1");
            }
            set
            {
                UpdateConfig("boolShowTopicNav", value ? "1" : "0");
            }
        }

        [Description("boolIPLogging")]
        public static bool LogIP
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolIPLogging"] == "1");
            }
            set
            {
                UpdateConfig("boolIPLogging", value ? "1" : "0");
            }
        }

        [Description("boolEditedByDate")]
        public static bool ShowEditBy
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolEditedByDate"] == "1");
            }
            set
            {
                UpdateConfig("boolEditedByDate", value ? "1" : "0");
            }
        }

        [Description("boolShowSendToFriend")]
        public static bool SendTopic
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowSendToFriend"] == "1");
            }
            set
            {
                UpdateConfig("boolShowSendToFriend", value ? "1" : "0");
            }
        }

        [Description("boolShowPrinterFriendly")]
        public static bool PrintTopic
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowPrinterFriendly"] == "1");
            }
            set
            {
                UpdateConfig("boolShowPrinterFriendly", value ? "1" : "0");
            }
        }

        [Description("boolShowSmiliesTable")]
        public static bool EmoticonTable
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowSmiliesTable"] == "1");
            }
            set
            {
                UpdateConfig("boolShowSmiliesTable", value ? "1" : "0");
            }
        }

        [Description("boolMoveNotify")]
        public static bool MoveNotify
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolMoveNotify"] == "1" && UseEmail);
            }
            set
            {
                UpdateConfig("boolMoveNotify", value ? "1" : "0");
            }

        }

        [Description("ping")]
        public static bool PingSearchEngine
        {
            get
            {
                return (ConfigurationManager.AppSettings["ping"] == "1");
            }
            set
            {
                UpdateConfig("ping", value ? "1" : "0");
            }
        }

        [Description("strForumDescription")]
        public static string ForumDescription
        {
            get
            {
                return ConfigurationManager.AppSettings["strForumDescription"];
            }
            set
            {
                UpdateConfig("strForumDescription", value);
            }
        }

        [Description("intActivePoll")]
        public static int ActivePoll
        {
            get { return ConfigurationManager.AppSettings["intActivePoll"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intActivePoll"]); }
            set { UpdateConfig("intActivePoll", value.ToString()); }
        }

        [Description("boolUseEmail")]
        public static bool UseEmail
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolUseEmail"] == "1" && EmailHost != "SMTPSERVER");
            }
            set
            {
                UpdateConfig("boolUseEmail", value ? "1" : "0");
            }
        }

        [Description("boolUniqueEmail")]
        public static bool UniqueEmail
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolUniqueEmail"] == "1");
            }
            set
            {
                UpdateConfig("boolUniqueEmail", value ? "1" : "0");
            }
        }

        [Description("boolEmailValidation")]
        public static bool EmailValidation
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolEmailValidation"] == "1");
            }
            set
            {
                UpdateConfig("boolEmailValidation", value ? "1" : "0");
            }
        }

        [Description("boolLogonForEmail")]
        public static bool LogonForEmail
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolLogonForEmail"] == "1");
            }
            set
            {
                UpdateConfig("boolLogonForEmail", value ? "1" : "0");
            }
        }

        [Description("intSubscriptionLevel")]
        public static Enumerators.SubscriptionLevel SubscriptionLevel
        {
            get { return ConfigurationManager.AppSettings["intSubscriptionLevel"] == null ? 0 : (Enumerators.SubscriptionLevel)Convert.ToInt32(ConfigurationManager.AppSettings["intSubscriptionLevel"]); }
            set { UpdateConfig("intSubscriptionLevel", ((int)value).ToString()); }
        }

        [Description("boolModeration")]
        public static bool Moderation
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolModeration"] == "1");
            }
            set
            {
                UpdateConfig("boolModeration", value ? "1" : "0");
            }
        }

        [Description("boolUserGalleries")]
        public static bool UserGallery
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolUserGalleries"] == "1");
            }
            set
            {
                UpdateConfig("boolUserGalleries", value ? "1" : "0");
            }
        }

        [Description("boolShowGalleries")]
        public static bool ShowGallery
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowGalleries"] == "1");
            }
            set
            {
                UpdateConfig("boolShowGalleries", value ? "1" : "0");
            }
        }

        [Description("boolShowRightColumn")]
        public static bool ShowRightColumn
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowRightColumn"] == "1"); 
            }
            set { UpdateConfig("boolShowRightColumn", value ? "1" : "0"); }
        }

        [Description("boolShowAnnouncement")]
        public static bool Announcement
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowAnnouncement"] == "1");
            }
            set { UpdateConfig("boolShowAnnouncement", value ? "1" : "0"); }
        }

        [Description("boolShowSideAds")]
        public static bool ShowSideAds
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowSideAds"] == "1");
            }
            set { UpdateConfig("boolShowSideAds", value ? "1" : "0"); }
        }

        [Description("boolShowHeaderAds")]
        public static bool ShowHeaderAds
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowHeaderAds"] == "1");
            }
            set { UpdateConfig("boolShowHeaderAds", value ? "1" : "0"); }
        }

        [Description("boolShowGoogleAds")]
        public static bool ShowGoogleAds
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolShowGoogleAds"] == "1");
            }
            set { UpdateConfig("boolShowGoogleAds", value ? "1" : "0"); }
        }

        [Description("strGoogleAdCode")]
        public static string GoogleAdCode
        {
            get { return ConfigurationManager.AppSettings["strGoogleAdCode"] ?? ""; }
            set { UpdateConfig("strGoogleAdCode", value); }
        }

        [Description("boolDebugMode")]
        public static bool DebugMode
        {
            get
            {
                return (ConfigurationManager.AppSettings["boolDebugMode"] == "1");
            }
        }

        [Description("intAdminUserId")]
        public static int AdminUserId
        {
            get { return ConfigurationManager.AppSettings["intAdminUserId"] == null ? 1 : Convert.ToInt32(ConfigurationManager.AppSettings["intAdminUserId"]); }
            set { UpdateConfig("intAdminUserId", value.ToString()); }
        }

        [Description("boolPrivateMessaging")]
        public static bool PrivateMessaging
        {
            get { return ConfigurationManager.AppSettings["boolPrivateMessaging"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["boolPrivateMessaging"]) == 1; }
            set { UpdateConfig("boolPrivateMessaging", value ? "1" : "0"); }
        }

        [Description("boolArchiveState")]
        public static bool Archive
        {
            get { return ConfigurationManager.AppSettings["boolArchiveState"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["boolArchiveState"]) == 1; }
            set { UpdateConfig("boolArchiveState", value ? "1" : "0"); }            
        }

        [Description("boolAllowSearchAllForums")]
        public static bool AllowSearchAllForums
        {
            get { return ConfigurationManager.AppSettings["boolAllowSearchAllForums"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["boolAllowSearchAllForums"]) == 1; }
            set { UpdateConfig("boolAllowSearchAllForums", value ? "1" : "0"); }
        }

        public static string ForumTablePrefix
        {
            get { return ConfigurationManager.AppSettings["strTablePrefix"]; }

        }
        public static string MemberTablePrefix
        {
            get { return ConfigurationManager.AppSettings["strMemberTablePrefix"]; }

        }
        public static string FilterTablePrefix
        {
            get { return ConfigurationManager.AppSettings["strFilterTablePrefix"]; }

        }

        [Description("strCookiePath")]
        public static string CookiePath
        {
            get { return ConfigurationManager.AppSettings["strCookiePath"]=="" ? "/" : ConfigurationManager.AppSettings["strCookiePath"]; }
            set { UpdateConfig("strCookiePath", value); }
        }
        [Description("strAnnounceAnon")]
        public static string AnonMessage
        {
            get { return ConfigurationManager.AppSettings["strAnnounceAnon"]; }
            set { UpdateConfig("strAnnounceAnon", value); }
        }
        [Description("strAnnounceAuth")]
        public static string AuthMessage
        {
            get { return ConfigurationManager.AppSettings["strAnnounceAuth"]; }
            set { UpdateConfig("strAnnounceAuth", value); }
        }


        [Description("intPreferredPasswordLength")]
        public static int PreferredPasswordLength
        {
            get { return ConfigurationManager.AppSettings["intPreferredPasswordLength"] == null ? 10 : Convert.ToInt32(ConfigurationManager.AppSettings["intPreferredPasswordLength"]); }
            set { UpdateConfig("intPreferredPasswordLength", value.ToString()); }
        }
        [Description("intMinimumNumericCharacters")]
        public static int MinimumNumericCharacters
        {
            get { return ConfigurationManager.AppSettings["intMinimumNumericCharacters"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intMinimumNumericCharacters"]); }
            set { UpdateConfig("intMinimumNumericCharacters", value.ToString()); }
        }
        [Description("intMinimumSymbolCharacters")]
        public static int MinimumSymbolCharacters
        {
            get { return ConfigurationManager.AppSettings["intMinimumSymbolCharacters"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intMinimumSymbolCharacters"]); }
            set { UpdateConfig("intMinimumSymbolCharacters", value.ToString()); }
        }
        [Description("intMinimumLowerCaseCharacters")]
        public static int MinimumLowerCaseCharacters
        {
            get { return ConfigurationManager.AppSettings["intMinimumLowerCaseCharacters"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intMinimumLowerCaseCharacters"]); }
            set { UpdateConfig("intMinimumLowerCaseCharacters", value.ToString()); }
        }
        [Description("intMinimumUpperCaseCharacters")]
        public static int MinimumUpperCaseCharacters
        {
            get { return ConfigurationManager.AppSettings["intMinimumUpperCaseCharacters"] == null ? 0 : Convert.ToInt32(ConfigurationManager.AppSettings["intMinimumUpperCaseCharacters"]); }
            set { UpdateConfig("intMinimumUpperCaseCharacters", value.ToString()); }
        }

        [Description("boolRequiresUpperAndLowerCaseCharacters")]
        public static bool RequiresUpperAndLowerCaseCharacters
        {
            get { return ConfigurationManager.AppSettings["boolAllowSearchAllForums"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["boolRequiresUpperAndLowerCaseCharacters"]) == 1; }
            set { UpdateConfig("boolRequiresUpperAndLowerCaseCharacters", value ? "1" : "0"); }
        }
        //TopicAvatar
        [Description("boolTopicAvatar")]
        public static bool TopicAvatar
        {
            get { return ConfigurationManager.AppSettings["boolTopicAvatar"] != null && Convert.ToInt32(ConfigurationManager.AppSettings["boolTopicAvatar"]) == 1; }
            set { UpdateConfig("boolTopicAvatar", value ? "1" : "0"); }
        }
        #endregion

        #region Config Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void UpdateConfig(string key, string value)
        {
            XmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "snitz.config");
            UpdateKey(key, value);
            SaveAppSettings();
        }

        /// <summary>
        /// Updates the appsettings key with a new value
        /// </summary>
        /// <param name="strKey">Key to update</param>
        /// <param name="newValue">ne value for the key</param>
        private static void UpdateKey(string strKey, string newValue)
        {
            if (!KeyExists(strKey))
            {
                AddKey(strKey, newValue);
                return;
            }
            XmlNode appSettingsNode = XmlDoc.SelectSingleNode("appSettings");
            foreach (XmlNode childNode in appSettingsNode)
            {
                if ((childNode.NodeType == XmlNodeType.Element) && (childNode.Name == "add"))
                    if (childNode.Attributes["key"].Value == strKey)
                        childNode.Attributes["value"].Value = newValue;
            }

        }

        /// <summary>
        /// Adds a new key + value to the appsettings files
        /// </summary>
        /// <param name="strKey">Key to add to appsettings</param>
        /// <param name="newValue">value for the new setting</param>
        private static void AddKey(string strKey, string newValue)
        {
            XmlNode appSettingsNode = XmlDoc.SelectSingleNode("appSettings");

            XmlElement newnode = XmlDoc.CreateElement("add");
            newnode.SetAttribute("key", strKey);
            newnode.SetAttribute("value", newValue);

            appSettingsNode.AppendChild(newnode);

        }

        /// <summary>
        /// Checks if the key exists in the appsettings file
        /// </summary>
        /// <param name="strKey">Key to look for</param>
        /// <returns></returns>
        private static bool KeyExists(string strKey)
        {
            XmlNode appSettingsNode = XmlDoc.SelectSingleNode("appSettings");

            var result = appSettingsNode != null &&
                         appSettingsNode.Cast<XmlNode>().Where(n => n.NodeType != XmlNodeType.Comment).Any(childNode => childNode.Attributes["key"].Value == strKey);
            return result;
        }

        /// <summary>
        /// Save the appsettings file
        /// </summary>
        private static void SaveAppSettings()
        {
            //save the appsettings file snitz.config
            XmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "snitz.config");
            //Touch the web.config file to force the Application Settings to reload
            XmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "web.config");
            XmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory + "web.config");
            //reload the appsettings
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Removes a key from the appsettings file
        /// </summary>
        /// <param name="strKey">key to remove</param>
        private static void DeleteKey(string strKey)
        {
            throw new Exception("Can not delete config keys");
        }

        /// <summary>
        /// Updates a collection of appsettings key value pairs
        /// </summary>
        /// <param name="toUpdate">Dictionary of appsettings to update</param>
        public static void UpdateKeys(Dictionary<string, string> toUpdate)
        {
            XmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "snitz.config");
            foreach (KeyValuePair<string, string> update in toUpdate)
            {

                UpdateKey(update.Key, update.Value);
                
            }
            SaveAppSettings();
        }

        #endregion

    }
}
