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

namespace mmsv4.Controllers
{
    [Authorize(Roles = "Administrator, Super Administrator")]

    public class DocumentController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        public ActionResult Index()
        {
            var media_documents = db.media_documents.OrderByDescending(x => x.datecreated);
            return View(media_documents.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(media_documents media_documents)
        {
            //ViewBag.Category = new SelectList(media_images.categories(), "id", "text", media_images.categoryid);

            string[] validImageTypes = { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

            if (media_documents.FileUrl == null && media_documents.FileUrl.ContentLength < 0)
            {
                ModelState.AddModelError("Error", "Please upload document");
            }

            if (!validImageTypes.Contains(media_documents.FileUrl.ContentType))
            {
                ModelState.AddModelError("Error", "Document must pdf");
            }

            if (ModelState.IsValid)
            {
                byte[] docByte = null;
                BinaryReader rdr = new BinaryReader(media_documents.FileUrl.InputStream);
                docByte = rdr.ReadBytes((int)media_documents.FileUrl.ContentLength);

                var m_i = new media_documents
                {
                    title = media_documents.title,
                    filename = Path.GetFileName(media_documents.FileUrl.FileName),
                    categoryid = media_documents.categoryid,
                    enable = media_documents.enable,
                    filedata = docByte,
                    datecreated = DateTime.Now
                };

                db.media_documents.Add(m_i);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(media_documents);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            media_documents media_documents = (media_documents)db.media_documents.Find(id);
            if (media_documents == null)
            {
                return HttpNotFound();
            }
            return View(media_documents);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(media_documents media_documents)
        {
            string[] validImageTypes = { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

            if (ModelState.IsValid)
            {
                media_documents m_i = db.media_documents.Find(media_documents.imageid);
                m_i.title = media_documents.title;
                m_i.categoryid = media_documents.categoryid;
                m_i.enable = media_documents.enable;
                if (media_documents.FileUrl != null && media_documents.FileUrl.ContentLength > 0)
                {
                    byte[] docByte = null;
                    BinaryReader rdr = new BinaryReader(media_documents.FileUrl.InputStream);
                    docByte = rdr.ReadBytes((int)media_documents.FileUrl.ContentLength);
                    m_i.filename = Path.GetFileName(media_documents.FileUrl.FileName);
                    m_i.filedata = docByte;
                }
                m_i.datecreated = media_documents.datecreated;
                m_i.datemodified = media_documents.datemodified;
                db.Entry(m_i).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(media_documents);
        }

        [AllowAnonymous]
        public FileResult byteArrayToDoc(int docId)
        {
            media_documents media_documents = db.media_documents.Find(docId);
            return new FileStreamResult(new MemoryStream(media_documents.filedata), "application/pdf");
        }

        //public ActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(media_documents media_documents)
        //{
        //    if (media_documents.FileUrl == null && media_documents.FileUrl.ContentLength < 0)
        //    {
        //        ModelState.AddModelError("Error", "Please upload image");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var filename = Path.GetFileName(media_documents.FileUrl.FileName);
        //        var path = Path.Combine(Server.MapPath("~/files/documents"), filename);
        //        media_documents.FileUrl.SaveAs(path);

        //        var m_i = new media_documents 
        //        {
        //            title = media_documents.title,
        //            filename = filename,
        //            categoryid = media_documents.categoryid,
        //            enable = media_documents.enable,
        //            datecreated = DateTime.Now
        //        };

        //        db.media_documents.Add(m_i);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(media_documents);
        //}

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    media_documents media_documents = (media_documents)db.media_documents.Find(id);
        //    if (media_documents == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(media_documents);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(media_documents media_documents)
        //{
        //    if (media_documents.FileUrl == null && media_documents.FileUrl.ContentLength < 0)
        //    {
        //        ModelState.AddModelError("Error", "Please upload image");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var filename = Path.GetFileName(media_documents.FileUrl.FileName);
        //        var path = Path.Combine(Server.MapPath("~/files/documents"), filename);
        //        media_documents.FileUrl.SaveAs(path);

        //        media_documents m_i = new media_documents
        //        {
        //            documentid =  media_documents.documentid,
        //            title = media_documents.title,
        //            filename = filename,
        //            categoryid = media_documents.categoryid,
        //            enable = media_documents.enable,
        //            datecreated = media_documents.datecreated,
        //            datemodified = media_documents.datemodified
        //        };
                
        //        db.Entry(m_i).State = EntityState.Modified;                
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(media_documents);
        //}

        public ActionResult Delete(int? id)
        {
            media_documents media_documents = db.media_documents.Find(id);
            db.media_documents.Remove(media_documents);
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
