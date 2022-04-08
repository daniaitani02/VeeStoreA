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
    [Authorize]
    public class CustomersController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Customers
        [Authorize (Users ="admin@admin.com")]
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            if (User.Identity.Name == "admin@admin.com") return View(customer);

            // Does the customer object belong to the logged in user? If not, return Forbidden error
            if (customer.Email != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(customer);
        }

        

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "ShortName",null);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            // Does the customer object belong to the logged in user? If not, return Forbidden error
            if (customer.Email != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Email,Name,CurrencyId,Gender,JoinedAt,Status")] Customer customer)
        {
            // Does the customer object belong to the logged in user? If not, return Forbidden error
          
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        public ActionResult AddCreditCard([Bind(Include = "Name,Number,CVV,Expiry")] CreditCard creditCard)
        {
            creditCard.CustomerEmail = User.Identity.Name;

            return RedirectToAction("Details");
        }
        // GET: Customers/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Customer customer = db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Customer customer = db.Customers.Find(id);
        //    db.Customers.Remove(customer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
