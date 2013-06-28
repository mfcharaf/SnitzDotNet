#region Copyright Notice
/*
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2012 Huw Reddick
## All rights reserved.
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
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
using System.Collections;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using SnitzCommon;

namespace SnitzUI
{
    public partial class Gallery : PageBase
    {
        const string FolderPath = "/gallery/";
        private string _currentGallery = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.Params["u"] != null)
            {
                if (IsAuthenticated)
                {
                    _currentGallery = Member.Name;
                    BindImages();
                }

            }
            else
            {
                BindGalleries();
            }
            
        }

        private void BindGalleries()
        {
            var galleries = new ArrayList();
            var dInfo = new DirectoryInfo(Server.MapPath("~" + FolderPath));

            foreach (DirectoryInfo directory in dInfo.GetDirectories())
            {
                if(directory.GetFiles("public.txt").Any())
                galleries.Add(directory.Name);
            }
            ListView1.DataSource = galleries;
            ListView1.DataBind();
        }

        private void BindImages()
        {
            rptImage.DataSource = GalleryFunctions.GetImages(_currentGallery);
            rptImage.DataBind();
        }

        protected void ShowGallery(object sender, ListViewCommandEventArgs  e)
        {
            _currentGallery = e.CommandArgument.ToString();
            BindImages();
        }
    }
}