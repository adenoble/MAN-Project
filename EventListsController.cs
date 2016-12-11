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
    public class EventListsController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        public ViewResult Index()
        {
            var eventlist = db.EventLists.OrderByDescending(x => x.datecreated).ToList();
            ViewBag.EventCount = eventlist.Sum(x => x.attendees);
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.EventLists.OrderByDescending(x => x.datecreated).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
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
