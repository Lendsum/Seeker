//using System;
//using System.Drawing;
//using System.IO;

//namespace Lendsum.Crosscutting.Common
//{
//    /// <summary>
//    /// Class to do operation with images:
//    ///     • Resize
//    /// </summary>
//    public static class ImageTool
//    {
//        /// <summary>
//        /// Resizes the image if necessary.
//        /// </summary>
//        /// <param name="imageToResize">My bytes.</param>
//        /// <param name="defaultValue">The default value.</param>
//        /// <returns></returns>
//        public static byte[] ResizeImageIfNecessary(byte[] imageToResize, int defaultValue = 700)
//        {
//            if (defaultValue == 0)
//                return imageToResize;

//            Image fullsizeImage;
//            using (var myMemStream = new MemoryStream(imageToResize))
//            {
//                fullsizeImage = Image.FromStream(myMemStream);
//            }

//            int sourceWidth = fullsizeImage.Width;
//            int sourceHeight = fullsizeImage.Height;
//            int newWidth = defaultValue;
//            int newHeight = defaultValue;

//            if (sourceWidth < newWidth && sourceHeight < newHeight)
//                return imageToResize;

//            decimal proportion;

//            //Vertical picture
//            if (sourceWidth < sourceHeight)
//            {
//                proportion = (decimal)sourceWidth / sourceHeight;
//                newWidth = Convert.ToInt32(newHeight * proportion);
//            }

//            //Horizontal picture
//            if (sourceWidth > sourceHeight)
//            {
//                proportion = (decimal)sourceHeight / sourceWidth;
//                newHeight = Convert.ToInt32(newWidth * proportion);
//            }

//            var newImage = fullsizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

//            MemoryStream myResult;
//            using (myResult = new MemoryStream())
//            {
//                newImage.Save(myResult, System.Drawing.Imaging.ImageFormat.Png);
//            }

//            return myResult.ToArray();
//        }
//    }
//}