
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
    public class RoleController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.webpages_Roles.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.webpages_Roles.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(webpages_Roles webpages_Roles)
        {
            try
            {
                db.webpages_Roles.Add(webpages_Roles);
                db.SaveChanges();
                return Json(webpages_Roles, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(webpages_Roles webpages_Roles)
        {
            try
            {
                db.Entry(webpages_Roles).State = EntityState.Modified;
                db.SaveChanges();
                return Json(webpages_Roles, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(webpages_Roles webpages_Roles)
        {
            try
            {
                db.webpages_Roles.Remove(webpages_Roles);
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