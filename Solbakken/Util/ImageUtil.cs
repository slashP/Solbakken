﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Solbakken.Util
{
    public static class ImageUtil
    {
        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }

        public static Stream ResizeImage(Image imgToResize, Size size, ImageFormat imageFormat)
        {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            float nPercent;
            var nPercentW = (size.Width / (float)sourceWidth);
            var nPercentH = (size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return b.ToStream(imageFormat);
        }

        public static Image Resize(this Image image, int width, int height)
        {
            var sourceWidth = image.Width;
            var sourceHeight = image.Height;
            var widthRatio = (width / (float)sourceWidth);
            var heightRatio = (height / (float)sourceHeight);
            var resizeRatio = heightRatio < widthRatio ? heightRatio : widthRatio;
            var destWidth = (int)(sourceWidth * resizeRatio);
            var destHeight = (int)(sourceHeight * resizeRatio);
            var resizedImage = new Bitmap(destWidth, destHeight);
            using (var g = Graphics.FromImage(resizedImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;
                g.DrawImage(image, new Rectangle(0, 0, resizedImage.Width, resizedImage.Height));
                return resizedImage;
            }
        }

        public static string[] AllowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/pjpeg" };

        public static ImageFormat GetImageFormatFromFileExtension(string fileExtension)
        {
            switch (fileExtension.ToUpperInvariant())
            {
                case "JPG":
                    return ImageFormat.Jpeg;
                case "PNG":
                    return ImageFormat.Png;
                case "GIF":
                    return ImageFormat.Gif;
            }
            return ImageFormat.Jpeg;
        }

        public static ImageFormat GetImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/jpeg":
                case "image/pjpeg":
                    return ImageFormat.Jpeg;
                case "image/png":
                    return ImageFormat.Png;
                case "image/gif":
                    return ImageFormat.Gif;
            }
            return ImageFormat.Png;
        }

        public static string GetImageContentType(string contentType)
        {
            switch (contentType.ToUpperInvariant())
            {
                case "JPEG":
                case "JPG":
                    return "image/jpeg";
                case "PNG":
                    return "image/png";
                case "GIF":
                    return "image/gif";
            }
            return "image/jpeg";
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            var ms = new MemoryStream(byteArrayIn);
            var returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}