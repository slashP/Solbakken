using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
        private readonly List<string> _incompatibleBrowsers = new List<string>{"Opera", "IE", "Safari"};
        public ActionResult LastOpp(string warning, string success)
        {
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Navn");
            ViewBag.Warning = warning;
            ViewBag.Success = success;
            return _incompatibleBrowsers.Contains(Request.Browser.Browser) ? View("LastOppEnkel") : View("Upload");
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
                    var newImage = im.Resize(640, 480);
                    var thumbnail = im.Resize(100, 75);
                    var album = _db.Albums.Find(albumId) ?? _db.Albums.FirstOrDefault();
                    var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                    var bilde = new Bilde
                    {
                        AlbumId = album.Id,
                        Beskrivelse = beskrivelse,
                        Format = imageFormat.ToString(),
                        BildeStream = ReadFully(newImage.ToStream(imageFormat)),
                        Filnavn = image.FileName,
                        LastetOppAvId = user.UserId,
                        Navn = image.FileName,
                        Thumbnail = ReadFully(thumbnail.ToStream(imageFormat))
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
                var newImage = im.Resize(640, 480);
                var thumbnail = im.Resize(80, 60);
                var album = _db.Albums.Find(albumId) ?? _db.Albums.FirstOrDefault();
                var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                var bilde = new Bilde
                {
                    AlbumId = album.Id,
                    Beskrivelse = beskrivelse,
                    Format = imageFormat.ToString(),
                    BildeStream = ReadFully(newImage.ToStream(imageFormat)),
                    Filnavn = filename,
                    LastetOppAvId = user.UserId,
                    Navn = filename,
                    Thumbnail = ReadFully(thumbnail.ToStream(imageFormat))
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

        public JsonResult UploadJqueryMultiple(int? albumId)
        {
            if(albumId == null)
            {
                return null;
            }
            if(Request.Files.Count == 0)
            {
                var imageJsons = new List<JsonUploadResponse>();
                return Json(imageJsons, JsonRequestBehavior.AllowGet);
            }
            var count = Request.Files[0].InputStream.Length;
            var pic = new byte[count];
            var bilde = new Bilde();
            Request.InputStream.Read(pic, 0, (int)count);
            try
            {
                var file = Request.Files[0];
                if (file != null)
                {
                    var im = Image.FromStream(file.InputStream);
                    var extension = file.FileName.Split('.').Last();
                    var imageFormat = ImageUtil.GetImageFormatFromFileExtension(extension);
                    var newImage = im.Resize(800, 600);
                    var thumbnail = im.Resize(80, 60);
                    var album = _db.Albums.Find(14) ?? _db.Albums.FirstOrDefault();
                    var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                    bilde = new Bilde
                                {
                                    AlbumId = albumId,
                                    Beskrivelse = "",
                                    Format = imageFormat.ToString(),
                                    BildeStream = ReadFully(newImage.ToStream(imageFormat)),
                                    Filnavn = file.FileName,
                                    LastetOppAvId = user.UserId,
                                    Navn = file.FileName,
                                    Thumbnail = ReadFully(thumbnail.ToStream(imageFormat))
                                };
                    _db.Bilder.Add(bilde);
                    _db.SaveChanges();
                }
                
            }
            catch (Exception)
            {
                throw new FormatException("Wrong format on picture");
            }
            return
                Json(new[]
                         {
                             new JsonUploadResponse
                                 {
                                     delete_type = "DELETE",
                                     delete_url = "/Bilde/SlettBekreftet/" + bilde.Id,
                                     name = bilde.Navn,
                                     size = bilde.BildeStream.Count(),
                                     thumbnail_url = "/Home/GetThumbnail/" + bilde.Id,
                                     url = "/Home/GetImage/" + bilde.Id
                                 }
                         });
        }

        public ActionResult Upload(string warning, string success)
        {
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Navn");
            ViewBag.Warning = warning;
            ViewBag.Success = success;
            return View();
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

        public JsonResult SlettJson()
        {
            var skip = GetReferredSkip();
            var userId = GetUserId();
            var bilder = _db.Bilder.Where(x => x.LastetOppAvId == userId).OrderBy(x => x.Id).Skip(skip).Take(50).ToList();
            var jsonBilder = bilder.Select(bilde => new JsonUploadResponse
                                                        {
                                                            delete_type = "DELETE",
                                                            delete_url = "/Bilde/SlettBekreftet/" + bilde.Id,
                                                            name = bilde.Navn,
                                                            size = bilde.BildeStream.Count(),
                                                            thumbnail_url = "/Home/GetThumbnail/" + bilde.Id,
                                                            url = "/Home/GetImage/" + bilde.Id
                                                        });
            return Json(jsonBilder, JsonRequestBehavior.AllowGet);
        }

        private int GetReferredSkip()
        {
            var skip = 0;
            if (Request.UrlReferrer != null)
            {
                int.TryParse(Request.UrlReferrer.Segments.Last(), out skip);
            }
            return Math.Max(skip, 0);
        }

        private int GetCurrentSkip()
        {
            var skip = 0;
            if(Request.Url != null)
            {
                int.TryParse(Request.Url.Segments.Last(), out skip);
            }
            return Math.Max(skip, 0);
        }

        public ActionResult Slett()
        {
            ViewBag.Skip = GetCurrentSkip();
            var userId = GetUserId();
            ViewBag.Count = _db.Bilder.Count(x => x.LastetOppAvId == userId);
            return View();
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
