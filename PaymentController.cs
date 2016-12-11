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
    public class PaymentController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        // GET: /Payment/
        public ActionResult Index()
        {
            var user_payment = db.user_payment.OrderByDescending(x => x.paymentdate);
            return View(user_payment.ToList());
        }

        // GET: /Payment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_payment user_payment = db.user_payment.Find(id);
            if (user_payment == null)
            {
                return HttpNotFound();
            }
            return View(user_payment);
        }

        // GET: /Payment/Create
        public ActionResult Create()
        {
            ViewBag.paymentby = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        // POST: /Payment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="paymentid,paymentby,amount,paymentdate,paymentfor")] user_payment user_payment)
        {
            if (ModelState.IsValid)
            {
                db.user_payment.Add(user_payment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.paymentby = new SelectList(db.UserProfiles, "UserId", "UserName", user_payment.paymentby);
            return View(user_payment);
        }

        // GET: /Payment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_payment user_payment = db.user_payment.Find(id);
            if (user_payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.paymentby = new SelectList(db.UserProfiles, "UserId", "UserName", user_payment.paymentby);
            return View(user_payment);
        }

        // POST: /Payment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="paymentid,paymentby,amount,paymentdate,paymentfor")] user_payment user_payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.paymentby = new SelectList(db.UserProfiles, "UserId", "UserName", user_payment.paymentby);
            return View(user_payment);
        }

        // GET: /Payment/Delete/5
        public ActionResult Delete(int? id)
        {
            user_payment user_payment = db.user_payment.Find(id);
            db.user_payment.Remove(user_payment);
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
