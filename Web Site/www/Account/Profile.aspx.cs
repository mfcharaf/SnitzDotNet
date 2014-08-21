/*
####################################################################################################################
##
## SnitzUI.Account - Profile.aspx
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
using System.IO;
using System.Net.Mail;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Resources;
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using Snitz.Providers;
using SnitzConfig;

using SnitzMembership;

namespace SnitzUI
{
    public partial class ProfilePage : PageBase
    {
        private bool _editmode;
        private bool _isMyProfile;
        private string _userProfile;
        private MemberInfo _user;
        private ProfileCommon _profile;
        private string _cancelUrl;
        private List<SnitzLink> _weblinks;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if(!IsAuthenticated)
            {
                throw new SecurityException("You must be a logged in member to view users profiles");
            }
            editorCSS.Attributes.Add("href", "/css/" + Page.Theme + "/editor.css");
            if (webResources.TextDirection == "rtl")
                pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/profilepagertl.css");
            else
                pageCSS.Attributes.Add("href", "/css/" + Page.Theme + "/profilepage.css");

            TabContainer1.ActiveTabIndex = 0;
            if (HttpContext.Current.Items["user"] != null)
            {
                _userProfile = HttpContext.Current.Items["user"].ToString();
            }else
                _userProfile = !String.IsNullOrEmpty(Request.Params["user"]) ? Request.Params["user"] : Member.Username;
            if (Session["CurrentProfile"] == null)
                Session.Add("CurrentProfile", _userProfile);
            else
                _userProfile = Session["CurrentProfile"].ToString();
            _user = Members.GetMember(_userProfile);

            _profile = ProfileCommon.GetUserProfile(_userProfile);
            Page.Title = String.Format(webResources.lblProfile, _userProfile);
            _isMyProfile = _userProfile == Member.Username;
            
            if (!IsPostBack)
            {
                _weblinks = _profile.FavLinks;
                Session.Add("WEBLINK", _weblinks);
            }else
            {
                _weblinks = (List<SnitzLink>)Session["WEBLINK"];
                SetupDynamicControls();
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                SetupTabs();
                SetupDynamicControls();
                if (ViewState["CancelUrl"] == null || String.IsNullOrEmpty(_cancelUrl))
                {
                    if (Request.UrlReferrer != null) _cancelUrl = Request.UrlReferrer.OriginalString;
                    if (!string.IsNullOrEmpty(_cancelUrl))
                        ViewState.Add("CancelUrl", _cancelUrl);
                }
            }
            else
            {
                _cancelUrl = (string) ViewState["CancelUrl"];
            }
            AsyncFileUpload1.UploaderStyle = AsyncFileUpload.UploaderStyleEnum.Traditional;
            AsyncFileUpload1.UploadedComplete += AsyncFileUpload1UploadedComplete;
            AsyncFileUpload1.UploadedFileError += AsyncFileUpload1UploadedFileError;
            Page.Form.DefaultButton = btnUpdate.UniqueID;
        }
        
        private void SetupDynamicControls()
        {
            
            _editmode = _editmode || _isMyProfile;
            
            if (_editmode)
            {   //Quote
                var tbxQuote = new TextBox
                {
                    ID = "tbxQuote",
                    TextMode = TextBoxMode.MultiLine,
                    CssClass = "QRMsgArea",
                    Height = new Unit(99, UnitType.Pixel),
                    Text = _user.FavouriteQuote
                };
                phQuote.Controls.Add(tbxQuote);

                var tbxSig = new TextBox
                {
                    ID = "tbxSig",
                    CssClass = "QRMsgArea",
                    TextMode = TextBoxMode.MultiLine,

                    Height = new Unit(99, UnitType.Pixel),
                    Text = _user.Signature.Replace(">", "]").Replace("<", "[")
                };
                phSig.Controls.Add(tbxSig);

                var tbxBiog = new TextBox
                {
                    ID = "tbxBiog",
                    TextMode = TextBoxMode.MultiLine,
                    CssClass = "QRMsgArea",
                    Height = new Unit(99, UnitType.Pixel),
                    Text = _user.Biography
                };
                phBiog.Controls.Add(tbxBiog);

                var tbxHobby = new TextBox
                {
                    ID = "tbxHobby",
                    TextMode = TextBoxMode.MultiLine,
                    CssClass = "QRMsgArea",
                    Height = new Unit(99, UnitType.Pixel),
                    Text = _user.Hobbies
                };
                phHobby.Controls.Add(tbxHobby);

                var tbxNews = new TextBox
                {
                    ID = "tbxNews",
                    TextMode = TextBoxMode.MultiLine,
                    CssClass = "QRMsgArea",
                    Height = new Unit(99, UnitType.Pixel),
                    Text = _user.LatestNews
                };
                phNews.Controls.Add(tbxNews);

                lblHomePage.Visible = true;
                lblHomePage.AssociatedControlID = "tbxHomePage";
                var tbxHomePage = new TextBox
                {
                    ID = "tbxHomePage",
                    TextMode = TextBoxMode.SingleLine,
                    Text = _user.HomePage
                };
                phHomePage.Controls.Add(tbxHomePage);

            }
            else
            {
                var tbxQuote = new Label
                {
                    ID = "lblQuote",
                    Width = new Unit(100, UnitType.Percentage),
                    Height = new Unit(),
                    Text = _user.FavouriteQuote
                };
                phQuote.Controls.Add(tbxQuote);

                var tbxSig = new Label
                {
                    ID = "lblSig",
                    CssClass = "bbcode",
                    Width = new Unit(100, UnitType.Percentage),
                    Height = new Unit(),
                    Text = @"<hr/>" + _user.Signature
                };
                phSig.Controls.Add(tbxSig);

                var tbxBiog = new Label
                {
                    ID = "lblBiog",
                    CssClass = "bbcode",
                    Width = new Unit(100, UnitType.Percentage),
                    Height = new Unit(),
                    Text = _user.Biography
                };
                phBiog.Controls.Add(tbxBiog);

                var tbxHobby = new Label
                {
                    ID = "lblHobby",
                    CssClass = "bbcode",
                    Width = new Unit(100, UnitType.Percentage),
                    Height = new Unit(),
                    Text = _user.Hobbies
                };
                phHobby.Controls.Add(tbxHobby);

                var tbxNews = new Label
                {
                    ID = "lblNews",
                    CssClass = "bbcode",
                    Width = new Unit(100, UnitType.Percentage),
                    Height = new Unit(),
                    Text = _user.LatestNews
                };
                phNews.Controls.Add(tbxNews);

                if (_user.HomePage != null)
                    _user.HomePage = _user.HomePage.Trim();
                string link = _user.HomePage;
                if (link != "")
                {
                    lblHomePage.Visible = false;
                    var tbxHomePage = new HyperLink
                    {
                        ID = "lnkHomePage",
                        Width = new Unit(100, UnitType.Percentage),
                        Height = new Unit(),
                        NavigateUrl = _user.HomePage,
                        Text = webResources.lblVisitHomePage
                    };
                    phHomePage.Controls.Add(tbxHomePage);
                }
                else
                {
                    lblHomePage.Text = webResources.NoHomePage;
                }
            }

            repFavLinks.DataSource = _weblinks;
            repFavLinks.DataBind();
            if (Config.UserGallery)
            {
                TabGallery.Visible = true;
                TabGallery.HeaderText = "Images";
                dlImages.DataSource = GalleryFunctions.GetImages(_userProfile);
                dlImages.DataBind();
            }
            else
            {
                TabGallery.Visible = false;
                TabGallery.HeaderText = "Images";                
            }

        }
        
        protected void CancelClick(object sender, EventArgs eventArgs)
        {
            Response.Redirect(_cancelUrl, true);
        }

        protected void UpdateClick(object sender, EventArgs eventArgs)
        {
            SaveMemberProfile();
        }

        private void SaveMemberProfile()
        {
            _profile = ProfileCommon.GetUserProfile(_userProfile);
            _profile.Gravatar = cbxGravatar.Checked;
            _profile.HideAge = cbxHideAge.Checked;
            _profile.Skype = tbxSkype.Text;
            _profile.PublicGallery = cbxPublic.Checked && Config.ShowGallery;
            _profile.LinkTarget = ddlTarget.SelectedValue;
            _profile.Save();

            string folderPath = "/gallery/";
            folderPath += _userProfile + "/public.txt";
            string path = Server.MapPath("~" + folderPath);
            if (_profile.PublicGallery && Config.ShowGallery)
            {
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
            else
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }                
            }
            
            if (tbxRealName.Text.Trim() != "")
            {
                int i = tbxRealName.Text.IndexOf(" ");
                _user.Firstname = i > 0 ? tbxRealName.Text.Substring(0, tbxRealName.Text.IndexOf(" ")).Trim() : tbxRealName.Text.Trim();

                _user.Lastname = tbxRealName.Text.Replace(_user.Firstname, "").Trim();
            }
            _user.Age = Common.GetAgeFromDOB(DobPicker.DOBStr);
            _user.DateOfBirth = DobPicker.DOBStr;
            if(ddlMarStatus.SelectedIndex >= 0)
                _user.MaritalStatus = ddlMarStatus.SelectedValue;
            if (ddlGender.SelectedIndex >= 0)
                _user.Gender = ddlGender.SelectedValue;

            _user.State = tbxState.Text;
            _user.City = tbxCity.Text;
            _user.Country = tbxCountry.Text;
            _user.Occupation = tbxOccupation.Text;
            _user.Title = tbxForumTitle.Text;
            if (ddlTimeZone.SelectedIndex >= 0)
            {
                TimeZoneInfo tzone = TimeZoneInfo.FindSystemTimeZoneById(ddlTimeZone.SelectedValue);
                _user.TimeZone = ddlTimeZone.SelectedValue;
                _user.TimeOffset = tzone.BaseUtcOffset.TotalHours;
            }
            _user.UseDaylightSaving = cbxDaylightSaving.Checked;
            //email
            _user.ReceiveEmails = cbxReceiveEmail.Checked;
            _user.HideEmail = cbxHideEmail.Checked;
            _user.Yahoo = tbxYAHOO.Text;
            _user.AIM = tbxAIM.Text;
            _user.Skype = tbxSkype.Text;
            _user.ICQ = tbxICQ.Text;
            
            _user.FavouriteQuote = ((TextBox)phQuote.FindControl("tbxQuote")).Text;
            _user.Signature = ((TextBox)phSig.FindControl("tbxSig")).Text;
            _user.Biography = ((TextBox)phBiog.FindControl("tbxBiog")).Text;
            _user.UseSignature = cbxUseSig.Checked;
            _user.ViewSignatures = cbxViewSig.Checked;

            _user.LatestNews = ((TextBox)phNews.FindControl("tbxNews")).Text;
            _user.Hobbies = ((TextBox)phHobby.FindControl("tbxHobby")).Text;
            _user.HomePage = ((TextBox)phHomePage.FindControl("tbxHomePage")).Text;
            _user.Theme = ddTheme.Text;
            Config.UserTheme = _user.Theme;
            //fav links

            Members.SaveMember(_user);
        }

        private void SetupTabs()
        {
            _editmode = _editmode || _isMyProfile;

            SetControlStatus();

            tbxName.Text = _user.Username;
            tbxRealName.Text = String.Format("{0} {1}", _user.Firstname, _user.Lastname);
            tbxAge.Text = Common.TranslateNumerals(Common.GetAgeFromDOB(_user.DateOfBirth));
            if (_profile.HideAge && _userProfile != Member.Username)
                tbxAge.Text = @"Mind your own business";
            ddlMarStatus.SelectedValue = _user.MaritalStatus;
            ddlGender.SelectedValue = _user.Gender;
            tbxState.Text = _user.State;
            tbxCity.Text = _user.City;
            tbxCountry.Text = _user.Country;
            tbxOccupation.Text = _user.Occupation;
            tbxForumTitle.Text = _user.Rank.Title;
            if(_user.DateOfBirth.Trim() != "")
            {
                var dateTime = _user.DateOfBirth.ToDateTime();
                if (dateTime != null) DobPicker.SetDOB(dateTime.Value);
            }
            cbxDaylightSaving.Checked = _user.UseDaylightSaving;
            if (!String.IsNullOrEmpty(_user.TimeZone))
            {
                ddlTimeZone.SelectedValue = _user.TimeZone;
            }
            ddTheme.SelectedValue = Config.UserTheme;
            if (_profile.Gravatar)
            {
                var grav = new Gravatar
                {
                    ID = "imgAvatar",
                    Email = _user.Email,
                    
                    DefaultImageBehavior = GravatarDefaultImageBehavior.Default,
                    Rating = GravatarRating.Default
                };
                if(_user.AvatarUrl != "")
                    grav.DefaultImage = _user.AvatarUrl;
                phAvatar.Controls.Add(grav);
            }else
            {
                var img = new Literal {Text = _user.AvatarUrl};
                phAvatar.Controls.Add(img);
            }

            cbxReceiveEmail.Checked = _user.ReceiveEmails;
            cbxHideEmail.Checked = _user.HideEmail;
            cbxUseSig.Checked = _user.UseSignature;
            cbxViewSig.Checked = _user.ViewSignatures;
            cbxHideAge.Checked = _profile.HideAge;
            cbxGravatar.Checked = _profile.Gravatar;

            tbxSkype.Text = _profile.Skype;
            tbxYAHOO.Text = _user.Yahoo;
            tbxAIM.Text = _user.AIM;
            tbxSkype.Text = _user.Skype;
            tbxICQ.Text = _user.ICQ;
            
            repBookMarks.DataSource = _profile.BookMarks;
            repBookMarks.DataBind();

            rptRecentTopics.DataSource = Members.GetRecentTopics(_user.Id,Member);
            rptRecentTopics.DataBind();

            string[] roles = Roles.GetRolesForUser(_userProfile);
            LitRoles.Text = String.Join("<br/>",roles);
            lblUserId.Text += @" : " + _user.Id;
            lblPosts.Text += @" : " + _user.PostCount;
            lblSince.Text += @" : " + Members.MemberSinceTimeAgo(_user);
            lblVisit.Text += @" : " + Members.LastVisitTimeAgo(_user);

            cbxPublic.Checked = _profile.PublicGallery;
            grdSubs.DataSource = Subscriptions.GetMemberSubscriptions(_user.Id);
            grdSubs.DataBind();

        }

        private void SetControlStatus()
        {
            cbxGravatar.Visible = _editmode;
            cbxHideAge.Visible = _editmode;
            cbxReceiveEmail.Visible = _editmode;
            cbxHideEmail.Visible = _editmode;
            cbxUseSig.Visible = _editmode;
            cbxViewSig.Visible = _editmode;

            ddlMarStatus.Enabled = _editmode;
            ddlGender.Enabled = _editmode;
            DobPicker.Enabled = _editmode;
            ddlTimeZone.Enabled = _editmode;
            ddTheme.Enabled = _editmode;
            ddlLang.Enabled = _editmode;
            ddlTarget.Enabled = _editmode;

            tbxName.ReadOnly = !_editmode;
            tbxRealName.ReadOnly = !_editmode;
            tbxAge.ReadOnly = true;
            tbxState.ReadOnly = !_editmode;
            tbxCity.ReadOnly = !_editmode;
            tbxCountry.ReadOnly = !_editmode;
            tbxOccupation.ReadOnly = !_editmode;
            tbxForumTitle.ReadOnly = !IsAdministrator;
            tbxSkype.ReadOnly = !_editmode;
            tbxYAHOO.ReadOnly = !_editmode;
            tbxAIM.ReadOnly = !_editmode;
            tbxSkype.ReadOnly = !_editmode;
            tbxICQ.ReadOnly = !_editmode;
            if(!_editmode)
            {
                tbxName.CssClass = "textToLabel";
                tbxRealName.CssClass = "textToLabel";
                tbxAge.CssClass = "textToLabel";
                tbxState.CssClass = "textToLabel";
                tbxCity.CssClass = "textToLabel";
                tbxCountry.CssClass = "textToLabel";
                tbxOccupation.CssClass = "textToLabel";
                tbxForumTitle.CssClass = "textToLabel";
                tbxSkype.CssClass = "textToLabel";
                tbxYAHOO.CssClass = "textToLabel";
                tbxAIM.CssClass = "textToLabel";
                tbxSkype.CssClass = "textToLabel";
                tbxICQ.CssClass = "textToLabel";
                showPass.Visible = false;
            }
            newemail.Visible = _editmode;

            pnlDOB.Visible = _isMyProfile || _editmode;
            pnlSiteInf.Visible = _isMyProfile || _editmode;
            btnUpdate.Visible = _editmode;
            btnCancel.Visible = _editmode;
            btnEdit.Visible = IsAdministrator && !_editmode;

            btnAddLink.Enabled = _editmode;
            btnAddLink.Visible = _editmode && _isMyProfile;
            btnSaveLinks.Visible = _editmode && _isMyProfile;
            btnChangeEmail.Visible = _editmode;
            btnAvatar.Visible = _editmode;
            repFavLinks.Visible = _editmode;
            TabBookmarks.Visible = _isMyProfile || IsAdministrator;
            pnlPassword.Visible = _isMyProfile;
            TabGallery.Visible = (_isMyProfile && Config.UserGallery) || IsAdministrator || (_profile.PublicGallery && Config.ShowGallery && Config.UserGallery);
            cbxPublic.Visible = Config.ShowGallery;
            TabSubscriptions.Visible = (Config.SubscriptionLevel != Enumerators.SubscriptionLevel.None);
        }
        
        private Image GetTopicIcon(TopicInfo topic)
        {
            var image = new Image { ID = "imgTopicIcon" };
            string _new = "";
            string locked;
            string sticky = "";

            if (topic.LastPostDate > LastVisitDateTime)
            {
                image.AlternateText = webResources.lblNewPosts;
                image.ToolTip = webResources.lblNewPosts;
                _new = "New";
            }

            switch ((Enumerators.PostStatus)topic.Status)
            {
                case Enumerators.PostStatus.Open:
                    locked = "";
                    image.ToolTip = webResources.lblOldPosts;
                    break;

                default:
                    locked = "Locked";
                    image.AlternateText = webResources.lblLockedTopic;
                    image.ToolTip = webResources.lblTopicLocked;
                    break;
            }
            if (topic.IsSticky)
            {
                sticky = "Sticky";
                image.AlternateText = webResources.lblStick;
                image.ToolTip = locked == "" ? webResources.lblStick : webResources.lblStick + " " + webResources.lblTopicLocked;
            }

            image.SkinID = "Folder" + _new + sticky + locked;


            return image;
        }

        private void SendEmail(MembershipUser sender)
        {
            string mailFile = Server.MapPath("~/App_Data/ChangeMail.html");

            string strSubject = "Sent From " + Regex.Replace(Config.ForumTitle, @"&\w+;", "") + ": Email change request";

                var builder = new UriBuilder("http",
                                                    Request.Url.DnsSafeHost,
                                                    Request.Url.Port, Page.ResolveUrl("~/Account/activate.aspx"), string.Format("?C={0}&E=T", SnitzMembershipProvider.CreateValidationCode(sender)));

                var file = new StreamReader(mailFile);
                string msgBody = file.ReadToEnd();
                msgBody = msgBody.Replace("<%UserName%>", sender.UserName);
                msgBody = msgBody.Replace("<%ForumTitle%>", Config.ForumTitle);
                msgBody = msgBody.Replace("<%validationURL%>", builder.Uri.AbsoluteUri);

                var mailsender = new SnitzEmail
                                            {
                                                toUser = new MailAddress(newemail.Text, sender.UserName),
                                                FromUser = "Administrator",
                                                subject = strSubject,
                                                msgBody = msgBody
                                            };
                mailsender.Send();

        }
        
        protected override SiteMapNode OnSiteMapResolve(SiteMapResolveEventArgs e)
        {
            if (SiteMap.CurrentNode != null)
            {
                SiteMapNode currentNode = SiteMap.CurrentNode.Clone(true);
                SiteMapNode tempNode = currentNode;

                tempNode.Title = _userProfile;
                return currentNode;
            }
            return null;
        }

        protected void RecentTopicsDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var thisrow = (TopicInfo)(e.Row.DataItem);
                Image img = GetTopicIcon(thisrow);
                img.ApplyStyleSheetSkin(Page);
                e.Row.Cells[0].Controls.Add(img);
            }
        }

        protected void WebLinksDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                if (e.Item.FindControl("EditLinks") != null)
                    e.Item.FindControl("EditLinks").Visible = _editmode;
                if (e.Item.FindControl("ViewLinks") != null)
                    e.Item.FindControl("ViewLinks").Visible = !_editmode;
            }
        }

        protected void SaveLinkClick(object sender, EventArgs e)
        {
            var links = new List<SnitzLink>();
            if (repFavLinks.Items.Count > 0)
            {
                for (int count = 0; count < repFavLinks.Items.Count; count++)
                {
                    var name = (TextBox)repFavLinks.Items[count].FindControl("tbxName");
                    var url = (TextBox)repFavLinks.Items[count].FindControl("tbxUrl");
                    if (url != null)
                    {
                        if(!String.IsNullOrEmpty(url.Text))
                            links.Add(new SnitzLink(name.Text,url.Text,count));
                    }
                }
            } 
            _profile.FavLinks = links;
            _profile.Save();
            Session["WEBLINK"] = links;
            repFavLinks.DataSource = links;
            repFavLinks.DataBind();
        }

        protected void AddLinkClick(object sender, EventArgs e)
        {
            var links = new List<SnitzLink>();
            repFavLinks.DataSource = _weblinks;
            if (repFavLinks.Items.Count > 0)
            {
                for (int count = 0; count < repFavLinks.Items.Count; count++)
                {
                    var name = (TextBox)repFavLinks.Items[count].FindControl("tbxName");
                    var url = (TextBox)repFavLinks.Items[count].FindControl("tbxUrl");
                    if (url != null)
                    {
                        if (!String.IsNullOrEmpty(url.Text))
                            links.Add(new SnitzLink(name.Text, url.Text,count));
                    }
                }
            }
            if (links.Count == 5)
                return;
            links.Add(new SnitzLink("new link", "http://",links.Count));
            if (links.Count == 5)
                btnAddLink.Enabled = false;
            Session["WEBLINK"] = links;
            repFavLinks.DataSource = links;
            repFavLinks.DataBind();
        }

        protected void ChangeEmailClick(object sender, EventArgs e)
        {
            if ((newemail.Text.Trim() == "") || (newemail.Text == @"Enter a new Email .."))
                return;

            var smp = (SnitzMembershipProvider)Membership.Providers["SnitzMembershipProvider"];
            if (smp != null)
            {
                MembershipUser mu = smp.GetUser(HttpContext.Current.User.Identity.Name, false);
                SnitzMembershipProvider.ChangeEmail(mu, false, newemail.Text);
                SendEmail(mu);
            }
            Lemail.Visible = true;
            btnChangeEmail.Visible = false;
            newemail.Visible = false;
        }

        protected void EditClick(object sender, EventArgs eventArgs)
        {
            if (IsAdministrator)
                _editmode = true;
            SetupTabs();
        }

        #region async file upload

        private void AsyncFileUpload1UploadedFileError(object sender, AsyncFileUploadEventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "error", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';});", true);
        }

        private void AsyncFileUpload1UploadedComplete(object sender, AsyncFileUploadEventArgs e)
        {
            string filename = e.FileName;
            if (filename == null)
                return;
            string contentType = AsyncFileUpload1.PostedFile.ContentType;
            if (!contentType.Contains("image"))
                return;
            var fileName = Path.GetFileName(filename);
            if (fileName.Contains(".pdf"))
            {
                AsyncFileUpload1.FailedValidation = true;
                return;
            }
            if (int.Parse(e.FileSize) > 2000000)
            {
                AsyncFileUpload1.FailedValidation = true;
                return;
            }
            string savePath = Page.MapPath(String.Format("~/Avatars/{0}", Path.GetFileName(filename).Replace(" ", "+")));
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "size", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " + AsyncFileUpload1.FileBytes.Length.ToString() + "';});", true);

            AsyncFileUpload1.SaveAs(savePath);
            _user.Avatar = Path.GetFileName(filename).Replace(" ", "+");
            Members.SaveMember(_user);

        }


        #endregion

        protected void BtnChangePassClick(object sender, EventArgs e)
        {
            var smp = new SnitzMembershipProvider();
            bool result = smp.ChangePassword(_userProfile, tbxPassword.Text, tbxNewPass.Text);

            if(result)
            {
                var req = new RequiredFieldValidator
                              {
                                  ValidationGroup = "passChange",
                                  ErrorMessage = "Password changed successfully",
                                  IsValid = false
                              };
                Page.Form.Controls.Add(req);
                req.Visible = false;
            }
            else
            {
                var req = new RequiredFieldValidator
                              {
                                  ValidationGroup = "passChange",
                                  ErrorMessage = "Failed to change password",
                                  IsValid = false
                              };
                Page.Form.Controls.Add(req);
                req.Visible = false;
            }
        }

        protected void TabContainer1ActiveTabChanged(object sender, EventArgs e)
        {

        }

        protected void GrdSubsRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAll")
            {
                Subscriptions.RemoveMemberSubscriptions(_user.Id);
            }
            else
            {
                var row = (GridViewRow) ((Control) e.CommandSource).NamingContainer;
                var dataKey = grdSubs.DataKeys[row.RowIndex];
                if (dataKey != null)
                {
                    int topicid = Convert.ToInt32(dataKey["TopicId"]);
                    int forumid = Convert.ToInt32(dataKey["ForumId"]);

                    if (topicid > 0)
                    {
                        Subscriptions.RemoveTopicSubscription(_user.Id, topicid);
                    }
                    else if (forumid > 0)
                    {
                        Subscriptions.RemoveForumSubscription(_user.Id, forumid);
                    }
                }
            }
        }

        protected void DeleteBookMark(object sender, ImageClickEventArgs e)
        {
            PageBase page = (PageBase)Page;
            var id = ((ImageButton)sender).CommandArgument;

            List<SnitzLink> bookmarks = page.Profile.BookMarks;
            var todelete = bookmarks.Find(i => i.ID == Convert.ToInt32(id));
            bookmarks.Remove(todelete);
            _profile.BookMarks = bookmarks;
            _profile.Save();
            repBookMarks.DataSource = _profile;
            repBookMarks.DataBind();
        }

        protected void ShowPasswordChars(object sender, EventArgs e)
        {
            if (!_editmode)
                return;
            if(showPass.Checked)
            {
                tbxPassword.TextMode = TextBoxMode.SingleLine;
                tbxNewPass.TextMode = TextBoxMode.SingleLine;
                tbxConfirmPass.TextMode = TextBoxMode.SingleLine;
            }
            else
            {
                var pass = tbxPassword.Text;
                var newpass = tbxNewPass.Text;
                var confirm = tbxConfirmPass.Text;
                tbxPassword.TextMode = TextBoxMode.Password;
                tbxNewPass.TextMode = TextBoxMode.Password;
                tbxConfirmPass.TextMode = TextBoxMode.Password;

                tbxPassword.Attributes.Add("value", pass);
                tbxNewPass.Attributes.Add("value", newpass);
                tbxConfirmPass.Attributes.Add("value", confirm);
            }
        }
    }
}