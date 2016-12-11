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

    public class DonationController : Controller
    {
        private mmsv4Entities db = new mmsv4Entities();

        public ActionResult Index()
        {
            return View(db.user_donation.OrderByDescending(x => x.donationdate).ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_donation user_donation = db.user_donation.Find(id);
            if (user_donation == null)
            {
                return HttpNotFound();
            }
            return View(user_donation);
        }

        // GET: /Donation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Donation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="donationid,donationby,amount,donationdate,donationfor")] user_donation user_donation)
        {
            if (ModelState.IsValid)
            {
                db.user_donation.Add(user_donation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user_donation);
        }

        // GET: /Donation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user_donation user_donation = db.user_donation.Find(id);
            if (user_donation == null)
            {
                return HttpNotFound();
            }
            return View(user_donation);
        }

        // POST: /Donation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="donationid,donationby,amount,donationdate,donationfor")] user_donation user_donation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_donation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user_donation);
        }

        // GET: /Donation/Delete/5
        public ActionResult Delete(int? id)
        {
            user_donation user_donation = db.user_donation.Find(id);
            db.user_donation.Remove(user_donation);
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
