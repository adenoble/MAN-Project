
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
    public class SolatController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var rt = db.content_solat.ToList();
            List<content_solatMetaData> result = new List<content_solatMetaData>();
            foreach(var st in rt)
            {
                result.Add(new content_solatMetaData
                {
                    solatid = st.solatid,
                    solatDay = st.solatDay,
                    solatMnt = st.solatMnt,
                    fajr = st.fajr.ToString(),
                    sunrise = st.sunrise.ToString(),
                    zuhr = st.zuhr.ToString(),
                    asr = st.asr.ToString(),
                    maghrib = st.maghrib.ToString(),
                    isha = st.isha.ToString()
                });
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.content_solat.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(content_solat content_solat)
        {
            try
            {
                db.content_solat.Add(content_solat);
                db.SaveChanges();
                return Json(content_solat, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(content_solat content_solat)
        {
            try
            {
                db.Entry(content_solat).State = EntityState.Modified;
                db.SaveChanges();
                var contentsolat = new content_solatMetaData
                {
                    solatid = content_solat.solatid,
                    solatDay = content_solat.solatDay,
                    solatMnt = content_solat.solatMnt,
                    fajr = content_solat.fajr.ToString(),
                    sunrise = content_solat.sunrise.ToString(),
                    zuhr = content_solat.zuhr.ToString(),
                    asr = content_solat.asr.ToString(),
                    maghrib = content_solat.maghrib.ToString(),
                    isha = content_solat.isha.ToString()
                };
                return Json(contentsolat, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(content_solat content_solat)
        {
            try
            {
                db.content_solat.Remove(content_solat);
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