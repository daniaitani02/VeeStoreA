using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VeeStoreA.Models;

namespace VeeStoreA.Controllers
{
    public class CouponCodesController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: CouponCodes
        public ActionResult Index()
        {
            return View(db.CouponCodes.ToList());
        }

        // GET: CouponCodes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CouponCode couponCode = db.CouponCodes.Find(id);
            if (couponCode == null)
            {
                return HttpNotFound();
            }
            return View(couponCode);
        }

        // GET: CouponCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CouponCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Code,ExpiryDate,CreatedAt,DiscountPercentage")] CouponCode couponCode)
        {
            if (ModelState.IsValid)
            {
                db.CouponCodes.Add(couponCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(couponCode);
        }

        // GET: CouponCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CouponCode couponCode = db.CouponCodes.Find(id);
            if (couponCode == null)
            {
                return HttpNotFound();
            }
            return View(couponCode);
        }

        // POST: CouponCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,ExpiryDate,CreatedAt,DiscountPercentage")] CouponCode couponCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(couponCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(couponCode);
        }

        // GET: CouponCodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CouponCode couponCode = db.CouponCodes.Find(id);
            if (couponCode == null)
            {
                return HttpNotFound();
            }
            return View(couponCode);
        }

        // POST: CouponCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CouponCode couponCode = db.CouponCodes.Find(id);
            db.CouponCodes.Remove(couponCode);
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
