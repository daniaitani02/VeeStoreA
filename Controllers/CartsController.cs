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
    [Authorize]
    public class CartsController : Controller
    {
        private VeeStoreDbEntities db = new VeeStoreDbEntities();

        // GET: Carts
        public ActionResult Index()
        {
            var carts = from c in db.Carts
                        select c;

            if (User.Identity.Name != "admin@admin.com")
            {
                //Only return carts of current logged in cutsomer
                carts = db.Carts.Where(c => c.CustomerEmail == User.Identity.Name);
            }
           
           
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
            if (cart.Customer.Email != User.Identity.Name && User.Identity.Name != "admin@admin.com") return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if(cart.Status == "Paid") { return RedirectToAction("Receipt","Transaction",new {id=cart.Id }); }
            //ViewBag.totalAmount = cart.CartItems.Sum(p => (int)p.Quantity * (int)p.Product.Price);
            ViewBag.Type = new SelectList(new List<string> { "Visa", "Mastercard","American Express" },"Visa");
            return View(cart);
        }

     
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
            if (cart.Customer.Email != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            if (cart.CartItems.Count() != 0)
            {
                TempData["error"] = "Cart Is Not Empty";
                return RedirectToAction("Index");

            }

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
            if (cart.Customer.Email != User.Identity.Name) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

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
