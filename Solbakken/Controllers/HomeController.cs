using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CodeFirstMembershipSharp;
using Solbakken.Models;
using Solbakken.Util;

namespace Solbakken.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _db = new DataContext();

        public ActionResult Index(int? albumId)
        {
            var albumer = _db.Albums.ToList();
            ViewBag.Album = albumer;
            var thumbnails = albumer.Select(album => _db.Bilder.FirstOrDefault(x => x.AlbumId == album.Id)).Where(pic => pic != null).ToList();
            ViewBag.Thumbnails = thumbnails;
            return View();
        }

        public ActionResult Album(int albumId)
        {
            var albumer = _db.Albums.ToList();
            ViewBag.Album = albumer;
            var thumbnails = albumer.Select(album => _db.Bilder.FirstOrDefault(x => x.AlbumId == album.Id)).Where(pic => pic != null).ToList();
            ViewBag.Thumbnails = thumbnails;
            var bilder = _db.Bilder.Where(x => x.AlbumId == albumId).ToList();
            return View(bilder);
        }

        public ActionResult GetImage(int id)
        {
            var bilde = _db.Bilder.Find(id);
            return File(bilde.BildeStream, ImageUtil.GetImageContentType(bilde.Format));
        }

        public ActionResult GetThumbnail(int id)
        {
            var thumbnail = _db.Bilder.Find(id);
            return File(thumbnail.Thumbnail, ImageUtil.GetImageContentType(thumbnail.Format));
        }
       
        public ActionResult Test()
        {
            return View();
        }

        public JsonResult UploadTest(string filename)
        {
            var count = Request.InputStream.Length;
            var pic = new byte[count];
            //using(var reader = new StreamReader(Request.InputStream))
            //{
            //    reader.ReadLine();
            //    reader.ReadLine();
            //    reader.ReadLine();
            //    reader.ReadLine();
            //    pic = Encoding.Default.GetBytes(reader.ReadToEnd());
            //}
            Request.InputStream.Read(pic, 0, (int)count);
            using (Stream file = System.IO.File.OpenWrite(@"c:\Users\perkrihe\Desktop\streams\" + filename))
            {
                CopyStream(new MemoryStream(pic), file);
            }
            var startOffset = 0; GetStartOffset(pic);
            var endOffset = 0; GetEndOffset(pic);
            var newPic = new byte[pic.Length];
            Array.Copy(pic, startOffset, newPic, 0, pic.Length);
            
            var im = Image.FromStream(new MemoryStream(newPic));
            var extension = filename.Split('.').Last();
            var imageFormat = ImageUtil.GetImageFormatFromFileExtension(extension);
            var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);
            var thumbnail = ImageUtil.ResizeImage(im, new Size(80, 60), imageFormat);
            
            return Json(string.Format("Added {0}", filename));
        }

        private int GetStartOffset(byte[] pic)
        {
            var start = new byte[2000];
            Array.Copy(pic, start, 2000);
            var startString = Encoding.Default.GetString(start);
            const string endString = "\r\n\r\n";
            var lastIndexOf = startString.LastIndexOf(endString, StringComparison.Ordinal);
            return lastIndexOf + endString.Length;
        }

        private static int GetEndOffset(byte[] newPic)
        {
            return Encoding.Default.GetString(newPic).LastIndexOf("-----------------------------", StringComparison.Ordinal);
        }

        public static void CopyStream(Stream input, ref MemoryStream output)
        {
            var buffer = new byte[3276800];
            int read;
            while ((read = input.Read(buffer, 144, buffer.Length - 144 - 47)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            
        }

        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[3276800];
            int len;
            while ((len = input.Read(buffer, 0, (int)input.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
