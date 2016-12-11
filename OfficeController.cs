
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
    public class OfficeController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.user_office.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.user_office.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(user_office user_office)
        {
            try
            {
                user_office.datecreated = DateTime.Now;
                db.user_office.Add(user_office);
                db.SaveChanges();
                return Json(user_office, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(user_office user_office)
        {
            try
            {
                var of = db.user_office.Find(user_office.officeid);
                of.office = user_office.office;
                of.enable = user_office.enable;
                of.datemodified = DateTime.Now;
                db.Entry(of).State = EntityState.Modified;
                db.SaveChanges();
                return Json(user_office, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(user_office user_office)
        {
            try
            {
                db.user_office.Remove(user_office);
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