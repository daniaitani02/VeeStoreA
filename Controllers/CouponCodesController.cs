﻿using System;
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
        public ActionResult Create([Bind(Include = "Id,Code,DiscountPercentage,ExpiryDate")] CouponCode couponCode,string ExpiryDate)
        {
            if (ModelState.IsValid)
            {
                if (db.CouponCodes.Where(x => x.Code == couponCode.Code).Count() != 0)
                {
                    TempData["errorAdminCouponC"] = "You cannot create a coupon code that is already present";
                    return View();
                }

                DateTime now = DateTime.Now;
                couponCode.CreatedAt = now;
                couponCode.ExpiryDate = DateTime.Parse(ExpiryDate);
                //couponCode.ExpiryDate = now.AddDays(30);
                db.CouponCodes.Add(couponCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(couponCode);
        }

        // GET: CouponCodes/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.canEdit = true;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CouponCode couponCode = db.CouponCodes.Find(id);
            if (couponCode == null)
            {
                return HttpNotFound();
            }

            if (db.Carts.Where(c => c.CouponCode.Code == couponCode.Code && c.Status == "Paid").Count() != 0)
            {
                TempData["errorAdminCouponC"] = "You cannot edit or delete this coupon code because it has been used";
                ViewBag.canEdit = false;
                return RedirectToAction("Index");
            }

            return View(couponCode);
        }

        // POST: CouponCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,CreatedAt,DiscountPercentage")] CouponCode couponCode,string ExpiryDate)
        {
            ViewBag.canEdit = true;

            if (ModelState.IsValid)
            {
                if (db.CouponCodes.Where(x => x.Code == couponCode.Code && couponCode.Id != x.Id).Count() != 0)
                {
                    TempData["errorAdminCouponC"] = "You cannot edit a coupon code that is already present in another record";
                    return View();
                }
                couponCode.ExpiryDate = DateTime.Parse(ExpiryDate);
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
            if (couponCode == null || db.Carts.Where(x => x.CouponCode.Code == couponCode.Code).Count() != 0)
            {
                TempData["errorAdminCouponC"] = "You cannot edit or delete this coupon card because it has been used";
                return RedirectToAction("Index");
            }
            db.CouponCodes.Remove(couponCode);
            db.SaveChanges();
            return RedirectToAction("Index");
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
