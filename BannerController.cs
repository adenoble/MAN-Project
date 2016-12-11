using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using mmsv4.Models;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]
    public class BannerController : Controller
    {
       private mmsv4Entities db = new mmsv4Entities();

        public ActionResult Index()
        {
            var web_banners = db.web_banners.Include(w => w.media_images).OrderByDescending(x => x.datecreated);
            return View(web_banners.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.imageid = new SelectList(db.media_images, "imageid", "title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(web_banners web_banners)
        {
            if (ModelState.IsValid)
            {
                db.web_banners.Add(web_banners);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.imageid = new SelectList(db.media_images, "imageid", "title", web_banners.imageid);
            return View(web_banners);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            web_banners web_banners = db.web_banners.Find(id);
            if (web_banners == null)
            {
                return HttpNotFound();
            }
            ViewBag.imageid = new SelectList(db.media_images, "imageid", "title", web_banners.imageid);
            return View(web_banners);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(web_banners web_banners)
        {
            if (ModelState.IsValid)
            {
                db.Entry(web_banners).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.imageid = new SelectList(db.media_images, "imageid", "title", web_banners.imageid);
            return View(web_banners);
        }

        public ActionResult Delete(int? id)
        {
            web_banners web_banners = db.web_banners.Find(id);
            db.web_banners.Remove(web_banners);
            db.SaveChanges();
            return RedirectToAction("Index");
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
