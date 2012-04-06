using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
            if(albumId == null)
            {
                return View(_db.Bilder.Take(10).ToList());
            }
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
    }
}
