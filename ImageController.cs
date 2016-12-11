using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mmsv4.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ImageResizer.Resizing;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]

    public class ImageController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        public ActionResult Index()
        {
            var media_images = db.media_images.OrderByDescending(x => x.datecreated);
            return View(media_images.ToList());
        }


        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(media_images.categories(), "id", "text");
            return View();
        }
		
		[HttpGet]
        public JsonResult GetAlldll()
        {
            var result = db.media_images.Select(x => new { x.imageid, x.title }).ToArray();
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(media_images media_images)
        {
            ViewBag.Category = new SelectList(media_images.categories(), "id", "text", media_images.categoryid);

            string[] validImageTypes = { "image/gif", "image/jpeg", "image/png" };

            if (media_images.FileUrl == null && media_images.FileUrl.ContentLength < 0)
            {
                ModelState.AddModelError("Error", "Please upload image");
            }

            if (!validImageTypes.Contains(media_images.FileUrl.ContentType))
            {
                ModelState.AddModelError("Error", "Image must be jpeg, gif, or png");
            }

            if (ModelState.IsValid)
            {
                //var filename = Path.GetFileName(media_images.FileUrl.FileName);
                //var path = Path.Combine(Server.MapPath("~/files/images"), filename);
                //media_images.FileUrl.SaveAs(path);

                byte[] imageByte = null;
                BinaryReader rdr = new BinaryReader(media_images.FileUrl.InputStream);
                imageByte = rdr.ReadBytes((int)media_images.FileUrl.ContentLength);

                var m_i = new media_images
                {
                    title = media_images.title,
                    filename = Path.GetFileName(media_images.FileUrl.FileName),
                    alttext = media_images.alttext,
                    categoryid = media_images.categoryid,
                    enable = media_images.enable,
                    //filedata = imageByte,
                    datecreated = DateTime.Now
                };

                db.media_images.Add(m_i);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(media_images);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            media_images media_images = (media_images)db.media_images.Find(id);
            if (media_images == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category = new SelectList(media_images.categories(), "id", "text", media_images.categoryid);
            return View(media_images);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(media_images media_images)
        {
            ViewBag.Category = new SelectList(media_images.categories(), "id", "text", media_images.categoryid);

            string[] validImageTypes = { "image/gif", "image/jpeg", "image/png" };

            //if (media_images.FileUrl != null && media_images.FileUrl.ContentLength > 0)
            //{
            //    //ModelState.AddModelError("Error", "Please upload image");
            //}

            //if (!validImageTypes.Contains(media_images.FileUrl.ContentType))
            //{
            //    ModelState.AddModelError("Error", "Image must be jpeg, gif, or png");
            //}

            if (ModelState.IsValid)
            {
                media_images m_i = db.media_images.Find(media_images.imageid);
                m_i.title = media_images.title;
                m_i.alttext = media_images.alttext;
                m_i.categoryid = media_images.categoryid;
                m_i.enable = media_images.enable;
                if (media_images.FileUrl != null && media_images.FileUrl.ContentLength > 0)
                {
                    byte[] imageByte = null;
                    BinaryReader rdr = new BinaryReader(media_images.FileUrl.InputStream);
                    imageByte = rdr.ReadBytes((int)media_images.FileUrl.ContentLength);
                    m_i.filename = Path.GetFileName(media_images.FileUrl.FileName);
                    //m_i.filedata = imageByte;
                }
                m_i.datecreated = media_images.datecreated;
                m_i.datemodified = media_images.datemodified;
                db.Entry(m_i).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(media_images);
        }

        public ActionResult Delete(int id)
        {
            media_images media_images = db.media_images.Find(id);
            db.media_images.Remove(media_images);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public FileResult byteArrayToImage(int imgId)
        {
            media_images_data media_images = db.media_images_data.Find(imgId);
            return new FileStreamResult(new MemoryStream(media_images.filedata), "image/png");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
