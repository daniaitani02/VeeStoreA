using Microsoft.AspNetCore.Http;
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
    public class ProductsController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Products
        public ActionResult Index(string SearchString)
        {
            var products = from p in db.Products
                       select p;
            if (!String.IsNullOrEmpty(SearchString))
            {
                products = products.Where(p => p.Name.Contains(SearchString));

            }
            return View(products);
            //return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Users = "admin@admin.com")]
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.Status = new SelectList(new List<string> { "Visible", "Not Visible" });
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name",null);

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create([Bind(Include = "Id,Name,Price,Description,CategoryId,Status")] Product product)
        {
            var dbProducts = db.Products;
            ViewBag.Status = new SelectList(new List<string> { "Visible", "Not Visible" });
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            
            if (ModelState.IsValid)
            {
                if(dbProducts.Where(p => product.Name == p.Name || product.Id == p.Id).Count() != 0)
                {
                    ViewBag.error = "Product already exists!";
                    return View(product);
                }
                DateTime now = DateTime.Now;
                product.CreatedAt = now;
                product.ImageName = "1.png";
               

                System.Diagnostics.Debug.WriteLine(product.CategoryId);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [Authorize(Users = "admin@admin.com")]
        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.canEdit = true;
           


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if(db.CartItems.Where(item => item.ProductId == product.Id).Count() != 0)
            {
                ViewBag.error = "This item cannot be edited because its already in a cart.";
                ViewBag.canEdit = false;
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name",product.Category);
            ViewBag.Status = new SelectList(new List<string> { "Visible", "Not Visible" },product.Status);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit([Bind(Include = "Id,Name,Price,Description,CategoryId,ImageName,Status,CreatedAt")] Product product)
        {
            ViewBag.canEdit = true;
            if (ModelState.IsValid)
            {
                if (db.CartItems.Where(item => item.ProductId == product.Id).Count() != 0)
                {
                    ViewBag.error = "This item cannot be edited because its already in a cart.";
                    ViewBag.canEdit = false;
                    return View(product);
                }
               
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Delete(int? id)
        {
            ViewBag.canDelete = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if (db.CartItems.Where(item => item.ProductId == product.Id).Count() != 0)
            {
                ViewBag.error = "This item cannot be deleted because its already in a cart.";
                ViewBag.canDelete = false;
                return View(product);
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize(Users = "admin@admin.com")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (db.CartItems.Where(item => item.ProductId == product.Id).Count() != 0)
            {
                ViewBag.error = "This item cannot be deleted because its already in a cart.";
                return View(product);
            }
            db.Products.Remove(product);
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
