using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VeeStoreA.Models;

namespace VeeStoreA.Controllers
{
    [Authorize(Users = "admin@admin.com")]
    public class AdminController : Controller

    {

        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Products(string SearchString)
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

        public ActionResult ToggleVisibility(int? id)
        {

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (product.Status == "Visible")
            {
                product.Status = "Invisible";
            } else
            {
                product.Status = "Visible";
            }

            db.SaveChanges();

            return RedirectToAction("Products");
        }

        // GET: Admin/Details/5
        public ActionResult cartDetails(int id)
        {
            var cartItems = from p in db.CartItems
                           select p;

            Cart cart = db.Carts.Find(id);

            if (cart == null)
            {
                return HttpNotFound();
            }

            //cartItems = cartItems.Where(x => x.CartId == id);

            return View(cart);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
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

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
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

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
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
