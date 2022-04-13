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
        public ActionResult Faq()
        {
            var faqs = from f in db.Faqs
                           select f;
            
            return View(faqs);
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
                if (db.CardCodes.Where(x => x.ProductId == product.Id && x.Status != "Used").Count() == 0)
                {
                    TempData["errorAdminProd"] = "You cannot toggle visibilty of a product that has no available card codes";
                }
                else
                {
                    product.Status = "Visible";
                }
            }

            db.SaveChanges();

            return RedirectToAction("Products");
        }
        public ActionResult ToggleApproval(int? id)
        {

            Faq faq = db.Faqs.Find(id);
            if (faq == null)
            {
                return HttpNotFound();
            }

            if (faq.Status == "Approved")
            {
                faq.Status = "Awaiting approval";
            }
            else
            {
                if (String.IsNullOrEmpty(faq.Answer))
                {
                    TempData["errorAdminFaq"] = "You cannot approve the question without the answer.";
                }
                else
                {
                    faq.Status = "Approved";
                }
            }

            db.SaveChanges();

            return RedirectToAction("Faq");
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

  
       
    }
}
