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
using Snitz.BLL;
using Snitz.Entities;
using SnitzCommon;
using SnitzConfig;


public partial class QuickReply : UserControl
{
    private PageBase page;
    public TopicInfo thisTopic { get; set; }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        page = (PageBase)this.Page;
        cbxSig.Checked = page.Member.UseSignature;
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
        string filename = e.FileName;
        if (filename == null)
            return;
        string contentType = AsyncFileUpload1.PostedFile.ContentType;
        if (!contentType.Contains("image"))
            return;
        var fileName = Path.GetFileName(filename);
        if (fileName != null && fileName.Contains(".pdf"))
        {
            AsyncFileUpload1.FailedValidation = true;
            return;
        }
        if (int.Parse(e.FileSize) > 2000000)
        {
            AsyncFileUpload1.FailedValidation = true;
            return;
        }
        var name = Path.GetFileName(filename);
        if (name != null)
        {
            string savePath = Page.MapPath(String.Format("~/Gallery/{0}/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")));
            string thumbPath = Page.MapPath(String.Format("~/Gallery/{0}/thumbnail/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")));
            if (File.Exists(savePath))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File already exists';});", true);
                AsyncFileUpload1.FailedValidation = true;
                return;
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "$(function() {top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " + AsyncFileUpload1.FileBytes.Length.ToString() + "';});", true);
            if (e.FileName != null)
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag", "$(function() {top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[img]" + String.Format("/Gallery/{0}/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) + "[/img]';});", true);

            if (!Directory.Exists(Page.MapPath(String.Format("~/Gallery/{0}", HttpContext.Current.User.Identity.Name))))
            {
                Directory.CreateDirectory(Page.MapPath(String.Format("~/Gallery/{0}", HttpContext.Current.User.Identity.Name)));
                Directory.CreateDirectory(Page.MapPath(String.Format("~/Gallery/{0}/thumbnail", HttpContext.Current.User.Identity.Name)));
            }

            AsyncFileUpload1.SaveAs(savePath);
            GalleryFunctions.CreateThumbnail(savePath, thumbPath, 100);
        }
    }

    #endregion

}
