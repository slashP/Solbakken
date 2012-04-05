using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeFirstMembershipSharp;
using Solbakken.Util;

namespace Solbakken.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _db = new DataContext();

        public ActionResult Index(int? albumId)
        {
            ViewBag.Album = _db.Albums.ToList();
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

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
