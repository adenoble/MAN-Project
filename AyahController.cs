
using System;
using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;
using mmsv4.Models;

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]
    public class AyahController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.content_monthayah
                .OrderByDescending(x => x.datecreated)
                .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.content_monthayah.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(content_monthayah content_monthayah)
        {
            try
            {
                db.content_monthayah.Add(content_monthayah);
                db.SaveChanges();
                return Json(content_monthayah, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(content_monthayah content_monthayah)
        {
            try
            {
                db.Entry(content_monthayah).State = EntityState.Modified;
                db.SaveChanges();
                return Json(content_monthayah, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(content_monthayah content_monthayah)
        {
            try
            {
                var aya = db.content_monthayah.Find(content_monthayah.ayahid);
                db.content_monthayah.Remove(aya);
                db.SaveChanges();
                return Json(content_monthayah, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }
    }
}