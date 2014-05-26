/*
####################################################################################################################
##
## SnitzUI.Content.Gallery - Gallery.aspx
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
using System.Collections;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Snitz.BLL;
using SnitzCommon;
using SnitzConfig;

namespace SnitzUI
{
    public partial class Gallery : PageBase
    {
        const string FolderPath = "/gallery/";
        private string _currentGallery = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Config.ShowGallery)
                throw new NotSupportedException("Gallery module is not enabled");

            if (Request.Params["u"] != null)
            {
                if (IsAuthenticated)
                {
                    _currentGallery = Member.Username;
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