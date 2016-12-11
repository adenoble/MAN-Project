
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
    public class ArticleController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            var result = db.content_article
           .OrderByDescending(x => x.datecreated)
           .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = db.content_article.Find(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult create(content_article content_article)
        {
            try
            {
                db.content_article.Add(content_article);
                db.SaveChanges();
                return Json(content_article, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult update(content_article content_article)
        {
            try
            {
                db.Entry(content_article).State = EntityState.Modified;
                db.SaveChanges();
                return Json(content_article, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult destroy(content_article content_article)
        {
            try
            {
                var art = db.content_article.Find(content_article.articleid);
                db.content_article.Remove(art);
                db.SaveChanges();
                return Json(content_article, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("fail", JsonRequestBehavior.AllowGet);
            }
        }
    }
}