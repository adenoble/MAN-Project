
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
        public class MailingController : Controller
         {
             private mmsv4Entities db = new mmsv4Entities(); 
             public ViewResult Index()
             {
                 return View();
             }
 
             [HttpGet]
             public JsonResult GetAll()
             {
                 var result = db.UpdateLists.OrderByDescending(x => x.datecreated).ToList();
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
 
             [HttpGet]
             public JsonResult Get(int id)
             {
                var result = db.UpdateLists.Find(id);
                 return Json(result, JsonRequestBehavior.AllowGet);
             }
 
             [HttpPost]
            public JsonResult create(UpdateList UpdateList)
             {
                 try
                 {
                   db.UpdateLists.Add(UpdateList);
                    db.SaveChanges();
                   return Json(UpdateList, JsonRequestBehavior.AllowGet);
                 }
                 catch
                {
                     return Json("fail", JsonRequestBehavior.AllowGet);
                 }
             }
 
 
            [HttpPost]
            public JsonResult update(UpdateList UpdateList)
             {
                 try
                 {
                   db.Entry(UpdateList).State = EntityState.Modified;
                    db.SaveChanges();
                   return Json(UpdateList, JsonRequestBehavior.AllowGet);
                 }
                 catch
                 {
                     return Json("fail", JsonRequestBehavior.AllowGet);
                 }
             }
 
             [HttpPost]
            public JsonResult destroy(UpdateList UpdateList)
             {
                 try
                 {
                    db.UpdateLists.Remove(UpdateList);
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