
using System;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using mmsv4.Models;
using System.Collections.Generic;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]
    public class AdminController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.content_pages
               .Select(x =>
                       new content_pages_Trim
                       {
                           pageid = x.pageid,
                           title = x.title,
                           content = x.content,
                           partialid = x.partialid,
                           enable = x.enable
                       });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllPartial()
        {
            var result = db.web_partial
                .Select(x => new valueText {id = x.partialid, text = x.description });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.content_pages.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(content_pages_Trim content_pages)
        {
            try
            {
                var pg = new content_pages();
                pg.title = content_pages.title;
                pg.content = content_pages.content;
                pg.partialid = content_pages.partialid;
                pg.enable = content_pages.enable;
                pg.datecreated = DateTime.Now;
                db.content_pages.Add(pg);
                db.SaveChanges();
                content_pages.pageid = pg.pageid;
                return Json(content_pages, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(content_pages_Trim content_pages)
        {
            try
            {
                var pg = db.content_pages.Find(content_pages.pageid);
                pg.title = content_pages.title;
                pg.content = content_pages.content;
                pg.partialid = content_pages.partialid;
                pg.enable = content_pages.enable;
                pg.datemodified = DateTime.Now;
                db.Entry(pg).State = EntityState.Modified;
                db.SaveChanges();
                return Json(content_pages, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(content_pages_Trim content_pages)
        {
            try
            {
                var pg = db.content_pages.Find(content_pages.pageid);
                db.content_pages.Remove(pg);
                db.SaveChanges();
                return Json("Successful", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }
    }
}