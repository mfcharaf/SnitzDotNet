using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SnitzUI.UserControls.Popups
{
    public class GalleryLink
    {
        public string ImagePath { get; set; }
        public string ThumbPath { get; set; }
    }
    public partial class BrowseImage : System.Web.UI.UserControl
    {
          
        protected void Page_Load(object sender, EventArgs e)
        {
            const string folderPath = "/gallery/";
            BindImages(folderPath);
        }

        private void BindImages(string folderPath)
        {
            List<GalleryLink> imageList = new List<GalleryLink>();
            //get all the files from flder
            folderPath += HttpContext.Current.User.Identity.Name + "/";
            if (Directory.Exists(Server.MapPath("~" + folderPath)))
            {
                string thumbPath = folderPath + "thumbnail/";
                string[] strImgs = Directory.GetFiles(Server.MapPath("~" + folderPath));
                //Loop through all the file and get only file name and concat it with folder name
                for (int count = 0; count < strImgs.Length; count++)
                {
                    string filePath = strImgs[count];
                    string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                    string newFilePath = folderPath + filePath;
                    imageList.Add(new GalleryLink() {ImagePath = folderPath + fileName, ThumbPath = thumbPath + fileName});
                }
            }
            // Bind an image file name to repeater.
            rptImage.DataSource = imageList;
            rptImage.DataBind();
        }

    }
}