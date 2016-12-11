using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mmsv4.Models;
using System.Data.Entity;

namespace mmsv4.Controllers
{
    public class RepoController : Controller
    {
        repo<UserProfile> art;
        mmsv4Entities db = new mmsv4Entities();

        public RepoController()
        {
            this.art = new repo<UserProfile>(db);
        }

        // GET: Repo
        public ActionResult Index()
        {
            return Json(art.GetAll().ToList(),  JsonRequestBehavior.AllowGet);
        }

        // GET: Repo/Details/5
        public ActionResult Details(int id)
        {
            return Json(art.Get(id), JsonRequestBehavior.AllowGet);
        }

        // GET: Repo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Repo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Repo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Repo/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Repo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Repo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
