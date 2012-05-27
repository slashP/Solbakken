using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CodeFirstMembershipSharp;
using Solbakken.Util;

namespace Solbakken.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DataContext _db = new DataContext();

        [AllowAnonymous]
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
            return Json(string.Format("Added {0}", filename));
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
