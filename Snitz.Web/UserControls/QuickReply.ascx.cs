#region copyright
/*'
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
#endregion
using System;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using AjaxControlToolkit;
using SnitzCommon;
using SnitzConfig;
using SnitzData;

public partial class QuickReply : UserControl
{
    private PageBase page;
    public Topic thisTopic { get; set; }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        page = (PageBase)this.Page;
        cbxSig.Checked = (page.Member.UseSig == 1);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = (Roles.IsUserInRole("Member") && thisTopic.Status==Enumerators.PostStatus.Open) || page.IsAdministrator;
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
                    throw new HttpException(403, "Access denied, please try again later");
                    //Response.Redirect("error.aspx?msg=errFloodError");
            }
        }
        else
        {
            Session.Add("LastPostMade", DateTime.UtcNow.ToForumDateStr());
        } 
        
        string MemberIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if ((MemberIP == "") || (MemberIP == "unknown") || (MemberIP == null))
            MemberIP = Request.ServerVariables["REMOTE_ADDR"];
        
        Reply reply = new Reply
                          {
                              TopicId = thisTopic.Id,
                              ForumId = thisTopic.ForumId,
                              CatId = thisTopic.CatId,
                              UseSignatures = cbxSig.Checked,
                              Message = qrMessage.Text,
                              PostersIP = MemberIP,
                              Status = thisTopic.Status,
                              Date = newdate
                          };

        int replyid = Util.AddReply(reply, page.Member);
        if (Session["LastPostMade"] == null)
        {
            Session.Add("LastPostMade", newdate.ToForumDateStr());
        }
        else
        {
            Session["LastPostMade"] = newdate.ToForumDateStr();
        }

        Page.Response.Redirect(string.Format("/Content/Forums/topic.aspx?whichpage=-1&TOPIC_ID={0}&#{1}", thisTopic.Id, replyid));

    }

    #region async file upload

    private void AsyncFileUpload1UploadedFileError(object sender, AsyncFileUploadEventArgs e)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "error", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';", true);
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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'File already exists';", true);
                AsyncFileUpload1.FailedValidation = true;
                return;
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " + AsyncFileUpload1.FileBytes.Length.ToString() + "';", true);
            if (e.FileName != null)
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "tag", "top.$get(\"" + imageTag.ClientID + "\").innerHTML = '[img]" + String.Format("/Gallery/{0}/{1}", HttpContext.Current.User.Identity.Name, name.Replace(" ", "+")) + "[/img]';", true);

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
