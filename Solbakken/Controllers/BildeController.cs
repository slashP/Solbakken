using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeFirstMembershipSharp;
using Ionic.Zip;
using Solbakken.Models;
using Solbakken.Util;

namespace Solbakken.Controllers
{
    [Authorize]
    public class BildeController : Controller
    {
        private readonly DataContext _db = new DataContext();

        public ActionResult LastOpp()
        {
            ViewBag.AlbumId = new SelectList(_db.Albums, "Id", "Navn");
            return View();
        }

        [HttpPost]
        public ActionResult LastOpp(HttpPostedFileBase fileBase, int? albumId, string beskrivelse)
        {
            if(albumId == null)
            {
                ModelState.AddModelError("AlbumId", "Du må velge et album");
                return RedirectToAction("LastOpp");
            }
            var image = Request.Files.Count > 0 ? Request.Files[0] : null;
            if (image != null)
            {
                if(image.ContentType == "application/zip")
                {
                    using (var zip1 = ZipFile.Read(image.InputStream))
                    {
                        // here, we extract every entry, but we could extract conditionally
                        // based on entry name, size, date, checkbox status, etc.  
                        foreach (var e in zip1)
                        {
                            using(var stream = new MemoryStream())
                            {
                                e.Extract(stream);
                                var extension = e.FileName.Split('.').Last();
                                var im = Image.FromStream(stream);
                                var imageFormat = ImageUtil.GetImageFormatFromFileExtension(extension);
                                var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);
                                var album = _db.Albums.Find(albumId) ?? _db.Albums.FirstOrDefault();
                                var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                                var bilde = new Bilde
                                {
                                    AlbumId = album.Id,
                                    Beskrivelse = beskrivelse,
                                    Format = imageFormat.ToString(),
                                    BildeStream = ReadFully(newImage),
                                    Filnavn = e.FileName,
                                    LastetOppAvId = user.UserId,
                                    Navn = e.FileName
                                };
                                _db.Bilder.Add(bilde);
                                _db.SaveChanges();
                            }
                        }
                    }
                }
                else if(ImageUtil.AllowedImageTypes.Contains(image.ContentType))
                {
                    var im = Image.FromStream(image.InputStream);
                    var imageFormat = ImageUtil.GetImageFormat(image.ContentType);
                    var newImage = ImageUtil.ResizeImage(im, new Size(640, 480), imageFormat);
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
                        Navn = image.FileName
                    };
                    _db.Bilder.Add(bilde);
                    _db.SaveChanges();
                }
                
            }
            return RedirectToAction("LastOpp");
        }

        public Guid GetUserId()
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
            var userid = user != null ? user.UserId : Guid.NewGuid();
            return userid;
        }

        public ActionResult Slett()
        {
            var userId = GetUserId();
            var bilder = _db.Bilder.Where(x => x.LastetOppAvId == userId).ToList();
            return View(bilder);
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
