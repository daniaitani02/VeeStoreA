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
    public class CartsController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Carts
        public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.Customer);
            return View(carts.ToList());
        }

        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            // Is id null (not given)? if yes, return BadRequest error

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            // Does the cart belong to the logged in user? If not, return Forbidden error
            if (cart.Customer.UserName != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(cart);
        }

        // GET: Carts/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
        //    return View();
        //}

        //// POST: Carts/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,CustomerName,Status")] Cart cart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Carts.Add(cart);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
        //    return View(cart);
        //}

        //// GET: Carts/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Cart cart = db.Carts.Find(id);
        //    if (cart == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
        //    return View(cart);
        //}

        //// POST: Carts/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,CustomerName,Status")] Cart cart)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(cart).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
        //    return View(cart);
        //}

        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            // Is id null (not given)? if yes, return BadRequest error
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Is there a cart with the given id? if not, return NotFound error
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            // Does the cart belong to the logged in user? If not, return Forbidden error
            if (cart.Customer.UserName != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);


            return View(cart);
        }

        //POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            // Is id null (not given)? if yes, return BadRequest error
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Is there a cart with the given id? if not, return NotFound error
            Cart cart = db.Carts.Find(id);
            if (cart == null) return HttpNotFound();

            // Does the cart belong to the logged in user? If not, return Forbidden error
            if (cart.Customer.UserName != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            // Is cart empty? if not, store an error in TempData and dont delete cart.
            if (cart.CartItems.Count() != 0) TempData["error"] = "Cart Is Not Empty";
            else
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
            }


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
