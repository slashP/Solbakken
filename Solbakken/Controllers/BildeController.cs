using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CodeFirstMembershipSharp;
using Solbakken.Models;
using Solbakken.Util;

namespace Solbakken.Controllers
{
    [Authorize]
    public class BildeController : Controller
    {
        private readonly DataContext _db = new DataContext();
        private readonly List<string> _incompatibleBrowsers = new List<string>{"Opera", "IE"};
        public ActionResult LastOpp(string warning, string success)
        {
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Navn");
            ViewBag.Warning = warning;
            ViewBag.Success = success;
            return _incompatibleBrowsers.Contains(Request.Browser.Browser) ? View("LastOppEnkel") : View();
        }

        [HttpPost]
        public ActionResult LastOpp(int? albumId, string beskrivelse)
        {
            if(albumId == null)
            {
                return RedirectToAction("LastOpp", new {warning = "Du må velge et album"});
            }
            var image = Request.Files.Count > 0 ? Request.Files[0] : null;
            var imagesUploadedCounter = 0;
            if (image != null)
            {
                if(ImageUtil.AllowedImageTypes.Contains(image.ContentType))
                {
                    var im = Image.FromStream(image.InputStream);
                    var imageFormat = ImageUtil.GetImageFormat(image.ContentType);
                    var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);
                    var thumbnail = ImageUtil.ResizeImage(im, new Size(100, 75), imageFormat);
                    var album = _db.Albums.Find(albumId) ?? _db.Albums.FirstOrDefault();
                    var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                    var bilde = new Bilde
                    {
                        AlbumId = album.Id,
                        Beskrivelse = beskrivelse,
                        Format = imageFormat.ToString(),
                        BildeStream = ReadFully(newImage),
                        Filnavn = image.FileName,
                        LastetOppAvId = user.UserId,
                        Navn = image.FileName,
                        Thumbnail = ReadFully(thumbnail)
                    };
                    imagesUploadedCounter++;
                    _db.Bilder.Add(bilde);
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("LastOpp", new {success = string.Format("{0} bilde{1} lastet opp.", imagesUploadedCounter, imagesUploadedCounter != 1 ? "r" : string.Empty)});
        }

        public JsonResult UploadMultiple(string filename, int albumId, string beskrivelse)
        {
            var count = Request.InputStream.Length;
            var pic = new byte[count];
            Request.InputStream.Read(pic, 0, (int)count);
            try
            {
                var im = Image.FromStream(new MemoryStream(pic));
                var extension = filename.Split('.').Last();
                var imageFormat = ImageUtil.GetImageFormatFromFileExtension(extension);
                var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);
                var thumbnail = ImageUtil.ResizeImage(im, new Size(80, 60), imageFormat);
                var album = _db.Albums.Find(albumId) ?? _db.Albums.FirstOrDefault();
                var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                var bilde = new Bilde
                {
                    AlbumId = album.Id,
                    Beskrivelse = beskrivelse,
                    Format = imageFormat.ToString(),
                    BildeStream = ReadFully(newImage),
                    Filnavn = filename,
                    LastetOppAvId = user.UserId,
                    Navn = filename,
                    Thumbnail = ReadFully(thumbnail)
                };
                _db.Bilder.Add(bilde);
                _db.SaveChanges();
            }
            catch (Exception)
            {
                throw new FormatException("Wrong format on picture");
            }
            return Json(string.Format("La til {0}", filename));
        }

        public Guid GetUserId()
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
            var userid = user != null ? user.UserId : Guid.NewGuid();
            return userid;
        }

        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public ActionResult Slett()
        {
            var userId = GetUserId();
            var bilder = _db.Bilder.Where(x => x.LastetOppAvId == userId).ToList();
            return View(bilder);
        }

        public ActionResult SlettBekreftet(int id)
        {
            var bilde = _db.Bilder.Find(id);
            var userId = GetUserId();
            if (bilde.LastetOppAvId == userId)
            {
                _db.Bilder.Remove(bilde);
                _db.SaveChanges();
            }
            return RedirectToAction("Slett");
        }
    }
}
