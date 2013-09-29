/*
####################################################################################################################
##
## Snitz.BLL - GalleryFunctions
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		01/08/2013
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;


namespace Snitz.BLL
{
    /// <summary>
    /// Properties for Gallery Links
    /// </summary>
    public class GalleryLink
    {
        public string Name { get; set;}
        public string ImagePath { get; set; }
        public string ThumbPath { get; set; }
    }

    /// <summary>
    /// Image Gallery methods
    /// </summary>
    public static class GalleryFunctions
    {
        /// <summary>
        /// Creates a thumbnail
        /// </summary>
        /// <param name="ImageFrom">Path of source image</param>
        /// <param name="ImageTo">Path of thumbnail to save</param>
        /// <param name="ImageHeight">Height of thumbnail</param>
        public static void CreateThumbnail(string ImageFrom, string ImageTo, int ImageHeight)
        {
            Image th;
            Image i = Image.FromFile(ImageFrom);

            //i.RotateFlip(RotateFlipType.Rotate180FlipNone);
            //i.RotateFlip(RotateFlipType.Rotate180FlipNone);
            if (i.Height > ImageHeight)
            {
                th = i.GetThumbnailImage
                    (
                        ImageHeight * i.Width / i.Height,
                        ImageHeight,
                        ThumbnailCallback,
                        IntPtr.Zero
                    );
                i.Dispose();
            }
            else
            {
                th = (Image)i.Clone();
            }

            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)80);
            ImageCodecInfo ici = GetEncoderInfo("image/jpeg");

            th.Save(ImageTo, ici, ep);
            CropImageFile(th, ImageTo, 80, 100);
            th.Dispose();
            return;
        }
        
        /// <summary>
        /// Abort callback delegate for thumbnail creation
        /// </summary>
        /// <returns></returns>
        public static bool ThumbnailCallback()
        {
            return true;
        }

        /// <summary>
        /// Crops an image
        /// </summary>
        /// <param name="imgPhoto"></param>
        /// <param name="ImageTo"></param>
        /// <param name="targetW"></param>
        /// <param name="targetH"></param>
        public static void CropImageFile(Image imgPhoto, string ImageTo, int targetW, int targetH)
        {

            //Image imgPhoto = Image.FromFile(ImageFrom);
            int targetX = (imgPhoto.Width - targetW) / 2;
            int targetY = (imgPhoto.Height - targetH) / 2;

            Bitmap bmPhoto = new Bitmap(targetW, targetH, PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(72, 72);
            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.FillRectangle(Brushes.White, 0, 0, targetW, targetH);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage
                    (
                    imgPhoto,
                    new Rectangle(0, 0, targetW, targetH),
                    targetX,
                    targetY,
                    targetW,
                    targetH,
                    GraphicsUnit.Pixel
                    );
            // Save out to memory and then to a file.  We dispose of all objects to make sure the files don't stay locked.
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(Encoder.Quality, (long)100);

            ImageCodecInfo ici = GetEncoderInfo("image/png");

            imgPhoto.Dispose();
            grPhoto.Dispose();

            bmPhoto.Save(ImageTo, ici, ep);
            bmPhoto.Dispose();
        }

        /// <summary>
        /// Gets Image codec info based on Mime Type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// Returns a list of Links for a Members Gallery images
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static List<GalleryLink> GetImages(string username)
        {
            string folderPath = "/gallery/";
            List<GalleryLink> imageList = new List<GalleryLink>();
            //get all the files from flder
            folderPath += username + "/";
            if (Directory.Exists(HttpContext.Current.Server.MapPath("~" + folderPath)))
            {
                string thumbPath = folderPath + "thumbnail/";
                string[] strImgs = Directory.GetFiles(HttpContext.Current.Server.MapPath("~" + folderPath)).Where(s => s.EndsWith(".gif") || s.EndsWith(".jpg") || s.EndsWith(".png")).ToArray(); 
                //Loop through all the file and get only file name and concat it with folder name
                for (int count = 0; count < strImgs.Length; count++)
                {
                    string filePath = strImgs[count];
                    string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                    string newFilePath = folderPath + filePath;
                    imageList.Add(new GalleryLink() { ImagePath = folderPath + fileName, ThumbPath = thumbPath + fileName, Name = fileName});
                }
            }

            return imageList;
        }
    }
}
