/*
####################################################################################################################
##
## SnitzUI.UserControls - QuickReply.ascx
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
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using AjaxControlToolkit;
using ModConfig;
using Snitz.BLL;
using Snitz.BLL.modconfig;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


public partial class QuickReply : UserControl
{
    private PageBase page;
    public TopicInfo thisTopic { get; set; }
    public string AllowedFileTypes { get; set; }
    public bool AllowAttachments { get; set; }
    public bool AllowImageUploads { get; set; }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        page = (PageBase)this.Page;
        cbxSig.Checked = page.Member.UseSignature;
        ModConfigBase controller = (ModConfigBase)ConfigHelper.ModClass("UploadConfig");

        AllowedFileTypes = "";
        AllowAttachments = Convert.ToBoolean(Convert.ToInt16(controller.ModConfiguration.Settings["AllowAttachments"]));
        AllowImageUploads = Convert.ToBoolean(Convert.ToInt16(controller.ModConfiguration.Settings["AllowImageUpload"]));
        if (AllowImageUploads)
            AllowedFileTypes += controller.ModConfiguration.Settings["AllowedImageTypes"].ToString();
        if (AllowAttachments)
        {
            if (AllowedFileTypes != "")
                AllowedFileTypes += ",";
            AllowedFileTypes += controller.ModConfiguration.Settings["AllowedAttachmentTypes"].ToString();
        }
        if (AllowImageUploads || AllowAttachments)
        {
            string style = ""; //!ShowAttachments ? ".upload{display:none;}" : 
            if (!Config.UserGallery || !AllowImageUploads)
                style += ".browse{display:none;}";

            uploadStyle.Text = !String.IsNullOrEmpty(style) ? string.Format("<style>{0}</style>", style) : "";
        }
        else
        {
            uploadStyle.Text = "<style>.upload{display:none;} .browse{display:none;}</style>";
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = (Roles.IsUserInRole("Member") && thisTopic.Status==(int)Enumerators.PostStatus.Open) || page.IsAdministrator;
        AsyncFileUpload1.UploaderStyle = AsyncFileUpload.UploaderStyleEnum.Modern;
        AsyncFileUpload1.UploadedComplete += AsyncFileUpload1UploadedComplete;
        AsyncFileUpload1.UploadedFileError += AsyncFileUpload1UploadedFileError;

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        DateTime newdate = DateTime.UtcNow;

        if (Session["LastPostMade"] != null)
        {
            if ((Config.FloodCheck)&& !(page.IsAdministrator || page.IsModerator))
            {
                TimeSpan diff1 = new TimeSpan(0, 0, 0, Config.FloodTimeout);
                DateTime dt = newdate - diff1;
                DateTime? lastpost = Session["LastPostMade"].ToString().ToDateTime();
                if (lastpost > dt)
                    throw new HttpException(403, "FloodCheck");
                    //Response.Redirect("error.aspx?msg=errFloodError");
            }
        }
        else
        {
            Session.Add("LastPostMade", DateTime.UtcNow.ToForumDateStr());
        } 
        
        string MemberIP = Common.GetIP4Address();
        ReplyInfo reply = new ReplyInfo
                          {
                              TopicId = thisTopic.Id,
                              ForumId = thisTopic.ForumId,
                              CatId = thisTopic.CatId,
                              UseSignatures = cbxSig.Checked,
                              Message = qrMessage.Text,
                              PosterIp = MemberIP,
                              Status = thisTopic.Status,
                              AuthorId = page.Member.Id,
                              Date = newdate
                          };
        var forum = Forums.GetForum(thisTopic.ForumId);
        if (forum.ModerationLevel == (int) Enumerators.Moderation.AllPosts ||
            forum.ModerationLevel == (int) Enumerators.Moderation.Replies)
        {
            reply.Status = (int) Enumerators.PostStatus.UnModerated;
            thisTopic.UnModeratedReplies += 1;
            Topics.Update(thisTopic);
        }

        int replyid = Replies.AddReply(reply);
        if (Session["LastPostMade"] == null)
        {
            Session.Add("LastPostMade", newdate.ToForumDateStr());
        }
        else
        {
            Session["LastPostMade"] = newdate.ToForumDateStr();
        }
        InvalidateForumCache();
        Page.Response.Redirect(string.Format("/Content/Forums/topic.aspx?whichpage=-1&TOPIC_ID={0}&#{1}", thisTopic.Id, replyid));

    }
    private void InvalidateForumCache()
    {
        object obj = -1;
        Cache["RefreshKey"] = obj;
    }
    #region async file upload

    private void AsyncFileUpload1UploadedFileError(object sender, AsyncFileUploadEventArgs e)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "error", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';});", true);
    }

    private void AsyncFileUpload1UploadedComplete(object sender, AsyncFileUploadEventArgs e)
    {
        ProcessUpload(e);
    }
    private void ProcessUpload(AsyncFileUploadEventArgs e)
    {
        if (e.FileName == null) return;

        ModConfigBase controller = (ModConfigBase)ConfigHelper.ModClass("UploadConfig");
        string[] allowedTypes = controller.ModConfiguration.Settings["AllowedFileTypes"].ToString().Split(',');
        int fileSizeLimit = Convert.ToInt32(controller.ModConfiguration.Settings["FileSizeLimit"].ToString()) * 1024;
        string uploadpath = controller.ModConfiguration.Settings["FileUploadLocation"].ToString();
        string filext = Path.GetExtension(AsyncFileUpload1.PostedFile.FileName).Replace(".", "");
        string contentType = AsyncFileUpload1.PostedFile.ContentType;
        bool allowed = false;

        if (contentType.Contains("image") && !AllowImageUploads)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "prohibited",
                "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Image upload prohibited';});",true);
            AsyncFileUpload1.FailedValidation = true;
            return;
        }
        if (!contentType.Contains("image") && !AllowAttachments)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "prohibited",
                "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File attachments prohibited';});",true);
            AsyncFileUpload1.FailedValidation = true;
            return;            
        }

        foreach (string allowedType in allowedTypes)
        {
            if (filext == allowedType)
            {
                allowed = true;
                break;
            }
        }
        if (!allowed || (int.Parse(e.FileSize) > fileSizeLimit))
        {
            AsyncFileUpload1.FailedValidation = true;
            return;
        }

        var name = Path.GetFileName(e.FileName);
        if (contentType.Contains("image"))
        {
            uploadpath = "/Gallery";
        }
        string savePath =
            Page.MapPath(String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name,
                name.Replace(" ", "+")));
        if (File.Exists(savePath))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "exists",
                "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File already exists';});", true);
            AsyncFileUpload1.FailedValidation = true;
            return;
        }

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size",
            "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " +
            e.FileSize + "';});", true);

        if (!Directory.Exists(Page.MapPath(String.Format("{0}/{1}", uploadpath, HttpContext.Current.User.Identity.Name))))
        {
            Directory.CreateDirectory(
                Page.MapPath(String.Format("{0}/{1}", uploadpath, HttpContext.Current.User.Identity.Name)));
        }

        if (contentType.Contains("image"))
        {
            if (e.FileName != null && AllowImageUploads)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag",
                    "$(function() {top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[img]" +
                    String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) +
                    "[/img]';});", true);
            }
        }
        else
        {
            if (e.FileName != null && AllowAttachments)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag",
                    "$(function() {top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[file=" + e.FileName + "]" +
                    String.Format("{0}/{1}/{2}", uploadpath, HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) +
                    "[/file]';});", true);
            }
        }

        AsyncFileUpload1.SaveAs(savePath);
        if (contentType.Contains("image"))
        {
            string thumbPath =
                Page.MapPath(String.Format("{0}/{1}/thumbnail/{2}", uploadpath, HttpContext.Current.User.Identity.Name,
                    name.Replace(" ", "+")));
            if (!Directory.Exists(Page.MapPath(String.Format("{0}/{1}/thumbnail", uploadpath, HttpContext.Current.User.Identity.Name))))
            {
                Directory.CreateDirectory(Page.MapPath(String.Format("{0}/{1}/thumbnail", uploadpath, HttpContext.Current.User.Identity.Name)));
            }
            GalleryFunctions.CreateThumbnail(savePath, thumbPath, 100);
        }
    }

    #endregion

}
